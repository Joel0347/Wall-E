using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G_Sharp;

public static class ScopeSupplies
{
    private static Dictionary<int, object> RandomElements = new();
    private static Dictionary<int, object> RandomPoints = new();
    public static object CircleFunction(Scope scope, List<ExpressionSyntax> parameters)
    {
        Points center = (Points)parameters[0].Evaluate(scope);
        var measure = (Measure) parameters[1].Evaluate(scope);

        return new Circle(center, measure);
    }

    public static object LineFunction(Scope scope, List<ExpressionSyntax> parameters)
    {
        Points point1 = (Points)parameters[0].Evaluate(scope);
        Points point2 = (Points)parameters[1].Evaluate(scope);

        return new Line(point1, point2);
    }

    public static object SegmentFunction(Scope scope, List <ExpressionSyntax> parameters)
    {
        Points point1 = (Points)parameters[0].Evaluate(scope);
        Points point2 = (Points)parameters[1].Evaluate(scope);

        return new Segment(point1, point2);
    }

    public static object RayFunction(Scope scope, List<ExpressionSyntax> parameters)
    {
        Points point1 = (Points)parameters[0].Evaluate(scope);
        Points point2 = (Points)parameters[1].Evaluate(scope);

        return new Ray(point1, point2);
    }

    public static object ArcFunction(Scope scope, List<ExpressionSyntax> parameters)
    {
        var point1 = (Points)parameters[0].Evaluate(scope);
        var point2 = (Points)parameters[1].Evaluate(scope);
        var point3 = (Points)parameters[2].Evaluate(scope);
        var measure = (Measure) parameters[3].Evaluate(scope);

        return new Arc(point1, point2, point3, measure);
    }

    public static object MeasureFunction(Scope scope, List<ExpressionSyntax> parameters)
    {
        var point1 = (Points)parameters[0].Evaluate(scope);
        var point2 = (Points)parameters[1].Evaluate(scope);

        return new Measure(point1, point2);
    }

    public static object CountFunction(Scope scope, List<ExpressionSyntax> parameters)
    {
        var sequence = parameters[0].Evaluate(scope);
        // ver si es infinita
        return ((SequenceExpressionSyntax)sequence).Count;
    }

    public static object RandomsFunction(Scope scope, List<ExpressionSyntax> list)
    {
        static object Randoms()
        {
            Random random = new();
            return random.NextDouble();
        }

        return new InfiniteSequence(Randoms, RandomElements);
    }

    internal static object SamplesFunction(Scope scope, List<ExpressionSyntax> list)
    {
        static object Samples()
        {
            return ParsingSupplies.CreateRandomPoint();
        }

        return new InfiniteSequence(Samples, RandomPoints);
    }

    internal static object PointsFunction(Scope scope, List<ExpressionSyntax> list)
    {
        var figure = (Figure)scope.Evaluate(list[0]);
        return figure.PointsInFigure();
    }
}
