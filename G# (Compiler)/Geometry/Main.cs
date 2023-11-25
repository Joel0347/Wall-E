using System.Drawing;

namespace WallE;

public class Main_Grapher 
{
    public static void Parsing(string s) {
        if (Arc.IsArc(s))
        {
            Arc.Eval(s);
            return;
        }

        if (Points.IsPoint(s)) {
            Points.Eval(s);
            return;
        }

        if (Line.IsLine(s)) {
            Line.Eval(s);
            return;
        }

        if (Segment.IsSegment(s)) {
            Segment.Eval(s);
            return;
        }

        if (Ray.IsRay(s)) {
            Ray.Eval(s);
            return;
        }

        if (Circle.IsCircle(s)) {
            Circle.Eval(s);
            return;
        }

        if (Colors.IsColor(s)) {
            Points.Eval(s);
            return;
        }
    }
}