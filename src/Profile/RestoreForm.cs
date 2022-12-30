using EnvDTE;
using Profile.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Profile
{
    public partial class RestoreForm : Form
    {
        private string _solution;
        public RestoreForm(string solution)
        {
            _solution = solution;
            InitializeComponent();
            BindListView();
        }

        private void BindListView()
        {
            this.listView1.View = View.LargeIcon;

            //this.listView1.LargeImageList = this.imageList2;

            this.listView1.BeginUpdate();

            var configList = SaveAsCommand.Instance.Database.Queryable<ProfileConfig>().Where(m => m.SolutionName == _solution).ToList();

            foreach(var config in configList)
            {
                ListViewItem lvi = new ListViewItem();

                //lvi.ImageIndex = configList.IndexOf(config) + 1;

                lvi.Text = config.ConfigName;

                lvi.Tag = config.ConfigId;

                this.listView1.Items.Add(lvi);
            }

            this.listView1.EndUpdate();
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = listView1.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                var configId = item.Tag as string;
                var fileList = SaveAsCommand.Instance.Database.Queryable<ProfileItem>().Where(m => m.ConfigId == configId).ToList();
                foreach(var file in fileList)
                {
                    System.IO.File.WriteAllBytes(file.Path, file.Cotnent);
                }
                MessageBox.Show("还原成功！");
                this.Close();
            }
        }
    }
}
