namespace QuantumComputer.OpenQasm;

public static class OpenQasmGateDefinitionExpander
{
    public static OpenQasmProgram Expand(OpenQasmProgram program)
    {
        if (program is null)
            throw new ArgumentNullException(nameof(program));

        var definitions = new Dictionary<string, OpenQasmGateDefinitionStatement>();
        var expandedStatements = new List<OpenQasmStatement>();

        foreach (OpenQasmStatement statement in program.Statements)
        {
            if (statement is OpenQasmGateDefinitionStatement definition)
            {
                if (definitions.ContainsKey(definition.Name))
                {
                    throw new OpenQasmParseException(
                        $"Duplicate gate definition '{definition.Name}'.",
                        1,
                        1);
                }

                if (definition.Parameters.Count != 0)
                {
                    throw new OpenQasmParseException(
                        $"Parameterized gate definitions are not supported yet: '{definition.Name}'.",
                        1,
                        1);
                }

                definitions[definition.Name] = definition;
                continue;
            }

            if (statement is OpenQasmGateCallStatement gateCall)
            {
                IReadOnlyList<OpenQasmGateCallStatement> expandedGateCalls =
                    ExpandGateCall(gateCall, definitions, new Stack<string>());

                expandedStatements.AddRange(expandedGateCalls);
                continue;
            }

            expandedStatements.Add(statement);
        }

        return new OpenQasmProgram(program.Version, expandedStatements);
    }

    private static IReadOnlyList<OpenQasmGateCallStatement> ExpandGateCall(
        OpenQasmGateCallStatement gateCall,
        IReadOnlyDictionary<string, OpenQasmGateDefinitionStatement> definitions,
        Stack<string> expansionStack)
    {
        if (!definitions.TryGetValue(gateCall.GateName, out OpenQasmGateDefinitionStatement? definition))
            return new[] { gateCall };

        if (expansionStack.Contains(definition.Name))
        {
            string path = string.Join(" -> ", expansionStack.Reverse().Append(definition.Name));

            throw new OpenQasmParseException(
                $"Recursive gate definition detected: {path}.",
                1,
                1);
        }

        if (gateCall.Parameters.Count != 0)
        {
            throw new OpenQasmParseException(
                $"Gate '{definition.Name}' does not take parameters.",
                1,
                1);
        }

        if (gateCall.Qubits.Count != definition.QubitParameters.Count)
        {
            throw new OpenQasmParseException(
                $"Gate '{definition.Name}' expects {definition.QubitParameters.Count} qubit argument(s), " +
                $"but received {gateCall.Qubits.Count}.",
                1,
                1);
        }

        var qubitMap = new Dictionary<string, OpenQasmQubitOperand>();

        for (int i = 0; i < definition.QubitParameters.Count; i++)
            qubitMap[definition.QubitParameters[i]] = gateCall.Qubits[i];

        expansionStack.Push(definition.Name);

        var expanded = new List<OpenQasmGateCallStatement>();

        foreach (OpenQasmGateCallStatement bodyCall in definition.Body)
        {
            OpenQasmGateCallStatement substituted = SubstituteQubits(bodyCall, qubitMap);

            IReadOnlyList<OpenQasmGateCallStatement> nested =
                ExpandGateCall(substituted, definitions, expansionStack);

            expanded.AddRange(nested);
        }

        expansionStack.Pop();

        return expanded;
    }

    private static OpenQasmGateCallStatement SubstituteQubits(
        OpenQasmGateCallStatement gateCall,
        IReadOnlyDictionary<string, OpenQasmQubitOperand> qubitMap)
    {
        var substitutedQubits = new List<OpenQasmQubitOperand>();

        foreach (OpenQasmQubitOperand qubit in gateCall.Qubits)
        {
            if (qubit.Index is null && qubitMap.TryGetValue(qubit.Name, out OpenQasmQubitOperand? replacement))
            {
                substitutedQubits.Add(replacement);
                continue;
            }

            substitutedQubits.Add(qubit);
        }

        return gateCall with
        {
            Qubits = substitutedQubits
        };
    }
}