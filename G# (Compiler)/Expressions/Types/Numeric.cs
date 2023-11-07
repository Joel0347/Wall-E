using System.Net.Http.Headers;

namespace WallE;

// Clase que evalúa los tipos numéricos
public class Numeric : Types
{  
    public static char[] NumericTokens = {'*', '/', '^', '%', '+', '-'};
    
    // Método que determina si la expresión es un número 
    public static bool IsNumber(string s) {
        if (s.Contains(",")) return false;
        return double.TryParse(s, out _) || s == "PI" || s == "E";
    }
    
    // Método que determina si la expresión es numérica
    public static bool IsNumeric(string s) {
        string n = String.StringsToSpaces(s);

        return IsNumber(s) || NumericTokens.Any(n.Contains);
    }

    // Método que evalúa las expresiones numéricas
    public static string Eval(string s)
    {
        string temp = (s.StartsWith("+") || s.StartsWith("-"))? s[1..] : s;
        if (Cache.constantValues.ContainsKey(temp.Trim())) {
            s = Cache.constantValues[temp.Trim()];
        }

        s = Extra.SpacesBetweenTokens(s);
        s = Extra.GetValueOf_PI_E(s);
        string newS = String.StringsToSpaces(s);

        // Se eliminan los signos dobles innecesarios sustituyéndolos por sus equivalentes simples
        while (newS.Contains("+-") || newS.Contains("-+") || 
            newS.Contains("++") || newS.Contains("--")) 
        {
            s = s.Replace("+-","-");
            s = s.Replace("-+","-");
            s = s.Replace("++","+");
            s = s.Replace("--","+");
            newS = String.StringsToSpaces(s);
        }   

        // Condición para revisar que no falte MI en la suma y la resta
        if ((s.StartsWith('+') || s.StartsWith('-')) && !IsNumeric(s[1..])) {
            char operation = s[0];
            string mssg = $"Operator '{operation}' needs a left member";
            Check.SetErrors("SEMANTIC", mssg);
            return "";
        }

        // Si es número se retorna directamente
        if (IsNumber(s)) return double.Parse(s).ToString();
        
        // Evaluación de la suma y la resta
        if (newS[1..].Contains('+') || newS[1..].Contains('-')) {
            
            // Condición para determinar si falta el MD en suma y resta
            if (s.EndsWith('+') || s.EndsWith('-')) {
                char operation = s[^1];
                string mssg = $"Operator '{operation}' needs a right member";
                Check.SetErrors("SEMANTIC", mssg);
                return "";
            }

            int index = Math.Max(newS.LastIndexOf('+'), newS.LastIndexOf('-'));
            
            /* Se corren de atrás hacia adelante los índices de '+' y '-' si se encuentra 
            con algo como: /-, *- ... y también 1E+1... pues esos no se ejecutan como suma ni resta*/ 
            while (index > 1 && newS[index - 1] == 'E' && char.IsDigit(newS[index - 2]) ||
                  (index > 0 && NumericTokens.Contains(newS[index - 1]))) 
            { 

                index = Math.Max(s[..index].LastIndexOf('+'), s[..index].LastIndexOf('-'));
            }

            /* Si el índice es mayor que cero se evalúa. La posición '0' no importa por si
            entra algo como: +1, -1 ... que son expresiones válidas */ 
            if (index > 0) {
                string leftSide = s[..index];
                string rightSide = s[index..];
                
                return Add.Eval(Eval(leftSide), Eval(rightSide));
            }
        }

        // Evaluación de la multiplicación, división, módulo
        if (newS.Contains('*') || newS.Contains('/') || newS.Contains('%')) {
            int index = Math.Max(newS.LastIndexOf('*'), Math.Max(newS.LastIndexOf('/'),newS.LastIndexOf('%')));
            string leftSide = s[..index];
            string rightSide = s[(index + 1)..];

            // Si el token es '*' se evalúa
            if (newS[index] == '*') {
                return Mult.Eval(Eval(leftSide), Eval(rightSide));
            } 

            // Si el token es '/' se evalúa
            if (newS[index] == '/') {
                return Division.Eval(Eval(leftSide), Eval(rightSide));
            }

            // Si el token es '%' se evalúa
            if (newS[index] == '%') {
                return Mod.Eval(Eval(leftSide), Eval(rightSide));
            }
        } 
       
        // Evaluación de la potencia
        if (newS.Contains("^")) {
            int index = newS.LastIndexOf('^');
            string leftSide = s[..index];
            string rightSide = s[(index + 1)..];
            string sign = "";

            /* Para respetar el signo si el exponente es par, sino lo cambiaría
            Ej: 1 - 2 ^ 2 debe devolver 1 - 4, no 1 + 4 */
            if (leftSide.StartsWith('-')) {
                sign = "-";
                leftSide = leftSide[1..];
            }
                
            return Eval(sign + Power.Eval(Eval(leftSide), Eval(rightSide)));
        }
        
        return "";
    }
}
    



