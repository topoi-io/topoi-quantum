using QuantumComputer.Cli;
using QuantumComputer.Core;
using QuantumComputer.Drawing;
using QuantumComputer.Parsing;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

var executor = new CommandExecutor();

if (args.Length > 0)
{
    CliOptions options = CliOptionsParser.Parse(args);

    if (options.ShowHelp)
    {
        HelpPrinter.PrintCliHelp();
        return;
    }

    Quantum.Init(options.Qubits);
    Quantum.Reset();

    RunCommandLine(options, executor);
    return;
}

Console.WriteLine("N-Qubit Gate Interpreter (state-vector)");
Console.Write("Number of qubits n (e.g. 1,2,3): ");

int n = 1;

string? s = Console.ReadLine();

if (!string.IsNullOrWhiteSpace(s) && int.TryParse(s, out int parsed) && parsed > 0)
    n = parsed;

Quantum.Init(n);
Quantum.Reset();
QuantumConsolePrinter.PrintStateTop(Quantum.Register);

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
        bool shouldExit = executor.Execute(line, echo: false);

        if (shouldExit)
            return;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void RunCommandLine(CliOptions options, CommandExecutor executor)
{
    if (!string.IsNullOrWhiteSpace(options.RunPath) &&
        !string.IsNullOrWhiteSpace(options.CircuitPath))
    {
        throw new ArgumentException("Use either --run <path> or --circuit <path>, not both.");
    }

    if (!string.IsNullOrWhiteSpace(options.RunPath))
    {
        executor.RunScriptPath(options.RunPath);
        RunPostExecutionOptions(options);
        return;
    }

    if (!string.IsNullOrWhiteSpace(options.CircuitPath))
    {
        QuantumCircuit circuit = CircuitFileLoader.Load(
            options.CircuitPath,
            Quantum.Register.QubitCount);

        if (options.PrintCircuit)
            circuit.Print();

        if (options.DrawCircuit)
            Console.Write(CircuitDrawer.Draw(circuit));

        circuit.Run(resetFirst: true);

        RunPostExecutionOptions(options);
        return;
    }

    throw new ArgumentException("No action specified. Use --run <path>, --circuit <path>, or --help.");
}

void RunPostExecutionOptions(CliOptions options)
{
    if (options.PrintState)
        QuantumConsolePrinter.PrintStateTop(Quantum.Register); ;

    if (options.PrintProbabilities)
        QuantumConsolePrinter.PrintProbabilitiesTop(Quantum.Register);

    foreach (string observable in options.Expectations)
    {
        PauliTerm[] terms = ObservableParser.Parse(observable);
        QuantumConsolePrinter.PrintExpectation(Quantum.DefaultSimulator, terms);
    }

    if (options.SampleCount is not null)
    {
        var executor = new CommandExecutor();
        executor.Execute($"SAMPLE {options.SampleCount.Value}");
    }
}