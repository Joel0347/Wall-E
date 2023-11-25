namespace WallE
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Grapher = new System.Windows.Forms.PictureBox();
            this.Input = new System.Windows.Forms.TextBox();
            this.Compile = new System.Windows.Forms.Button();
            this.Save = new System.Windows.Forms.Button();
            this.Clear = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Zoom = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.View_Files = new System.Windows.Forms.Button();
            this.MoveUp = new System.Windows.Forms.Button();
            this.MoveDown = new System.Windows.Forms.Button();
            this.MoveRight = new System.Windows.Forms.Button();
            this.MoveLeft = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Grapher)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Zoom)).BeginInit();
            this.SuspendLayout();
            // 
            // Grapher
            // 
            this.Grapher.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Grapher.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Grapher.InitialImage = null;
            this.Grapher.Location = new System.Drawing.Point(627, 93);
            this.Grapher.Name = "Grapher";
            this.Grapher.Size = new System.Drawing.Size(1227, 883);
            this.Grapher.TabIndex = 0;
            this.Grapher.TabStop = false;
            // 
            // Input
            // 
            this.Input.Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Input.Location = new System.Drawing.Point(5, 295);
            this.Input.Multiline = true;
            this.Input.Name = "Input";
            this.Input.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Input.Size = new System.Drawing.Size(616, 638);
            this.Input.TabIndex = 1;
            // 
            // Compile
            // 
            this.Compile.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.Compile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Compile.Font = new System.Drawing.Font("Magneto", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point);
            this.Compile.Location = new System.Drawing.Point(511, 939);
            this.Compile.Name = "Compile";
            this.Compile.Size = new System.Drawing.Size(110, 37);
            this.Compile.TabIndex = 2;
            this.Compile.Text = "Compile";
            this.Compile.UseVisualStyleBackColor = true;
            this.Compile.Click += new System.EventHandler(this.Compile_Click);
            // 
            // Save
            // 
            this.Save.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Save.Font = new System.Drawing.Font("Magneto", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Save.Location = new System.Drawing.Point(391, 939);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(96, 37);
            this.Save.TabIndex = 3;
            this.Save.Text = " Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // Clear
            // 
            this.Clear.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.Clear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Clear.Font = new System.Drawing.Font("Magneto", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Clear.Location = new System.Drawing.Point(267, 939);
            this.Clear.Name = "Clear";
            this.Clear.Size = new System.Drawing.Size(96, 37);
            this.Clear.TabIndex = 4;
            this.Clear.Text = "Clear";
            this.Clear.UseVisualStyleBackColor = true;
            this.Clear.Click += new System.EventHandler(this.Clear_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::WallE.Properties.Resources.OIP__1_;
            this.pictureBox1.Location = new System.Drawing.Point(5, 1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(616, 288);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // Zoom
            // 
            this.Zoom.Location = new System.Drawing.Point(1009, 18);
            this.Zoom.Name = "Zoom";
            this.Zoom.Size = new System.Drawing.Size(549, 56);
            this.Zoom.TabIndex = 6;
            this.Zoom.Scroll += new System.EventHandler(this.Zoom_Scroll);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Monotype Corsiva", 13.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(853, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 39);
            this.label1.TabIndex = 7;
            this.label1.Text = "Zoom";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.MidnightBlue;
            this.label2.Font = new System.Drawing.Font("Monotype Corsiva", 16.2F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point);
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Image = global::WallE.Properties.Resources.OIP__1_;
            this.label2.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.label2.Location = new System.Drawing.Point(5, 254);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(136, 35);
            this.label2.TabIndex = 8;
            this.label2.Text = "Code Here:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // View_Files
            // 
            this.View_Files.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.View_Files.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.View_Files.Font = new System.Drawing.Font("Magneto", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.View_Files.Location = new System.Drawing.Point(5, 939);
            this.View_Files.Name = "View_Files";
            this.View_Files.Size = new System.Drawing.Size(108, 37);
            this.View_Files.TabIndex = 9;
            this.View_Files.Text = "View Files";
            this.View_Files.UseVisualStyleBackColor = true;
            this.View_Files.Click += new System.EventHandler(this.View_Files_Click);
            // 
            // MoveUp
            // 
            this.MoveUp.AutoSize = true;
            this.MoveUp.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.MoveUp.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.MoveUp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.MoveUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MoveUp.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.MoveUp.Location = new System.Drawing.Point(682, 5);
            this.MoveUp.Name = "MoveUp";
            this.MoveUp.Size = new System.Drawing.Size(52, 37);
            this.MoveUp.TabIndex = 10;
            this.MoveUp.Text = " ↑";
            this.MoveUp.UseVisualStyleBackColor = true;
            this.MoveUp.Click += new System.EventHandler(this.MoveUp_Click);
            // 
            // MoveDown
            // 
            this.MoveDown.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.MoveDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MoveDown.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.MoveDown.Location = new System.Drawing.Point(682, 45);
            this.MoveDown.Name = "MoveDown";
            this.MoveDown.Size = new System.Drawing.Size(52, 34);
            this.MoveDown.TabIndex = 11;
            this.MoveDown.Text = " ↓";
            this.MoveDown.UseVisualStyleBackColor = true;
            this.MoveDown.Click += new System.EventHandler(this.MoveDown_Click);
            // 
            // MoveRight
            // 
            this.MoveRight.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.MoveRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MoveRight.Font = new System.Drawing.Font("Segoe UI", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.MoveRight.Location = new System.Drawing.Point(740, 5);
            this.MoveRight.Name = "MoveRight";
            this.MoveRight.Size = new System.Drawing.Size(49, 74);
            this.MoveRight.TabIndex = 12;
            this.MoveRight.Text = "→";
            this.MoveRight.UseVisualStyleBackColor = true;
            this.MoveRight.Click += new System.EventHandler(this.MoveRight_Click);
            // 
            // MoveLeft
            // 
            this.MoveLeft.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.MoveLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MoveLeft.Font = new System.Drawing.Font("Segoe UI", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.MoveLeft.Location = new System.Drawing.Point(627, 5);
            this.MoveLeft.Name = "MoveLeft";
            this.MoveLeft.Size = new System.Drawing.Size(49, 74);
            this.MoveLeft.TabIndex = 13;
            this.MoveLeft.Text = "←";
            this.MoveLeft.UseVisualStyleBackColor = true;
            this.MoveLeft.Click += new System.EventHandler(this.MoveLeft_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(1866, 1043);
            this.Controls.Add(this.MoveLeft);
            this.Controls.Add(this.MoveRight);
            this.Controls.Add(this.MoveDown);
            this.Controls.Add(this.MoveUp);
            this.Controls.Add(this.View_Files);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.Clear);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.Compile);
            this.Controls.Add(this.Input);
            this.Controls.Add(this.Grapher);
            this.Controls.Add(this.Zoom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "GeoWallE";
            ((System.ComponentModel.ISupportInitialize)(this.Grapher)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Zoom)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PictureBox Grapher;
        private TextBox Input;
        private Button Compile;
        private Button Save;
        private Button Clear;
        private PictureBox pictureBox1;
        private TrackBar Zoom;
        private Label label1;
        private Label label2;
        private Button View_Files;
        private Button MoveUp;
        private Button MoveDown;
        private Button MoveRight;
        private Button MoveLeft;
    }
}