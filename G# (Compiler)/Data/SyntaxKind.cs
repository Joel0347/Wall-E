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

    // Expressions
    LiteralExpression,
    BinaryExpression,
    ParenthesizedExpression,
    UnaryExpression,
    NameExpression,
    AssignmentExpression,

    // Keywords
    AndKeyword,
    OrKeyword,
    NotKeyword,
    LetKeyword,
    InKeyword,
    IfKeyword,
    ElseKeyword,
    ThenKeyword,
}