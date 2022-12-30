using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profile.Model
{
    /// <summary>
    /// 配置文件
    /// </summary>
    [SugarTable("ProfileItem")]
    public class ProfileItem
    {
        /// <summary>
        /// 文件id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string ItemId { get; set; }

        /// <summary>
        /// 配置id
        /// </summary>
        public string ConfigId { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// 文件内容
        /// </summary>
        public byte[] Cotnent { get; set; }
    }
}
