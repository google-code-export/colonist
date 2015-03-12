This wiki is aim to group knowledege and equation of common used Cg/HLSL.

# Introduction #

1. Convert texture RGB tp normal direction.
2. Equation to do CrossProduct between two 3-d vectors.

# Details #
1.colorComponent = 0.5 **normalComponent + 0.5;
> normalComponent = 2** (colorComponent - 0.5);

2. Vector3 A = [a1, a2, a3]
> Vector3 B = [b1, b2, b3]
> A X B = [- a3b2, a3b1 - a1b3, a1b2 - a2b1](a2b3.md)