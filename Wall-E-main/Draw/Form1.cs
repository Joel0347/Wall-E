using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using G_Sharp;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Emit;
using G_Sharp;

namespace WallE
{
    public partial class Form1 : Form
    {
        private bool enabledRun = false;
        public static Graphics? graphic;
        public static List<string>? DirectoriesOfFiles = new();
        private static List<(Figure, Color, string)> Geometries = new();

        public Form1()
        {
            InitializeComponent();
        }

        #region Compilar y correr
        private void Compile_Click(object sender, EventArgs e)
        {
            //enabledRun = false;
            graphic = Grapher.CreateGraphics();
            graphic.Clear(Color.White);
            Geometries = new();
            MessageBoxButtons messageBoxButtons = MessageBoxButtons.RetryCancel;
            Colors.InitializeColor();

            Dictionary<string, Constant> constants = new();
            Dictionary<string, Function> functions = new();

            Scope global = new(constants, functions);

            Error.ResetError();

            string line = Input.Text.Replace("\t", " ");

            if (string.IsNullOrWhiteSpace(line))
                return;

            //try
            //{
            var syntaxTree = SyntaxTree.Parse(line);
            List<object> obj = new();

            foreach (var root in syntaxTree.Root)
            {
                var checking = global.Check(root);
                if (!checking) break;

                var result = global.Evaluate(root);

                if (result is List<object> list)
                    obj.AddRange(list);

                else obj.Add(result);
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
                    Input.Text = result.ToString();
                }

                MessageBoxButtons messageBoxButtons1 = MessageBoxButtons.OK;

                MessageBox.Show("You can run the code now", "Compile done",
                messageBoxButtons1, MessageBoxIcon.Information);
            }
            //}

            //catch (Exception)
            //{
            //DialogResult result2 = MessageBox.Show("Exception not detected", "!!COMPILE ERROR",
            //    messageBoxButtons, MessageBoxIcon.Error);

            //if (result2 == DialogResult.Retry)
            //{
            //    graphic.Clear(Color.White);
            //    Input.Clear();
            //    return;
            //}
            //}

        }

        private void Run_Click(object sender, EventArgs e)
        {
            if (!enabledRun)
            {
                MessageBoxButtons messageBoxButtons = MessageBoxButtons.OK;

                MessageBox.Show("You must compile the code before running it", "!!RUNTIME ERROR",
                        messageBoxButtons, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return;
            }

            MethodsDrawing.DrawFigure(Geometries, graphic!);
            enabledRun = false;
        }

        #endregion

        #region Resetear el textbox y el picturebox
        private void Clear_Click(object sender, EventArgs e)
        {
            Graphics graphic = Grapher.CreateGraphics();
            graphic.Clear(Color.White);
            Input.Clear();
        }
        #endregion

        #region Botones de movimiento
        private void MoveRight_Click(object sender, EventArgs e)
        {
            if (Geometries is not null && Geometries.Count > 0)
            {
                graphic!.TranslateTransform(-50, 0);

                graphic.Clear(Color.White);

                MethodsDrawing.DrawFigure(Geometries, graphic);
            }
        }


        private void MoveLeft_Click(object sender, EventArgs e)
        {
            if (Geometries is not null && Geometries.Count > 0)
            {
                graphic!.TranslateTransform(50, 0);

                graphic.Clear(Color.White);

                MethodsDrawing.DrawFigure(Geometries, graphic);
            }
        }


        private void MoveUp_Click(object sender, EventArgs e)
        {
            if (Geometries is not null && Geometries.Count > 0)
            {
                graphic!.TranslateTransform(0, 50);

                graphic.Clear(Color.White);

                MethodsDrawing.DrawFigure(Geometries, graphic);
            }
        }

        private void MoveDown_Click(object sender, EventArgs e)
        {
            if (Geometries is not null && Geometries.Count > 0)
            {
                graphic!.TranslateTransform(0, -50);

                graphic.Clear(Color.White);

                MethodsDrawing.DrawFigure(Geometries, graphic);
            }
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            if (Geometries is not null && Geometries.Count > 0)
            {
                graphic!.ResetTransform();

                graphic.Clear(Color.White);

                MethodsDrawing.DrawFigure(Geometries, graphic);
            }
        }

        private void ZoomPlus_Click(object sender, EventArgs e)
        {
            if (Geometries is not null && Geometries.Count > 0)
            {
                graphic!.ScaleTransform(1.2F, 1.2F);

                graphic.Clear(Color.White);

                MethodsDrawing.DrawFigure(Geometries, graphic);
            }
        }

        private void ZoomMinus_Click(object sender, EventArgs e)
        {
            if (Geometries is not null && Geometries.Count > 0)
            {
                graphic!.ScaleTransform(0.8F, 0.8F);

                graphic.Clear(Color.White);

                MethodsDrawing.DrawFigure(Geometries, graphic);
            }
        }
        #endregion

        #region Salvar y ver documentos creados

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
                List<string> paths = saveFileDialog.FileNames.ToList();
                DirectoriesOfFiles!.AddRange(paths);
                Stream file = saveFileDialog.OpenFile();
                StreamWriter writer = new(file);
                writer.Write(Input.Text);
                writer.Close();
            }
        }
        private void View_Files_Click(object sender, EventArgs e)
        {
            string path = Path.Join("..", Path.Join("..", Path.Join("..", "Files")));
            List<string> filePaths = Directory.GetFiles(path).ToList();
            MessageBoxButtons messageBoxButtons = MessageBoxButtons.OK;

            filePaths.AddRange(DirectoriesOfFiles!);

            string files = string.Join("\n", filePaths);

            if (filePaths.Count > 0)
            {

                MessageBox.Show(files, "File paths", messageBoxButtons, MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1);
            }
            else
            {
                MessageBox.Show("No files have been created", "File names", messageBoxButtons, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);
            }
            return;
        }
        #endregion

        #region Regresar a la ventana principal
        private void GoBack_Click(object sender, EventArgs e)
        {
            MessageBoxButtons messageBoxButtons = MessageBoxButtons.YesNo;

            if (Input.Text.Trim() == "")
            {
                DialogResult result1 = MessageBox.Show("Are you sure?", "You are returning to the main menu",
                    messageBoxButtons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (result1 == DialogResult.Yes)
                {
                    this.Close();
                }
            }
            else
            {
                DialogResult result1 = MessageBox.Show("If you return now, you will lose the data. Do you want to save before returning?",
                    "You are returning to the main menu", messageBoxButtons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

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
        #endregion
    }
}