using System.Text.RegularExpressions;
namespace WallE;

// Clase que chequea la gran mayoría de los errores
public class Check
{
   public static List<string> funcVars = new();
   public static List<string> vars = new();
   public static string funcName = "";

    // Revisión léxica y sintáctica
    #region Syntax Checking
    
    // Método que revisa la sintaxis
    public static bool SyntaxCheck(string s) {

        string n = String.StringsToSpaces(s);
        int pointIndex = n.LastIndexOf(".");

        /* Como el '.' tiene un tratamiento especial por su utilización para
        representar números de punto flotante, se verifica por separado */
        while (pointIndex > -1) {

            if (pointIndex == 0 && (n.Length == 1 || !char.IsDigit(n[pointIndex + 1]))) {
                SetErrors("LEXICAL", "Invalid token '.'");
                return false;
            }

            else if (n[pointIndex] == '.' && 
                (pointIndex == n.Length - 1 || (n[pointIndex - 1] != ' ' && 
                !char.IsDigit(n[pointIndex - 1])) || !char.IsDigit(n[pointIndex + 1])))
            {
                SetErrors("LEXICAL", "Invalid token '.'");
                return false;
            }

            n = n[..pointIndex];
            pointIndex = n.LastIndexOf(".");
        }

        s = Extra.SpacesBeforeParenthesis(s);
        n = String.StringsToSpaces(s);

        // Si es 'let-in' o 'if-else' no se revisará, pues dichas clases lo verifican por sí mismas
        if (Conditional.IsConditional(s) || Let_in.IsLet_in(s)) return true;

        // A este punto si llega un llamado 'function' será un error
        if (FuncInstruction.IsFunctionInstruction(s)) {
            string m = "Invalid 'function' instruction";
            SetErrors("SYNTAX", m);
            return false;
        }

        string[] simpleSymbols = {
            "*", "/", "^", "%", "+", "-", "(", ")", 
            ">", "<", "&","|","!", ",", "@","_", 
        };

        string[] doubleSymbols = {">=", "<=", "!=", "=="};
        string[] invalidSymbols = {
            "~", "`", "#", "$", "{", "}", "[", "]", 
            "\\", ":", ";", "'", "?"
        };

        string[] scapes = {"if", "else", "then", "and", "or", "not"};

        List<string> tokens = new();

        bool invalid = false;
        string mssg = "";
        int quotes = n.IndexOf("\"");

        if (quotes == 0) quotes = n.IndexOf("\"", quotes + 1);

        // Se revisan errores sintácticos que involucren strings
        while (quotes != -1 && quotes < n.Length - 1) {

            if (Extra.Quotes(n[..(quotes + 1)]) % 2 == 0 && 
                n[(quotes + 1)..].Replace(" ", "")[0] == '"') 
            {
                string m = "Strings must be concatenated using operator '@'";
                SetErrors("SYNTAX", m);
                return false;
            }

            if (Extra.Quotes(n[..(quotes + 1)]) % 2 == 0 && 
                char.IsLetterOrDigit(n[(quotes + 1)..].Replace(" ", "")[0])) 
            {
                string str = s[n[..quotes].LastIndexOf("\"")..(quotes + 1)];
                str = String.SlashEval(str);
                string m = $"Operator '@' was expected after {str}";
                SetErrors("SYNTAX", m);
                return false;
            }
             
            if (Extra.Quotes(n[..(quotes + 1)]) % 2 != 0 && 
                char.IsLetterOrDigit(n[..quotes].Replace(" ", "")[^1])) 
            {
                string str = s[quotes..(n.IndexOf("\"", quotes + 1) + 1)];
                str = String.SlashEval(str);
                string m = $"Operator '@' was expected before {str}";
                SetErrors("SYNTAX", m);
                return false;
            }     
            
            quotes = n.IndexOf("\"", quotes + 1);
        }

        /* Se revisan tokens inválidos en la entrada, que no sean admitidos por 
        ninguna instrucción */
        for (int i = 0; i < n.Length; i++)
        {
            if (n[i] != '∞' && !char.IsAscii(n[i]) || invalidSymbols.Contains(n[i].ToString())) {
                string m = $"Invalid token '{n[i]}'";
                SetErrors("LEXICAL", m);
                return false;
            }
        }

        // Si ignorando las funciones aún quedan comas, se lanza el error
        if (Extra.FunctionsToSpaces(n).Contains(",")) {
            SetErrors("LEXICAL", "Invalid token ','");
            return false;
        }
        
        // Se forma un array con la entrada separa por tokens
        foreach (var item in doubleSymbols) 
            n = n.Replace(item, $" {item} ");
        foreach (var item in simpleSymbols) 
            n = n.Replace(item, $" {item} ");
        
        n = n.Insert(n.Length, " ");
        int index = n.IndexOf(" ");

        while (index != -1) {
            if (Extra.Quotes(n[(index + 1)..]) % 2 == 0) {
                tokens.Add(n[..index]);
                n = n[(index + 1)..];
                index = n.IndexOf(" ");
            }
            else index = n.IndexOf(" ", index + 1);
        }
        
        tokens.RemoveAll(string.IsNullOrWhiteSpace);

        // Se recorre el array en busca de tokens mal ubicados
        for (int i = 0; i < tokens.Count - 1; i++)
        {
            // 1. Letra o dígito después de un ')'
            if (tokens[i] == ")" && (tokens[i + 1].Any(char.IsLetterOrDigit) || 
                tokens[i + 1].Any(char.IsPunctuation))) {

                if (simpleSymbols.Contains(tokens[i + 1]) && tokens[i + 1] != "(") continue;
                if (scapes.Contains(tokens[i + 1])) continue;

                invalid = true;
                if(tokens[i + 1].Contains("\"")) tokens[i + 1] = "string";
                mssg = $"'{tokens[i + 1]}' after ')'";
                break;
            }

            // 2. Signo de puntuación después de '('
            if (i > 0 && tokens[i] == "(" && tokens[i - 1].Any(char.IsPunctuation)) {

                if (simpleSymbols.Contains(tokens[i - 1])) continue;

                invalid = true;
                if(tokens[i + 1].Contains("\"")) tokens[i - 1] = "string";
                mssg = $"'(' after '{tokens[i - 1]}'";
                break;
            }

            // 3. Palabras o números contiguos separados por espacios sin operadores
            if (tokens[i].Any(char.IsLetterOrDigit) && tokens[i + 1].Any(char.IsLetterOrDigit) ) { 
        
                if (simpleSymbols.Contains(tokens[i]) || simpleSymbols.Contains(tokens[i + 1])) continue;
                if (scapes.Contains(tokens[i + 1])) continue;
                if (scapes.Contains(tokens[i])) continue;

                invalid = true;
                mssg = $"'{tokens[i + 1]}' after '{tokens[i]}'";
                break;
            }

            // 4. Signos de puntuación contiguos
            if (tokens[i].Any(char.IsPunctuation) && tokens[i + 1].Any(char.IsPunctuation)) { 
        
                if (simpleSymbols.Contains(tokens[i]) || simpleSymbols.Contains(tokens[i + 1])) continue;

                invalid = true;
                if(tokens[i + 1].Contains("\"")) tokens[i + 1] = "string";
                if(tokens[i].Contains("\"")) tokens[i + 1] = "string";
                mssg = $"'{tokens[i + 1]}' after '{tokens[i]}'";
                break;
            }
            
            // 5. Instrucción 'function' inválida
            if (tokens[i] == "function") {
                string m = "Invalid 'function' instruction";
                SetErrors("SYNTAX", m);
                return false;
            }
        }

        // Se imprime el error si lo hay
        if (invalid) {
            SetErrors("SYNTAX", $"Invalid {mssg}");
            return false;
        }
        
        return true;
    }

