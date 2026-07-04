using System.Text;

using Topoi.Quantum.Cli;
using Topoi.Quantum;
using Topoi.Quantum.Drawing;
using Topoi.Quantum.OpenQasm;
using Topoi.Quantum.Parsing;

Console.OutputEncoding = Encoding.UTF8;

QuantumSimulator? simulator = null;
CommandExecutor? executor = null;

if (args.Length > 0)
{
    CliOptions options = CliOptionsParser.Parse(args);

    if (options.ShowHelp)
    {
        HelpPrinter.PrintCliHelp();
        return;
    }

    simulator = new QuantumSimulator(options.Qubits);
    simulator.Reset();

    executor = new CommandExecutor(simulator);

    RunCommandLine(options, executor);
    return;
}

Console.WriteLine("Topoi Quantum CLI (state-vector simulator)");
Console.Write("Number of qubits n (e.g. 1,2,3): ");

int n = 1;

string? s = Console.ReadLine();

if (!string.IsNullOrWhiteSpace(s) && int.TryParse(s, out int parsed) && parsed > 0)
    n = parsed;

simulator = new QuantumSimulator(n);
simulator.Reset();

executor = new CommandExecutor(simulator);

QuantumConsolePrinter.PrintStateTop(simulator.Register);

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
        RunPostExecutionOptions(options, executor);
        return;
    }

    if (!string.IsNullOrWhiteSpace(options.CircuitPath))
    {
        QuantumCircuit circuit = CircuitFileLoader.Load(
                                                        options.CircuitPath,
                                                        executor.Simulator.Register.QubitCount);

        circuit.Run(executor.Simulator, resetFirst: true);

        RunPostExecutionOptions(options, executor);
        return;
    }

    if (!string.IsNullOrWhiteSpace(options.OpenQasmPath))
    {
        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromFile(options.OpenQasmPath);

        var qasmExecutor = new CommandExecutor(new QuantumSimulator(circuit.QubitCount));

        if (options.PrintCircuit)
            QuantumConsolePrinter.PrintCircuit(circuit);

        if (options.DrawCircuit)
            Console.Write(CircuitDrawer.Draw(circuit));

        circuit.Run(qasmExecutor.Simulator, resetFirst: true);

        RunPostExecutionOptions(options, qasmExecutor);
        return;
    }

    throw new ArgumentException("No action specified. Use --run <path>, --circuit <path>, or --help.");
}

void RunPostExecutionOptions(CliOptions options, CommandExecutor executor)
{
    if (options.PrintState)
        QuantumConsolePrinter.PrintStateTop(executor.Simulator.Register);

    if (options.PrintProbabilities)
        QuantumConsolePrinter.PrintProbabilitiesTop(executor.Simulator.Register);

    foreach (string observable in options.Expectations)
    {
        try
        {
            PauliTerm[] terms = ObservableParser.Parse(observable);
            QuantumConsolePrinter.PrintExpectation(executor.Simulator, terms);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Expectation error for \"{observable}\": {ex.Message}");
        }
    }

    if (options.SampleCount is not null)
        executor.Execute($"SAMPLE {options.SampleCount.Value}");
}