namespace G_Sharp;

public static class Program
{
    public static void Main(string[] args)
    {
        Dictionary<string, object> variables = new();
        Dictionary<string, List<ExpressionSyntax>> functionsVariables = new();
        Dictionary<string, ExpressionSyntax> functionsBody = new();

        Scope global = new(variables, functionsVariables, functionsBody);

        while (true)
        {
            Error.ResetError();
            Console.Write("> ");

            string line = Console.ReadLine()!;
            // string line = "f(x, y = 1";

            // if (string.IsNullOrWhiteSpace(line))
            // return;

            var syntaxTree = SyntaxTree.Parse(line);

            var evaluation = new Evaluator(syntaxTree.Root, global);
            try {
                var result = evaluation.Evaluate();

                if (Error.Wrong)
                {
                    Console.WriteLine(Error.Mssg);
                }
                
                else {
                    Console.WriteLine(result);
                }
            }

            catch (Exception) {
                Console.WriteLine("!!COMPILE ERROR: Unexpected error has ocurred");
            }

            // line = "k(x) = f(x) + 1";

            // syntaxTree = SyntaxTree.Parse(line);

            // evaluation = new Evaluator(syntaxTree.Root, global);
            // result = evaluation.Evaluate();

            // if (Error.Wrong)
            // {
            //     Console.WriteLine(Error.Mssg);
            // }
            // else {
            //     Console.WriteLine(result);
            // }

            // line = "k(2)";

            // syntaxTree = SyntaxTree.Parse(line);

            // evaluation = new Evaluator(syntaxTree.Root, global);
            // result = evaluation.Evaluate();

            // if (Error.Wrong)
            // {
            //     Console.WriteLine(Error.Mssg);
            // }
            // else {
            //     Console.WriteLine(result);
            // }
        }    
    }
}