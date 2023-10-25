using System.Text.RegularExpressions;
namespace WallE;

// Clase que contiene métodos necesarios en el funcionamiento del intérprete
public static class Extra
{
    // Primer llamado recursivo 
    public static bool recursion = false;

    // Visualización en consola
    #region Visualization
    public static void Visualization() {
        string[] words = {
            "»»»»»»»»»»»»»» ", "H", "avana ", "U", "niversity ",
            "L", "anguage for ", "K", "ompilers", " ", "««««««««««««««\n"
        }; 

        static void Visualization(string[] words, int i) {

            if (i == words.Length) return;

            if (i % 2 == 0) Console.ForegroundColor = ConsoleColor.Green;
            else Console.ForegroundColor = ConsoleColor.Red;

            Console.Write(words[i]);
            Visualization(words, i + 1); 
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("                           H   U   L   K");
        Visualization(words, 0);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("»»»»»»»»»»» Press CRTL + C or type 'good bye hulk' to exit «««««««««««\n");
    }

    #endregion

    // Métodos para trabajar con paréntesis
    #region Parenthesis Methods

    // Método para obtener el paréntesis que cierra a uno abierto dado
    public static (int, string) GetClosingParenthesis(int i, int start, string s) {
         while (i != start) {
            int indexParenthesis2 = s.IndexOf(")", start);
            s = s.Remove(start, 1);
            s = s.Insert(start, " ");
            s = s.Remove(indexParenthesis2, 1);
            s = s.Insert(indexParenthesis2, " ");
            start = s.LastIndexOf("(");
        }

        return (start, s);
    }

    // Método para remover espacios innecesarios antes de los paréntesis
    public static string SpacesBeforeParenthesis(string s) {
        
        while (s.StartsWith(" ")) s = s.Remove(0, 1);
        while (s.EndsWith(" ")) s = s.Remove(s.Length - 1);
        
        int index = s.IndexOf("(");
        index = (index == 0)? s.IndexOf("(", index + 1) : index;
        
        while (index != -1) {
            if (Quotes(s[index..]) % 2 == 0 && s[index - 1] == ' ') {
                s = s.Remove(index - 1, 1);
                index --;
            }
            else index = s.IndexOf("(", index + 1);
        }

        index = s.IndexOf(")");
        
        while (index != -1 && index != 0) {
            if (Quotes(s[index..]) % 2 == 0 && s[index - 1] == ' ') {
                s = s.Remove(index - 1, 1);
                index --;
            }
            else index = s.IndexOf(")", index + 1);
        }

        return s;
    }

    #endregion

    // Métodos para trabajar con strings
    #region String Methods
    
    // Método para contar comillas
    public static int Quotes(string s) {
        
        int d = 0;
        int count = 0;
        int index = s.IndexOf("\"");

        while (index != -1) {

            for (int i = index; i > 0; i--) {
                if (s[i - 1] != '\\') break;
                count ++;
            }

            if (count % 2 == 0) d++;
            count = 0;
            index = s.IndexOf("\"", index + 1);  
        }

        return d;
    }

    // Método para revisar los '/' como secuencias de escape
    public static bool SlashRevision(string s) {
        char[] scapes = {'n', 'r', 't', 'a', 'f', 'b', 'v', '"', '\'', '\\'};
         int index = s.IndexOf("\\"); 

        while (index != -1) {
            int count = 0;

            for (int i = index; i < s.Length; i++) {
                if (s[i] != '\\') break;
                count ++;
            }
            
            s = s.Remove(index, count / 2);
            
            if (count % 2 != 0) {
                int scapeIndex  = Array.IndexOf(scapes, s[index + count - count / 2]);

                if (scapeIndex == -1) {
                    Check.SetErrors("SYNTAX", "Scape sequency not allowed in 'string'");
                    return false;
                }

                else s = s.Remove(index, 1);
            }

            index = s.IndexOf("\\", index + count / 2);
        }
        
        return true;
    }

    #endregion
    
    // Métodos para trabajar con numbers
    #region Numeric Methods

    // Método para eliminar espacios entre símbolos
    public static string SpacesBetweenTokens(string s) {
        char[] symbols = {'+', '-', '*', '/', '%', '^'};
        string m = SpacesBeforeParenthesis(s);

        int index_1 = m.IndexOfAny(symbols);

        while (index_1 != -1 && index_1 < (s.Length - 1))
        {
            while (m[index_1 + 1] == ' ') {
                s = s.Remove(index_1 + 1, 1);
                m = SpacesBeforeParenthesis(s);
            }

            index_1 = m.IndexOfAny(symbols, index_1 + 1);
        }

        return s;
    }

    // Método para convertir PI y E en sus valores numéricos
    public static string GetValueOf_PI_E(string s) {

        List<string> constants = new() {"PI", "E"};
        List<string> values = new() {Math.PI.ToString(), Math.E.ToString()};
        s = FuncInstruction.Eval(s, constants, values);

        return s;
    }

    #endregion

    // Métodos para trabajar con funciones
    #region Function Methods

    // Método que permite la evaluación de funciones recursivas con argumentos grandes
    public static void RecursiveEval(string[] args, string f) {
        if (args.Length == 1 && Numeric.IsNumber(args[0]) && double.Parse(args[0]) >= 290 && 
            Cache.recursionFunc.Contains(f) && !recursion) 
        {
                /* El punto crítico de las funciones recursivas es de alrededor de 300 llamados
                antes de dar overflow cuando se utilizan parámetros numéricos gigantes. La solución 
                consiste en ir llenando la caché poco a poco, de forma segura, digamos de 290 en 290. 
                De esta forma es posible evaluar funciones recursivas con valores relativamente grandes. 
                Si supera las 10 veces los 290 llamados entonces se lanza el error */
                int count = (int) double.Parse(args[0]) / 290;
                recursion = true;

                if (count > 10) {
                    Check.SetErrors("RUNTIME", "Stack Overflow");
                    return;
                }

                int times = 2;
                string recursionLimit = "290";
                
                while (count > 0) {
                    List<string> val = new() {recursionLimit};

                    Main.Parse($"{f}{val[0]})");
                    Cache.recursionCount[f] = 0;
                    double temp = 290 * times;
                    recursionLimit = temp.ToString();
                    count--;
                    times++;
                }
        }
    }
    // Método para ignorar funciones
    public static string FunctionsToSpaces(string s) {
        string n = String.StringsToSpaces(s);
        int index = n.LastIndexOf("(");

        while (index != -1) {
            
            int index2 = n.IndexOf(")", index);

            if (index > 0 && (char.IsLetterOrDigit(n[index - 1]) || n[index - 1] == '_' || 
                n[index - 1] == ' ')) {
                    
                int start = -1;
                
                for (int k = index - 1; k >= 0; k--) 
                {
                    if ((!char.IsLetterOrDigit(n[k]) && n[k] != '_' && n[k] != ' ') ||
                        (n[k] == ' ' && (char.IsLetterOrDigit(n[k + 1]) || n[k + 1] != '_'))) {

                        start = k;
                        break;
                    }
                }

                if (string.IsNullOrWhiteSpace(n[(start + 1)..index])) {

                    n = n.Remove(index, 1);
                    n = n.Insert(index, "\"");
                    n = n.Remove(index2, 1);
                    n = n.Insert(index2, "\"");
                    n = String.StringsToSpaces(n);
                }

                else {

                    string f = $"\"{s[(start + 1)..(index2 + 1)]}\"" ;
                    f = String.StringsToSpaces(f);
                    s = s.Remove(start + 1, index2 - start);
                    s = s.Insert(start + 1, f[1..(f.Length - 1)]);
                }
            }

            else {
                n = n.Remove(index, 1);
                n = n.Insert(index, "\"");
                n = n.Remove(index2, 1);
                n = n.Insert(index2, "\"");
                n = String.StringsToSpaces(n);
            }

            index = n[..index].LastIndexOf("(");
        }

        return s;
    }

    #endregion

    //  Métodos para trabajar con condicionales
    #region Conditionals Methods

    // Método para convertir los 'else if' a 'elif'
    public static string CreateElif(string s) {
        string m = String.StringsToSpaces(s);
        string n = Regex.Replace(m, @"[^_""ñÑ,A-Za-z0-9]", " ");

        int index_1 = n.IndexOf(" else ");
        int index_2 = n.IndexOf(" if ", index_1 + 1);
        
        while (index_1 != -1 && index_2 != -1) {

            if (string.IsNullOrWhiteSpace(s[(index_1 + 5)..(index_2 + 1)])) {
                s = s.Remove(index_1 + 3, index_2 - index_1 - 2);
            }

            m = String.StringsToSpaces(s);
            n = Regex.Replace(m, @"[^_""ñÑ,A-Za-z0-9]", " ");
            index_1 = n.IndexOf(" else ", index_1 + 1);
            
            index_2 = n.IndexOf(" if ", index_1 + 1);
        }

        return s;
    }

    #endregion

    // Métodos para resetear el programa
    #region Reset Program
    public static void Reset() {
        Main.error = false;
        Cache.recursionCount = new();
        Check.funcVars = new();
        Check.vars = new();
        Check.funcName = "";
        recursion = false;
    }

    #endregion
}