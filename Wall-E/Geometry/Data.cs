using System.Collections.Generic;
namespace WallE;

public class Data 
{
    public static Dictionary<string, string> constantsType = new();
    public static List<string> color = new() {"black"};
    public static Dictionary<string, string> constantValues = new();

    public static bool IsDraw(string s)
    {
        s = s.Trim();
        return s.StartsWith("draw ");
    }

    public static (string, string) Draw(string s)
    {
        s = s.Trim();
        s = s[4..s.IndexOf(";")].Trim();

        //Main_Grapher.Parsing(s);
        return (constantsType[s], constantValues[s]);
    }
}