using QuantumComputer.Core;

namespace QuantumComputer.OpenQasm;

public static class OpenQasmExecutableConverter
{
    public static OpenQasmExecutableProgram Convert(OpenQasmProgram program)
    {
        var qubitRegisters = new Dictionary<string, (int Offset, int Size)>();
        var bitRegisters = new Dictionary<string, (int Offset, int Size)>();

        int totalQubits = 0;
        int totalBits = 0;

        foreach (OpenQasmStatement statement in program.Statements)
        {
            if (statement is OpenQasmQubitDeclaration q)
            {
                if (qubitRegisters.ContainsKey(q.Name))
                    throw new OpenQasmParseException($"Duplicate qubit register '{q.Name}'.", 1, 1);

                qubitRegisters[q.Name] = (totalQubits, q.Size);
                totalQubits += q.Size;
            }
            else if (statement is OpenQasmBitDeclaration b)
            {
                if (bitRegisters.ContainsKey(b.Name))
                    throw new OpenQasmParseException($"Duplicate bit register '{b.Name}'.", 1, 1);

                bitRegisters[b.Name] = (totalBits, b.Size);
                totalBits += b.Size;
            }
        }

        var operations = new List<OpenQasmExecutableOperation>();

        foreach (OpenQasmStatement statement in program.Statements)
        {
            switch (statement)
            {
                case OpenQasmGateCallStatement gate:
                    operations.Add(new OpenQasmGateOperation(
                        ConvertGateCall(gate, qubitRegisters)));
                    break;

                case OpenQasmMeasureStatement measure:
                    operations.Add(new OpenQasmMeasureOperation(
                        ResolveQubit(measure.Qubit, qubitRegisters),
                        ResolveBit(measure.Bit, bitRegisters)));
                    break;

                case OpenQasmResetStatement reset:
                    operations.Add(new OpenQasmResetOperation(
                        ResolveQubit(reset.Qubit, qubitRegisters)));
                    break;

                case OpenQasmBarrierStatement barrier:
                    operations.Add(new OpenQasmBarrierOperation(
                        barrier.Qubits.Select(q => ResolveQubit(q, qubitRegisters)).ToArray()));
                    break;
            }
        }

        return new OpenQasmExecutableProgram(
            totalQubits,
            qubitRegisters,
            bitRegisters,
            operations);
    }

    private static GateOperation ConvertGateCall(OpenQasmGateCallStatement gateCall, IReadOnlyDictionary<string, (int Offset, int Size)> registers)
    {
        int[] qubits = gateCall.Qubits.Select(q => ResolveGateQubitOperand(q, registers)).ToArray();

        return OpenQasmGateMapper.Map(gateCall, qubits);
    }

    private static int ResolveQubit(OpenQasmQubitReference qubit, IReadOnlyDictionary<string, (int Offset, int Size)> registers)
    {
        if (!registers.TryGetValue(qubit.RegisterName, out var register))
            throw new OpenQasmParseException($"Unknown qubit register '{qubit.RegisterName}'.", 1, 1);

        if ((uint)qubit.Index >= (uint)register.Size)
            throw new OpenQasmParseException(
                $"Qubit index {qubit.Index} is out of range for register '{qubit.RegisterName}' of size {register.Size}.",
                1,
                1);

        return register.Offset + qubit.Index;
    }

    private static int ResolveBit(OpenQasmBitReference bit, IReadOnlyDictionary<string, (int Offset, int Size)> registers)
    {
        if (!registers.TryGetValue(bit.RegisterName, out var register))
            throw new OpenQasmParseException($"Unknown bit register '{bit.RegisterName}'.", 1, 1);

        if ((uint)bit.Index >= (uint)register.Size)
            throw new OpenQasmParseException(
                $"Bit index {bit.Index} is out of range for register '{bit.RegisterName}' of size {register.Size}.",
                1,
                1);

        return register.Offset + bit.Index;
    }

    private static int ResolveGateQubitOperand(OpenQasmQubitOperand qubit, IReadOnlyDictionary<string, (int Offset, int Size)> registers)
    {
        if (qubit.Index is null)
        {
            throw new OpenQasmParseException(
                $"Gate call qubit '{qubit.Name}' must be an indexed qubit outside a gate definition.",
                1,
                1);
        }

        if (!registers.TryGetValue(qubit.Name, out var register))
        {
            throw new OpenQasmParseException(
                $"Unknown qubit register '{qubit.Name}'.",
                1,
                1);
        }

        if ((uint)qubit.Index.Value >= (uint)register.Size)
        {
            throw new OpenQasmParseException(
                $"Qubit index {qubit.Index.Value} is out of range for register '{qubit.Name}' of size {register.Size}.",
                1,
                1);
        }

        return register.Offset + qubit.Index.Value;
    }
}