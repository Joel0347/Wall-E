using System.ComponentModel;
using System.Net;
using System.Text.RegularExpressions;

namespace WallE;

// Clase que evalúa las instrucciones 'let-in'
public class Let_in 
{
    static readonly string[] symbols = {
        "*", "/", "^", "%", "+", "-", "(", 
        ")", ">", "<", "&","|","!", ",", "=", " ", ";"
    };

    // Método que determina si la instrucción es 'let-in'
    public static bool IsLet_in(string s) {
        s = String.StringsToSpaces(s);
        string[] token = s.Split(symbols, StringSplitOptions.RemoveEmptyEntries);
        return token.Contains("let");
    }

    // Método que evalúa la instrucción 'let-in'
    public static string Eval(string s, bool function = false) {  
        s = s.Insert(0, " ");
        s = s.Insert(s.Length, " ");
    
        List<string> allVars = new();
        List<string> allValues = new();
        
        // Se ignoran los strings en 'm' y todo lo que no sea letra, número o '_' en 'n'
        string m = String.StringsToSpaces(s);
        string n = Regex.Replace(m, @"[^_""ñÑA-Za-z0-9]", " ");
        
        // Se buscan los tokens principales de la expresión
        int index_let = n.LastIndexOf(" let ");
        while (s[(index_let + 4)..].Trim().StartsWith(";")) {
            int semicolonIndex = m.IndexOf(";", index_let + 1);
            s = s.Remove(semicolonIndex, 1);
            s = s.Insert(semicolonIndex, " ");
        }

        m = String.StringsToSpaces(s);
        n = Regex.Replace(m, @"[^_""ñÑA-Za-z0-9]", " ");

        int index_in = n.IndexOf(" in ", index_let) + 1;

        // Se revisa que no haya nada inválido delante de la instrucción
        if (s[..(index_let + 1)].Trim() != "") {
            string tempS = s[..(index_let + 1)].Trim();
            string[] keys = {"in", "else"};
            
            if (!keys.Any(Regex.Replace(tempS, @"[^_""ñÑA-Za-z0-9]", " ").EndsWith) &&
                !symbols.Any(s[..(index_let + 1)].Trim().EndsWith)) 
            {
                string mssg = tempS.Contains(" ")? tempS[(tempS.LastIndexOf(" ") + 1)..] : tempS;
                Check.SetErrors("SYNTAX", $"Invalid token '{mssg}' before 'let-in' expression");
                return "";
            }
        }
    
        int stop = m.IndexOf(";", index_in + 1);

        // Se obtiene el índice donde termina la instrucción 'let-in'
        if (m[index_let] == '(') {
            int indexOpen = m.LastIndexOf("(");
            string temp = m;
            (int, string) tuple = Extra.GetClosingParenthesis(index_let, indexOpen, temp);
            stop = tuple.Item2.IndexOf(")", tuple.Item1);
        }

        // Si falta el token 'in' se detecta el error
        if (index_in == 0 || stop < index_in) {
            Check.SetErrors("SYNTAX", "Missing 'in' in 'let-in' expression");
            return "";
        }

        // Se guarda el argumento completo
        string argument = m[(index_let + 4)..index_in];

        if (Check.ParenthesisRevision(argument) == -1) {
            Check.SetErrors("SYNTAX", "Invalid token ')' after 'in' in 'let-in' expression");
            return "";
        }

        // Si pasa las revisiones entonces se separan las declaraciones del argumento en una lista
        int tempIndex = argument.IndexOf(";");
        List<string> declarations = new(); 

        while (tempIndex != -1) {
            declarations.Add(argument[..tempIndex]);
            argument = argument[(tempIndex + 1)..];
            tempIndex = argument.IndexOf(";");
        }

        declarations.Add(argument);

        List<string> vars = new();
        List<string> values = new();
        m = m[(index_let + 4)..];
        
        // Se divide cada declaración en variables y valores(izq. y der. del '=')
        for (int i = declarations.Count - 1; i >= 0; i--)
        {
            // Se realizan algunas revisiones
            if (string.IsNullOrWhiteSpace(declarations[i])) {
                continue;
            }

            int equal = declarations[i].IndexOf("=");

            if (equal == -1) {                
                Check.SetErrors("SYNTAX", $"Missing '=' in '{declarations[i].Trim()}' declaration");
                return "";
            }

            if (declarations[i][..equal].Contains("(")) {
                Check.SetErrors("SYNTAX", "Invalid token '(' in constant in 'let-in' expression");
                return "";
            }

            // Se obtiene la variable y si no tiene errores se guarda
            int lengthVar = declarations[i][..equal].Length;
            int start = m.IndexOf(declarations[i]) + index_let + 4;
            string var = s.Substring(start, lengthVar);   
            
            if (!Check.SyntaxCheck(var)) return "";

            if (string.IsNullOrWhiteSpace(var)) {
                Check.SetErrors("SEMANTIC", "Missing constant in 'let-in' expression");
                return "";
            }

            vars.Add(var.Trim());
            allVars.Add(var.Trim());

            // Se obtiene el valor y si no tiene errores se guarda
            int lengthVal = declarations[i][(equal + 1)..].Length;
            start = m.IndexOf(declarations[i]) + index_let + lengthVar + 5;
            string val = s.Substring(start, lengthVal);

            if (!Check.SyntaxCheck($"({val})")) return "";

            if (val.Replace(" ","") != "()") {
                if (string.IsNullOrWhiteSpace(val)) {
                    string mssg = $"No value was assigned to constant '{var.Trim()}' in 'let-in' expression";
                    Check.SetErrors("SYNTAX", mssg);
                }

                values.Add(val);
                allValues.Add(val);
            }
        }

        string body = s[(index_in + 2)..stop];
        m = Extra.FunctionsToSpaces(body);
        
        // Condicionales para ajustar el 'stop' de la instrucción
        /*1. En caso de ser argumento de una función de varios argumentos el 'stop' 
        marcará la ',' correspondiente */
        if (m.Contains(",")) {
            stop = index_in + 2 + String.StringsToSpaces(m).IndexOf(",");
            body = s[(index_in + 2)..stop];
        }
        
        m = String.StringsToSpaces(body);
        m = Regex.Replace(m, @"[^_""ñÑ,A-Za-z0-9]", " ");

        /* 2. En caso de haber otro 'in' implica que la instrucción es un valor asignado 
        a una de las variables de un 'let-in' más externo. Por tanto se ajusta el 'stop' */
        if (m.Contains(" in ")) {
            stop = index_in + 2 + m.IndexOf(" in ");
            body = s[(index_in + 2)..stop];
        }

        /* 3. Similar al anterior, la instrucción puede ser cuerpo de una condicional. En
        dependencia el 'stop' marcará el 'else' o el 'elif' */
        if (m.Contains(" else ")) {
            int else_index = m.LastIndexOf(" else ");
            int if_index = m[..else_index].LastIndexOf(" if ");
           
            while (if_index != -1) {
                else_index = m.IndexOf(" else ", if_index);
                m = m.Remove(if_index, else_index + 5 - if_index);
                string spaces = new(' ', else_index + 5 - if_index);
                m = m.Insert(if_index, spaces);
                else_index = m.LastIndexOf(" else ");
                if (else_index == -1) break;
                if_index = m[..else_index].LastIndexOf(" if ");
            }
           
            if (m.Contains(" else ") && Conditional.IsConditional(s[..index_let])) {
                stop = index_in + 2 + m.IndexOf(" else ");
            }

            body = s[(index_in + 2)..stop];
        }

        m = String.StringsToSpaces(body);
        m = Regex.Replace(m, @"[^_""ñÑ,A-Za-z0-9]", " ");

        if (m.Contains(" elif ")) {
            int elif_index = m.LastIndexOf(" elif ");
            int if_index = m[..elif_index].LastIndexOf(" if ");
           
            while (if_index != -1) {
                elif_index = m.IndexOf(" elif ", if_index);
                m = m.Remove(if_index, elif_index + 5 - if_index);
                string spaces = new(' ', elif_index + 5 - if_index);
                m = m.Insert(if_index, spaces);
                elif_index = m.LastIndexOf(" elif ");
                if (elif_index == -1) break;
                if_index = m[..elif_index].LastIndexOf(" if ");
            }
           
            if (m.Contains(" elif ") && Conditional.IsConditional(s[..(index_in + 2 + m.IndexOf(" elif "))])) {
                stop = index_in + 2 + m.IndexOf(" elif ");
            }

            body = s[(index_in + 2)..stop];
        }

        // Se realizan más revisiones a las variables
        if (!vars.All(x => Check.VariableRevision(x))) return "";

        if (n[..index_let].LastIndexOf(" let ") == -1) {
            if (!Check.NumberOfArgs(vars, values)) return "";
            if (!Check.Let_in_Check(allVars, allValues, function)) return "";
            if (!Check.BodyRevision(body, vars)) return "";
        }

        // Finalmente se sustituyen con el mismo método que las funciones definidas mediante 'function'
        s = s.Remove(index_let + 1, stop - index_let - 1);
        s = s.Insert(index_let + 1, $"({FuncInstruction.Eval(body, vars, values)})");

        /* En caso de estar dentro de una función no se retorna la evaluación de la expresión,
        sino solamente la expresión con el cuerpo sustituido */
        return  function? s : Principal.Analize(s);
    }
}
