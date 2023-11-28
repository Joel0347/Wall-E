
using System.Drawing;
using System.Windows.Forms;

namespace G_Sharp;

internal sealed class Parser
{
    private List<ExpressionSyntax> functionParams = new();
    private int positionAfterFuncParams;
    private readonly List<SyntaxToken> tokens;
    private int position;

    private SyntaxToken Current => Peek(0);
    public bool ContainsError { get; }
    private Dictionary<SyntaxKind, Func<ExpressionSyntax>> tokensEvaluation { get; }
    private Dictionary<SyntaxKind, Func<ExpressionSyntax>> assignmentEvaluation { get; }
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

        tokensEvaluation = new()
        {
            [SyntaxKind.RestoreKeyword]       = RestoreParsing,
            [SyntaxKind.ColorKeyword]         = ColorParsing,
            [SyntaxKind.OpenParenthesisToken] = ParenthesizedExpressionParsing,
            [SyntaxKind.LetKeyword]           = LetInExpressionParsing,
            [SyntaxKind.IfKeyword]            = ConditionalExpressionParsing,
            [SyntaxKind.IdentifierToken]      = IdentifierParsing,
            [SyntaxKind.StringToken]          = StringParsing,
            [SyntaxKind.NumberToken]          = NumberParsing
        };

        assignmentEvaluation = new()
        {
            [SyntaxKind.DrawKeyword]     = DrawParsing,
            [SyntaxKind.GeometryKeyword] = AssignmentGeometryEval,
            [SyntaxKind.IdentifierToken] = AssignmentIdentifierEval,
        };
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

            semicolonToken = MatchToken(SyntaxKind.SemicolonToken, ";");

            if (semicolonToken.Kind == SyntaxKind.ErrorToken)
            {
                expressions.Add(new ErrorExpressionSyntax());
                break;
            }

