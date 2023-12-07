using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace G_Sharp;

public class Intersect
{
    public static SequenceExpressionSyntax IntersectPointPoint(Points p1, Points p2)
    {
        if (p1.Equals(p2))
            return new FiniteSequence(new List<object>() { p1 });

        return new FiniteSequence(new List<object>());
    }

    public static SequenceExpressionSyntax IntersectPointLine(Points point, Line line)
    {
        var y1 = Utilities.PointInLine(line.M, line.N, point.X);
        var y2 = point.Y;
        var errorRange = 0.5;
        if (Math.Abs(y1 - y2) <= errorRange)
            return new FiniteSequence(new List<object>() { point });

        return new FiniteSequence(new List<object>());
    }

    public static SequenceExpressionSyntax IntersectPointSegment(Points point, Segment segment)
    {
        var y1 = Utilities.PointInLine(segment.M, segment.N, point.X);
        var y2 = point.Y;
        var errorRange = 0.5;
        if (Math.Abs(y1 - y2) <= errorRange && Utilities.IsInSegment(segment.P1.X, segment.P2.X, point.X))
            return new FiniteSequence(new List<object>() { point });

        return new FiniteSequence(new List<object>());
    }

    public static SequenceExpressionSyntax IntersectPointRay(Points point, Ray ray)
    {
        Segment raySegment = new(ray.P1, ray.End);
        return IntersectPointSegment(point, raySegment);
    }

    public static SequenceExpressionSyntax IntersectPointCircle(Points point, Circle circle)
    {
        var distance = Utilities.DistanceBetweenPoints(point, circle.Center);
        var radius = circle.Radio;
        var errorRange = 0.5;
        if (Math.Abs(distance - radius) <= errorRange)
            return new FiniteSequence(new List<object>() { point });

        return new FiniteSequence(new List<object>());
    }

    public static SequenceExpressionSyntax IntersectPointArc(Points point, Arc arc)
    {
        var distance = Utilities.DistanceBetweenPoints(point, arc.Center);
        var radius = arc.Radio;
        var errorRange = 0.5;
        if (Math.Abs(distance - radius) > errorRange)
            return new FiniteSequence(new List<object>());

        Segment centerPoint = new(arc.Center, point);
        Segment chord = new(arc.PointLeft, arc.PointRight);
        bool intersect = (int)(IntersectSegmentSegment(chord, centerPoint).Count) > 0;

        if (Math.Abs(arc.SweepAngle) >= 180 && !intersect)
            return new FiniteSequence(new List<object>() { point });

        if (Math.Abs(arc.SweepAngle) < 180 && intersect)
            return new FiniteSequence(new List<object>() { point });
        

        return new FiniteSequence(new List<object>());
    }

    public static SequenceExpressionSyntax IntersectLineLine(Line l1, Line l2)
    {
        if (l1.Equals(l2))
            return new FiniteSequence(new List<object>() { null! });

        if (l1.M == l2.M)
            return new FiniteSequence(new List<object>());

        float x = (l2.N - l1.N) / (l1.M - l2.M);
        float y = Utilities.PointInLine(l1.M, l1.N, x);
        Points point = new(x, y);

        return new FiniteSequence(new List<object>() { point });
    }

    public static SequenceExpressionSyntax IntersectLineSegment(Line line, Segment segment)
    {
        Line line1 = new(segment.P1, segment.P2);
        var intersect = IntersectLineLine(line, line1);
        
        if (intersect[0] is not null && intersect is FiniteSequence sequence)
        {
            var point = (Points)sequence[0];
            if (Utilities.IsInSegment(segment.P1.X, segment.P2.X, point.X))
            {
                return new FiniteSequence(new List<object>() { point });
            }
        }

        return new FiniteSequence(new List<object>());
    }

    public static SequenceExpressionSyntax IntersectLineRay(Line line, Ray ray)
    {
        Segment segment = new(ray.P1, ray.End);
        return IntersectLineSegment(line, segment);
    }

