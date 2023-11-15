namespace WallE;

public class Circle : Draw
{
    public Points Center { get; private set; }
    public float Diameter { get; private set; }
    public float Radio { get; private set; }

    public Circle(Points center, float radio)
    {
        Center = center;
        Radio = radio;
        Diameter = 2 * radio;
    }

    public static bool IsCircle(string s) {
        s = s.Trim();
        return s.StartsWith("circle ");
    }

    public static void Eval(string s) {
        s = s[6..s.IndexOf(";")];
        s = s.Trim();

        Random random = new();
        Points center = new((float)(random.Next(200, 1000) + random.NextDouble()),
                           (float)(random.Next(200, 700) + random.NextDouble()));
        float radio = (float)(random.Next(40, 200) + random.NextDouble());

        Circle circle = new(center, radio);

        Cache.constantsType[s] = "circle";
        Cache.geometryValues[s] = circle;
    }

    public override void Drawing(Graphics graphics, string color)
    {
        Pen pen = new(Cache.colors[color]);
        Points extrem = new(this.Center.X - Radio, this.Center.Y - Radio);
        graphics.DrawEllipse(pen, extrem.X, extrem.Y, Diameter, Diameter);
        this.Center.Drawing(graphics, color);
    }
}