    #endregion

    // Revisión semántica
    #region Semantic Checking

    // Método que revisa los tokens de las operaciones
    public static bool TokensCheck(string leftSide, string rightSide, string symbol) {
        string[] numerics = {"+", "-", "*", "/", "%", "^", ">", "<", "<=", ">="};
        string[] booleans1 = {"&", "|"};
        string[] booleans2 = {"!=", "=="};

        // Revisión del '!'
        if (symbol == "!") {

            // Miembro derecho vacío o izquierdo no vacío
            if (!string.IsNullOrWhiteSpace(leftSide) || string.IsNullOrWhiteSpace(rightSide)) {
                string message = (!string.IsNullOrWhiteSpace(leftSide))? "can not have a left" : "needs a right";
                string mssg = $"Operator 'not' {message} member";
                SetErrors("SEMANTIC", mssg);
                return false;
            }

            if (Cache.constantValues.ContainsKey(rightSide.Trim())) {
                rightSide = Cache.constantValues[rightSide.Trim()];
            }

            // Se revisa el MD y además se verifica que sea bool
            if(!SemanticCheck(rightSide)) return false;
        }

        // Si alguno de los miembros es vacío
        else if (string.IsNullOrWhiteSpace(leftSide) || string.IsNullOrWhiteSpace(rightSide)) {
            if (symbol == "&") symbol = "and";
            else if (symbol == "|") symbol = "or";
            string mssg = $"Missing expression in '{symbol}' operation";
            SetErrors("SEMANTIC", mssg);
            Console.ForegroundColor = ConsoleColor.Red;
            return false;
        }

        // Revisión de '@'
        if (symbol == "@") {

            if (Cache.constantValues.ContainsKey(leftSide.Trim())) {
                leftSide = Cache.constantValues[leftSide.Trim()];
            }

            if (Cache.constantValues.ContainsKey(rightSide.Trim())) {
                rightSide = Cache.constantValues[rightSide.Trim()];
            }

            // Se revisa que ambos miembros sean correctos y al menos uno sea string
            if (!SemanticCheck(leftSide) || !SemanticCheck(rightSide)) {
                return false;
            }

            if (Types.GetType(leftSide) != "string" && Types.GetType(rightSide) != "string") {
                string leftType  = Types.GetType(leftSide);
                string rightType = Types.GetType(rightSide);
                
                if (leftType == "undefined expression" || rightType == "undefined expression") {
                    leftSide = (leftType == "undefined expression")? leftSide : rightSide;

                    SetErrors("SYNTAX", $"'{leftSide.Trim()}' is not defined");
                }

                else SetErrors(leftType, rightType, symbol);
                
                return false;
            }
        }

        // Revisión de tokens con miembros numéricos
        else if (numerics.Contains(symbol)) {

            if (Cache.constantValues.ContainsKey(leftSide.Trim())) {
                leftSide = Cache.constantValues[leftSide.Trim()];
            }

            if (Cache.constantValues.ContainsKey(rightSide.Trim())) {
                rightSide = Cache.constantValues[rightSide.Trim()];
            }

            // Se revisan ambos miembros
            if (!SemanticCheck(leftSide) || !SemanticCheck(rightSide)) {
                return false;
            }

            // Se revisa que ambos miembros sean numéricos
            if (!Numeric.IsNumeric(leftSide) || !Numeric.IsNumeric(rightSide)) {
                string leftType  = Types.GetType(leftSide);
                string rightType = Types.GetType(rightSide);

                if (leftType == "undefined expression" || rightType == "undefined expression") {
                    leftSide = (leftType == "undefined expression")? leftSide : rightSide;

                    SetErrors("SYNTAX", $"'{leftSide.Trim()}' is not defined");
                }

                else SetErrors(leftType, rightType, symbol);

                return false;
            }

            // Se revisa el caso de la división por '0'
            if ((symbol == "/" || symbol == "%") && rightSide.Trim() == "0") {
                string mssg = "Division by '0' is not defined";
                SetErrors("SEMANTIC", mssg);
                return false;
            }
        }

        //  Revisión de tokens con miembros booleanos
        else if (booleans1.Contains(symbol)) {

            if (Cache.constantValues.ContainsKey(leftSide.Trim())) {
                leftSide = Cache.constantValues[leftSide.Trim()];
            }

            if (Cache.constantValues.ContainsKey(rightSide.Trim())) {
                rightSide = Cache.constantValues[rightSide.Trim()];
            }

            // Se revisan ambos miembros
            if (!SemanticCheck(leftSide) || !SemanticCheck(rightSide)) {
                return false;
            }
        }

        // Revisión de '==' y '!='
        else if (booleans2.Contains(symbol)) {

            if (Cache.constantValues.ContainsKey(leftSide.Trim())) {
                leftSide = Cache.constantValues[leftSide.Trim()];
            }

            if (Cache.constantValues.ContainsKey(rightSide.Trim())) {
                rightSide = Cache.constantValues[rightSide.Trim()];
            }

            // Se revisan ambos miembros
            if(!SemanticCheck(rightSide) || !SemanticCheck(leftSide)) {
                return false; 
            }

            string leftType = Types.GetType(leftSide);
            string rightType = Types.GetType(rightSide);

            // Se revisa que sean del mismo tipo
            if (leftType != rightType) {
                if (leftType == "undefined expression" || rightType == "undefined expression") {
                    leftSide = (leftType == "undefined expression")? leftSide : rightSide;

                    SetErrors("SYNTAX", $"'{leftSide.Trim()}' is not defined");
                }

                else SetErrors(leftType, rightType, symbol);

                return false;
            }     
        }

        return true;
    }

    // Método que revisa la semántica general
    public static bool SemanticCheck(string s, string name = "", List<string> variables = null!) {
        
        // Revisión de caracteres de escape en strings
        if(!Extra.SlashRevision(s)) return false;

        // Se normaliza la expresión quitando los strings y espacios innceserarios
    
        s = $" {String.StringsToSpaces(s)} ";
        string m =  Regex.Replace(s, @"[^_""ñÑA-Za-z0-9]", " ");
        
        variables ??= new();
        if (vars.Count == 0) vars.AddRange(variables);
        if (funcName == "") funcName = name;

        // Si es condicional se obtienen sus datos y se revisan
        if (Conditional.IsConditional(s)) {
            (bool, string, string, string, int, int, string) conditionalData = Conditional.GetData(s, vars);
            if (!conditionalData.Item1) return false;
            if (vars.Contains(conditionalData.Item2)) {
                Cache.inputType[funcName + "("][vars.IndexOf(conditionalData.Item2)] = "boolean";
            }

            string body_true = conditionalData.Item3;
            string body_false = conditionalData.Item4;
            int index_1 = conditionalData.Item5;
            int stop = conditionalData.Item6;
            s = conditionalData.Item7;
            string body1 = s;
            string body2 = s;

            body1 = body1.Remove(index_1 + 1, stop - index_1 - 1);
            body1 = body1.Insert(index_1 + 1, $"{body_true}");
            body2 = body2.Remove(index_1 + 1, stop - index_1 - 1);
            body2 = body2.Insert(index_1 + 1, $"{body_false}");

            // Se revisa la expresión sustituyendo ambos cuerpos de la condicional
            bool check = SemanticCheck(body1) && SemanticCheck(body2);

            // En el caso de las funciones, las condicionales deben devolver el mismo tipo
            if (!check) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("!! Conditional may return an invalid type !!");
            }

            return check;
        } 
        
