namespace WallE;

// Clase para evaluar el tipo punto.
public class Points : Draw
{

    public float X { get; private set; }    
    public float Y { get; private set; }

    public Points(float x, float y)
    {
        X = x;
        Y = y;
    }

    // Método que determina si es un punto.
    public static bool IsPoint(string s) {
        s = s.Trim();
        return s.StartsWith("point ");
    }

    // Método que le provee el tipo 'point' a la constante.
    public static void Eval(string s) {
        s = s[5..s.IndexOf(";")];
        s = s.Trim();

        Random random = new();
        Points point = new((float)(random.Next(200, 1000) + random.NextDouble()),
                           (float)(random.Next(200, 700) + random.NextDouble()));

        Cache.constantsType[s] = "point";
        Cache.geometryValues[s] = point;
    }

    public override void Drawing(Graphics graphics, string color)
    {
        Brush brush = new SolidBrush(Cache.colors[color]);
        graphics.FillEllipse(brush, this.X - 5, this.Y - 5, 10, 10);
    }
}