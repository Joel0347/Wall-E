using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G_Sharp;

public sealed class InfiniteIntegerSequence : SequenceExpressionSyntax
{
    public override object this[int index]
    {
        get
        {
            return First + index;
        }
    }

    private long First { get; }
    private long Last { get; }

    public override object Count => Last == long.MaxValue ? null! : Last;

    public InfiniteIntegerSequence(long first, long last)
    {
        First = first;
        Last = last;
    }

    public override bool Checker(Scope scope)
    {
        return true;
    }

    public override object Evaluate(Scope scope)
    {
        return new InfiniteIntegerSequence(First, Last);
    }

    public override SequenceExpressionSyntax RestOfSequence(int startIndex)
    {
        return new InfiniteIntegerSequence(startIndex, Last);
    }
}