        // Se definen los valores por defecto de cada tipo
        Dictionary<string, string> defaultValues = new() {
            ["string"] = "\" \"", ["number"] = "2", ["boolean"] = "0", 
            ["undefined expression"] = "", ["all"] = "~", [""] = ""
        };

        // Se analizan paréntesis
        while (s.Contains('(')) {
            
            int index1 = s.LastIndexOf("(");
            int index2 = s.IndexOf(")", index1);
            string parenthesis = s[(index1 + 1)..index2];

            if (parenthesis.Contains(",") || SemanticCheck(parenthesis, funcName, vars)) {
                
                (bool, string, string[]) data = Function.GetData(s, index1, index2);

                string f = data.Item2;
                string[] args = data.Item3;

                // Si es función se analiza:
                if (data.Item1) {
                    
                    // 1. Que la función exista
                    if (!Cache.inputType.ContainsKey(f)) {
                        string mssg = $"'{f[..^1]}' is not a valid function";
                        if (double.TryParse(f[..^1], out _)) mssg += " name";
                        SetErrors("SEMANTIC", mssg);
                        return false;
                    }

                    // 2. Que tenga la misma cantidad de argumentos
                    if (!NumberOfArgs(Cache.inputType[f], args.ToList(), f)) return false;

                    /* En caso de funciones recursivas, en el cuerpo se eliminan los paréntesis
                    y los argumentos, dejando solo el nombre, para que lo tome como una variable más
                    a la hora de revisar */
                    if (funcName + "(" == f) {
                        /* Como contramedida se añade al array de variables el nombre de la función
                        para que sea admitido */
                        s = s.Remove(index1, parenthesis.Length + 2);
                        Array.Resize(ref args, args.Length + 1);
                        args[^1] = funcName;
                        vars.Add(funcName);
                        Cache.inputType[funcName + "("].Add("all");
                    }
                    
                    for (int i = 0; i < args.Length; i++) {
                        
                        if (args[i] == "") break;

                        // Se chequea cada argumento
                        if (!SemanticCheck(args[i], funcName, vars)) return false;

                        // Si es 'print' simplemente se deja en la expresión global su argumento
                        if (f == "print(") {
                            s = s.Remove(index2 , 1);
                            s = s.Remove(index1 - 5, 6);
                            break;
                        }

                        // Se intercambian las variables de los argumentos por su valor por defecto
                        if (vars.Contains(args[i].Trim()) && Cache.inputType[f][i] != "all") {
                            int pos = vars.IndexOf(args[i].Trim());
                            Cache.inputType[funcName + "("][pos] = Cache.inputType[f][i];
                            args[i] = defaultValues[Cache.inputType[f][i]];
                        }

                        /* Si la función en la posición del argumento en cuestión recibe cualquier
                        tipo, pues se sustituye la variable por un '2' sin perder generalidad */
                        else if (Cache.inputType[f][i] == "all") {
                            args[i] = "2";
                        } 
                    }

                    if (!Cache.defaultFunctions.Contains(f)) Cache.newFunctions[f] = "";   

                    // Se vuelve a restablecer las variables en las funciones recursivas
                    if (funcName + "(" == f) {
                        Array.Resize(ref args, args.Length - 1);
                        Cache.inputType[funcName + "("].RemoveAt(Cache.inputType[funcName + "("].Count - 1); 
                    }

                    // 3. Se analiza cada uno de los argumentos
                    if (CheckingArgs(f, parenthesis, args)) {
                        if (!Cache.defaultFunctions.Contains(f)) Cache.newFunctions.Remove(f);

                        /* Si la función es correcta se intercambia en la expresión global
                        por el valor por defecto de su tipo de retorno */
                        if (f != funcName + "(" && f != "print(") {
                            int start = index1  + 1 - f.Length;
                            s = s.Remove(start, f.Length + parenthesis.Length + 1);

                            if (Cache.defaultFunctions.Contains(f)) {
                                s = s.Insert(start, defaultValues[Cache.returnType[f]]);
                            }

                            else s = s.Insert(start, funcName);
                        }
                    }

                    else {
                        return false;
                    }
                }

                /* Si el paréntesis no es de una función entonces se intercambia por
                el valor por defecto del tipo de su expresión interna */
                else {
                    string val = defaultValues[Types.GetType(parenthesis)];
                    if (val == "") val = parenthesis;
                    s = s[..index1] + val + s[(index2 + 1)..];
                }
            }

            else return false;
        }

        //Se analizan los operadores
        s = Extra.SpacesBeforeParenthesis(s);
        
        if (s.Contains('@')) {
            int index = s.LastIndexOf('@');
            string left = Extra.SpacesBeforeParenthesis(s[..index]);
            string right = Extra.SpacesBeforeParenthesis(s[(index + 1)..]);
            left = (left == "~")? "\" \"" : left;
            right = (right == "~")? "\" \"" : right;

            /* (solo para instrucciones 'function'). Si el MI es una variable definida en la función
            se analiza */
            if (vars.Contains(left)) {
                
                // Si es la propia función se añade su valor de retorno según el requerido por el operador
                if (left == funcName) { 
                    if (Cache.returnType[funcName + "("] == "all") {
                        Cache.returnType[funcName + "("] = "string";
                    }

                    else if (Cache.returnType[funcName + "("] != "string") {
                        string type = Cache.returnType[funcName + "("];
                        string mssg = $"'{left}' can not return 'string' and '{type}'";
                        SetErrors("SEMANTIC", mssg);
                        return false;
                    }

                    left = defaultValues["string"]; 
                }

                /* Si la variable puede ser de cualquier tipo, entonces se especifica, según 
                el operador, que a partir de ahora tomará cierto tipo. Y luego se sustituye
                por el valor por defecto del tipo */
                else if (Cache.inputType[funcName + "("][vars.IndexOf(left)] == "all") {
                    left = defaultValues["string"]; 
                }

                else if (Cache.inputType[funcName + "("][vars.IndexOf(left)] == "string") {
                    left = defaultValues["string"];    
                }

                else {
                    string type = Cache.inputType[funcName + "("][vars.IndexOf(left)];
                    string mssg = $"Variable '{left}' can not be 'string' and '{type}'";
                    SetErrors("SEMANTIC", mssg);
                    return false;
                }      
            }
            
            // Explicación análoga para el MD (Sirve para la gran mayoría de operadores)
            if (vars.Contains(right)) {

                if (right == funcName) { 
                    if (Cache.returnType[funcName + "("] == "all") {
                        Cache.returnType[funcName + "("] = "string";
                    }

                    else if (Cache.returnType[funcName + "("] != "string") {
                        string type = Cache.returnType[funcName + "("];
                        string mssg = $"'{right}' can not return 'string' and '{type}'";
                        SetErrors("SEMANTIC", mssg);
                        return false;
                    }

                    right = defaultValues["string"]; 
                }

                else if (Cache.inputType[funcName + "("][vars.IndexOf(right)] == "all") {
                    right = defaultValues["string"]; 
                }

                else if (Cache.inputType[funcName + "("][vars.IndexOf(right)] == "string") {
                    right = defaultValues["string"];    
                }

                else {
                    string type = Cache.inputType[funcName + "("][vars.IndexOf(right)];
                    string mssg = $"Varibale '{right}' can not be 'string' and '{type}'";
                    SetErrors("SEMANTIC", mssg);
                    return false;
                }
            }

            if (left.Contains("==") || left.Contains("!=")) (left, right) = (right, left);

            return TokensCheck(left, right, "@");

        }

