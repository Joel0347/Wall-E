using System.Data;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;

namespace G_Sharp;

#region Interface que caracteriza todas las expresiones del programa
public abstract class ExpressionSyntax 
{
    public abstract SyntaxKind Kind { get; }
    public abstract string ReturnType { get; }
    public abstract object Evaluate(Scope scope);
    public abstract bool Check(Scope scope);
}

#endregion


// Objeto que permite devolver errores
public sealed class ErrorExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.ErrorToken;

    public override string ReturnType => "error";

    public ErrorExpressionSyntax() { }

    public override object Evaluate(Scope scope)
    {
        return "";
    }

    public override bool Check(Scope scope)
    {
        return false;
    }
}