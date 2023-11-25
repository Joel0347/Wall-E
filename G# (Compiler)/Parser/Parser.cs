
namespace G_Sharp;

internal sealed class Parser
{
    public bool ContainsError { get; }
    private List<ExpressionSyntax> functionParams = new();
    private readonly List<SyntaxToken> tokens;
    private SyntaxToken Current => Peek(0);
    private int position;
    public Parser(string text)
    {
        List<SyntaxToken> tokens = new();

        var lexer = new Lexer(text);

        SyntaxToken token;

        do
        {
            token = lexer.Lex();
            
            if (token.Kind == SyntaxKind.ErrorToken) {
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
 
        while (true) {
            
            var expression = ParseExpression();
            expressions.Add(expression);

            semicolonToken = MatchToken(SyntaxKind.SemicolonToken);

            if (semicolonToken.Kind == SyntaxKind.ErrorToken) {
                expressions.Add(new ErrorExpressionSyntax());
                break;
            }

            if (Current.Kind == SyntaxKind.EndOfFileToken) break;
        }

        return new SyntaxTree(Error.Wrong, expressions, semicolonToken);
    }

    public ExpressionSyntax ParseExpression()
    {
        return ParseAssignmentExpression();
    }

    public ExpressionSyntax ParseAssignmentExpression()
    {
        if (Peek(0).Kind == SyntaxKind.IdentifierToken)
        {
            if (Peek(1).Kind == SyntaxKind.OpenParenthesisToken)
            {
                int actualPos = position;
                var identifierToken = NextToken();

                functionParams = GetFunctionParams(identifierToken.Text);

                NextToken();
                
                if (Peek(0).Kind == SyntaxKind.AssignmentToken)
                {
                    var operatorToken = NextToken();
                    var body = ParseAssignmentExpression();

                    if (body.Kind == SyntaxKind.SemicolonToken) {
                        Error.SetError($"!!SEMANTIC ERROR: Missing expression in '{identifierToken.Text}' declaration");
                        return new ErrorExpressionSyntax();
                    }

                    return new AssignmentFunctionExpressionSyntax(identifierToken, functionParams, operatorToken, body);
                }

                position = actualPos;
            }

            else if (Peek(1).Kind == SyntaxKind.AssignmentToken)
            {
                var identifierToken = NextToken();
                var operatorToken = NextToken();
                var right = ParseAssignmentExpression();

                if (right.Kind == SyntaxKind.SemicolonToken) {
                    Error.SetError($"!!SEMANTIC ERROR: Missing expression in '{identifierToken.Text}' assignment");
                    return new ErrorExpressionSyntax();
                }

                return new AssignmentExpressionSyntax(identifierToken, operatorToken, right);
            }
        }

        return ParseBinaryExpression();
    }
    private List<ExpressionSyntax> GetFunctionParams(string name)
    {
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
            if (Current.Kind == SyntaxKind.ClosedParenthesisToken) {
                Error.SetError($"!!SEMANTIC ERROR: Missing argument in '{name}'");
                parameters.Add(new ErrorExpressionSyntax());
                return parameters;
            }

            parameter = ParseBinaryExpression();
            parameters.Add(parameter);
        }

        if(Current.Kind != SyntaxKind.ClosedParenthesisToken) {
            
            Error.SetError($"!!SYNTAX ERROR: Missing closing parenthesis after '{token.Text}'");
            parameters.Add(new ErrorExpressionSyntax());
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

                if (right.Kind == SyntaxKind.ErrorToken)
                    return new ErrorExpressionSyntax();

                return new ParenthesizedExpressionSyntax(left, expression, right);
            }

            case SyntaxKind.IdentifierToken:
            {
                var identifierToken = NextToken();

                if (Current.Kind == SyntaxKind.OpenParenthesisToken)
                {
                    NextToken();

                    if (functionParams.Any(x => x is ErrorExpressionSyntax))
                        return new ErrorExpressionSyntax();

                    if (Current.Kind != SyntaxKind.ClosedParenthesisToken) 
                        position += 2 * functionParams.Count - 1;
                        
                    NextToken();

                    return new FunctionExpressionSyntax(identifierToken, functionParams);
                }

                return new NameExpressionSyntax(identifierToken);
            }

            case SyntaxKind.StringToken:
            {
                var stringToken = MatchToken(SyntaxKind.StringToken);

                if (stringToken.Kind == SyntaxKind.ErrorToken)
                    return new ErrorExpressionSyntax();
                
                return new StringLiteralExpressionSyntax(stringToken);
            }

            case SyntaxKind.NumberToken:
            {
                var numberToken = MatchToken(SyntaxKind.NumberToken);

                if (numberToken.Kind == SyntaxKind.ErrorToken)
                    return new ErrorExpressionSyntax();

                return new NumberLiteralExpressionSyntax(numberToken);
            }

            default:
            {
                var semicolonToken = MatchToken(SyntaxKind.SemicolonToken);

                if (semicolonToken.Kind == SyntaxKind.ErrorToken)
                    return new ErrorExpressionSyntax();

                return new EndOfStatementExpressionSyntax(semicolonToken);
            }
        }
    }

    private SyntaxToken MatchToken(SyntaxKind kind)
    {
        if (Current.Kind == kind)
            return NextToken();

        Error.SetError($"!!SEMANTIC ERROR: Unexpected token '{Current.Kind}', expected '{kind}'");
        kind = SyntaxKind.ErrorToken;
        return new SyntaxToken(kind, Current.Position, "", 0.0);
    }
}