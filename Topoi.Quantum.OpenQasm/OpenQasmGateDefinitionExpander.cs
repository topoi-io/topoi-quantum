namespace Topoi.Quantum.OpenQasm;

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
                        definition.Line,
                        definition.Column);
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
                gateCall.Line,
                gateCall.Column);
        }

        if (gateCall.Parameters.Count != definition.Parameters.Count)
        {
            throw new OpenQasmParseException(
                $"Gate '{definition.Name}' expects {definition.Parameters.Count} parameter(s), " +
                $"but received {gateCall.Parameters.Count}.",
                gateCall.Line,
                gateCall.Column);
        }

        if (gateCall.Qubits.Count != definition.QubitParameters.Count)
        {
            throw new OpenQasmParseException(
                $"Gate '{definition.Name}' expects {definition.QubitParameters.Count} qubit argument(s), " +
                $"but received {gateCall.Qubits.Count}.",
                gateCall.Line,
                gateCall.Column);
        }

        var qubitMap = new Dictionary<string, OpenQasmQubitOperand>();

        for (int i = 0; i < definition.QubitParameters.Count; i++)
            qubitMap[definition.QubitParameters[i]] = gateCall.Qubits[i];

        var parameterMap = new Dictionary<string, OpenQasmAngleExpression>();

        for (int i = 0; i < definition.Parameters.Count; i++)
            parameterMap[definition.Parameters[i]] = gateCall.Parameters[i];

        expansionStack.Push(definition.Name);

        var expanded = new List<OpenQasmGateCallStatement>();

        foreach (OpenQasmGateCallStatement bodyCall in definition.Body)
        {
            OpenQasmGateCallStatement substituted =
                Substitute(bodyCall, qubitMap, parameterMap);

            IReadOnlyList<OpenQasmGateCallStatement> nested =
                ExpandGateCall(substituted, definitions, expansionStack);

            expanded.AddRange(nested);
        }

        expansionStack.Pop();

        return expanded;
    }

    private static OpenQasmGateCallStatement Substitute(
        OpenQasmGateCallStatement gateCall,
        IReadOnlyDictionary<string, OpenQasmQubitOperand> qubitMap,
        IReadOnlyDictionary<string, OpenQasmAngleExpression> parameterMap)
    {
        var substitutedQubits = new List<OpenQasmQubitOperand>();

        foreach (OpenQasmQubitOperand qubit in gateCall.Qubits)
        {
            if (qubit.Index is null &&
                qubitMap.TryGetValue(qubit.Name, out OpenQasmQubitOperand? replacement))
            {
                substitutedQubits.Add(replacement);
                continue;
            }

            substitutedQubits.Add(qubit);
        }

        var substitutedParameters = gateCall.Parameters
            .Select(parameter => SubstituteAngle(parameter, parameterMap))
            .ToArray();

        return gateCall with
        {
            Parameters = substitutedParameters,
            Qubits = substitutedQubits
        };
    }

    private static OpenQasmAngleExpression SubstituteAngle(
        OpenQasmAngleExpression expression,
        IReadOnlyDictionary<string, OpenQasmAngleExpression> parameterMap)
    {
        return expression switch
        {
            OpenQasmAngleConstant => expression,

            OpenQasmAngleParameter parameter
                when parameterMap.TryGetValue(parameter.Name, out OpenQasmAngleExpression? replacement)
                => replacement,

            OpenQasmAngleParameter
                => expression,

            OpenQasmAngleUnary unary
                => unary with
                {
                    Operand = SubstituteAngle(unary.Operand, parameterMap)
                },

            OpenQasmAngleBinary binary
                => binary with
                {
                    Left = SubstituteAngle(binary.Left, parameterMap),
                    Right = SubstituteAngle(binary.Right, parameterMap)
                },

            _ => throw new OpenQasmParseException(
                $"Unsupported angle expression '{expression.GetType().Name}'.",
                expression.Line,
                expression.Column)
        };
    }
}