        // Explicación análoga a la anterior
        if (m.Contains(" and ") || s.Contains(" or ")) {
            int index = Math.Max(m.LastIndexOf(" and "), m.LastIndexOf(" or "));
            char operation = (s[(index + 1)..(index + 4)] == "and")? '&' : '|';
            string left = Extra.SpacesBeforeParenthesis(s[..(index + 1)]);
            string right = Extra.SpacesBeforeParenthesis(s[(index + 4)..]);
            left = (left == "~")? "0" : left;
            right = (right == "~")? "0" : right;

            if (vars.Contains(left)) {

                // if (left == funcName) { 
                //     if (Cache.returnType[funcName + "("] == "all") {
                //         Cache.returnType[funcName + "("] = "number";
                //     }

                //     else if (Cache.returnType[funcName + "("] != "boolean") {
                //         string type = Cache.returnType[funcName + "("];
                //         string mssg = $"'{left}' can not return 'boolean' and '{type}'";
                //         SetErrors("SEMANTIC", mssg);
                //         return false;
                //     }

                //     left = defaultValues["boolean"]; 
                // }

                // else if (Cache.InputType[funcName + "("][vars.IndexOf(left)] == "all") {

                //     Cache.InputType[funcName + "("][vars.IndexOf(left)] = "boolean";
                //     left = defaultValues["boolean"]; 
                // }

                // else if (Cache.InputType[funcName + "("][vars.IndexOf(left)] == "boolean") {
                //     left = defaultValues["boolean"];    
                // }

                // else {
                //     string type = Cache.InputType[funcName + "("][vars.IndexOf(left)];
                //     string mssg = $"Variable '{left}' can not be 'boolean' and '{type}'";
                //     SetErrors("SEMANTIC", mssg);
                //     return false;
                // }      
            }

            if (vars.Contains(right)) {

                // if (right == funcName) { 
                //     if (Cache.returnType[funcName + "("] == "all") {
                //         Cache.returnType[funcName + "("] = "boolean";
                //     }

                //     else if (Cache.returnType[funcName + "("] != "boolean") {
                //         string type = Cache.returnType[funcName + "("];
                //         string mssg = $"'{right}' can not return 'boolean' and '{type}'";
                //         SetErrors("SEMANTIC", mssg);
                //         return false;
                //     }

                //     right = defaultValues["boolean"]; 
                // }

                // else if (Cache.InputType[funcName + "("][vars.IndexOf(right)] == "all") {

                //     Cache.InputType[funcName + "("][vars.IndexOf(right)] = "boolean";
                //     right = defaultValues["boolean"]; 
                // }

                // else if (Cache.InputType[funcName + "("][vars.IndexOf(right)] == "boolean") {
                //     right = defaultValues["boolean"];    
                // }

                // else {
                //     string type = Cache.InputType[funcName + "("][vars.IndexOf(right)];
                //     string mssg = $"Variable '{right}' can not be 'boolean' and '{type}'";
                //     SetErrors("SEMANTIC", mssg);
                //     return false;
                // }
            }

            if (left.Contains("==") || left.Contains("!=")) (left, right) = (right, left);

            return TokensCheck(left, right, operation.ToString());
        }

        if (s.Contains("==") || s.Contains("!=")) {
            int index = Math.Max(s.LastIndexOf("=="), s.LastIndexOf("!="));
            string operation = s.Substring(index, 2);
            string left = Extra.SpacesBeforeParenthesis(s[..index]);
            string right = Extra.SpacesBeforeParenthesis(s[(index + 2)..]);
            
            string typeR = vars.Contains(right)? 
                Cache.inputType[funcName + "("][vars.IndexOf(right)] : Types.GetType(right);

            string typeL = vars.Contains(left)? 
                Cache.inputType[funcName + "("][vars.IndexOf(left)] : Types.GetType(left);

            /* En este caso las variables tomarán el tipo del miembro opuesto. En caso de ser ambos
            variables entonces si son de tipos definidos pero distintos dará error en la revisión, y
            en caso de que una esté 'abierta' a cualquier tipo, pues se restringirá según la otra */
            if (vars.Contains(left)) {
                if (left == funcName) { 
                    if (Cache.returnType[funcName + "("] == "all") {
                        Cache.returnType[funcName + "("] = typeR;
                        typeL = typeR;
                    }

                    else if (Cache.returnType[funcName + "("] != typeR) {
                        string type = Cache.returnType[funcName + "("];
                        string mssg = $"'{left}' can not return '{typeR}' and '{type}'";
                        SetErrors("SEMANTIC", mssg);
                        return false;
                    }

                    left = defaultValues[typeR]; 
                }

                if (Cache.inputType[funcName + "("][vars.IndexOf(left)] == "all") {

                    Cache.inputType[funcName + "("][vars.IndexOf(left)] = typeR;
                    typeL = typeR;
                }

                else if (Cache.inputType[funcName + "("][vars.IndexOf(left)] != typeR) {
                    string type = Cache.inputType[funcName + "("][vars.IndexOf(left)];
                    string mssg = $"Variable '{left}' can not be '{typeR}' and '{type}'";
                    SetErrors("SEMANTIC", mssg);
                    return false;    
                }

                left = defaultValues[typeR];   
            }

            if (vars.Contains(right)) {
                if (right == funcName) { 
                    if (Cache.returnType[funcName + "("] == "all") {
                        Cache.returnType[funcName + "("] = typeL;
                        typeR = typeL;
                    }

                    else if (Cache.returnType[funcName + "("] != typeL) {
                        string type = Cache.returnType[funcName + "("];
                        string mssg = $"'{right}' can not return '{typeL}' and '{type}'";
                        SetErrors("SEMANTIC", mssg);
                        return false;
                    }

                    right = defaultValues[typeL]; 
                }

                if (Cache.inputType[funcName + "("][vars.IndexOf(right)] == "all") {

                    Cache.inputType[funcName + "("][vars.IndexOf(right)] = typeL;
                    typeR = typeL;
                }

                else if (Cache.inputType[funcName + "("][vars.IndexOf(right)] != typeL) {
                    string type = Cache.inputType[funcName + "("][vars.IndexOf(right)];
                    string mssg = $"Variable '{right}' can not be '{typeL}' and '{type}'";
                    SetErrors("SEMANTIC", mssg);
                    return false;    
                }

                right = defaultValues[typeL];   
            }

            if (left == "~") return true;

            return TokensCheck(left, right, operation);
        }

