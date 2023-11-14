using System;

namespace WallE;

public class Line : Draw 
{
    public Points P1 { get; private set; }
    public Points P2 { get; private set; }

    public Line(Points p1, Points p2)
    {
        P1 = p1;
        P2 = p2;
    }

    public static bool IsLine(string s) {
        s = s.Trim();
        return s.StartsWith("line ");
    }

    public static void Eval(string s) {
        s = s[4..s.IndexOf(";")];
        s = s.Trim();

        Random random = new();
        Points p1 = new((float)(random.Next(200, 1000) + random.NextDouble()),
                        (float)(random.Next(200, 700) + random.NextDouble()));
        Points p2 = new((float)(random.Next(200, 1000) + random.NextDouble()),
                        (float)(random.Next(200, 700) + random.NextDouble()));

        Line line = new(p1, p2);

        Cache.constantsType[s] = "line";
        Cache.geometryValues[s] = line;
    }

    public override void Drawing(Graphics graphics, string color)
    {
        (float, float) equation = Utilities.LineEquation(this.P1, this.P2);
        float m = equation.Item1;
        float n = equation.Item2;

        float x_start = this.P1.X - 50000;
        float x_end = this.P2.Y + 50000;

        float y_start = Utilities.PointInLine(m, n, x_start);
        float y_end = Utilities.PointInLine(m, n, x_end);

        Points start = new(x_start, y_start);
        Points end = new(x_end, y_end);

        Segment s1 = new(start, this.P1);
        Segment s2 = new(this.P1, this.P2);
        Segment s3 = new(this.P2, end);

        s1.Drawing(graphics, color);
        s2.Drawing(graphics, color);
        s3.Drawing(graphics, color);
    }

}