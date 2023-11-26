using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using G_Sharp;

namespace WallE
{
    public partial class Form1 : Form
    {  
        public static Graphics? graphic;
        public static List<string>? FileNames;
        private static List<(GeometrySyntax, Color)> Geometries = new();

        public Form1()
        {
            InitializeComponent();
        }

        private void Compile_Click(object sender, EventArgs e)
        {
            graphic = Grapher.CreateGraphics();
            graphic.Clear(Color.White);
            Geometries = new();

            string s = Input.Text;
                   //s = s.Replace("\r", " ");
                   s = s.Replace("\t", " ");
            //       s = $" {s} ";

            //string n = String.StringsToSpaces(s);
            //string m = Regex.Replace(n, @"[^_""Ò—A-Za-z0-9]", " ");

            //int letIndex = m.LastIndexOf(" let ");

            //while (letIndex >= 0)
            //{
            //    int skip = n.IndexOf("\n", letIndex);
            //    int inIndex = m.IndexOf(" in ", letIndex) + 1;

            //    if (inIndex == 0)
            //    {
            //        Check.SetErrors("SYNTAX", "Missing token 'in' in 'let-in' expression");
            //    }

            //    if (skip == -1) skip = s.Length;

            //    if (inIndex > skip)
            //    {
            //        int newSkip = n.IndexOf("\n", skip + 1, inIndex - skip - 1);
            //        while (newSkip >= 0)
            //        {
            //            s = s.Remove(newSkip, 1);
            //            n = n.Remove(newSkip, 1);
            //            s = s.Insert(newSkip, " ");
            //            n = n.Insert(newSkip, " ");
            //            newSkip = n.IndexOf("\n", skip + 1, inIndex - skip - 1);
            //        }

            //        s = s.Remove(skip, 1);
            //        s = s.Insert(skip, " ");
            //        n = n.Remove(skip, 1);
            //        n = n.Insert(skip, " ");
            //    }

            //    s = s.Remove(inIndex, 1);
            //    s = s.Insert(inIndex, "I");
            //    n = n.Remove(inIndex, 1);
            //    n = n.Insert(inIndex, "I");
            //    m = Regex.Replace(n, @"[^_""Ò—A-Za-z0-9]", " ");
            //    letIndex = m[..letIndex].LastIndexOf(" let ");
            //}

            List<string> instructions = s.Split("\n", StringSplitOptions.TrimEntries).ToList();
            instructions.RemoveAll(x => x == "");

            MessageBoxButtons messageBoxButtons = MessageBoxButtons.RetryCancel;

            Dictionary<string, Constant> constants = new();
            Dictionary<string, Function> functions = new();

            Scope global = new(constants, functions);

            Error.ResetError();

            string line = s;
                
            if (string.IsNullOrWhiteSpace(line))
                return;

            try
            {
                var syntaxTree = SyntaxTree.Parse(line);
                List<object> obj = new();

                foreach (var root in syntaxTree.Root)
                {
                    var evaluation = new Evaluator(root, global);
                    var result = evaluation.Evaluate();
                    obj.Add(result);
                }

                if (Error.Wrong)
                {
                    DialogResult result1 = MessageBox.Show(Error.Msg, $"!!{Error.TypeMsg} ERROR",
                        messageBoxButtons, MessageBoxIcon.Error);

                    if (result1 == DialogResult.Retry)
                    {
                        graphic.Clear(Color.White);
                        Input.Clear();
                        return;
                    }
                }

                else
                {
                    foreach (var result in obj)
                    {
                        if (result is List<(GeometrySyntax, Color)> geometries)
                        {
                            Geometries.AddRange(geometries);

                            MethodsDrawing.DrawFigure(geometries, graphic);
                        }
                        else
                        {
                            Input.Text = result.ToString();
                        }
                    }   
                }
            }

            catch (Exception)
            {
                DialogResult result1 = MessageBox.Show("Exception not detected", "!!COMPILE ERROR",
                    messageBoxButtons, MessageBoxIcon.Error);

                if (result1 == DialogResult.Retry)
                {
                    graphic.Clear(Color.White);
                    Input.Clear();
                    return;
                }
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                AddExtension = true,
                Filter = "txt files (*.txt) |*.txt",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream file = saveFileDialog.OpenFile();
                StreamWriter writer = new(file);
                writer.Write(Input.Text);
                writer.Close();
            }
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            Graphics graphic = Grapher.CreateGraphics();
            graphic.Clear(Color.White);
            Input.Clear();
        }

        private void MoveRight_Click(object sender, EventArgs e)
        {
            graphic!.TranslateTransform(-100, 0);

            graphic.Clear(Color.White);

            MethodsDrawing.DrawFigure(Geometries, graphic);
        }

        private void MoveLeft_Click(object sender, EventArgs e)
        {
            graphic!.TranslateTransform(100, 0);

            graphic.Clear(Color.White);

            MethodsDrawing.DrawFigure(Geometries, graphic);
        }
            

        private void MoveUp_Click(object sender, EventArgs e)
        {
            graphic!.TranslateTransform(0, 100);

            graphic.Clear(Color.White);

            MethodsDrawing.DrawFigure(Geometries, graphic);
        }

        private void MoveDown_Click(object sender, EventArgs e)
        {
            graphic!.TranslateTransform(0, -100);

            graphic.Clear(Color.White);

            MethodsDrawing.DrawFigure(Geometries, graphic);
        }

        private void View_Files_Click(object sender, EventArgs e)
        {
            string[] filePaths = Directory.GetFiles(Path.Join("..", Path.Join("..", Path.Join("..", "Files"))));
            string[] fileNames = new string[filePaths.Length];

            for (int i = 0; i < filePaths.Length; i++)
            {
                fileNames[i] = Path.GetFileNameWithoutExtension(filePaths[i]);
            }

            FileNames = fileNames.ToList();

            MessageBoxButtons messageBoxButtons = MessageBoxButtons.OK;

            string files = string.Join("\n", fileNames);
            if (filePaths.Length > 0)
            {
               MessageBox.Show(files, "File names", messageBoxButtons, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No files have been created", "File names", messageBoxButtons, MessageBoxIcon.Warning);
            }
            return;
        }

        private void Zoom_Scroll(object sender, EventArgs e)
        {
            
        }
    }
}