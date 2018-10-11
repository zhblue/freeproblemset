namespace EasyFPSViewer
{
    partial class SplitForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label_OutputDirectory = new System.Windows.Forms.Label();
            this.textBox_OutputDir = new System.Windows.Forms.TextBox();
            this.label_Prefix = new System.Windows.Forms.Label();
            this.textBox_Prefix = new System.Windows.Forms.TextBox();
            this.button_Split = new System.Windows.Forms.Button();
            this.button_Browse = new System.Windows.Forms.Button();
            this.label_Size = new System.Windows.Forms.Label();
            this.textBox_Size = new System.Windows.Forms.TextBox();
            this.label_MB = new System.Windows.Forms.Label();
            this.button_Help = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label_OutputDirectory
            // 
            this.label_OutputDirectory.AutoSize = true;
            this.label_OutputDirectory.Location = new System.Drawing.Point(22, 37);
            this.label_OutputDirectory.Name = "label_OutputDirectory";
            this.label_OutputDirectory.Size = new System.Drawing.Size(143, 15);
            this.label_OutputDirectory.TabIndex = 0;
            this.label_OutputDirectory.Text = "Output Directory:";
            // 
            // textBox_OutputDir
            // 
            this.textBox_OutputDir.Location = new System.Drawing.Point(172, 34);
            this.textBox_OutputDir.Name = "textBox_OutputDir";
            this.textBox_OutputDir.Size = new System.Drawing.Size(291, 25);
            this.textBox_OutputDir.TabIndex = 1;
            // 
            // label_Prefix
            // 
            this.label_Prefix.AutoSize = true;
            this.label_Prefix.Location = new System.Drawing.Point(62, 85);
            this.label_Prefix.Name = "label_Prefix";
            this.label_Prefix.Size = new System.Drawing.Size(103, 15);
            this.label_Prefix.TabIndex = 2;
            this.label_Prefix.Text = "File Prefix:";
            // 
            // textBox_Prefix
            // 
            this.textBox_Prefix.Location = new System.Drawing.Point(172, 82);
            this.textBox_Prefix.Name = "textBox_Prefix";
            this.textBox_Prefix.Size = new System.Drawing.Size(100, 25);
            this.textBox_Prefix.TabIndex = 3;
            this.textBox_Prefix.Text = "FPS_";
            // 
            // button_Split
            // 
            this.button_Split.Location = new System.Drawing.Point(118, 137);
            this.button_Split.Name = "button_Split";
            this.button_Split.Size = new System.Drawing.Size(313, 62);
            this.button_Split.TabIndex = 4;
            this.button_Split.Text = "Split";
            this.button_Split.UseVisualStyleBackColor = true;
            this.button_Split.Click += new System.EventHandler(this.button_Split_Click);
            // 
            // button_Browse
            // 
            this.button_Browse.Location = new System.Drawing.Point(471, 34);
            this.button_Browse.Name = "button_Browse";
            this.button_Browse.Size = new System.Drawing.Size(75, 26);
            this.button_Browse.TabIndex = 5;
            this.button_Browse.Text = "Browse";
            this.button_Browse.UseVisualStyleBackColor = true;
            this.button_Browse.Click += new System.EventHandler(this.button_Browse_Click);
            // 
            // label_Size
            // 
            this.label_Size.AutoSize = true;
            this.label_Size.Location = new System.Drawing.Point(303, 85);
            this.label_Size.Name = "label_Size";
            this.label_Size.Size = new System.Drawing.Size(47, 15);
            this.label_Size.TabIndex = 6;
            this.label_Size.Text = "Size:";
            // 
            // textBox_Size
            // 
            this.textBox_Size.Location = new System.Drawing.Point(356, 82);
            this.textBox_Size.Name = "textBox_Size";
            this.textBox_Size.Size = new System.Drawing.Size(75, 25);
            this.textBox_Size.TabIndex = 7;
            this.textBox_Size.Text = "5";
            // 
            // label_MB
            // 
            this.label_MB.AutoSize = true;
            this.label_MB.Location = new System.Drawing.Point(440, 85);
            this.label_MB.Name = "label_MB";
            this.label_MB.Size = new System.Drawing.Size(23, 15);
            this.label_MB.TabIndex = 8;
            this.label_MB.Text = "mb";
            // 
            // button_Help
            // 
            this.button_Help.Location = new System.Drawing.Point(471, 151);
            this.button_Help.Name = "button_Help";
            this.button_Help.Size = new System.Drawing.Size(60, 35);
            this.button_Help.TabIndex = 9;
            this.button_Help.Text = "Help?";
            this.button_Help.UseVisualStyleBackColor = true;
            this.button_Help.Click += new System.EventHandler(this.button_Help_Click);
            // 
            // SplitForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(558, 222);
            this.Controls.Add(this.button_Help);
            this.Controls.Add(this.label_MB);
            this.Controls.Add(this.textBox_Size);
            this.Controls.Add(this.label_Size);
            this.Controls.Add(this.button_Browse);
            this.Controls.Add(this.button_Split);
            this.Controls.Add(this.textBox_Prefix);
            this.Controls.Add(this.label_Prefix);
            this.Controls.Add(this.textBox_OutputDir);
            this.Controls.Add(this.label_OutputDirectory);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "SplitForm";
            this.Text = "Split";
            this.Load += new System.EventHandler(this.SplitForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_OutputDirectory;
        private System.Windows.Forms.TextBox textBox_OutputDir;
        private System.Windows.Forms.Label label_Prefix;
        private System.Windows.Forms.TextBox textBox_Prefix;
        private System.Windows.Forms.Button button_Split;
        private System.Windows.Forms.Button button_Browse;
        private System.Windows.Forms.Label label_Size;
        private System.Windows.Forms.TextBox textBox_Size;
        private System.Windows.Forms.Label label_MB;
        private System.Windows.Forms.Button button_Help;
    }
}