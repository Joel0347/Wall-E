namespace WallE;

public class Constants
{
    public static bool IsConstant(string s) {
        string n = String.StringsToSpaces(s);
        int equalIndex = n.IndexOf("=");
        if(equalIndex != s.Length - 1 && equalIndex != -1 && n[equalIndex + 1] != '=') return true;
        return false;
    }

    public static void Eval(string s) {
        string n = String.StringsToSpaces(s);
        int equalIndex = n.IndexOf("=");
        string constant = s[..equalIndex].Trim();
        string value = Main.Parse(s[(equalIndex + 1)..]);

        if (Check.VariableRevision(constant)) {
            Cache.constantsType[constant] = Types.GetType(value);
            Cache.constantValues[constant] = value;
        }
    }
}