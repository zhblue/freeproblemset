using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using EasyFPSViewer.Core;
using EasyFPSViewer.Models;

namespace EasyFPSViewer
{
    public partial class MainForm : Form
    {
        private List<FPSItem> _fpsItemList = new List<FPSItem>();
        private AboutForm _aboutForm;
        private SplitForm _splitForm;
        public MainForm()
        {
            InitializeComponent();
        }

        #region Event
        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void listView_Problem_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void listView_Problem_DragDrop(object sender, DragEventArgs e)
        {
            Array files = e.Data.GetData(DataFormats.FileDrop) as Array;
            List<string> fileList = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                AddFPSFile(files.GetValue(i) as string);
            }
        }

        private void listView_Problem_DoubleClick(object sender, EventArgs e)
        {
            ShowSelectedFPSItems();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "FPS Files|*.xml|All Files|*.*";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string fileName in openFileDialog.FileNames)
                {
                    AddFPSFile(fileName);
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "freeproblemset.xml";
            saveFileDialog.Filter = "*.xml|XML File";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        new FPSParser().ConvertToStream(_fpsItemList.ToArray(), fs);
                    }
                    MessageBox.Show("Save successful!");
                }
                catch
                {
                    MessageBox.Show("Save failed!");
                }
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearFPSItem();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteNowSelectedItems();
        }

        private void replaceNewLineToNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const string message =
                "This operation will replace all NewLine Chars of TestData to \\n(Linux)\r\nAre you sure ? ";
            if (MessageBox.Show(message, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ReplaceItemsNewLineToLinux();
                MessageBox.Show("Replace success");
            }
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSelectedFPSItems();
        }

        private void clearToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ClearFPSItem();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_aboutForm == null || _aboutForm.IsDisposed)
            {
                _aboutForm = new AboutForm();
                _aboutForm.Show();
            }
            else
            {
                _aboutForm.Focus();
            }
        }

        private void splitFPSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_splitForm == null || _splitForm.IsDisposed)
            {
                _splitForm = new SplitForm(_fpsItemList);
                _splitForm.Show();
            }
            else
            {
                _splitForm.Focus();
            }
        }
        #endregion

        #region Method
        private void ClearFPSItem()
        {
            _fpsItemList.Clear();
            listView_Problem.Items.Clear();
        }

        private void LoadFPSFiles(string[] files)
        {
            ClearFPSItem();
            foreach (string file in files)
            {
                AddFPSFile(file);
            }
        }

        private void AddFPSFile(string xmlFilePath)
        {
            FPSItem[] fpsItems;
            try
            {
                fpsItems = new FPSParser().ParseFile(xmlFilePath);
            }
            catch
            {
                MessageBox.Show("Parse error!\r\n" + xmlFilePath);
                return;
            }

            _fpsItemList.AddRange(fpsItems);

            foreach (FPSItem fpsItem in fpsItems)
            {
                ListViewItem viewItem = listView_Problem.Items.Add(fpsItem.Title);
                viewItem.SubItems.Add(fpsItem.TimeLimit + fpsItem.TimeLimitUnit);
                viewItem.SubItems.Add(fpsItem.MemoryLimit + fpsItem.MemoryLimitUnit);

                int casesCount = fpsItem.TestOutput.Length;
                if (!string.IsNullOrEmpty(fpsItem.SampleOutput))
                {
                    casesCount++;
                }
                viewItem.SubItems.Add(casesCount.ToString());

                viewItem.SubItems.Add((Math.Max(fpsItem.TestDataSize / 1024, 1)).ToString() + "KB");
            }
        }

        private void DeleteNowSelectedItems()
        {
            List<ListViewItem> viewItemList = new List<ListViewItem>();
            List<FPSItem> fpsItemList = new List<FPSItem>();
            foreach (ListViewItem viewItem in listView_Problem.SelectedItems)
            {
                viewItemList.Add(viewItem);
                fpsItemList.Add(_fpsItemList[viewItem.Index]);
            }

            foreach (ListViewItem item in viewItemList)
            {
                listView_Problem.Items.Remove(item);
            }

            foreach (FPSItem item in fpsItemList)
            {
                _fpsItemList.Remove(item);
            }
        }

        private void ReplaceItemsNewLineToLinux()
        {
            foreach (FPSItem item in _fpsItemList)
            {
                item.SampleInput = item.SampleInput.Replace("\r\n", "\n").Replace("\r", "\n");
                item.SampleOutput = item.SampleOutput.Replace("\r\n", "\n").Replace("\r", "\n");
                for (int i = 0; i < item.TestInput.Length; i++)
                {
                    item.TestInput[i] = item.TestInput[i].Replace("\r\n", "\n").Replace("\r", "\n");
                }

                for (int i = 0; i < item.TestOutput.Length; i++)
                {
                    item.TestOutput[i] = item.TestOutput[i].Replace("\r\n", "\n").Replace("\r", "\n");
                }
            }
        }

        private void ShowSelectedFPSItems()
        {
            foreach (ListViewItem viewItem in listView_Problem.SelectedItems)
            {
                new ProblemForm(_fpsItemList[viewItem.Index]).Show();
            }
        }

        #endregion

    }
}
