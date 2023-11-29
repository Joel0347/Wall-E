using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G_Sharp;
public class SemanticChecker
{
    public ExpressionSyntax Root { get; }
    public Scope Scope { get; }

    public SemanticChecker(ExpressionSyntax root, Scope scope)
    {
        Root = root;
        Scope = scope;
    }

    public bool Check()
    {
        return CheckExpression(Root);
    }

    private bool CheckExpression(ExpressionSyntax node)
    {
        return node.Checker(Scope);
    }
}