    public static SequenceExpressionSyntax IntersectLineCircle(Line line, Circle circle)
    {
        var distance = Utilities.DistancePointLine(circle.Center, line);

        if (distance > circle.Radio)
            return new FiniteSequence(new List<object>());

        var a = 1 + Math.Pow(line.M, 2);
        var b = 2 * line.M * (line.N - circle.Center.Y) - 2 * circle.Center.X;
        var c = Math.Pow(circle.Center.X, 2) + Math.Pow(line.N - circle.Center.Y, 2) - Math.Pow(circle.Radio, 2);

        var D = Math.Pow(b, 2) - 4 * a * c;

        var x1 = (float)((-b + Math.Sqrt(D)) / (2 * a));
        float y1 = Utilities.PointInLine(line.M, line.N, x1);

        Points p1 = new(x1, y1);
        if (D == 0)
            return new FiniteSequence(new List<object>() { p1 });

        var x2 = (float)((-b - Math.Sqrt(D)) / (2 * a));
        float y2 = Utilities.PointInLine(line.M, line.N, x2);

        Points p2 = new(x2, y2);


        return new FiniteSequence(new List<object>() { p1, p2 });
    }

    public static SequenceExpressionSyntax IntersectLineArc(Line line, Arc arc)
    {
        List<object> list = new();
        Circle arcCircle = new(arc.Center, arc.Measure);

        var intersect = IntersectLineCircle(line, arcCircle);

        if ((int)intersect.Count > 0)
        {
            for (int i = 0; i < (int)intersect.Count; i++)
            {
                var pointInArc = IntersectPointArc((Points)intersect[i], arc);
                if ((int)pointInArc.Count > 0) 
                    list.Add(intersect[i]);
            }
        }

        return new FiniteSequence(list);
    }

    public static SequenceExpressionSyntax IntersectSegmentSegment(Segment segment1, Segment segment2)
    {
        Line line1 = new(segment1.P1, segment1.P2);
        Line line2 = new(segment2.P1, segment2.P2);

        var intersect = IntersectLineLine(line1, line2);

        if ((int)intersect.Count > 0)
        {
            var point = (Points)intersect[0];

            if (Utilities.IsInSegment(segment1.P1.X, segment1.P2.X, point.X) &&
            Utilities.IsInSegment(segment2.P1.X, segment2.P2.X, point.X))
                return new FiniteSequence(new List<object>() { point });
        }

        return new FiniteSequence(new List<object>());
    }

    public static SequenceExpressionSyntax IntersectSegmentRay(Segment segment, Ray ray)
    {
        Segment raySegment = new(ray.P1, ray.End);
        return IntersectSegmentSegment(segment, raySegment);
    }

    public static SequenceExpressionSyntax IntersectSegmentCircle(Segment segment, Circle circle)
    {
        List<object> list = new();
        Line line = new(segment.P1, segment.P2);

        var intersect = IntersectLineCircle(line, circle);

        if ((int)intersect.Count > 0)
        {
            for (int i = 0; i < (int)intersect.Count; i++)
            {
                var point = (Points)intersect[i];
                if (Utilities.IsInSegment(segment.P1.X, segment.P2.X, point.X)) list.Add(point);
            }
        }

        return new FiniteSequence(list);
    }

    public static SequenceExpressionSyntax IntersectSegmentArc(Segment segment, Arc arc)
    {
        List<object> list = new();
        Line line = new(segment.P1, segment.P2);

        var intersect = IntersectLineArc(line, arc);

        if ((double)intersect.Count > 0)
        {
            for (int i = 0; i < (double)intersect.Count; i++)
            {
                var point = (Points)intersect[i];
                if (Utilities.IsInSegment(segment.P1.X, segment.P2.X, point.X)) list.Add(point);
            }
        }

        return new FiniteSequence(list);
    }

