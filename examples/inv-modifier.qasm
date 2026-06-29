OPENQASM 3.1;
include "stdgates.inc";

qubit[1] q;

s q[0];
inv @ s q[0];