using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G_Sharp;

public sealed class ImportExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.ImportKeyword;

    public override string ReturnType => "void expression";

    public SyntaxToken ImportToken { get; }
    public SyntaxTree SyntaxTree { get; }

    public ImportExpressionSyntax(SyntaxToken importToken, SyntaxTree syntaxTree)
    {
        ImportToken = importToken;
        SyntaxTree = syntaxTree;
    }

    public override bool Checker(Scope scope)
    {
        foreach (var root in SyntaxTree.Root)
        {
            var checking = scope.Check(root);
            if (!checking) return false;
        }

        return true;
    }

    public override object Evaluate(Scope scope)
    {
        List<object> obj = new();

        foreach (var root in SyntaxTree.Root)
        {
            var result = scope.Evaluate(root);
            obj.Add(result);
        }

        return obj;
    }
}
