namespace WallE;

// Clase que evalúa los paréntesis
public class Parenthesis
{

    // Método que revisa si la expresión lleva paréntesis
    public static bool IsParenthetical(string s) {
        s = String.StringsToSpaces(s);
        return s.Contains("(");
    }

    public static string Eval(string s) {

        /* Se eliminan los espacios innecesarios y se ignoran
        los strings */
        s = Extra.SpacesBeforeParenthesis(s);
        s = Extra.SpacesBetweenTokens(s);
        
        string n = String.StringsToSpaces(s);
       
        int open = n.LastIndexOf("(");
        int closed = n.IndexOf(")", open);

        /* Comprobamos si los paréntesis pertenecen a una función, en cuyo caso
        obtenemos sus datos (nombre, posición en la cadena de entrada, argumentos...) */
        (bool, string, string[]) functionData = Function.GetData(s, open, closed);

        if (functionData.Item1) {
            string funcName = functionData.Item2;
            int start = open + 1 - funcName.Length;
            string f = s[start..(closed + 1)];
            string[] arg = functionData.Item3;
            
            for (int i = 0; i < functionData.Item3.Length; i++) arg[i] = Main.Parse(arg[i]);
    
            string argument = string.Join(", ", arg);  

            // Se comprueba que la función esté creada con anterioridad
            if (Cache.defaultFunctions.Contains(funcName)) {
                
                // Se evalúa la función
                string function = Function.Eval(f, arg);

                if (function == "") return "";

                // Se llena el campo que cuenta los llamados de cada función
                if (Cache.recursionCount.ContainsKey(funcName)) Cache.recursionCount[funcName] ++;
                else Cache.recursionCount[funcName] = 1;

                /* Si una función es llamada más de 300 veces entonces se lanza
                un error de ejecución para evitar excepción por Overflow */
                if (Cache.recursionCount[funcName] == 300) {
                    Check.SetErrors("RUNTIME", "Stack Overflow");
                    return "";
                }

                // Se parsea la evaluación de la función
                function = Main.Parse(function);

                /* Se verifica si en la caché esta guardado el valor de retorno de la función
                para agilizar el parsing, sobre todo con la recursividad */
                if (!Cache.recursionSpeed.ContainsKey($"{funcName}{argument})") && 
                    function != "" && Cache.recursionFunc.Contains(funcName)) 
                {
                    Cache.recursionSpeed[$"{funcName}{argument})"] =  function;    
                }         

                /* Si el valor está guardado en la caché no se cuenta como llamado recursivo,
                para que así puedan existir aún más llamados */
                else Cache.recursionCount[funcName] --;     
                
                // Se parsea la expresión global con la función evaluada
                return Main.Parse(s[..start] + $"({function})" + s[(closed + 1)..]);
            }
            
            /* En caso de que la función sea 'print' (la cual está guardada como keyword, no como
            función) se evalúa */
            if (f.StartsWith("print(")) { 
                return Main.Parse(s[..start] + Main.Parse(argument) + s[(closed + 1)..]);
            }                
        }

        // Si no es una función se parsea lo que está dentro de los paréntesis
        string parenthesis = Main.Parse(s[(open + 1)..closed]);

        if (parenthesis == "") {
            // Se lanza un error por si la entrada es '()'
            Check.SetErrors("SYNTAX", "Invalid ')' after '('");
            return "";
        }

        // Si los paréntesis son de una potencia, se evalúa
        if (s[(closed + 1)..].Replace(" ", "").StartsWith("^")) 
        {
            (string, int) tuple = Power.EvalWithParenthesis(s, n, closed, parenthesis);
            parenthesis = tuple.Item1;
            closed = tuple.Item2;
        }
        
        // Se parsea la expresión global con lo que estaba entre paréntesis evaluado
        return Main.Parse(s[..open] + parenthesis + s[(closed + 1)..]);
    }
}