namespace G_Sharp;

public static class SemanticCheck
{
    public static string GetType(object obj)
    {
        return obj switch
        {
            double or int or decimal or float or long => "number",
            string => "string",
            null => "undefined",
            ExpressionSyntax expression => expression.ReturnType,
            _ => "undefined expression"
        }; 
    }
}