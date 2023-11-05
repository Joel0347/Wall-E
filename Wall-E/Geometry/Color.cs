namespace WallE;

public class Colors
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
            Cache.color.Add(s);
        }

        else Cache.color.RemoveAt(Cache.color.Count - 1);
    }
}