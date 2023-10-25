namespace WallE;

// Clases que evalúan los tokens numéricos
public class Add : Numeric 
{
    public static string Eval(string leftSide, string rightSide) {

        if (rightSide == "" || leftSide == "") return "";

        result = (double.Parse(rightSide) + double.Parse(leftSide)).ToString();

        return result;
    }
}

public class Mult : Numeric
{
    public static string Eval(string leftSide, string rightSide) {

        if (rightSide == "" || leftSide == "") return "";

        result = (double.Parse(rightSide) * double.Parse(leftSide)).ToString();

        return result;
    }
}

public class Division : Numeric
{
    public static string Eval(string leftSide, string rightSide) {
        
        if (rightSide == "" || leftSide == "") return "";

        // Se revisa el caso de la división por '0'
        if (double.Parse(rightSide) == 0) {
            string mssg = "Division by '0' is not defined";
            Check.SetErrors("SEMANTIC", mssg);
            return "";
        }

        result = (double.Parse(leftSide) / double.Parse(rightSide)).ToString();

        return result;
    }
}

public class Mod : Numeric
{
    public static string Eval(string leftSide, string rightSide) {

        if(rightSide == "" || leftSide == "") return "";

        // Se revisa el caso de la división por '0'
        if (double.Parse(rightSide) == 0) {
            string mssg = "Division by '0' is not defined";
            Check.SetErrors("SEMANTIC", mssg);
            return "";
        }

        result = (double.Parse(leftSide) % double.Parse(rightSide)).ToString();

        return result;
    }
}

public class Power : Numeric
{
    public static string Eval(string leftSide, string rightSide) {

        if(rightSide == "" || leftSide == "") return "";

        result = Math.Pow(double.Parse(leftSide), double.Parse(rightSide)).ToString();

        return result;
    }

    /* Método que evalúa la potencia si lleva paréntesis. Está hecho para evitar errores de
    signos en la solución. Por ejemplo no es lo mismo ' 4 - 2 ^ 2' que '4 + (-2) ^ 2'. Por lo 
    tanto ese paréntesis no puede evaluarse normal. Debe resolverse la potencia directamente */
    public static (string, int) EvalWithParenthesis(string s, string n, int end, string parenthesis) {
        char[] symbols = {
            '+', '-', '/', '*', '%', '@', '&', 
            '|', '!', '=', '<', '>', ')', ','
        };
        
        int pow = n[(end + 1)..].IndexOf("^");
        int digit = n[(end + 1)..].IndexOf(n[(end + 1)..].FirstOrDefault(char.IsDigit));

        if (digit == -1) return (parenthesis, end);
        
        int stop = n[(end + 1)..].IndexOfAny(symbols, digit);
        
        while ((s[stop + end + 1] == '+' || s[stop + end + 1] == '-') && 
                s[stop + end] == 'E' && char.IsDigit(s[stop + end - 1])) 
        {
            stop = n[(end + 1)..].IndexOfAny(symbols, stop + 1);

            if (stop == -1) {
                stop = s[(end + 1)..].Length;
                break;
            }
        }
        
        if (stop == -1) stop = s[(end + 1)..].Length;

        string exp = Main.Parse(s[(end + 2 + pow)..(stop + end + 1)]);

        if (IsNumber(exp)) {
            parenthesis = Eval(parenthesis, exp);
            end = stop + end; 
        }

        return (parenthesis, end);
    }
}






