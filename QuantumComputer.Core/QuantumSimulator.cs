using System.Numerics;

namespace QuantumComputer.Core;

public sealed class QuantumSimulator
{
    private const long NormalizeEveryUnitaryGates = 4096;

    private long _unitaryGateCount;

    public QuantumRegister Register { get; }

    public QuantumSimulator(int qubitCount, IRandomSource? randomSource = null)
    {
        Register = new QuantumRegister(qubitCount, randomSource);
    }

    public void Reset()
    {
        Register.Reset();
        _unitaryGateCount = 0;
    }

    public Complex ExpectPauliString(IReadOnlyList<PauliTerm> terms)
        => Register.ExpectPauliString(terms);

    public void Apply1(int t, Complex m00, Complex m01, Complex m10, Complex m11)
    {
        if ((uint)t >= (uint)Register.QubitCount)
            throw new ArgumentOutOfRangeException(nameof(t), $"Qubit index out of range. Must be 0..{Register.QubitCount - 1}");

        int stride = 1 << t;
        int step = stride << 1;
        var v = Register.MutableState;

        for (int block = 0; block < v.Length; block += step)
        {
            for (int i = 0; i < stride; i++)
            {
                int i0 = block + i;
                int i1 = i0 + stride;

                Complex a = v[i0];
                Complex b = v[i1];

                v[i0] = m00 * a + m01 * b;
                v[i1] = m10 * a + m11 * b;
            }
        }

        AfterUnitaryGate();
    }

    public void X(int t) => Apply1(t, 0, 1, 1, 0);

    public void Y(int t) => Apply1(t, 0, -Complex.ImaginaryOne, Complex.ImaginaryOne, 0);

    public void Z(int t) => Apply1(t, 1, 0, 0, -1);

    public void H(int t)
    {
        double invSqrt2 = 1.0 / Math.Sqrt(2.0);
        Apply1(t, invSqrt2, invSqrt2, invSqrt2, -invSqrt2);
    }

    public void S(int t) => Apply1(t, 1, 0, 0, Complex.ImaginaryOne);

    public void T(int t)
    {
        Complex phase = Complex.Exp(Complex.ImaginaryOne * (Math.PI / 4.0));
        Apply1(t, 1, 0, 0, phase);
    }

    public void RX(int t, double theta)
    {
        double c = Math.Cos(theta / 2.0);
        double s = Math.Sin(theta / 2.0);
        Complex minusIS = -Complex.ImaginaryOne * s;

        Apply1(t, c, minusIS, minusIS, c);
    }

    public void RY(int t, double theta)
    {
        double c = Math.Cos(theta / 2.0);
        double s = Math.Sin(theta / 2.0);

        Apply1(t, c, -s, s, c);
    }

    public void RZ(int t, double theta)
    {
        Complex p0 = Complex.Exp(-Complex.ImaginaryOne * (theta / 2.0));
        Complex p1 = Complex.Exp(Complex.ImaginaryOne * (theta / 2.0));

        Apply1(t, p0, 0, 0, p1);
    }

    public void CX(int control, int target)
    {
        Controlled1(control, target, 0, 1, 1, 0);
    }

    public void CNOT(int control, int target)
    {
        CX(control, target);
    }

    public void CZ(int control, int target)
    {
        Register.ApplyCZ(control, target);
        AfterUnitaryGate();
    }

    public void SWAP(int q1, int q2)
    {
        Register.ApplySWAP(q1, q2);
        AfterUnitaryGate();
    }

    public void CCX(int control1, int control2, int target)
    {
        Controlled1(new[] { control1, control2 }, target, 0, 1, 1, 0);
    }

    public void TOFFOLI(int control1, int control2, int target)
    {
        CCX(control1, control2, target);
    }

    public void CRX(int control, int target, double theta)
    {
        double c = Math.Cos(theta / 2.0);
        double s = Math.Sin(theta / 2.0);
        Complex minusIS = -Complex.ImaginaryOne * s;

        Controlled1(control, target, c, minusIS, minusIS, c);
    }

