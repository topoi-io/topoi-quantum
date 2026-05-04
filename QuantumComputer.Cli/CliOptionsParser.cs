using System.Globalization;

namespace QuantumComputer.Cli;

public static class CliOptionsParser
{
    public static CliOptions Parse(string[] args)
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

    private static string RequireValue(string[] args, ref int index, string optionName)
    {
        if (index + 1 >= args.Length)
            throw new ArgumentException($"{optionName} requires a value.");

        index++;
        return args[index];
    }

    private static int ParsePositiveIntCli(string[] args, ref int index, string optionName)
    {
        string value = RequireValue(args, ref index, optionName);

        if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result) || result <= 0)
            throw new ArgumentException($"{optionName} requires a positive integer.");

        return result;
    }
}