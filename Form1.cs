using System.Drawing;

namespace WallE
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Compile_Click(object sender, EventArgs e)
        {
            Graphics graphic = Grapher.CreateGraphics();
            graphic.Clear(Color.White);
            Color[] colors = 
            {
                Color.Blue, Color.Red, Color.Yellow, Color.Green, Color.Cyan, 
                Color.Magenta, Color.White,  Color.Gray, Color.Black 
            };

            string input = Input.Text;
            char[] splits = { '\r', '\n' };
            string[] mssgs = input.Split(splits, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < mssgs.Length; i++)
            {
                Main_Grapher.Parsing(mssgs[i]);
                if (Data.IsDraw(mssgs[i]))
                {
                    (string, string) data = Data.Draw(mssgs[i]);
                    string type = data.Item1;

                    if (type == "point")
                    {
                        Random random = new();
                        Brush brush = new SolidBrush(colors[8]);
                        graphic.FillEllipse(brush, random.Next(400), random.Next(400), 10, 10);  // Para pintar un punto 
                    }
                }
            }



            //Pen pen = new(colors[8]);

            //// Fill rellena las figuras y Dra
            ////graphic.DrawEllipse(pen, random.Next(200), random.Next(200), random.Next(200), random.Next(200));  // Para pintar un punto 


            //Point point1 = new()
            //{
            //    X = random.Next(500),
            //    Y = random.Next(500)
            //};
            //Point point2 = new()
            //{
            //    X = random.Next(500),
            //    Y = random.Next(500)
            //};
            //Point point3 = new()
            //{
            //    X = random.Next(500),
            //    Y = random.Next(500)
            //};
            //Point point4 = new()
            //{
            //    X = random.Next(500),
            //    Y = random.Next(500)
            //};
            //Point[] points = { point1, point2, point3, point4};
            //Rectangle rectangle = new(random.Next(500), random.Next(500), random.Next(500), random.Next(500));

            ////graphic.DrawLine(pen, point1, point2);
            //graphic.FillRectangle(brush, rectangle);

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