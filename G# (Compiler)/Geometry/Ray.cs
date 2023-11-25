namespace WallE;

public class Ray : Draw
{
    public Points P1 { get; private set; }
    public Points P2 { get; private set; }
    public Points End;

    public Ray(Points p1, Points p2)
    {
        P1 = p1;
        P2 = p2;
    }
    public static bool IsRay(string s) {
        s = s.Trim();
        return s.StartsWith("ray ");
    }

    public static void Eval(string s) {
        s = s[3..s.IndexOf(";")];
        s = s.Trim();

        Random random = new();
        Points p1 = new((float)(random.Next(200, 1000) + random.NextDouble()),
                        (float)(random.Next(200, 700) + random.NextDouble()));
        Points p2 = new((float)(random.Next(200, 1000) + random.NextDouble()),
                        (float)(random.Next(200, 700) + random.NextDouble()));

        Ray ray = new Ray(p1, p2);

        Cache.constantsType[s] = "ray";
        Cache.geometryValues[s] = ray;
    }

    public override void Drawing(Graphics graphics, string color)
    {
        (float, float) equation = Utilities.LineEquation(this.P1, this.P2);
        float m = equation.Item1;
        float n = equation.Item2;

        float x_end = 0;

        if (this.P1.X <= this.P2.X)
        {
            x_end = this.P2.X + 50000;
        }
        else
        {
            x_end = this.P2.X - 50000;
        }

        float y_end = Utilities.PointInLine(m, n, x_end);

        Points end = new(x_end, y_end);
        End = end;

        Segment s1 = new(this.P1, this.P2);
        Segment s2 = new(this.P2, end);

        s1.Drawing(graphics, color);
        s2.Drawing(graphics, color);
    }
}