namespace QuantumComputer.OpenQasm;

public sealed class OpenQasmLexer
{
    private readonly string _source;
    private int _position;
    private int _line = 1;
    private int _column = 1;

    public OpenQasmLexer(string source)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public IReadOnlyList<OpenQasmToken> Tokenize()
    {
        var tokens = new List<OpenQasmToken>();

        while (true)
        {
            OpenQasmToken token = NextToken();
            tokens.Add(token);

            if (token.Kind == OpenQasmTokenKind.EndOfFile)
                break;
        }

        return tokens;
    }

    private OpenQasmToken NextToken()
    {
        SkipWhitespaceAndComments();

        int line = _line;
        int column = _column;

        if (IsAtEnd)
            return new OpenQasmToken(OpenQasmTokenKind.EndOfFile, string.Empty, line, column);

        char c = Current;

        if (char.IsLetter(c) || c == '_')
            return ReadIdentifierOrKeyword();

        if (char.IsDigit(c))
            return ReadNumber();

        if (c == '"')
            return ReadString();

        if (c == '-' && Peek() == '>')
        {
            Advance();
            Advance();

            return new OpenQasmToken(OpenQasmTokenKind.Arrow, "->", line, column);
        }

        Advance();

        return c switch
        {
            '(' => new OpenQasmToken(OpenQasmTokenKind.OpenParen, "(", line, column),
            ')' => new OpenQasmToken(OpenQasmTokenKind.CloseParen, ")", line, column),
            '[' => new OpenQasmToken(OpenQasmTokenKind.OpenBracket, "[", line, column),
            ']' => new OpenQasmToken(OpenQasmTokenKind.CloseBracket, "]", line, column),
            ',' => new OpenQasmToken(OpenQasmTokenKind.Comma, ",", line, column),
            ';' => new OpenQasmToken(OpenQasmTokenKind.Semicolon, ";", line, column),
            '+' => new OpenQasmToken(OpenQasmTokenKind.Plus, "+", line, column),
            '-' => new OpenQasmToken(OpenQasmTokenKind.Minus, "-", line, column),
            '*' => new OpenQasmToken(OpenQasmTokenKind.Star, "*", line, column),
            '/' => new OpenQasmToken(OpenQasmTokenKind.Slash, "/", line, column),
            '=' => new OpenQasmToken(OpenQasmTokenKind.Equals, "=", line, column),
            '{' => new OpenQasmToken(OpenQasmTokenKind.OpenBrace, "{", line, column),
            '}' => new OpenQasmToken(OpenQasmTokenKind.CloseBrace, "}", line, column),
            _ => throw new OpenQasmParseException($"Unexpected character '{c}'", line, column)
        };
    }

    private OpenQasmToken ReadIdentifierOrKeyword()
    {
        int start = _position;
        int line = _line;
        int column = _column;

        while (!IsAtEnd && (char.IsLetterOrDigit(Current) || Current == '_'))
            Advance();

        string text = _source[start.._position];

        OpenQasmTokenKind kind = text switch
        {
            "OPENQASM" => OpenQasmTokenKind.OpenQasm,
            "include" => OpenQasmTokenKind.Include,
            "qubit" => OpenQasmTokenKind.Qubit,
            "bit" => OpenQasmTokenKind.Bit,
            "measure" => OpenQasmTokenKind.Measure,
            "reset" => OpenQasmTokenKind.Reset,
            "barrier" => OpenQasmTokenKind.Barrier,
            "gate" => OpenQasmTokenKind.Gate,
            "pi" => OpenQasmTokenKind.Pi,
            _ => OpenQasmTokenKind.Identifier
        };

        return new OpenQasmToken(kind, text, line, column);
    }

    private OpenQasmToken ReadNumber()
    {
        int start = _position;
        int line = _line;
        int column = _column;

        while (!IsAtEnd && char.IsDigit(Current))
            Advance();

        bool isNumber = false;

        if (!IsAtEnd && Current == '.')
        {
            isNumber = true;
            Advance();

            while (!IsAtEnd && char.IsDigit(Current))
                Advance();
        }

        string text = _source[start.._position];

        return new OpenQasmToken(
            isNumber ? OpenQasmTokenKind.Number : OpenQasmTokenKind.Integer,
            text,
            line,
            column);
    }

    private OpenQasmToken ReadString()
    {
        int line = _line;
        int column = _column;

        Advance(); // opening quote

        int start = _position;

        while (!IsAtEnd && Current != '"')
        {
            if (Current == '\n')
                throw new OpenQasmParseException("Unterminated string literal", line, column);

            Advance();
        }

        if (IsAtEnd)
            throw new OpenQasmParseException("Unterminated string literal", line, column);

        string text = _source[start.._position];

        Advance(); // closing quote

        return new OpenQasmToken(OpenQasmTokenKind.String, text, line, column);
    }

    private void SkipWhitespaceAndComments()
    {
        while (!IsAtEnd)
        {
            if (char.IsWhiteSpace(Current))
            {
                Advance();
                continue;
            }

            if (Current == '/' && Peek() == '/')
            {
                while (!IsAtEnd && Current != '\n')
                    Advance();

                continue;
            }

            break;
        }
    }

    private char Current => _source[_position];

    private bool IsAtEnd => _position >= _source.Length;

    private char Peek()
    {
        int next = _position + 1;
        return next >= _source.Length ? '\0' : _source[next];
    }

    private void Advance()
    {
        if (IsAtEnd)
            return;

        if (_source[_position] == '\n')
        {
            _line++;
            _column = 1;
        }
        else
        {
            _column++;
        }

        _position++;
    }
}