    public void CRY(int control, int target, double theta)
    {
        double c = Math.Cos(theta / 2.0);
        double s = Math.Sin(theta / 2.0);

        Controlled1(control, target, c, -s, s, c);
    }

    public void CRZ(int control, int target, double theta)
    {
        Complex p0 = Complex.Exp(-Complex.ImaginaryOne * (theta / 2.0));
        Complex p1 = Complex.Exp(Complex.ImaginaryOne * (theta / 2.0));

        Controlled1(control, target, p0, 0, 0, p1);
    }

    public void Controlled1(
        int control,
        int target,
        Complex m00,
        Complex m01,
        Complex m10,
        Complex m11)
    {
        Register.ApplyControlled1(
            new[] { control },
            target,
            m00,
            m01,
            m10,
            m11);

        AfterUnitaryGate();
    }

    public void Controlled1(
        IReadOnlyList<int> controls,
        int target,
        Complex m00,
        Complex m01,
        Complex m10,
        Complex m11)
    {
        Register.ApplyControlled1(
            controls,
            target,
            m00,
            m01,
            m10,
            m11);

        AfterUnitaryGate();
    }

    public int MeasureAll()
    {
        int result = Register.MeasureAll();
        _unitaryGateCount = 0;
        return result;
    }

    public int Measure(int q)
    {
        int result = Register.MeasureQubit(q);
        _unitaryGateCount = 0;
        return result;
    }

    public Complex[] SnapshotState()
        => (Complex[])Register.MutableState.Clone();

    public void RestoreState(Complex[] snapshot)
    {
        if (snapshot.Length != Register.MutableState.Length)
            throw new ArgumentException("Snapshot size does not match register.");

        Array.Copy(snapshot, Register.MutableState, snapshot.Length);

#if DEBUG
        Register.AssertNormalized();
#endif

        _unitaryGateCount = 0;
    }

    public void Normalize()
    {
        Register.Normalize();
        _unitaryGateCount = 0;
    }

    public void I(int t)
    {
        if ((uint)t >= (uint)Register.QubitCount)
            throw new ArgumentOutOfRangeException(nameof(t), $"Qubit index out of range. Must be 0..{Register.QubitCount - 1}");

        // Identity gate intentionally changes nothing, but still counts as a unitary operation.
        AfterUnitaryGate();
    }

    public void SDG(int t)
    {
        Apply1(t, 1, 0, 0, -Complex.ImaginaryOne);
    }

    public void TDG(int t)
    {
        Complex phase = Complex.Exp(-Complex.ImaginaryOne * (Math.PI / 4.0));
        Apply1(t, 1, 0, 0, phase);
    }

    public void SX(int t)
    {
        // sqrt(X) = 1/2 [[1+i, 1-i], [1-i, 1+i]]
        Complex a = new Complex(0.5, 0.5);
        Complex b = new Complex(0.5, -0.5);

        Apply1(t, a, b, b, a);
    }

    public void SXDG(int t)
    {
        // inverse sqrt(X) = 1/2 [[1-i, 1+i], [1+i, 1-i]]
        Complex a = new Complex(0.5, -0.5);
        Complex b = new Complex(0.5, 0.5);

        Apply1(t, a, b, b, a);
    }

    public void CY(int control, int target)
    {
        Controlled1(control, target, 0, -Complex.ImaginaryOne, Complex.ImaginaryOne, 0);
    }

    public void CH(int control, int target)
    {
        double invSqrt2 = 1.0 / Math.Sqrt(2.0);
        Controlled1(control, target, invSqrt2, invSqrt2, invSqrt2, -invSqrt2);
    }

    public void CP(int control, int target, double theta)
    {
        Register.ApplyControlledPhase(control, target, theta);
        AfterUnitaryGate();
    }

    private void AfterUnitaryGate()
    {
        _unitaryGateCount++;

#if DEBUG
        Register.AssertNormalized();
#endif

        if (_unitaryGateCount % NormalizeEveryUnitaryGates == 0)
            Register.Normalize();
    }
}