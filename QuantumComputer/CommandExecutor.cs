using System.Globalization;
using System.Numerics;

namespace QuantumComputer;

public sealed class CommandExecutor
{
    private QuantumCircuit? _loadedCircuit;

    public QuantumCircuit? LoadedCircuit => _loadedCircuit;

    public bool Execute(string line, bool echo = false)
    {
        line = StripComment(line).Trim();

        if (line.Length == 0)
            return false;

        if (echo)
            Console.WriteLine($"> {line}");

        string[] parts = line.Split(
            ' ',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        string cmd = parts[0].ToUpperInvariant();

        switch (cmd)
        {
            case "HELP":
                HelpPrinter.PrintInteractiveHelp();
                break;

            case "PRINT":
                Quantum.PrintStateTop();
                break;

            case "PROBS":
            case "PROBABILITIES":
                Quantum.PrintProbabilitiesTop();
                break;

            case "EXPECT":
                {
                    PauliTerm[] terms = ObservableParser.Parse(parts, 1);
                    QuantumConsolePrinter.PrintExpectation(Quantum.DefaultSimulator, terms);
                    break;
                }

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
                    int q = ParseQubit(parts, 1);
                    int bit = Quantum.Measure(q);

                    Console.WriteLine($"Measured qubit {q}: {bit} (partial collapse)");
                    break;
                }

            case "MEM":
                Quantum.PrintMemoryEstimate();
                break;

            case "NORM":
                Quantum.PrintNorm();
                break;

            case "NORMALIZE":
                Quantum.Normalize();
                Console.WriteLine("State normalized.");
                break;

            case "X":
                Quantum.X(ParseQubit(parts, 1));
                break;

            case "Y":
                Quantum.Y(ParseQubit(parts, 1));
                break;

            case "Z":
                Quantum.Z(ParseQubit(parts, 1));
                break;

            case "H":
                Quantum.H(ParseQubit(parts, 1));
                break;

            case "S":
                Quantum.S(ParseQubit(parts, 1));
                break;

            case "T":
                Quantum.T(ParseQubit(parts, 1));
                break;

            case "RX":
                Quantum.RX(ParseQubit(parts, 1), ParseAngle(parts, 2));
                break;

            case "RY":
                Quantum.RY(ParseQubit(parts, 1), ParseAngle(parts, 2));
                break;

            case "RZ":
                Quantum.RZ(ParseQubit(parts, 1), ParseAngle(parts, 2));
                break;

            case "CX":
            case "CNOT":
                Quantum.CX(ParseQubit(parts, 1), ParseQubit(parts, 2));
                break;

            case "CZ":
                Quantum.CZ(ParseQubit(parts, 1), ParseQubit(parts, 2));
                break;

            case "SWAP":
                Quantum.SWAP(ParseQubit(parts, 1), ParseQubit(parts, 2));
                break;

            case "CCX":
            case "TOFFOLI":
                Quantum.CCX(
                    ParseQubit(parts, 1),
                    ParseQubit(parts, 2),
                    ParseQubit(parts, 3));
                break;

            case "CRX":
                Quantum.CRX(
                    ParseQubit(parts, 1),
                    ParseQubit(parts, 2),
                    ParseAngle(parts, 3));
                break;

            case "CRY":
                Quantum.CRY(
                    ParseQubit(parts, 1),
                    ParseQubit(parts, 2),
                    ParseAngle(parts, 3));
                break;

            case "CRZ":
                Quantum.CRZ(
                    ParseQubit(parts, 1),
                    ParseQubit(parts, 2),
                    ParseAngle(parts, 3));
                break;

            case "QRAND":
                RunQRand(parts);
                break;

            case "SAMPLE":
                RunSample(parts);
                break;

            case "RUN":
                RunScript(parts);
                break;

            case "LOAD":
                _loadedCircuit = LoadCircuit(parts);
                Console.WriteLine($"Loaded circuit with {_loadedCircuit.QubitCount} qubits and {_loadedCircuit.Operations.Count} operations.");
                break;

            case "CIRCUIT":
                if (_loadedCircuit is null)
                    Console.WriteLine("No circuit loaded. Use LOAD <path> first.");
                else
                    _loadedCircuit.Print();
                break;

            case "DRAW":
                if (_loadedCircuit is null)
                    Console.WriteLine("No circuit loaded. Use LOAD <path> first.");
                else
                    Console.Write(_loadedCircuit.Draw());
                break;

            case "RUNCIRCUIT":
                if (_loadedCircuit is null)
                {
                    Console.WriteLine("No circuit loaded. Use LOAD <path> first.");
                }
                else
                {
                    _loadedCircuit.Run(resetFirst: true);
                    Console.WriteLine("Circuit executed.");
                }
                break;

            case "CLEARCIRCUIT":
                _loadedCircuit = null;
                Console.WriteLine("Loaded circuit cleared.");
                break;

            case "QUIT":
            case "EXIT":
                return true;

            default:
                Console.WriteLine($"Unknown command: {cmd}. Type HELP.");
                break;
        }

        return false;
    }