        // Explicación igual a la de '@'
        if (s.Contains("<=") || s.Contains(">=")) {
            int index = Math.Max(s.LastIndexOf("<="), s.LastIndexOf(">="));
            string operation = s.Substring(index, 2);
            string left = Extra.SpacesBeforeParenthesis(s[..index]);
            string right = Extra.SpacesBeforeParenthesis(s[(index + 2)..]);
            left = (left == "~")? "2" : left;
            right = (right == "~")? "2" : right;


            if (vars.Contains(left)) {

                if (left == funcName) { 
                    if (Cache.returnType[funcName + "("] == "all") {
                        Cache.returnType[funcName + "("] = "number";
                    }

                    else if (Cache.returnType[funcName + "("] != "number") {
                        string type = Cache.returnType[funcName + "("];
                        string mssg = $"'{left}' can not return 'number' and '{type}'";
                        SetErrors("SEMANTIC", mssg);
                        return false;
                    }

                    left = defaultValues["number"]; 
                }

                else if (Cache.inputType[funcName + "("][vars.IndexOf(left)] == "all") {

                    Cache.inputType[funcName + "("][vars.IndexOf(left)] = "number";
                    left = defaultValues["number"]; 
                }

                else if (Cache.inputType[funcName + "("][vars.IndexOf(left)] == "number") {
                    left = defaultValues["number"];    
                }

                else {
                    string type = Cache.inputType[funcName + "("][vars.IndexOf(left)];
                    string mssg = $"Variable '{left}' can not be 'number' and '{type}'";
                    SetErrors("SEMANTIC", mssg);
                    return false;
                }      
            }

            if (vars.Contains(right)) {

                if (right == funcName) { 
                    if (Cache.returnType[funcName + "("] == "all") {
                        Cache.returnType[funcName + "("] = "number";
                    }

                    else if (Cache.returnType[funcName + "("] != "number") {
                        string type = Cache.returnType[funcName + "("];
                        string mssg = $"'{right}' can not return 'number' and '{type}'";
                        SetErrors("SEMANTIC", mssg);
                        return false;
                    }

                    right = defaultValues["number"]; 
                }

                else if (Cache.inputType[funcName + "("][vars.IndexOf(right)] == "all") {

                    Cache.inputType[funcName + "("][vars.IndexOf(right)] = "number";
                    right = defaultValues["number"]; 
                }

                else if (Cache.inputType[funcName + "("][vars.IndexOf(right)] == "number") {
                    right = defaultValues["number"];    
                }

                else {
                    string type = Cache.inputType[funcName + "("][vars.IndexOf(right)];
                    string mssg = $"Variable '{right}' can not be 'number' and '{type}'";
                    SetErrors("SEMANTIC", mssg);
                    return false;
                }
            }

            return TokensCheck(left, right, operation);
        }

        //Explicación igual a la de '@'
        if (s.Contains('<') || s.Contains('>')) {
            int index = Math.Max(s.LastIndexOf("<"), s.LastIndexOf(">"));
            char operation = s[index];
            string left = Extra.SpacesBeforeParenthesis(s[..index]);
            string right = Extra.SpacesBeforeParenthesis(s[(index + 1)..]);
            left = (left == "~")? "2" : left;
            right = (right == "~")? "2" : right;

             if (vars.Contains(left)) {

                if (left == funcName) { 
                    if (Cache.returnType[funcName + "("] == "all") {
                        Cache.returnType[funcName + "("] = "number";
                    }

                    else if (Cache.returnType[funcName + "("] != "number") {
                        string type = Cache.returnType[funcName + "("];
                        string mssg = $"'{left}' can not return 'number' and '{type}'";
                        SetErrors("SEMANTIC", mssg);
                        return false;
                    }

                    left = defaultValues["number"]; 
                }

                else if (Cache.inputType[funcName + "("][vars.IndexOf(left)] == "all") {

                    Cache.inputType[funcName + "("][vars.IndexOf(left)] = "number";
                    left = defaultValues["number"]; 
                }

                else if (Cache.inputType[funcName + "("][vars.IndexOf(left)] == "number") {
                    left = defaultValues["number"];    
                }

                else {
                    string type = Cache.inputType[funcName + "("][vars.IndexOf(left)];
                    string mssg = $"Variable '{left}' can not be 'number' and '{type}'";
                    SetErrors("SEMANTIC", mssg);
                    return false;
                }      
            }

            if (vars.Contains(right)) {

                if (right == funcName) { 
                    if (Cache.returnType[funcName + "("] == "all") {
                        Cache.returnType[funcName + "("] = "number";
                    }

                    else if (Cache.returnType[funcName + "("] != "number") {
                        string type = Cache.returnType[funcName + "("];
                        string mssg = $"'{right}' can not return 'number' and '{type}'";
                        SetErrors("SEMANTIC", mssg);
                        return false;
                    }

                    right = defaultValues["number"]; 
                }

                else if (Cache.inputType[funcName + "("][vars.IndexOf(right)] == "all") {

                    Cache.inputType[funcName + "("][vars.IndexOf(right)] = "number";
                    right = defaultValues["number"]; 
                }

                else if (Cache.inputType[funcName + "("][vars.IndexOf(right)] == "number") {
                    right = defaultValues["number"];    
                }

                else {
                    string type = Cache.inputType[funcName + "("][vars.IndexOf(right)];
                    string mssg = $"Variable '{right}' can not be 'number' and '{type}'";
                    SetErrors("SEMANTIC", mssg);
                    return false;
                }
            }

            return TokensCheck(left, right, operation.ToString());
        }

        if (m.Contains(" not ")) {
            int index = m.LastIndexOf(" not ");
            string left = Extra.SpacesBeforeParenthesis(s[..(index + 1)]);
            string right = Extra.SpacesBeforeParenthesis(s[(index + 4)..]);
            left = (left == "~")? "0" : left;
            right = (right == "~")? "0" : right;

            // Misma explicación, pero solo para un miembro
            // if (vars.Contains(right)) {

            //     if (right == funcName) { 
            //         if (Cache.returnType[funcName + "("] == "all") {
            //             Cache.returnType[funcName + "("] = "boolean";
            //         }

            //         else if (Cache.returnType[funcName + "("] != "boolean") {
            //             string type = Cache.returnType[funcName + "("];
            //             string mssg = $"'{right}' can not return 'boolean' and '{type}'";
            //             SetErrors("SEMANTIC", mssg);
            //             return false;
            //         }

            //         right = defaultValues["boolean"]; 
            //     }

            //     else if (Cache.InputType[funcName + "("][vars.IndexOf(right)] == "all") {

            //         Cache.InputType[funcName + "("][vars.IndexOf(right)] = "boolean";
            //         right = defaultValues["boolean"]; 
            //     }

            //     else if (Cache.InputType[funcName + "("][vars.IndexOf(right)] == "boolean") {
            //         right = defaultValues["boolean"];    
            //     }

            //     else {
            //         string type = Cache.InputType[funcName + "("][vars.IndexOf(right)];
            //         string mssg = $"Variable '{right}' can not be 'boolean' and '{type}'";
            //         SetErrors("SEMANTIC", mssg);
            //         return false;
            //     }
            // }

            return TokensCheck(left, right, "!"); 
        }

        s = Extra.SpacesBetweenTokens(s);

