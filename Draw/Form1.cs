using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using G_Sharp;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Emit;

namespace WallE
{
    public partial class Form1 : Form
    {
        private bool enabledRun = false;
        public static Graphics? graphic;
        public static List<string>? FileNames;
        private static List<(GeometrySyntax, Color)> Geometries = new();

        public Form1()
        {
            InitializeComponent();
        }

        private void Compile_Click(object sender, EventArgs e)
        {
            //enabledRun = false;
            graphic = Grapher.CreateGraphics();
            graphic.Clear(Color.White);
            Geometries = new();
            MessageBoxButtons messageBoxButtons = MessageBoxButtons.RetryCancel;

            SyntaxToken p1 = new(SyntaxKind.IdentifierToken, 0, "p1", null!);
            SyntaxToken p2 = new(SyntaxKind.IdentifierToken, 0, "p2", null!);
            SyntaxToken p3 = new(SyntaxKind.IdentifierToken, 0, "p3", null!);
            SyntaxToken m = new(SyntaxKind.IdentifierToken, 0, "m", null!);

            var body = new LiteralExpressionSyntax(p1);
            var point1 = new ConstantExpressionSyntax(p1);
            var point2 = new ConstantExpressionSyntax(p2);
            var point3 = new ConstantExpressionSyntax(p3);
            var measure = new ConstantExpressionSyntax(m);

            Dictionary<string, Constant> constants = new();
            Dictionary<string, Function> functions = new()
            {
                ["line"] = new Function(body, new List<ExpressionSyntax> { point1, point2 }),
                ["segment"] = new Function(body, new List<ExpressionSyntax> { point1, point2 }),
                ["ray"] = new Function(body, new List<ExpressionSyntax> { point1, point2 }),
                ["circle"] = new Function(body, new List<ExpressionSyntax> { point1, measure }),
                ["measure"] = new Function(body, new List<ExpressionSyntax> { point1, point2 }),
                ["arc"] = new Function(body, new List<ExpressionSyntax> { point1, point2, point3, measure }),
            };

            Scope global = new(constants, functions);

            Error.ResetError();

            string line = Input.Text.Replace("\t", " ");

            if (string.IsNullOrWhiteSpace(line))
                return;

            try
            {
                var syntaxTree = SyntaxTree.Parse(line);
                List<object> obj = new();

                foreach (var root in syntaxTree.Root)
                {
                    var semanticChecker = new SemanticChecker(root, global);
                    var checking = semanticChecker.Check();
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
                    enabledRun = true;

                    foreach (var result in obj)
                    {
                        if (result is Draw geometries)
                        {
                            Geometries.AddRange(geometries.Geometries);
                        }
                    }

                    MessageBoxButtons messageBoxButtons1 = MessageBoxButtons.OK;

                    MessageBox.Show("You can run the code now", "Compile done",
                    messageBoxButtons1, MessageBoxIcon.Information);
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

        private void GoBack_Click(object sender, EventArgs e)
        {
            MessageBoxButtons messageBoxButtons = MessageBoxButtons.YesNo;

            if (Input.Text.Trim() == "")
            {
                DialogResult result1 = MessageBox.Show("Are you sure?", "You are returning to the main menu",
                    messageBoxButtons, MessageBoxIcon.Question);

                if (result1 == DialogResult.Yes)
                {
                    this.Close();
                }
            }
            else
            {
                DialogResult result1 = MessageBox.Show("If you return now, you will lose the data. Do you want to save before returning?",
                    "You are returning to the main menu", messageBoxButtons, MessageBoxIcon.Question);

                if (result1 == DialogResult.Yes)
                {
                    Save_Click(sender, e);
                    this.Close();
                }
                else
                {
                    this.Close();
                }
            }
        }

        private void Run_Click(object sender, EventArgs e)
        {
            if (!enabledRun)
            {
                MessageBoxButtons messageBoxButtons = MessageBoxButtons.OK;

                MessageBox.Show("You must compile the code before running it", "!!RUNTIME ERROR",
                        messageBoxButtons, MessageBoxIcon.Warning);
                return;
            }
            MethodsDrawing.DrawFigure(Geometries, graphic!);
            enabledRun = false;
        }
    }
}