using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Metadata;

namespace G_Sharp;

public class Scope
{
    public Dictionary<string, Constant> Constants { get; }
    public Dictionary<string, Function> Functions { get; }

    public Scope(
        Dictionary<string, Constant> constants,
        Dictionary<string, Function> functions
    )
    {
        Constants = constants;
        Functions = functions;
    }
}