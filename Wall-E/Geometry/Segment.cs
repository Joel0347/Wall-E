namespace Wall_E;

public class Segment
{
    public static bool IsSegment(string s) {
        s = s.Trim();
        return s.StartsWith("segment ");
    }

    public static void Eval(string s) {
        s = s[7..];
        s = s.Trim();
        Data.constantsType[s] = "segment";
    }
}