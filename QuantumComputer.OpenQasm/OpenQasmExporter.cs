using System.Globalization;
using System.Text;
using QuantumComputer.Core;

namespace QuantumComputer.OpenQasm;

public static class OpenQasmExporter
{
    public static string Export(QuantumCircuit circuit)
    {
        if (circuit is null)
            throw new ArgumentNullException(nameof(circuit));

        var sb = new StringBuilder();

        sb.AppendLine("OPENQASM 3.1;");
        sb.AppendLine("include \"stdgates.inc\";");
        sb.AppendLine();
        sb.AppendLine($"qubit[{circuit.QubitCount}] q;");
        sb.AppendLine();

        foreach (GateOperation operation in circuit.Operations)
        {
            sb.AppendLine(FormatOperation(operation));
        }

        return sb.ToString();
    }

    private static string FormatOperation(GateOperation operation)
    {
        return operation.Kind switch
        {
            GateKind.I => $"id q[{operation.Qubits[0]}];",

            GateKind.X => $"x q[{operation.Qubits[0]}];",
            GateKind.Y => $"y q[{operation.Qubits[0]}];",
            GateKind.Z => $"z q[{operation.Qubits[0]}];",
            GateKind.H => $"h q[{operation.Qubits[0]}];",
            GateKind.S => $"s q[{operation.Qubits[0]}];",
            GateKind.T => $"t q[{operation.Qubits[0]}];",
            GateKind.SDG => $"sdg q[{operation.Qubits[0]}];",
            GateKind.TDG => $"tdg q[{operation.Qubits[0]}];",
            GateKind.SX => $"sx q[{operation.Qubits[0]}];",
            GateKind.SXDG => $"sxdg q[{operation.Qubits[0]}];",

            GateKind.RX => $"rx({FormatAngle(operation)}) q[{operation.Qubits[0]}];",
            GateKind.RY => $"ry({FormatAngle(operation)}) q[{operation.Qubits[0]}];",
            GateKind.RZ => $"rz({FormatAngle(operation)}) q[{operation.Qubits[0]}];",

            GateKind.CX => $"cx q[{operation.Qubits[0]}], q[{operation.Qubits[1]}];",
            GateKind.CY => $"cy q[{operation.Qubits[0]}], q[{operation.Qubits[1]}];",
            GateKind.CZ => $"cz q[{operation.Qubits[0]}], q[{operation.Qubits[1]}];",
            GateKind.CH => $"ch q[{operation.Qubits[0]}], q[{operation.Qubits[1]}];",
            GateKind.SWAP => $"swap q[{operation.Qubits[0]}], q[{operation.Qubits[1]}];",
            GateKind.CCX => $"ccx q[{operation.Qubits[0]}], q[{operation.Qubits[1]}], q[{operation.Qubits[2]}];",

            GateKind.CRX => $"crx({FormatAngle(operation)}) q[{operation.Qubits[0]}], q[{operation.Qubits[1]}];",
            GateKind.CRY => $"cry({FormatAngle(operation)}) q[{operation.Qubits[0]}], q[{operation.Qubits[1]}];",
            GateKind.CRZ => $"crz({FormatAngle(operation)}) q[{operation.Qubits[0]}], q[{operation.Qubits[1]}];",
            GateKind.CP => $"cp({FormatAngle(operation)}) q[{operation.Qubits[0]}], q[{operation.Qubits[1]}];",

            _ => throw new NotSupportedException($"Cannot export gate {operation.Kind} to OpenQASM.")
        };
    }

    private static string FormatAngle(GateOperation operation)
    {
        if (operation.Angle is null)
            throw new InvalidOperationException($"Gate {operation.Kind} requires an angle.");

        return operation.Angle.Value.ToString("R", CultureInfo.InvariantCulture);
    }
}