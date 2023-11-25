
using System.Drawing;
using System.Windows.Forms;

namespace G_Sharp;

internal sealed class Parser
{
    private readonly List<SyntaxToken> Tokens;
    private int Position;
    private SyntaxToken Current => Peek(0);
    public Parser(string text)
    {
        List<SyntaxToken> tokens = new();

        var lexer = new Lexer(text);

        SyntaxToken token;

        do
        {
            token = lexer.Lex();
            
            if (token.Kind != SyntaxKind.WhitespacesToken &&
                token.Kind != SyntaxKind.ErrorToken)
            tokens.Add(token);
        } 

        while (token.Kind != SyntaxKind.EndOfFileToken);

        Tokens = tokens;
    }

    private SyntaxToken Peek(int offset)
    {
        int index = Position + offset;

        return (index >= Tokens.Count) ? Tokens[^1] : Tokens[index];
    }

    private SyntaxToken NextToken()
    {
        var current = Current;
        Position++;
        return current;
    }

    public SyntaxTree Parse()
    {
        var expression = ParseExpression();
        var endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);

        return new SyntaxTree(Error.Wrong, expression, endOfFileToken);
    }

    public ExpressionSyntax ParseExpression(int parentPrecedence = 0)
    {
        return ParseAssignmentExpression();
    }

    public ExpressionSyntax ParseAssignmentExpression()
    {
        if (Peek(0).Kind == SyntaxKind.DrawKeyword)
        {
            var identifier = NextToken();
            List<ExpressionSyntax> geometry = new();

            if (Peek(0).Kind != SyntaxKind.OpenCurlyBracketToken)
            {
                var value = ParseAssignmentExpression();
                if (value.Kind == SyntaxKind.NameExpression || value.Kind == SyntaxKind.FunctionExpression)
                {
                    geometry.Add(value);
                }
            }

            else
            {
                List<ExpressionSyntax> expressions = GetFunctionParams();

                for (int i = 0; i < expressions.Count; i++)
                {
                    if (expressions[i].Kind != SyntaxKind.NameExpression )
                    {
                        geometry.Add(expressions[i]);
                    }
                }
            }

            return new Draw(geometry, Colors.ColorDraw.Last());
        }

        if (Peek(0).Kind == SyntaxKind.GeometryKeyword)
        {
            var typeGeometry = NextToken();

            if (Peek(0).Kind == SyntaxKind.OpenParenthesisToken)
            {
                List<ExpressionSyntax> parameters = GetFunctionParams();

                NextToken();

                return new FunctionExpressionSyntax(typeGeometry, parameters);
            }

            else
            {
                var name = NextToken();
                var operatorToken = new SyntaxToken(SyntaxKind.AssignmentToken, 0, "=", "");

                if (typeGeometry.Text == "point")
                {
                    var point = new Points(CreateRandomsCoordinates(), CreateRandomsCoordinates());

                    return new AssignmentExpressionSyntax(name, operatorToken, point);
                }       

                if (typeGeometry.Text == "segment")
                {
                    var point1 = new Points(CreateRandomsCoordinates(), CreateRandomsCoordinates());
                    var point2 = new Points(CreateRandomsCoordinates(), CreateRandomsCoordinates());
                    var segment = new Segment(point1, point2);

                    return new AssignmentExpressionSyntax(name, operatorToken, segment);
                }
                    

                if (typeGeometry.Text == "line")
                {
                    var point1 = new Points(CreateRandomsCoordinates(), CreateRandomsCoordinates());
                    var point2 = new Points(CreateRandomsCoordinates(), CreateRandomsCoordinates());
                    var line = new Line(point1, point2);

                    return new AssignmentExpressionSyntax(name, operatorToken, line);

                }

                if (typeGeometry.Text == "ray")
                {
                    var point1 = new Points(CreateRandomsCoordinates(), CreateRandomsCoordinates());
                    var point2 = new Points(CreateRandomsCoordinates(), CreateRandomsCoordinates());
                    var ray = new Ray(point1, point2);

                    return new AssignmentExpressionSyntax(name, operatorToken, ray);
                }

                if (typeGeometry.Text == "circle")
                {
                    var center = new Points(CreateRandomsCoordinates(), CreateRandomsCoordinates());
                    var measure = CreateRandomsCoordinates() / 2;
                    var circle = new Circle(center, measure);

                    return new AssignmentExpressionSyntax(name, operatorToken, circle);
                }

                if (typeGeometry.Text == "arc")
                {
                    var point1 = new Points(CreateRandomsCoordinates(), CreateRandomsCoordinates());
                    var point2 = new Points(CreateRandomsCoordinates(), CreateRandomsCoordinates());
                    var point3 = new Points(CreateRandomsCoordinates(), CreateRandomsCoordinates());
                    var measure = CreateRandomsCoordinates() / 2;
                    var arc = new Arc(point1, point2, point3, measure);

                    return new AssignmentExpressionSyntax(name, operatorToken, arc);
                }
            }
        }

        if (Peek(0).Kind == SyntaxKind.IdentifierToken && 
            Tokens.Any(x => Tokens.IndexOf(x) > Position && x.Kind == SyntaxKind.AssignmentToken))
        {
            if (Peek(1).Kind == SyntaxKind.OpenParenthesisToken)
            {
                var identifierToken = NextToken();

                List<ExpressionSyntax> parameters = GetFunctionParams();

                NextToken();
                
                if (Peek(0).Kind == SyntaxKind.AssignmentToken)
                {
                    var operatorToken = NextToken();
                    var body = ParseAssignmentExpression();

                    return new AssignmentFunctionExpressionSyntax(identifierToken, parameters, operatorToken, body);
                }
            }

            else if (Peek(1).Kind == SyntaxKind.AssignmentToken)
            {
                var identifierToken = NextToken();
                var operatorToken = NextToken();
                var right = ParseAssignmentExpression();

                return new AssignmentExpressionSyntax(identifierToken, operatorToken, right);
            }
        }

        return ParseBinaryExpression();
    }

    private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
    {
        ExpressionSyntax left;
        var unaryOperatorPrecedent = Current.Kind.GetUnaryOperatorPrecedence();
        
        if (unaryOperatorPrecedent != 0 && unaryOperatorPrecedent >= parentPrecedence) 
        {
            var operatorToken = NextToken();
            var operand = ParseBinaryExpression(unaryOperatorPrecedent);
            left = new UnaryExpressionSyntax(operatorToken, operand);
        }
        else 
        {
            left = ParsePrimaryExpression();
        }

        while (true)
        {
            var precedence = Current.Kind.GetBinaryOperatorPrecedence();

            if (precedence == 0 || precedence <= parentPrecedence) break;

            var operatorToken = NextToken();
            var right = ParseBinaryExpression(precedence);
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }

        return left;
    }

    private ExpressionSyntax ParsePrimaryExpression()
    {
        switch (Current.Kind)
        {
            case SyntaxKind.RestoreKeyword:
            {
                NextToken();
                Colors.ColorDraw.RemoveAt(Colors.ColorDraw.Count - 1);
                var obj = new SyntaxToken(SyntaxKind.StringToken, 0, "", "");
                return new LiteralExpressionSyntax(obj);
                }
            case SyntaxKind.ColorKeyword:
            {
                NextToken();
                Colors.ColorDraw.Add(Colors._Colors[NextToken().Value.ToString()!]);
                var obj = new SyntaxToken(SyntaxKind.StringToken, 0, "", "");
                return new LiteralExpressionSyntax(obj);
            }
            case SyntaxKind.OpenParenthesisToken:
            {
                var left = NextToken();
                var expression = ParseBinaryExpression();
                var right = MatchToken(SyntaxKind.ClosedParenthesisToken);

                return new ParenthesizedExpressionSyntax(left, expression, right);
            }

            case SyntaxKind.IdentifierToken:
            {
                var identifierToken = NextToken();

                if (Current.Kind == SyntaxKind.OpenParenthesisToken)
                {
                    List<ExpressionSyntax> parameters = GetFunctionParams();

                    NextToken();

                    return new FunctionExpressionSyntax(identifierToken, parameters);
                }

                return new NameExpressionSyntax(identifierToken);
            }

            case SyntaxKind.StringToken:
            {
                var stringToken = MatchToken(SyntaxKind.StringToken);
                return new LiteralExpressionSyntax(stringToken);
            }

            default:
            {
                var numberToken = MatchToken(SyntaxKind.NumberToken);
                return new LiteralExpressionSyntax(numberToken);
            }
        }
    }

    private SyntaxToken MatchToken(SyntaxKind kind)
    {
        if (Current.Kind == kind)
            return NextToken();

        Error.SetError("SEMANTIC", $"Unexpected token '{Current.Text}', expected '{kind}'");
        return new SyntaxToken(kind, Current.Position, "", 0.0);
    }
    private List<ExpressionSyntax> GetFunctionParams()
    {
        var kind = Current.Kind;
        var endKind = (kind == SyntaxKind.OpenParenthesisToken) ? SyntaxKind.ClosedParenthesisToken : SyntaxKind.ClosedCurlyBracketToken;

        NextToken();
        List<ExpressionSyntax> parameters = new();

        var parameter = ParseBinaryExpression();
        parameters.Add(parameter);

        while (Current.Kind == SyntaxKind.SeparatorToken)
        {
            NextToken();
            parameter = ParseBinaryExpression();
            parameters.Add(parameter);
        }

        if (Current.Kind != endKind)
        {
            var text = (kind == SyntaxKind.OpenParenthesisToken) ? "parenthesis" : "curly brackets";
            Error.SetError("SYNTAX", $"Missing closing parenthesis after ...");
        }

        return parameters;
    }
    private float CreateRandomsCoordinates()
    {
        Random random = new();

        return (float)(random.Next(200, 700) + random.NextDouble());
    }
}