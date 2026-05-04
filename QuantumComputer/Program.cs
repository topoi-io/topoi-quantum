using QuantumComputer;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;

Console.OutputEncoding = System.Text.Encoding.UTF8;

QuantumCircuit? loadedCircuit = null;

if (args.Length > 0)
{
    CliOptions options = ParseCliArgs(args);

    if (options.ShowHelp)
    {
        PrintCliHelp();
        return;
    }

    Quantum.Init(options.Qubits);
    Quantum.Reset();

    RunCommandLine(options);
    return;
}

Console.WriteLine("N-Qubit Gate Interpreter (state-vector)");
Console.Write("Number of qubits n (e.g. 1,2,3): ");

int n = 1;
{
    string? s = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(s) && int.TryParse(s, out int parsed) && parsed > 0)
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

    if (line is null)
        break;

    try
    {
        bool shouldExit = ExecuteCommand(line, echo: false);

        if (shouldExit)
            return;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

bool ExecuteCommand(string line, bool echo)
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
            PrintHelp();
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
                PrintExpectation(terms);
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
                if (parts.Length == 1)
                    throw new ArgumentException("Usage: MEASURE <q>   Example: MEASURE 0");

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

        // Single-qubit gates
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

        // Entangling / controlled gates
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
            loadedCircuit = LoadCircuit(parts);
            Console.WriteLine($"Loaded circuit with {loadedCircuit.QubitCount} qubits and {loadedCircuit.Operations.Count} operations.");
            break;

        case "CIRCUIT":
            if (loadedCircuit is null)
                Console.WriteLine("No circuit loaded. Use LOAD <path> first.");
            else
                loadedCircuit.Print();
            break;

        case "DRAW":
            if (loadedCircuit is null)
                Console.WriteLine("No circuit loaded. Use LOAD <path> first.");
            else
                Console.Write(loadedCircuit.Draw());
            break;

        case "RUNCIRCUIT":
            if (loadedCircuit is null)
            {
                Console.WriteLine("No circuit loaded. Use LOAD <path> first.");
            }
            else
            {
                loadedCircuit.Run(resetFirst: true);
                Console.WriteLine("Circuit executed.");
            }
            break;

        case "CLEARCIRCUIT":
            loadedCircuit = null;
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

void RunCommandLine(CliOptions options)
{
    if (!string.IsNullOrWhiteSpace(options.RunPath) &&
        !string.IsNullOrWhiteSpace(options.CircuitPath))
    {
        throw new ArgumentException("Use either --run <path> or --circuit <path>, not both.");
    }

    if (!string.IsNullOrWhiteSpace(options.RunPath))
    {
        RunScriptPath(options.RunPath);
        RunPostExecutionOptions(options);
        return;
    }

    if (!string.IsNullOrWhiteSpace(options.CircuitPath))
    {
        loadedCircuit = LoadCircuitFromPath(options.CircuitPath);

        if (options.PrintCircuit)
            loadedCircuit.Print();

        if (options.DrawCircuit)
            Console.Write(loadedCircuit.Draw());

        loadedCircuit.Run(resetFirst: true);

        RunPostExecutionOptions(options);
        return;
    }

    throw new ArgumentException("No action specified. Use --run <path>, --circuit <path>, or --help.");
}

void RunPostExecutionOptions(CliOptions options)
{
    if (options.PrintState)
        Quantum.PrintStateTop();

    if (options.PrintProbabilities)
        Quantum.PrintProbabilitiesTop();

    foreach (string observable in options.Expectations)
    {
        string line = $"EXPECT {observable}";

        string[] parts = line.Split(
            ' ',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        PauliTerm[] terms = ObservableParser.Parse(parts, 1);
        PrintExpectation(terms);
    }

    if (options.SampleCount is not null)
        RunSample(new[] { "SAMPLE", options.SampleCount.Value.ToString(CultureInfo.InvariantCulture) });
}

void PrintExpectation(PauliTerm[] terms)
{
    Complex value = Quantum.ExpectPauliString(terms);

    Console.WriteLine($"⟨{ObservableParser.Format(terms)}⟩ = {value.Real:+0.############;-0.############;0}");

    if (Math.Abs(value.Imaginary) > 1e-10)
        Console.WriteLine($"  note: small imaginary residue = {value.Imaginary:+0.###e+0;-0.###e+0;0}");
}

void RunScript(string[] parts)
{
    if (parts.Length < 2)
        throw new ArgumentException("Usage: RUN <path>   Example: RUN examples/bell.qc");

    string path = ReconstructPath(parts, 1);
    RunScriptPath(path);
}

string StripComment(string line)
{
    int hash = line.IndexOf('#');

    if (hash < 0)
        return line;

    return line[..hash];
}

string ReconstructPath(string[] parts, int startIndex)
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

int ParseQubit(string[] parts, int index)
{
    if (parts.Length <= index)
        throw new ArgumentException("Missing qubit index. Example: H 0");

    if (!int.TryParse(parts[index], out int q) || q < 0)
        throw new ArgumentException("Qubit index must be a non-negative integer.");

    return q;
}

double ParseAngle(string[] parts, int index)
{
    if (parts.Length <= index)
        throw new ArgumentException("Missing angle. Example: RX 0 pi/2");

    return AngleParser.Parse(parts[index]);
}

void RunQRand(string[] parts)
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

void RunSample(string[] parts)
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

string TakeLowBits(string bitString, int k)
{
    // BitString prints MSB..LSB.
    // Qubit 0 is LSB, so the lowest k bits are the rightmost k characters.
    if (k <= 0)
        return "";

    if (k >= bitString.Length)
        return bitString;

    return bitString[^k..];
}

bool TryParsePiExpression(string s, out double value)
{
    value = 0.0;

    if (string.IsNullOrWhiteSpace(s))
        return false;

    s = s.Trim()
         .ToLowerInvariant()
         .Replace(" ", "");

    if (!s.Contains("pi", StringComparison.Ordinal))
        return false;

    try
    {
        return TryParsePiExpressionCore(s, out value);
    }
    catch
    {
        value = 0.0;
        return false;
    }
}

bool TryParsePiExpressionCore(string s, out double value)
{
    value = 0.0;

    string[] frac = s.Split('/', StringSplitOptions.RemoveEmptyEntries);

    if (frac.Length is < 1 or > 2)
        return false;

    if (!TryParsePiNumerator(frac[0], out double numerator))
        return false;

    double denominator = 1.0;

    if (frac.Length == 2)
    {
        if (!double.TryParse(frac[1], NumberStyles.Float, CultureInfo.InvariantCulture, out denominator))
            return false;

        if (denominator == 0.0)
            return false;
    }

    value = numerator / denominator;
    return true;
}

bool TryParsePiNumerator(string s, out double value)
{
    value = 0.0;

    if (string.IsNullOrWhiteSpace(s))
        return false;

    if (s == "pi" || s == "+pi")
    {
        value = Math.PI;
        return true;
    }

    if (s == "-pi")
    {
        value = -Math.PI;
        return true;
    }

    // Accept implicit multiplication: 3pi, -3pi, +3pi
    s = Regex.Replace(
        s,
        @"^([+-]?\d+(?:\.\d+)?(?:[eE][+-]?\d+)?)pi$",
        "$1*pi");

    string[] mul = s.Split('*', StringSplitOptions.RemoveEmptyEntries);

    if (mul.Length != 2)
        return false;

    if (mul[0] == "pi")
    {
        if (!double.TryParse(mul[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double factor))
            return false;

        value = Math.PI * factor;
        return true;
    }

    if (mul[1] == "pi")
    {
        if (!double.TryParse(mul[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double factor))
            return false;

        value = factor * Math.PI;
        return true;
    }

    return false;
}

QuantumCircuit LoadCircuit(string[] parts)
{
    if (parts.Length < 2)
        throw new ArgumentException("Usage: LOAD <path>   Example: LOAD circuits/bell.qc");

    string path = ReconstructPath(parts, 1);
    return LoadCircuitFromPath(path);
}

string ResolveInputPath(string path)
{
    path = path.Trim();

    if ((path.StartsWith('"') && path.EndsWith('"')) ||
        (path.StartsWith('\'') && path.EndsWith('\'')))
    {
        path = path[1..^1];
    }

    if (Path.IsPathRooted(path))
        return path;

    string currentDirectoryPath = Path.GetFullPath(path);

    if (File.Exists(currentDirectoryPath))
        return currentDirectoryPath;

    string appBasePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, path));

    if (File.Exists(appBasePath))
        return appBasePath;

    string projectPath = FindUpwardsForFile(Directory.GetCurrentDirectory(), path);

    if (projectPath is not null)
        return projectPath;

    projectPath = FindUpwardsForFile(AppContext.BaseDirectory, path);

    if (projectPath is not null)
        return projectPath;

    return currentDirectoryPath;
}

string? FindUpwardsForFile(string startDirectory, string relativePath)
{
    DirectoryInfo? directory = new DirectoryInfo(startDirectory);

    while (directory is not null)
    {
        string candidate = Path.GetFullPath(Path.Combine(directory.FullName, relativePath));

        if (File.Exists(candidate))
            return candidate;

        directory = directory.Parent;
    }

    return null;
}

void RunScriptPath(string path)
{
    path = ResolveInputPath(path);

    if (!File.Exists(path))
        throw new FileNotFoundException($"Script file not found: {path}");

    Console.WriteLine($"Running script: {path}");

    string[] lines = File.ReadAllLines(path);

    for (int i = 0; i < lines.Length; i++)
    {
        string rawLine = lines[i];

        try
        {
            bool shouldExit = ExecuteCommand(rawLine, echo: true);

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

QuantumCircuit LoadCircuitFromPath(string path)
{
    path = ResolveInputPath(path);

    if (!File.Exists(path))
        throw new FileNotFoundException($"Circuit file not found: {path}");

    int qubitCount = Quantum.Register.QubitCount;
    var circuit = new QuantumCircuit(qubitCount);

    string[] lines = File.ReadAllLines(path);

    for (int i = 0; i < lines.Length; i++)
    {
        string line = StripComment(lines[i]).Trim();

        if (line.Length == 0)
            continue;

        string[] lineParts = line.Split(
            ' ',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        string cmd = lineParts[0].ToUpperInvariant();

        if (cmd == "RESET")
            continue;

        if (!GateOperationParser.TryParse(lineParts, out GateOperation? operation, out string? error))
        {
            throw new InvalidOperationException(
                $"Line {i + 1}: cannot load '{line}' as a circuit operation. {error}");
        }

        circuit.Add(operation);
    }

    return circuit;
}

CliOptions ParseCliArgs(string[] args)
{
    var options = new CliOptions();

    for (int i = 0; i < args.Length; i++)
    {
        string arg = args[i];

        switch (arg.ToLowerInvariant())
        {
            case "--help":
            case "-h":
            case "/?":
                options.ShowHelp = true;
                break;

            case "--qubits":
            case "-q":
                options.Qubits = ParsePositiveIntCli(args, ref i, "--qubits");
                break;

            case "--run":
                options.RunPath = RequireValue(args, ref i, "--run");
                break;

            case "--circuit":
                options.CircuitPath = RequireValue(args, ref i, "--circuit");
                break;

            case "--print":
                options.PrintState = true;
                break;

            case "--probs":
            case "--probabilities":
                options.PrintProbabilities = true;
                break;

            case "--print-circuit":
                options.PrintCircuit = true;
                break;

            case "--draw":
                options.DrawCircuit = true;
                break;

            case "--sample":
                options.SampleCount = ParsePositiveIntCli(args, ref i, "--sample");
                break;

            case "--expect":
                options.Expectations.Add(RequireValue(args, ref i, "--expect"));
                break;

            default:
                throw new ArgumentException($"Unknown command-line argument: {arg}. Use --help.");
        }
    }

    return options;
}

string RequireValue(string[] args, ref int index, string optionName)
{
    if (index + 1 >= args.Length)
        throw new ArgumentException($"{optionName} requires a value.");

    index++;
    return args[index];
}

int ParsePositiveIntCli(string[] args, ref int index, string optionName)
{
    string value = RequireValue(args, ref index, optionName);

    if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result) || result <= 0)
        throw new ArgumentException($"{optionName} requires a positive integer.");

    return result;
}

void PrintHelp()
{
    Console.WriteLine("Commands:");

    Console.WriteLine("  PRINT                       Show top amplitudes and probabilities");
    Console.WriteLine("  PROBS                       Show top basis probabilities without measurement");
    Console.WriteLine("  EXPECT <observable>         Expectation value of Pauli observable");
    Console.WriteLine("                             Examples: EXPECT Z 0, EXPECT X 1, EXPECT ZZ 0 1, EXPECT Z0 Z1");

    Console.WriteLine();

    Console.WriteLine("Single-qubit gates:");
    Console.WriteLine("  X <q>, Y <q>, Z <q>         Apply Pauli gates to qubit q");
    Console.WriteLine("  H <q>, S <q>, T <q>         Apply standard single-qubit gates to qubit q");
    Console.WriteLine("  RX <q> <theta>              Rotation about X on qubit q");
    Console.WriteLine("  RY <q> <theta>              Rotation about Y on qubit q");
    Console.WriteLine("  RZ <q> <theta>              Rotation about Z on qubit q");

    Console.WriteLine();

    Console.WriteLine("Controlled / entangling gates:");
    Console.WriteLine("  CX <c> <t>                  Controlled-X / CNOT: flip target t if control c is 1");
    Console.WriteLine("  CNOT <c> <t>                Alias for CX");
    Console.WriteLine("  CZ <c> <t>                  Controlled-Z: phase flip when c and t are both 1");
    Console.WriteLine("  SWAP <q1> <q2>              Swap two qubits");
    Console.WriteLine("  CCX <c1> <c2> <t>           Toffoli gate: flip target if both controls are 1");
    Console.WriteLine("  TOFFOLI <c1> <c2> <t>       Alias for CCX");
    Console.WriteLine("  CRX <c> <t> <theta>         Controlled RX rotation");
    Console.WriteLine("  CRY <c> <t> <theta>         Controlled RY rotation");
    Console.WriteLine("  CRZ <c> <t> <theta>         Controlled RZ rotation");

    Console.WriteLine();

    Console.WriteLine("Measurement / diagnostics:");
    Console.WriteLine("  MEASURE <q>                 Measure one qubit and partially collapse state");
    Console.WriteLine("  MEASUREALL                  Measure full computational basis and collapse state");
    Console.WriteLine("  SAMPLE <n>                  Repeated measurement sampling, restoring state each trial");
    Console.WriteLine("  NORM                        Show current state norm");
    Console.WriteLine("  NORMALIZE                   Manually renormalize the state vector");
    Console.WriteLine("  MEM                         Show estimated dense state-vector memory usage");

    Console.WriteLine();

    Console.WriteLine("Other:");
    Console.WriteLine("  RESET                       Reset to |00..0>");
    Console.WriteLine("  QRAND [k]                   Generate k random bits, default k = number of qubits");
    Console.WriteLine("  RUN <path>                  Run commands from a .qc script file");
    Console.WriteLine("  LOAD <path>                 Load gate operations from a .qc file into a QuantumCircuit");
    Console.WriteLine("  CIRCUIT                     Print the currently loaded circuit");
    Console.WriteLine("  DRAW                        Draw the currently loaded circuit");
    Console.WriteLine("  RUNCIRCUIT                  Execute the currently loaded circuit");
    Console.WriteLine("  CLEARCIRCUIT                Clear the currently loaded circuit");
    Console.WriteLine("  QUIT                        Exit");

    Console.WriteLine();
    Console.WriteLine("CLI examples:");
    Console.WriteLine("  dotnet run -- --qubits 2 --run examples/bell.qc");
    Console.WriteLine("  dotnet run -- --qubits 2 --circuit circuits/bell.qc --print --expect \"ZZ 0 1\"");

    Console.WriteLine();
    Console.WriteLine("Angle formats:");
    Console.WriteLine("  1.57079632679   pi   +pi/2   -pi/8   3*pi/4   -3*pi/4   0.5*pi");
}

void PrintCliHelp()
{
    Console.WriteLine("N-Qubit Gate Interpreter CLI");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run -- [options]");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  --help, -h                  Show command-line help");
    Console.WriteLine("  --qubits <n>, -q <n>         Number of qubits to initialise, default 1");
    Console.WriteLine("  --run <path>                 Run a full interpreter script and exit");
    Console.WriteLine("  --circuit <path>             Load and run a gate-only QuantumCircuit file and exit");
    Console.WriteLine("  --print-circuit              Print loaded circuit before execution");
    Console.WriteLine("  --draw                       Draw loaded circuit before execution");
    Console.WriteLine("  --print                      Print final state amplitudes and probabilities");
    Console.WriteLine("  --probs                      Print final basis-state probabilities");
    Console.WriteLine("  --expect \"observable\"        Print expectation value, e.g. --expect \"ZZ 0 1\"");
    Console.WriteLine("  --sample <n>                 Sample final state n times");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  dotnet run -- --qubits 2 --run examples/bell.qc");
    Console.WriteLine("  dotnet run -- --qubits 2 --circuit circuits/bell.qc --print-circuit --print");
    Console.WriteLine("  dotnet run -- --qubits 2 --circuit circuits/bell.qc --expect \"ZZ 0 1\" --expect \"XX 0 1\"");
    Console.WriteLine("  dotnet run -- --qubits 3 --circuit circuits/ghz3.qc --probs --sample 1000");
    Console.WriteLine("  dotnet run -- --qubits 2 --circuit circuits/bell.qc --draw --expect \"ZZ 0 1\"");
}