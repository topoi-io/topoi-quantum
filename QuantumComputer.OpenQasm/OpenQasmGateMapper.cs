using QuantumComputer.Core;

namespace QuantumComputer.OpenQasm;

internal static class OpenQasmGateMapper
{
    public static GateOperation Map(OpenQasmGateCallStatement gateCall, int[] qubits)
    {
        string gate = gateCall.GateName;

        return gate switch
        {
            "id" => NoParamGate(gateCall, GateKind.I, qubits, 1),

            "x" => NoParamGate(gateCall, GateKind.X, qubits, 1),
            "y" => NoParamGate(gateCall, GateKind.Y, qubits, 1),
            "z" => NoParamGate(gateCall, GateKind.Z, qubits, 1),
            "h" => NoParamGate(gateCall, GateKind.H, qubits, 1),
            "s" => NoParamGate(gateCall, GateKind.S, qubits, 1),
            "t" => NoParamGate(gateCall, GateKind.T, qubits, 1),
            "sdg" => NoParamGate(gateCall, GateKind.SDG, qubits, 1),
            "tdg" => NoParamGate(gateCall, GateKind.TDG, qubits, 1),
            "sx" => NoParamGate(gateCall, GateKind.SX, qubits, 1),
            "sxdg" => NoParamGate(gateCall, GateKind.SXDG, qubits, 1),

            "rx" => OneParamGate(gateCall, GateKind.RX, qubits, 1),
            "ry" => OneParamGate(gateCall, GateKind.RY, qubits, 1),
            "rz" => OneParamGate(gateCall, GateKind.RZ, qubits, 1),

            "cx" => NoParamGate(gateCall, GateKind.CX, qubits, 2),
            "cy" => NoParamGate(gateCall, GateKind.CY, qubits, 2),
            "cz" => NoParamGate(gateCall, GateKind.CZ, qubits, 2),
            "ch" => NoParamGate(gateCall, GateKind.CH, qubits, 2),
            "swap" => NoParamGate(gateCall, GateKind.SWAP, qubits, 2),
            "ccx" => NoParamGate(gateCall, GateKind.CCX, qubits, 3),

            "crx" => OneParamGate(gateCall, GateKind.CRX, qubits, 2),
            "cry" => OneParamGate(gateCall, GateKind.CRY, qubits, 2),
            "crz" => OneParamGate(gateCall, GateKind.CRZ, qubits, 2),
            "cp" => OneParamGate(gateCall, GateKind.CP, qubits, 2),

            _ => throw new OpenQasmParseException(
                $"Unsupported OpenQASM gate '{gateCall.GateName}'.",
                1,
                1)
        };
    }

    private static GateOperation NoParamGate(
        OpenQasmGateCallStatement gateCall,
        GateKind kind,
        int[] qubits,
        int expectedQubits)
    {
        if (gateCall.Parameters.Count != 0)
            throw new OpenQasmParseException($"Gate '{gateCall.GateName}' does not take parameters.", 1, 1);

        if (qubits.Length != expectedQubits)
            throw new OpenQasmParseException($"Gate '{gateCall.GateName}' expects {expectedQubits} qubit(s).", 1, 1);

        return new GateOperation(kind, qubits);
    }

    private static GateOperation OneParamGate(
        OpenQasmGateCallStatement gateCall,
        GateKind kind,
        int[] qubits,
        int expectedQubits)
    {
        if (gateCall.Parameters.Count != 1)
            throw new OpenQasmParseException($"Gate '{gateCall.GateName}' expects one parameter.", 1, 1);

        if (qubits.Length != expectedQubits)
            throw new OpenQasmParseException($"Gate '{gateCall.GateName}' expects {expectedQubits} qubit(s).", 1, 1);

        return new GateOperation(kind, qubits, gateCall.Parameters[0]);
    }
}