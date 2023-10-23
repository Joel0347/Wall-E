namespace Wall_E;

public class Ray 
{
    public static bool IsRay(string s) {
        s = s.Trim();
        return s.StartsWith("ray ");
    }

    public static void Eval(string s) {
        s = s[3..];
        s = s.Trim();
        Data.constantsType[s] = "ray";
    }
}