            if (Current.Kind == SyntaxKind.EndOfFileToken) break;
        }

        return new SyntaxTree(Error.Wrong, expressions, semicolonToken);
    }

    private SyntaxToken MatchToken(SyntaxKind kind, string expectedText)
    {
        if (Current.Kind == kind)
            return NextToken();

        Error.SetError("SYNTAX", $"Unexpected token '{Current.Text}', expected '{expectedText}'");
        kind = SyntaxKind.ErrorToken;
        return new SyntaxToken(kind, Current.Position, "", 0.0);
    }

    public ExpressionSyntax ParseExpression()
    {
        return ParseAssignmentExpression();
    }

    public ExpressionSyntax ParseAssignmentExpression()
    {
        if (assignmentEvaluation.TryGetValue(Current.Kind, out Func<ExpressionSyntax>? value))
            return value();

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

        else left = ParsePrimaryExpression();

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
        if (tokensEvaluation.TryGetValue(Current.Kind, out Func<ExpressionSyntax>? value))
            return value();
            
        var semicolonToken = MatchToken(SyntaxKind.SemicolonToken, ";");

        if (semicolonToken.Kind == SyntaxKind.ErrorToken)
            return new ErrorExpressionSyntax();

        return new LiteralExpressionSyntax(semicolonToken);
    }

    private List<ExpressionSyntax> GetFunctionParams(string name)
    {
        var kind = Current.Kind;
        var endKind = (kind == SyntaxKind.OpenParenthesisToken) ? SyntaxKind.ClosedParenthesisToken : 
                                                                  SyntaxKind.ClosedCurlyBracketToken;

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

            parameter = ParseExpression();
            parameters.Add(parameter);
        }

        if (Current.Kind != endKind)
        {
            var text = (kind == SyntaxKind.OpenParenthesisToken) ? "parenthesis" : "curly brackets";
            Error.SetError("SYNTAX", $"Missing closing {text} after '{token.Text}'");
        }

        return parameters;
    }
    private LiteralExpressionSyntax RestoreParsing()
    {
        NextToken();
        Colors.ColorDraw.RemoveAt(Colors.ColorDraw.Count - 1);
        var obj = new SyntaxToken(SyntaxKind.StringToken, 0, "", "");
        return new LiteralExpressionSyntax(obj);
    }

    private LiteralExpressionSyntax ColorParsing()
    {
        NextToken();
        Colors.ColorDraw.Add(Colors._Colors[NextToken().Value.ToString()!]);
        var obj = new SyntaxToken(SyntaxKind.StringToken, 0, "", "");
        return new LiteralExpressionSyntax(obj);
    }

    private ParenthesizedExpressionSyntax ParenthesizedExpressionParsing()
    {
        var left = NextToken();
        var expression = ParseBinaryExpression();
        var right = MatchToken(SyntaxKind.ClosedParenthesisToken, ")");

        return new ParenthesizedExpressionSyntax(left, expression, right);
    }

    private ExpressionSyntax LetInExpressionParsing()
    {
        var letToken = NextToken();
        List<ExpressionSyntax> instructions = new();

        while (Current.Kind != SyntaxKind.InKeyword)
        {
            var statement = ParseExpression();

            var semicolonToken = MatchToken(SyntaxKind.SemicolonToken, ";");

            if (semicolonToken.Kind == SyntaxKind.ErrorToken)
                return new ErrorExpressionSyntax();

            instructions.Add(statement);
        }

        var inToken = MatchToken(SyntaxKind.InKeyword, "in");

        if (inToken.Kind == SyntaxKind.ErrorToken)
            return new ErrorExpressionSyntax();
        
        var body = ParseExpression();
        //revisar vacio

        return new LetInExpressionSyntax(letToken, instructions, inToken, body);
    }

    private ExpressionSyntax ConditionalExpressionParsing()
    {
        var ifKeyword = NextToken();
        var condition = ParseBinaryExpression();

        var thenKeyword = MatchToken(SyntaxKind.ThenKeyword, "then");
        var bodyTrue = ParseExpression();

        var elseKeyword = MatchToken(SyntaxKind.ElseKeyword, "else");
        var bodyFalse = ParseExpression();

        return new ConditionalExpressionSyntax(ifKeyword, condition, thenKeyword, bodyTrue, elseKeyword, bodyFalse);
    }

    private ExpressionSyntax IdentifierParsing()
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
                    position = positionAfterFuncParams;

                ExpressionSyntax[] parameters = new ExpressionSyntax[functionParams.Count];
                Array.Copy(functionParams.ToArray(), parameters, parameters.Length);
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

        return new ConstantExpressionSyntax(identifierToken);
    }

    private ExpressionSyntax StringParsing()
    {
        var stringToken = MatchToken(SyntaxKind.StringToken, "string");

        if (stringToken.Kind == SyntaxKind.ErrorToken)
            return new ErrorExpressionSyntax();

        return new LiteralExpressionSyntax(stringToken);
    }

    private ExpressionSyntax NumberParsing()
    {
        var numberToken = MatchToken(SyntaxKind.NumberToken, "number");

        if (numberToken.Kind == SyntaxKind.ErrorToken)
            return new ErrorExpressionSyntax();

        return new LiteralExpressionSyntax(numberToken);
    }

    private ExpressionSyntax DrawParsing()
    {
        NextToken();
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

    private ExpressionSyntax AssignmentGeometryEval()
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
            return ParsingSupplies.GeometricKeywordsEvaluation[typeGeometry.Text](name, operatorToken);
        }
    }

    private ExpressionSyntax AssignmentIdentifierEval()
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

                foreach (var item in parameters)
                {
                    if(item.Kind != SyntaxKind.NameExpression) {
                        Error.SetError("SYNTAX", "Expected identifier token in function parameters");
                    }
                }

                return new AssignmentFunctionSyntax(identifierToken, parameters, operatorToken, body);
            }

            functionParams = parameters;
            positionAfterFuncParams = position;
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

            return new ConstantAssignmentSyntax(identifierToken, operatorToken, right);
        }

        return ParseBinaryExpression();
    }
}