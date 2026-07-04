namespace Topoi.Quantum.Parsing;

public static class GateOperationParser
{
    public static bool TryParse(
        string line,
        out GateOperation? operation,
        out string? error)
    {
        operation = null;
        error = null;

        line = StripComment(line).Trim();

        if (line.Length == 0)
        {
            error = "Empty command.";
            return false;
        }

        string[] parts = line.Split(
            ' ',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return TryParse(parts, out operation, out error);
    }

    public static bool TryParse(
        string[] parts,
        out GateOperation? operation,
        out string? error)
    {
        operation = null;
        error = null;

        if (parts.Length == 0)
        {
            error = "Empty command.";
            return false;
        }

        string cmd = parts[0].ToUpperInvariant();

        try
        {
            switch (cmd)
            {
                case "X":
                    operation = new GateOperation(GateKind.X, new[] { ParseQubit(parts, 1) });
                    return true;

                case "Y":
                    operation = new GateOperation(GateKind.Y, new[] { ParseQubit(parts, 1) });
                    return true;

                case "Z":
                    operation = new GateOperation(GateKind.Z, new[] { ParseQubit(parts, 1) });
                    return true;

                case "H":
                    operation = new GateOperation(GateKind.H, new[] { ParseQubit(parts, 1) });
                    return true;

                case "S":
                    operation = new GateOperation(GateKind.S, new[] { ParseQubit(parts, 1) });
                    return true;

                case "T":
                    operation = new GateOperation(GateKind.T, new[] { ParseQubit(parts, 1) });
                    return true;

                case "I":
                    operation = new GateOperation(GateKind.I, new[] { ParseQubit(parts, 1) });
                    return true;

                case "SDG":
                    operation = new GateOperation(GateKind.SDG, new[] { ParseQubit(parts, 1) });
                    return true;

                case "TDG":
                    operation = new GateOperation(GateKind.TDG, new[] { ParseQubit(parts, 1) });
                    return true;

                case "SX":
                    operation = new GateOperation(GateKind.SX, new[] { ParseQubit(parts, 1) });
                    return true;

                case "SXDG":
                    operation = new GateOperation(GateKind.SXDG, new[] { ParseQubit(parts, 1) });
                    return true;

                case "RX":
                    operation = new GateOperation(
                        GateKind.RX,
                        new[] { ParseQubit(parts, 1) },
                        ParseAngle(parts, 2));
                    return true;

                case "RY":
                    operation = new GateOperation(
                        GateKind.RY,
                        new[] { ParseQubit(parts, 1) },
                        ParseAngle(parts, 2));
                    return true;

                case "RZ":
                    operation = new GateOperation(
                        GateKind.RZ,
                        new[] { ParseQubit(parts, 1) },
                        ParseAngle(parts, 2));
                    return true;

                case "CX":
                case "CNOT":
                    operation = new GateOperation(
                        GateKind.CX,
                        new[] { ParseQubit(parts, 1), ParseQubit(parts, 2) });
                    return true;

                case "CZ":
                    operation = new GateOperation(
                        GateKind.CZ,
                        new[] { ParseQubit(parts, 1), ParseQubit(parts, 2) });
                    return true;

                case "CY":
                    operation = new GateOperation(
                        GateKind.CY,
                        new[] { ParseQubit(parts, 1), ParseQubit(parts, 2) });
                    return true;

                case "CH":
                    operation = new GateOperation(
                        GateKind.CH,
                        new[] { ParseQubit(parts, 1), ParseQubit(parts, 2) });
                    return true;

                case "SWAP":
                    operation = new GateOperation(
                        GateKind.SWAP,
                        new[] { ParseQubit(parts, 1), ParseQubit(parts, 2) });
                    return true;

                case "CCX":
                case "TOFFOLI":
                    operation = new GateOperation(
                        GateKind.CCX,
                        new[]
                        {
                            ParseQubit(parts, 1),
                            ParseQubit(parts, 2),
                            ParseQubit(parts, 3)
                        });
                    return true;

                case "CRX":
                    operation = new GateOperation(
                        GateKind.CRX,
                        new[] { ParseQubit(parts, 1), ParseQubit(parts, 2) },
                        ParseAngle(parts, 3));
                    return true;

                case "CRY":
                    operation = new GateOperation(
                        GateKind.CRY,
                        new[] { ParseQubit(parts, 1), ParseQubit(parts, 2) },
                        ParseAngle(parts, 3));
                    return true;

                case "CRZ":
                    operation = new GateOperation(
                        GateKind.CRZ,
                        new[] { ParseQubit(parts, 1), ParseQubit(parts, 2) },
                        ParseAngle(parts, 3));
                    return true;

                case "CP":
                    operation = new GateOperation(
                        GateKind.CP,
                        new[] { ParseQubit(parts, 1), ParseQubit(parts, 2) },
                        ParseAngle(parts, 3));
                    return true;

                default:
                    error =
                        $"Only unitary gate commands can be loaded into a QuantumCircuit. " +
                        $"'{cmd}' is an interpreter command, not a circuit gate.";
                    return false;
            }
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }

    private static int ParseQubit(string[] parts, int index)
    {
        if (parts.Length <= index)
            throw new ArgumentException("Missing qubit index. Example: H 0");

        if (!int.TryParse(parts[index], out int q) || q < 0)
            throw new ArgumentException("Qubit index must be a non-negative integer.");

        return q;
    }

    private static double ParseAngle(string[] parts, int index)
    {
        if (parts.Length <= index)
            throw new ArgumentException("Missing angle. Example: RX 0 pi/2");

        return AngleParser.Parse(parts[index]);
    }

    private static string StripComment(string line)
    {
        int hash = line.IndexOf('#');

        if (hash < 0)
            return line;

        return line[..hash];
    }
}