using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G_Sharp;
using Microsoft.VisualBasic.Devices;

namespace WallE;
public static class MethodsDrawing
{
    public static void Drawing(this Points point, Graphics graphics, Color color)
    {
        Brush brush = new SolidBrush(color);
        graphics.FillEllipse(brush, point.X - 5, point.Y - 5, 10, 10);
    }
    public static void Drawing(this Segment segment, Graphics graphics, Color color)
    {
        Pen pen = new(color)
        {
            Width = 2
        };
        graphics.DrawLine(pen, segment.P1.X, segment.P1.Y, segment.P2.X, segment.P2.Y);
        segment.P1.Drawing(graphics, color);
        segment.P2.Drawing(graphics, color);
    }
    public static void Drawing(this Ray ray, Graphics graphics, Color color)
    {
        (float, float) equation = Utilities.LineEquation(ray.P1, ray.P2);
        float m = equation.Item1;
        float n = equation.Item2;

        float x_end = 0;

        if (ray.P1.X <= ray.P2.X)
        {
            x_end = ray.P2.X + 50000;
        }
        else
        {
            x_end = ray.P2.X - 50000;
        }

        float y_end = Utilities.PointInLine(m, n, x_end);

        Points end = new(x_end, y_end);
        ray.End = end;

        Segment s1 = new(ray.P1, ray.P2);
        Segment s2 = new(ray.P2, end);

        s1.Drawing(graphics, color);
        s2.Drawing(graphics, color);
    }
    public static void Drawing(this Line line, Graphics graphics, Color color)
    {
        (float, float) equation = Utilities.LineEquation(line.P1, line.P2);
        float m = equation.Item1;
        float n = equation.Item2;

        float x_start = line.P1.X - 50000;
        float x_end = line.P2.Y + 50000;

        float y_start = Utilities.PointInLine(m, n, x_start);
        float y_end = Utilities.PointInLine(m, n, x_end);

        Points start = new(x_start, y_start);
        Points end = new(x_end, y_end);

        Segment s1 = new(start, line.P1);
        Segment s2 = new(line.P1, line.P2);
        Segment s3 = new(line.P2, end);

        s1.Drawing(graphics, color);
        s2.Drawing(graphics, color);
        s3.Drawing(graphics, color);
    }
    public static void Drawing(this Circle circle, Graphics graphics, Color color)
    {
        Pen pen = new(color)
        {
            Width = 2
        };

        Points extrem = new(circle.Center.X - circle.Radio, circle.Center.Y - circle.Radio);
        graphics.DrawEllipse(pen, extrem.X, extrem.Y, circle.Diameter, circle.Diameter);
        circle.Center.Drawing(graphics, color);
    }
    public static void Drawing(this Arc arc, Graphics graphics, Color color)
    {
        Pen pen = new(Color.Red)
        {
            Width = 5
        };

        Points right = arc.Right;
        Points left = arc.Left;

        Ray r1 = new(arc.Center, left);
        Ray r2 = new(arc.Center, right);
        Circle circle = new(arc.Center, arc.Measure);

        r1.Drawing(graphics, color);
        r2.Drawing(graphics, color);
        circle.Drawing(graphics, color);

        (float, float) equation1 = Utilities.LineEquation(arc.Center, left);
        float m1 = equation1.Item1;
        float n1 = equation1.Item2;

        (float, float) equation2 = Utilities.LineEquation(arc.Center, right);
        float m2 = equation2.Item1;
        float n2 = equation2.Item2;

        Points point_left = Utilities.IntersectionCircle_Ray(arc.Center, r1.End, arc.Radio, m1, n1);
        Points point_right = Utilities.IntersectionCircle_Ray(arc.Center, r2.End, arc.Radio, m2, n2);

        Points cero = new(arc.Center.X + arc.Radio, arc.Center.Y);

        float sweepAngle = Utilities.AngleBetweenVectors(arc.Center, point_left, point_right);
        float angleLeftCero = Utilities.AngleBetweenVectors(arc.Center, point_left, cero);
        float angleRightCero = Utilities.AngleBetweenVectors(arc.Center, point_right, cero);
        float startAngle = 0;

        int quadrant_left = Utilities.GetAngleQuadrant(arc.Center, arc.Radio, point_left);
        int quadrant_right = Utilities.GetAngleQuadrant(arc.Center, arc.Radio, point_right);

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
                float m = quadrant_left == 1 ? m1 : m2;
                float n = quadrant_left == 1 ? n1 : n2;
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
                float m = quadrant_left == 2 ? m1 : m2;
                float n = quadrant_left == 2 ? n1 : n2;
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
        Points extrem = new(arc.Center.X - arc.Radio, arc.Center.Y - arc.Radio);

        graphics.DrawArc(pen, extrem.X, extrem.Y, arc.Diameter, arc.Diameter, startAngle, sweepAngle);
    }
    public static void DrawFigure(List<(GeometrySyntax, Color)> geometries, Graphics graphic)
    {
        foreach (var geom in geometries)
        {
            switch (geom.Item1.Kind)
            {
                case SyntaxKind.PointToken:
                    {
                        Points point = (Points)geom.Item1;
                        point.Drawing(graphic, geom.Item2);
                        break;
                    }
                case SyntaxKind.SegmentToken:
                    {
                        Segment segment = (Segment)geom.Item1;
                        segment.Drawing(graphic, geom.Item2);
                        break;
                    }
                case SyntaxKind.RayToken:
                    {
                        Ray ray = (Ray)geom.Item1;
                        ray.Drawing(graphic, geom.Item2);
                        break;
                    }
                case SyntaxKind.LineToken:
                    {
                        Line _line = (Line)geom.Item1;
                        _line.Drawing(graphic, geom.Item2);
                        break;
                    }
                case SyntaxKind.CircleToken:
                    {
                        Circle circle = (Circle)geom.Item1;
                        circle.Drawing(graphic, geom.Item2);
                        break;
                    }
                case SyntaxKind.ArcToken:
                    {
                        Arc arc = (Arc)geom.Item1;
                        arc.Drawing(graphic, geom.Item2);
                        break;
                    }
            }
        }
    }
}
