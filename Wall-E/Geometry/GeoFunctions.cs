using System.Runtime.InteropServices;

namespace WallE;

public class GeoFunction
{
    public static string[] functions = {
        "line(", "segment(", "ray(", "arc(", "circle(", "measure(", 
        "intersect(", "count(", "randoms(", "points(", "samples("
    };

    public static bool IsGeoFunction(string name) {
        return functions.Contains(name);
    }

    public static bool Revision(string name, string[] args) {

        if ((args.Length > 1 && args.Any(string.IsNullOrWhiteSpace)) ||
            (args.Length == 1 && string.IsNullOrWhiteSpace(args[0]) && name != "samples("))
        {
            Check.SetErrors("SEMANTIC", $"Invalid empty argument in '{name[..^1]}' function");
        }

        else if (name == "samples(" && (args.Length != 1 || !string.IsNullOrWhiteSpace(args[0]))) {
            Check.SetErrors("SEMANTIC", $"Function 'samples' doesn't receive any argument, but {args.Length} were given");
        }

        else if (args.Length == 1 && name != "samples(" && name != "points(") {
            int count = Cache.inputType[name].Count;
            Check.SetErrors("SEMANTIC", $"Function '{name[..^1]}' receives {count} arguments, but 1 was given");
        }

        else if (name == "points(" && args.Length != 1) {
            Check.SetErrors("SEMANTIC", $"Function 'points' receives 1 argument, but {args.Length} were given");
        }

        else if (args.Length > 2 && name != "arc(") {
            int count = Cache.inputType[name].Count;
            Check.SetErrors("SEMANTIC", $"Function '{name[..^1]}' receives {count} arguments, but {args.Length} were given");
        }

        else if (name == "arc(" && args.Length != 4) {
            Check.SetErrors("SEMANTIC", $"Function 'arc' receives 4 arguments, but {args.Length} were given");
        }

         // Se revisa que los argumentos se correspondan con el tipo de entrada de la función
        else if (args.Any(x => Types.GetType(x) != Cache.inputType[name][Array.IndexOf(args, x)] && 
            Cache.inputType[name][Array.IndexOf(args, x)] != "all")) 
        {
            string not = args.First(x => Types.GetType(x) != Cache.inputType[name][Array.IndexOf(args, x)]);
            string type = Cache.inputType[name][Array.IndexOf(args, not)];
            not = Types.GetType(not);
            string mssg = $"Function '{name[..^1]}' receives '{type}', not '{not}'";
            Check.SetErrors("SEMANTIC", mssg);
        }

        else return true;

        return false;
    }
    public static Draw Eval(string name, string[] args) {
        if (name == "line(")      return new Line(args[0], args[1]);
        if (name == "segment(")   return new Segment(args[0], args[1]); 
        if (name == "ray(")       return new Ray(args[0], args[1]); 
        if (name == "circle(")    return new Circle(args[0], args[1]); 
        if (name == "measure(")   return new Measure(args[0], args[1]); 
        //if (name == "intersect(") return new (args[0], args[1]); 
        if (name == "arc(")       return new Arc(args[0], args[1], args[2], args[3]);

        return "";
    }
}