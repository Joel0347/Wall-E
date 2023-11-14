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

        Points center = new((float)(random.Next(200) + random.NextDouble()),
                            (float)(random.Next(200) + random.NextDouble()));
        float radio = random.Next(100, 500);

        //graphics.TranslateTransform((center.X + radio / 2) - 5, (center.Y + radio / 2) - 5);

        Points left = new((float)(random.Next(0, 900) + random.NextDouble()),
                            (float)(random.Next(0, 900) + random.NextDouble()));
        Points right = new((float)(random.Next(0, 900) + random.NextDouble()),
                            (float)(random.Next(0, 900) + random.NextDouble()));
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

        r1.Drawing(graphics, "blue");
        r2.Drawing(graphics, "blue");
        circle.Drawing(graphics, "blue");

        (float, float) equation1 = Utilities.LineEquation(this.Center, left);
        float m1 = equation1.Item1;
        float n1 = equation1.Item2;

        (float, float) equation2 = Utilities.LineEquation(this.Center, right);
        float m2 = equation2.Item1;
        float n2 = equation2.Item2;

        Points point_left = Utilities.IntersectionCircle_Ray(this.Center, r1.End, this.Radio, m1, n1);
        Points point_right = Utilities.IntersectionCircle_Ray(this.Center, r2.End, this.Radio, m2, n2);

        point_left.Drawing(graphics, "black");
        point_right.Drawing(graphics, "green");

        float sweepAngle = Utilities.AngleBetweenVectors(this.Center, point_left, point_right);

        Points extrem = new(this.Center.X - Radio, this.Center.Y - Radio);

        graphics.DrawArc(pen, extrem.X, extrem.Y, this.Diameter, this.Diameter, 0, sweepAngle);
    }
}
