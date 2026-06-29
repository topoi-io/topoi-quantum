OPENQASM 3.1;
include "stdgates.inc";

gate phase(theta) a {
    rz(theta) a;
}

gate double_phase(theta) a {
    phase(theta) a;
    phase(theta) a;
}

qubit[1] q;

double_phase(pi / 4) q[0];