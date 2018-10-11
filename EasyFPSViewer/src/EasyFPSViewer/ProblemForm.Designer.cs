namespace EasyFPSViewer
{
    partial class ProblemForm
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
            this.webBrowser_Main = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // webBrowser_Main
            // 
            this.webBrowser_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser_Main.Location = new System.Drawing.Point(0, 0);
            this.webBrowser_Main.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser_Main.Name = "webBrowser_Main";
            this.webBrowser_Main.Size = new System.Drawing.Size(800, 450);
            this.webBrowser_Main.TabIndex = 0;
            this.webBrowser_Main.Url = new System.Uri("", System.UriKind.Relative);
            // 
            // ProblemForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.webBrowser_Main);
            this.Name = "ProblemForm";
            this.Text = "Problem";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ProblemForm_FormClosed);
            this.Load += new System.EventHandler(this.ProblemForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser_Main;
    }
}