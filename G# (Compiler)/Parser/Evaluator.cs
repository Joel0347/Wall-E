using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;

namespace G_Sharp;

public sealed class Evaluator
{
    private List<(GeometrySyntax, Color)> geometries = new();

    private static readonly List<object> DefaultFalseValues = new() {
        "{}", "undefined", 0.0
    };

    private static readonly Dictionary<SyntaxKind, Func<object, object, double>> BinaryOperationEvaluation = new()
    {
        // numeric operations
        [SyntaxKind.PlusToken] = (left, right) => (double)left + (double)right,
        [SyntaxKind.MinusToken] = (left, right) => (double)left - (double)right,
        [SyntaxKind.MultToken] = (left, right) => (double)left * (double)right,
        [SyntaxKind.DivisionToken] = (left, right) => (double)left / (double)right,
        [SyntaxKind.ModToken] = (left, right) => (double)left % (double)right,

        // booleans operations
        [SyntaxKind.AndKeyword] = (left, right) =>
                                (DefaultFalseValues.Contains(left) || DefaultFalseValues.Contains(right)) ? 0 : 1,
        [SyntaxKind.OrKeyword] = (left, right) =>
                                (DefaultFalseValues.Contains(left) && DefaultFalseValues.Contains(right)) ? 0 : 1,
        [SyntaxKind.EqualToken] = (left, right) => (left == right) ? 1 : 0,
        [SyntaxKind.DifferentToken] = (left, right) => (left != right) ? 1 : 0,
        [SyntaxKind.GreatherToken] = (left, right) => ((double)left > (double)right) ? 1 : 0,
        [SyntaxKind.LessToken] = (left, right) => ((double)left < (double)right) ? 1 : 0,
        [SyntaxKind.GreatherOrEqualToken] = (left, right) => ((double)left >= (double)right) ? 1 : 0,
        [SyntaxKind.LessOrEqualToken] = (left, right) => ((double)left <= (double)right) ? 1 : 0,
    };

    private static readonly Dictionary<SyntaxKind, Func<object, double>> UnaryOperationEvaluation = new()
    {
        [SyntaxKind.PlusToken] = (operand) => (double)operand,
        [SyntaxKind.MinusToken] = (operand) => - (double)operand,
        [SyntaxKind.NotKeyword] = (operand) => DefaultFalseValues.Contains(operand) ? 1 : 0,
    };
    
    public ExpressionSyntax Root { get; }
    public Scope Scope { get; }

    public Evaluator(ExpressionSyntax root, Scope scope)
    {
        Root = root;
        Scope = scope;
    }

    public object Evaluate()
    {
        return EvaluateExpression(Root);
    }
    
    private object EvaluateExpression(ExpressionSyntax node)
    {
        if (node is LiteralExpressionSyntax node1)
            return node1.LiteralToken.Value;

        if (node is FunctionExpressionSyntax node2)
        {
            if (node2.IdentifierToken.Kind != SyntaxKind.GeometryKeyword && !Scope.FunctionsVariables.ContainsKey(node2.IdentifierToken.Text)) {
                Error.SetError("SEMANTIC", $"Function '{node2.IdentifierToken.Text}' is not defined yet");
                return "";
            }

            return EvaluateFunctionExpression(node2.IdentifierToken, node2.Values);
        }

        if (node is AssignmentFunctionExpressionSyntax node3)
        {
            if (Scope.FunctionsVariables.ContainsKey(node3.FunctionIdentifierToken.Text))
            {
                Error.SetError("SYNTAX", $"Function '{node3.FunctionIdentifierToken.Text}' is already defined");
                return "";
            }

            if (!Error.Wrong) {
                Scope.FunctionsBody[node3.FunctionIdentifierToken.Text] = node3.Expression;
                Scope.FunctionsVariables[node3.FunctionIdentifierToken.Text] = node3.IdentifiersToken;
            }

            return "";
        }

        if (node is Draw node4)
        {
            foreach (var item in node4.Parameters)
            {
                var value = EvaluateExpression(item);
                if (value is ExpressionSyntax val)
                {
                    if ((int)val.Kind <= 27 && (int)val.Kind >= 22)
                    {
                        var geometryValue = (GeometrySyntax)value;
                        geometries.Add((geometryValue, node4.Color));
                    }
                }
            }

            return geometries;
        }

        if (node is NameExpressionSyntax node6)
        {
            if (Scope.Variables.ContainsKey(node6.IdentifierToken.Text))
            {
                return Scope.Variables[node6.IdentifierToken.Text];
            }
            
            Error.SetError("SYNTAX", $"Constant '{node6.IdentifierToken.Text}' is not defined yet");
            return "";
        }    
        
        if (node is AssignmentExpressionSyntax node7)
        {
            object value = node7.Expression;

            if ((int)node7.Kind < 22 && (int)node7.Kind > 27)
            {
                value = EvaluateExpression(node7.Expression);
            }

            if (!Scope.Variables.ContainsKey(node7.IdentifierToken.Text))
            {
                Scope.Variables[node7.IdentifierToken.Text] = value;
                return value;
            }

            Error.SetError("SYNTAX", $"Constant '{node7.IdentifierToken.Text}' is already defined");
            return "";
        }

