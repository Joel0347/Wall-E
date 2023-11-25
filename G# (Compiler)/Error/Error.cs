namespace G_Sharp;

public static class Error
{
    public static bool Wrong = false;
    public static string Msg = "";

    public static void SetError(string msg)
    {
        if (Wrong) return;
        Wrong = true;
        Msg = msg;
    }

    public static void ResetError()
    {
        Wrong = false;
        Msg = "";
    }
}