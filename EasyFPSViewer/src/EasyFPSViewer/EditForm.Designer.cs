namespace EasyFPSViewer
{
    partial class EditForm
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
            this.label_Title = new System.Windows.Forms.Label();
            this.button_Save = new System.Windows.Forms.Button();
            this.textBox_Title = new System.Windows.Forms.TextBox();
            this.label_Description = new System.Windows.Forms.Label();
            this.textBox_Description = new System.Windows.Forms.TextBox();
            this.label_Input = new System.Windows.Forms.Label();
            this.label_Output = new System.Windows.Forms.Label();
            this.label_SampleInput = new System.Windows.Forms.Label();
            this.textBox_Input = new System.Windows.Forms.TextBox();
            this.textBox_Output = new System.Windows.Forms.TextBox();
            this.label_SampleOutput = new System.Windows.Forms.Label();
            this.textBox_SampleInput = new System.Windows.Forms.TextBox();
            this.textBox_SampleOutput = new System.Windows.Forms.TextBox();
            this.textBox_Hint = new System.Windows.Forms.TextBox();
            this.label_Hint = new System.Windows.Forms.Label();
            this.label_Source = new System.Windows.Forms.Label();
            this.textBox_Source = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label_Title
            // 
            this.label_Title.AutoSize = true;
            this.label_Title.Location = new System.Drawing.Point(48, 19);
            this.label_Title.Name = "label_Title";
            this.label_Title.Size = new System.Drawing.Size(55, 15);
            this.label_Title.TabIndex = 1;
            this.label_Title.Text = "Title:";
            // 
            // button_Save
            // 
            this.button_Save.Location = new System.Drawing.Point(204, 639);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(250, 40);
            this.button_Save.TabIndex = 2;
            this.button_Save.Text = "Save";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // textBox_Title
            // 
            this.textBox_Title.Location = new System.Drawing.Point(109, 16);
            this.textBox_Title.Name = "textBox_Title";
            this.textBox_Title.Size = new System.Drawing.Size(547, 25);
            this.textBox_Title.TabIndex = 3;
            // 
            // label_Description
            // 
            this.label_Description.AutoSize = true;
            this.label_Description.Location = new System.Drawing.Point(0, 57);
            this.label_Description.Name = "label_Description";
            this.label_Description.Size = new System.Drawing.Size(103, 15);
            this.label_Description.TabIndex = 4;
            this.label_Description.Text = "description:";
            // 
            // textBox_Description
            // 
            this.textBox_Description.Location = new System.Drawing.Point(109, 54);
            this.textBox_Description.MaxLength = 32767000;
            this.textBox_Description.Multiline = true;
            this.textBox_Description.Name = "textBox_Description";
            this.textBox_Description.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_Description.Size = new System.Drawing.Size(545, 100);
            this.textBox_Description.TabIndex = 5;
            // 
            // label_Input
            // 
            this.label_Input.AutoSize = true;
            this.label_Input.Location = new System.Drawing.Point(48, 167);
            this.label_Input.Name = "label_Input";
            this.label_Input.Size = new System.Drawing.Size(55, 15);
            this.label_Input.TabIndex = 6;
            this.label_Input.Text = "Input:";
            // 
            // label_Output
            // 
            this.label_Output.AutoSize = true;
            this.label_Output.Location = new System.Drawing.Point(40, 285);
            this.label_Output.Name = "label_Output";
            this.label_Output.Size = new System.Drawing.Size(63, 15);
            this.label_Output.TabIndex = 7;
            this.label_Output.Text = "Output:";
            // 
            // label_SampleInput
            // 
            this.label_SampleInput.AutoSize = true;
            this.label_SampleInput.Location = new System.Drawing.Point(106, 394);
            this.label_SampleInput.Name = "label_SampleInput";
            this.label_SampleInput.Size = new System.Drawing.Size(95, 15);
            this.label_SampleInput.TabIndex = 8;
            this.label_SampleInput.Text = "SampleInput";
            // 
            // textBox_Input
            // 
            this.textBox_Input.Location = new System.Drawing.Point(109, 167);
            this.textBox_Input.MaxLength = 32767000;
            this.textBox_Input.Multiline = true;
            this.textBox_Input.Name = "textBox_Input";
            this.textBox_Input.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_Input.Size = new System.Drawing.Size(545, 100);
            this.textBox_Input.TabIndex = 9;
            // 
            // textBox_Output
            // 
            this.textBox_Output.Location = new System.Drawing.Point(109, 282);
            this.textBox_Output.Multiline = true;
            this.textBox_Output.Name = "textBox_Output";
            this.textBox_Output.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_Output.Size = new System.Drawing.Size(545, 100);
            this.textBox_Output.TabIndex = 10;
            // 
            // label_SampleOutput
            // 
            this.label_SampleOutput.AutoSize = true;
            this.label_SampleOutput.Location = new System.Drawing.Point(458, 394);
            this.label_SampleOutput.Name = "label_SampleOutput";
            this.label_SampleOutput.Size = new System.Drawing.Size(103, 15);
            this.label_SampleOutput.TabIndex = 11;
            this.label_SampleOutput.Text = "SampleOutput";
            // 
            // textBox_SampleInput
            // 
            this.textBox_SampleInput.Location = new System.Drawing.Point(12, 425);
            this.textBox_SampleInput.MaxLength = 32767000;
            this.textBox_SampleInput.Multiline = true;
            this.textBox_SampleInput.Name = "textBox_SampleInput";
            this.textBox_SampleInput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_SampleInput.Size = new System.Drawing.Size(300, 100);
            this.textBox_SampleInput.TabIndex = 12;
            // 
            // textBox_SampleOutput
            // 
            this.textBox_SampleOutput.Location = new System.Drawing.Point(354, 425);
            this.textBox_SampleOutput.MaxLength = 32767000;
            this.textBox_SampleOutput.Multiline = true;
            this.textBox_SampleOutput.Name = "textBox_SampleOutput";
            this.textBox_SampleOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_SampleOutput.Size = new System.Drawing.Size(300, 100);
            this.textBox_SampleOutput.TabIndex = 13;
            // 
            // textBox_Hint
            // 
            this.textBox_Hint.Location = new System.Drawing.Point(109, 543);
            this.textBox_Hint.MaxLength = 3276700;
            this.textBox_Hint.Multiline = true;
            this.textBox_Hint.Name = "textBox_Hint";
            this.textBox_Hint.Size = new System.Drawing.Size(545, 25);
            this.textBox_Hint.TabIndex = 14;
            // 
            // label_Hint
            // 
            this.label_Hint.AutoSize = true;
            this.label_Hint.Location = new System.Drawing.Point(40, 548);
            this.label_Hint.Name = "label_Hint";
            this.label_Hint.Size = new System.Drawing.Size(47, 15);
            this.label_Hint.TabIndex = 15;
            this.label_Hint.Text = "Hint:";
            // 
            // label_Source
            // 
            this.label_Source.AutoSize = true;
            this.label_Source.Location = new System.Drawing.Point(40, 592);
            this.label_Source.Name = "label_Source";
            this.label_Source.Size = new System.Drawing.Size(63, 15);
            this.label_Source.TabIndex = 16;
            this.label_Source.Text = "Source:";
            // 
            // textBox_Source
            // 
            this.textBox_Source.Location = new System.Drawing.Point(109, 587);
            this.textBox_Source.Name = "textBox_Source";
            this.textBox_Source.Size = new System.Drawing.Size(545, 25);
            this.textBox_Source.TabIndex = 17;
            // 
            // EditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(667, 703);
            this.Controls.Add(this.textBox_Source);
            this.Controls.Add(this.label_Source);
            this.Controls.Add(this.label_Hint);
            this.Controls.Add(this.textBox_Hint);
            this.Controls.Add(this.textBox_SampleOutput);
            this.Controls.Add(this.textBox_SampleInput);
            this.Controls.Add(this.label_SampleOutput);
            this.Controls.Add(this.textBox_Output);
            this.Controls.Add(this.textBox_Input);
            this.Controls.Add(this.label_SampleInput);
            this.Controls.Add(this.label_Output);
            this.Controls.Add(this.label_Input);
            this.Controls.Add(this.textBox_Description);
            this.Controls.Add(this.label_Description);
            this.Controls.Add(this.textBox_Title);
            this.Controls.Add(this.button_Save);
            this.Controls.Add(this.label_Title);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "EditForm";
            this.Text = "Edit";
            this.Load += new System.EventHandler(this.EditForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label_Title;
        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.TextBox textBox_Title;
        private System.Windows.Forms.Label label_Description;
        private System.Windows.Forms.TextBox textBox_Description;
        private System.Windows.Forms.Label label_Input;
        private System.Windows.Forms.Label label_Output;
        private System.Windows.Forms.Label label_SampleInput;
        private System.Windows.Forms.TextBox textBox_Input;
        private System.Windows.Forms.TextBox textBox_Output;
        private System.Windows.Forms.Label label_SampleOutput;
        private System.Windows.Forms.TextBox textBox_SampleInput;
        private System.Windows.Forms.TextBox textBox_SampleOutput;
        private System.Windows.Forms.TextBox textBox_Hint;
        private System.Windows.Forms.Label label_Hint;
        private System.Windows.Forms.Label label_Source;
        private System.Windows.Forms.TextBox textBox_Source;
    }
}