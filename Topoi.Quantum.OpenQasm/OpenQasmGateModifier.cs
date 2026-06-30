namespace Topoi.Quantum.OpenQasm;

public enum OpenQasmGateModifierKind
{
    Ctrl,
    Inv,
    Pow,
    NegCtrl
}

public sealed record OpenQasmGateModifier(
    OpenQasmGateModifierKind Kind,
    OpenQasmAngleExpression? Argument = null);