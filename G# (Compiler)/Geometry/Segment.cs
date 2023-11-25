namespace WallE;

public class Segment : Draw 
{
    public Points P1 { get; private set; }
    public Points P2 { get; private set; }

    public Segment(Points p1, Points p2)
    {
        P1 = p1;
        P2 = p2;
    }
    public static bool IsSegment(string s) {
        s = s.Trim();
        return s.StartsWith("segment ");
    }

    public static void Eval(string s) {
        s = s[7..s.IndexOf(";")];
        s = s.Trim();

        Random random = new();
        Points p1 = new((float)(random.Next(200, 1000) + random.NextDouble()),
                        (float)(random.Next(200, 700) + random.NextDouble()));
        Points p2 = new((float)(random.Next(200, 1000) + random.NextDouble()),
                        (float)(random.Next(200, 700) + random.NextDouble()));

        Segment segment= new(p1, p2);

        Cache.constantsType[s] = "segment";
        Cache.geometryValues[s] = segment;
    }

    public override void Drawing(Graphics graphics, string color)
    {
        Pen pen = new(Cache.colors[color]);
        graphics.DrawLine(pen, this.P1.X, this.P1.Y, this.P2.X, this.P2.Y);
        this.P1.Drawing(graphics, color);
        this.P2.Drawing(graphics, color);
    }
}