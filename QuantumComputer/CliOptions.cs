namespace QuantumComputer;

public sealed class CliOptions
{
    public bool ShowHelp { get; set; }

    public int Qubits { get; set; } = 1;

    public string? RunPath { get; set; }

    public string? CircuitPath { get; set; }

    public bool PrintState { get; set; }

    public bool PrintProbabilities { get; set; }

    public bool PrintCircuit { get; set; }

    public int? SampleCount { get; set; }

    public bool DrawCircuit { get; set; }

    public List<string> Expectations { get; } = [];
}