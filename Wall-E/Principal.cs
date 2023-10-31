using System.Text.RegularExpressions;
namespace WallE;

public class Principal
{
    public static string Analize(string s) {
        // string[] lines = n.Split(";");

        // foreach (var line in lines)
        // {
        //      // Se comprueba que estén balanceadas las comillas
        //     if (Extra.Quotes(line) % 2 != 0) {
        //         string mssg = "Undeterminated 'string'. Missing quote";
        //         Check.SetErrors("SYNTAX", mssg);
        //         return "";
        //     }

        //     // Se comprueba que estén balanceados los paréntesis
        //     if (Check.ParenthesisRevision(line) != 0)  {
        //         string parenthesis = (Check.ParenthesisRevision(line) == 1)? "'('" : "')'";
        //         Check.SetErrors("SYNTAX", $"Missing {parenthesis} in global expression");
        //         return "";
        //     }
        // }

        // s = s.ReplaceLineEndings(";");

        //  // Si la expresión es una instrucción se retorna su evaluación
        // while (Instructions.IsInstruction(s)) {
        //     if (Main.error) return "";
        //     s = Instructions.result;
        // }
        s = s.Replace("\r", " ");
        s = s.Replace("\t", " ");
        s = $" {s} ";
        string n = String.StringsToSpaces(s);
        string m = Regex.Replace(n, @"[^_""ñÑA-Za-z0-9]", " ");
        int letIndex = m.LastIndexOf(" let ");
        while (letIndex >= 0) {
            int skip = n.IndexOf("\n", letIndex);
            int inIndex = m.IndexOf(" in ", letIndex);
            if (inIndex == - 1) {
                Check.SetErrors("SYNTAX", "Missing token 'in' in 'let-in' expression");
                return "";
            }

            if(skip == - 1) skip = s.Length;

            if (inIndex > skip) {
                int newSkip = n.IndexOf("\n", skip + 1, inIndex - skip - 1);
                while(newSkip >= 0) {
                    s = s.Remove(newSkip, 1);
                    n = n.Remove(newSkip, 1);
                    s = s.Insert(newSkip, " ");
                    n = n.Insert(newSkip, " ");
                    newSkip = n.IndexOf("\n", skip + 1, inIndex - skip - 1);
                }

                s = s.Remove(skip, 1);
                s = s.Insert(skip, " ");
                n = n.Remove(skip, 1);
                n = n.Insert(skip, " ");
            }

            s = s.Remove(inIndex + 1, 1);
            s = s.Insert(inIndex + 1, "I");
            n = n.Remove(inIndex + 1, 1);
            n = n.Insert(inIndex + 1, "I");
            m = Regex.Replace(n, @"[^_""ñÑA-Za-z0-9]", " ");
            letIndex = m[..letIndex].LastIndexOf(" let ");
        }

        List<string> instructions = new();

        // int semicolonIndex = n.IndexOf(";");
        // while(semicolonIndex != -1) {
        //     instructions.Add(s[..semicolonIndex]);
        //     s = s.Remove(0, semicolonIndex + 1);
        //     n = n.Remove(0, semicolonIndex + 1);
        //     semicolonIndex = n.IndexOf(";");
        // }

        // instructions.Add(s);
        instructions = s.Split("\n", StringSplitOptions.TrimEntries).ToList();
        instructions.RemoveAll(x => x == "");

        string p = "";
        for (int i = 0; i < instructions.Count; i++)
        {
            p += Main.GlobalInput(instructions[i]);
        }

        return p;
    }
}