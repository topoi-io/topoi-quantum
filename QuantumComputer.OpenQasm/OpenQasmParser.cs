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

        if (Match(OpenQasmTokenKind.Bit))
            return ParseBitDeclaration();

        if (Match(OpenQasmTokenKind.Gate))
            return ParseGateDefinition();

        if (Check(OpenQasmTokenKind.Identifier) && PeekKind(1) == OpenQasmTokenKind.OpenBracket)
        {
            if (LooksLikeMeasurementAssignment())
                return ParseMeasurementAssignment();
        }

        if (Check(OpenQasmTokenKind.Identifier) ||
            Check(OpenQasmTokenKind.Ctrl) ||
            Check(OpenQasmTokenKind.Inv) ||
            Check(OpenQasmTokenKind.Pow) ||
            Check(OpenQasmTokenKind.NegCtrl))
        {
            return ParseGateCall();
        }

        if (Match(OpenQasmTokenKind.Measure))
            return ParseLegacyMeasurement(Previous);

        if (Match(OpenQasmTokenKind.Reset))
            return ParseResetStatement(Previous);

        if (Match(OpenQasmTokenKind.Barrier))
            return ParseBarrierStatement(Previous);

        if (StartsGateCall())
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

        return new OpenQasmQubitDeclaration(
                    name.Text,
                    size,
                    name.Line,
                    name.Column);
    }

    private OpenQasmGateCallStatement ParseGateCall()
    {
        List<OpenQasmGateModifier> modifiers = ParseGateModifiers();

        OpenQasmToken gate = Consume(
            OpenQasmTokenKind.Identifier,
            "Expected gate name.");

        var parameters = new List<OpenQasmAngleExpression>();

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

        var qubits = new List<OpenQasmQubitOperand>();

        do
        {
            qubits.Add(ParseQubitOperand());
        }
        while (Match(OpenQasmTokenKind.Comma));

        Consume(OpenQasmTokenKind.Semicolon, "Expected ';' after gate call.");

        return new OpenQasmGateCallStatement(
                        gate.Text,
                        parameters,
                        qubits,
                        modifiers,
                        gate.Line,
                        gate.Column);
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

    private OpenQasmAngleExpression ParseAngleExpression()
    {
        OpenQasmAngleExpression expression = ParseAngleTerm();

        while (Match(OpenQasmTokenKind.Plus) || Match(OpenQasmTokenKind.Minus))
        {
            OpenQasmToken op = Previous;
            OpenQasmAngleExpression rhs = ParseAngleTerm();

            expression = new OpenQasmAngleBinary(
                        expression,
                        op.Text,
                        rhs,
                        op.Line,
                        op.Column);
        }

        return expression;
    }

    private OpenQasmAngleExpression ParseAngleTerm()
    {
        OpenQasmAngleExpression expression = ParseAngleFactor();

        while (Match(OpenQasmTokenKind.Star) || Match(OpenQasmTokenKind.Slash))
        {
            OpenQasmToken op = Previous;
            OpenQasmAngleExpression rhs = ParseAngleFactor();

            expression = new OpenQasmAngleBinary(
                            expression,
                            op.Text,
                            rhs,
                            op.Line,
                            op.Column);
        }

        return expression;
    }

    private OpenQasmAngleExpression ParseAngleFactor()
    {
        if (Match(OpenQasmTokenKind.Minus))
        {
            OpenQasmToken op = Previous;

            return new OpenQasmAngleUnary(
                "-",
                ParseAngleFactor(),
                op.Line,
                op.Column);
        }

        if (Match(OpenQasmTokenKind.Plus))
        {
            OpenQasmToken op = Previous;

            return new OpenQasmAngleUnary(
                "+",
                ParseAngleFactor(),
                op.Line,
                op.Column);
        }

        if (Match(OpenQasmTokenKind.Pi))
        {
            OpenQasmToken token = Previous;
            return new OpenQasmAngleConstant(Math.PI, token.Line, token.Column);
        }

        if (Match(OpenQasmTokenKind.Identifier))
        {
            OpenQasmToken token = Previous;
            return new OpenQasmAngleParameter(token.Text, token.Line, token.Column);
        }

        if (Match(OpenQasmTokenKind.Integer) || Match(OpenQasmTokenKind.Number))
        {
            OpenQasmToken token = Previous;

            if (!double.TryParse(
                    token.Text,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double value))
            {
                throw Error(token, $"Invalid number literal '{token.Text}'.");
            }

            return new OpenQasmAngleConstant(value, token.Line, token.Column);
        }

        if (Match(OpenQasmTokenKind.OpenParen))
        {
            OpenQasmAngleExpression expression = ParseAngleExpression();
            Consume(OpenQasmTokenKind.CloseParen, "Expected ')' after angle expression.");
            return expression;
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

    private OpenQasmTokenKind PeekKind(int offset)
    {
        int index = _position + offset;
        return index >= _tokens.Count
            ? OpenQasmTokenKind.EndOfFile
            : _tokens[index].Kind;
    }

    private OpenQasmBitDeclaration ParseBitDeclaration()
    {
        Consume(OpenQasmTokenKind.OpenBracket, "Expected '[' after bit.");
        int size = ParseIntegerLiteral();
        Consume(OpenQasmTokenKind.CloseBracket, "Expected ']' after bit size.");

        OpenQasmToken name = Consume(OpenQasmTokenKind.Identifier, "Expected bit register name.");
        Consume(OpenQasmTokenKind.Semicolon, "Expected ';' after bit declaration.");

        if (size <= 0)
            throw Error(name, "Bit register size must be positive.");

        return new OpenQasmBitDeclaration(
                    name.Text,
                    size,
                    name.Line,
                    name.Column);
    }

    private bool LooksLikeMeasurementAssignment()
    {
        int p = _position;

        return p + 6 < _tokens.Count
               && _tokens[p].Kind == OpenQasmTokenKind.Identifier
               && _tokens[p + 1].Kind == OpenQasmTokenKind.OpenBracket
               && _tokens[p + 2].Kind == OpenQasmTokenKind.Integer
               && _tokens[p + 3].Kind == OpenQasmTokenKind.CloseBracket
               && _tokens[p + 4].Kind == OpenQasmTokenKind.Equals
               && _tokens[p + 5].Kind == OpenQasmTokenKind.Measure;
    }

    private OpenQasmMeasureStatement ParseMeasurementAssignment()
    {
        OpenQasmBitReference bit = ParseBitReference();

        Consume(OpenQasmTokenKind.Equals, "Expected '=' in measurement assignment.");
        OpenQasmToken measureToken = Consume(OpenQasmTokenKind.Measure, "Expected 'measure' in measurement assignment.");

        OpenQasmQubitReference qubit = ParseQubitReference();

        Consume(OpenQasmTokenKind.Semicolon, "Expected ';' after measurement.");

        return new OpenQasmMeasureStatement(
            qubit,
            bit,
            measureToken.Line,
            measureToken.Column);
    }

    private OpenQasmMeasureStatement ParseLegacyMeasurement(OpenQasmToken measureToken)
    {
        OpenQasmQubitReference qubit = ParseQubitReference();

        Consume(OpenQasmTokenKind.Arrow, "Expected '->' after measured qubit.");

        OpenQasmBitReference bit = ParseBitReference();

        Consume(OpenQasmTokenKind.Semicolon, "Expected ';' after measurement.");

        return new OpenQasmMeasureStatement(
            qubit,
            bit,
            measureToken.Line,
            measureToken.Column);
    }

    private OpenQasmBitReference ParseBitReference()
    {
        OpenQasmToken name = Consume(OpenQasmTokenKind.Identifier, "Expected bit register name.");

        Consume(OpenQasmTokenKind.OpenBracket, "Expected '[' after bit register name.");
        int index = ParseIntegerLiteral();
        Consume(OpenQasmTokenKind.CloseBracket, "Expected ']' after bit index.");

        return new OpenQasmBitReference(name.Text, index);
    }

    private OpenQasmGateDefinitionStatement ParseGateDefinition()
    {
        OpenQasmToken name = Consume(OpenQasmTokenKind.Identifier, "Expected gate name.");

        var parameters = new List<string>();

        if (Match(OpenQasmTokenKind.OpenParen))
        {
            if (!Check(OpenQasmTokenKind.CloseParen))
            {
                do
                {
                    OpenQasmToken parameter = Consume(
                        OpenQasmTokenKind.Identifier,
                        "Expected gate parameter name.");

                    parameters.Add(parameter.Text);
                }
                while (Match(OpenQasmTokenKind.Comma));
            }

            Consume(OpenQasmTokenKind.CloseParen, "Expected ')' after gate parameter list.");
        }

        var qubitParameters = new List<string>();

        do
        {
            OpenQasmToken qubitParameter = Consume(
                OpenQasmTokenKind.Identifier,
                "Expected gate qubit argument name.");

            qubitParameters.Add(qubitParameter.Text);
        }
        while (Match(OpenQasmTokenKind.Comma));

        Consume(OpenQasmTokenKind.OpenBrace, "Expected '{' before gate body.");

        var body = new List<OpenQasmGateCallStatement>();

        while (!Check(OpenQasmTokenKind.CloseBrace))
        {
            if (Check(OpenQasmTokenKind.EndOfFile))
                throw Error(Current, "Unterminated gate definition body.");

            if (!StartsGateCall())
                throw Error(Current, "Only gate calls are currently supported inside gate definitions.");

            body.Add(ParseGateCall());
        }

        Consume(OpenQasmTokenKind.CloseBrace, "Expected '}' after gate body.");

        if (qubitParameters.Count != qubitParameters.Distinct().Count())
            throw Error(name, $"Gate '{name.Text}' contains duplicate qubit parameter names.");

        if (parameters.Count != parameters.Distinct().Count())
            throw Error(name, $"Gate '{name.Text}' contains duplicate angle parameter names.");

        return new OpenQasmGateDefinitionStatement(
                    name.Text,
                    parameters,
                    qubitParameters,
                    body,
                    name.Line,
                    name.Column);
    }

    private OpenQasmQubitOperand ParseQubitOperand()
    {
        OpenQasmToken name = Consume(OpenQasmTokenKind.Identifier, "Expected qubit name.");

        if (Match(OpenQasmTokenKind.OpenBracket))
        {
            int index = ParseIntegerLiteral();
            Consume(OpenQasmTokenKind.CloseBracket, "Expected ']' after qubit index.");

            return new OpenQasmQubitOperand(name.Text, index);
        }

        return new OpenQasmQubitOperand(name.Text, null);
    }

    private List<OpenQasmGateModifier> ParseGateModifiers()
    {
        var modifiers = new List<OpenQasmGateModifier>();

        while (true)
        {
            if (Match(OpenQasmTokenKind.Ctrl))
            {
                Consume(OpenQasmTokenKind.At, "Expected '@' after 'ctrl'.");
                modifiers.Add(new OpenQasmGateModifier(OpenQasmGateModifierKind.Ctrl));
                continue;
            }

            if (Match(OpenQasmTokenKind.Inv))
            {
                Consume(OpenQasmTokenKind.At, "Expected '@' after 'inv'.");
                modifiers.Add(new OpenQasmGateModifier(OpenQasmGateModifierKind.Inv));
                continue;
            }

            if (Match(OpenQasmTokenKind.Pow))
            {
                Consume(OpenQasmTokenKind.OpenParen, "Expected '(' after 'pow'.");
                OpenQasmAngleExpression argument = ParseAngleExpression();
                Consume(OpenQasmTokenKind.CloseParen, "Expected ')' after pow argument.");
                Consume(OpenQasmTokenKind.At, "Expected '@' after pow(...).");

                modifiers.Add(new OpenQasmGateModifier(
                    OpenQasmGateModifierKind.Pow,
                    argument));

                continue;
            }

            break;
        }

        return modifiers;
    }

    private bool StartsGateCall()
    {
        return Check(OpenQasmTokenKind.Identifier)
               || Check(OpenQasmTokenKind.Ctrl)
               || Check(OpenQasmTokenKind.Inv)
               || Check(OpenQasmTokenKind.Pow)
               || Check(OpenQasmTokenKind.NegCtrl);
    }

    private OpenQasmResetStatement ParseResetStatement(OpenQasmToken resetToken)
    {
        OpenQasmQubitReference qubit = ParseQubitReference();
        Consume(OpenQasmTokenKind.Semicolon, "Expected ';' after reset statement.");

        return new OpenQasmResetStatement(
            qubit,
            resetToken.Line,
            resetToken.Column);
    }

    private OpenQasmBarrierStatement ParseBarrierStatement(OpenQasmToken barrierToken)
    {
        var qubits = new List<OpenQasmQubitReference>();

        do
        {
            qubits.Add(ParseQubitReference());
        }
        while (Match(OpenQasmTokenKind.Comma));

        Consume(OpenQasmTokenKind.Semicolon, "Expected ';' after barrier statement.");

        return new OpenQasmBarrierStatement(
            qubits,
            barrierToken.Line,
            barrierToken.Column);
    }
}