using System.Globalization;

namespace QuantumComputer.OpenQasm;

public sealed class OpenQasmParser
{
    private readonly IReadOnlyList<OpenQasmToken> _tokens;
    private int _position;

    public OpenQasmParser(IReadOnlyList<OpenQasmToken> tokens)
    {
        _tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
    }

    public OpenQasmProgram ParseProgram()
    {
        string version = ParseVersion();

        var statements = new List<OpenQasmStatement>();

        while (!Check(OpenQasmTokenKind.EndOfFile))
        {
            statements.Add(ParseStatement());
        }

        return new OpenQasmProgram(version, statements);
    }

    private string ParseVersion()
    {
        Consume(OpenQasmTokenKind.OpenQasm, "Expected OPENQASM version declaration.");

        OpenQasmToken versionToken;

        if (Check(OpenQasmTokenKind.Integer) || Check(OpenQasmTokenKind.Number))
            versionToken = Advance();
        else
            throw Error(Current, "Expected OpenQASM version number.");

        Consume(OpenQasmTokenKind.Semicolon, "Expected ';' after OPENQASM version.");

        if (versionToken.Text is not ("3" or "3.0" or "3.1"))
        {
            throw Error(
                versionToken,
                $"Unsupported OpenQASM version '{versionToken.Text}'. Supported versions are 3, 3.0, and 3.1.");
        }

        return versionToken.Text;
    }

    private OpenQasmStatement ParseStatement()
    {
        if (Match(OpenQasmTokenKind.Include))
            return ParseIncludeStatement();

        if (Match(OpenQasmTokenKind.Qubit))
            return ParseQubitDeclaration();

        if (Check(OpenQasmTokenKind.Identifier))
            return ParseGateCall();

        throw Error(Current, $"Unsupported OpenQASM statement starting with '{Current.Text}'.");
    }

    private OpenQasmIncludeStatement ParseIncludeStatement()
    {
        OpenQasmToken path = Consume(OpenQasmTokenKind.String, "Expected include path string.");
        Consume(OpenQasmTokenKind.Semicolon, "Expected ';' after include statement.");

        if (path.Text != "stdgates.inc")
            throw Error(path, $"Only include \"stdgates.inc\" is currently supported. Found \"{path.Text}\".");

        return new OpenQasmIncludeStatement(path.Text);
    }

    private OpenQasmQubitDeclaration ParseQubitDeclaration()
    {
        Consume(OpenQasmTokenKind.OpenBracket, "Expected '[' after qubit.");
        int size = ParseIntegerLiteral();
        Consume(OpenQasmTokenKind.CloseBracket, "Expected ']' after qubit size.");

        OpenQasmToken name = Consume(OpenQasmTokenKind.Identifier, "Expected qubit register name.");
        Consume(OpenQasmTokenKind.Semicolon, "Expected ';' after qubit declaration.");

        if (size <= 0)
            throw Error(name, "Qubit register size must be positive.");

        return new OpenQasmQubitDeclaration(name.Text, size);
    }

    private OpenQasmGateCallStatement ParseGateCall()
    {
        OpenQasmToken gate = Consume(OpenQasmTokenKind.Identifier, "Expected gate name.");

        var parameters = new List<double>();

        if (Match(OpenQasmTokenKind.OpenParen))
        {
            if (!Check(OpenQasmTokenKind.CloseParen))
            {
                do
                {
                    parameters.Add(ParseAngleExpression());
                }
                while (Match(OpenQasmTokenKind.Comma));
            }

            Consume(OpenQasmTokenKind.CloseParen, "Expected ')' after gate parameters.");
        }

        var qubits = new List<OpenQasmQubitReference>();

        do
        {
            qubits.Add(ParseQubitReference());
        }
        while (Match(OpenQasmTokenKind.Comma));

        Consume(OpenQasmTokenKind.Semicolon, "Expected ';' after gate call.");

        return new OpenQasmGateCallStatement(gate.Text, parameters, qubits);
    }

    private OpenQasmQubitReference ParseQubitReference()
    {
        OpenQasmToken name = Consume(OpenQasmTokenKind.Identifier, "Expected qubit register name.");

        Consume(OpenQasmTokenKind.OpenBracket, "Expected '[' after qubit register name.");
        int index = ParseIntegerLiteral();
        Consume(OpenQasmTokenKind.CloseBracket, "Expected ']' after qubit index.");

        return new OpenQasmQubitReference(name.Text, index);
    }

    private int ParseIntegerLiteral()
    {
        OpenQasmToken token = Consume(OpenQasmTokenKind.Integer, "Expected integer literal.");

        if (!int.TryParse(token.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
            throw Error(token, $"Invalid integer literal '{token.Text}'.");

        return value;
    }

    private double ParseAngleExpression()
    {
        // Supports:
        // pi
        // -pi
        // pi/2
        // -pi/2
        // 3*pi/4
        // 0.5*pi
        // numeric literals
        double value = ParseAngleTerm();

        while (Match(OpenQasmTokenKind.Plus) || Match(OpenQasmTokenKind.Minus))
        {
            OpenQasmToken op = Previous;
            double rhs = ParseAngleTerm();

            value = op.Kind == OpenQasmTokenKind.Plus
                ? value + rhs
                : value - rhs;
        }

        return value;
    }

    private double ParseAngleTerm()
    {
        double value = ParseAngleFactor();

        while (Match(OpenQasmTokenKind.Star) || Match(OpenQasmTokenKind.Slash))
        {
            OpenQasmToken op = Previous;
            double rhs = ParseAngleFactor();

            if (op.Kind == OpenQasmTokenKind.Star)
            {
                value *= rhs;
            }
            else
            {
                if (rhs == 0.0)
                    throw Error(op, "Division by zero in angle expression.");

                value /= rhs;
            }
        }

        return value;
    }

    private double ParseAngleFactor()
    {
        if (Match(OpenQasmTokenKind.Minus))
            return -ParseAngleFactor();

        if (Match(OpenQasmTokenKind.Plus))
            return ParseAngleFactor();

        if (Match(OpenQasmTokenKind.Pi))
            return Math.PI;

        if (Match(OpenQasmTokenKind.Integer) || Match(OpenQasmTokenKind.Number))
        {
            OpenQasmToken token = Previous;

            if (!double.TryParse(token.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                throw Error(token, $"Invalid number literal '{token.Text}'.");

            return value;
        }

        if (Match(OpenQasmTokenKind.OpenParen))
        {
            double value = ParseAngleExpression();
            Consume(OpenQasmTokenKind.CloseParen, "Expected ')' after angle expression.");
            return value;
        }

        throw Error(Current, $"Expected angle expression but found '{Current.Text}'.");
    }

    private bool Match(OpenQasmTokenKind kind)
    {
        if (!Check(kind))
            return false;

        Advance();
        return true;
    }

    private bool Check(OpenQasmTokenKind kind)
    {
        return Current.Kind == kind;
    }

    private OpenQasmToken Consume(OpenQasmTokenKind kind, string message)
    {
        if (Check(kind))
            return Advance();

        throw Error(Current, message);
    }

    private OpenQasmToken Advance()
    {
        if (!Check(OpenQasmTokenKind.EndOfFile))
            _position++;

        return Previous;
    }

    private OpenQasmToken Current => _tokens[_position];

    private OpenQasmToken Previous => _tokens[_position - 1];

    private static OpenQasmParseException Error(OpenQasmToken token, string message)
    {
        return new OpenQasmParseException(message, token.Line, token.Column);
    }
}