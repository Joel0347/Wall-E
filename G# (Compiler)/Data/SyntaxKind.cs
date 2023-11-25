namespace G_Sharp;

public enum SyntaxKind
{
    // tokens

    PlusToken,
    MinusToken,
    MultToken,
    DivisionToken,
    ModToken,
    GreaterToken,
    LessToken,
    EqualToken,
    GreaterOrEqualToken,
    LessOrEqualToken,
    DifferentToken, 
    WhitespaceToken,
    NumberToken,
    OpenParenthesisToken,
    ClosedParenthesisToken,
    ErrorToken,
    EndOfFileToken,
    SemicolonToken,
    IdentifierToken,
    AssignmentToken,
    StringToken,
    SeparatorToken,
    CommentToken,

    // Expressions
    BinaryExpression,
    ParenthesizedExpression,
    UnaryExpression,
    NameExpression,
    AssignmentExpression,   
    FunctionExpression,
    AssignmentFunctionExpression,

    // Keywords
    AndKeyword,
    OrKeyword,
    NotKeyword,
    LetKeyword,
    InKeyword,
    IfKeyword,
    ElseKeyword,
    ThenKeyword,

    // Statement
    BlockStatement,
    OpenCurlyBracketToken,
    ClosedCurlyBracketToken,

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

                else {
                    position = -1;
                    return false;
                }
            } 

            else if (tokens[i].Kind == SyntaxKind.ClosedCurlyBracketToken)
            {
                if (parenthesis.Count > 0 && parenthesis.Peek() == '{')
                    parenthesis.Pop();

                else {
                    position = -1;
                    return false;
                }
            }   

            position++;
        }

        bool balanced = parenthesis.Count == 0;
        position = balanced? position - start : -1;

        return balanced;
    }
}