namespace G_Sharp;

public sealed class SyntaxTree
{
    public bool Error { get; }
    public ExpressionSyntax Root { get; }
    public SyntaxToken EndOfFileToken { get; }

    public SyntaxTree(bool error, ExpressionSyntax root, SyntaxToken endOfFileToken)
    {
        Error = error;
        Root = root;
        EndOfFileToken = endOfFileToken;
    }

    public static SyntaxTree Parse(string text)
    {
        var parser = new Parser(text);
        return parser.Parse();
    }
}