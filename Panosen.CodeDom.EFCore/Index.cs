using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panosen.CodeDom.EFCore
{
    /// <summary>
    /// 索引
    /// </summary>
    public class Index
    {
        /// <summary>
        /// 索引名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 如果索引不能包括重复词,则为0,如果可以则为1
        /// INFORMATION_SCHEMA.STATISTICS.NONE_QUNIQUE
        /// </summary>
        public int NONE_UNIQUE { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        public List<Column> Properties { get; set; }
    }
}
