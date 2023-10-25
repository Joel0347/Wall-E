using System.Text.RegularExpressions;
namespace WallE;

// Clase que evalúa las condicionales (if-else)
public class Conditional       
{
    private const List<string> value = null!;
    static readonly string[] symbols = {
        "*", "/", "^", "%", "+", "-", "(", ")", 
        ">", "<", "&","|","!", ",", "=", " ", "@", ";"
    };

    static readonly List<string> booleanValues = new() {
        "0", "undefined", "{}"
    };
    
    // Método para comprobar que la entrada sea una condicional
    public static bool IsConditional(string s) {
        s = String.StringsToSpaces(s);
        string[] token = s.Split(symbols, StringSplitOptions.RemoveEmptyEntries);
        return token.Contains("if");
    }

    // Método que evalúa la condicional
    public static string Eval(string s) {

        /* Se obtienen los datos de la condicional
        - sintaxis correcta, condición, cuerpos, posición inicial y final en la expresión
        global */
        (bool, string, string, string, int, int, string) conditionalData = GetData(s);

        // Se revisa que la condicional tenga una sintaxis correcta
        if (!conditionalData.Item1) return "";

        // Se copian los datos en sus variables correspondientes
        string eval = Main.Parse(conditionalData.Item2);
        if (eval == "") return "";
        
        bool condition = bool.Parse(eval);
        string body_true = conditionalData.Item3;
        string body_false = conditionalData.Item4;
        int index_if = conditionalData.Item5;
        int stop = conditionalData.Item6;
        s = conditionalData.Item7;

        // Se decide cuál es el cuerpo que retorna la condicional
        string body = condition? body_true : body_false;

        if(string.IsNullOrEmpty(body)) return "";

        // Se evalúa el cuerpo y se inserta en la expresión global
        s = s.Remove(index_if + 1, stop - index_if - 1);
        body = Main.Parse(body);
        s = s.Insert(index_if + 1, $"{body}");

        // Se evalúa la expresión global con el valor de retorno de la condicional
        return Principal.Analize(s);
    }

