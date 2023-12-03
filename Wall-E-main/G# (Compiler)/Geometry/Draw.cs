using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G_Sharp;

public class Draw : ExpressionSyntax
{
    public List<(Figure, Color, string)> Geometries { get; }
    public override SyntaxKind Kind => SyntaxKind.DrawExpression;

    public List<ExpressionSyntax> Parameters { get; }
    public Color Color { get; }
    public string Msg { get; }

    public override string ReturnType => "void";

    public Draw(List<ExpressionSyntax> parameters, Color color, string msg)
    {
        Parameters = parameters;
        Color = color;
        Msg = msg;
        Geometries = new();
    }

    public Draw(List<(Figure, Color, string)> geometries, List<ExpressionSyntax> parameters, Color color)
    { 
        Geometries = geometries;
        Parameters = parameters;
        Color = color;
    }

    public override object Evaluate(Scope scope)
    {
        foreach (var item in Parameters)
        {
            var value = item.Evaluate(scope);

            if (value is SequenceExpressionSyntax sequence)
            {
                foreach (var element in sequence.ElementsEvaluation)
                {
                    if (element is ExpressionSyntax evalElement)
                    {
                        if ((int)evalElement.Kind <= 28 && (int)evalElement.Kind >= 22)
                        {
                            var geometryValue = (Figure)evalElement;
                            Geometries.Add((geometryValue, Color, Msg));
                        }
                    }
                }
            }

            else if (value is ExpressionSyntax val)
            {
                 if ((int)val.Kind <= 28 && (int)val.Kind >= 22)
                 {
                    var geometryValue = (Figure)value;
                    Geometries.Add((geometryValue, Color, Msg));
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
