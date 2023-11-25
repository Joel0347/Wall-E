
namespace G_Sharp;

public static class SyntaxFact
{
    public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
    {
        return kind switch
        {
            SyntaxKind.AndKeyword or SyntaxKind.OrKeyword => 6,
            SyntaxKind.EqualToken or SyntaxKind.DifferentToken => 5,
            SyntaxKind.GreatherToken or SyntaxKind.LessToken or 
            SyntaxKind.GreatherOrEqualToken or SyntaxKind.LessOrEqualToken => 4,
            SyntaxKind.MultToken or SyntaxKind.DivisionToken or SyntaxKind.ModToken => 2,
            SyntaxKind.PlusToken or SyntaxKind.MinusToken => 1,
            _ => 0,
        };
    }

    public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
    {
        return kind switch
        {
            SyntaxKind.NotKeyword => 7,
            SyntaxKind.PlusToken or SyntaxKind.MinusToken => 3,
            _ => 0,
        };
    }

    public static SyntaxKind GetKeywordKind(string token)
    {
        return token switch
        {
            "and" => SyntaxKind.AndKeyword,
            "or" => SyntaxKind.OrKeyword,
            "not" => SyntaxKind.NotKeyword,
            "let" => SyntaxKind.LetKeyword,
            "in" => SyntaxKind.InKeyword,
            "if" => SyntaxKind.IfKeyword,
            "else" => SyntaxKind.ElseKeyword,
            "then" => SyntaxKind.ThenKeyword,
            "point" => SyntaxKind.GeometryKeyword,
            "segment" => SyntaxKind.GeometryKeyword,
            "ray" => SyntaxKind.GeometryKeyword,
            "line" => SyntaxKind.GeometryKeyword,
            "circle" => SyntaxKind.GeometryKeyword,
            "arc" => SyntaxKind.GeometryKeyword,
            "color" => SyntaxKind.ColorKeyword,
            "draw" => SyntaxKind.DrawKeyword,
            "restore" => SyntaxKind.RestoreKeyword,
            "PI" => SyntaxKind.NumberToken,
            "E" => SyntaxKind.NumberToken,
            _ => SyntaxKind.IdentifierToken,
        };
    }
}