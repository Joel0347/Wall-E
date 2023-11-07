using System.Reflection;
using System.Reflection.Metadata;

namespace WallE;

public class Constants
{
    public static bool IsConstant(string s) {
        string n = String.StringsToSpaces(s);
        int equalIndex = n.IndexOf("=");
        if(equalIndex != s.Length - 1 && equalIndex != -1 && n[equalIndex + 1] != '=') return true;
        return false;
    }

    public static void Eval(string s) {
        string withOutStrings = String.StringsToSpaces(s);
        int equalIndex = withOutStrings.IndexOf("=");
        string constant = s[..equalIndex].Trim();
        withOutStrings = withOutStrings[..equalIndex].Trim();
        string withOutFunctions = Extra.FunctionsToSpaces(withOutStrings);

        if(withOutFunctions.Contains(",")) {
            if (!Sequence.IsSequency(s[(equalIndex + 1)..])) {
                Check.SetErrors("SEMANTIC", "A 'sequency' was expected after '='");
                return;
            }

            Sequence sequence = new(s[(equalIndex + 1)..]);
            if (Main.error) return;

            List<string> constants = new();

            int tempIndex = withOutFunctions.IndexOf(",");

            while (tempIndex != -1) {
                constants.Add(constant[..tempIndex].Trim());
                withOutFunctions = withOutFunctions[(tempIndex + 1)..];
                constant = constant[(tempIndex + 1)..];
                tempIndex = withOutFunctions.IndexOf(","); 
            }

            constants.Add(constant.Trim());

            if (!constants.All(x => x == "_") && constants.Any(x => x == "_" && 
                constants.IndexOf(x) > 0 && constants.LastIndexOf(x) < constants.Count - 1)) 
            {
                Check.SetErrors("SYNTAX", "Invalid '_' as identifier of sequence value");
                return;
            }

            int minIndex = Math.Min(constants.Count - 1, sequence.Count);

            for (int i = 0; i < minIndex; i++)
            {
                if (constants[i] == "_") continue;
                
                if (Check.VariableRevision(constants[i])) {
                    string value = Main.Parse(sequence[i]);
                    Cache.constantsType[constant] = sequence.Type;
                    Cache.constantValues[constant] = value;
                }

                else return;
            }

            if (constants.Count  - 1 != minIndex) {
                for (int i = minIndex; i < constants.Count - 1; i++)
                {
                    if (Check.VariableRevision(constants[i])) {
                        Cache.constantsType[constant] = sequence.Type;
                        Cache.constantValues[constant] = "undefined";
                    }

                    else return;
                }
            }

            if(constants[^1] != "_") {
                if (Check.VariableRevision(constants[^1])) {
                    List<string> temp = new();

                    for (int i = constants.Count - 1; i < sequence.Elements.Count; i++)
                    {
                        temp.Add(sequence[i]);
                    }

                    Sequence rest = new(temp);
                    Cache.constantsType[constants[^1]] = rest.Type;
                    // Cache.constantValues[constants[^1]] = rest;
                }

                else return;
            }
        }

        else if (Check.VariableRevision(constant)) {
            string value = Main.Parse(s[(equalIndex + 1)..]);
            Cache.constantsType[constant] = Types.GetType(value);
            Cache.constantValues[constant] = value;
        }
    }
}