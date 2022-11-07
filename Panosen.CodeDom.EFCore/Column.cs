using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panosen.CodeDom.EFCore
{
    /// <summary>
    /// 字段
    /// </summary>
    public class Column
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 字段名
        /// </summary>
        public string RealColumnName { get; set; }

        /// <summary>
        /// 列类型，带数据长度
        /// </summary>
        public string ColumnType { get; set; }

        /// <summary>
        /// 可为空
        /// </summary>
        public bool NotNullable { get; set; }

        /// <summary>
        /// 最大长度
        /// </summary>
        public uint? MaxLength { get; set; }

        /// <summary>
        /// csharp 属性类型
        /// </summary>
        public string CSharpType { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string Comment { get; set; }
    }
}
