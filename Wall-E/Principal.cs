namespace Hulk;

public class Principal
{
    public static string Analize(string s) {
        s = s.ReplaceLineEndings(";");

         // Si la expresión es una instrucción se retorna su evaluación
        if (Instructions.IsInstruction(s)) {
            if (Main.error) return "";
            return Instructions.result;
        }

        string n = String.StringsToSpaces(s);
        List<string> instructions = new();

        int semicolonIndex = n.IndexOf(";");
        int initial = 0;
        while(semicolonIndex != -1) {
            instructions.Add(s[initial..semicolonIndex]);
            initial = semicolonIndex + 1;
            semicolonIndex = n.IndexOf(";", semicolonIndex + 1);
        }
        string m = "";
        for (int i = 0; i < instructions.Count; i++)
        {
            m += Main.Parse(instructions[i]) + ";";
        }

        return m;
    }
}