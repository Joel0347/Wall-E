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

    public static readonly Dictionary<string, 
    Func<string, Scope, List<ExpressionSyntax>, object>> GeometricFunctionEvaluations = new()
    {
        ["line"]    = EvaluateAllKindOfLine,
        ["segment"] = EvaluateAllKindOfLine,
        ["ray"]     = EvaluateAllKindOfLine,
        ["circle"]  = EvaluateCircle,
        ["arc"]     = EvaluateArc,
        ["measure"] = EvaluateMeasure
    };
    
    public static object EvaluateExpression(this Scope scope, ExpressionSyntax expression)
    {
        var evaluator = new Evaluator(expression, scope);
        return evaluator.Evaluate();
    }

    

    public static Points[] GetPoints(Scope scope, List<ExpressionSyntax> parameters, int quantity)
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
        var measure = (Measure)scope.EvaluateExpression(parameters[1]);
        
        return new Circle(center, measure);
    }

    private static object EvaluateArc(string id, Scope scope, List<ExpressionSyntax> parameters)
    {
        Points[] points = GetPoints(scope, parameters, 3);
        var point1 = points[0];
        var point2 = points[1];
        var point3 = points[2];
        var measure = (Measure)scope.EvaluateExpression(parameters[3]);
        
        return new Arc(point1, point2, point3, measure);
    }

    private static object EvaluateMeasure(string id, Scope scope, List<ExpressionSyntax> parameters)
    {
        Points[] points = GetPoints(scope, parameters, 2);
        var point1 = points[0];
        var point2 = points[1];

        return new Measure(point1, point2);
    }
}