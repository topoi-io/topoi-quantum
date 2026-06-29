OPENQASM 3.1;
include "stdgates.inc";

qubit[2] q;

h q[0];
ctrl @ x q[0], q[1];