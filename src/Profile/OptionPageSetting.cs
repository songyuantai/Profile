using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profile
{
    public class OptionPageSetting: DialogPage
    {
        private string optionPath = "D:\\propfile";

        [Category("profile配置")]
        [DisplayName("存储目录")]
        [Description("存储文件目录...")]
        public string OptionPath
        {
            get { return optionPath; }
            set { 
                optionPath = value;
                //SaveAsCommand.Instance.InitTables();
            }
        }

        public override void SaveSettingsToStorage()
        {
            base.SaveSettingsToStorage();
            SaveAsCommand.Instance.InitTables();
        }
    }
}
