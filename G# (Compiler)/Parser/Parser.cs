
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

    private List<ExpressionSyntax> GetFunctionParams()
    {
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

        if(Current.Kind != SyntaxKind.ClosedParenthesisToken) {
            
            Error.SetError($"!!SYNTAX ERROR: Missing closing parenthesis after ...");
        }

        return parameters;
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

        Error.SetError($"!!SEMANTIC ERROR: Unexpected token '{Current.Kind}', expected '{kind}'");
        return new SyntaxToken(kind, Current.Position, "", 0.0);
    }
}