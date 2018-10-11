using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using EasyFPSViewer.Models;

namespace EasyFPSViewer.Core
{
    public class FPSParser
    {
        public FPSItem[] ParseFile(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            return Parse(doc);
        }
        public FPSItem[] Parse(string xmlText)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlText);

            return Parse(doc);
        }

        public FPSItem[] Parse(XmlDocument doc)
        {
            List<XmlNode> itemNodeList = new List<XmlNode>();
            foreach (XmlNode rootChild in doc.ChildNodes)
            {
                if (rootChild.Name == "fps")
                {
                    foreach (XmlNode item in rootChild.ChildNodes)
                    {
                        if (item.Name == "item")
                        {
                            itemNodeList.Add(item);
                        }
                    }
                }
            }

            List<FPSItem> itemList = new List<FPSItem>();
            foreach (var itemNode in itemNodeList)
            {
                itemList.Add(ParseItem(itemNode));
            }

            return itemList.ToArray();
        }

        public FPSItem ParseItem(XmlNode itemNode)
        {
            List<FPSItemSolution> solutionsList = new List<FPSItemSolution>();
            foreach(XmlNode node in itemNode.ChildNodes)
            {
                if(node.Name == "solution")
                {
                    solutionsList.Add(new FPSItemSolution
                    {
                        Language = node.Attributes[0].Value,
                        SourceCode = node.InnerText
                    });
                }
            }

            List<string> testInputList = new List<string>();
            foreach (XmlNode node in itemNode.ChildNodes)
            {
                if (node.Name == "test_input")
                {
                    testInputList.Add(node.InnerText);
                }
            }

            List<string> testOutputList = new List<string>();
            foreach (XmlNode node in itemNode.ChildNodes)
            {
                if (node.Name == "test_output")
                {
                    testOutputList.Add(node.InnerText);
                }
            }

            List<FPSItemImage> imagesList = new List<FPSItemImage>();
            foreach (XmlNode node in itemNode.ChildNodes)
            {
                try
                {
                    if (node.Name == "img")
                    {
                        string src = node.ChildNodes[0].InnerText;
                        byte[] img = Convert.FromBase64String(node.ChildNodes[1].InnerText);
                        imagesList.Add(new FPSItemImage
                        {
                            Path = src,
                            Image = img
                        });
                    }
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }

            List<FPSItemSpecialJudge> spjList = new List<FPSItemSpecialJudge>();
            foreach (XmlNode node in itemNode.ChildNodes)
            {
                if (node.Name == "spj")
                {
                    spjList.Add(new FPSItemSpecialJudge
                    {
                        Language = node.Attributes[0].Value,
                        SourceCode = node.InnerText
                    });
                }
            }

            int timeLimit = Int32.Parse(TryGetChildValue(itemNode, "time_limit", "1"));
            string timeLimitUnit = TryGetChildAttrValue(itemNode, "time_limit", "unit", "s");
            int memoryLimit = Int32.Parse(TryGetChildValue(itemNode, "memory_limit", "1"));
            string memoryLimitUnit = TryGetChildAttrValue(itemNode, "memory_limit", "unit", "s");

            string title = TryGetChildValue(itemNode, "title", "No Title");
            string description = TryGetChildValue(itemNode, "description", "Description");
            string input = TryGetChildValue(itemNode, "input", "");
            string output = TryGetChildValue(itemNode, "output", "");
            string sampleInput = TryGetChildValue(itemNode, "sample_input", "");
            string sampleOutput = TryGetChildValue(itemNode, "sample_output", "");
            string hint = TryGetChildValue(itemNode, "hint", "");
            string source = TryGetChildValue(itemNode, "source", "");

            int size = 0;
            size += Encoding.UTF8.GetByteCount(sampleInput);
            size += Encoding.UTF8.GetByteCount(sampleOutput);
            foreach(string data in testInputList)
            {
                size += Encoding.UTF8.GetByteCount(data);
            }
            foreach(string data in testOutputList)
            {
                size += Encoding.UTF8.GetByteCount(data);
            }

            FPSItem fpsItem = new FPSItem
            {
                Title = title,
                Description = description,
                TimeLimit = timeLimit,
                TimeLimitUnit = timeLimitUnit,
                MemoryLimit = memoryLimit,
                MemoryLimitUnit = memoryLimitUnit,
                Hint = hint,
                Source = source,
                Input = input,
                Output = output,
                SampleInput = sampleInput,
                SampleOutput = sampleOutput,
                TestInput = testInputList.ToArray(),
                TestOutput = testOutputList.ToArray(),
                Solutions = solutionsList.ToArray(),
                Images = imagesList.ToArray(),
                SpecialJudge = spjList.ToArray(),
                TestDataSize = size
            };

            return fpsItem;
        }
        public void ConvertToStream(FPSItem[] fpsItems, Stream stream)
        {
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
            writer.Formatting = Formatting.None;

            writer.WriteStartDocument();

            ConvertToXmlDoc(fpsItems).WriteTo(writer);

            writer.WriteEndDocument();
            writer.Flush();
        }

        public XmlDocument ConvertToXmlDoc(FPSItem[] fpsItems)
        {
            XmlDocument doc = new XmlDocument();

            XmlElement fpsNode = doc.CreateElement("fps");
            fpsNode.SetAttribute("version", "1.2");
            doc.AppendChild(fpsNode);

            foreach(FPSItem item in fpsItems)
            {
                ConvertAndAddToXmlNode(doc, fpsNode, item);
            }

            return doc;
        }

        private void ConvertAndAddToXmlNode(XmlDocument doc, XmlNode node,  FPSItem fpsItem)
        {
            XmlNode itemRoot = doc.CreateElement("item");
            node.AppendChild(itemRoot);

            XmlElement title = doc.CreateElement("title");
            title.InnerXml = GetCDATAString(fpsItem.Title);
            itemRoot.AppendChild(title);

            XmlElement timeLimit = doc.CreateElement("time_limit");
            timeLimit.InnerXml = GetCDATAString(fpsItem.TimeLimit);
            timeLimit.SetAttribute("unit", fpsItem.TimeLimitUnit);
            itemRoot.AppendChild(timeLimit);

            XmlElement memoryLimit = doc.CreateElement("memory_limit");
            memoryLimit.InnerXml = GetCDATAString(fpsItem.MemoryLimit);
            memoryLimit.SetAttribute("unit", fpsItem.MemoryLimitUnit);
            itemRoot.AppendChild(memoryLimit);

            foreach(FPSItemImage fpsImg in fpsItem.Images)
            {
                XmlElement imgNode = doc.CreateElement("img");
                XmlElement src = doc.CreateElement("src");
                src.InnerXml = GetCDATAString(fpsImg.Path);
                XmlElement base64 = doc.CreateElement("base64");
                base64.InnerXml = GetCDATAString(Convert.ToBase64String(fpsImg.Image));
                imgNode.AppendChild(src);
                imgNode.AppendChild(base64);

                itemRoot.AppendChild(imgNode);
            }

            XmlElement description = doc.CreateElement("description");
            description.InnerXml = GetCDATAString(fpsItem.Description);
            itemRoot.AppendChild(description);

            XmlElement input = doc.CreateElement("input");
            input.InnerXml = GetCDATAString(fpsItem.Input);
            itemRoot.AppendChild(input);

            XmlElement output = doc.CreateElement("output");
            output.InnerXml = GetCDATAString(fpsItem.Output);
            itemRoot.AppendChild(output);

            XmlElement sampleInput = doc.CreateElement("sample_input");
            sampleInput.InnerXml = GetCDATAString(fpsItem.SampleInput);
            itemRoot.AppendChild(sampleInput);

            XmlElement sampleOutput = doc.CreateElement("sample_output");
            sampleOutput.InnerXml = GetCDATAString(fpsItem.SampleOutput);
            itemRoot.AppendChild(sampleOutput);

            int inIndex = 0;
            int outIndex = 0;
            while(inIndex < fpsItem.TestInput.Length || outIndex < fpsItem.TestOutput.Length)
            {
                if(inIndex < fpsItem.TestInput.Length)
                {
                    XmlElement testInput = doc.CreateElement("test_input");
                    testInput.InnerXml = GetCDATAString(fpsItem.TestInput[inIndex++]);
                    itemRoot.AppendChild(testInput);
                }

                if(outIndex < fpsItem.TestOutput.Length)
                {
                    XmlElement testOutput = doc.CreateElement("test_output");
                    testOutput.InnerXml = GetCDATAString(fpsItem.TestOutput[outIndex++]);
                    itemRoot.AppendChild(testOutput);
                }
            }

            XmlElement hint = doc.CreateElement("hint");
            hint.InnerXml = GetCDATAString(fpsItem.Hint);
            itemRoot.AppendChild(hint);

            XmlElement source = doc.CreateElement("source");
            source.InnerXml = GetCDATAString(fpsItem.Source);
            itemRoot.AppendChild(source);

            foreach(FPSItemSolution solu in fpsItem.Solutions)
            {
                XmlElement soluNode = doc.CreateElement("solution");
                soluNode.SetAttribute("language", solu.Language);
                soluNode.InnerXml = GetCDATAString(solu.SourceCode);
                itemRoot.AppendChild(soluNode);
            }

            foreach(FPSItemSpecialJudge spj in fpsItem.SpecialJudge)
            {
                XmlElement spjNode = doc.CreateElement("spj");
                spjNode.SetAttribute("language", spj.Language);
                spjNode.InnerXml = GetCDATAString(spj.SourceCode);
            }
        }

        private string GetCDATAString(string str)
        {
            return "<![CDATA[" + str + "]]>";
        }

        private string GetCDATAString(object obj)
        {
            return "<![CDATA[" + obj.ToString() + "]]>";
        }

        private string TryGetChildValue(XmlNode node, string name, string defaultValue = "")
        {
            foreach(XmlNode item in node.ChildNodes)
            {
                if(item.Name == name)
                {
                    return item.InnerText;
                }
            }

            return defaultValue;
        }

        private string TryGetChildAttrValue(XmlNode node, string nodeName, string attrName,string defaultValue = "")
        {
            foreach (XmlNode item in node.ChildNodes)
            {
                if (item.Name == nodeName)
                {
                    foreach(XmlAttribute attr in item.Attributes)
                    {
                        if(attr.Name == attr.Name)
                        {
                            return attr.InnerText;
                        }
                    }
                }
            }

            return defaultValue;
        }
    }
}
