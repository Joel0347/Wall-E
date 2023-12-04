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
    #region Pintar puntos 
    public static void Drawing(this Points point, Graphics graphics, Color color)
    {
        Brush brush = new SolidBrush(color);
        graphics.FillEllipse(brush, point.X - 5, point.Y - 5, 10, 10);
    }

    public static void DrawingString(this Points point, Graphics graphics, Color color, string msg)
    {
        Brush brush = new SolidBrush(color);
        Font font = new("Consolas", 10);
        graphics.DrawString(msg, font, brush, point.X, point.Y);
    }
    #endregion

    #region Pintar segmentos
    public static void Drawing(this Segment segment, Graphics graphics, Color color)
    {
        Pen pen = new(color)
        {
            Width = 2
        };
        graphics.DrawLine(pen, segment.P1.X, segment.P1.Y, segment.P2.X, segment.P2.Y);
    }

    public static void DrawingString(this Segment segment, Graphics graphics, Color color, string msg)
    {
        Brush brush = new SolidBrush(color);
        Font font = new("Consolas", 10);
        graphics.DrawString(msg, font, brush, segment.P1.X, segment.P1.Y);
    }
    #endregion

    #region Pintar rayos
    public static void Drawing(this Ray ray, Graphics graphics, Color color)
    {
        Segment s1 = new(ray.P1, ray.P2);
        Segment s2 = new(ray.P2, ray.End);

        s1.Drawing(graphics, color);
        s2.Drawing(graphics, color);
    }

    public static void DrawingString(this Ray ray, Graphics graphics, Color color, string msg)
    {
        Brush brush = new SolidBrush(color);
        Font font = new("Consolas", 10);
        graphics.DrawString(msg, font, brush, ray.P1.X, ray.P1.Y);
    }
    #endregion

    #region Pintar lineas
    public static void Drawing(this Line line, Graphics graphics, Color color)
    {
        Segment s1 = new(line.Start, line.P1);
        Segment s2 = new(line.P1, line.P2);
        Segment s3 = new(line.P2, line.End) ;

        s1.Drawing(graphics, color);
        s2.Drawing(graphics, color);
        s3.Drawing(graphics, color);
    }

    public static void DrawingString(this Line line, Graphics graphics, Color color, string msg)
    {
        Brush brush = new SolidBrush(color);
        Font font = new("Consolas", 10);
        graphics.DrawString(msg, font, brush, line.P1.X, line.P1.Y);
    }
    #endregion

    #region Pintar circunsferncias
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

    public static void DrawingString(this Circle circle, Graphics graphics, Color color, string msg)
    {
        Brush brush = new SolidBrush(color);
        Font font = new("Consolas", 10);
        graphics.DrawString(msg, font, brush, circle.Center.X, circle.Center.Y);
    }
    #endregion

    #region Pintar arcos
    public static void Drawing(this Arc arc, Graphics graphics, Color color)
    {
        Color componentsColor = Color.DodgerBlue;
        if (color == Color.DodgerBlue)
        {
            componentsColor = Color.Black;
        }

        Pen pen = new(color)
        {
            Width = 5
        };

        Circle circle = new(arc.Center, arc.Measure);
        Ray r1 = new(arc.Center, arc.Left);
        Ray r2 = new(arc.Center, arc.Right);

        Points point_left = Utilities.IntersectionCircle_Ray(arc.Center, r1.End, arc.Radio, r1.M, r1.N);
        Points point_right = Utilities.IntersectionCircle_Ray(arc.Center, r2.End, arc.Radio, r2.M, r2.N);

        Segment segment1 = new(arc.Center, point_left);
        Segment segment2 = new(arc.Center, point_right);

        Points extrem = new(arc.Center.X - arc.Radio, arc.Center.Y - arc.Radio);

        circle.Drawing(graphics, componentsColor);
        segment1.Drawing(graphics, componentsColor);
        segment2.Drawing(graphics, componentsColor);
        graphics.DrawArc(pen, extrem.X, extrem.Y, arc.Diameter, arc.Diameter, arc.StartAngle, arc.SweepAngle);
    }

    public static void DrawingString(this Arc arc, Graphics graphics, Color color, string msg)
    {
        Brush brush = new SolidBrush(color);
        Font font = new("Consolas", 10);
        graphics.DrawString(msg, font, brush, arc.Center.X, arc.Center.Y);
    }
    #endregion
    public static void DrawFigure(List<(Figure, Color, string)> geometries, Graphics graphic)
    {
        foreach (var geom in geometries)
        {
            switch (geom.Item1.Kind)
            {
                case SyntaxKind.PointToken:
                    {
                        Points point = (Points)geom.Item1;
                        point.Drawing(graphic, geom.Item2);
                        point.DrawingString(graphic, geom.Item2, geom.Item3);
                        break;
                    }
                case SyntaxKind.SegmentToken:
                    {
                        Segment segment = (Segment)geom.Item1;
                        segment.Drawing(graphic, geom.Item2);
                        segment.DrawingString(graphic, geom.Item2, geom.Item3);
                        break;
                    }
                case SyntaxKind.RayToken:
                    {
                        Ray ray = (Ray)geom.Item1;
                        ray.Drawing(graphic, geom.Item2);
                        ray.DrawingString(graphic, geom.Item2, geom.Item3);
                        break;
                    }
                case SyntaxKind.LineToken:
                    {
                        Line line = (Line)geom.Item1;
                        line.Drawing(graphic, geom.Item2);
                        line.DrawingString(graphic, geom.Item2, geom.Item3);
                        break;
                    }
                case SyntaxKind.CircleToken:
                    {
                        Circle circle = (Circle)geom.Item1;
                        circle.Drawing(graphic, geom.Item2);
                        circle.DrawingString(graphic, geom.Item2, geom.Item3);
                        break;
                    }
                case SyntaxKind.ArcToken:
                    {
                        Arc arc = (Arc)geom.Item1;
                        arc.Drawing(graphic, geom.Item2);
                        arc.DrawingString(graphic, geom.Item2, geom.Item3);
                        break;
                    }
            }
        }
    }
}
