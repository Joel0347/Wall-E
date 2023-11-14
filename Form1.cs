using System.Drawing;
using System.Text.RegularExpressions;

namespace WallE
{
    public partial class Form1 : Form
    {
        public static string Mssg { private get; set; }
        public static string TypeError { private get; set; }
        public static bool Error { get; set; }

        public Form1()
        {
            InitializeComponent();
        }

        private void Compile_Click(object sender, EventArgs e)
        {
            Graphics graphic = Grapher.CreateGraphics();
            graphic.Clear(Color.White);

            string s = Input.Text;
                   s = s.Replace("\r", " ");
                   s = s.Replace("\t", " ");
                   s = $" {s} ";

            string n = String.StringsToSpaces(s);
            string m = Regex.Replace(n, @"[^_""Ò—A-Za-z0-9]", " ");

            int letIndex = m.LastIndexOf(" let ");

            while (letIndex >= 0)
            {
                int skip = n.IndexOf("\n", letIndex);
                int inIndex = m.IndexOf(" in ", letIndex) + 1;

                if (inIndex == -1)
                {
                    Check.SetErrors("SYNTAX", "Missing token 'in' in 'let-in' expression");
                    return;
                }

                if (skip == -1) skip = s.Length;

                if (inIndex > skip)
                {
                    int newSkip = n.IndexOf("\n", skip + 1, inIndex - skip - 1);
                    while (newSkip >= 0)
                    {
                        s = s.Remove(newSkip, 1);
                        n = n.Remove(newSkip, 1);
                        s = s.Insert(newSkip, " ");
                        n = n.Insert(newSkip, " ");
                        newSkip = n.IndexOf("\n", skip + 1, inIndex - skip - 1);
                    }

                    s = s.Remove(skip, 1);
                    s = s.Insert(skip, " ");
                    n = n.Remove(skip, 1);
                    n = n.Insert(skip, " ");
                }

                s = s.Remove(inIndex, 1);
                s = s.Insert(inIndex, "I");
                n = n.Remove(inIndex, 1);
                n = n.Insert(inIndex, "I");
                m = Regex.Replace(n, @"[^_""Ò—A-Za-z0-9]", " ");
                letIndex = m[..letIndex].LastIndexOf(" let ");
            }

            List<string> instructions = s.Split("\n", StringSplitOptions.TrimEntries).ToList();
            instructions.RemoveAll(x => x == "");

            for (int i = 0; i < instructions.Count; i++)
            {
                if (instructions[i].StartsWith("draw "))
                {
                    instructions[i] = instructions[i][4..instructions[i].IndexOf(";")].Trim();
                    Cache.geometryValues[instructions[i]].Drawing(graphic);
                    continue;
                }

                Main_Grapher.Parsing(instructions[i]);

                if (Main.error)
                {
                    MessageBoxButtons messageBoxButtons = MessageBoxButtons.RetryCancel;

                    DialogResult result = MessageBox.Show(Mssg, $"!!{TypeError} ERROR: ",
                        messageBoxButtons, MessageBoxIcon.Error);

                    if (result == DialogResult.Retry)
                    {
                        graphic.Clear(Color.White);
                        Input.Clear();
                        return;
                    }
                }
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            Graphics graphic = Grapher.CreateGraphics();
            graphic.Clear(Color.White);
            Input.Clear();
        }
    }
}