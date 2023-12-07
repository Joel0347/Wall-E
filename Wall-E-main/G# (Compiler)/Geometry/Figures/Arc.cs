using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
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
    public float AngleLeft { get; }
    public float AngleRight { get; }
    public Points PointLeft { get; }
    public Points PointRight { get; }
    public override string ReturnType => "arc";

    public Arc(Points center, Points left, Points right, Measure measure)
    {
        Center = center;
        Left = left;
        Right = right;
        Measure = measure;

        (StartAngle, SweepAngle, PointLeft, PointRight, AngleLeft, AngleRight) = Components();
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

    private (float, float, Points, Points, float, float) Components()
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

        return (startAngle, sweepAngle, point_left, point_right, angleLeftCero, angleRightCero);
    }

    public override SequenceExpressionSyntax PointsInFigure()
    {
        Dictionary<int, object> elements = new();
        
        object PointsInArc()
        {
            var min = Center.X - Radio - 1;
            var max = Center.X + Radio + 1;

            do
            {
                float x = ParsingSupplies.CreateRandomsCoordinates((int)min, (int)max);
                
                (float y1, float y2) = IsInCircle(x);
                var point1 = new Points(x, y1);
                var point2 = new Points(x, y2);
                var intersect1 = Intersect.IntersectPointArc(point1, this);
                var intersect2 = Intersect.IntersectPointArc(point2, this);

                if ((int)(intersect1.Count) > 0)
                    return point1;

                else if ((int)(intersect2.Count) > 0)
                    return point2;
            }
            while (true);
        }

        return new InfiniteSequence(PointsInArc, elements);
    }

    //private Points IsInArc(float x)
    //{
    //    (float y1, float y2) = IsInCircle(x);
    //    Points point = new(x, y1);

    //    var startPoint = (Math.Abs(StartAngle) == AngleLeft) ? PointLeft : PointRight;
    //    var endPoint = (Math.Abs(StartAngle) != AngleLeft) ? PointLeft : PointRight;

    //    var startAngle = Math.Atan2(startPoint.Y - Center.Y, startPoint.X - Center.X);
    //    var endAngle = Math.Atan2(endPoint.Y - Center.Y, endPoint.X - Center.X);
    //    var pointAngle = Math.Atan2(point.Y - Center.Y, point.X - Center.X);

    //    if (pointAngle >= startAngle && pointAngle <= endAngle)
    //        return point;

    //    return new Points(x, y2);
    //}

    //private (float, float) IsInCircle(float x)
    //{
    //    float distance1 = (float)(Math.Sqrt(Math.Pow(Radio, 2) - Math.Pow(x - Center.X, 2))) + Center.Y;
    //    float distance2 = (float)(-Math.Sqrt(Math.Pow(Radio, 2) - Math.Pow(x - Center.X, 2))) + Center.Y;

    //    return (distance1, distance2);
    //}
    //
    private bool IsInArc(Points point)
    {
        var startPoint = (Math.Abs(StartAngle) == AngleLeft) ? PointLeft : PointRight;
        var endPoint = (Math.Abs(StartAngle) != AngleLeft) ? PointLeft : PointRight;

        var startAngle = Math.Atan2(startPoint.Y - Center.Y, startPoint.X - Center.X);
        var endAngle = Math.Atan2(endPoint.Y - Center.Y, endPoint.X - Center.X);
        var pointAngle = Math.Atan2(point.Y - Center.Y, point.X - Center.X);

        if (pointAngle >= startAngle && pointAngle <= endAngle)
            return true;

        return false;
    }

    private (float, float) IsInCircle(float x)
    {
        float distance1 = (float)(Math.Sqrt(Math.Pow(Radio, 2) - Math.Pow(x - Center.X, 2))) + Center.Y;
        float distance2 = (float)(-Math.Sqrt(Math.Pow(Radio, 2) - Math.Pow(x - Center.X, 2))) + Center.Y;

        return (distance1, distance2);
    }
}

