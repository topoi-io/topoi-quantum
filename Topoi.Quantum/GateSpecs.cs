namespace Topoi.Quantum;

internal readonly record struct GateSpec(int QubitCount, bool RequiresAngle);

internal static class GateSpecs
{
    public static GateSpec For(GateKind kind)
    {
        return kind switch
        {
            GateKind.X or GateKind.Y or GateKind.Z or GateKind.H or GateKind.S or GateKind.T
                or GateKind.I or GateKind.SX or GateKind.SXDG or GateKind.SDG or GateKind.TDG
                => new GateSpec(1, false),

            GateKind.RX or GateKind.RY or GateKind.RZ
                => new GateSpec(1, true),

            GateKind.CX or GateKind.CY or GateKind.CZ or GateKind.CH or GateKind.SWAP
                => new GateSpec(2, false),

            GateKind.CCX
                => new GateSpec(3, false),

            GateKind.CRX or GateKind.CRY or GateKind.CRZ or GateKind.CP
                => new GateSpec(2, true),

            _ => throw new NotSupportedException($"Unsupported gate kind: {kind}")
        };
    }
}