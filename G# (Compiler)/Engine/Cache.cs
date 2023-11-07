namespace WallE;

// Clase que almacena los datos guardados (cache)
public static class Cache
{
    // Funciones recursivas creadas
    public static List<string> recursionFunc = new(); 

    // Valor de retorno de las funciones para agilizar la recursividad
    public static Dictionary<string, string> recursionSpeed = new();  

    // Cantidad de llamados recursivos
    public static Dictionary<string, int> recursionCount = new();

    // Funciones creadas por el usuario
    public static Dictionary<string, string> newFunctions = new();
    
    // Variables de las funciones creadas por el usuario
    public static Dictionary<string, List<string>> funcVars = new(); 
    
    // Tipo de valor de retorno de cada función 
    public static Dictionary<string, string> returnType = new() {
        ["print("] = "all",    ["cos("] = "number", ["sin(" ] = "number",  ["tan("] = "number", 
        ["sqrt(" ] = "number", ["log("] = "number", ["rand("] = "number", ["exp("] = "number"
    };

    // Tipos de entrada que reciben las funciones
    public static Dictionary<string, List<string>> inputType = new() {
        ["print("] = new() {"all"},    ["cos(" ] = new() {"number"},  ["sin("] = new() {"number"}, 
        ["tan("  ] = new() {"number"}, ["sqrt("] = new() {"number"},  ["log("] = new() {"number", "number"}, 
        ["rand(" ] = new() {""},       ["exp(" ] = new() {"number"}
    };

    // Palabras reservadas por el intérprete
    public static List<string> keyWords = new() {
        "and", "not", "or", "if", "elif", "else", 
        "then", "let", "in", "PI", "E", "print"
    };

    // Todas las funciones (Creadas por el usuario y las definidas por defecto)
    public static List<string> defaultFunctions = new() {
        "cos(", "sin(", "tan(", "sqrt(", "log(", "rand(", "exp("
    };

    public static Dictionary<string, string> constantsType = new();
    public static List<string> color = new() {"black"};
    public static Dictionary<string, string> constantValues = new();

    public static void CacheReset(
        int recursionFuncLength, int recursionSpeedLength, 
        int recursionCountLength, int functionLength, 
        int constantLength, int colorLength
    ) 
    {
        if(recursionFunc.Count != recursionFuncLength) {
            recursionFunc.RemoveRange(recursionFuncLength, recursionFunc.Count - recursionFuncLength);
        }

        if(color.Count != colorLength) {
            color.RemoveRange(colorLength, color.Count - colorLength);
        }

        if(recursionCount.Count != recursionCountLength) {
            List<string> keys = recursionCount.Keys.ToList();
            for (int i = recursionCountLength; i < keys.Count; i++)
            {
                recursionCount.Remove(keys[i]);
            }
        }

        if(recursionSpeed.Count != recursionSpeedLength) {
            List<string> keys = recursionSpeed.Keys.ToList();
            for (int i = recursionSpeedLength; i < keys.Count; i++)
            {
                recursionSpeed.Remove(keys[i]);
            }
        }

        if(funcVars.Count != functionLength) {
            List<string> keys = funcVars.Keys.ToList();
            for (int i = functionLength; i < keys.Count; i++)
            {
                funcVars.Remove(keys[i]);
                newFunctions.Remove(keys[i]);
                inputType.Remove(keys[i]);
                returnType.Remove(keys[i]);
            }
        }

        if(constantsType.Count != constantLength) {
            List<string> keys = constantsType.Keys.ToList();
            for (int i = constantLength; i < keys.Count; i++)
            {
                constantsType.Remove(keys[i]);
                constantValues.Remove(keys[i]);
            }
        }
    }

}