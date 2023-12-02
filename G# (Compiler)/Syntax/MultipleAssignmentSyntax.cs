using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            if (Expression.ReturnType != "sequence")
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
        }

        return true;
    }

    public override object Evaluate(Scope scope)
    {
        Sequence sequence = (Sequence) Expression.Evaluate(scope); ;

        for (int i = 0; i < Identifiers.Count - 1; i++)
        {
            string name = Identifiers[i].Text;

            if (sequence.Values.Count > 0)
            {
                if (name != "_")
                    scope.Constants[name] = new Constant(sequence.Values[0]);
                sequence.Values.RemoveAt(0);
            }
            
            else
            {
                scope.Constants[name] = new Constant(null!);
            }
        }

        scope.Constants[Identifiers.Last().Text] = new Constant(sequence);
        
        return "";
    }
}