    public static SequenceExpressionSyntax IntersectRayRay(Ray ray1, Ray ray2)
    {
        Segment ray1Segment = new(ray1.P1, ray1.End);
        Segment ray2Segment = new(ray2.P1, ray2.End);
        return IntersectSegmentSegment(ray1Segment, ray2Segment);
    }

    public static SequenceExpressionSyntax IntersectRayCircle(Ray ray, Circle circle)
    {
        Segment raySegment = new(ray.P1, ray.End);
        return IntersectSegmentCircle(raySegment, circle);
    }

    public static SequenceExpressionSyntax IntersectRayArc(Ray ray, Arc arc)
    {
        Segment raySegment = new(ray.P1, ray.End);
        return IntersectSegmentArc(raySegment, arc);
    }

    public static SequenceExpressionSyntax IntersectCircleCircle(Circle c1, Circle c2)
    {
        var a1 = c1.Center.X;
        var b1 = c1.Center.Y;
        var r1 = c1.Radio;
        var a2 = c2.Center.X;
        var b2 = c2.Center.Y;
        var r2 = c2.Radio;

        if (c1.Equals(c2) && r1.Equals(r2))
            return new FiniteSequence(new List<object>() { null! });

        var distanceBetweenCenters = Utilities.DistanceBetweenPoints(c1.Center, c2.Center);
        var sumRadious = c1.Radio + c2.Radio;

        if (distanceBetweenCenters > sumRadious || distanceBetweenCenters < Math.Abs(r1 - r2))
            return new FiniteSequence(new List<object>());

        var a = (Math.Pow(r1, 2) - Math.Pow(r2, 2) + Math.Pow(distanceBetweenCenters, 2)) / (2 * distanceBetweenCenters);
        var h = Math.Sqrt(Math.Pow(r1, 2) - Math.Pow(a, 2));

        var x3 = a1 + a * (a2 - a1) / distanceBetweenCenters;
        var y3 = b1 + a * (b2 - b1) / distanceBetweenCenters;

        var x1 = (float)(x3 + h * (b2 - b1) / distanceBetweenCenters);
        var y1 = (float)(y3 - h * (a2 - a1) / distanceBetweenCenters);

        var x2 = (float)(x3 - h * (b2 - b1) / distanceBetweenCenters);
        var y2 = (float)(y3 + h * (a2 - a1) / distanceBetweenCenters);

        Points intersect1 = new(x1, y1);
        Points intersect2 = new(x2, y2);

        return new FiniteSequence(new List<object>() { intersect1, intersect2 });
    }

    public static SequenceExpressionSyntax IntersectCircleArc(Circle circle, Arc arc)
    {
        List<object> list = new();
        Circle arcCircle = new(arc.Center, arc.Measure);

        var intersect = IntersectCircleCircle(circle, arcCircle);

        if ((int)intersect.Count > 0)
        {
            for (int i = 0; i < (int)intersect.Count; i++)
            {
                var pointInArc = IntersectPointArc((Points)intersect[i], arc);
                if ((int)pointInArc.Count > 0) 
                    list.Add(intersect[i]);
            }
        }

        return new FiniteSequence(list);
    }

    public static SequenceExpressionSyntax IntersectArcArc(Arc arc1, Arc arc2)
    {
        List<object> list = new();
        Circle arc1Circle = new(arc1.Center, arc1.Measure);
        Circle arc2Circle = new(arc2.Center, arc2.Measure);

        var intersect = IntersectCircleCircle(arc1Circle, arc2Circle);

        if ((int)intersect.Count > 0)
        {
            for (int i = 0; i < (int)intersect.Count; i++)
            {
                var pointInArc1 = IntersectPointArc((Points)intersect[i], arc1);
                var pointInArc2 = IntersectPointArc((Points)intersect[i], arc2);

                if ((int)pointInArc1.Count > 0 && (int)pointInArc2.Count > 0) 
                    list.Add(intersect[i]);
            }
        }

        return new FiniteSequence(list);
    }
}
