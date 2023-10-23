namespace Wall_E;

public class Color
{
    public static bool IsColor(string s) {
        s = s.Trim();
        return s.StartsWith("color ") || s.StartsWith("reset ");
    }

    public static void Eval(string s) {
        s = s.Trim();
        if (s.StartsWith("color ")) {
            s = s[5..];
            s = s.Trim();
            Data.color.Add(s);
        }

        else Data.color.RemoveAt(Data.color.Count - 1);
    }
}