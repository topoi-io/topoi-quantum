namespace QuantumComputer.OpenQasm;

public abstract record OpenQasmAngleExpression(int Line, int Column)
{
    public abstract double Evaluate(IReadOnlyDictionary<string, double> parameters);
}

public sealed record OpenQasmAngleConstant(
    double Value,
    int Line,
    int Column) : OpenQasmAngleExpression(Line, Column)
{
    public override double Evaluate(IReadOnlyDictionary<string, double> parameters)
        => Value;
}

public sealed record OpenQasmAngleParameter(
    string Name,
    int Line,
    int Column) : OpenQasmAngleExpression(Line, Column)
{
    public override double Evaluate(IReadOnlyDictionary<string, double> parameters)
    {
        if (!parameters.TryGetValue(Name, out double value))
        {
            throw new OpenQasmParseException(
                $"Unknown angle parameter '{Name}'.",
                Line,
                Column);
        }

        return value;
    }
}

public sealed record OpenQasmAngleUnary(
    string Operator,
    OpenQasmAngleExpression Operand,
    int Line,
    int Column) : OpenQasmAngleExpression(Line, Column)
{
    public override double Evaluate(IReadOnlyDictionary<string, double> parameters)
    {
        double value = Operand.Evaluate(parameters);

        return Operator switch
        {
            "+" => value,
            "-" => -value,
            _ => throw new OpenQasmParseException(
                $"Unsupported unary angle operator '{Operator}'.",
                Line,
                Column)
        };
    }
}

public sealed record OpenQasmAngleBinary(
    OpenQasmAngleExpression Left,
    string Operator,
    OpenQasmAngleExpression Right,
    int Line,
    int Column) : OpenQasmAngleExpression(Line, Column)
{
    public override double Evaluate(IReadOnlyDictionary<string, double> parameters)
    {
        double left = Left.Evaluate(parameters);
        double right = Right.Evaluate(parameters);

        return Operator switch
        {
            "+" => left + right,
            "-" => left - right,
            "*" => left * right,
            "/" when right != 0.0 => left / right,
            "/" => throw new OpenQasmParseException(
                "Division by zero in angle expression.",
                Line,
                Column),

            _ => throw new OpenQasmParseException(
                $"Unsupported binary angle operator '{Operator}'.",
                Line,
                Column)
        };
    }
}