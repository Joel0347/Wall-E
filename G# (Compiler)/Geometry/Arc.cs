using Microsoft.VisualBasic.Devices;

namespace WallE;

public class Arc : Draw
{
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

    public static bool IsArc(string s)
    {
        s = s.Trim();
        return s.StartsWith("arc ");
    }

    public static void Eval(string s)
    {
        s = s.Trim();
        s = s[3..s.IndexOf(";")];
        s = s.Trim();

        Random random = new();

        Points center = new((float)(random.Next(200, 1000) + random.NextDouble()),
                           (float)(random.Next(200, 700) + random.NextDouble()));
        float radio = random.Next(100, 500);

        Points left = new((float)(random.Next(200, 1000) + random.NextDouble()),
                           (float)(random.Next(200, 700) + random.NextDouble()));
        Points right = new((float)(random.Next(200, 1000) + random.NextDouble()),
                           (float)(random.Next(200, 700) + random.NextDouble()));
        Arc arc = new(center, left, right, radio);

        Cache.constantsType[s] = "arc";
        Cache.geometryValues[s] = arc;
    }

    public override void Drawing(Graphics graphics, string color)
    {
        Pen pen = new(Cache.colors["red"]);

        Points right = this.Right;
        Points left = this.Left;

        Ray r1 = new(this.Center, left);
        Ray r2 = new(this.Center, right);
        Circle circle = new(this.Center, this.Radio);

        r1.Drawing(graphics, color);
        r2.Drawing(graphics, color);
        circle.Drawing(graphics, color);

        (float, float) equation1 = Utilities.LineEquation(this.Center, left);
        float m1 = equation1.Item1;
        float n1 = equation1.Item2;

        (float, float) equation2 = Utilities.LineEquation(this.Center, right);
        float m2 = equation2.Item1;
        float n2 = equation2.Item2;

        Points point_left = Utilities.IntersectionCircle_Ray(this.Center, r1.End, this.Radio, m1, n1);
        Points point_right = Utilities.IntersectionCircle_Ray(this.Center, r2.End, this.Radio, m2, n2);
        Points cero = new(this.Center.X + this.Radio, this.Center.Y);

        float sweepAngle = Utilities.AngleBetweenVectors(this.Center, point_left, point_right);
        float angleLeftCero = Utilities.AngleBetweenVectors(this.Center, point_left, cero);
        float angleRightCero = Utilities.AngleBetweenVectors(this.Center, point_right, cero);
        float startAngle = 0;

        int quadrant_left = Utilities.GetAngleQuadrant(this.Center, this.Radio, point_left);
        int quadrant_right = Utilities.GetAngleQuadrant(this.Center, this.Radio, point_right);

        if (quadrant_left == quadrant_right)
        {
            startAngle = (quadrant_left < 3) ?  Math.Min(angleLeftCero, angleRightCero) 
                                             : -Math.Max(angleLeftCero, angleRightCero);
        }
        else
        {
            if (quadrant_left == 1 || quadrant_right == 1)
            {
                startAngle = (quadrant_left == 1) ? angleLeftCero : angleRightCero;
                float m = (quadrant_left == 1) ? m1 : m2;
                float n = (quadrant_left == 1) ? n1 : n2;
                Points point = (quadrant_left != 1) ? point_left : point_right;
                float evaluationInRay = Utilities.PointInLine(m, n, point.X);

                if (point.Y <= evaluationInRay)
                {
                    sweepAngle = 360 - sweepAngle;
                }
            }

            else if (quadrant_left == 2 || quadrant_right == 2)
            {
                startAngle = (quadrant_left == 2) ? angleLeftCero : angleRightCero;
                float m = (quadrant_left == 2) ? m1 : m2;
                float n = (quadrant_left == 2) ? n1 : n2;
                Points point = (quadrant_left != 2) ? point_left : point_right;
                float evaluationInRay = Utilities.PointInLine(m, n, point.X);

                if (point.Y > evaluationInRay)
                {
                    sweepAngle = 360 - sweepAngle;
                }
            }

            else if (quadrant_left == 3 || quadrant_right == 3)
            {
                startAngle = (quadrant_left == 3) ? -angleLeftCero : -angleRightCero;
            }
        }

        Points extrem = new(this.Center.X - Radio, this.Center.Y - Radio);

        graphics.DrawArc(pen, extrem.X, extrem.Y, this.Diameter, this.Diameter, startAngle, sweepAngle);
    }
}
