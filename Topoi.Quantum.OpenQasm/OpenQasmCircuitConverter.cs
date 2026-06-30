namespace Topoi.Quantum.OpenQasm;

public static class OpenQasmCircuitConverter
{
    public static QuantumCircuit Convert(OpenQasmProgram program)
    {
        if (program is null)
            throw new ArgumentNullException(nameof(program));

        var qubitRegisters = new Dictionary<string, (int Offset, int Size)>();
        int totalQubits = 0;

        foreach (OpenQasmQubitDeclaration declaration in program.Statements.OfType<OpenQasmQubitDeclaration>())
        {
            if (qubitRegisters.ContainsKey(declaration.Name))
            {
                throw new OpenQasmParseException(
                    $"Duplicate qubit register '{declaration.Name}'.",
                    declaration.Line,
                    declaration.Column);
            }

            qubitRegisters[declaration.Name] = (totalQubits, declaration.Size);
            totalQubits += declaration.Size;
        }

        if (totalQubits <= 0)
        {
            throw new OpenQasmParseException(
                "OpenQASM program does not declare any qubits.",
                1,
                1);
        }

        var circuit = new QuantumCircuit(totalQubits);

        foreach (OpenQasmGateCallStatement gateCall in program.Statements.OfType<OpenQasmGateCallStatement>())
        {
            GateOperation operation = ConvertGateCall(gateCall, qubitRegisters);
            circuit.Add(operation);
        }

        return circuit;
    }

    private static GateOperation ConvertGateCall(
        OpenQasmGateCallStatement gateCall,
        IReadOnlyDictionary<string, (int Offset, int Size)> registers)
    {
        int[] qubits = gateCall.Qubits
            .Select(q => ResolveGateQubitOperand(q, registers, gateCall))
            .ToArray();

        return OpenQasmGateMapper.Map(gateCall, qubits);
    }

    private static int ResolveGateQubitOperand(
        OpenQasmQubitOperand qubit,
        IReadOnlyDictionary<string, (int Offset, int Size)> registers,
        OpenQasmGateCallStatement gateCall)
    {
        if (qubit.Index is null)
        {
            throw new OpenQasmParseException(
                $"Gate call qubit '{qubit.Name}' must be an indexed qubit outside a gate definition.",
                gateCall.Line,
                gateCall.Column);
        }

        if (!registers.TryGetValue(qubit.Name, out var register))
        {
            throw new OpenQasmParseException(
                $"Unknown qubit register '{qubit.Name}'.",
                gateCall.Line,
                gateCall.Column);
        }

        if ((uint)qubit.Index.Value >= (uint)register.Size)
        {
            throw new OpenQasmParseException(
                $"Qubit index {qubit.Index.Value} is out of range for register '{qubit.Name}' of size {register.Size}.",
                gateCall.Line,
                gateCall.Column);
        }

        return register.Offset + qubit.Index.Value;
    }
}