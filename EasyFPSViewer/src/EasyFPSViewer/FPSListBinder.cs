using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EasyFPSViewer.Core;
using EasyFPSViewer.Models;

namespace EasyFPSViewer
{
    public class FPSListBinder
    {
        public ListView ListView { get; private set; }
        public List<FPSItem> FPSItemList { get; private set; }
        public Dictionary<FPSItem, ListViewItem> ViewItemDic { get; private set; }
        public Dictionary<FPSItem, ProblemForm> ProblemFormDic { get; private set; }
        public Dictionary<FPSItem, EditForm> EditFormDic { get; private set; }

        public FPSListBinder(ListView listView)
        {
            ListView = listView;
            FPSItemList = new List<FPSItem>();
            ViewItemDic = new Dictionary<FPSItem, ListViewItem>();
            ProblemFormDic = new Dictionary<FPSItem, ProblemForm>();
            EditFormDic = new Dictionary<FPSItem, EditForm>();
        }

        public void Add(FPSItem fpsItem)
        {
            ListViewItem viewItem = ListView.Items.Add(fpsItem.Title);
            viewItem.SubItems.Add(fpsItem.TimeLimit + fpsItem.TimeLimitUnit);
            viewItem.SubItems.Add(fpsItem.MemoryLimit + fpsItem.MemoryLimitUnit);

            int casesCount = fpsItem.TestOutput.Length;
            if (!string.IsNullOrEmpty(fpsItem.SampleOutput))
            {
                casesCount++;
            }
            viewItem.SubItems.Add(casesCount.ToString());
            viewItem.SubItems.Add((Math.Max(fpsItem.TestDataSize / 1024, 1)).ToString() + "KB");

            FPSItemList.Add(fpsItem);
            ViewItemDic[fpsItem] = viewItem;
        }

        public int Search(string title, string description)
        {
            ClearSelectStatus();
            var quary = from n in FPSItemList
                        where n.Title.ToLower().Contains(title.ToLower())
                        where n.Description.ToLower().Contains(description.ToLower())
                        select n;

            FPSItem firstItem = null;
            foreach(var fpsItem in quary)
            {
                if (firstItem == null)
                {
                    firstItem = fpsItem;
                }
                ViewItemDic[fpsItem].Selected = true;
            }

            if (firstItem != null)
            {
                ViewItemDic[firstItem].EnsureVisible();
                ViewItemDic[firstItem].Focused = true;
            }

            return quary.Count();
        }

        public void ClearSelectStatus()
        {
            foreach(ListViewItem item in ListView.Items)
            {
                if(item.Selected)
                {
                    item.Selected = false;
                }
            }
        }
        public void Flush()
        {
            ListView.Items.Clear();
            ViewItemDic.Clear();

            foreach(FPSItem fpsItem in FPSItemList)
            {
                ListViewItem viewItem = ListView.Items.Add(fpsItem.Title);
                viewItem.SubItems.Add(fpsItem.TimeLimit + fpsItem.TimeLimitUnit);
                viewItem.SubItems.Add(fpsItem.MemoryLimit + fpsItem.MemoryLimitUnit);

                int casesCount = fpsItem.TestOutput.Length;
                if (!string.IsNullOrEmpty(fpsItem.SampleOutput))
                {
                    casesCount++;
                }
                viewItem.SubItems.Add(casesCount.ToString());
                viewItem.SubItems.Add((Math.Max(fpsItem.TestDataSize / 1024, 1)).ToString() + "KB");

                ViewItemDic[fpsItem] = viewItem;
            }
        }

        public void Remove(FPSItem fpsItem)
        {
            CloseProblemForm(fpsItem);
            CloseEditForm(fpsItem);

            ListView.Items.Remove(ViewItemDic[fpsItem]);
            ViewItemDic.Remove(fpsItem);
            ProblemFormDic.Remove(fpsItem);
            EditFormDic.Remove(fpsItem);

            FPSItemList.Remove(fpsItem);

            GC.Collect();
        }

        public void Sort<T>(Func<FPSItem, T> selector)
        {
            FPSItemList = FPSItemList.OrderBy(selector).ToList();
            Flush();
        }

        public void Clear()
        {
            foreach(FPSItem key in ProblemFormDic.Keys)
            {
                CloseProblemForm(key);
            }

            foreach (FPSItem key in EditFormDic.Keys)
            {
                CloseEditForm(key);
            }

            ListView.Items.Clear();
            ProblemFormDic.Clear();
            ViewItemDic.Clear();
            FPSItemList.Clear();
            EditFormDic.Clear();

            GC.Collect();
        }

        public FPSItem[] GetSelectedItems()
        {
            List<FPSItem> selectedList = new List<FPSItem>();
            foreach(ListViewItem viewItem in ListView.SelectedItems)
            {
                selectedList.Add(FPSItemList[viewItem.Index]);
            }

            return selectedList.ToArray();
        }

        public void CreatePorblemForm(FPSItem fpsItem)
        {
            if (!ProblemFormDic.ContainsKey(fpsItem) || ProblemFormDic[fpsItem].IsDisposed)
            {
                ProblemFormDic[fpsItem] = new ProblemForm(fpsItem);
                ProblemFormDic[fpsItem].Show();
            }
            else
            {
                ProblemFormDic[fpsItem].Focus();
            }
        }

        public void CloseProblemForm(FPSItem fpsItem)
        {
            if (ProblemFormDic.ContainsKey(fpsItem) && !ProblemFormDic[fpsItem].IsDisposed)
            {
                ProblemFormDic[fpsItem].Close();
            }
        }

        public void CreateEditForm(FPSItem fpsItem)
        {
            if (!EditFormDic.ContainsKey(fpsItem) || EditFormDic[fpsItem].IsDisposed)
            {
                EditFormDic[fpsItem] = new EditForm(fpsItem);
                EditFormDic[fpsItem].Show();
            }
            else
            {
                EditFormDic[fpsItem].Focus();
            }
        }

        public void CloseEditForm(FPSItem fpsItem)
        {
            if (EditFormDic.ContainsKey(fpsItem) && !EditFormDic[fpsItem].IsDisposed)
            {
                EditFormDic[fpsItem].Close();
            }
        }
    }
}
