using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Drawing;

namespace G_Sharp;

public static class EvaluationSupplies
{
    public static readonly List<object> DefaultFalseValues = new() {
        "{}", "undefined", 0.0
    };

    private static readonly Dictionary<SyntaxKind, Func<object, object, double>> BinaryOperationEvaluation = new()
    {
        // numeric operations
        [SyntaxKind.PlusToken]     = (left, right) => (double)left + (double)right,
        [SyntaxKind.MinusToken]    = (left, right) => (double)left - (double)right,
        [SyntaxKind.MultToken]     = (left, right) => (double)left * (double)right,
        [SyntaxKind.DivisionToken] = Division,
        [SyntaxKind.ModToken]      = Module,

        // booleans operations
        [SyntaxKind.AndKeyword] = (left, right) => (DefaultFalseValues.Contains(left) || DefaultFalseValues.Contains(right)) ? 0 : 1,
        [SyntaxKind.OrKeyword]  = (left, right) => (DefaultFalseValues.Contains(left) && DefaultFalseValues.Contains(right)) ? 0 : 1,

        [SyntaxKind.EqualToken]          = (left, right) => (left == right) ? 1 : 0,
        [SyntaxKind.DifferentToken]      = (left, right) => (left != right) ? 1 : 0,
        [SyntaxKind.GreaterToken]        = (left, right) => ((double)left > (double)right) ? 1 : 0,
        [SyntaxKind.LessToken]           = (left, right) => ((double)left < (double)right) ? 1 : 0,
        [SyntaxKind.GreaterOrEqualToken] = (left, right) => ((double)left >= (double)right) ? 1 : 0,
        [SyntaxKind.LessOrEqualToken]    = (left, right) => ((double)left <= (double)right) ? 1 : 0,
    };

    private static readonly Dictionary<SyntaxKind, Func<object, double>> UnaryOperationEvaluation = new()
    {
        [SyntaxKind.PlusToken]  = (operand) => (double)operand,
        [SyntaxKind.MinusToken] = (operand) => -(double)operand,
        [SyntaxKind.NotKeyword] = (operand) => DefaultFalseValues.Contains(operand) ? 1 : 0,
    };

    private static readonly Dictionary<string, 
    Func<string, Scope, List<ExpressionSyntax>, object>> GeometricFunctionEvaluations = new()
    {
        ["line"]    = EvaluateAllKindOfLine,
        ["segment"] = EvaluateAllKindOfLine,
        ["ray"]     = EvaluateAllKindOfLine,
        ["circle"]  = EvaluateCircle,
        ["arc"]     = EvaluateArc,
        ["measure"] = EvaluateMeasure
    };
    
    private static object EvaluateExpression(this Scope scope, ExpressionSyntax expression)
    {
        var evaluator = new Evaluator(expression, scope);
        return evaluator.Evaluate();
    }

    public static object EvaluateLiteralExpression(this Scope scope, ExpressionSyntax expression)
    {
        var literal = (LiteralExpressionSyntax) expression;
        return literal.LiteralToken.Value;
    }

    public static object EvaluateFunctionExpression(this Scope scope, ExpressionSyntax expression)
    {
        var functionExpression = (FunctionExpressionSyntax) expression;
        var functionToken = functionExpression.IdentifierToken;
        var values = functionExpression.Values;
        string name = functionToken.Text;

        if (!scope.Functions.TryGetValue(name, out Function? function))
        {
            if (functionToken.Kind != SyntaxKind.GeometryKeyword)
            {
                Error.SetError("SEMANTIC", $"Function '{name}' is not defined yet");
                return "";
            }
        }

        Dictionary<string, Constant> constants = new();
        
        for (int i = 0; i < values.Count; i++)
        {
            ConstantExpressionSyntax parameter = (ConstantExpressionSyntax)function!.Parameters[i];
            var value = scope.EvaluateExpression(values[i]);
            var identifier = parameter.IdentifierToken;
            constants[identifier.Text] = new Constant(value);
        }

