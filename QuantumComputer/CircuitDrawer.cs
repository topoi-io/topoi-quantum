using System.Text;

namespace QuantumComputer;

public static class CircuitDrawer
{
    private const string Wire = "───";
    private const string Vertical = " │ ";
    private const string Control = "─●─";
    private const string TargetX = "─X─";
    private const string Swap = "─×─";

    public static string Draw(QuantumCircuit circuit)
    {
        if (circuit.QubitCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(circuit));

        int labelWidth = Math.Max(2, circuit.QubitCount.ToString().Length + 1);

        // We use 2n - 1 rows:
        // q0 row
        // connector row between q0 and q1
        // q1 row
        // connector row between q1 and q2
        // q2 row
        int rowCount = (circuit.QubitCount * 2) - 1;
        var rows = new string[rowCount];

        for (int q = 0; q < circuit.QubitCount; q++)
        {
            int row = QubitRow(q);
            rows[row] = $"q{q.ToString().PadRight(labelWidth - 1)}: ";
        }

        string blankPrefix = new string(' ', labelWidth + 3);

        for (int row = 0; row < rowCount; row++)
        {
            if (rows[row] is null)
                rows[row] = blankPrefix;
        }

        foreach (GateOperation operation in circuit.Operations)
        {
            string[] column = DrawOperationColumn(circuit.QubitCount, operation);

            for (int row = 0; row < rowCount; row++)
                rows[row] += column[row];
        }

        var sb = new StringBuilder();

        foreach (string row in rows)
            sb.AppendLine(row.TrimEnd());

        return sb.ToString();
    }

    private static string[] DrawOperationColumn(int qubitCount, GateOperation operation)
    {
        int rowCount = (qubitCount * 2) - 1;
        string[] column = Enumerable.Repeat("   ", rowCount).ToArray();

        // Qubit rows get wires by default.
        for (int q = 0; q < qubitCount; q++)
            column[QubitRow(q)] = Wire;

        switch (operation.Kind)
        {
            case GateKind.X:
            case GateKind.Y:
            case GateKind.Z:
            case GateKind.H:
            case GateKind.S:
            case GateKind.T:
                DrawSingleQubitGate(column, operation.Qubits[0], operation.Kind.ToString());
                break;

            case GateKind.RX:
            case GateKind.RY:
            case GateKind.RZ:
                DrawSingleQubitGate(column, operation.Qubits[0], operation.Kind.ToString());
                break;

            case GateKind.CX:
                DrawConnectedGate(column, operation.Qubits[0], operation.Qubits[1], Control, TargetX);
                break;

            case GateKind.CZ:
                DrawConnectedGate(column, operation.Qubits[0], operation.Qubits[1], Control, "─Z─");
                break;

            case GateKind.SWAP:
                DrawConnectedGate(column, operation.Qubits[0], operation.Qubits[1], Swap, Swap);
                break;

            case GateKind.CCX:
                DrawToffoli(column, operation.Qubits[0], operation.Qubits[1], operation.Qubits[2]);
                break;

            case GateKind.CRX:
                DrawConnectedGate(column, operation.Qubits[0], operation.Qubits[1], Control, "RX ");
                break;

            case GateKind.CRY:
                DrawConnectedGate(column, operation.Qubits[0], operation.Qubits[1], Control, "RY ");
                break;

            case GateKind.CRZ:
                DrawConnectedGate(column, operation.Qubits[0], operation.Qubits[1], Control, "RZ ");
                break;

            default:
                throw new NotSupportedException($"Unsupported gate kind: {operation.Kind}");
        }

        return column;
    }

    private static void DrawSingleQubitGate(string[] column, int q, string label)
    {
        column[QubitRow(q)] = FormatGateLabel(label);
    }

    private static void DrawConnectedGate(
        string[] column,
        int q1,
        int q2,
        string symbol1,
        string symbol2)
    {
        int row1 = QubitRow(q1);
        int row2 = QubitRow(q2);

        int min = Math.Min(row1, row2);
        int max = Math.Max(row1, row2);

        for (int row = min + 1; row < max; row++)
            column[row] = Vertical;

        column[row1] = NormalizeCell(symbol1);
        column[row2] = NormalizeCell(symbol2);
    }

    private static void DrawToffoli(string[] column, int control1, int control2, int target)
    {
        int rowControl1 = QubitRow(control1);
        int rowControl2 = QubitRow(control2);
        int rowTarget = QubitRow(target);

        int min = Math.Min(Math.Min(rowControl1, rowControl2), rowTarget);
        int max = Math.Max(Math.Max(rowControl1, rowControl2), rowTarget);

        for (int row = min + 1; row < max; row++)
            column[row] = Vertical;

        column[rowControl1] = Control;
        column[rowControl2] = Control;
        column[rowTarget] = TargetX;
    }

    private static int QubitRow(int q)
    {
        return q * 2;
    }

    private static string FormatGateLabel(string label)
    {
        label = label.ToUpperInvariant();

        return label.Length switch
        {
            1 => $"─{label}─",
            2 => label + " ",
            _ => label[..3]
        };
    }

    private static string NormalizeCell(string cell)
    {
        if (cell.Length == 3)
            return cell;

        if (cell.Length > 3)
            return cell[..3];

        return cell.PadRight(3);
    }
}