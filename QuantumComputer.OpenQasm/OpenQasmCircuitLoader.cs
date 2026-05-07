using QuantumComputer.Core;

namespace QuantumComputer.OpenQasm;

public static class OpenQasmCircuitLoader
{
    public static QuantumCircuit LoadFromString(string source)
    {
        var lexer = new OpenQasmLexer(source);
        IReadOnlyList<OpenQasmToken> tokens = lexer.Tokenize();

        var parser = new OpenQasmParser(tokens);
        OpenQasmProgram program = parser.ParseProgram();

        return OpenQasmCircuitConverter.Convert(program);
    }

    public static QuantumCircuit LoadFromFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be empty.", nameof(path));

        if (!File.Exists(path))
            throw new FileNotFoundException($"OpenQASM file not found: {path}", path);

        string source = File.ReadAllText(path);

        return LoadFromString(source);
    }

    public static OpenQasmExecutableProgram LoadExecutableFromString(string source)
    {
        var lexer = new OpenQasmLexer(source);
        IReadOnlyList<OpenQasmToken> tokens = lexer.Tokenize();

        var parser = new OpenQasmParser(tokens);
        OpenQasmProgram program = parser.ParseProgram();

        return OpenQasmExecutableConverter.Convert(program);
    }

    public static OpenQasmExecutableProgram LoadExecutableFromFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be empty.", nameof(path));

        if (!File.Exists(path))
            throw new FileNotFoundException($"OpenQASM file not found: {path}", path);

        string source = File.ReadAllText(path);

        return LoadExecutableFromString(source);
    }
}