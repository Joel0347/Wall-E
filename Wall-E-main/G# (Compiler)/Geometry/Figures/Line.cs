using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.LinkLabel;

namespace G_Sharp;

public sealed class Line : Figure, IEquatable<Line>
{
    public override SyntaxKind Kind => SyntaxKind.LineToken;
    public Points P1 { get; }
    public Points P2 { get; }
    public Points Start { get; }
    public Points End { get; }
    public float M { get; }
    public float N { get; }
    public override string ReturnType => "line";

    public Line(Points p1, Points p2)
    {
        P1 = p1;
        P2 = p2;
        (M, N) = Utilities.LineEquation(P1, P2);

        float x_start = P1.X - 50000;
        float x_end = P2.Y + 50000;

        float y_start = Utilities.PointInLine(M, N, x_start);
        float y_end = Utilities.PointInLine(M, N, x_end);

        if (M is float.NaN)
        {
            y_start = x_start;
            y_end = x_end;
        }

        Start = new Points(x_start, y_start);
        End = new Points(x_end, y_end); 
    }

    public override object Evaluate(Scope scope)
    {
        return this;
    }

    public override bool Check(Scope scope)
    {
        return true;
    }

    public bool Equals(Line? other)
    {
        return M.Equals(other!.M) && N.Equals(other!.N);
    }

    public override bool Equals(object? obj) => Equals(obj as Line);
    public override int GetHashCode() => P1.GetHashCode();

    public override SequenceExpressionSyntax PointsInFigure()
    {
        Dictionary<int, object> elements = new();
        object PointsInLine()
        {
            float x;
            float y;
            do
            {
                x = ParsingSupplies.CreateRandomsCoordinates();
                y = Utilities.PointInLine(M, N, x);
            }
            while (!Utilities.IsInSegment(Start.X, End.X, x) || !Utilities.IsInSegment(Start.Y, End.Y, y));

            return new Points(x, y);
        }

        var result = new InfiniteSequence(PointsInLine, elements)
        {
            valuesType = "point"
        };

        return result;
    }
}
