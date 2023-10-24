namespace Wall_E;

// Clase para evaluar el tipo punto.
public class Point
{
    // Método que determina si es un punto.
    public static bool IsPoint(string s) {
        s = s.Trim();
        return s.StartsWith("point ");
    }

    // Método que le provee el tipo 'point' a la constante.
    public static void Eval(string s) {
        s = s[5..];
        s = s.Trim();
        Data.constantsType[s] = "point";
    }
}