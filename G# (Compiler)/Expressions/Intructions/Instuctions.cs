namespace WallE;

// Clase que evalúa las instrucciones (Let-in, if-else, function)
public class Instructions
{
    public static string result = "";

    // Si es una instrucción se evalúa
    public static bool IsInstruction(string s) {

        if (FuncInstruction.IsFunctionInstruction(s))  result = FuncInstruction.CreateFunction(s);
        else if (Let_in.IsLet_in(s)) result = Let_in.Eval(s);
        else if (Conditional.IsConditional(s)) result = Conditional.Eval(s);
        else return false;
        
        return true;
    }
}