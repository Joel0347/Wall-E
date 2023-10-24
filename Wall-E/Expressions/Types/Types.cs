namespace Hulk;

// Clase que evalúa los tipos específicos (bool, string, number)
public class Types
{
    public static string result = "";

    // Se evalúa dependiendo del tipo
    public static bool IsType(string s) {

        // Se sustituyen las constantes por sus valores
        s = Extra.GetValueOf_PI_E(s);

        if (Parenthesis.IsParenthetical(s)) result = Parenthesis.Eval(s);
        else if (String.IsString(s)) result = String.Eval(s);
        else if (Boolean.IsBoolean(s))  result = Boolean.Eval(s);
        else if (Numeric.IsNumeric(s)) result = Numeric.Eval(s);
        else return false;

        return true;
    }

    // Método para obtener el tipo de una expresión
    public static string GetType(string s) {
        s = s.Trim();

        if (string.IsNullOrWhiteSpace(s)) return "";
        
        if (String.IsString(s)) return "string";
        if (Boolean.IsBoolean(s)) return "boolean";
        if (Numeric.IsNumeric(s)) return "number";
        if (Function.IsFunction(s)) return Cache.returnType[s[..(s.IndexOf("(") + 1)]];

        return "undefined expression";
    }

    // Método para obtener el tipo de retorno de una función
    public static string GetFunctionType(string body, List<string> vars) {
        string n = Extra.FunctionsToSpaces(body);
        
        if (n.Contains('(')) {
            int index = n.LastIndexOf("(");
            int index2 = n.IndexOf(")", index);
            string parenthesis = body[(index + 1)..index2];
            
            return GetFunctionType(body[..index] + parenthesis + body[(index2 + 1)..], vars);
        }

        if (vars.Any(x => x == body)) return "all";

        return GetType(body);     
        
    }
}
