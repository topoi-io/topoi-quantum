namespace Topoi.Quantum.Parsing;

public static class CircuitFileLoader
{
    public static QuantumCircuit Load(
        string path,
        int qubitCount,
        string? baseDirectory = null)
    {
        string resolvedPath = ResolveInputPath(path, baseDirectory);

        if (!File.Exists(resolvedPath))
            throw new FileNotFoundException($"Circuit file not found: {resolvedPath}");

        var circuit = new QuantumCircuit(qubitCount);

        string[] lines = File.ReadAllLines(resolvedPath);

        for (int i = 0; i < lines.Length; i++)
        {
            string line = StripComment(lines[i]).Trim();

            if (line.Length == 0)
                continue;

            string[] lineParts = line.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            string cmd = lineParts[0].ToUpperInvariant();

            if (cmd == "RESET")
                continue;

            if (!GateOperationParser.TryParse(lineParts, out GateOperation? operation, out string? error))
            {
                throw new InvalidOperationException(
                    $"Line {i + 1}: cannot load '{line}' as a circuit operation. {error}");
            }

            circuit.Add(operation!);
        }

        return circuit;
    }

    public static string ResolveInputPath(string path, string? baseDirectory = null)
    {
        path = path.Trim();

        if ((path.StartsWith('"') && path.EndsWith('"')) ||
            (path.StartsWith('\'') && path.EndsWith('\'')))
        {
            path = path[1..^1];
        }

        if (Path.IsPathRooted(path))
            return path;

        string startDirectory = baseDirectory ?? Directory.GetCurrentDirectory();

        string currentDirectoryPath = Path.GetFullPath(Path.Combine(startDirectory, path));

        if (File.Exists(currentDirectoryPath))
            return currentDirectoryPath;

        string appBasePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, path));

        if (File.Exists(appBasePath))
            return appBasePath;

        string? projectPath = FindUpwardsForFile(startDirectory, path);

        if (projectPath is not null)
            return projectPath;

        projectPath = FindUpwardsForFile(AppContext.BaseDirectory, path);

        if (projectPath is not null)
            return projectPath;

        return currentDirectoryPath;
    }

    private static string? FindUpwardsForFile(string startDirectory, string relativePath)
    {
        DirectoryInfo? directory = new DirectoryInfo(startDirectory);

        while (directory is not null)
        {
            string candidate = Path.GetFullPath(Path.Combine(directory.FullName, relativePath));

            if (File.Exists(candidate))
                return candidate;

            directory = directory.Parent;
        }

        return null;
    }

    private static string StripComment(string line)
    {
        int hash = line.IndexOf('#');

        if (hash < 0)
            return line;

        return line[..hash];
    }
}