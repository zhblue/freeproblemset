using System;
using System.Text;
using System.IO;
using System.Windows.Forms;
using EasyFPSViewer.Core;
using EasyFPSViewer.Models;

namespace EasyFPSViewer
{
    public partial class ProblemForm : Form
    {
        private FPSItem fpsItem;
        private string tempDir;
        private ProblemForm()
        {
            InitializeComponent();
        }

        public ProblemForm(FPSItem item) : this()
        {
            this.Text = item.Title;

            fpsItem = item;
            tempDir = GetTempDirectory();

            ReleaseFPSResources();
            webBrowser_Main.Url = new Uri(@"file:\\" + Path.Combine(tempDir, "index.html"));
        }

        private void ProblemForm_Load(object sender, EventArgs e)
        {
            
        }


        private void ReleaseFPSResources()
        {
            foreach(FPSItemImage img in fpsItem.Images)
            {
                
                string imagePath = new Uri(img.Path).PathAndQuery;
                imagePath = Path.Combine(tempDir, imagePath);
                Directory.CreateDirectory(Path.GetDirectoryName(imagePath));

                File.WriteAllBytes(imagePath, img.Image);
            }

            File.WriteAllText(Path.Combine(tempDir, "index.html"), FPSViewBuilder.Build(fpsItem));
        }

        private string GetTempDirectory()
        {
            Random random = new Random();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                sb.Append(random.Next(0, 10));
            }

            string path = Path.GetFullPath(Path.Combine("Pages", sb.ToString()));
            Directory.CreateDirectory(path);

            return path;
        }

        private void ProblemForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Directory.Delete(tempDir, true);
        }
    }
}
