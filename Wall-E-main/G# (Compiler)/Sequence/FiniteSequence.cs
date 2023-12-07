using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G_Sharp;

public sealed class FiniteSequence : SequenceExpressionSyntax
{
    public List<ExpressionSyntax> Elements { get; }
    public List<object> ElementsEvaluation { get; }

    public override object Count
    {
        get
        {
            return ElementsEvaluation.Count;
        }
    }

    public override object this[int index]
    {
        get { return ElementsEvaluation[index]; }
    }

    public FiniteSequence(List<ExpressionSyntax> elements)
    {
        Elements = elements;
        ElementsEvaluation = new();
        ElementsEvaluation.AddRange(Elements);
    }

    public FiniteSequence(List<object> elementsEvaluation)
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

        return new FiniteSequence(values);
    }

    public override bool Checker(Scope scope)
    {
        return true;
    }

    public override SequenceExpressionSyntax RestOfSequence(int startIndex)
    {
        List<object> elements = new();

        for (int i = startIndex; i < ElementsEvaluation.Count; i++)
            elements.Add(ElementsEvaluation[i]);

        return new FiniteSequence(elements);
    }
}
