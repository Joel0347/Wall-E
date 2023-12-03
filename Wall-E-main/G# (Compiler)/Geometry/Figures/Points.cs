using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G_Sharp;

public sealed class Points : Figure, IEquatable<Points>
{
    public override SyntaxKind Kind => SyntaxKind.PointToken;
    public float X { get; private set; }
    public float Y { get; private set; }
    public override string ReturnType => "point";

    public Points(float x, float y)
    {
        X = x;
        Y = y;
    }

    public override object Evaluate(Scope scope)
    {
        return new Points(X, Y);
    }

    public override bool Checker(Scope scope)
    {
        return true;
    }

    public bool Equals(Points? other)
    {
        return X.Equals(other!.X) && Y.Equals(other.Y);
    }

    public override bool Equals(object? obj) => Equals(obj as Points);

    public override int GetHashCode() => X.GetHashCode();
}
