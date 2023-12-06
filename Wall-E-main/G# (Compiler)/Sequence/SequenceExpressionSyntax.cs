using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G_Sharp;

public abstract class SequenceExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.SequenceExpression;
    public abstract object Count { get; }
    public override string ReturnType => "sequence";
    public abstract string ValuesType { get; }
    public abstract object this[int index] { get; }
    public abstract SequenceExpressionSyntax RestOfSequence(int startIndex);
}