namespace WallE;
public class Program
{
   private static void Main(string[] args)
   {
       Extra.Visualization();

    //    while(true) {
           Extra.Reset();
           string[] filePaths = Directory.GetFiles(Path.Join("..", "Content"));
           string expression = File.ReadAllText(filePaths[0]);
            
           Principal.Analize(expression);

        //    if (result != "") {
        //        if (result.StartsWith("\"")) {
        //            result = String.SlashEval(result);
        //            result = result[(result.IndexOf("\"") + 1)..result.LastIndexOf("\"")];
        //        }
                
        //        Console.WriteLine(result);
        //    }                 
    //    }
   }
}