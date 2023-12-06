namespace G_Sharp;

public sealed class LetInExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.LetInExpression;
    public SyntaxToken LetToken { get; }
    public List<ExpressionSyntax> Instructions { get; }
    public SyntaxToken InToken { get; }
    public override string ReturnType => Instructions.Last().ReturnType;

    private Scope internalScope = null!;

    public LetInExpressionSyntax(
        SyntaxToken letToken, List<ExpressionSyntax> instructions, 
        SyntaxToken inToken, ExpressionSyntax body
    )
    {
        LetToken = letToken;
        Instructions = instructions;
        Instructions.Add(body);
        InToken = inToken;
    }

    public override object Evaluate(Scope scope)
    {
        Scope child;
        if (internalScope is not null)
            child = GetInternalScope(internalScope);

        else
        {
            internalScope = null!;
            child = GetInternalScope(scope);
        }

        object result = null!;

        foreach (var statement in Instructions)
            result = child.Evaluate(statement);

        return result;
    }

    public override bool Checker(Scope scope)
    {
        internalScope = GetInternalScope(scope);

        foreach (var statement in Instructions)
        {
            var check = internalScope.Check(statement);
            if (!check) return false;
        }

        return true;
    }

    private Scope GetInternalScope(Scope scope)
    {
        Dictionary<string, Constant> internalConstants = new();
        Dictionary<string, Function> internalFunctions = new();

        foreach (string key in scope.Constants.Keys)
            internalConstants[key] = scope.Constants[key];

        foreach (string key in scope.Functions.Keys)
            internalFunctions[key] = scope.Functions[key];

        return new(internalConstants, internalFunctions);
    }
}