        if (s.Contains('+') || s.Contains('-')) {

            // Se eliminan signos dobles innecesarios por sus equivalentes
            while (s.Contains("+-") || s.Contains("-+") || s.Contains("++") || s.Contains("--")) {
                s = s.Replace("+-","-");
                s = s.Replace("-+","-");
                s = s.Replace("++","+");
                s = s.Replace("--","+");
            }  

            int index = Math.Max(s.LastIndexOf("+"), s.LastIndexOf("-"));
            char operation = s[index];
            string[] binaries = {"+", "-", "*", "/", "%", "^"};

            // Se actualiza el índice en caso de que no haya que tomar el signo
            while (index > 1 && s[index - 1] == 'E' && char.IsDigit(s[index - 2]) ||
                  (index > 0 && binaries.Contains(s[index - 1].ToString()))) { // *- /-

                index = Math.Max(s[..index].LastIndexOf('+'), s[..index].LastIndexOf('-'));
            }

            if (index > 0) {
                string left = Extra.SpacesBeforeParenthesis(s[..index]);
                string right = Extra.SpacesBeforeParenthesis(s[(index + 1)..]);
                left = (left == "~")? "2" : left;
                right = (right == "~")? "2" : right;

                // Misma explicación
                if (vars.Contains(left)) {

                    if (left == funcName) { 
                        if (Cache.returnType[funcName + "("] == "all") {
                            Cache.returnType[funcName + "("] = "number";
                        }

                        else if (Cache.returnType[funcName + "("] != "number") {
                            string type = Cache.returnType[funcName + "("];
                            string mssg = $"'{left}' can not return 'number' and '{type}'";
                            SetErrors("SEMANTIC", mssg);
                            return false;
                        }

                        left = defaultValues["number"]; 
                    }

                    else if (Cache.inputType[funcName + "("][vars.IndexOf(left)] == "all") {

                        Cache.inputType[funcName + "("][vars.IndexOf(left)] = "number";
                        left = defaultValues["number"]; 
                    }

                    else if (Cache.inputType[funcName + "("][vars.IndexOf(left)] == "number") {
                        left = defaultValues["number"];    
                    }

                    else {
                        string type = Cache.inputType[funcName + "("][vars.IndexOf(left)];
                        string mssg = $"Variable '{left}' can not be 'number' and '{type}'";
                        SetErrors("SEMANTIC", mssg);
                        return false;
                    }      
                }

                if (vars.Contains(right)) {

                    if (right == funcName) { 
                        if (Cache.returnType[funcName + "("] == "all") {
                            Cache.returnType[funcName + "("] = "number";
                        }

                        else if (Cache.returnType[funcName + "("] != "number") {
                            string type = Cache.returnType[funcName + "("];
                            string mssg = $"'{right}' can not return 'number' and '{type}'";
                            SetErrors("SEMANTIC", mssg);
                            return false;
                        }

                        right = defaultValues["number"]; 
                    }

                    else if (Cache.inputType[funcName + "("][vars.IndexOf(right)] == "all") {

                        Cache.inputType[funcName + "("][vars.IndexOf(right)] = "number";
                        right = defaultValues["number"]; 
                    }

                    else if (Cache.inputType[funcName + "("][vars.IndexOf(right)] == "number") {
                        right = defaultValues["number"];    
                    }

                    else {
                        string type = Cache.inputType[funcName + "("][vars.IndexOf(right)];
                        string mssg = $"Variable '{right}' can not be 'number' and '{type}'";
                        SetErrors("SEMANTIC", mssg);
                        return false;
                    }
                }

                return TokensCheck(left, right, operation.ToString());
            }
        }

        // Misma explicación
        if (s.Contains('*') || s.Contains('/') || s.Contains('%')) {
            int index = Math.Max(s.LastIndexOf("*"), Math.Max(s.LastIndexOf("/"), s.LastIndexOf("%")));
            char operation = s[index];
            string left = Extra.SpacesBeforeParenthesis(s[..index]);
            string right = Extra.SpacesBeforeParenthesis(s[(index + 1)..]);
            left = (left == "~")? "2" : left;
            right = (right == "~")? "2" : right;

             if (vars.Contains(left)) {

                if (left == funcName) { 
                    if (Cache.returnType[funcName + "("] == "all") {
                        Cache.returnType[funcName + "("] = "number";
                    }

                    else if (Cache.returnType[funcName + "("] != "number") {
                        string type = Cache.returnType[funcName + "("];
                        string mssg = $"'{left}' can not return 'number' and '{type}'";
                        SetErrors("SEMANTIC", mssg);
                        return false;
                    }

                    left = defaultValues["number"]; 
                }

                else if (Cache.inputType[funcName + "("][vars.IndexOf(left)] == "all") {

                    Cache.inputType[funcName + "("][vars.IndexOf(left)] = "number";
                    left = defaultValues["number"]; 
                }

                else if (Cache.inputType[funcName + "("][vars.IndexOf(left)] == "number") {
                    left = defaultValues["number"];    
                }

                else {
                    string type = Cache.inputType[funcName + "("][vars.IndexOf(left)];
                    string mssg = $"Variable '{left}' can not be 'number' and '{type}'";
                    SetErrors("SEMANTIC", mssg);
                    return false;
                }      
            }

            if (vars.Contains(right)) {

                if (right == funcName) { 
                    if (Cache.returnType[funcName + "("] == "all") {
                        Cache.returnType[funcName + "("] = "number";
                    }

                    else if (Cache.returnType[funcName + "("] != "number") {
                        string type = Cache.returnType[funcName + "("];
                        string mssg = $"'{right}' can not return 'number' and '{type}'";
                        SetErrors("SEMANTIC", mssg);
                        return false;
                    }

                    right = defaultValues["number"]; 
                }

                else if (Cache.inputType[funcName + "("][vars.IndexOf(right)] == "all") {

                    Cache.inputType[funcName + "("][vars.IndexOf(right)] = "number";
                    right = defaultValues["number"]; 
                }

                else if (Cache.inputType[funcName + "("][vars.IndexOf(right)] == "number") {
                    right = defaultValues["number"];    
                }

                else {
                    string type = Cache.inputType[funcName + "("][vars.IndexOf(right)];
                    string mssg = $"Variable '{right}' can not be 'number' and '{type}'";
                    SetErrors("SEMANTIC", mssg);
                    return false;
                }
            }

            return TokensCheck(left, right, operation.ToString());
        }

        // Misma explicación
        if (s.Contains('^')) {
            int index = s.LastIndexOf('^');
            string left = Extra.SpacesBeforeParenthesis(s[..index]);
            string right = Extra.SpacesBeforeParenthesis(s[(index + 1)..]);
            left = (left == "~")? "2" : left;
            right = (right == "~")? "2" : right;

             if (vars.Contains(left)) {

                if (left == funcName) { 
                    if (Cache.returnType[funcName + "("] == "all") {
                        Cache.returnType[funcName + "("] = "number";
                    }

                    else if (Cache.returnType[funcName + "("] != "number") {
                        string type = Cache.returnType[funcName + "("];
                        string mssg = $"'{left}' can not return 'number' and '{type}'";
                        SetErrors("SEMANTIC", mssg);
                        return false;
                    }

                    left = defaultValues["number"]; 
                }

                else if (Cache.inputType[funcName + "("][vars.IndexOf(left)] == "all") {

                    Cache.inputType[funcName + "("][vars.IndexOf(left)] = "number";
                    left = defaultValues["number"]; 
                }

                else if (Cache.inputType[funcName + "("][vars.IndexOf(left)] == "number") {
                    left = defaultValues["number"];    
                }

                else {
                    string type = Cache.inputType[funcName + "("][vars.IndexOf(left)];
                    string mssg = $"Variable '{left}' can not be 'number' and '{type}'";
                    SetErrors("SEMANTIC", mssg);
                    return false;
                }      
            }

            if (vars.Contains(right)) {

                if (right == funcName) { 
                    if (Cache.returnType[funcName + "("] == "all") {
                        Cache.returnType[funcName + "("] = "number";
                    }

                    else if (Cache.returnType[funcName + "("] != "number") {
                        string type = Cache.returnType[funcName + "("];
                        string mssg = $"'{right}' can not return 'number' and '{type}'";
                        SetErrors("SEMANTIC", mssg);
                        return false;
                    }

                    right = defaultValues["number"]; 
                }

                else if (Cache.inputType[funcName + "("][vars.IndexOf(right)] == "all") {

                    Cache.inputType[funcName + "("][vars.IndexOf(right)] = "number";
                    right = defaultValues["number"]; 
                }

                else if (Cache.inputType[funcName + "("][vars.IndexOf(right)] == "number") {
                    right = defaultValues["number"];    
                }

                else {
                    string type = Cache.inputType[funcName + "("][vars.IndexOf(right)];
                    string mssg = $"Variable '{right}' can not be 'number' and '{type}'";
                    SetErrors("SEMANTIC", mssg);
                    return false;
                }
            }

            return TokensCheck(left, right, "^");
        }
        
