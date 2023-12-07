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
        return true;
    }

    public override object Evaluate(Scope scope)
    {
        return null!;
    }

    public override SequenceExpressionSyntax RestOfSequence(int startIndex)
    {
        for (int i = 0; i < startIndex; i++)
            Elements.Remove(i);

        return new InfiniteSequence(Func, Elements);
    }
}