        if (node is UnaryExpressionSyntax node8)
        {
            var operand = EvaluateExpression(node8.Operand);
            if (!SemanticCheck.UnaryOperatorsCheck[node8.OperatorToken.Kind](operand)) {
                var operandType = SemanticCheck.GetType(operand);
                var operation = node8.OperatorToken.Text;
                Error.SetError("SEMANTIC", $"Operator '{operation}' can't not be used before '{operandType}'");
                return "";
            }

            return UnaryOperationEvaluation[node8.OperatorToken.Kind](operand);
        }

        if (node is BinaryExpressionSyntax node9)
        {
            var left = EvaluateExpression(node9.Left);
            var right = EvaluateExpression(node9.Right);
            var operation = node9.OperatorToken;
            var operatorKind = operation.Kind;
            
            
            if (!SemanticCheck.BinaryOperatorsCheck[operatorKind](left, right)) {
                string leftType = SemanticCheck.GetType(left);
                string rightType = SemanticCheck.GetType(right);
                Error.SetError("SEMANTIC", $"Operator '{operation.Text}' can't be used between '{leftType}' and '{rightType}'");
                return "";
            }

            return BinaryOperationEvaluation[operatorKind](left, right);
        }

        if (node is ParenthesizedExpressionSyntax node10)
        {
            return EvaluateExpression(node10.Expression);
        }

        Error.SetError("ns", "lqs");
        return 0;
    }

    private object EvaluateFunctionExpression(SyntaxToken function, List<ExpressionSyntax> values)
    {
        Dictionary<string, object> variables = new();
        string name = function.Text;

        if (function.Kind != SyntaxKind.GeometryKeyword)
        {
            for (int i = 0; i < values.Count; i++)
            {
                NameExpressionSyntax variable = (NameExpressionSyntax)Scope.FunctionsVariables[name][i];
                variables[variable.IdentifierToken.Text] = EvaluateExpression(values[i]);
            }
        }

        foreach (var item in Scope.Variables.Keys)
        {
            if (!variables.ContainsKey(item))  
                variables[item] = Scope.Variables[item];
        }

        Scope child = new(variables, Scope.FunctionsVariables, Scope.FunctionsBody); 

        if (function.Kind == SyntaxKind.GeometryKeyword)
        {
            var typeGeometry = function;

            if (typeGeometry.Text == "segment")
            {
                var point1Identifier = (NameExpressionSyntax) values[0];
                var point1 = (Points)Scope.Variables[point1Identifier.IdentifierToken.Text];
                var point2Identifier = (NameExpressionSyntax) values[1];
                var point2 = (Points)Scope.Variables[point2Identifier.IdentifierToken.Text];

                var segment = new Segment(point1, point2);

                return segment;
            }


            if (typeGeometry.Text == "line")
            {
                var point1Identifier = (NameExpressionSyntax)values[0];
                var point1 = (Points)Scope.Variables[point1Identifier.IdentifierToken.Text];
                var point2Identifier = (NameExpressionSyntax)values[1];
                var point2 = (Points)Scope.Variables[point2Identifier.IdentifierToken.Text];

                var line = new Line(point1, point2);

                return line;

            }

            if (typeGeometry.Text == "ray")
            {
                var point1Identifier = (NameExpressionSyntax)values[0];
                var point1 = (Points)Scope.Variables[point1Identifier.IdentifierToken.Text];
                var point2Identifier = (NameExpressionSyntax)values[1];
                var point2 = (Points)Scope.Variables[point2Identifier.IdentifierToken.Text];

                var ray = new Ray(point1, point2);

                return ray;
            }

            if (typeGeometry.Text == "circle")
            {
                var centerIdentifier = (NameExpressionSyntax) values[0];
                var center = (Points)Scope.Variables[centerIdentifier.IdentifierToken.Text];
                var measureIdentifier = (NameExpressionSyntax)values[1];
                var measure = (float) Scope.Variables[measureIdentifier.IdentifierToken.Text];
                var circle = new Circle(center, measure);

                return circle;
            }

            if (typeGeometry.Text == "arc")
            {
                var point1Identifier = (NameExpressionSyntax)values[0];
                var point1 = (Points)Scope.Variables[point1Identifier.IdentifierToken.Text];
                var point2Identifier = (NameExpressionSyntax)values[1];
                var point2 = (Points)Scope.Variables[point2Identifier.IdentifierToken.Text];
                var point3Identifier = (NameExpressionSyntax)values[1];
                var point3 = (Points)Scope.Variables[point3Identifier.IdentifierToken.Text];
                var measureIdentifier = (NameExpressionSyntax)values[3];
                var measure = (float) Scope.Variables[measureIdentifier.IdentifierToken.Text];

                var arc = new Arc(point1, point2, point3, measure);

                return arc;
            }
        }

        var evaluator = new Evaluator(Scope.FunctionsBody[name], child);
        return evaluator.Evaluate();
    }
}