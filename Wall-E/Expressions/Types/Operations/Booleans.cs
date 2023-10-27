namespace WallE;

// Clases que evalúan los tokens Booleanos
public class And : Boolean 
{
    public static string Eval(string leftSide, string rightSide) {

        if (rightSide == "" || leftSide == "") return "";

        result = (!defaultValues.Contains(leftSide) && !defaultValues.Contains(rightSide)).ToString();
        result = bool.Parse(result)? "1" : "0";
        return result;
    }
}

public class Or : Boolean
{
    public static string Eval(string leftSide, string rightSide) {

        if (rightSide == "" || leftSide == "") return "";

        result = (!defaultValues.Contains(leftSide) || !defaultValues.Contains(rightSide)).ToString();
        result = bool.Parse(result)? "1" : "0";
        return result;
    }
}

public class Not : Boolean
{
    public new static string Eval(string val) {

        if (val == "") return "";

        result = defaultValues.Contains(val).ToString();
        result = bool.Parse(result)? "1" : "0";
        return result;
    }
}

public class Equal : Boolean
{
    public static string Eval(string leftSide, string rightSide) {

        if (rightSide == "" || leftSide == "") return "";
        
        result = (leftSide == rightSide).ToString();
        result = bool.Parse(result)? "1" : "0";
        return result;
    }
}

public class NotEqual : Boolean
{
    public static string Eval(string leftSide, string rightSide) {

        if (rightSide == "" || leftSide == "") return "";
        
        result = (leftSide != rightSide).ToString();
        result = bool.Parse(result)? "1" : "0";
        return result;
    }
}

public class GreatThan : Boolean
{
    public static string Eval(string leftSide, string rightSide) {

        if (rightSide == "" || leftSide == "") return "";

        result = (double.Parse(leftSide) > double.Parse(rightSide)).ToString();
        result = bool.Parse(result)? "1" : "0";
        return result;
    }
}

public class GreatEqual : Boolean
{
    public static string Eval(string leftSide, string rightSide) {

        if (rightSide == "" || leftSide == "") return "";

        result = (double.Parse(leftSide) >= double.Parse(rightSide)).ToString();
        result = bool.Parse(result)? "1" : "0";
        return result;
    }
}

public class LessThan : Boolean
{
    public static string Eval(string leftSide, string rightSide) {

        if (rightSide == "" || leftSide == "") return "";

        result = (double.Parse(leftSide) < double.Parse(rightSide)).ToString();
        result = bool.Parse(result)? "1" : "0";
        return result;
    }
}

public class LessEqual : Boolean
{
    public static string Eval(string leftSide, string rightSide) {

        if (rightSide == "" || leftSide == "") return "";

        result = (double.Parse(leftSide) <= double.Parse(rightSide)).ToString();
        result = bool.Parse(result)? "1" : "0";
        return result;
    }
}


