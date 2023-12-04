using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G_Sharp;

public sealed class Segment : Figure, IEquatable<Segment>
{
    public override SyntaxKind Kind => SyntaxKind.SegmentToken;
    public Points P1 { get; private set; }
    public Points P2 { get; private set; }
    public float M { get; }
    public float N { get; }
    public override string ReturnType => "segment";

    public Segment(Points p1, Points p2)
    {
        P1 = p1;
        P2 = p2;

        (M, N) = Utilities.LineEquation(P1, P2);
    }

    public override object Evaluate(Scope scope)
    {
        return new Segment(P1, P2);
    }

    public override bool Checker(Scope scope)
    {
        return true;
    }

    public bool Equals(Segment? other)
    {
        var thisMeasure = Utilities.DistanceBetweenPoints(P1, P2);
        var otherMeasure = Utilities.DistanceBetweenPoints(other!.P1, other.P2);
        return thisMeasure == otherMeasure;
    }

    public override bool Equals(object? obj) => Equals(obj as Segment);
    public override int GetHashCode() => P1.GetHashCode();

    public override SequenceExpressionSyntax PointsInFigure()
    {
        Dictionary<int, object> elements = new();
        object PointsInSegment()
        {
            float x;
            float y;
            do
            {
                x = ParsingSupplies.CreateRandomsCoordinates();
                y = Utilities.PointInLine(M, N, x);
            }
            while (!Utilities.IsInSegment(P1.X, P2.X, x) || !Utilities.IsInSegment(P1.Y, P2.Y, y));

            return new Points(x, y);
        }

        return new InfiniteSequence(PointsInSegment, elements);
    }
}