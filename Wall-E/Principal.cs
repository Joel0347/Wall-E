namespace WallE;

public class Principal
{
    public static string Analize(string s) {
        string n = String.StringsToSpaces(s);
        string[] lines = n.Split(";");

        foreach (var line in lines)
        {
             // Se comprueba que estén balanceadas las comillas
            if (Extra.Quotes(line) % 2 != 0) {
                string mssg = "Undeterminated 'string'. Missing quote";
                Check.SetErrors("SYNTAX", mssg);
                return "";
            }

            // Se comprueba que estén balanceados los paréntesis
            if (Check.ParenthesisRevision(line) != 0)  {
                string parenthesis = (Check.ParenthesisRevision(line) == 1)? "'('" : "')'";
                Check.SetErrors("SYNTAX", $"Missing {parenthesis} in global expression");
                return "";
            }
        }

        s = s.ReplaceLineEndings(";");

         // Si la expresión es una instrucción se retorna su evaluación
        while (Instructions.IsInstruction(s)) {
            if (Main.error) return "";
            s = Instructions.result;
        }

        n = String.StringsToSpaces(s);
        List<string> instructions = new();

        int semicolonIndex = n.IndexOf(";");
        while(semicolonIndex != -1) {
            instructions.Add(s[..semicolonIndex]);
            s = s.Remove(0, semicolonIndex + 1);
            n = n.Remove(0, semicolonIndex + 1);
            semicolonIndex = n.IndexOf(";");
        }

        instructions.Add(s);

        string m = "";
        for (int i = 0; i < instructions.Count; i++)
        {
            m += Main.Parse(instructions[i]) + ";";
        }

        return m;
    }
}