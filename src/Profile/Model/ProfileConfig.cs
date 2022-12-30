using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profile.Model
{
    /// <summary>
    /// 配置项
    /// </summary>
    [SugarTable("ProfileConfig")]
    public class ProfileConfig
    {
        /// <summary>
        /// 配置id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string ConfigId { get; set; }

        /// <summary>
        /// 配置名称
        /// </summary>
        public string ConfigName { get; set; } = string.Empty;

        /// <summary>
        /// 解决方案
        /// </summary>
        public string SolutionName { get; set; } = string.Empty;
    }
}