    public void RunScriptPath(string path)
    {
        path = CircuitFileLoader.ResolveInputPath(path);

        if (!File.Exists(path))
            throw new FileNotFoundException($"Script file not found: {path}");

        Console.WriteLine($"Running script: {path}");

        string[] lines = File.ReadAllLines(path);

        for (int i = 0; i < lines.Length; i++)
        {
            string rawLine = lines[i];

            try
            {
                bool shouldExit = Execute(rawLine, echo: true);

                if (shouldExit)
                {
                    Console.WriteLine($"Script requested exit at line {i + 1}.");
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Script error in '{path}' at line {i + 1}: {ex.Message}\n" +
                    $"Line: {rawLine}");
            }
        }

        Console.WriteLine($"Finished script: {path}");
    }

    private void RunScript(string[] parts)
    {
        if (parts.Length < 2)
            throw new ArgumentException("Usage: RUN <path>   Example: RUN examples/bell.qc");

        string path = ReconstructPath(parts, 1);
        RunScriptPath(path);
    }

    private QuantumCircuit LoadCircuit(string[] parts)
    {
        if (parts.Length < 2)
            throw new ArgumentException("Usage: LOAD <path>   Example: LOAD circuits/bell.qc");

        string path = ReconstructPath(parts, 1);
        return CircuitFileLoader.Load(path, Quantum.Register.QubitCount);
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

    private static string ReconstructPath(string[] parts, int startIndex)
    {
        string path = string.Join(' ', parts.Skip(startIndex)).Trim();

        if (path.Length == 0)
            throw new ArgumentException("Missing script path.");

        if ((path.StartsWith('"') && path.EndsWith('"')) ||
            (path.StartsWith('\'') && path.EndsWith('\'')))
        {
            path = path[1..^1];
        }

        return path;
    }

    private static void RunQRand(string[] parts)
    {
        int n = Quantum.Register.QubitCount;
        int k = n;

        if (parts.Length >= 2)
        {
            if (!int.TryParse(parts[1], out k) || k <= 0)
                throw new ArgumentException("Usage: QRAND [k]  where k is a positive integer.");

            if (k > n)
                throw new ArgumentException($"QRAND {k} requested, but register has only {n} qubits. Start with n >= {k}.");
        }

        Quantum.Reset();

        for (int q = 0; q < n; q++)
            Quantum.H(q);

        int outcome = Quantum.MeasureAll();
        string bits = Quantum.Register.BitString(outcome);

        string kBits = TakeLowBits(bits, k);
        int value = Convert.ToInt32(kBits, 2);

        Console.WriteLine(kBits);
        Console.WriteLine($"(int: {value})");
    }

    private static void RunSample(string[] parts)
    {
        if (parts.Length < 2 || !int.TryParse(parts[1], out int trials) || trials <= 0)
            throw new ArgumentException("Usage: SAMPLE <n>   Example: SAMPLE 1000");

        Complex[] snap = Quantum.SnapshotState();

        var counts = new Dictionary<int, int>();

        for (int i = 0; i < trials; i++)
        {
            int outcome = Quantum.MeasureAll();

            counts[outcome] = counts.TryGetValue(outcome, out int c)
                ? c + 1
                : 1;

            Quantum.RestoreState(snap);
        }

        Console.WriteLine($"Samples: {trials}");

        foreach (var kv in counts.OrderByDescending(kv => kv.Value).Take(16))
        {
            Console.WriteLine(
                $"|{Quantum.Register.BitString(kv.Key)}⟩ : {kv.Value} ({(double)kv.Value / trials:P2})");
        }

        if (counts.Count > 16)
            Console.WriteLine("... (showing top 16 outcomes)");
    }

    private static string TakeLowBits(string bitString, int k)
    {
        if (k <= 0)
            return "";

        if (k >= bitString.Length)
            return bitString;

        return bitString[^k..];
    }
}