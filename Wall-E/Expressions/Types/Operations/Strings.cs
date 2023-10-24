namespace Hulk;

// Clase donde se evalúa el operador de concatenación '@'
public class Concatenate : String 
{
    public static string Eval(string leftSide, string rightSide) {
        
        if (rightSide == "" || leftSide == "") return "";
        
        // Se remueven comillas iniciales o finales antes de concatenar
        leftSide = leftSide.EndsWith("\"")? leftSide.Remove(leftSide.Length - 1) : leftSide.Insert(0, "\"");
        rightSide = rightSide.StartsWith("\"")? rightSide.Remove(0, 1) : rightSide.Insert(rightSide.Length, "\"");

        result  = leftSide + rightSide;
        return result;
    }
}