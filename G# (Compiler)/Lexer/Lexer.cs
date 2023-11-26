using System.Runtime.CompilerServices;

namespace G_Sharp;

internal sealed class Lexer
{
    private int line = 1;
    private readonly string? Text;
    private int position;
    private char Current => Peek(0);
    private char NextCurrent => Peek(1);
    private char Peek(int offset)
    {
        var index = position + offset;
        if (index >= Text!.Length)
            return '\0';

        return Text[index];
    }
    private void Next()
    {
        position++;
    }
    public Lexer(string text)
    {
        Text = text;
    }

    public SyntaxToken Lex()
    {
        if (position >= Text!.Length)
            return new SyntaxToken(SyntaxKind.EndOfFileToken, position, "\0", null!);

        if (Current == '\r')
        {
            line++;
            return new SyntaxToken(SyntaxKind.WhitespaceToken, position++, "\0", null!);
        }

        if (Current == '\n')
        {
            return new SyntaxToken(SyntaxKind.WhitespaceToken, position++, "\0", null!);
        }

        if (Current == '"')
        {
            int start = position;
            int backSlashCount = 0;
            Next();

            while (Current != '"' || (Current == '"' && int.IsOddInteger(backSlashCount)))
            {
                if (Peek(1) == '\0')
                {
                    Error.SetError("SYNTAX", $"Line '{line}': Undetermined string literal");
                    return new SyntaxToken(SyntaxKind.ErrorToken!, position++, Text![position - 1].ToString(), null!);
                }

                if (Current == '\\')
                    backSlashCount++;

                else backSlashCount = 0;

                Next();
            }

            Next();
            int length = position - start;
            string token = Text!.Substring(start, length);
            token = SyntaxFact.BackSlashEvaluate(token);
            string value = token[1..^1];
            return new SyntaxToken(SyntaxKind.StringToken, start, token, value);
        }

        if (Current == '/' && NextCurrent == '/')
        {
            int start = position;
            while (Current != '\r')
            {
                Next();
                if (Current == '\0') break;
            }

            return new SyntaxToken(SyntaxKind.CommentToken, start, "//", null!);
        }

        if (char.IsDigit(Current))
        {
            int start = position;

            while (char.IsDigit(Current) || Current == '.')
                Next();

            int length = position - start;
            string token = Text!.Substring(start, length);
            double.TryParse(token, out double value);
            return new SyntaxToken(SyntaxKind.NumberToken, start, token, value);
        }

        if (char.IsLetterOrDigit(Current) || Current == '_')
        {
            int start = position;
            int spaces = 0;

            while (char.IsLetterOrDigit(Current) || Current == '_')
                Next();

            while (Current == ' ')
            {
                Next();
                spaces++;
            }

            int length = position - start - spaces;
            string token = Text!.Substring(start, length);
            var kind = SyntaxFact.GetKeywordKind(token);

            if (kind == SyntaxKind.NumberToken)
            {
                double value = (token == "PI") ? Math.PI : Math.E;
                return new SyntaxToken(SyntaxKind.NumberToken, start, token, value);
            }

            return new SyntaxToken(kind, start, token, null!);
        }

        if (char.IsWhiteSpace(Current))
        {
            int start = position;

            while (char.IsWhiteSpace(Current))
                Next();

            int length = position - start;
            string token = Text!.Substring(start, length);
            return new SyntaxToken(SyntaxKind.WhitespaceToken, start, token, null!);
        }

        switch (Current)
        {
            case '+':
                return new SyntaxToken(SyntaxKind.PlusToken, position++, "+", null!);
            case '-':
                return new SyntaxToken(SyntaxKind.MinusToken, position++, "-", null!);
            case '*':
                return new SyntaxToken(SyntaxKind.MultToken, position++, "*", null!);
            case '/':
                return new SyntaxToken(SyntaxKind.DivisionToken, position++, "/", null!);
            case '%':
                return new SyntaxToken(SyntaxKind.ModToken, position++, "%", null!);
            case '(':
                return new SyntaxToken(SyntaxKind.OpenParenthesisToken, position++, "(", null!);
            case ')':
                return new SyntaxToken(SyntaxKind.ClosedParenthesisToken, position++, ")", null!);
            case '{':
                return new SyntaxToken(SyntaxKind.OpenCurlyBracketToken, position++, "{", null!);
            case '}':
                return new SyntaxToken(SyntaxKind.ClosedCurlyBracketToken, position++, "}", null!);
            case ';':
                return new SyntaxToken(SyntaxKind.SemicolonToken, position++, ";", null!);
            case ',':
                return new SyntaxToken(SyntaxKind.SeparatorToken, position++, ",", null!);
            case '>':
                if (NextCurrent == '=')
                    return new SyntaxToken(SyntaxKind.GreaterOrEqualToken, position += 2, ">=", null!);
                return new SyntaxToken(SyntaxKind.GreaterToken, position++, ">", null!);
            case '<':
                if (NextCurrent == '=')
                    return new SyntaxToken(SyntaxKind.LessOrEqualToken, position += 2, "<=", null!);
                return new SyntaxToken(SyntaxKind.LessToken, position++, "<", null!);
            case '=':
                if (NextCurrent == '=')
                    return new SyntaxToken(SyntaxKind.EqualToken, position += 2, "==", null!);
                return new SyntaxToken(SyntaxKind.AssignmentToken, position++, "=", null!);
            case '!':
                if (NextCurrent == '=')
                    return new SyntaxToken(SyntaxKind.DifferentToken, position += 2, "!=", null!);
                break;
        }

        Error.SetError("LEXICAL", $"Line '{line}': Unexpected character '{Current}'");
        return new SyntaxToken(SyntaxKind.ErrorToken!, position++, Text![position - 1].ToString(), null!);
    }
}