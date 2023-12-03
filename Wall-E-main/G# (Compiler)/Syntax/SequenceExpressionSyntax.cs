using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G_Sharp;

public class SequenceExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.SequenceExpression;
    public List<ExpressionSyntax> Elements { get; }
    public override string ReturnType => "sequence";//cambiar
    public List<object> ElementsEvaluation { get; }

    public object Count
    {
        get
        {
            return ElementsEvaluation.Count;
        }
    }

    public SequenceExpressionSyntax(List<ExpressionSyntax> elements)
    {
        Elements = elements;
        ElementsEvaluation = new();
        ElementsEvaluation.AddRange(Elements);
    }

    public SequenceExpressionSyntax(List<object> elementsEvaluation)
    {
        ElementsEvaluation = elementsEvaluation;
        Elements = new();
    }

    public override object Evaluate(Scope scope)
    {
        List<object> values = new();

        foreach (var item in Elements)
        {
            values.Add(item.Evaluate(scope));
        }

        return new SequenceExpressionSyntax(values);
    }

    public override bool Checker(Scope scope)
    {
        return true;
    }
}
