namespace G_Sharp;

public enum SyntaxKind
{
    // tokens
    WhitespaceToken,
    NumberToken,
    PlusToken,
    MinusToken,
    MultToken,
    DivisionToken,
    ModToken,
    OpenParenthesisToken,
    ClosedParenthesisToken,
    ErrorToken,
    EndOfFileToken,
    SemicolonToken,
    IdentifierToken,
    GreaterToken,
    LessToken,
    EqualToken,
    GreaterOrEqualToken,
    LessOrEqualToken,
    AssignmentToken,
    DifferentToken, 
    StringToken,
    SeparatorToken,
    CircleToken,
    PointToken,
    ArcToken,
    LineToken,
    SegmentToken,
    RayToken,
    MeasureToken,
    ClosedCurlyBracketToken,
    OpenCurlyBracketToken,
    CommentToken,
    MathToken,
    ColorToken,
    SuspenseToken,

    // Expressions
    LiteralExpression,
    BinaryExpression,
    ParenthesizedExpression,
    UnaryExpression,
    ConstantExpression,
    ConstantAssignmentExpression,   
    FunctionExpression,
    AssignmentFunctionExpression,
    LetInExpression,
    ConditionalExpression,
    SequenceExpression,
    DrawExpression,
    VoidExpression,

    // Keywords
    AndKeyword,
    OrKeyword,
    NotKeyword,
    LetKeyword,
    InKeyword,
    IfKeyword,
    ElseKeyword,
    ThenKeyword,
    ColorKeyword,
    GeometryKeyword,
    DrawKeyword,
    RestoreKeyword,
    SequenceKeyword,
    ImportKeyword
}

public static class Data
{
    public static bool IsBalanced(List<SyntaxToken> tokens, int start, out int position)
    {
        position = start;

        Stack<char> parenthesis = new();

        for (int i = start; i < tokens.Count; i++)
        {
            if (tokens[i].Kind == SyntaxKind.OpenParenthesisToken)
                parenthesis.Push('(');

            else if (tokens[i].Kind == SyntaxKind.OpenCurlyBracketToken)
                parenthesis.Push('{');

            else if (tokens[i].Kind == SyntaxKind.ClosedParenthesisToken)
            {
                if (parenthesis.Count > 0 && parenthesis.Peek() == '(')
                    parenthesis.Pop();

                else
                {
                    position = -1;
                    return false;
                }
            }

            else if (tokens[i].Kind == SyntaxKind.ClosedCurlyBracketToken)
            {
                if (parenthesis.Count > 0 && parenthesis.Peek() == '{')
                    parenthesis.Pop();

                else
                {
                    position = -1;
                    return false;
                }
            }

            position++;
        }

        bool balanced = parenthesis.Count == 0;
        position = balanced ? position - start : -1;

        return balanced;
    }
}