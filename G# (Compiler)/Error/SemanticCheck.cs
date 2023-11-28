namespace G_Sharp;

public static class SemanticCheck
{
    public static readonly Dictionary<SyntaxKind, Func<object, object, bool>> BinaryOperatorsCheck = new()
    {
        //Numeric operators
        [SyntaxKind.PlusToken] = NumericOperatorsCheck,
        [SyntaxKind.MinusToken] = NumericOperatorsCheck,
        [SyntaxKind.MultToken] = NumericOperatorsCheck,
        [SyntaxKind.DivisionToken] = NumericOperatorsCheck,
        [SyntaxKind.ModToken] = NumericOperatorsCheck,
        [SyntaxKind.GreaterToken] = NumericOperatorsCheck,
        [SyntaxKind.LessToken] = NumericOperatorsCheck,
        [SyntaxKind.GreaterOrEqualToken] = NumericOperatorsCheck,
        [SyntaxKind.LessOrEqualToken] = NumericOperatorsCheck,

        //Booleans operators
        [SyntaxKind.AndKeyword] = (left, right) => true,
        [SyntaxKind.OrKeyword] = (left, right) => true,
        [SyntaxKind.EqualToken] = EqualsCheck,
        [SyntaxKind.DifferentToken] = EqualsCheck
    };

    public static readonly Dictionary<SyntaxKind, Func<object, bool>> UnaryOperatorsCheck = new()
    {
        [SyntaxKind.PlusToken] = (operand) => GetType(operand) == "number",
        [SyntaxKind.MinusToken] = (operand) => GetType(operand) == "number",
        [SyntaxKind.NotKeyword] = (operand) => true
    };

    public static string GetType(object obj)
    {
        return obj switch
        {
            double or int or decimal or float or long => "number",
            string => "string",
            _ => "undefined"
        };
    }

    private static bool NumericOperatorsCheck(object left, object right)
    {
        return GetType(left) == "number" && GetType(right) == "number";
    }

    private static bool EqualsCheck(object left, object right)
    {
        return GetType(left) == GetType(right);
    }
}