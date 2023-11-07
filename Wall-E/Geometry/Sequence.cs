namespace WallE;

public class Sequence
{
    public List<string> Elements {get;}
    public string Type {get;}

    public int Count {
        get {return Elements.Count;}
    }

    public Sequence(List<string> values) {
        string type = Types.GetType(values[0]);
        foreach (var item in values)
        {
            if(Types.GetType(item) != type) {
                Check.SetErrors("SYNTAX", "'Sequence' can't have elements of different types");
                return;
            }
        }

        this.Elements = values;
        this.Type = type;
    }
    public Sequence(string values) {
        values = values.Trim();
        string vals = $" {values[1..^1]} ";
        string withOutStrings = String.StringsToSpaces(values);
        string withOutSequences = $"{{{Extra.SequenceToSpaces(vals)}}}";
        int startIndex = withOutStrings.IndexOf("{") + 1;
        List<string> elements = new();

        int tempIndex = withOutSequences.IndexOf(",", startIndex);
        values = $" {values[startIndex..^1]} ";

        while (tempIndex != -1) {
            elements.Add(values[..tempIndex].Trim());
            withOutSequences = withOutSequences[(tempIndex + 1)..];
            values = values[(tempIndex + 1)..];
            tempIndex = withOutSequences.IndexOf(",");
        }

        elements.Add(values.Trim());

        string type = Types.GetType(elements[0]);
        foreach (var item in elements)
        {
            if(Types.GetType(item) != type) {
                Check.SetErrors("SYNTAX", "'Sequence' can't have elements of different types");
                return;
            }
        }

        this.Elements = elements;
        this.Type = type;
    }

    public string this[int i] {
        get {return Elements[i];}
        set {} 
    }

    public static bool IsSequency(string s) {
        s = s.Trim();
        s = String.StringsToSpaces(s);
        return s.Contains("{");
    }
}