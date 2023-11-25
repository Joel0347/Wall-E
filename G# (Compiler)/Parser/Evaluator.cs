using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;

namespace G_Sharp;

public sealed class Evaluator
{
    public ExpressionSyntax Root { get; }
    public Scope Scope { get; }

    public Evaluator(ExpressionSyntax root, Scope scope)
    {
        Root = root;
        Scope = scope;
    }

    public object Evaluate()
    {
        return EvaluateExpression(Root);
    }
    
    private object EvaluateExpression(ExpressionSyntax node)
    {
        if (node is ErrorExpressionSyntax) 
            return "";

        if (node is LiteralExpressionSyntax node1)
            return node1.LiteralToken.Value;

        if (node is FunctionExpressionSyntax node2)
        {
            string name = node2.IdentifierToken.Text;
            return EvaluateFunctionExpression(name, node2.Values);
        }

        if (node is AssignmentFunctionExpressionSyntax node3)
        {            
            string name = node3.FunctionIdentifierToken.Text;

            if (Scope.Functions.ContainsKey(name))
            {
                Error.SetError($"!!SYNTAX ERROR: Function '{name}' is already defined");
                return "";
            }

            if (!Error.Wrong) {
                Scope.Functions[name] = new Function(node3.Expression, node3.IdentifiersToken);
            }

            return "";
        }

        if (node is NameExpressionSyntax node6)
        {
            string name = node6.IdentifierToken.Text;

            if (Scope.Constants.TryGetValue(name, out Constant? value))
            {
                return value.Expression;
            }
            
            Error.SetError($"!!SYNTAX ERROR: Constant '{name}' is not defined yet");
            return "";
        }    
        
        if (node is AssignmentExpressionSyntax node7)
        {
            string name = node7.IdentifierToken.Text;
            var value = EvaluateExpression(node7.Expression);

            if (!Scope.Constants.ContainsKey(name))
            {
                Scope.Constants[name] = new Constant(value);
                return value;
            }

            Error.SetError($"!!SYNTAX ERROR: Constant '{name}' is already defined");
            return "";
        }

        if (node is UnaryExpressionSyntax node8)
        {
            var operand = EvaluateExpression(node8.Operand);
            if (!SemanticCheck.UnaryOperatorsCheck[node8.OperatorToken.Kind](operand)) {
                var operandType = SemanticCheck.GetType(operand);
                var operation = node8.OperatorToken.Text;
                Error.SetError($"!!SEMANTIC ERROR: Operator '{operation}' can't not be used before '{operandType}'");
                return "";
            }

            return EvaluatorFacts.UnaryOperationEvaluation[node8.OperatorToken.Kind](operand);
        }

        if (node is BinaryExpressionSyntax node9)
        {
            var left = EvaluateExpression(node9.Left);
            var right = EvaluateExpression(node9.Right);
            var operation = node9.OperatorToken;
            var operatorKind = operation.Kind;
            
            
            if (!SemanticCheck.BinaryOperatorsCheck[operatorKind](left, right)) {
                string leftType = SemanticCheck.GetType(left);
                string rightType = SemanticCheck.GetType(right);
                Error.SetError($"!!SEMANTIC ERROR: Operator '{operation.Text}' can't be used between '{leftType}' and '{rightType}'");
                return "";
            }

            return EvaluatorFacts.BinaryOperationEvaluation[operatorKind](left, right);
        }

        if (node is ParenthesizedExpressionSyntax node10)
        {
            return EvaluateExpression(node10.Expression);
        }

        Error.SetError("lqs");
        return 0;
    }

    private object EvaluateFunctionExpression(string name, List<ExpressionSyntax> values)
    {
        if (!Scope.Functions.TryGetValue(name, out Function? function)) {
            Error.SetError($"!!SEMANTIC ERROR: Function '{name}' is not defined yet");
            return "";
        }

        Dictionary<string, Constant> constants = new();

        for (int i = 0; i < values.Count; i++)
        {
            NameExpressionSyntax variable = (NameExpressionSyntax) function.Parameters[i];
            var expression = EvaluateExpression(values[i]);
            var identifier = variable.IdentifierToken;
            constants[identifier.Text] = new Constant(expression);
        }

        foreach (var item in Scope.Constants.Keys)
        {
            if (!constants.ContainsKey(item))  
                constants[item] = Scope.Constants[item];
        }

        Scope child = new(constants, Scope.Functions); 

        var evaluator = new Evaluator(function.Body, child);
        return evaluator.Evaluate();
    }
}