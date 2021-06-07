using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panosen.CodeDom.EFCore
{
    /// <summary>
    /// INFORMATION_SCHEMA.KEY_COLUMN_USAG
    /// </summary>
    public class KeyColumnUsage
    {

        /// <summary>
        /// 约束名称
        /// INFORMATION_SCHEMA.KEY_COLUMN_USAGE.CONSTRAINT_NAME
        /// </summary>
        public string ConstraintName { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 字段名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 被引用的表
        /// INFORMATION_SCHEMA.KEY_COLUMN_USAGE.REFERENCED_TABLE_NAME
        /// </summary>
        public string ReferencedTableName { get; set; }

        /// <summary>
        /// 被引用的字段
        /// INFORMATION_SCHEMA.KEY_COLUMN_USAGE.REFERENCED_COLUMN_NAME
        /// </summary>
        public string ReferencedColumnName { get; set; }

        /// <summary>
        /// 表
        /// INFORMATION_SCHEMA.KEY_COLUMN_USAGE.TABLE_NAME
        /// </summary>
        public string RealTableName { get; set; }

        /// <summary>
        /// 字段
        /// INFORMATION_SCHEMA.KEY_COLUMN_USAGE.COLUMN_NAME
        /// </summary>
        public string RealColumnName { get; set; }

        /// <summary>
        /// 被引用的表
        /// INFORMATION_SCHEMA.KEY_COLUMN_USAGE.REFERENCED_TABLE_NAME
        /// </summary>
        public string RealReferencedTableName { get; set; }

        /// <summary>
        /// 被引用的字段
        /// INFORMATION_SCHEMA.KEY_COLUMN_USAGE.REFERENCED_COLUMN_NAME
        /// </summary>
        public string RealReferencedColumnName { get; set; }
    }
}
