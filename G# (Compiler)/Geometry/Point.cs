namespace WallE;

// Clase para evaluar el tipo punto.
public class Points
{
    // Método que determina si es un punto.
    public static bool IsPoint(string s) {
        s = s.Trim();
        return s.StartsWith("point ");
    }

    // Método que le provee el tipo 'point' a la constante.
    public static void Eval(string s) {
        s = s[5..s.IndexOf(";")];
        s = s.Trim();
        Cache.constantsType[s] = "point";
        Cache.constantValues[s] = s;
    }
}