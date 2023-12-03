using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G_Sharp;

public sealed class Ray : Figure, IEquatable<Ray>
{
    public override SyntaxKind Kind => SyntaxKind.RayToken;
    public Points P1 { get; private set; }
    public Points P2 { get; private set; }
    public float M { get; private set; }
    public float N { get; private set; }
    public override string ReturnType => "ray";

    public Points End;

    public Ray(Points p1, Points p2)
    {
        P1 = p1;
        P2 = p2;
        (M, N) = Utilities.LineEquation(P1, P2);
    }

    public override object Evaluate(Scope scope)
    {
        return new Ray(P1, P2);
    }

    public override bool Checker(Scope scope)
    {
        return true;
    }

    public bool Equals(Ray? other)
    {
        var sameLine = M.Equals(other!.M) && N.Equals(other!.N);
        return sameLine && P1.Equals(other!.P1);
    }

    public override bool Equals(object? obj) => Equals(obj as Ray);
    public override int GetHashCode() => P1.GetHashCode();
}
