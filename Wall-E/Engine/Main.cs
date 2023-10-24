using Wall_E;

namespace Hulk;

// Clase principal del intérprete
public static class Main
{
    public static bool error = false;

    /* Se chequea que cumpla los requisitos mínimos para interpretar la
    expresión*/
    #region Checking the global input
    public static string GlobalInput(string s) {

        // Se comprueba que no sea vacía la entrada
        if (string.IsNullOrWhiteSpace(s)) return "";
        s = s.Trim();
        
        // Se comprueba que estén balanceadas las comillas
        if (Extra.Quotes(s) % 2 != 0) {
            string mssg = "Undeterminated 'string'. Missing quote";
            Check.SetErrors("SYNTAX", mssg);
            return "";
        }

        // Se comprueba que estén balanceados los paréntesis
        if (Check.ParenthesisRevision(s) != 0)  {
            string parenthesis = (Check.ParenthesisRevision(s) == 1)? "'('" : "')'";
            Check.SetErrors("SYNTAX", $"Missing {parenthesis} in global expression");
            return "";
        }
        
        // Se comprueba que la línea termine en punto y coma
        if (!s.EndsWith(";")) {
            string mssg = "Missing ';' at the end of the line";
            Check.SetErrors("SYNTAX", mssg);
            return "";
        }

        // Eliminamos los ';' al final
        while (s.EndsWith(";") || s.EndsWith(" ")){
            s = s.TrimEnd(';');
            s = s.TrimEnd();
        }

        // Se llama a la función que parsea la entrada
        return Parse(s);
    }

    #endregion

    // Se parsea la entrada
    #region Parsing input
    public static string Parse(string s) {

        if (Data.constantValues.ContainsKey(s.Trim())) {
            s = Data.constantValues[s.Trim()];
        }
        
        if (string.IsNullOrWhiteSpace(s)) return "";

        s = s.Trim();

        // Si la expresión es una instrucción se retorna su evaluación
        if (Instructions.IsInstruction(s)) {
            if (error) return "";
            return Instructions.result;
        }
        
        if(Constants.IsConstant(s)) {
            Constants.Eval(s);
            return "";
        }

        // Se realizan las revisiones sintáctica y semántica
        if (!Check.SyntaxCheck(s)) return "";
        if (!Check.SemanticCheck(s)) return "";

        /* Si la expresión es un tipo específico (paréntesis, boolean, string o number)
        se evalúan*/
        if (Types.IsType(s))  {
            if (error) return "";
            return Types.result;
        }

        // Si no cumplió alguna de las condiciones anteriores se imprime el error
        Check.SetErrors("SYNTAX", $"Expression '{s}' is not valid");
        return "";      
    }

    #endregion
}