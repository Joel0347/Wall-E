namespace G_Sharp;

public static class Error
{
    public static bool Wrong = false;
    public static string Mssg = "";

    public static void SetError(string mssg)
    {
        if (Wrong) return;
        Wrong = true;
        Mssg = mssg;
    }

    public static void ResetError()
    {
        Wrong = false;
        Mssg = "";
    }
}