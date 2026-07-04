using NUnit.Framework;
using Topoi.Quantum.Parsing;

namespace Topoi.Quantum.Tests;

[TestFixture]
public sealed class GateOperationParserTests
{
    private static readonly object[] OneQubitNoAngleCases =
    {
        new object[] { "I 0", GateKind.I },
        new object[] { "SDG 0", GateKind.SDG },
        new object[] { "TDG 0", GateKind.TDG },
        new object[] { "SX 0", GateKind.SX },
        new object[] { "SXDG 0", GateKind.SXDG }
    };

    [TestCaseSource(nameof(OneQubitNoAngleCases))]
    public void TryParse_OneQubitMissingGates_Parses(string command, GateKind expectedKind)
    {
        bool ok = GateOperationParser.TryParse(
            command,
            out GateOperation? operation,
            out string? error);

        Assert.That(ok, Is.True, error);
        Assert.That(operation, Is.Not.Null);
        Assert.That(operation!.Kind, Is.EqualTo(expectedKind));
        Assert.That(operation.Qubits, Is.EqualTo(new[] { 0 }));
        Assert.That(operation.Angle, Is.Null);
    }

    private static readonly object[] TwoQubitNoAngleCases =
    {
        new object[] { "CY 0 1", GateKind.CY },
        new object[] { "CH 0 1", GateKind.CH }
    };

    [TestCaseSource(nameof(TwoQubitNoAngleCases))]
    public void TryParse_TwoQubitMissingGates_Parses(string command, GateKind expectedKind)
    {
        bool ok = GateOperationParser.TryParse(
            command,
            out GateOperation? operation,
            out string? error);

        Assert.That(ok, Is.True, error);
        Assert.That(operation, Is.Not.Null);
        Assert.That(operation!.Kind, Is.EqualTo(expectedKind));
        Assert.That(operation.Qubits, Is.EqualTo(new[] { 0, 1 }));
        Assert.That(operation.Angle, Is.Null);
    }

    [Test]
    public void TryParse_CP_ParsesControlTargetAndAngle()
    {
        bool ok = GateOperationParser.TryParse(
            "CP 0 1 pi/2",
            out GateOperation? operation,
            out string? error);

        Assert.That(ok, Is.True, error);
        Assert.That(operation, Is.Not.Null);
        Assert.That(operation!.Kind, Is.EqualTo(GateKind.CP));
        Assert.That(operation.Qubits, Is.EqualTo(new[] { 0, 1 }));
        Assert.That(operation.Angle, Is.EqualTo(Math.PI / 2).Within(TestHelpers.Tolerance));
    }

    [Test]
    public void TryParse_MissingGateNames_AreCaseInsensitive()
    {
        bool ok = GateOperationParser.TryParse(
            "sxdg 0",
            out GateOperation? operation,
            out string? error);

        Assert.That(ok, Is.True, error);
        Assert.That(operation!.Kind, Is.EqualTo(GateKind.SXDG));
    }
}