namespace Hulk;

// Clase que evalúa las funciones
public class Function 
{
    public Dictionary<string, string> defaultFunc = new();
    public string arg1 = "";
    public string arg2 = Math.E.ToString();

    // Constructor que evalúa los argumentos en cada una de las funciones
    public Function(string[] s) {

        if (s.Length == 1) arg1 = s[0];

        else {
            arg1 = s[1];
            arg2 = s[0];
        }

        if (!Main.error) {
            defaultFunc["cos(" ] = Math.Cos(double.Parse(arg1)).ToString();
            defaultFunc["sin(" ] = Math.Sin(double.Parse(arg1)).ToString();
            defaultFunc["tan(" ] = Math.Tan(double.Parse(arg1)).ToString();
            defaultFunc["sqrt("] = Math.Sqrt(double.Parse(arg1)).ToString();
            defaultFunc["log(" ] = Math.Log(double.Parse(arg1), double.Parse(arg2)).ToString();
            defaultFunc["exp(" ] = Math.Pow(Math.E, double.Parse(arg1)).ToString();
        } 
    } 

    // Método que determinar si la entrada es una función
    public static bool IsFunction(string s) {
        if(string.IsNullOrWhiteSpace(s)) return false;
        s = Extra.FunctionsToSpaces(s);
        s = s.Replace(" ","");
        return s == "";
    }

    // Método que obtiene los datos de la función
    public static (bool, string, string[]) GetData(string s, int index, int index2) {
        
        string n = String.StringsToSpaces(s);

        // Se revisa que antes del paréntesis exista un caracter que pueda pertenecer al nombre
        if (index > 0 && (char.IsLetterOrDigit(n[index - 1]) || n[index - 1] == '_')) {
            int start = -1;

            //  Se busca dónde empieza el nombre de la función
            for (int k = index - 1; k >= 0; k--) {

                if (!char.IsLetterOrDigit(n[k]) && n[k] != '_') {
                    start = k;
                    break;
                }
            }

            // Se guarda el nombre y se crea el array de argumentos
            string f = s[(start + 1)..(index2 + 1)];
            string argument = f[(f.IndexOf("(") + 1)..(f.Length - 1)];
            string newArgument = String.StringsToSpaces(argument);
            int tempIndex = newArgument.IndexOf(",");
            List<string> values = new();
    
            while (tempIndex != -1) {
                values.Add(argument[..tempIndex]);
                argument = argument.Remove(0, tempIndex + 1);
                newArgument = newArgument.Remove(0, tempIndex + 1);
                tempIndex = newArgument.IndexOf(",");
            }

            values.Add(argument);
            string[] arg = values.ToArray();
            string funcName = f[..(f.IndexOf("(") + 1)];

            return (true, funcName, arg);
        }

        // Se retorna un valor por defecto por si no es función
        return (false, "", new string[0]);
    }

    // Método que evalúa las funciones 
    public static string Eval(string s, string[] args) {
        string n = String.StringsToSpaces(s);
        if (Main.error) return "";

        int index1 = n.IndexOf('(');
        string argument = string.Join(", ", args);
        string f = s[..(index1 + 1)];

        // Si la función fue creada por el usuario se evalúa en la clase 'FuncInstruction'
        if (Cache.newFunctions.ContainsKey(f)) {
            
            // Si la función(recursiva) fue llamada con el mismo argumento estará guardada en la caché
            if(Cache.recursionSpeed.ContainsKey($"{f}{argument})")) {
                return Cache.recursionSpeed[$"{f}{argument})"];
            }

            /* Si la función es recursiva se trata de incrementar la cant de llamados al máximo antes
            de dar overflow */
            Extra.RecursiveEval(args, f);
            if (Main.error) return "";

            return FuncInstruction.Eval(Cache.newFunctions[f], Cache.funcVars[f], args.ToList());
        }

        // Si es 'rand' se evalúa aparte
        if (f[..^1] == "rand") {
            Random r = new();
            return r.NextDouble().ToString();         
        }

        // Se revisa que el argumento de 'sqrt' no sea negativo
        if (f == "sqrt(") {
            string arg1 = Main.Parse(args[0]);
            if (double.Parse(arg1) < 0) {
                string mssg = $"√({args[0]}) is not defined. Argument must be positive";
                Check.SetErrors("SEMANTIC", mssg);
                return "";
            }
        }

        // Se revisan las restricciones del logaritmo
        if (f == "log(") {
            string arg1 = Main.Parse(args[0]);
            if (args.Length == 1 && double.Parse(arg1) <= 0) {
                string mssg = "Argument must be greater than '0' in 'log' function";
                Check.SetErrors("SEMANTIC", mssg);
                return "";
            }

            if (args.Length > 1) {
                string arg2 = Main.Parse(args[1]);

                if (double.Parse(arg2) <= 0) {
                    string mssg = "Argument must be greater than '0' in 'log' function";
                    Check.SetErrors("SEMANTIC", mssg);
                    return "";
                }

                if (double.Parse(arg1) <= 0 || double.Parse(arg1) == 1) {
                    string mssg = "Base must be greater than '0' and diferent from '1' in 'log' function";
                    Check.SetErrors("SEMANTIC", mssg);
                    return "";
                }
            }
        }
        
        /* De lo contrario es una función matemática predefinida. Por tanto
        se crea un objeto de tipo 'Function' que evaluará cada una con los 
        argumentos pasados */
        Function result = new(args);

        if (Main.error) return "";
        if (result.defaultFunc.ContainsKey(f)) return result.defaultFunc[f];
        
        Check.SetErrors("SEMANTIC", $"'{f[..^1]}' is not defined");
        return "";
    }
}