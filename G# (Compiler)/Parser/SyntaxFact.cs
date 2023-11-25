
using System.Collections;

namespace G_Sharp;

public static class SyntaxFact
{
    private static Dictionary<string, SyntaxKind> keywordKind = new()
    {
        ["and" ] = SyntaxKind.AndKeyword,
        ["or"  ] = SyntaxKind.OrKeyword,
        ["not" ] = SyntaxKind.NotKeyword,
        ["let" ] = SyntaxKind.LetKeyword,
        ["in"  ] = SyntaxKind.InKeyword,
        ["if"  ] = SyntaxKind.IfKeyword,
        ["else"] = SyntaxKind.ElseKeyword,
        ["then"] = SyntaxKind.ThenKeyword,
        ["PI"  ] = SyntaxKind.NumberToken,
        ["E"   ] = SyntaxKind.NumberToken,
    };

    private static Dictionary<SyntaxKind, int> binaryOperatorPrecedence = new()
    {
        [SyntaxKind.AndKeyword         ] = 6,
        [SyntaxKind.OrKeyword          ] = 6,
        [SyntaxKind.EqualToken         ] = 5,
        [SyntaxKind.DifferentToken     ] = 5,
        [SyntaxKind.GreaterToken       ] = 4,
        [SyntaxKind.LessToken          ] = 4,
        [SyntaxKind.GreaterOrEqualToken] = 4,
        [SyntaxKind.LessOrEqualToken   ] = 4,
        [SyntaxKind.MultToken          ] = 2,
        [SyntaxKind.DivisionToken      ] = 2,
        [SyntaxKind.ModToken           ] = 2,
        [SyntaxKind.PlusToken          ] = 1,
        [SyntaxKind.MinusToken         ] = 1
    };

    private static Dictionary<SyntaxKind, int> unaryOperatorPrecedence = new()
    {
        [SyntaxKind.NotKeyword] = 7,
        [SyntaxKind.PlusToken ] = 3,
        [SyntaxKind.MinusToken] = 3
    };


    public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
    {
        if (binaryOperatorPrecedence.TryGetValue(kind, out int value)) 
            return value;

        return 0;
    }

    public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
    {
        if (unaryOperatorPrecedence.TryGetValue(kind, out int value)) 
            return value;

        return 0;
    }

    public static SyntaxKind GetKeywordKind(string token)
    {
        if (keywordKind.TryGetValue(token, out SyntaxKind value)) 
            return value;

        return SyntaxKind.IdentifierToken;
    }

    // Método que evalúa los slash en el string
    public static string BackSlashEvaluate(string text) {
        // Secuencias de escape permitidas
        char[] scapes = {'n', 'r', 't', 'a', 'f', 'b', 'v', '"', '\'', '\\'};
        char[] scapeSequency = {'\n', '\r', '\t', '\a', '\f', '\b', '\v'};

        int backSlashIndex = text.IndexOf("\\"); 

        // Se itera mientras tenga slash
        while (backSlashIndex != -1) {
            int count = 0;

            // Se cuentan cuántos seguidos hay
            for (int i = backSlashIndex; i < text.Length; i++) {
                if (text[i] != '\\') break;
                count ++;
            }

            // Se remueve la mitad de los slash
            text = text.Remove(backSlashIndex, count / 2);

            // Si no es par, se inserta el caracter de escape deseado
            if (count % 2 != 0) {
                int scapeIndex  = Array.IndexOf(scapes, text[backSlashIndex + count - count / 2]);
                text = text.Remove(backSlashIndex, 1);

                if (!(scapes[scapeIndex] == '"' || scapes[scapeIndex] == '\'' || 
                    scapes[scapeIndex] == '\\')) 
                {
                    text = text.Remove(backSlashIndex + count - count / 2 - 1, 1);
                    text = text.Insert(backSlashIndex + count - count / 2 - 1, scapeSequency[scapeIndex].ToString());
                }
            }
            
            backSlashIndex = text.IndexOf("\\", backSlashIndex + count / 2);
        }
        
        return text;
    }
}