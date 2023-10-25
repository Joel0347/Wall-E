using System.Numerics;

namespace WallE;

// Clase que evalúa los tipos booleanos
public class Boolean : Types
{
    static readonly string[] booleansTokens = {"&", "|", "!", "!=", "==", ">", "<", "<=", ">="};
    
    // Método que determina si la expresión es booleana
    public static bool IsBoolean(string s) {
        s = String.StringsToSpaces(s);
        s = s.Replace("(","");
        s = s.Replace(")","");

        return bool.TryParse(s, out bool _) || booleansTokens.Any(s.Contains);
    }

    // Método que evalúa las expresiones booleanas
    public static string Eval(string s) {

        string n = String.StringsToSpaces(s);

        // Si es 'true' o 'false' se retorna directamente
        if (bool.TryParse(s, out bool val)) return val.ToString();

        // Evaluación de 'and' y 'or'
        if (n.Contains("&") || n.Contains("|")) {
            int index = Math.Max(n.LastIndexOf("&"), n.LastIndexOf("|"));
            char operation = s[index];
            string leftSide = Eval(s[..index]);
            string rightSide = Eval(s[(index + 1)..]);
            
            return (operation == '&')? And.Eval(leftSide, rightSide) : Or.Eval(leftSide, rightSide); 
        }

        // Evaluación de 'igualdad' y 'desigualdad'
        if (n.Contains("==") || n.Contains("!=")) {
            int index = Math.Max(n.LastIndexOf("=="), n.LastIndexOf("!="));
            string operation = s.Substring(index, 2);
            string leftSide = Main.Parse(s[..index]);
            string rightSide = Main.Parse(s[(index + 2)..]);

            return (operation == "==")? Equal.Eval(leftSide, rightSide) : NotEqual.Eval(leftSide, rightSide); 
        }

        // Evaluación de 'mayor o igual' y 'menor o igual'
        if (n.Contains("<=") || n.Contains(">=")) {
            int index = Math.Max(n.LastIndexOf("<="), n.LastIndexOf(">="));
            string operation = s.Substring(index, 2);
            string leftSide = Eval(s[..index]);
            string rightSide = Eval(s[(index + 2)..]);

            return (operation == "<=")? LessEqual.Eval(leftSide, rightSide) : GreatEqual.Eval(leftSide, rightSide);   
        }

        // Evaluación de 'mayor' y 'menor'
        if (n.Contains(">") || n.Contains("<")) {
            int index = Math.Max(n.LastIndexOf(">"), n.LastIndexOf("<"));
            char operation = s[index];
            string leftSide = Eval(s[..index]);
            string rightSide = Eval(s[(index + 1)..]);

            return (operation == '>')? GreatThan.Eval(leftSide, rightSide) : LessThan.Eval(leftSide, rightSide);
        }

        // Evaluación del 'Not'
        if (n.Contains("!")) {
            int index = n.LastIndexOf("!");

            return Not.Eval(Eval(s[(index + 1)..]));   
        }

        return Main.Parse(s);
    }
}