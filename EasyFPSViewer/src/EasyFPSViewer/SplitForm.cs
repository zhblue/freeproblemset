using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using EasyFPSViewer.Core;
using EasyFPSViewer.Models;

namespace EasyFPSViewer
{
    public partial class SplitForm : Form
    {
        private List<FPSItem> _fpsItemList;
        private SplitForm()
        {
            InitializeComponent();
            I18N.InitControl(this);
        }

        public SplitForm(List<FPSItem> fpsItemList) : this()
        {
            _fpsItemList = fpsItemList;
        }

        private void SplitForm_Load(object sender, EventArgs e)
        {
            textBox_OutputDir.Text = Environment.CurrentDirectory;
        }

        private void button_Browse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                textBox_OutputDir.Text = dialog.SelectedPath;
            }
        }

        private void button_Split_Click(object sender, EventArgs e)
        {
            try
            {
                SplitFPSFile();
                MessageBox.Show(I18N.GetStr("Split successful!"));
            }
            catch
            {
                MessageBox.Show(I18N.GetStr("Split failed!"));
            }
        }

        private void SplitFPSFile()
        {
            int size;
            if (!Int32.TryParse(textBox_Size.Text, out size))
            {
                MessageBox.Show(I18N.GetStr("Size must be a number."));
                return;
            }

            int outputNum = 1;
            List<FPSItem> itemBufferList = new List<FPSItem>();
            int nowSize = 0;
            foreach (FPSItem item in _fpsItemList)
            {
                nowSize += item.TestDataSize / 1024 / 1024;
                itemBufferList.Add(item);

                if (nowSize >= size)
                {
                    WriteFPSFile(itemBufferList, textBox_OutputDir.Text, textBox_Prefix.Text, outputNum++);

                    itemBufferList.Clear();
                    nowSize = 0;
                }
            }

            if(itemBufferList.Count > 0)
            {
                WriteFPSFile(itemBufferList, textBox_OutputDir.Text, textBox_Prefix.Text, outputNum++);
            }
        }

        private void WriteFPSFile(List<FPSItem> itemList, string dir, string prefix, int num)
        {
            string path = Path.Combine(dir, prefix + num + ".xml");
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                new FPSParser().ConvertToStream(itemList.ToArray(), fs);
            }
        }

        private void button_Help_Click(object sender, EventArgs e)
        {
            MessageBox.Show(I18N.GetStr("Split tool can split a big FPS file to some little files."));
        }
    }
}
