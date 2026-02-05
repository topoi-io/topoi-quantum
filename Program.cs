using QuantumComputer;
using System.Globalization;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("Single-Qubit Gate Interpreter");
Console.WriteLine("Type HELP for commands. Angles are in radians.\n");

Quantum.Reset();
Quantum.PrintState();
Console.WriteLine();

while (true)
{
    Console.Write("> ");
    string? line = Console.ReadLine();
    if (line is null) break;

    line = line.Trim();
    if (line.Length == 0) continue;

    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    var cmd = parts[0].ToUpperInvariant();

    try
    {
        switch (cmd)
        {
            case "EXPECT":
                Quantum.Expect();
                break;

            case "HELP":
                PrintHelp();
                break;

            case "PRINT":
                Quantum.PrintState();
                break;

            case "RESET":
                Quantum.Reset();
                Console.WriteLine("Reset to |0⟩.");
                break;

            case "MEASURE":
                int outcome = Quantum.Measure();
                Console.WriteLine($"Measured: {outcome} (state collapsed to |{outcome}⟩)");
                break;

            case "X": Quantum.X(); break;
            case "Y": Quantum.Y(); break;
            case "Z": Quantum.Z(); break;
            case "H": Quantum.H(); break;
            case "S": Quantum.S(); break;
            case "T": Quantum.T(); break;

            case "RX":
                Quantum.RX(ParseAngle(parts, 1));
                break;

            case "RY":
                Quantum.RY(ParseAngle(parts, 1));
                break;

            case "RZ":
                Quantum.RZ(ParseAngle(parts, 1));
                break;

            case "SAMPLE":
                {
                    if (parts.Length < 2 || !int.TryParse(parts[1], out int n) || n <= 0)
                        throw new ArgumentException("Usage: SAMPLE <n>   (e.g. SAMPLE 1000)");

                    // Assume user has already prepared the state (e.g. applied H)
                    // We'll repeatedly measure and then restore the pre-measurement state each time.
                    // So: copy state, measure, restore, repeat.
                    var a0 = Quantum.Alpha;
                    var b0 = Quantum.Beta;

                    int c0 = 0, c1 = 0;
                    for (int i = 0; i < n; i++)
                    {
                        outcome = Quantum.Measure();
                        if (outcome == 0) c0++; else c1++;

                        // restore original state for the next trial
                        Quantum.SetState(a0, b0);
                    }

                    Console.WriteLine($"Samples: {n}");
                    Console.WriteLine($"0: {c0} ({(double)c0 / n:P2})");
                    Console.WriteLine($"1: {c1} ({(double)c1 / n:P2})");
                    break;
                }

            case "QUIT":
            case "EXIT":
                return;

            default:
                Console.WriteLine($"Unknown command: {cmd}. Type HELP.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

static double ParseAngle(string[] parts, int index)
{
    if (parts.Length <= index)
        throw new ArgumentException("Missing angle. Example: RX 1.57079632679");

    // Accept "pi" expressions lightly: e.g. "pi", "pi/2", "2*pi", "3*pi/4"
    // If it doesn't match, fall back to normal double parsing.
    string token = parts[index].ToLowerInvariant();

    if (TryParsePiExpression(token, out double theta))
        return theta;

    if (!double.TryParse(parts[index], NumberStyles.Float, CultureInfo.InvariantCulture, out theta))
        throw new ArgumentException("Angle must be a number (use invariant culture, e.g. 1.234) or a pi expression like pi/2.");

    return theta;
}

static bool TryParsePiExpression(string s, out double value)
{
    // Simple patterns:
    //   pi
    //   pi/2
    //   2*pi
    //   3*pi/4
    //   -pi, -pi/2, etc.
    value = 0;

    s = s.Replace(" ", "");
    if (!s.Contains("pi")) return false;

    // Normalize: replace "pi" with "*pi" when preceded by digit (e.g. "2pi" -> "2*pi")
    s = System.Text.RegularExpressions.Regex.Replace(s, @"(\d)pi", "$1*pi");

    // Handle leading "pi" as "1*pi"
    if (s.StartsWith("pi", StringComparison.Ordinal))
        s = "1*" + s;

    // Now attempt to parse forms: a*pi or a*pi/b
    // Split by '/'
    string[] frac = s.Split('/', StringSplitOptions.RemoveEmptyEntries);
    if (frac.Length > 2) return false;

    double numerator = ParseAPi(frac[0]);
    double denom = 1.0;

    if (frac.Length == 2)
    {
        if (!double.TryParse(frac[1], NumberStyles.Float, CultureInfo.InvariantCulture, out denom))
            return false;
    }

    value = numerator / denom;
    return true;

    static double ParseAPi(string left)
    {
        // left expected: something like "3*pi" or "-1*pi" or "0.5*pi"
        string[] mul = left.Split('*', StringSplitOptions.RemoveEmptyEntries);
        if (mul.Length == 1 && mul[0] == "pi") return Math.PI;
        if (mul.Length == 2 && mul[1] == "pi")
        {
            if (!double.TryParse(mul[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double a))
                throw new ArgumentException("Bad pi expression.");
            return a * Math.PI;
        }
        if (mul.Length == 2 && mul[0] == "pi")
        {
            if (!double.TryParse(mul[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double a))
                throw new ArgumentException("Bad pi expression.");
            return a * Math.PI;
        }
        throw new ArgumentException("Bad pi expression.");
    }
}

static void PrintHelp()
{
    Console.WriteLine("Commands:");
    Console.WriteLine("  EXPECT                    Show theoretical probabilities (no measurement)");
    Console.WriteLine("  X, Y, Z, H, S, T          Apply standard single-qubit gates");
    Console.WriteLine("  RX <theta>                Rotation about X (theta in radians, e.g. RX pi/2)");
    Console.WriteLine("  RY <theta>                Rotation about Y");
    Console.WriteLine("  RZ <theta>                Rotation about Z");
    Console.WriteLine("  PRINT                     Show current state and probabilities");
    Console.WriteLine("  MEASURE                   Measure in computational basis (collapses state)");
    Console.WriteLine("  RESET                     Reset to |0>");
    Console.WriteLine("  QUIT                      Exit");
    Console.WriteLine();
    Console.WriteLine("Angle formats:");
    Console.WriteLine("  1.57079632679   pi   pi/2   3*pi/4   -pi/8");
}
