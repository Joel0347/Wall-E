using System.Text.RegularExpressions;
namespace WallE;

public class Principal
{
    public static void Analize(string s) {
        
        Save.WriteDocument(s, "Joel");
        s = s.Replace("\r", " ");
        s = s.Replace("\t", " ");
        s = $" {s} ";
        string n = String.StringsToSpaces(s);
        string m = Regex.Replace(n, @"[^_""ñÑA-Za-z0-9]", " ");
        int letIndex = m.LastIndexOf(" let ");
        while (letIndex >= 0) {
            int skip = n.IndexOf("\n", letIndex);
            int inIndex = m.IndexOf(" in ", letIndex) + 1;
            if (inIndex == - 1) {
                Check.SetErrors("SYNTAX", "Missing token 'in' in 'let-in' expression");
                return;
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

            s = s.Remove(inIndex, 1);
            s = s.Insert(inIndex, "I");
            n = n.Remove(inIndex, 1);
            n = n.Insert(inIndex, "I");
            m = Regex.Replace(n, @"[^_""ñÑA-Za-z0-9]", " ");
            letIndex = m[..letIndex].LastIndexOf(" let ");
        }

        List<string> instructions = s.Split("\n", StringSplitOptions.TrimEntries).ToList();
        instructions.RemoveAll(x => x == "");

       
        // for (int i = 0; i < instructions.Count; i++) {
        //     Main.GlobalInput(instructions[i]);
        // }
    }
}