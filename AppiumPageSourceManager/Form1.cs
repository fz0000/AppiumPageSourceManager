using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;

namespace AppiumPageSourceManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            SetProcessDpiAwareness();
            InitializeComponent();
        }

        private static void SetProcessDpiAwareness()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                try
                {
                    SetProcessDpiAwareness(PROCESS_DPI_AWARENESS.Process_Per_Monitor_DPI_Aware);
                }
                catch
                {
                    SetProcessDPIAware();
                }
            }
        }


        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [DllImport("shcore.dll")]
        private static extern int SetProcessDpiAwareness(PROCESS_DPI_AWARENESS awareness);

        private enum PROCESS_DPI_AWARENESS
        {
            Process_DPI_Unaware = 0,
            Process_System_DPI_Aware = 1,
            Process_Per_Monitor_DPI_Aware = 2
        }

        private void ToolStripButtonLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "XML Files|*.xml",
                Title = "Select Appium PageSource XML File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadXmlToTree(openFileDialog.FileName);
            }
        }

        private void LoadXmlToTree(string filePath)
        {
            treeView1.Nodes.Clear();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            TreeNode rootNode = CreateNode(xmlDoc.DocumentElement);
            treeView1.Nodes.Add(rootNode);
            AddNodes(xmlDoc.DocumentElement, rootNode);
            treeView1.CollapseAll();
        }

        private TreeNode CreateNode(XmlNode xmlNode)
        {
            string localizedControlType = xmlNode.Attributes?["LocalizedControlType"]?.Value ?? "";
            string name = xmlNode.Attributes?["Name"]?.Value ?? "";
            string autoid = xmlNode.Attributes?["AutomationId"]?.Value ?? "";
            string nodeName = $"\"{localizedControlType}\" \"{name}\" \"{autoid}\"";
            return new TreeNode(nodeName) { Tag = xmlNode };
        }

        private void AddNodes(XmlNode xmlNode, TreeNode treeNode)
        {
            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                TreeNode childTreeNode = CreateNode(childNode);
                treeNode.Nodes.Add(childTreeNode);
                AddNodes(childNode, childTreeNode);
            }
        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is XmlNode xmlNode)
            {
                dataGridView1.Columns.Clear();
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Add("Attribute", "Attribute");
                dataGridView1.Columns.Add("Value", "Value");
                if (xmlNode.Attributes != null)
                {
                    foreach (XmlAttribute attr in xmlNode.Attributes)
                    {
                        dataGridView1.Rows.Add(attr.Name, attr.Value);
                    }
                }
            }
        }
    }
}
