OPENQASM 3.1;
include "stdgates.inc";

gate half_phase(theta) a {
    rz(theta / 2) a;
}

qubit[1] q;

half_phase(pi) q[0];