using System.Text;

using Topoi.Quantum.Cli;
using Topoi.Quantum;
using Topoi.Quantum.Drawing;
using Topoi.Quantum.OpenQasm;
using Topoi.Quantum.Parsing;

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

Console.WriteLine("Topoi Quantum CLI (state-vector simulator)");
Console.Write("Number of qubits n (e.g. 1,2,3): ");

int n = 1;

string? s = Console.ReadLine();

if (!string.IsNullOrWhiteSpace(s) && int.TryParse(s, out int parsed) && parsed > 0)
    n = parsed;

Quantum.Init(n);
Quantum.Reset();
QuantumConsolePrinter.PrintStateTop(Quantum.Register);

Console.WriteLine();
Console.WriteLine("Type HELP for Topoi Quantum commands. Angles are in radians.\n");

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
    int sourceCount = 0;

    if (!string.IsNullOrWhiteSpace(options.RunPath))
        sourceCount++;

    if (!string.IsNullOrWhiteSpace(options.CircuitPath))
        sourceCount++;

    if (!string.IsNullOrWhiteSpace(options.OpenQasmPath))
        sourceCount++;

    if (sourceCount > 1)
        throw new ArgumentException("Use only one of --run, --circuit, or --qasm.");

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

    if (!string.IsNullOrWhiteSpace(options.OpenQasmPath))
    {
        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromFile(options.OpenQasmPath);

        Quantum.Init(circuit.QubitCount);

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
        try
        {
            PauliTerm[] terms = ObservableParser.Parse(observable);
            QuantumConsolePrinter.PrintExpectation(Quantum.DefaultSimulator, terms);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Expectation error for \"{observable}\": {ex.Message}");
        }
    }

    if (options.SampleCount is not null)
    {
        var executor = new CommandExecutor();
        executor.Execute($"SAMPLE {options.SampleCount.Value}");
    }
}