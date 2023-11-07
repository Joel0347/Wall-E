using System.Numerics;
using System.Text.RegularExpressions;

namespace WallE;

// Clase que evalúa los tipos booleanos
public class Boolean : Types
{
    static readonly string[] booleansTokens = {" and ", " or ", " not ", "!=", "==", ">", "<", "<=", ">="};
    protected static string[] defaultValues = {"0", "{}", "undefined"};
    
    // Método que determina si la expresión es booleana
    public static bool IsBoolean(string s) {
        s = String.StringsToSpaces(s);
        s = s.Replace("(","");
        s = s.Replace(")","");
        s = $" {s} ";
        string m =  Regex.Replace(s, @"[^_""ñÑA-Za-z0-9]", " ");

        return booleansTokens.Any(s.Contains);
    }

    // Método que evalúa las expresiones booleanas
    public static string Eval(string s) {
        s = $" {s} ";
        string n = String.StringsToSpaces(s);
        string m =  Regex.Replace(n, @"[^_""ñÑA-Za-z0-9]", " ");

        // Si es 'true' o 'false' se retorna directamente
        if (bool.TryParse(s, out bool val)) return val.ToString();

        // Evaluación de 'and' y 'or'
        if (m.Contains(" and ") || n.Contains(" or ")) {
            int index = Math.Max(m.LastIndexOf(" and "), m.LastIndexOf(" or "));
            char operation = (s[(index + 1)..(index + 4)] == "and")? '&' : '|';
            string leftSide = Eval(s[..(index + 1)]);
            string rightSide = Eval(s[(index + 4)..]);
            
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
        if (m.Contains(" not ")) {
            int index = m.LastIndexOf(" not ");

            return Not.Eval(Eval(s[(index + 4)..]));   
        }

        return Main.Parse(s);
    }
}