using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallE;

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
    public float StartAngle { get; }
    public float SweepAngle { get; }
    public override string ReturnType => "arc";

    public Arc(Points center, Points left, Points right, Measure measure)
    {
        Center = center;
        Left = left;
        Right = right;
        Measure = measure;

        (StartAngle, SweepAngle) = Components();
    }

    public override object Evaluate(Scope scope)
    {
        return new Arc(Center, Left, Right, Measure);
    }

    public override bool Check(Scope scope)
    {
        return true;
    }

    public bool Equals(Arc? other)
    {
        var sameAngle = SweepAngle == other!.SweepAngle;
        return sameAngle && Measure.Equals(other.Measure);
    }

    public override bool Equals(object? obj) => Equals(obj as Arc);
    public override int GetHashCode() => Center.GetHashCode();

    private (float, float) Components()
    {
        Ray r1 = new(Center, Left);
        Ray r2 = new(Center, Right);

        Points point_left = Utilities.IntersectionCircle_Ray(Center, r1.End, Radio, r1.M, r1.N);
        Points point_right = Utilities.IntersectionCircle_Ray(Center, r2.End, Radio, r2.M, r2.N);

        Points cero = new(Center.X + Radio, Center.Y);

        float sweepAngle = Utilities.AngleBetweenVectors(Center, point_left, point_right);
        float angleLeftCero = Utilities.AngleBetweenVectors(Center, point_left, cero);
        float angleRightCero = Utilities.AngleBetweenVectors(Center, point_right, cero);
        float startAngle = 0;

        int quadrant_left = Utilities.GetAngleQuadrant(Center, Radio, point_left);
        int quadrant_right = Utilities.GetAngleQuadrant(Center, Radio, point_right);

        if (quadrant_left == quadrant_right)
        {
            startAngle = quadrant_left < 3 ? Math.Min(angleLeftCero, angleRightCero)
                                             : -Math.Max(angleLeftCero, angleRightCero);
        }
        else
        {
            if (quadrant_left == 1 || quadrant_right == 1)
            {
                startAngle = quadrant_left == 1 ? angleLeftCero : angleRightCero;
                float m = quadrant_left == 1 ? r1.M : r2.M;
                float n = quadrant_left == 1 ? r1.N : r2.N;
                Points point = quadrant_left != 1 ? point_left : point_right;
                float evaluationInRay = Utilities.PointInLine(m, n, point.X);

                if (point.Y <= evaluationInRay)
                {
                    sweepAngle = 360 - sweepAngle;
                }
            }

            else if (quadrant_left == 2 || quadrant_right == 2)
            {
                startAngle = quadrant_left == 2 ? angleLeftCero : angleRightCero;
                float m = quadrant_left == 2 ? r1.M : r2.M;
                float n = quadrant_left == 2 ? r1.N : r2.N;
                Points point = quadrant_left != 2 ? point_left : point_right;
                float evaluationInRay = Utilities.PointInLine(m, n, point.X);

                if (point.Y > evaluationInRay)
                {
                    sweepAngle = 360 - sweepAngle;
                }
            }

            else if (quadrant_left == 3 || quadrant_right == 3)
            {
                startAngle = quadrant_left == 3 ? -angleLeftCero : -angleRightCero;
            }
        }

        return (startAngle, sweepAngle);
    }

    public override SequenceExpressionSyntax PointsInFigure()
    {
        Dictionary<int, object> elements = new();
        object PointsInArc()
        {
            float x;
            float y;
            do
            {
                x = ParsingSupplies.CreateRandomsCoordinates();
                y = ParsingSupplies.CreateRandomsCoordinates();
            }
            while (((Math.Pow(x, 2) + Math.Pow(y, 2)) <= Radio));

            return new Points(x, y);
        }

        var result = new InfiniteSequence(PointsInArc, elements);
        result.valuesType = "point";

        return result;
    }
}

