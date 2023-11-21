using System.Diagnostics.Tracing;

namespace G_Sharp;

public sealed class Evaluator
{
    private Dictionary<string, object> Variables = new();

    private static readonly List<object> DefaultFalseValues = new() {
        "{}", "undefined", 0.0
    };
    public ExpressionSyntax Root { get; }
    public Evaluator(ExpressionSyntax root, Dictionary<string, object> variables)
    {
        Root = root;
        Variables = variables;
    }

    public object Evaluate()
    {
        return EvaluateExpression(Root);
    }

    private object EvaluateExpression(ExpressionSyntax node)
    {
        if (node is LiteralExpressionSyntax node1)
            return (double) node1.LiteralToken.Value;

        if (node is NameExpressionSyntax node2)
        {
            if (Variables.ContainsKey(node2.IdentifierToken.Text))
            {
                return Variables[node2.IdentifierToken.Text];
            }
            
            Error.SetError($"!!SYNTAX ERROR: Constant '{node2.IdentifierToken.Text}' is not defined yet");
            return 0.0;
        }    
        
        if (node is AssignmentExpressionSyntax node3)
        {
            var value = EvaluateExpression(node3.Expression);

            if (!Variables.ContainsKey(node3.IdentifierToken.Text))
            {
                Variables[node3.IdentifierToken.Text] = value;
                return value;
            }

            Error.SetError($"!!SYNTAX ERROR: Constant '{node3.IdentifierToken.Text}' is already defined");
            return 0.0;
        }

        if (node is UnaryExpressionSyntax node4)
        {
            var operand = (double)EvaluateExpression(node4.Operand);
            
            if (node4.OperatorToken.Kind == SyntaxKind.PlusToken)
                return operand;
            if (node4.OperatorToken.Kind == SyntaxKind.MinusToken)
                return -operand;
            if (node4.OperatorToken.Kind == SyntaxKind.NotKeyword)
                return DefaultFalseValues.Contains(operand) ? 1 : 0;
        }

        if (node is BinaryExpressionSyntax node5)
        {
            var left = (double) EvaluateExpression(node5.Left);
            var right = (double) EvaluateExpression(node5.Right);

            return node5.OperatorToken.Kind switch
            {
                SyntaxKind.PlusToken => left + right,
                SyntaxKind.MinusToken => left - right,
                SyntaxKind.MultToken => left * right,
                SyntaxKind.DivisionToken => left / right,
                SyntaxKind.ModToken => left % right,
                SyntaxKind.AndKeyword =>
                (DefaultFalseValues.Contains(left) || DefaultFalseValues.Contains(right)) ? 0 : 1,
                SyntaxKind.OrKeyword =>
                (DefaultFalseValues.Contains(left) && DefaultFalseValues.Contains(right)) ? 0 : 1,
                SyntaxKind.EqualToken => (left == right) ? 1 : 0,
                SyntaxKind.DifferentToken => (left != right) ? 1 : 0,
                SyntaxKind.GreatherToken => (left > right) ? 1 : 0,
                SyntaxKind.LessToken => (left < right) ? 1 : 0,
                SyntaxKind.GreatherOrEqualToken => (left >= right) ? 1 : 0,
                SyntaxKind.LessOrEqualToken => (left <= right) ? 1 : 0,
                _ => throw new Exception("sdakdn")
            };
        }

        if (node is ParenthesizedExpressionSyntax node6)
        {
            return EvaluateExpression(node6.Expression);
        }

        Error.SetError("lqs");
        return 0;
    }
}