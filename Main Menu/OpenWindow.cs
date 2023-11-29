using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WallE
{
    public partial class OpenWindow : Form
    {
        public OpenWindow()
        {
            InitializeComponent();
        }

        private void GraphicWindow_Click(object sender, EventArgs e)
        {
            var drawingWindow = new Form1();

            this.Hide();
            drawingWindow.ShowDialog();

            this.Show();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            MessageBoxButtons messageBoxButtons = MessageBoxButtons.YesNo;

            DialogResult result1 = MessageBox.Show("Are you sure?", "You are exiting the application",
                    messageBoxButtons, MessageBoxIcon.Question);

            if (result1 == DialogResult.Yes)
            {
                this.Close();
            }
        }

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

        private void MoreAbout_Click(object sender, EventArgs e)
        {
            MessageBoxButtons messageBoxButtons = MessageBoxButtons.OK;

            string files = "To know more about the G# language and the operation of the " +
                "commands and functions, it is recommended to read the report that is contained " +
                "in the download file with the name \"GeoWall-E Report\". \nEnjoy and draw!!";

            MessageBox.Show(files, "More about...", messageBoxButtons);
        }
    }
}
