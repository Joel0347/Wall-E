using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G_Sharp;

public sealed class InfiniteSequence : SequenceExpressionSyntax
{
    public override object Count => null!;
    public Func<object> Func { get; }
    private Dictionary<int, object> Elements { get; }
    public override string ValuesType 
    {
        get 
        {
            if (Elements.Count == 0) return "undefined";
            return SemanticCheck.GetType(Elements[0]);
        }
    }

    public override object this[int index]
    {
        get
        {
            if (!Elements.ContainsKey(index))
                Elements[index] = Func();

            return Elements[index];
        }
    }

    public InfiniteSequence(Func<object> func, Dictionary<int, object> elements)
    {
        Func = func;
        Elements = elements;
    }

    public override bool Checker(Scope scope)
    {
        if (Elements.Count == 0) return true;

        string type = SemanticCheck.GetType(Elements[0]);

        foreach (var element in Elements)
        {
            if (SemanticCheck.GetType(element) != type)
                return false;
        }

        return true;
    }

    public override object Evaluate(Scope scope)
    {
        return null!;
    }

    public override SequenceExpressionSyntax RestOfSequence(int startIndex)
    {
        return new InfiniteSequence(Func, Elements);
    }
}