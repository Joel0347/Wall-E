namespace WallE;

public class Save 
{
    public static void WriteDocument(string text, string name) {
        name = name.Trim();
        
        string[] filePaths = Directory.GetFiles(Path.Join("..", "Files"));
        string[] fileNames = new string[filePaths.Length];
        
        for (int i = 0; i < filePaths.Length; i++)
        {
            fileNames[i] = Path.GetFileNameWithoutExtension(filePaths[i]);
        }

        if (fileNames.Contains(name)) {
            //error
            // !COMPILE ERROR: File "name" already exists at the library. Try to change the name.
        }

        else {
            string path = Path.Join("..", $"Files\\{name}.txt");
            StreamWriter writer = new(path);
            writer.Write(text);
            writer.Close();
        }
    }
}