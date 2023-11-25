namespace G_Sharp;

public static class Error
{
    public static bool Wrong = false;
    public static string Mssg = "";
    public static string TypeMssg = "";

    public static void SetError(string type, string mssg)
    {
        if (Wrong) return;
        Wrong = true;
        Mssg = mssg;
        TypeMssg = type;
    }

    public static void ResetError()
    {
        Wrong = false;
        Mssg = "";
        TypeMssg = "";
    }
}