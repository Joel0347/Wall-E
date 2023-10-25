namespace WallE;

public class Circle 
{
    public static bool IsCircle(string s) {
        s = s.Trim();
        return s.StartsWith("circle ");
    }

    public static void Eval(string s) {
        s = s[6..];
        s = s.Trim();
        Data.constantsType[s] = "circle";
    }
}