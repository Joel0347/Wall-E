using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace G_Sharp;
public static class SemanticChecker
{
    public static bool Check(this Scope scope, ExpressionSyntax node)
    {
        return scope.CheckExpression(node);
    }

    private static bool CheckExpression(this Scope scope, ExpressionSyntax node)
    {
        return node.Checker(scope);
    }
}

