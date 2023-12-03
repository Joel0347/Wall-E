using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security.Policy;

namespace WallE
{
    public partial class OpenWindow : Form
    {
        public OpenWindow()
        {
            InitializeComponent();
        }

        #region Enlace con el graficador
        private void GraphicWindow_Click(object sender, EventArgs e)
        {
            var drawingWindow = new Form1();

            this.Hide();
            drawingWindow.ShowDialog();

            this.Show();
        }
        #endregion

        #region Muestra los posibles comandos que puedes utilizar
        private void AllCommands_Click(object sender, EventArgs e)
        {
            List<string> commands = new()
            {
                "point <id>", "line <id>", "segment <id>", "ray <id>", "circle <id>", "point sequency",
                "line sequency <id>", "segment sequency <id>", "ray sequency <id>", "circle sequency <id>",
                "color <string>", "restore", "import <string>", "draw <exp> <string>", "line(p1, p2)",
                "segment(p1, p2)", "ray(p1, p2)", "arc(p1, p2, p3, m)", "circle(p, m)", "measure(p1, p2)",
                "intersect(f1, f2)", "count(s)", "randoms()", "points(f)", "samples()"
            };

            MessageBoxButtons messageBoxButtons = MessageBoxButtons.OK;

            string files = string.Join("\n", commands);

            MessageBox.Show(files, "All commands and functions you can use",
                                                   messageBoxButtons, MessageBoxIcon.Information);
        }
        #endregion

        #region Abre el informe del proyecto
        private void MoreAbout_Click(object sender, EventArgs e)
        {
            Process process = new();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = Path.Join("..", Path.Join("..", Path.Join("..", "Report\\GeoWall-E.pdf")));
            process.Start();
        }
        #endregion

        #region Salir de la aplicación
        private void Exit_Click(object sender, EventArgs e)
        {
            MessageBoxButtons messageBoxButtons = MessageBoxButtons.YesNo;

            DialogResult result1 = MessageBox.Show("Are you sure?", "You are exiting the application",
                    messageBoxButtons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (result1 == DialogResult.Yes)
            {
                this.Close();
            }
        }
        #endregion
    }
}
