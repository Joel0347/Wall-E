using System.Security.Cryptography.X509Certificates;

namespace G_Sharp;

public static class EvaluatorFacts
{
    public static readonly List<object> DefaultFalseValues = new() {
        "{}", "undefined", 0.0
    };
 
    public static readonly Dictionary<SyntaxKind, Func<object, object, double>> BinaryOperationEvaluation = new()
    {
        // numeric operations
        [SyntaxKind.PlusToken    ] = (left, right) => (double)left + (double)right,
        [SyntaxKind.MinusToken   ] = (left, right) => (double)left - (double)right,
        [SyntaxKind.MultToken    ] = (left, right) => (double)left * (double)right,
        [SyntaxKind.DivisionToken] = Division,
        [SyntaxKind.ModToken     ] = Module,

        // booleans operations
        [SyntaxKind.AndKeyword] = (left, right) => (DefaultFalseValues.Contains(left) || DefaultFalseValues.Contains(right)) ? 0 : 1,
        [SyntaxKind.OrKeyword ] = (left, right) =>(DefaultFalseValues.Contains(left) && DefaultFalseValues.Contains(right)) ? 0 : 1,
        
        [SyntaxKind.EqualToken          ] = (left, right) => (left == right) ? 1 : 0,
        [SyntaxKind.DifferentToken      ] = (left, right) => (left != right) ? 1 : 0,
        [SyntaxKind.GreaterToken       ] = (left, right) => ((double)left > (double)right) ? 1 : 0,
        [SyntaxKind.LessToken           ] = (left, right) => ((double)left < (double)right) ? 1 : 0,
        [SyntaxKind.GreaterOrEqualToken] = (left, right) => ((double)left >= (double)right) ? 1 : 0,
        [SyntaxKind.LessOrEqualToken    ] = (left, right) => ((double)left <= (double)right) ? 1 : 0,
    };

    public static readonly Dictionary<SyntaxKind, Func<object, double>> UnaryOperationEvaluation = new()
    {
        [SyntaxKind.PlusToken ] = (operand) => (double)operand,
        [SyntaxKind.MinusToken] = (operand) => - (double)operand,
        [SyntaxKind.NotKeyword] = (operand) => DefaultFalseValues.Contains(operand) ? 1 : 0,
    };
    
    private static double Division(object left, object right) {
        if ((double)right == 0) {
            Error.SetError("!!SEMANTIC ERROR: Division by '0' is not defined");
            return 0;
        }

        return (double)left / (double)right;
    }

    private static double Module(object left, object right) {
        if ((double)right == 0) {
            Error.SetError("!!SEMANTIC ERROR: Division by '0' is not defined");
            return 0;
        }

        return (double)left % (double)right;
    }
}