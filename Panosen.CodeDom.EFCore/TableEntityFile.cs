using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panosen.CodeDom.EFCore
{
    /// <summary>
    /// TableEntityEngine
    /// </summary>
    public class TableEntityFile
    {
        /// <summary>
        /// 项目命名空间
        /// </summary>
        public string CSharpRootNamespace { get; set; }

        /// <summary>
        /// IgnoreForeignKey
        /// </summary>
        public bool IgnoreForeignKey { get; set; }

        /// <summary>
        /// 主体
        /// </summary>
        public Table Table { get; set; }

        /// <summary>
        /// 所有的表
        /// </summary>
        public Dictionary<string, Table> TableMap { get; set; }
    }
}
