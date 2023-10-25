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
    public static Dictionary<string, List<string>> InputType = new() {
        ["print("] = new() {"all"},    ["cos(" ] = new() {"number"},  ["sin("] = new() {"number"}, 
        ["tan("  ] = new() {"number"}, ["sqrt("] = new() {"number"},  ["log("] = new() {"number", "number"}, 
        ["rand(" ] = new() {""},       ["exp(" ] = new() {"number"}
    };

    // Palabras reservadas por el intérprete
    public static List<string> keyWords = new() {
        "True", "False", "true", "false", "function", "if", 
        "elif", "else", "let", "in", "PI", "E", "print"
    };

    // Todas las funciones (Creadas por el usuario y las definidas por defecto)
    public static List<string> defaultFunctions = new() {
        "cos(", "sin(", "tan(", "sqrt(", "log(", "rand(", "exp("
    };
}