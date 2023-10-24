using System.Drawing;

namespace Wall_E;

public class Main 
{
    public static void Parsing(string s) {
        if (Point.IsPoint(s)) {
            Point.Eval(s);
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

        if (Color.IsColor(s)) {
            Point.Eval(s);
            return;
        }
    }
}