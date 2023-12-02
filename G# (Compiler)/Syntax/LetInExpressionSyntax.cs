namespace G_Sharp;

public sealed class LetInExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.LetInExpression;
    public SyntaxToken LetToken { get; }
    public List<ExpressionSyntax> Instructions { get; }
    public SyntaxToken InToken { get; }
    public override string ReturnType => Instructions.Last().ReturnType;

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
        Dictionary<string, Constant> internalConstants = new();
        Dictionary<string, Function> internalFunctions = new();

        foreach (string key in scope.Constants.Keys)
            internalConstants[key] = scope.Constants[key];

        foreach (string key in scope.Functions.Keys)
            internalFunctions[key] = scope.Functions[key];

        Scope internalScope = new(internalConstants, internalFunctions);

        object result = null!;

        foreach (var statement in Instructions)
            result = internalScope.Evaluate(statement);

        return result;
    }

    public override bool Checker(Scope scope)
    {
        foreach (var statement in Instructions)
        {
            var check = scope.Check(statement);
            if (!check) return false;
        }

        return true;
    }
}