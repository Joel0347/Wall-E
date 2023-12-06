using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G_Sharp;

public sealed class FiniteSequence<T> : SequenceExpressionSyntax
{
    public List<T> Elements { get; }
    //public List<object> ElementsEvaluation { get; }

    public override object Count
    {
        get
        {
            //return ElementsEvaluation.Count;
            return Elements.Count;
        }
    }
    public override string ValuesType
    {
        get
        {
            if (Elements.Count == 0) return "undefined";
            return SemanticCheck.GetType(Elements[0]!);
        }
    }

    public override object this[int index]
    {
        //get { return ElementsEvaluation[index]; }
        get { return Elements[index]!; }
    }

    public FiniteSequence(List<T> elements)
    {
        Elements = elements;
        //ElementsEvaluation = new();
        //ElementsEvaluation.AddRange(Elements);
    }

    //public FiniteSequence(List<object> elementsEvaluation)
    //{
    //    ElementsEvaluation = elementsEvaluation;
    //    Elements = new();
    //}

    public override object Evaluate(Scope scope)
    {
        List<object> values = new();

        foreach (var item in Elements as List<ExpressionSyntax>)
        {
            values.Add(item.Evaluate(scope));
        }

        return new FiniteSequence<object>(values);
    }

    public override bool Checker(Scope scope)
    {
        var elements = Elements as List<ExpressionSyntax>;
        if (Count.Equals(0)) return true;

        if (!elements![0].Checker(scope)) return false;
        string type = SemanticCheck.GetType(Elements[0]!);

        foreach (var element in elements)
        {
            if (!element!.Checker(scope) || SemanticCheck.GetType(element) != type)
                return false;
        }

        return true;
    }

    public override SequenceExpressionSyntax RestOfSequence(int startIndex)
    {
        List<object> elements = new();

        for (int i = startIndex; i < Elements.Count; i++)
            elements.Add(Elements[i]!);

        return new FiniteSequence<object>(elements);
    }
}
