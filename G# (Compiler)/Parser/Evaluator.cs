using System.Diagnostics.Tracing;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace G_Sharp;

public sealed class Evaluator
{
    private List<(GeometrySyntax, Color)> geometries = new();
  
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
        if (node is ErrorExpressionSyntax)
            return "";

        if (node is LiteralExpressionSyntax node1)
            return node1.LiteralToken.Value;

        if (node is FunctionExpressionSyntax node2)
        {
            return EvaluateFunctionExpression(node2.IdentifierToken, node2.Values);
        }

        if (node is AssignmentFunctionExpressionSyntax node3)
        {
            string name = node3.FunctionIdentifierToken.Text;

            if (Scope.Functions.ContainsKey(name))
            {
                Error.SetError("SYNTAX", $"Function '{name}' is already defined");
                return "";
            }

            if (!Error.Wrong)
            {
                Scope.Functions[name] = new Function(node3.Expression, node3.IdentifiersToken);
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

        if (node is LetInExpressionSyntax node5)
        {
            return LetInExpressionEvaluation(node5);
        }

        if (node is NameExpressionSyntax node6)
        {
            string name = node6.IdentifierToken.Text;

            if (Scope.Constants.TryGetValue(name, out Constant? value))
            {
                return value.Expression;
            }

            Error.SetError("SYNTAX", $"Constant '{name}' is not defined yet");
            return "";
        }    
        
        if (node is AssignmentExpressionSyntax node7)
        {
            string name = node7.IdentifierToken.Text;
            object value = node7.Expression;

            if ((int)node7.Expression.Kind < 22 || (int)node7.Expression.Kind > 27)
            {
                value = EvaluateExpression(node7.Expression);
            }

            if (!Scope.Constants.ContainsKey(name))
            {
                Scope.Constants[name] = new Constant(value);
                return value;
            }

            Error.SetError("SYNTAX", $"Constant '{name}' is already defined");
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

            return EvaluatorFacts.UnaryOperationEvaluation[node8.OperatorToken.Kind](operand);
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

            return EvaluatorFacts.BinaryOperationEvaluation[operatorKind](left, right);
        }

        if (node is ParenthesizedExpressionSyntax node10)
        {
            return EvaluateExpression(node10.Expression);
        }

        Error.SetError("ns", "lqs");
        return 0;
    }

    private object EvaluateFunctionExpression(SyntaxToken functionToken, List<ExpressionSyntax> values)
    {
        string name = functionToken.Text;

        if (!Scope.Functions.TryGetValue(name, out Function? function))
        {
            if (functionToken.Kind != SyntaxKind.GeometryKeyword)
            {
                Error.SetError("SEMANTIC", $"Function '{name}' is not defined yet");
                return "";
            }
        }

        Dictionary<string, Constant> constants = new();

        if (functionToken.Kind != SyntaxKind.GeometryKeyword)
        {
            for (int i = 0; i < values.Count; i++)
            {
                NameExpressionSyntax variable = (NameExpressionSyntax)function!.Parameters[i];
                var expression = EvaluateExpression(values[i]);
                var identifier = variable.IdentifierToken;
                constants[identifier.Text] = new Constant(expression);
            }
        }

        foreach (var item in Scope.Constants.Keys)
        {
            if (!constants.ContainsKey(item))
                constants[item] = Scope.Constants[item];
        }

        Scope child = new(constants, Scope.Functions);

        if (functionToken.Kind == SyntaxKind.GeometryKeyword)
        {
            var typeGeometry = functionToken;

            if (typeGeometry.Text == "segment")
            {
                var point1Identifier = (NameExpressionSyntax) values[0];
                var point1 = (Points)Scope.Constants[point1Identifier.IdentifierToken.Text].Expression;
                var point2Identifier = (NameExpressionSyntax) values[1];
                var point2 = (Points)Scope.Constants[point2Identifier.IdentifierToken.Text].Expression;

                var segment = new Segment(point1, point2);

                return segment;
            }


            if (typeGeometry.Text == "line")
            {
                var point1Identifier = (NameExpressionSyntax)values[0];
                var point1 = (Points)Scope.Constants[point1Identifier.IdentifierToken.Text].Expression;
                var point2Identifier = (NameExpressionSyntax)values[1];
                var point2 = (Points)Scope.Constants[point2Identifier.IdentifierToken.Text].Expression;

                var line = new Line(point1, point2);

                return line;

            }

            if (typeGeometry.Text == "ray")
            {
                var point1Identifier = (NameExpressionSyntax)values[0];
                var point1 = (Points)Scope.Constants[point1Identifier.IdentifierToken.Text].Expression;
                var point2Identifier = (NameExpressionSyntax)values[1];
                var point2 = (Points)Scope.Constants[point2Identifier.IdentifierToken.Text].Expression;

                var ray = new Ray(point1, point2);

                return ray;
            }

            if (typeGeometry.Text == "circle")
            {
                var centerIdentifier = (NameExpressionSyntax) values[0];
                var center = (Points)Scope.Constants[centerIdentifier.IdentifierToken.Text].Expression;
                var measureIdentifier = (NameExpressionSyntax)values[1];
                var measure = (float)Scope.Constants[measureIdentifier.IdentifierToken.Text].Expression;
                var circle = new Circle(center, measure);

                return circle;
            }

            if (typeGeometry.Text == "arc")
            {
                var point1Identifier = (NameExpressionSyntax)values[0];
                var point1 = (Points)Scope.Constants[point1Identifier.IdentifierToken.Text].Expression;
                var point2Identifier = (NameExpressionSyntax)values[1];
                var point2 = (Points)Scope.Constants[point2Identifier.IdentifierToken.Text].Expression;
                var point3Identifier = (NameExpressionSyntax)values[1];
                var point3 = (Points)Scope.Constants[point3Identifier.IdentifierToken.Text].Expression;
                var measureIdentifier = (NameExpressionSyntax)values[3];
                var measure = (float)Scope.Constants[measureIdentifier.IdentifierToken.Text].Expression;

                var arc = new Arc(point1, point2, point3, measure);

                return arc;
            }
        }

        var evaluator = new Evaluator(function!.Body, child);
        return evaluator.Evaluate();
    }

    private object LetInExpressionEvaluation(LetInExpressionSyntax letInExpression)
    {
        Dictionary<string, Constant> internalConstants = new();
        Dictionary<string, Function> internalFunctions = new();

        foreach (string key in Scope.Constants.Keys)
        {
            internalConstants[key] = Scope.Constants[key];
        }

        foreach (string key in Scope.Functions.Keys)
        {
            internalFunctions[key] = Scope.Functions[key];
        }

        Scope internalScope = new(internalConstants, internalFunctions);

        object result = null!;

        foreach (var statement in letInExpression.Instructions)
        {
            var evaluation = new Evaluator(statement, internalScope);
            result = evaluation.Evaluate();
        }

        return result;
    }
}