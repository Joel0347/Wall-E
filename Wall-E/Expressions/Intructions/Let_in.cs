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

        // Se ignoran los strings en 'm' y todo lo que no sea letra, número o '_' en 'n'
        string m = String.StringsToSpaces(s);
        string n = Regex.Replace(m, @"[^_""ñÑA-Za-z0-9]", " ");
        
        // Se buscan los tokens principales de la expresión
        int index_let = n.IndexOf(" let ");
        (int, string) closing_in = Extra.GetClosingExpression(index_let, n.LastIndexOf(" let "), n, " In ", " let ");
        int index_in = closing_in.Item2.IndexOf(" In ", closing_in.Item1) + 1;

        // Se revisa que no haya nada inválido delante de la instrucción
        if (s[..(index_let + 1)].Trim() != "") {
            string tempS = s[..(index_let + 1)].Trim();
            string[] keys = {"if", "In", "else", "elif", "then"};
            
            if (!keys.Any(Regex.Replace(tempS, @"[^_""ñÑA-Za-z0-9]", " ").EndsWith) &&
                !symbols.Any(s[..(index_let + 1)].Trim().EndsWith)) 
            {
                string mssg = tempS.Contains(" ")? tempS[(tempS.LastIndexOf(" ") + 1)..] : tempS;
                Check.SetErrors("SYNTAX", $"Invalid token '{mssg}' before 'let-in' expression");
                return "";
            }
        }

        int tempStop = m.IndexOf(";", index_in + 1);
        int stop =  tempStop != -1? tempStop : s.Length;

        // Se obtiene el índice donde termina la instrucción 'let-in'
        if (m[index_let] == '(') {
            int indexOpen = m.LastIndexOf("(");
            string temp = m;
            (int, string) tuple = Extra.GetClosingExpression(index_let, indexOpen, temp);
            stop = tuple.Item2.IndexOf(")", tuple.Item1);
        }

        // Se guarda el argumento completo
        string argument = s[(index_let + 4)..index_in];

        if (Check.ParenthesisRevision(argument) == -1) {
            Check.SetErrors("SYNTAX", "Invalid token ')' after 'in' in 'let-in' expression");
            return "";
        }

        string argumentWithOutLetIn = Extra.Let_In_To_Spaces(argument);

        // Si pasa las revisiones entonces se separan las declaraciones del argumento en una lista
        int tempIndex = argumentWithOutLetIn.IndexOf(";");
        List<string> declarations = new(); 

        while (tempIndex != -1) {
            declarations.Add(argument[..tempIndex]);
            argument = argument[(tempIndex + 1)..];
            argumentWithOutLetIn = argumentWithOutLetIn[(tempIndex + 1)..];
            tempIndex = argumentWithOutLetIn.IndexOf(";");
        }

        declarations.Add(argument);
        m = m[(index_let + 4)..];
        
        int recursionFuncLength     = Cache.recursionFunc.Count, 
            recursionSpeedLength    = Cache.recursionSpeed.Count, 
            recursionCountLength    = Cache.recursionCount.Count, 
            functionLength          = Cache.newFunctions.Count, 
            constantLength          = Cache.constantsType.Count, 
            colorLength             = Cache.color.Count;

        for (int i = 0; i < declarations.Count; i++)
        {
            // Se realizan algunas revisiones
            if (string.IsNullOrWhiteSpace(declarations[i])) {
                continue;
            }

            Main.Parse(declarations[i]);
            if (Main.error) return "";
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
        if (m.Contains(" In ")) {
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

        m = String.StringsToSpaces(body);
        m = Regex.Replace(m, @"[^_""ñÑ,A-Za-z0-9]", " ");

        if (m.Contains(" then ")) {
            int then_index = m.LastIndexOf(" then ");
            int if_index = m[..then_index].LastIndexOf(" if ");
           
            while (if_index != -1) {
                then_index = m.IndexOf(" then ", if_index);
                m = m.Remove(if_index, then_index + 5 - if_index);
                string spaces = new(' ', then_index + 5 - if_index);
                m = m.Insert(if_index, spaces);
                then_index = m.LastIndexOf(" then ");
                if (then_index == -1) break;
                if_index = m[..then_index].LastIndexOf(" if ");
            }
           
            if (m.Contains(" then ") && Conditional.IsConditional(s[..(index_in + 2 + m.IndexOf(" then "))])) {
                stop = index_in + 2 + m.IndexOf(" then ");
            }

            body = s[(index_in + 2)..stop];
        }

        // Finalmente se sustituyen con el mismo método que las funciones definidas mediante 'function'
        s = s.Remove(index_let + 1, stop - index_let - 1);
        s = s.Insert(index_let + 1, Main.Parse(body));
        Cache.CacheReset(
                recursionFuncLength, recursionSpeedLength, recursionCountLength,
                functionLength, constantLength, colorLength
            );

        /* En caso de estar dentro de una función no se retorna la evaluación de la expresión,
        sino solamente la expresión con el cuerpo sustituido */
        return  function? s : Main.Parse(s);
    }
}
