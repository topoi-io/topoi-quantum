using System.Globalization;
using System.Text.RegularExpressions;

namespace QuantumComputer.Parsing;

public static class AngleParser
{
    public static double Parse(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Angle cannot be empty.", nameof(token));

        string normalized = token.ToLowerInvariant();

        if (TryParsePiExpression(normalized, out double theta))
            return theta;

        if (!double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out theta))
        {
            throw new ArgumentException(
                "Angle must be a number or a pi expression like pi/2, -pi/8, or 3*pi/4.");
        }

        return theta;
    }

    private static bool TryParsePiExpression(string s, out double value)
    {
        value = 0.0;

        s = s.Trim()
             .ToLowerInvariant()
             .Replace(" ", "");

        if (!s.Contains("pi", StringComparison.Ordinal))
            return false;

        try
        {
            return TryParsePiExpressionCore(s, out value);
        }
        catch
        {
            value = 0.0;
            return false;
        }
    }

    private static bool TryParsePiExpressionCore(string s, out double value)
    {
        value = 0.0;

        string[] frac = s.Split('/');

        if (frac.Length is < 1 or > 2)
            return false;

        if (string.IsNullOrWhiteSpace(frac[0]))
            return false;

        if (!TryParsePiNumerator(frac[0], out double numerator))
            return false;

        double denominator = 1.0;

        if (frac.Length == 2)
        {
            if (string.IsNullOrWhiteSpace(frac[1]))
                return false;

            if (!double.TryParse(frac[1], NumberStyles.Float, CultureInfo.InvariantCulture, out denominator))
                return false;

            if (denominator == 0.0)
                return false;
        }

        value = numerator / denominator;
        return true;
    }

    private static bool TryParsePiNumerator(string s, out double value)
    {
        value = 0.0;

        if (string.IsNullOrWhiteSpace(s))
            return false;

        if (s == "pi" || s == "+pi")
        {
            value = Math.PI;
            return true;
        }

        if (s == "-pi")
        {
            value = -Math.PI;
            return true;
        }

        s = Regex.Replace(
            s,
            @"^([+-]?\d+(?:\.\d+)?(?:[eE][+-]?\d+)?)pi$",
            "$1*pi");

        string[] mul = s.Split('*', StringSplitOptions.RemoveEmptyEntries);

        if (mul.Length != 2)
            return false;

        if (mul[0] == "pi")
        {
            if (!double.TryParse(mul[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double factor))
                return false;

            value = Math.PI * factor;
            return true;
        }

        if (mul[1] == "pi")
        {
            if (!double.TryParse(mul[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double factor))
                return false;

            value = factor * Math.PI;
            return true;
        }

        return false;
    }
}