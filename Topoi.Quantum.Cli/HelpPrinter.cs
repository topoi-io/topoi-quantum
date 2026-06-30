namespace Topoi.Quantum.Cli;

public static class HelpPrinter
{
    public static void PrintInteractiveHelp()
    {
        Console.WriteLine("Topoi Quantum interactive commands:");

        Console.WriteLine("  PRINT                       Show top amplitudes and probabilities");
        Console.WriteLine("  PROBS                       Show top basis probabilities without measurement");
        Console.WriteLine("  EXPECT <observable>         Expectation value of Pauli observable");
        Console.WriteLine("                             Examples: EXPECT Z 0, EXPECT X 1, EXPECT ZZ 0 1, EXPECT Z0 Z1");

        Console.WriteLine();

        Console.WriteLine("Single-qubit gates:");
        Console.WriteLine("  X <q>, Y <q>, Z <q>         Apply Pauli gates to qubit q");
        Console.WriteLine("  H <q>, S <q>, T <q>         Apply standard single-qubit gates to qubit q");
        Console.WriteLine("  RX <q> <theta>              Rotation about X on qubit q");
        Console.WriteLine("  RY <q> <theta>              Rotation about Y on qubit q");
        Console.WriteLine("  RZ <q> <theta>              Rotation about Z on qubit q");

        Console.WriteLine();

        Console.WriteLine("Controlled / entangling gates:");
        Console.WriteLine("  CX <c> <t>                  Controlled-X / CNOT: flip target t if control c is 1");
        Console.WriteLine("  CNOT <c> <t>                Alias for CX");
        Console.WriteLine("  CZ <c> <t>                  Controlled-Z: phase flip when c and t are both 1");
        Console.WriteLine("  SWAP <q1> <q2>              Swap two qubits");
        Console.WriteLine("  CCX <c1> <c2> <t>           Toffoli gate: flip target if both controls are 1");
        Console.WriteLine("  TOFFOLI <c1> <c2> <t>       Alias for CCX");
        Console.WriteLine("  CRX <c> <t> <theta>         Controlled RX rotation");
        Console.WriteLine("  CRY <c> <t> <theta>         Controlled RY rotation");
        Console.WriteLine("  CRZ <c> <t> <theta>         Controlled RZ rotation");

        Console.WriteLine();

        Console.WriteLine("Measurement / diagnostics:");
        Console.WriteLine("  MEASURE <q>                 Measure one qubit and partially collapse state");
        Console.WriteLine("  MEASUREALL                  Measure full computational basis and collapse state");
        Console.WriteLine("  SAMPLE <n>                  Repeated measurement sampling, restoring state each trial");
        Console.WriteLine("  NORM                        Show current state norm");
        Console.WriteLine("  NORMALIZE                   Manually renormalize the state vector");
        Console.WriteLine("  MEM                         Show estimated dense state-vector memory usage");

        Console.WriteLine();

        Console.WriteLine("Other:");
        Console.WriteLine("  RESET                       Reset to |00..0>");
        Console.WriteLine("  QRAND [k]                   Generate k random bits, default k = number of qubits");
        Console.WriteLine("  RUN <path>                  Run commands from a .qc script file");
        Console.WriteLine("  LOAD <path>                 Load gate operations from a .qc file into a QuantumCircuit");
        Console.WriteLine("  CIRCUIT                     Print the currently loaded circuit");
        Console.WriteLine("  DRAW                        Draw the currently loaded circuit");
        Console.WriteLine("  RUNCIRCUIT                  Execute the currently loaded circuit");
        Console.WriteLine("  CLEARCIRCUIT                Clear the currently loaded circuit");
        Console.WriteLine("  QUIT                        Exit");

        Console.WriteLine();
        Console.WriteLine("CLI examples:");
        Console.WriteLine("  tq --qubits 2 --run examples/bell.qc");
        Console.WriteLine("  tq --qubits 2 --circuit circuits/bell.qc --print --expect \"ZZ 0 1\"");
        Console.WriteLine("  tq --qasm examples/bell.qasm --draw --print --expect \"ZZ 0 1\"");

        Console.WriteLine();
        Console.WriteLine("Angle formats:");
        Console.WriteLine("  1.57079632679   pi   +pi/2   -pi/8   3*pi/4   -3*pi/4   0.5*pi");
    }

    public static void PrintCliHelp()
    {
        Console.WriteLine("Topoi Quantum CLI");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  tq [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --help, -h                  Show command-line help");
        Console.WriteLine("  --qubits <n>, -q <n>         Number of qubits to initialise, default 1");
        Console.WriteLine("  --qasm <path>                Load and run an OpenQASM 3 circuit file");
        Console.WriteLine("  --run <path>                 Run a full interpreter script and exit");
        Console.WriteLine("  --circuit <path>             Load and run a gate-only QuantumCircuit file and exit");
        Console.WriteLine("  --print-circuit              Print loaded circuit before execution");
        Console.WriteLine("  --draw                       Draw loaded circuit before execution");
        Console.WriteLine("  --print                      Print final state amplitudes and probabilities");
        Console.WriteLine("  --probs                      Print final basis-state probabilities");
        Console.WriteLine("  --expect \"observable\"        Print expectation value, e.g. --expect \"ZZ 0 1\"");
        Console.WriteLine("  --sample <n>                 Sample final state n times");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  tq --qubits 2 --run examples/bell.qc");
        Console.WriteLine("  tq --qubits 2 --circuit circuits/bell.qc --print-circuit --print");
        Console.WriteLine("  tq --qubits 2 --circuit circuits/bell.qc --expect \"ZZ 0 1\" --expect \"XX 0 1\"");
        Console.WriteLine("  tq --qubits 3 --circuit circuits/ghz3.qc --probs --sample 1000");
        Console.WriteLine("  tq --qubits 2 --circuit circuits/bell.qc --draw --expect \"ZZ 0 1\"");
        Console.WriteLine("  tq --qasm examples/bell.qasm --draw --print --expect \"ZZ 0 1\"");
    }
}