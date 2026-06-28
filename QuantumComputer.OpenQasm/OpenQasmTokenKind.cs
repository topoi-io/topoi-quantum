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
    OpenBrace,
    CloseBrace,
    Comma,
    Semicolon,

    Plus,
    Minus,
    Star,
    Slash,

    OpenQasm,
    Include,
    Qubit,
    Pi,
    Bit,
    Measure,
    Reset,
    Barrier,
    Arrow,
    Equals,
    Gate,

    At,
    Ctrl,
    Inv,
    Pow,
    NegCtrl
}