    // Método para obtener los datos de la condicional
    public static (bool, string, string, string, int, int, string) GetData(string s, List<string> vars = value) 
    {
        // Valor por defecto por si existe algún error
        (bool, string, string, string, int, int, string) defaultOutput = (false, "", "", "", 0, 0, "");
        s = s.Insert(0, " ");
        s = s.Insert(s.Length, " ");

        // Se transforman todos los 'else-if' en 'elif'
        s = Extra.CreateElif(s);
        string m = String.StringsToSpaces(s);
        string n = Regex.Replace(m, @"[^_""ñÑA-Za-z0-9]", " ");

        int index_if = n.LastIndexOf(" if ");
        int index_then = n.IndexOf(" then ", index_if) + 1;

        while (s[(index_if + 4)..].Trim().StartsWith(";")) {
            int semicolonIndex = m.IndexOf(";", index_if + 1);
            s = s.Remove(semicolonIndex, 1);
            s = s.Insert(semicolonIndex, " ");
        }

        // Se revisa que no haya nada inválido antes del 'if'
        if (s[..(index_if + 1)].Trim() != "") {
            string tempS = s[..(index_if + 1)].Trim();
            string[] keys = {"in", "else"};
            
            if (!keys.Any(Regex.Replace(tempS, @"[^_""ñÑA-Za-z0-9]", " ").EndsWith) &&
                !symbols.Any(s[..(index_if + 1)].Trim().EndsWith)) 
            {
                string mssg = tempS.Contains(" ")? tempS[(tempS.LastIndexOf(" ") + 1)..] : tempS;
                Check.SetErrors("SYNTAX", $"Invalid token '{mssg}' before 'if-else' expression");
                return defaultOutput;
            }
        }

        // La variable 'stop' determina hasta dónde llega la condicional
        int stop = m.IndexOf(";", index_then + 1);

        // Si la condicional no tiene 'then' se detecta el error
        if (index_then == 0 || stop < index_then) {
            Check.SetErrors("SYNTAX", "Missing 'then' in 'if-else' expression");
            return defaultOutput;
        }

        string condition = s[(index_if + 3)..index_then];

        while (m[index_if] == ' ' && index_if > 0) index_if --;

        // Se trabaja con los índices de paréntesis para determinar el 'stop' y la condición
        if (m[index_if] == '(') {
            int indexParenthesis_1 = m.LastIndexOf("(");
            string temp = m;
            (int, string) tuple = Extra.GetClosingParenthesis(index_if, indexParenthesis_1, temp);
            stop = tuple.Item2.IndexOf(")", tuple.Item1);
        }

        /* Condición por si la condicional es interna de una declaración de función y tiene una variable
        como condición */
        if (vars is null || !vars.Contains(condition)) {
            condition = Principal.Analize(condition);
            if(booleanValues.Contains(condition)) condition = "false";
            else condition = "true";
        }

        // Se determina si la condicional tiene como segundo cuerpo un 'elif' o 'else's
        int index_elif = n.IndexOf(" elif ", index_then);
        int index_else = n.IndexOf(" else ", index_then);
        int index_2 = (index_elif != -1)? Math.Min(index_elif, index_else) + 1 : index_else + 1;

        // Si la condicional no tiene 'else' se detecta el error
        if (index_2 == 0 || stop < index_2) {
            Check.SetErrors("SYNTAX", "Missing 'else' in 'if-else' expression");
            return defaultOutput;
        }
        
        // Se determina el cuerpo para la parte verdadera
        int start_body = index_then + 5;
        string body_true = s[start_body..index_2];

        // Se chequea que los paréntesis estén balanceados en el cuerpo true
        if (Check.ParenthesisRevision(body_true) == -1) {
            Check.SetErrors("SYNTAX", $"Missing ')' in '{body_true.Trim()}'");
            return defaultOutput;
        }

        // Se determina el cuerpo para la parte falsa (o igualmente la del 'elif')
        int start = (s[index_2..(index_2 + 4)] == "elif") ? (index_2 + 2) : (index_2 + 4);
        string body_false = s[start..stop];

        // Se detecta el error si alguno de los cuerpos es vacío
        if (string.IsNullOrWhiteSpace(body_true) || string.IsNullOrWhiteSpace(body_false)) {
            string mssg = string.IsNullOrWhiteSpace(body_true)? $"if({condition})" : "else";
            Check.SetErrors("SEMANTIC", $"Missing expression after '{mssg}' in 'if-else' instruction");
            return defaultOutput;
        }
        
        m = Extra.FunctionsToSpaces(body_false);

        // Se procede a determinar el 'stop' de la condicional en cada uno de los casos siguientes

        /* 1. Exista al menos un paréntesis desbalanceado, en ese caso ahí detenemos el 'stop' (el
        paréntesis que lo abre debe estar antes del 'if', sino habría dado error) */
        while (Check.ParenthesisRevision(m) == 1) {
            stop = start + String.StringsToSpaces(m).LastIndexOf(")");
            body_false = s[start..stop];
            m = body_false;
        }

        /* 2. Si contiene comas ('m' ignora las funciones) es porque será argumento de función y por
        tanto se detiene el 'stop' */
        if (m.Contains(",")) {
            stop = start + String.StringsToSpaces(m).IndexOf(",");
            body_false = s[start..stop];
        }

        m = String.StringsToSpaces(body_false);
        m = Regex.Replace(m, @"[^_""ñÑ,A-Za-z0-9]", " ");

        /* 3. Si contiene un 'else' significa que se está trabajando sobre el cuerpo del 'elif', e 
        igualmente se detiene el 'stop' */
        if (m.Contains(" else ") && (start == (index_2 + 4))) {
            stop = start + String.StringsToSpaces(m).LastIndexOf(" else ");
            body_false = s[start..stop];
        }

        // 4. Análogo a lo anterior se detiene el 'stop'd
        if (m.Contains(" elif ") && (start == (index_2 + 4))) {
            stop = start + String.StringsToSpaces(m).LastIndexOf(" elif ");
            body_false = s[start..stop];
        }

        // Se revisan los cuerpos sintáctica y semánticamente
        if (!Check.SyntaxCheck(body_true) || !Check.SyntaxCheck(body_false)) return defaultOutput;
        if (!Check.SemanticCheck(body_true) || !Check.SemanticCheck(body_false)) return defaultOutput;

        /* Se retornan: (si está correcta la sintaxis, condición, cuerpo del verdadero, cuerpo del
        falso, inicio de la condicional, final de la condicional, expresión global con algunos cambios) */
        return (true, condition, body_true, body_false, index_if, stop, s);
    }
}