using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace G_Sharp;

public sealed class Ray : Figure, IEquatable<Ray>
{
    public override SyntaxKind Kind => SyntaxKind.RayToken;
    public Points P1 { get; }
    public Points P2 { get; }
    public Points End { get; }
    public float M { get; }
    public float N { get; }
    public override string ReturnType => "ray";

    public Ray(Points p1, Points p2)
    {
        P1 = p1;
        P2 = p2;

        (M, N) = Utilities.LineEquation(P1, P2);

        float x_end = 0;
        if (P1.X <= P2.X)
        {
            x_end = P2.X + 50000;
        }
        else
        {
            x_end = P2.X - 50000;
        }

        float y_end = Utilities.PointInLine(M, N, x_end);

        if (M is float.NaN)
        {
            y_end = x_end;
        }

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

    public bool Equals(Ray? other)
    {
        var sameLine = M.Equals(other!.M) && N.Equals(other!.N);
        return sameLine && P1.Equals(other!.P1);
    }

    public override bool Equals(object? obj) => Equals(obj as Ray);
    public override int GetHashCode() => P1.GetHashCode();

    public override SequenceExpressionSyntax PointsInFigure()
    {
        Dictionary<int, object> elements = new();
        object PointsInRay()
        {
            float x;
            float y;
            do
            {
                x = ParsingSupplies.CreateRandomsCoordinates();
                y = Utilities.PointInLine(M, N, x);
            }
            while (!Utilities.IsInSegment(P1.X, End.X, x) || !Utilities.IsInSegment(P1.Y, End.Y, y));

            return new Points(x, y);
        }

        var result = new InfiniteSequence(PointsInRay, elements)
        {
            valuesType = "point"
        };

        return result;
    }
}
