
using System.Drawing;
using System.Windows.Forms;

namespace G_Sharp;

internal sealed class Parser
{
    public bool ContainsError { get; }
    private List<ExpressionSyntax> functionParams = new();
    private readonly List<SyntaxToken> tokens;
    private int position;
    private SyntaxToken Current => Peek(0);
    public Parser(string text)
    {
        List<SyntaxToken> tokens = new();

        var lexer = new Lexer(text);

        SyntaxToken token;

        do
        {
            token = lexer.Lex();

            if (token.Kind == SyntaxKind.ErrorToken)
            {
                ContainsError = true;
                break;
            }

            else if (token.Kind != SyntaxKind.WhitespaceToken &&
                token.Kind != SyntaxKind.CommentToken)

                tokens.Add(token);
        }

        while (token.Kind != SyntaxKind.EndOfFileToken);

        this.tokens = tokens;
    }

    private SyntaxToken Peek(int offset)
    {
        int index = position + offset;

        return (index >= tokens.Count) ? tokens[^1] : tokens[index];
    }

    private SyntaxToken NextToken()
    {
        var current = Current;
        position++;
        return current;
    }

    public SyntaxTree Parse()
    {
        List<ExpressionSyntax> expressions = new();
        var semicolonToken = new SyntaxToken(SyntaxKind.SemicolonToken, 0, ";", "");

        while (true)
        {

            var expression = ParseExpression();
            expressions.Add(expression);

            semicolonToken = MatchToken(SyntaxKind.SemicolonToken);

            if (semicolonToken.Kind == SyntaxKind.ErrorToken)
            {
                expressions.Add(new ErrorExpressionSyntax());
                break;
            }

            if (Current.Kind == SyntaxKind.EndOfFileToken) break;
        }

        return new SyntaxTree(Error.Wrong, expressions, semicolonToken);
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
                List<ExpressionSyntax> expressions = GetFunctionParams("sequency");

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
                List<ExpressionSyntax> parameters = GetFunctionParams(typeGeometry.Text);

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

        if (Peek(0).Kind == SyntaxKind.IdentifierToken)
        {
            if (Peek(1).Kind == SyntaxKind.OpenParenthesisToken)
            {
                int actualPos = position;
                var identifierToken = NextToken();

                List<ExpressionSyntax> parameters = GetFunctionParams(identifierToken.Text);

                NextToken();

                if (Peek(0).Kind == SyntaxKind.AssignmentToken)
                {
                    var operatorToken = NextToken();
                    var body = ParseAssignmentExpression();

                    if (body.Kind == SyntaxKind.SemicolonToken)
                    {
                        Error.SetError("SEMANTIC", $"Missing expression in '{identifierToken.Text}' declaration");
                        return new ErrorExpressionSyntax();
                    }

                    
                    return new AssignmentFunctionExpressionSyntax(identifierToken, parameters, operatorToken, body);
                }

                functionParams = parameters;
                position = actualPos;
            }

            else if (Peek(1).Kind == SyntaxKind.AssignmentToken)
            {
                var identifierToken = NextToken();
                var operatorToken = NextToken();
                var right = ParseAssignmentExpression();

                if (right.Kind == SyntaxKind.SemicolonToken)
                {
                    Error.SetError("SEMANTIC", $"Missing expression in '{identifierToken.Text}' assignment");
                    return new ErrorExpressionSyntax();
                }

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

            case SyntaxKind.LetKeyword:
            {
                var letToken = NextToken();
                List<ExpressionSyntax> instructions = new();

                while (Current.Kind != SyntaxKind.InKeyword)
                {
                    var statement = ParseExpression();

                    var semicolonToken = MatchToken(SyntaxKind.SemicolonToken);

                    if (semicolonToken.Kind == SyntaxKind.ErrorToken)
                    {
                        return new ErrorExpressionSyntax();
                    }

                    instructions.Add(statement);
                }

                var inToken = MatchToken(SyntaxKind.InKeyword);
                if (inToken.Kind == SyntaxKind.ErrorToken)
                {
                    return new ErrorExpressionSyntax();
                }

                var body = ParseExpression();

                return new LetInExpressionSyntax(letToken, instructions, inToken, body);
            }

            case SyntaxKind.IdentifierToken:
            {
                var identifierToken = NextToken();

                if (Current.Kind == SyntaxKind.OpenParenthesisToken)
                {
                    if (functionParams.Count > 0)
                    {
                        NextToken();

                        if (functionParams.Any(x => x is ErrorExpressionSyntax))
                            return new ErrorExpressionSyntax();

                        if (Current.Kind != SyntaxKind.ClosedParenthesisToken)
                            position += 2 * functionParams.Count - 1;

                        ExpressionSyntax[] parameters = new ExpressionSyntax[functionParams.Count];
                        Array.Copy(functionParams.ToArray(), parameters, parameters.Length);
                        NextToken();
                        functionParams.Clear();
                        return new FunctionExpressionSyntax(identifierToken, parameters.ToList());
                    }

                    else
                    {
                        List<ExpressionSyntax> parameters = GetFunctionParams(identifierToken.Text);
                        NextToken();
                        return new FunctionExpressionSyntax(identifierToken, parameters);
                    }
                }

                return new NameExpressionSyntax(identifierToken);
            }

            case SyntaxKind.StringToken:
            {
                var stringToken = MatchToken(SyntaxKind.StringToken);

                if (stringToken.Kind == SyntaxKind.ErrorToken)
                    return new ErrorExpressionSyntax();

                return new LiteralExpressionSyntax(stringToken);
            }

            case SyntaxKind.NumberToken:
            {
                var numberToken = MatchToken(SyntaxKind.NumberToken);

                if (numberToken.Kind == SyntaxKind.ErrorToken)
                    return new ErrorExpressionSyntax();

                return new LiteralExpressionSyntax(numberToken);
            }

            default:
            {
                var semicolonToken = MatchToken(SyntaxKind.SemicolonToken);

                if (semicolonToken.Kind == SyntaxKind.ErrorToken)
                    return new ErrorExpressionSyntax();

                return new LiteralExpressionSyntax(semicolonToken);
            }
        }
    }

    private SyntaxToken MatchToken(SyntaxKind kind)
    {
        if (Current.Kind == kind)
            return NextToken();

        Error.SetError("SEMANTIC", $"Unexpected token '{Current.Kind}', expected '{kind}'");
        kind = SyntaxKind.ErrorToken;
        return new SyntaxToken(kind, Current.Position, "", 0.0);
    }
    private List<ExpressionSyntax> GetFunctionParams(string name)
    {
        var kind = Current.Kind;
        var endKind = (kind == SyntaxKind.OpenParenthesisToken) ? SyntaxKind.ClosedParenthesisToken : SyntaxKind.ClosedCurlyBracketToken;

        NextToken();
        SyntaxToken token = Current;
        List<ExpressionSyntax> parameters = new();

        if (Current.Kind == SyntaxKind.ClosedParenthesisToken)
            return parameters;

        var parameter = ParseBinaryExpression();
        parameters.Add(parameter);

        while (Current.Kind == SyntaxKind.SeparatorToken)
        {
            NextToken();
            token = Current;
            if (Current.Kind == SyntaxKind.ClosedParenthesisToken)
            {
                Error.SetError("SEMANTIC", $"Missing argument in '{name}'");
                parameters.Add(new ErrorExpressionSyntax());
                return parameters;
            }

            parameter = ParseBinaryExpression();
            parameters.Add(parameter);
        }

        if (Current.Kind != endKind)
        {
            var text = (kind == SyntaxKind.OpenParenthesisToken) ? "parenthesis" : "curly brackets";
            Error.SetError("SYNTAX", $"Missing closing {text} after '{token.Text}'");
        }

        return parameters;
    }
    private float CreateRandomsCoordinates()
    {
        Random random = new();

        return (float)(random.Next(200, 700) + random.NextDouble());
    }
}