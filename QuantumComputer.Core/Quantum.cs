using System.Numerics;

namespace QuantumComputer.Core;

public static class Quantum
{
    private static QuantumSimulator _simulator = new(1);

    public static QuantumSimulator DefaultSimulator => _simulator;

    public static QuantumRegister Register => _simulator.Register;

    public static void Init(int n)
    {
        _simulator = new QuantumSimulator(n);
    }

    public static void Init(int n, IRandomSource randomSource)
    {
        _simulator = new QuantumSimulator(n, randomSource);
    }

    public static void Reset() => _simulator.Reset();

    public static Complex ExpectPauliString(IReadOnlyList<PauliTerm> terms)
        => _simulator.ExpectPauliString(terms);

    public static void X(int t) => _simulator.X(t);
    public static void Y(int t) => _simulator.Y(t);
    public static void Z(int t) => _simulator.Z(t);
    public static void H(int t) => _simulator.H(t);
    public static void S(int t) => _simulator.S(t);
    public static void T(int t) => _simulator.T(t);

    public static void RX(int t, double theta) => _simulator.RX(t, theta);
    public static void RY(int t, double theta) => _simulator.RY(t, theta);
    public static void RZ(int t, double theta) => _simulator.RZ(t, theta);

    public static void CX(int control, int target) => _simulator.CX(control, target);
    public static void CNOT(int control, int target) => _simulator.CNOT(control, target);
    public static void CZ(int control, int target) => _simulator.CZ(control, target);
    public static void SWAP(int q1, int q2) => _simulator.SWAP(q1, q2);
    public static void CCX(int control1, int control2, int target) => _simulator.CCX(control1, control2, target);
    public static void TOFFOLI(int control1, int control2, int target) => _simulator.TOFFOLI(control1, control2, target);

    public static void CRX(int control, int target, double theta) => _simulator.CRX(control, target, theta);
    public static void CRY(int control, int target, double theta) => _simulator.CRY(control, target, theta);
    public static void CRZ(int control, int target, double theta) => _simulator.CRZ(control, target, theta);

    public static void I(int t) => _simulator.I(t);
    public static void SDG(int t) => _simulator.SDG(t);
    public static void TDG(int t) => _simulator.TDG(t);
    public static void SX(int t) => _simulator.SX(t);
    public static void SXDG(int t) => _simulator.SXDG(t);

    public static void CY(int control, int target) => _simulator.CY(control, target);
    public static void CH(int control, int target) => _simulator.CH(control, target);
    public static void CP(int control, int target, double theta) => _simulator.CP(control, target, theta);

    public static int MeasureAll() => _simulator.MeasureAll();
    public static int Measure(int q) => _simulator.Measure(q);

    public static Complex[] SnapshotState() => _simulator.SnapshotState();
    public static void RestoreState(Complex[] snapshot) => _simulator.RestoreState(snapshot);

    public static void Normalize() => _simulator.Normalize();
}