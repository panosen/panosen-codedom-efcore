using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panosen.CodeDom.EFCore
{
    /// <summary>
    /// DBContext
    /// </summary>
    public class DBContextFile
    {
        /// <summary>
        /// 项目命名空间
        /// </summary>
        public string CSharpRootNamespace { get; set; }

        /// <summary>
        /// 上下文名称
        /// </summary>
        public string ContextName { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DBName { get; set; }

        /// <summary>
        /// TableList
        /// </summary>
        public Dictionary<string, Table> TableMap { get; set; }

        /// <summary>
        /// ViewList
        /// </summary>
        public List<View> ViewList { get; set; }
    }
}
