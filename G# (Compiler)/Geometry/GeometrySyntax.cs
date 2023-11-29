using System.Drawing;
namespace G_Sharp;

public abstract class GeometrySyntax : ExpressionSyntax { }

public class Points : GeometrySyntax
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
}
public class Segment : GeometrySyntax
{
    public override SyntaxKind Kind => SyntaxKind.SegmentToken;
    public Points P1 { get; private set; }
    public Points P2 { get; private set; }
    public override string ReturnType => "segment";

    public Segment(Points p1, Points p2)
    {
        P1 = p1;
        P2 = p2;
    }

    public override object Evaluate(Scope scope)
    {
        return new Segment(P1, P2);
    }

    public override bool Checker(Scope scope)
    {
        return true;
    }
}
public class Ray : GeometrySyntax
{
    public override SyntaxKind Kind => SyntaxKind.RayToken;
    public Points P1 { get; private set; }
    public Points P2 { get; private set; }
    public override string ReturnType => "ray";

    public Points End;

    public Ray(Points p1, Points p2)
    {
        P1 = p1;
        P2 = p2;
    }

    public override object Evaluate(Scope scope)
    {
        return new Ray(P1, P2);
    }

    public override bool Checker(Scope scope)
    {
        return true;
    }
}
public class Line : GeometrySyntax
{
    public override SyntaxKind Kind => SyntaxKind.LineToken;
    public Points P1 { get; private set; }
    public Points P2 { get; private set; }
    public override string ReturnType => "line";

    public Line(Points p1, Points p2)
    {
        P1 = p1;
        P2 = p2;
    }

    public override object Evaluate(Scope scope)
    {
        return new Line(P1, P2);
    }

    public override bool Checker(Scope scope)
    {
        return true;
    }
}
public class Circle : GeometrySyntax
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
}
public class Arc : GeometrySyntax
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
}

public class Measure : GeometrySyntax
{
    public override SyntaxKind Kind => SyntaxKind.MeasureToken;

    public Points P1 { get; }
    public Points P2 { get; }
    public float Value { get; }
    public override string ReturnType => "measure";

    public Measure(Points p1, Points p2)
    {
        P1 = p1;
        P2 = p2;
        Value = Utilities.DistanceBetweenPoints(p1, p2);
    }

    public override object Evaluate(Scope scope)
    {
        return new Measure(P1, P2);
    }

    public override bool Checker(Scope scope)
    {
        return true;
    }
}