        foreach (var item in scope.Constants.Keys)
        {
            if (!constants.ContainsKey(item))
                constants[item] = scope.Constants[item];
        }

        Scope child = new(constants, scope.Functions);

        if (functionToken.Kind == SyntaxKind.GeometryKeyword)
            return EvaluateGeometricFunctions(child, functionToken.Text, values);

        var evaluator = new Evaluator(function!.Body, child);
        return evaluator.Evaluate();
    }

    public static object EvaluateLetInExpression(this Scope scope, ExpressionSyntax expression)
    {
        var letInExpression = (LetInExpressionSyntax) expression;
        Dictionary<string, Constant> internalConstants = new();
        Dictionary<string, Function> internalFunctions = new();

        foreach (string key in scope.Constants.Keys)
        {
            internalConstants[key] = scope.Constants[key];
        }

        foreach (string key in scope.Functions.Keys)
        {
            internalFunctions[key] = scope.Functions[key];
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

    public static object EvaluateConditionalExpression(this Scope scope, ExpressionSyntax expression)
    {
        var conditional = (ConditionalExpressionSyntax) expression;

        //var valueTrue = scope.EvaluateExpression(conditional.BodyTrue);
        //var valueFalse = scope.EvaluateExpression(conditional.BodyFalse);

        //string TypeTrue = SemanticCheck.GetType(valueTrue);
        //string TypeFalse = SemanticCheck.GetType(valueFalse);

        //if (TypeTrue == TypeFalse) 
        //{
        var condition = scope.EvaluateExpression(conditional.Condition);

        if(DefaultFalseValues.Contains(condition))
        {
            return scope.EvaluateExpression(conditional.BodyFalse);
        }

        return scope.EvaluateExpression(conditional.BodyTrue);
        //}

        //Error.SetError("SEMANTIC", "Conditional must return the same type of value in the 'then' clause and the 'else' clause");
        //return "";


    }

    public static object EvaluateFunctionAssignment(this Scope scope, ExpressionSyntax expression)
    {
        var funcAssignment = (AssignmentFunctionSyntax) expression;
        string name = funcAssignment.FunctionIdentifierToken.Text;
        var body = funcAssignment.Expression;
        var parameters = funcAssignment.IdentifiersToken;

        if (!scope.Functions.ContainsKey(name))
            scope.Functions[name] = new Function(body, parameters);

        else Error.SetError("SYNTAX", $"Function '{name}' is already defined");

        return "";
    }

    public static object EvaluateConstantExpression(this Scope scope, ExpressionSyntax expression)
    {
        var constant = (ConstantExpressionSyntax) expression;
        string name = constant.IdentifierToken.Text;

        if (scope.Constants.TryGetValue(name, out Constant? value))
            return value.Expression;

        Error.SetError("SYNTAX", $"Constant '{name}' is not defined yet");
        return "";
    }

    public static object EvaluateConstantAssignment(this Scope scope, ExpressionSyntax expression)
    {
        var constantAssignment = (ConstantAssignmentSyntax) expression;
        string name = constantAssignment.IdentifierToken.Text;
        object value = constantAssignment.Expression;

        if ((int)constantAssignment.Expression.Kind < 22 || (int)constantAssignment.Expression.Kind > 27)
        {  
            value = scope.EvaluateExpression(constantAssignment.Expression);

            if (value.Equals("")) {
                Error.SetError("SEMANTIC", $"Constant '{name}' can't be assigned to statement");
                return "";
            }
        }

        if (!scope.Constants.ContainsKey(name))
        {
            scope.Constants[name] = new Constant(value);
            return value;
        }

        Error.SetError("SYNTAX", $"Constant '{name}' is already defined");
        return "";
    }

    public static object EvaluateUnaryExpression(this Scope scope, ExpressionSyntax expression)
    {
        var unary = (UnaryExpressionSyntax) expression;
        var operand = scope.EvaluateExpression(unary.Operand);

        if (!SemanticCheck.UnaryOperatorsCheck[unary.OperatorToken.Kind](operand))
        {
            var operandType = SemanticCheck.GetType(operand);
            var operation = unary.OperatorToken.Text;
            Error.SetError("SEMANTIC", $"Operator '{operation}' can't not be used before '{operandType}'");
            return "";
        }

        return UnaryOperationEvaluation[unary.OperatorToken.Kind](operand);
    }

    public static object EvaluateBinaryExpression(this Scope scope, ExpressionSyntax expression)
    {
        var binary = (BinaryExpressionSyntax) expression;

        var left = scope.EvaluateExpression(binary.Left);
        var right = scope.EvaluateExpression(binary.Right);
        var operation = binary.OperatorToken;
        var operatorKind = operation.Kind;


        if (!SemanticCheck.BinaryOperatorsCheck[operatorKind](left, right))
        {
            string leftType = SemanticCheck.GetType(left);
            string rightType = SemanticCheck.GetType(right);
            Error.SetError("SEMANTIC", $"Operator '{operation.Text}' can't be used between '{leftType}' and '{rightType}'");
            return "";
        }

        return BinaryOperationEvaluation[operatorKind](left, right);
    }

    public static object EvaluateParenthesizedExpression(this Scope scope, ExpressionSyntax expression)
    {
        var parenthesized = (ParenthesizedExpressionSyntax) expression;
        return scope.EvaluateExpression(parenthesized.Expression);
    }

    private static double Division(object left, object right)
    {
        if ((double)right == 0)
        {
            Error.SetError("SEMANTIC", "Division by '0' is not defined");
            return 0;
        }

        return (double)left / (double)right;
    }

    private static double Module(object left, object right)
    {
        if ((double)right == 0)
        {
            Error.SetError("SEMANTIC", "Division by '0' is not defined");
            return 0;
        }

        return (double)left % (double)right;
    }

    private static Points[] GetPoints(Scope scope, List<ExpressionSyntax> parameters, int quantity)
    {
        Points[] points = new Points[quantity];

        for (int i = 0; i < quantity; i++)
        {
            var point1Identifier = (ConstantExpressionSyntax)parameters[i];
            string id = point1Identifier.IdentifierToken.Text;
            var point = (Points)scope.Constants[id].Expression;
            points[i] = point;
        }

        return points;
    }

    private static object EvaluateAllKindOfLine(string id, Scope scope, List<ExpressionSyntax> parameters)
    {
        Points[] points = GetPoints(scope, parameters, 2);
        Points point1 = points[0];
        Points point2 = points[1];

        GeometrySyntax anyLine;

        if (id == "segment")
            anyLine = new Segment(point1, point2);

        else if (id == "line")
            anyLine = new Line(point1, point2);

        else 
            anyLine = new Ray(point1, point2);

        return anyLine;
    }

    private static object EvaluateCircle(string id, Scope scope, List<ExpressionSyntax> parameters)
    {
        Points[] points = GetPoints(scope, parameters, 1);
        Points center = points[0];
        var measure = (Measure)EvaluateExpression(scope, parameters[1]); ;
        
        return new Circle(center, measure.Value);
    }

    private static object EvaluateArc(string id, Scope scope, List<ExpressionSyntax> parameters)
    {
        Points[] points = GetPoints(scope, parameters, 3);
        var point1 = points[0];
        var point2 = points[1];
        var point3 = points[2];
        var measure = (Measure)EvaluateExpression(scope, parameters[3]);
        
        return new Arc(point1, point2, point3, measure.Value);
    }

    private static object EvaluateMeasure(string id, Scope scope, List<ExpressionSyntax> parameters)
    {
        Points[] points = GetPoints(scope, parameters, 2);
        var point1 = points[0];
        var point2 = points[1];

        return new Measure(point1, point2);
    }

    private static object EvaluateGeometricFunctions(
        Scope scope, string identifier, List<ExpressionSyntax> parameters
    )
    {
        return GeometricFunctionEvaluations[identifier](identifier, scope, parameters);
    }
}