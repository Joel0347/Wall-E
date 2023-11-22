namespace G_Sharp;

public enum SyntaxKind
{
    // tokens
    WhitespacesToken,
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
    GreatherToken,
    LessToken,
    EqualToken,
    GreatherOrEqualToken,
    LessOrEqualToken,
    AssignmentToken,
    DifferentToken, 
    StringToken,
    SeparatorToken,

    // Expressions
    LiteralExpression,
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
}

public static class Data
{
    public static bool IsBalanced(string statment)
    {
        Stack<char> parenthesis = new();

        for (int i = 0; i < statment.Length; i++)
        {
            if (statment[i] == '(')
                parenthesis.Push('(');

            else if (statment[i] == '{')
                parenthesis.Push('{');    

            else if (statment[i] == ')')
            {
                if (parenthesis.Count > 0 && parenthesis.Peek() == '(')
                    parenthesis.Pop();
                else return false;
            } 

            else if (statment[i] == '}')
            {
                if (parenthesis.Count > 0 && parenthesis.Peek() == '{')
                    parenthesis.Pop();
                else return false;
            }   
        }

        return parenthesis.Count == 0;
    }
}