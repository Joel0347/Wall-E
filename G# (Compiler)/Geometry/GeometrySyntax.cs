using System.Drawing;
namespace G_Sharp;

public abstract class GeometrySyntax : ExpressionSyntax { }
public class Points : GeometrySyntax
{
    public override SyntaxKind Kind => SyntaxKind.PointToken;
    public float X { get; private set; }
    public float Y { get; private set; }

    public Points(float x, float y)
    {
        X = x;
        Y = y;
    }
}
public class Segment : GeometrySyntax
{
    public override SyntaxKind Kind => SyntaxKind.SegmentToken;
    public Points P1 { get; private set; }
    public Points P2 { get; private set; }

    public Segment(Points p1, Points p2)
    {
        P1 = p1;
        P2 = p2;
    }
    public static bool IsSegment(string s)
    {
        s = s.Trim();
        return s.StartsWith("segment ");
    }
}
public class Ray : GeometrySyntax
{
    public override SyntaxKind Kind => SyntaxKind.RayToken;
    public Points P1 { get; private set; }
    public Points P2 { get; private set; }

    public Points End;

    public Ray(Points p1, Points p2)
    {
        P1 = p1;
        P2 = p2;
    }
}
public class Line : GeometrySyntax
{
    public override SyntaxKind Kind => SyntaxKind.LineToken;
    public Points P1 { get; private set; }
    public Points P2 { get; private set; }

    public Line(Points p1, Points p2)
    {
        P1 = p1;
        P2 = p2;
    }
}
public class Circle : GeometrySyntax
{
    public override SyntaxKind Kind => SyntaxKind.CircleToken;
    public Points Center { get; private set; }
    public float Diameter { get; private set; }
    public float Radio { get; private set; }

    public Circle(Points center, float radio)
    {
        Center = center;
        Radio = radio;
        Diameter = 2 * radio;
    }
}
public class Arc : GeometrySyntax
{
    public override SyntaxKind Kind => SyntaxKind.ArcToken;
    public Points Center { get; private set; }
    public Points Left { get; private set; }
    public Points Right { get; private set; }
    public float Diameter { get; private set; }
    public float Radio { get; private set; }

    public Arc(Points center, Points left, Points right, float radio)
    {
        Center = center;
        Left = left;
        Right = right;
        Radio = radio;
        Diameter = 2 * radio;
    }
}

public class Measure : GeometrySyntax
{
    public override SyntaxKind Kind => SyntaxKind.MeasureToken;

    public Points P1 { get; }
    public Points P2 { get; }
    public float Value { get; }

    public Measure(Points p1, Points p2)
    {
        P1 = p1;
        P2 = p2;
        Value = Utilities.DistanceBetweenPoints(p1, p2);
    }
}