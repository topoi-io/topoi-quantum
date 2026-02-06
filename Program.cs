using QuantumComputer;
using System.Globalization;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("N-Qubit Gate Interpreter (state-vector)");
Console.Write("Number of qubits n (e.g. 1,2,3): ");
int n = 1;
{
    var s = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(s) && int.TryParse(s, out var parsed) && parsed > 0)
        n = parsed;
}

Quantum.Init(n);
Quantum.Reset();
Quantum.PrintStateTop();
Console.WriteLine();
Console.WriteLine("Type HELP for commands. Angles are in radians.\n");

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
            case "HELP":
                PrintHelp();
                break;

            case "PRINT":
                Quantum.PrintStateTop();
                break;

            case "EXPECT":
                Quantum.ExpectTop();
                break;

            case "RESET":
                Quantum.Reset();
                Console.WriteLine("Reset to |00..0⟩.");
                break;

            case "MEASUREALL":
                {
                    int outcome = Quantum.MeasureAll();
                    Console.WriteLine($"Measured: |{Quantum.Register.BitString(outcome)}⟩ (state collapsed)");
                    break;
                }

            case "MEASURE":
                {
                    if (parts.Length == 1)
                        throw new ArgumentException("Usage: MEASURE <q>   (e.g. MEASURE 0). Use MEASUREALL to measure full register.");

                    int q = ParseQubit(parts, 1);
                    int bit = Quantum.Measure(q);
                    Console.WriteLine($"Measured qubit {q}: {bit} (partial collapse)");
                    break;
                }

            // 1-qubit gates now require target: e.g. H 0
            case "X": Quantum.X(ParseQubit(parts, 1)); break;
            case "Y": Quantum.Y(ParseQubit(parts, 1)); break;
            case "Z": Quantum.Z(ParseQubit(parts, 1)); break;
            case "H": Quantum.H(ParseQubit(parts, 1)); break;
            case "S": Quantum.S(ParseQubit(parts, 1)); break;
            case "T": Quantum.T(ParseQubit(parts, 1)); break;

            case "RX":
                Quantum.RX(ParseQubit(parts, 1), ParseAngle(parts, 2));
                break;

            case "RY":
                Quantum.RY(ParseQubit(parts, 1), ParseAngle(parts, 2));
                break;

            case "RZ":
                Quantum.RZ(ParseQubit(parts, 1), ParseAngle(parts, 2));
                break;

            case "QRAND":
                {
                    n = Quantum.Register.QubitCount;
                    int k = n;

                    // QRAND <k> optional
                    if (parts.Length >= 2)
                    {
                        if (!int.TryParse(parts[1], out k) || k <= 0)
                            throw new ArgumentException("Usage: QRAND [k]  where k is a positive integer (<= number of qubits).");

                        if (k > n)
                            throw new ArgumentException($"QRAND {k} requested, but register has only {n} qubits. Start with n >= {k}.");
                    }

                    // Prepare uniform superposition on ALL n qubits (not just k),
                    // because MEASUREALL measures the full register.
                    // (We then slice out k bits if requested.)
                    Quantum.Reset();
                    ApplyHadamardAll(n);

                    int outcome = Quantum.MeasureAll();
                    string bits = Quantum.Register.BitString(outcome);

                    // Return k random bits (lowest k, i.e. qubits 0..k-1)
                    string kBits = TakeLowBits(bits, k);

                    // Also print integer value of those k bits (optional but handy)
                    int value = Convert.ToInt32(kBits, 2);

                    Console.WriteLine(kBits);
                    Console.WriteLine($"(int: {value})");
                    break;
                }

            case "SAMPLE":
                {
                    if (parts.Length < 2 || !int.TryParse(parts[1], out int trials) || trials <= 0)
                        throw new ArgumentException("Usage: SAMPLE <n>   (e.g. SAMPLE 1000)");

                    var snap = Quantum.SnapshotState();

                    var counts = new Dictionary<int, int>();
                    for (int i = 0; i < trials; i++)
                    {
                        int outcome = Quantum.MeasureAll();
                        counts[outcome] = counts.TryGetValue(outcome, out var c) ? c + 1 : 1;
                        Quantum.RestoreState(snap);
                    }

                    Console.WriteLine($"Samples: {trials}");
                    foreach (var kv in counts.OrderByDescending(kv => kv.Value).Take(16))
                    {
                        Console.WriteLine($"|{Quantum.Register.BitString(kv.Key)}⟩ : {kv.Value} ({(double)kv.Value / trials:P2})");
                    }
                    if (counts.Count > 16) Console.WriteLine("... (showing top 16 outcomes)");
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

static int ParseQubit(string[] parts, int index)
{
    if (parts.Length <= index)
        throw new ArgumentException("Missing qubit index. Example: H 0");

    if (!int.TryParse(parts[index], out int q) || q < 0)
        throw new ArgumentException("Qubit index must be a non-negative integer.");

    return q;
}

static double ParseAngle(string[] parts, int index)
{
    if (parts.Length <= index)
        throw new ArgumentException("Missing angle. Example: RX 0 pi/2");

    string token = parts[index].ToLowerInvariant();

    if (TryParsePiExpression(token, out double theta))
        return theta;

    if (!double.TryParse(parts[index], NumberStyles.Float, CultureInfo.InvariantCulture, out theta))
        throw new ArgumentException("Angle must be a number (invariant culture) or a pi expression like pi/2.");

    return theta;
}

static void ApplyHadamardAll(int k)
{
    for (int q = 0; q < k; q++)
        Quantum.H(q);
}

static string TakeLowBits(string bitString, int k)
{
    // bitString is MSB..LSB (your BitString() prints MSB left)
    // qubit 0 is LSB, so "lowest k bits" are the rightmost k chars.
    if (k <= 0) return "";
    if (k >= bitString.Length) return bitString;
    return bitString[^k..];
}

static bool TryParsePiExpression(string s, out double value)
{
    value = 0;
    s = s.Replace(" ", "");
    if (!s.Contains("pi")) return false;

    s = System.Text.RegularExpressions.Regex.Replace(s, @"(\d)pi", "$1*pi");

    if (s.StartsWith("pi", StringComparison.Ordinal))
        s = "1*" + s;

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
    Console.WriteLine("  EXPECT                     Show top basis probabilities (no measurement)");
    Console.WriteLine("  X <q>, Y <q>, Z <q>         Apply Pauli gates to qubit q");
    Console.WriteLine("  H <q>, S <q>, T <q>         Apply standard single-qubit gates to qubit q");
    Console.WriteLine("  RX <q> <theta>              Rotation about X on qubit q (theta in radians, e.g. RX 0 pi/2)");
    Console.WriteLine("  RY <q> <theta>              Rotation about Y");
    Console.WriteLine("  RZ <q> <theta>              Rotation about Z");
    Console.WriteLine("  PRINT                       Show top amplitudes/probabilities");
    Console.WriteLine("  MEASUREALL (or MEASURE)     Measure full computational basis (collapses state)");
    Console.WriteLine("  SAMPLE <n>                  Repeated measurement sampling (restores state each trial)");
    Console.WriteLine("  RESET                       Reset to |00..0>");
    Console.WriteLine("  QRAND [k]                  Generate k random bits (default k=n). Uses RESET; H all; MEASUREALL");
    Console.WriteLine("  QUIT                        Exit");
    Console.WriteLine();
    Console.WriteLine("Angle formats:");
    Console.WriteLine("  1.57079632679   pi   pi/2   3*pi/4   -pi/8");
}