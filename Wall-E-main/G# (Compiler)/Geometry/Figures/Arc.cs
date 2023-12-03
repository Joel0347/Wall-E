using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G_Sharp;

public sealed class Arc : Figure, IEquatable<Arc>
{
    public override SyntaxKind Kind => SyntaxKind.ArcToken;
    public Points Center { get; private set; }
    public Points Left { get; private set; }
    public Points Right { get; private set; }
    public float Diameter => Radio * 2;
    public float Radio => Measure.Value;
    public Measure Measure { get; }
    public override string ReturnType => "arc";

    public Arc(Points center, Points left, Points right, Measure measure)
    {
        Center = center;
        Left = left;
        Right = right;
        Measure = measure;
    }

    public override object Evaluate(Scope scope)
    {
        return new Arc(Center, Left, Right, Measure);
    }

    public override bool Checker(Scope scope)
    {
        return true;
    }

    public bool Equals(Arc? other)
    {
        var samePoints = Center.Equals(other!.Center) && Left.Equals(other!.Left) && Right.Equals(other!.Right);
        return samePoints && Measure.Equals(other.Measure);
    }

    public override bool Equals(object? obj) => Equals(obj as Arc);
    public override int GetHashCode() => Center.GetHashCode();
}

