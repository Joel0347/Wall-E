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
            this.Grapher = new System.Windows.Forms.PictureBox();
            this.Input = new System.Windows.Forms.TextBox();
            this.Compile = new System.Windows.Forms.Button();
            this.Save = new System.Windows.Forms.Button();
            this.Clear = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Grapher)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // Grapher
            // 
            this.Grapher.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Grapher.InitialImage = null;
            this.Grapher.Location = new System.Drawing.Point(936, -1);
            this.Grapher.Name = "Grapher";
            this.Grapher.Size = new System.Drawing.Size(691, 499);
            this.Grapher.TabIndex = 0;
            this.Grapher.TabStop = false;
            // 
            // Input
            // 
            this.Input.Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Input.Location = new System.Drawing.Point(374, -1);
            this.Input.Multiline = true;
            this.Input.Name = "Input";
            this.Input.Size = new System.Drawing.Size(556, 499);
            this.Input.TabIndex = 1;
            // 
            // Compile
            // 
            this.Compile.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.Compile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Compile.Font = new System.Drawing.Font("Magneto", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point);
            this.Compile.Location = new System.Drawing.Point(807, 450);
            this.Compile.Name = "Compile";
            this.Compile.Size = new System.Drawing.Size(110, 37);
            this.Compile.TabIndex = 2;
            this.Compile.Text = "Compile";
            this.Compile.UseVisualStyleBackColor = true;
            this.Compile.Click += new System.EventHandler(this.Compile_Click);
            // 
            // Save
            // 
            this.Save.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Save.Font = new System.Drawing.Font("Magneto", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Save.Location = new System.Drawing.Point(693, 450);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(96, 37);
            this.Save.TabIndex = 3;
            this.Save.Text = " Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // Clear
            // 
            this.Clear.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.Clear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Clear.Font = new System.Drawing.Font("Magneto", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Clear.Location = new System.Drawing.Point(591, 450);
            this.Clear.Name = "Clear";
            this.Clear.Size = new System.Drawing.Size(96, 37);
            this.Clear.TabIndex = 4;
            this.Clear.Text = "Clear";
            this.Clear.UseVisualStyleBackColor = true;
            this.Clear.Click += new System.EventHandler(this.Clear_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackgroundImage = global::WallE.Properties.Resources.Screenshot__23_;
            this.pictureBox2.Location = new System.Drawing.Point(0, 1);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(377, 497);
            this.pictureBox2.TabIndex = 5;
            this.pictureBox2.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(1627, 633);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.Clear);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.Compile);
            this.Controls.Add(this.Input);
            this.Controls.Add(this.Grapher);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.Grapher)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PictureBox Grapher;
        private TextBox Input;
        private Button Compile;
        private Button Save;
        private Button Clear;
        private PictureBox pictureBox2;
    }
}