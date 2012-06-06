using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace FPSParser
{
    public partial class FPSParser : Form
    {
        private string template;
        private string folder_name = "";

        public FPSParser()
        {
            InitializeComponent();
            if (!File.Exists("template.tpl"))
            {
                MessageBox.Show("Template file not exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }

            template = File.ReadAllText("template.tpl", Encoding.UTF8);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void parseProblem(XmlNode node, int idx)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string code = template;
            string baseurl = baseUrl.Text;
            if (baseurl.Trim().ToLower() == "base url" || baseurl.Trim().ToLower() == "" || baseurl.Trim().ToLower() == "http://")
            {
                dict["baseurl"] = "";
            }
            else
            {
                dict["baseurl"] = "<base href=\"" + baseurl.Trim() + "\">";
            }

            XmlNodeList list = node.ChildNodes;
            foreach (XmlNode ch in list)
            {
                if (ch.Name == "time_limit")
                {
                    int time = Convert.ToInt32(ch.InnerText);
                    time *= 1000;
                    dict[ch.Name] = time.ToString();
                }
                else
                    if (ch.Name == "memory_limit")
                    {
                        int memo = Convert.ToInt32(ch.InnerText);
                        memo *= 1024;
                        dict[ch.Name] = memo.ToString();
                    }
                    else
                        if (ch.Name == "solution")
                        {
                            string name;
                            try
                            {
                                name = ch.Attributes["language"].InnerText;
                            }
                            catch (Exception e)
                            {
                                name = "C";
                            }

                            name = name.ToUpper();
                            if (name == "CPP") name = "C++";
                            dict[name] = ch.InnerText;
                        }
                        else
                        {
                            dict[ch.Name] = ch.InnerText;
                        }
            }

            if (!dict.ContainsKey("title")) dict["title"] = "";
            if (!dict.ContainsKey("description")) dict["description"] = "";
            if (!dict.ContainsKey("input")) dict["input"] = "";
            if (!dict.ContainsKey("output")) dict["output"] = "";
            if (!dict.ContainsKey("sample_input")) dict["sample_input"] = "";
            if (!dict.ContainsKey("sample_output")) dict["sample_output"] = "";
            if (!dict.ContainsKey("hint")) dict["hint"] = "";
            if (!dict.ContainsKey("source")) dict["source"] = "";
            if (!dict.ContainsKey("time_limit")) dict["time_limit"] = "1000";
            if (!dict.ContainsKey("memory_limit")) dict["memory_limit"] = "65536";
            if (!dict.ContainsKey("test_input")) dict["test_input"] = "";
            if (!dict.ContainsKey("test_output")) dict["test_output"] = "";

            /** 标程 */
            {
                dict["solution"] = "";
                if (dict.ContainsKey("C++"))
                {
                    StreamWriter sw = File.CreateText(folder_name + "\\solution\\P" + idx.ToString() + ".cpp");
                    sw.Write(dict["C++"]);
                    sw.Close();
                    if (dict["solution"] != "") dict["solution"] += " | ";
                    dict["solution"] += "<a href=\"solution\\P" + idx.ToString() + ".cpp\" alt=\"CPP\">CPP</a>";
                }

                if (dict.ContainsKey("C"))
                {
                    StreamWriter sw = File.CreateText(folder_name + "\\solution\\P" + idx.ToString() + ".c");
                    sw.Write(dict["C"]);
                    sw.Close();
                    if (dict["solution"] != "") dict["solution"] += " | ";
                    dict["solution"] += "<a href=\"solution\\P" + idx.ToString() + ".c\" alt=\"C\">C</a>";
                }

                if (dict.ContainsKey("JAVA"))
                {
                    StreamWriter sw = File.CreateText(folder_name + "\\solution\\P" + idx.ToString() + ".java");
                    sw.Write(dict["JAVA"]);
                    sw.Close();
                    if (dict["solution"] != "") dict["solution"] += " | ";
                    dict["solution"] += "<a href=\"solution\\P" + idx.ToString() + ".java\" alt=\"JAVA\">JAVA</a>";
                }
            }

            /** 页面代码 */
            code = code.Replace("{$idx}", idx.ToString());
            code = code.Replace("{$baseurl}", dict["baseurl"]);
            code = code.Replace("{$title}", dict["title"]);
            code = code.Replace("{$description}", dict["description"]);
            code = code.Replace("{$input}", dict["input"]);
            code = code.Replace("{$output}", dict["output"]);
            code = code.Replace("{$sample_input}", dict["sample_input"]);
            code = code.Replace("{$sample_output}", dict["sample_output"]);
            code = code.Replace("{$hint}", dict["hint"]);
            code = code.Replace("{$source}", dict["source"]);
            code = code.Replace("{$time_limit}", dict["time_limit"]);
            code = code.Replace("{$memory_limit}", dict["memory_limit"]);
            code = code.Replace("{$solution}", dict["solution"]);

            StreamWriter stw = File.CreateText(folder_name + "\\P" + idx.ToString() + ".html");
            stw.Write(code);
            stw.Close();

            /** 测试数据 */
            FileStream fs = File.Create(folder_name + "\\data\\data" + idx.ToString() + ".in");
            fs.Close();
            fs = File.Create(folder_name + "\\data\\data" + idx.ToString() + ".out");
            fs.Close();
            File.WriteAllText(folder_name + "\\data\\data" + idx.ToString() + ".in", dict["test_input"], Encoding.Default);
            File.WriteAllText(folder_name + "\\data\\data" + idx.ToString() + ".out", dict["test_output"], Encoding.Default);

            stw = File.AppendText(folder_name + "\\index.html");
            stw.Write("<a href=\"P" + idx.ToString() + ".html\" alt=\"" + idx.ToString() + "\">[" + idx.ToString() + "] " + dict["title"] + "</a><br />");
            stw.Close();
        }

        private bool parse(string filename)
        {
            if (!File.Exists(textBox1.Text))
            {
                MessageBox.Show("FPS file not exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                XmlReaderSettings xs = new XmlReaderSettings();
                xs.XmlResolver = null;
                xs.ProhibitDtd = false;
                XmlReader reader = XmlReader.Create(filename, xs);
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);

                XmlNode root = doc.SelectSingleNode("fps");
                XmlNodeList list = root.ChildNodes;
                int idx = 1000;
                foreach (XmlNode node in list)
                {
                    if (node.Name != "item") continue;
                    parseProblem(node, idx++);
                }
            }
            catch (XmlException e)
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Please select a FPS file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                folder_name = folderBrowserDialog1.SelectedPath;
                StreamWriter sw = File.CreateText(folder_name + "\\index.html");
                sw.Write("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />");
                sw.Close();
                
                Directory.CreateDirectory(folder_name + "\\solution");
                Directory.CreateDirectory(folder_name + "\\data");

                if (parse(textBox1.Text))
                {
                    MessageBox.Show("FPS file parsed!", "Succeed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    System.Diagnostics.Process.Start(folder_name + "\\index.html");
                }
            }
        }

        private void baseUrl_Enter(object sender, EventArgs e)
        {
            if ("Base Url" == baseUrl.Text)
            {
                baseUrl.Text = "http://";
                baseUrl.ForeColor = Color.Black;
            }
        }

        private void baseUrl_Leave(object sender, EventArgs e)
        {
            if ("http://" == baseUrl.Text.ToLower().Trim() || "" == baseUrl.Text.Trim())
            {
                baseUrl.Text = "Base Url";
                baseUrl.ForeColor = Color.FromArgb(255, 224, 224, 224);
            }
        }
    }
}
