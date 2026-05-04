namespace QuantumComputer;

public static class ObservableParser
{
    public static PauliTerm[] Parse(string observable)
    {
        if (string.IsNullOrWhiteSpace(observable))
        {
            throw new ArgumentException(
                "Usage: EXPECT <observable>. Examples: EXPECT Z 0, EXPECT X 1, EXPECT ZZ 0 1, EXPECT Z0 Z1",
                nameof(observable));
        }

        string[] parts = observable.Split(
            ' ',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return Parse(parts, 0);
    }

    public static PauliTerm[] Parse(string[] parts, int index)
    {
        if (parts.Length <= index)
        {
            throw new ArgumentException(
                "Usage: EXPECT <observable>. Examples: EXPECT Z 0, EXPECT X 1, EXPECT ZZ 0 1, EXPECT Z0 Z1");
        }

        var terms = new List<PauliTerm>();
        int i = index;

        while (i < parts.Length)
        {
            string token = parts[i].Trim().ToUpperInvariant();

            // Compact form: Z0, X1, Y2, I3
            if (TryParsePauliWithIndex(token, out char compactPauli, out int compactQubit))
            {
                terms.Add(new PauliTerm(compactPauli, compactQubit));
                i++;
                continue;
            }

            // Separated form: Z 0
            // Grouped form: ZZ 0 1, ZX 0 1
            if (IsPauliLetters(token))
            {
                if (token.Length == 1)
                {
                    if (i + 1 >= parts.Length)
                        throw new ArgumentException($"Missing qubit index after {token}. Example: EXPECT {token} 0");

                    int q = ParseQubit(parts, i + 1);
                    terms.Add(new PauliTerm(token[0], q));
                    i += 2;
                    continue;
                }

                if (i + token.Length >= parts.Length)
                    throw new ArgumentException($"Observable {token} needs {token.Length} qubit indices.");

                for (int k = 0; k < token.Length; k++)
                {
                    int q = ParseQubit(parts, i + 1 + k);
                    terms.Add(new PauliTerm(token[k], q));
                }

                i += 1 + token.Length;
                continue;
            }

            throw new ArgumentException(
                $"Bad observable token '{parts[i]}'. Use forms like EXPECT Z 0, EXPECT ZZ 0 1, or EXPECT Z0 Z1.");
        }

        if (terms.Count == 0)
            throw new ArgumentException("No observable supplied.");

        return terms.ToArray();
    }

    public static string Format(IReadOnlyList<PauliTerm> terms)
    {
        return string.Join(" ⊗ ", terms.Select(t => $"{t.Pauli}{t.Qubit}"));
    }

    private static int ParseQubit(string[] parts, int index)
    {
        if (parts.Length <= index)
            throw new ArgumentException("Missing qubit index. Example: H 0");

        if (!int.TryParse(parts[index], out int q) || q < 0)
            throw new ArgumentException("Qubit index must be a non-negative integer.");

        return q;
    }

    private static bool IsPauliLetters(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        foreach (char c in token)
        {
            if (c is not ('I' or 'X' or 'Y' or 'Z'))
                return false;
        }

        return true;
    }

    private static bool TryParsePauliWithIndex(string token, out char pauli, out int qubit)
    {
        pauli = '\0';
        qubit = -1;

        if (token.Length < 2)
            return false;

        char p = token[0];

        if (p is not ('I' or 'X' or 'Y' or 'Z'))
            return false;

        string qText = token[1..];

        if (!int.TryParse(qText, out int q) || q < 0)
            return false;

        pauli = p;
        qubit = q;

        return true;
    }
}