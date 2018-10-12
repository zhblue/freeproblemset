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
        public FPSListBinder FPSListBinder { get; private set; }

        private AboutForm _aboutForm;
        private SplitForm _splitForm;
        public MainForm()
        {
            InitializeComponent();
            FPSListBinder = new FPSListBinder(listView_Problem);
            I18N.InitControl(this);
            I18N.InitContextMenuStrip(contextMenuStrip_FPSItemList);
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
                        new FPSParser().ConvertToStream(FPSListBinder.FPSItemList.ToArray(), fs);
                    }
                    MessageBox.Show(I18N.GetStr("Save successful!"));
                }
                catch
                {
                    MessageBox.Show(I18N.GetStr("Save failed!"));
                }
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearFPSItem();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedItems();
        }

        private void replaceNewLineToNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message =
                I18N.GetStr("This operation will replace all NewLine Chars of TestData to \\n(Linux). Are you sure ? ");
            if (MessageBox.Show(message, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ReplaceItemsNewLineToLinux();
                MessageBox.Show(I18N.GetStr("Replace successful!"));
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
                _splitForm = new SplitForm(FPSListBinder.FPSItemList);
                _splitForm.Show();
            }
            else
            {
                _splitForm.Focus();
            }
        }

        private void listView_Problem_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            switch (e.Column)
            {
                case 0: FPSListBinder.Sort(n => n.Title); break;
                case 1: FPSListBinder.Sort(n => n.TimeLimit); break;
                case 2: FPSListBinder.Sort(n => n.MemoryLimit); break;
                case 3: FPSListBinder.Sort(n => n.TestInput.Length); break;
                case 4: FPSListBinder.Sort(n => n.TestDataSize); break;
            }
        }

        private void showDetailsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowSelectedFPSItems();
        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            EditSelectedFPSItems();
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DeleteSelectedItems();
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchForm searchForm = new SearchForm();
            searchForm.ShowDialog();

            if (searchForm.Result.Item1)
            {
                int count = FPSListBinder.Search(searchForm.Result.Item2, searchForm.Result.Item3);
                MessageBox.Show(count.ToString(), I18N.GetStr("Result Count"));
            }

        }

        private void editStripMenuItem_Click(object sender, EventArgs e)
        {
            EditSelectedFPSItems();
        }

        private void addProblemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FPSListBinder.Add(new FPSItem
            {
                Title = "New problem",
                TimeLimit = 1,
                TimeLimitUnit = "s",
                MemoryLimit = 256,
                MemoryLimitUnit = "mb",
                TestInput = new string[0],
                TestOutput = new string[0],
                Solutions = new FPSItemSolution[0],
                Images = new FPSItemImage[0],
                SpecialJudge = new FPSItemSpecialJudge[0]
            });
        }

        private void bugReportStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Azure99/EasyFPSViewer/issues");
        }
        #endregion

        #region Method
        private void ClearFPSItem()
        {
            FPSListBinder.Clear();
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
            

            foreach (FPSItem fpsItem in fpsItems)
            {
                FPSListBinder.Add(fpsItem);
            }
        }

        private void DeleteSelectedItems()
        {
            foreach (FPSItem item in FPSListBinder.GetSelectedItems())
            {
                FPSListBinder.Remove(item);
            }
        }

        private void CloseItemForm(FPSItem fpsItem)
        {
            FPSListBinder.CloseProblemForm(fpsItem);
        }

        private void ReplaceItemsNewLineToLinux()
        {
            foreach (FPSItem item in FPSListBinder.FPSItemList)
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
            foreach (FPSItem item in FPSListBinder.GetSelectedItems())
            {
                FPSListBinder.CreatePorblemForm(item);
            }
        }

        private void EditSelectedFPSItems()
        {
            foreach (FPSItem item in FPSListBinder.GetSelectedItems())
            {
                FPSListBinder.CreateEditForm(item);
            }
        }

        #endregion
    }
}
