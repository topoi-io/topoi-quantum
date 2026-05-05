namespace QuantumComputer.OpenQasm;

public enum OpenQasmTokenKind
{
    EndOfFile,

    Identifier,
    Integer,
    Number,
    String,

    OpenParen,
    CloseParen,
    OpenBracket,
    CloseBracket,
    Comma,
    Semicolon,

    Plus,
    Minus,
    Star,
    Slash,

    OpenQasm,
    Include,
    Qubit,
    Pi
}
