
namespace G_Sharp;

public static class ParsingSupplies
{

    private static readonly Dictionary<SyntaxKind, int> binaryOperatorPrecedence = new()
    {
        [SyntaxKind.AndKeyword]           = 6,
        [SyntaxKind.OrKeyword]            = 6,
        [SyntaxKind.EqualToken]           = 5,
        [SyntaxKind.DifferentToken]       = 5,
        [SyntaxKind.GreaterToken]         = 4,
        [SyntaxKind.LessToken]            = 4,
        [SyntaxKind.GreaterOrEqualToken]  = 4,
        [SyntaxKind.LessOrEqualToken]     = 4,
        [SyntaxKind.MultToken]            = 2,
        [SyntaxKind.DivisionToken]        = 2,
        [SyntaxKind.ModToken]             = 2,
        [SyntaxKind.PlusToken]            = 1,
        [SyntaxKind.MinusToken]           = 1
    };

    private static readonly Dictionary<SyntaxKind, int> unaryOperatorPrecedence = new()
    {
        [SyntaxKind.NotKeyword] = 7,
        [SyntaxKind.PlusToken]  = 3,
        [SyntaxKind.MinusToken] = 3
    };

    
    public static Dictionary<string, Func<SyntaxToken, SyntaxToken, ExpressionSyntax>> GeometricKeywordsEvaluation = new()
    {
        ["point"]   = PointParsing,
        ["line"]    = LineParsing,
        ["segment"] = SegmentParsing,
        ["ray"]     = RayParsing,
        ["measure"] = MeasureParsing,
        ["circle"]  = CircleParsing,
        ["arc"]     = ArcParsing
    };

    public static float CreateRandomsCoordinates()
    {
        Random random = new();
        return (float)(random.Next(200, 700) + random.NextDouble());
    }

    public static Points[] CreateRandomPoints(int quantity)
    {
        Points[] points = new Points[quantity];

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new Points(CreateRandomsCoordinates(), CreateRandomsCoordinates());
        }

        return points;
    }

    private static ExpressionSyntax PointParsing(SyntaxToken name, SyntaxToken operatorToken)
    {
        var point = CreateRandomPoints(1);
        return new ConstantAssignmentSyntax(name, operatorToken, point[0]);
    }

    private static ExpressionSyntax SegmentParsing(SyntaxToken name, SyntaxToken operatorToken)
    {   
        var points = CreateRandomPoints(2);
        var segment = new Segment(points[0], points[1]);
        return new ConstantAssignmentSyntax(name, operatorToken, segment);
    }

    private static ExpressionSyntax LineParsing(SyntaxToken name, SyntaxToken operatorToken)
    {   
        var points = CreateRandomPoints(2);
        var segment = new Line(points[0], points[1]);
        return new ConstantAssignmentSyntax(name, operatorToken, segment);
    }

    private static ExpressionSyntax RayParsing(SyntaxToken name, SyntaxToken operatorToken)
    {   
        var points = CreateRandomPoints(2);
        var segment = new Ray(points[0], points[1]);
        return new ConstantAssignmentSyntax(name, operatorToken, segment);
    }

    private static ExpressionSyntax MeasureParsing(SyntaxToken name, SyntaxToken operatorToken)
    {
        var points = CreateRandomPoints(2);
        var measure = new Measure(points[0], points[1]);
        return new ConstantAssignmentSyntax(name, operatorToken, measure);
    }

    private static ExpressionSyntax CircleParsing(SyntaxToken name, SyntaxToken operatorToken)
    {   
        var points = CreateRandomPoints(2);
        var measure = new Measure(points[0], points[1]);
        var circle = new Circle(points[0], measure);

        return new ConstantAssignmentSyntax(name, operatorToken, circle);
    }

    private static ExpressionSyntax ArcParsing(SyntaxToken name, SyntaxToken operatorToken)
    {   
        var points = CreateRandomPoints(4);
        var measure = new Measure(points[0], points[3]);
        var arc = new Arc(points[0], points[1], points[2], measure);

        return new ConstantAssignmentSyntax(name, operatorToken, arc);
    }

    public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
    {
        if (binaryOperatorPrecedence.TryGetValue(kind, out int value))
            return value;

        return 0;
    }

    public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
    {
        if (unaryOperatorPrecedence.TryGetValue(kind, out int value))
            return value;

        return 0;
    }

    // Metodo que evalua los slash en el string
    public static string BackSlashEval(string text)
    {
        // Secuencias de escape permitidas
        char[] scapes = { 'n', 'r', 't', 'a', 'f', 'b', 'v', '"', '\'', '\\' };
        char[] scapeSequency = { '\n', '\r', '\t', '\a', '\f', '\b', '\v' };

        int backSlashIndex = text.IndexOf("\\");

        // Se itera mientras tenga slash
        while (backSlashIndex != -1)
        {
            int count = 0;

            // Se cuentan cuantos seguidos hay
            for (int i = backSlashIndex; i < text.Length; i++)
            {
                if (text[i] != '\\') break;
                count++;
            }

            // Se remueve la mitad de los slash
            text = text.Remove(backSlashIndex, count / 2);

            // Si no es par, se inserta el caracter de escape deseado
            if (count % 2 != 0)
            {
                int scapeIndex = Array.IndexOf(scapes, text[backSlashIndex + count - count / 2]);
                text = text.Remove(backSlashIndex, 1);

                if (!(scapes[scapeIndex] == '"' || scapes[scapeIndex] == '\'' ||
                    scapes[scapeIndex] == '\\'))
                {
                    text = text.Remove(backSlashIndex + count - count / 2 - 1, 1);
                    text = text.Insert(backSlashIndex + count - count / 2 - 1, scapeSequency[scapeIndex].ToString());
                }
            }

            backSlashIndex = text.IndexOf("\\", backSlashIndex + count / 2);
        }

        return text;
    }

}