using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G_Sharp;

public sealed class Line : Figure, IEquatable<Line>
{
    public override SyntaxKind Kind => SyntaxKind.LineToken;
    public Points P1 { get; private set; }
    public Points P2 { get; private set; }
    public float M { get; private set; }
    public float N { get; private set; }
    public override string ReturnType => "line";

    public Line(Points p1, Points p2)
    {
        P1 = p1;
        P2 = p2;
        (M, N) = Utilities.LineEquation(P1, P2);
    }

    public override object Evaluate(Scope scope)
    {
        return new Line(P1, P2);
    }

    public override bool Checker(Scope scope)
    {
        return true;
    }

    public bool Equals(Line? other)
    {
        return M.Equals(other!.M) && N.Equals(other!.N);
    }

    public override bool Equals(object? obj) => Equals(obj as Line);
    public override int GetHashCode() => P1.GetHashCode();
}
