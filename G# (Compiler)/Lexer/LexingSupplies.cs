using System.Runtime.InteropServices;

namespace G_Sharp;

public static class LexingSupplies
{
    private static readonly Dictionary<string, SyntaxKind> keywordKind = new()
    {
        ["and"]       = SyntaxKind.AndKeyword,
        ["or"]        = SyntaxKind.OrKeyword,
        ["not"]       = SyntaxKind.NotKeyword,
        ["let"]       = SyntaxKind.LetKeyword,
        ["in"]        = SyntaxKind.InKeyword,
        ["if"]        = SyntaxKind.IfKeyword,
        ["else"]      = SyntaxKind.ElseKeyword,
        ["then"]      = SyntaxKind.ThenKeyword,
        ["point"]     = SyntaxKind.GeometryKeyword,
        ["segment"]   = SyntaxKind.GeometryKeyword,
        ["ray"]       = SyntaxKind.GeometryKeyword,
        ["line"]      = SyntaxKind.GeometryKeyword,
        ["circle"]    = SyntaxKind.GeometryKeyword,
        ["arc"]       = SyntaxKind.GeometryKeyword,
        ["measure"]   = SyntaxKind.GeometryKeyword,
        ["color"]     = SyntaxKind.ColorKeyword,
        ["draw"]      = SyntaxKind.DrawKeyword,
        ["restore"]   = SyntaxKind.RestoreKeyword,
        ["PI"]        = SyntaxKind.NumberToken,
        ["E"]         = SyntaxKind.NumberToken,
    };


    public static readonly Dictionary<char, Func<int, char, (SyntaxToken, int)>> LexMathCharacters = new()
    {
        ['+'] = (position, _) => (new SyntaxToken(SyntaxKind.PlusToken, position, "+", null!), ++position),
        ['-'] = (position, _) => (new SyntaxToken(SyntaxKind.MinusToken, position, "-", null!), ++position),
        ['*'] = (position, _) => (new SyntaxToken(SyntaxKind.MultToken, position, "*", null!), ++position),
        ['/'] = (position, _) => (new SyntaxToken(SyntaxKind.DivisionToken, position, "/", null!), ++position),
        ['%'] = (position, _) => (new SyntaxToken(SyntaxKind.ModToken, position, "%", null!), ++position),
        ['('] = (position, _) => (new SyntaxToken(SyntaxKind.OpenParenthesisToken, position, "(", null!), ++position),
        [')'] = (position, _) => (new SyntaxToken(SyntaxKind.ClosedParenthesisToken, position, ")", null!), ++position),
        ['{'] = (position, _) => (new SyntaxToken(SyntaxKind.OpenCurlyBracketToken, position, "{", null!), ++position),
        ['}'] = (position, _) => (new SyntaxToken(SyntaxKind.ClosedCurlyBracketToken, position, "}", null!), ++position),
        [';'] = (position, _) => (new SyntaxToken(SyntaxKind.SemicolonToken, position, ";", null!), ++position),
        [','] = (position, _) => (new SyntaxToken(SyntaxKind.SeparatorToken, position, ",", null!), ++position),
        ['>'] = LexGreaterThanChar,
        ['<'] = LexLessThanChar,
        ['='] = LexEqualsChar,
        ['!'] = LexDifferentChar
    };

    public static SyntaxKind GetKeywordKind(string token)
    {
        if (keywordKind.TryGetValue(token, out SyntaxKind value))
            return value;

        return SyntaxKind.IdentifierToken;
    }

    private static (SyntaxToken, int) LexGreaterThanChar(int pos, char NextCurrent)
    {
        if (NextCurrent == '=')
                return (new SyntaxToken(SyntaxKind.GreaterOrEqualToken, pos, ">=", null!), pos + 2);
        return (new SyntaxToken(SyntaxKind.GreaterToken, pos, ">", null!), ++pos);
    }

    private static (SyntaxToken, int) LexLessThanChar(int pos, char NextCurrent)
    {
        if (NextCurrent == '=')
                return (new SyntaxToken(SyntaxKind.LessOrEqualToken, pos, "<=", null!), pos + 2);
        return (new SyntaxToken(SyntaxKind.LessToken, pos, "<", null!), ++pos);
    }

    private static (SyntaxToken, int) LexEqualsChar(int pos, char NextCurrent)
    {
        if (NextCurrent == '=')
                return (new SyntaxToken(SyntaxKind.EqualToken, pos, "==", null!), pos + 2);
        return (new SyntaxToken(SyntaxKind.AssignmentToken, pos, "=", null!), ++pos);
    }
    private static (SyntaxToken, int) LexDifferentChar(int pos, char NextCurrent)
    {
        if (NextCurrent == '=')
            return (new SyntaxToken(SyntaxKind.DifferentToken, pos, "!=", null!), pos + 2);
        return (new SyntaxToken(SyntaxKind.ErrorToken!, pos, "", null!), ++pos);
    }
}