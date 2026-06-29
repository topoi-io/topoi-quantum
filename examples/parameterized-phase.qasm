OPENQASM 3.1;
include "stdgates.inc";

gate phase(theta) a {
    rz(theta) a;
}

qubit[1] q;

phase(pi / 2) q[0];