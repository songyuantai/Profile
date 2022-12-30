using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Profile.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Profile
{
    public partial class SaveForm : Form
    {
        private readonly string _solution;

        private readonly Dictionary<string, List<string>> _dic;
        public SaveForm(string solution, Dictionary<string, List<string>> dic)
        {
            _solution = solution;
            _dic = dic;
            InitializeComponent();
            BindTree();
        }

        private void BindTree()
        {
            treeView1.CheckBoxes = true;
            treeView1.Indent = 20;
            treeView1.Height= 20;
            foreach (var kv in _dic)
            {
                var treeNode = new TreeNode(kv.Key);
                foreach(var text in kv.Value)
                {
                    var fileName = Path.GetFileName(text);
                    var subNode = new TreeNode(fileName)
                    {
                        Tag = text
                    };
                    treeNode.Nodes.Add(subNode);
                }
                treeView1.Nodes.Add(treeNode);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var configName = textBox1.Text.Trim();
            var config = SaveAsCommand.Instance.Database.Queryable<ProfileConfig>().First(m => m.SolutionName == _solution && m.ConfigName == configName);
            if (config == null)
            {
                config = new ProfileConfig
                {
                    ConfigId = Guid.NewGuid().ToString(),
                    SolutionName = _solution,
                    ConfigName = configName,
                };

                SaveAsCommand.Instance.Database.Insertable(config).ExecuteCommand();
            }
            else
            {
                config.SolutionName= _solution;
                config.ConfigName = configName;
                SaveAsCommand.Instance.Database.Updateable(config).ExecuteCommand();
            }

            var items = SaveAsCommand.Instance.Database.Queryable<ProfileItem>().Where(m => m.ConfigId == config.ConfigId).ToList();

            foreach (TreeNode node in GetCheckedNodeList())
            {
                var path = node.Tag as string;
                if (!string.IsNullOrEmpty(path))
                {
                    var item = items.FirstOrDefault(m => m.Path == path);
                    if (null != item)
                    {
                        item.Cotnent = File.ReadAllBytes(path);
                        SaveAsCommand.Instance.Database.Updateable(item).ExecuteCommand();
                        items.Remove(item);
                    }
                    else
                    {
                        item = new ProfileItem
                        {
                            ItemId = Guid.NewGuid().ToString(),
                            ConfigId = config.ConfigId,
                            Path = path,
                            Cotnent = File.ReadAllBytes(path),
                        };

                        SaveAsCommand.Instance.Database.Insertable(item).ExecuteCommand();
                    }
                }
            }

            SaveAsCommand.Instance.Database.Deleteable(items).ExecuteCommand();

            MessageBox.Show("保存成功！");
        }

        /// <summary>
        /// 获取选中的node节点
        /// </summary>
        /// <returns></returns>
        private List<TreeNode> GetCheckedNodeList()
        {
            List<TreeNode> nodes = new List<TreeNode>();
            foreach (TreeNode node in treeView1.Nodes)
            {
                AddCheckedNode(node, nodes);
            }
            return nodes;
        }

        /// <summary>
        /// 将一个节点及其子节点加入列表
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodes"></param>
        private void AddCheckedNode(TreeNode node, List<TreeNode> nodes)
        {
            if (node.Checked)
            {
                nodes.Add(node);
            }

            foreach(TreeNode subNode in node.Nodes)
            {
                AddCheckedNode(subNode, nodes);
            }
        }
    }


}
