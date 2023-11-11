using System.Collections.Generic;
namespace WallE;

public class abstract Draw 
{
    public static bool IsDraw(string s)
    {
        s = s.Trim();
        return s.StartsWith("draw ");
    }

    public abstract void Drawing(string[] args, string color = "black");
}