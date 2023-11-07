using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

namespace WallE;

/* Clase que evalúa la instrucción 'function' y las funciones ya creadas
mediante esta instrucción */
public class FuncInstruction
{
    // Método que determina si la entrada es una instrucción 'function'
    public static bool IsFunctionInstruction(string s) {
        string n = String.StringsToSpaces(s);
        int equalIndex = n.IndexOf("=");
        
        while (equalIndex != -1) {
            if(equalIndex != s.Length - 1 && n[equalIndex + 1] != '=') {
                string leftSide = n[..equalIndex].Trim();
                if(leftSide.EndsWith(")")) return true;
            }

            equalIndex = n.IndexOf("=", equalIndex + 1);
        }

        return false;
    }

    // Método que crea las funciones
    public static string CreateFunction(string s) {
        int count = 0;

        while (s.StartsWith("(")) {
            s = s.Remove(0, 1); 
            count ++;
        }
        
        s = s.Remove(s.Length - count);
        string n = s.Replace(" ", "");

        if (n.StartsWith('(') || (!char.IsDigit(n[0]) && !char.IsLetter(n[0]) && n[0] != '_') ||
            Check.BalanceRevision(s, '(') != 0) 
        {
            Check.SetErrors("SYNTAX", $"Invalid 'function' declaration");
            return "";
        }

        // Se revisa que no haya errores
        if (!Check.FunctionRevision(s)) return "";

        // Se almacenan los datos de la función
        string funcName = Extra.SpacesBeforeParenthesis(s[..(s.IndexOf("(") + 1)]);
        string argument = s.Substring(s.IndexOf("(") + 1, s.IndexOf(")") - s.IndexOf("(") - 1);
        string body = Extra.SpacesBeforeParenthesis(s[(s.IndexOf("=") + 1)..]);
        List<string> vars = argument.Split(",").ToList();

        for (int i = 0; i < vars.Count; i++) 
            vars[i] = vars[i].Trim();
        
        Cache.newFunctions[funcName] = body;
        Cache.funcVars[funcName] = vars;
        Cache.defaultFunctions.Add(funcName);
    
        if (Cache.returnType[funcName] == "all") {
            Cache.returnType[funcName] = Types.GetFunctionType(body, vars);
        }

        // s = s.Remove(initial, stop - initial);

        return s;     
    }

    // Método que evalúa las funciones creadas mediante 'function'
    public static string Eval(string body, List<string> vars, List<string> values) {
       
        string s = String.StringsToSpaces(body);
        string[] tokens = {
            "*", "/", "^", "%", "+", "-", "(", ")", ">", 
            "<", "&","|","!", ",", "@", "=", "\"", " "
        };
        
        /* Ciclo para hacer las sustituciones en el cuerpo de la función, de las variables
        por los valores entrados como argumentos */
        for (int i = 0; i < vars.Count; i++)
        {
            if (s.Contains(vars[i])) {

                /* Se revisa cada token del cuerpo y si es una de las variables de la función 
                entonces se intercambia por su valor correspondiente */
                List<string> expressions = s.Split(tokens, StringSplitOptions.RemoveEmptyEntries).ToList();
                expressions.RemoveAll(string.IsNullOrWhiteSpace);
                string[] newExpressions = new string[expressions.Count];

                for (int j = 0; j < expressions.Count; j++) {
                    newExpressions[j] = (Extra.SpacesBeforeParenthesis(expressions[j]) == vars[i])? 
                                    $"({values[i]})" : expressions[j];
                }
                
                int position = s.Length;

                for (int j = expressions.Count - 1; j >= 0; j--) {
                    position = s[..position].LastIndexOf(expressions[j]);
                    body = body.Remove(position,expressions[j].Length);
                    body = body.Insert(position, newExpressions[j]);
                } 
                
                s = String.StringsToSpaces(body);
            }
        }

        // Se retorna el cuerpo modificado 
        return body;
    }
}