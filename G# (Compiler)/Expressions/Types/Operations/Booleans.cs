namespace WallE;

// Clases que evalúan los tokens Booleanos
public class And : Boolean 
{
    public static string Eval(string leftSide, string rightSide) {

        if (rightSide == "" || leftSide == "") return "";

        result = (bool.Parse(leftSide) && bool.Parse(rightSide)).ToString();

        return result;
    }
}

public class Or : Boolean
{
    public static string Eval(string leftSide, string rightSide) {

        if (rightSide == "" || leftSide == "") return "";

        result = (bool.Parse(leftSide) || bool.Parse(rightSide)).ToString();

        return result;
    }
}

public class Not : Boolean
{
    public new static string Eval(string val) {

        if (val == "") return "";

        result = (!bool.Parse(val)).ToString();

        return result;
    }
}

public class Equal : Boolean
{
    public static string Eval(string leftSide, string rightSide) {

        if (rightSide == "" || leftSide == "") return "";
        
        result = (leftSide == rightSide).ToString();

        return result;
    }
}

public class NotEqual : Boolean
{
    public static string Eval(string leftSide, string rightSide) {

        if (rightSide == "" || leftSide == "") return "";
        
        result = (leftSide != rightSide).ToString();

        return result;
    }
}

public class GreatThan : Boolean
{
    public static string Eval(string leftSide, string rightSide) {

        if (rightSide == "" || leftSide == "") return "";

        result = (double.Parse(leftSide) > double.Parse(rightSide)).ToString();

        return result;
    }
}

public class GreatEqual : Boolean
{
    public static string Eval(string leftSide, string rightSide) {

        if (rightSide == "" || leftSide == "") return "";

        result = (double.Parse(leftSide) >= double.Parse(rightSide)).ToString();

        return result;
    }
}

public class LessThan : Boolean
{
    public static string Eval(string leftSide, string rightSide) {

        if (rightSide == "" || leftSide == "") return "";

        result = (double.Parse(leftSide) < double.Parse(rightSide)).ToString();

        return result;
    }
}

public class LessEqual : Boolean
{
    public static string Eval(string leftSide, string rightSide) {

        if (rightSide == "" || leftSide == "") return "";

        result = (double.Parse(leftSide) <= double.Parse(rightSide)).ToString();

        return result;
    }
}


