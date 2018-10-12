using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EasyFPSViewer.Core;
using EasyFPSViewer.Models;

namespace EasyFPSViewer
{
    public partial class EditForm : Form
    {
        private FPSItem _fpsItem;
        private EditForm()
        {
            InitializeComponent();
            I18N.InitControl(this);
        }

        public EditForm(FPSItem fpsItem) : this()
        {
            this.Text = this.Text + " - " + fpsItem.Title;
            _fpsItem = fpsItem;
        }

        private void EditForm_Load(object sender, EventArgs e)
        {
            textBox_Title.Text = _fpsItem.Title;
            textBox_Description.Text = _fpsItem.Description;
            textBox_Input.Text = _fpsItem.Input;
            textBox_Output.Text = _fpsItem.Output;
            textBox_SampleInput.Text = _fpsItem.SampleInput.Replace("\r\n", "\r").Replace("\n", "\r").Replace("\r", "\r\n");
            textBox_SampleOutput.Text = _fpsItem.SampleOutput.Replace("\r\n", "\r").Replace("\n", "\r").Replace("\r", "\r\n");
            textBox_Hint.Text = _fpsItem.Hint;
            textBox_Source.Text = _fpsItem.Source;
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            _fpsItem.Title = textBox_Title.Text;
            _fpsItem.Description = textBox_Description.Text;
            _fpsItem.Input = textBox_Input.Text;
            _fpsItem.Output = textBox_Output.Text;
            _fpsItem.SampleInput = textBox_SampleInput.Text.Replace("\r\n", "\n").Replace("\r", "\n");
            _fpsItem.SampleOutput = textBox_SampleOutput.Text.Replace("\r\n", "\n").Replace("\r", "\n");
            _fpsItem.Hint = textBox_Hint.Text;
            _fpsItem.Source = textBox_Source.Text;

            if (!_fpsItem.SampleInput.EndsWith("\n"))
            {
                _fpsItem.SampleInput += "\n";
            }

            if (!_fpsItem.SampleOutput.EndsWith("\n"))
            {
                _fpsItem.SampleOutput += "\n";
            }

            MessageBox.Show(I18N.GetStr("Save successful!"));
            this.Close();
        }
    }
}
