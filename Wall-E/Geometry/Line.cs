namespace Wall_E;

public class Line
{
    public static bool IsLine(string s) {
        s = s.Trim();
        return s.StartsWith("line ");
    }

    public static void Eval(string s) {
        s = s[4..];
        s = s.Trim();
        Data.constantsType[s] = "line";
    }
}