        return true;
    }
    
    #endregion

    // Revisión de paréntesis balanceados
    #region Parenthesis Checking

    // Retorna 1 si falta '(', -1 si falta ')', y 0 si están balanceados
    public static int ParenthesisRevision(string s) {
        s = String.StringsToSpaces(s);

        static int ParenthesisRevision(string s, int i, int count = 0) {

            if (s[i] == '(') count++;
            else if (s[i] == ')') count--;
            
            if (count < 0) return 1;
            if (s.Length - 1 == i) return (count == 0)? 0 : -1;

            return ParenthesisRevision(s, i + 1, count);
        }

        if (!s.Contains('(') && !s.Contains(')')) return 0;
        return ParenthesisRevision(s, 0);
    }
    
    #endregion

    // Revisión de funciones
    #region Function Checking

    // Método que hace la revisión general de la función
    public static bool FunctionRevision(string s) {

        string n = String.StringsToSpaces(s);

        // Se revisa que contenga al menos un '('
        // if (!n.Contains("(")) {
        //     Console.ForegroundColor = ConsoleColor.Red;
        //     SetErrors("SYNTAX", "'(' was expected after the 'function name'");
        //     return false;
        // }

        string funcName = Extra.SpacesBeforeParenthesis(s[..n.IndexOf("(")]);

        // Se revisa que la función tenga nombre
        if (funcName == "") {
            Console.ForegroundColor = ConsoleColor.Red;
            SetErrors("SYNTAX", "Missing name before '('");
            return false;
        }

        s = Extra.SpacesBeforeParenthesis(s[(n.IndexOf("(") + 1)..]);
        n = String.StringsToSpaces(s);

        string argument = s[..n.IndexOf(")")];
        n = n[..n.IndexOf(")")];
        string body = Extra.SpacesBeforeParenthesis(s[(argument.Length + 1)..]);

        // Se revisa que no haya paréntesis inválidos en los argumentos
        if (n.Contains("(")) {
            Console.ForegroundColor = ConsoleColor.Red;
            SetErrors("SYNTAX", $"Invalid parenthesis in the argument of the function");
            return false;
        }
        
        // Se revisa que el cuerpo comience con '=>'
        if (!body.StartsWith("=")) {
            Console.ForegroundColor = ConsoleColor.Red;
            SetErrors("SYNTAX", $"Token '=' was expected after '{funcName}({argument})'");
            return false;
        }

        // Se obtienen las variables del argumento de la función
        List<string> vars = new();
        string temp = argument;
        int index = n.IndexOf(",");

        while(index != -1) {
            vars.Add(temp[..index]);
            n = n[(index + 1)..];
            temp = temp[(index + 1)..];
            index = n.IndexOf(",");
        }

        vars.Add(temp);

        // Se inicializa en 'all' los tipos de entrada de la función
        if (!Cache.inputType.ContainsKey(funcName + "(")) {
            Cache.inputType[funcName + "("] = new();
            Cache.returnType[funcName + "("] = "all";

            for (int i = 0; i < vars.Count; i++) {
                if (string.IsNullOrWhiteSpace(argument)) {
                    Cache.inputType[funcName + "("].Add("");
                    break;
                }

                Cache.inputType[funcName + "("].Add("all");
            }
        } 

        for (int i = 0; i < vars.Count; i++) vars[i] = vars[i].Trim();

        // Se revisa que el nombre sea correcto
        if (!VariableRevision(funcName, true)) {
            if(!Cache.defaultFunctions.Contains(funcName + "(")) {
                Cache.inputType.Remove(funcName + "(");
                Cache.returnType.Remove(funcName + "(");
            }
            
            return false;
        }

        // Se revisa que una variable no sea utilizada más de una vez como argumento
        if (vars.Distinct().Count() != vars.Count) {
            SetErrors("SEMANTIC", "One argument is used more than once");
            if(!Cache.defaultFunctions.Contains(funcName + "(")) {
                Cache.inputType.Remove(funcName + "(");
                Cache.returnType.Remove(funcName + "(");
            }

            return false;
        }
        
        // Se revisa que ninguna variable se llame igual que la función
        if (vars.Contains(funcName)) {
            SetErrors("SEMANTIC", $"'{funcName}' is the function name. It can not be a variable");
            if(!Cache.defaultFunctions.Contains(funcName + "(")) {
                Cache.inputType.Remove(funcName + "(");
                Cache.returnType.Remove(funcName + "(");
            }

            return false;
        }

        // Se revisa que no hayan argumentos vacíos inválidos
        if (argument != "" && vars.Any(string.IsNullOrWhiteSpace)) {
            SetErrors("SEMANTIC", $"One argument is missing in '{funcName}' function");
            if(!Cache.defaultFunctions.Contains(funcName + "(")) {
                Cache.inputType.Remove(funcName + "(");
                Cache.returnType.Remove(funcName + "(");
            }

            return false;
        }

        // Se revisa cada variable
        if (argument != "" && !vars.All(x => VariableRevision(x))) {
            if(!Cache.defaultFunctions.Contains(funcName + "(")) {
                Cache.inputType.Remove(funcName + "(");
                Cache.returnType.Remove(funcName + "(");
            }

            return false;
        }
        
        // Se revisa que el cuerpo no sea vacío
        if (body == "=") {
            SetErrors("SYNTAX", $"Missing expression after '=' in '{funcName}' declaration");
            if(!Cache.defaultFunctions.Contains(funcName + "(")) {
                Cache.inputType.Remove(funcName + "(");
                Cache.returnType.Remove(funcName + "(");
            }

            return false;
        }

        // Si el cuerpo es un 'let-in' se resuelve internamente
        while (Let_in.IsLet_in(body[1..])) {
            funcVars = vars;
            Cache.keyWords.AddRange(vars);
            body = $"={Let_in.Eval(body[1..], true)}";
            Cache.keyWords.RemoveRange(Cache.keyWords.Count - vars.Count, vars.Count);
            funcVars = new();

            if (body == "=") {
                if(!Cache.defaultFunctions.Contains(funcName + "(")) {
                    Cache.inputType.Remove(funcName + "(");
                    Cache.returnType.Remove(funcName + "(");
                }

                return false;
            }
        }

        // Se revisa la sintaxis del cuerpo
        if (!BodyRevision(body[1..], vars, funcName)) {
            if(!Cache.defaultFunctions.Contains(funcName + "(")) {
                if (Cache.recursionFunc.Contains(funcName + "(")) {
                    Cache.recursionFunc.Remove(funcName + "(");
                }

                Cache.inputType.Remove(funcName + "(");
                Cache.returnType.Remove(funcName + "(");
            }

            return false;
        }

        body = String.StringsToSpaces(body);
        Check.vars = new();
        Check.funcName = "";

        // Se revisa la semántica del cuerpo
        if (!SemanticCheck(body[1..], funcName, vars)) {
            if(!Cache.defaultFunctions.Contains(funcName + "(")) {
                if (Cache.recursionFunc.Contains(funcName + "(")) {
                    Cache.recursionFunc.Remove(funcName + "(");
                }

                Cache.inputType.Remove(funcName + "(");
                Cache.returnType.Remove(funcName + "(");
            }

            return false;
        }

        return true;
    }

    // Método que revisa el cuerpo
    public static bool BodyRevision(string body, List<string> vars, string funcName = "") {
        
        if (!SyntaxCheck(body)) return false;

        // En caso de 'let-in', pues el cuerpo vacío en funciones habrá dado error antes
        if (string.IsNullOrWhiteSpace(body)) {
            SetErrors("SYNTAX", $"Missing expression after 'in' in 'let-in' expression");
            return false;
        }

        body = $" {String.StringsToSpaces(body)} "; 
        
        /* Se revisa que no exista ninguna variable en el cuerpo con el mismo nombre de
        la función */
        if (funcName != "" && body.Contains($" {funcName} ")) {
            SetErrors("SYNTAX", $"Invalid token '{funcName}'");
            return false;
        }

        string n = body.Replace(" ", "");

        // Se revisa que no haya una 'keyword' sin contexto
        if (Cache.keyWords.GetRange(0, 12).Contains(n) && n != "and" &&  
            n != "or" && n != "not" && n != "PI" &&  n != "E") 
        {
            string mssg = $"Invalid token '{n}'";
            SetErrors("SYNTAX", mssg);
            return false;
        }

        body = Regex.Replace(body, @"[^_ñÑA-Za-z0-9]", " ");
        string[] words = body.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        List<string> wrongVars = new();

        // Se revisa cada variable y se agrupan las que estén incorrectas
        foreach (string word in words)
        {
            if (!vars.Contains(word) && word != funcName && !Cache.defaultFunctions.Contains(word + "(") 
                && !Cache.keyWords.Contains(word) && !Numeric.IsNumber(word) && !String.IsString(word) && 
                !Cache.constantValues.ContainsKey(word))
            { 
                if (!wrongVars.Contains($"'{word}'")) wrongVars.Add($"'{word}'");
            }

            if (word == funcName && !Cache.recursionFunc.Contains(word + "(")) {
                Cache.recursionFunc.Add(word + "(");
            }
        }

        if (wrongVars.Count > 0) {
            Console.ForegroundColor = ConsoleColor.Red;
            string are = (wrongVars.Count > 1)? "are" : "is";
            string wrongs = string.Join(",", wrongVars);
            int i = wrongs.LastIndexOf(",");
            string mssg = (wrongVars.Count > 1)? $"{wrongs[..i]} and {wrongs[(i + 1)..]}" : wrongs;
            SetErrors("SYNTAX", $"{mssg} {are} not defined");
            return false;
        }

        return true;
    }

    // Método que revisa los argumentos de las funciones
    public static bool CheckingArgs(string f, string argument, string[] args) {

        // Se revisa que 'rand' tenga argumento vacío
        if (!Cache.newFunctions.ContainsKey(f)) {

            if (f == "rand(" && argument != "") {
                string mssg = "No argument was expected in 'rand' function";
                SetErrors("SEMANTIC", mssg);
                return false;
            }

            if (f == "rand(") return true;
        }

        // Se revisa que los argumentos se correspondan con el tipo de entrada de la función
        if (args.Any(x => Types.GetType(x) != Cache.inputType[f][Array.IndexOf(args, x)] && 
            Cache.inputType[f][Array.IndexOf(args, x)] != "all")) 
        {
            string not = args.First(x => Types.GetType(x) != Cache.inputType[f][Array.IndexOf(args, x)]);
            string type = Cache.inputType[f][Array.IndexOf(args, not)];
            not = Types.GetType(not);
            string mssg = $"Function '{f[..^1]}' receives '{type}', not '{not}'";
            SetErrors("SEMANTIC", mssg);
            return false;
        }

        return true;
    }

    /* Método que revisa que el número de argumentos dados se corresponda con la cantidad 
    que recibe la función */
    public static bool NumberOfArgs(List<string> vars, List<string> values, string funcName = "") {
        
        // Se determina si se está revisando un valor o un argumento de función
        string mssg = (funcName != "")? "argument" : "value";
        string argument = (vars.Count == 1)? $"{mssg} was" : $"{mssg}s were";
        string was = (values.Count == 1)? "was" : "were";
        string f = (funcName == "")? "" : $"in '{funcName[..^1]}' function";

        // Si no se esperaba argumento y se dio alguno
        if (vars.Contains("") && !values.Contains("")) {
            string m = $"No {mssg} was expected but {values.Count} {was} given {f}";
            SetErrors("SEMANTIC", m);
            return false;
        }

        // Si se esperaba algún argumento y no se dio ninguno 
        if (values.Contains("") && !vars.Contains("")) {
            string m = $"Invalid empty argument given {f}";
            SetErrors("SEMANTIC", m);
            return false;
        }

        // Si es 'log' se revisa primero porque puede tener 1 o 2 argumentos
        if (funcName == "log(") {
            if (values.Count > 2) {
                string m = $"{vars.Count} {argument} expected but {values.Count} {was} given {f}";
                SetErrors("SEMANTIC", m);
                return false;
            }
        }

        // En otro caso sí se revisa que las cantidades correspondan
        else if (vars.Count != values.Count) {
            string m = $"{vars.Count} {argument} expected but {values.Count} {was} given {f}";
            SetErrors("SEMANTIC", m);
            return false;
        }

        return true;
    }

    #endregion

    // Revisión de variables
    #region Variable Checking

    // Método que revisa las variables
    public static bool VariableRevision(string var, bool funcName = false) {
        // Se detectan varios errores:

        // 1. Variable con igual nombre que la función que la declara
        if (funcVars.Contains(var)) {
            string mssg = $"'{var}' is already defined as a variable of the function";
            SetErrors("SYNTAX", mssg);
            return false;
        }

        if (Cache.constantValues.ContainsKey(var)) {
            string mssg = $"'{var}' is already defined as a constant";
            SetErrors("SYNTAX", mssg);
            return false;
        }

        // 2. La variable es una keyword
        if (Cache.keyWords.Contains(var)) {
            string mssg = $"'{var}' is a keyword";
            SetErrors("SYNTAX", mssg);
            return false;
        }

        // 3. La variable es una función ya creada
        if (Cache.defaultFunctions.Contains(var + "(")) {
            string mssg = $"'{var}' is already defined";
            SetErrors("SYNTAX", mssg);
            return false;
        }

        // 4. La variable está mal escrita (empieza con número, o posee símbolos incorrectos)
        if (char.IsDigit(var[0]) || var.ToLower() != Regex.Replace(var.ToLower(), @"[^_ña-z0-9]", "")) {
            string mssg = $"'{var}' is not a valid token";
            string func = funcName? "Function" : "Constant";
            if (char.IsDigit(var[0])) mssg += ". It can not begin with number";
            else mssg += $". {func} name can only have letters, numbers and '_'";
            
            SetErrors("LEXICAL", mssg);
            return false;
        }

        return true;
    }

    // Método que revisa las variables y valores en 'let-in'
    public static bool Let_in_Check(List<string> vars, List<string> values, bool function = false) {

        List<string> newVars = new();
        List<string> newValues = new();

        for (int i = vars.Count - 1; i >= 0 ; i--)
        {
            if (!BodyRevision(values[i], newVars)) return false;

            string evaluation = FuncInstruction.Eval(values[i], newVars, newValues);
            string temp =  function? evaluation : Main.Parse(evaluation);

            if (temp == "") return false;

            newVars.Add(vars[i]);
            newValues.Add(temp);
        }
        
        return true;
    }

    #endregion

    // Métodos para setear Errores
    #region Set Errors

    // Ambos métodos imprimen el error en rojo y actualizan el campo 'error'
    public static void SetErrors(string typeError, string mssg)
    {
        if (Main.error) return;

        Main.error = true;
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine($"!{typeError} ERROR: {mssg}");
    }

    public static void SetErrors(string left, string right, string op) {
        
        if (Main.error) return;

        Main.error = true;
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine($"!SEMANTIC ERROR: Operator '{op}' can not be used between '{left}' and '{right}'");
    }

    #endregion
}