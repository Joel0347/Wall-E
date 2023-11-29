using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G_Sharp;

public class Draw : ExpressionSyntax
{
    public List<(GeometrySyntax, Color)> Geometries { get; }
    public override SyntaxKind Kind => SyntaxKind.DrawExpression;

    public List<ExpressionSyntax> Parameters { get; }
    public Color Color { get; }
    public override string ReturnType => "void";

    public Draw(List<ExpressionSyntax> parameters, Color color)
    {
        Parameters = parameters;
        Color = color;
        Geometries = new();
    }

    public Draw(List<(GeometrySyntax, Color)> geometries, List<ExpressionSyntax> parameters, Color color)
    { 
        Geometries = geometries;
        Parameters = parameters;
        Color = color;
    }

    public override object Evaluate(Scope scope)
    {
        foreach (var item in Parameters)
        {
            var value = scope.EvaluateExpression(item);
            if (value is ExpressionSyntax val)
            {
                if ((int)val.Kind <= 27 && (int)val.Kind >= 22)
                {
                    var geometryValue = (GeometrySyntax)value;
                    Geometries.Add((geometryValue, Color));
                }
            }
        }

        return new Draw(Geometries, Parameters, Color);
    }

    public override bool Checker(Scope scope)
    {
        return true;
    }
}
