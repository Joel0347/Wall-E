using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G_Sharp;
public class Draw : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.DrawExpression;

    public List<ExpressionSyntax> Parameters { get; }
    public Color Color { get; }

    public Draw(List<ExpressionSyntax> parameters, Color color)
    {
        Parameters = parameters;
        Color = color;
    }

}
