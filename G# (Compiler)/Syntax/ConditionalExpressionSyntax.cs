namespace G_Sharp;

public sealed class ConditionalExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.ConditionalExpression;

    public SyntaxToken IfKeyword { get; }
    public ExpressionSyntax Condition { get; }
    public SyntaxToken ThenKeyword { get; }
    public ExpressionSyntax BodyTrue { get; }
    public SyntaxToken ElseKeyword { get; }
    public ExpressionSyntax BodyFalse { get; }
    public override string ReturnType => BodyTrue.ReturnType;

    public ConditionalExpressionSyntax(
        SyntaxToken ifKeyword, ExpressionSyntax condition, SyntaxToken thenKeyword, 
        ExpressionSyntax bodyTrue, SyntaxToken elseKeyword, ExpressionSyntax bodyFalse
    )
    {
        IfKeyword = ifKeyword;
        Condition = condition;
        ThenKeyword = thenKeyword;
        BodyTrue = bodyTrue;
        ElseKeyword = elseKeyword;
        BodyFalse = bodyFalse;

    }

    public override object Evaluate(Scope scope)
    {
        //var valueTrue = scope.EvaluateExpression(conditional.BodyTrue);
        //var valueFalse = scope.EvaluateExpression(conditional.BodyFalse);

        //string TypeTrue = SemanticCheck.GetType(valueTrue);
        //string TypeFalse = SemanticCheck.GetType(valueFalse);

        //if (TypeTrue == TypeFalse) 
        //{
        var condition = Condition.Evaluate(scope);

        if (Evaluator.DefaultFalseValues.Contains(condition))
        {
            return BodyFalse.Evaluate(scope);
        }

        return BodyTrue.Evaluate(scope);
        //}

        //Error.SetError("SEMANTIC", "Conditional must return the same type of value in the 'then' clause and the 'else' clause");
        //return "";
    }

    public override bool Checker(Scope scope)
    {
        throw new NotImplementedException();
    }
}