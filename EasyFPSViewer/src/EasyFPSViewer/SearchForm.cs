using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EasyFPSViewer.Core;

namespace EasyFPSViewer
{
    public partial class SearchForm : Form
    {
        public Tuple<bool, string, string> Result { get; private set; }
        public SearchForm()
        {
            InitializeComponent();
            I18N.InitControl(this);
            Result = new Tuple<bool, string, string>(false, "", "");
        }

        private void SearchForm_Load(object sender, EventArgs e)
        {

        }

        private void button_Search_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_Description.Text) &&
                string.IsNullOrEmpty(textBox_Title.Text)) 
            {
                MessageBox.Show(I18N.GetStr("Keywords can not be empty."));
                return;
            }
            Result = new Tuple<bool, string, string>(true, textBox_Title.Text, textBox_Description.Text);
            this.Hide();
            this.Close();
        }
    }
}
