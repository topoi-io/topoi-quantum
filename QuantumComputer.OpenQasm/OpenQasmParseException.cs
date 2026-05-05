namespace QuantumComputer.OpenQasm;

public sealed class OpenQasmParseException : Exception
{
    public int Line { get; }
    public int Column { get; }

    public OpenQasmParseException(string message, int line, int column)
        : base($"{message} at line {line}, column {column}.")
    {
        Line = line;
        Column = column;
    }
}