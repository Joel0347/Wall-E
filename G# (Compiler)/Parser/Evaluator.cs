using System.Diagnostics.Tracing;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace G_Sharp;

public sealed class Evaluator
{
    private List<(GeometrySyntax, Color)> geometries = new();
    private Dictionary<Type, Func<ExpressionSyntax, object>> evaluations { get; }
    public ExpressionSyntax Root { get; }
    public Scope Scope { get; }

    public Evaluator(ExpressionSyntax root, Scope scope)
    {
        Root = root;
        Scope = scope;
        evaluations = new()
        {
            [typeof(ErrorExpressionSyntax)]         = (x) => "",
            [typeof(LiteralExpressionSyntax)]       = Scope.EvaluateLiteralExpression,
            [typeof(FunctionExpressionSyntax)]      = Scope.EvaluateFunctionExpression,
            [typeof(AssignmentFunctionSyntax)]      = Scope.EvaluateFunctionAssignment,
            [typeof(ConstantExpressionSyntax)]      = Scope.EvaluateConstantExpression,
            [typeof(ConstantAssignmentSyntax)]      = Scope.EvaluateConstantAssignment,
            [typeof(LetInExpressionSyntax)]         = Scope.EvaluateLetInExpression,
            [typeof(ConditionalExpressionSyntax)]   = Scope.EvaluateConditionalExpression, 
            [typeof(Draw)]                          = EvaluateDrawExpression,
            [typeof(UnaryExpressionSyntax)]         = Scope.EvaluateUnaryExpression,
            [typeof(BinaryExpressionSyntax)]        = Scope.EvaluateBinaryExpression,
            [typeof(ParenthesizedExpressionSyntax)] = Scope.EvaluateParenthesizedExpression
        };
    }

    public object Evaluate()
    {
        return EvaluateExpression(Root);
    }
    
    private object EvaluateExpression(ExpressionSyntax node)
    {
        if (evaluations.ContainsKey(node.GetType()))
            return evaluations[node.GetType()](node);
        
        Error.SetError("ns", "lqs");
        return 0;
    }

    private object EvaluateDrawExpression(ExpressionSyntax expression)
    {
        var draw = (Draw) expression;
        foreach (var item in draw.Parameters)
        {
            var value = EvaluateExpression(item);
            if (value is ExpressionSyntax val)
            {
                if ((int)val.Kind <= 27 && (int)val.Kind >= 22)
                {
                    var geometryValue = (GeometrySyntax)value;
                    geometries.Add((geometryValue, draw.Color));
                }
            }
        }

        return geometries;
    }
}