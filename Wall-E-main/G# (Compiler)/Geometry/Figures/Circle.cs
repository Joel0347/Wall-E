using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G_Sharp;

public sealed class Circle : Figure, IEquatable<Circle>
{
    public override SyntaxKind Kind => SyntaxKind.CircleToken;
    public Points Center { get; private set; }
    public float Diameter => Radio * 2;
    public float Radio => Measure.Value;
    public Measure Measure { get; }
    public override string ReturnType => "circle";

    public Circle(Points center, Measure measure)
    {
        Center = center;
        Measure = measure;
    }

    public override object Evaluate(Scope scope)
    {
        return new Circle(Center, Measure);
    }

    public override bool Checker(Scope scope)
    {
        return true;
    }

    public bool Equals(Circle? other)
    {
        return Center.Equals(other!.Center) && Measure.Equals(other!.Measure);
    }

    public override bool Equals(object? obj) => Equals(obj as Circle);
    public override int GetHashCode() => Center.GetHashCode();
}
