using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace G_Sharp;

public sealed class MultipleAssignmentSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.ConstantAssignmentExpression;

    public override string ReturnType => "void expression";

    public List<SyntaxToken> Identifiers { get; }
    public SyntaxToken AssignmentToken { get; }
    public ExpressionSyntax Expression { get; }

    public MultipleAssignmentSyntax(
        List<SyntaxToken> identifiers, SyntaxToken assignmentToken, 
        ExpressionSyntax expression
    )
    {
        Identifiers = identifiers;
        AssignmentToken = assignmentToken;
        Expression = expression;
    }

    public override bool Checker(Scope scope)
    {
        foreach (var item in Identifiers)
        {
            string name = item.Text;

            if (!Expression.Checker(scope))
                return false;

            if (Expression.ReturnType != "sequence" && Expression.ReturnType != "undefined")
            {
                Error.SetError("SEMANTIC", $"Line '{AssignmentToken.Line}' : " +
                                $"Multiple constants can only be assignment to elements in a sequence");
                return false;
            }

            if (scope.Constants.ContainsKey(name))
            {
                Error.SetError("SYNTAX", $"Line '{item.Line}' : Constant '{name}' is already defined");
                return false;
            }

            if (name != "_")
                scope.Constants[name] = new Constant(Expression);
        }

        return true;
    }

    public override object Evaluate(Scope scope)
    {
        SequenceExpressionSyntax sequence = (SequenceExpressionSyntax) Expression.Evaluate(scope);

        long count = Identifiers.Count;
        if (sequence.Count is not null)
            count = long.Parse(sequence.Count.ToString()!);

        for (int i = 0; i < Identifiers.Count - 1; i++)
        {
            string name = Identifiers[i].Text;

            if (i < count)
            {
                if (name != "_")
                    scope.Constants[name] = new Constant(sequence[i]);
            }

            else
            {
                scope.Constants[name] = new Constant(null!);
            }
        }

        var lastId = Identifiers.Last().Text;
        if (lastId != "_")
        {
            int startIndex = (int)Math.Min(count, Identifiers.Count);
            var rest = sequence.RestOfSequence(startIndex);
            scope.Constants[lastId] = new Constant(rest);
        }

        return "";
    }
}
