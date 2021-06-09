using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panosen.CodeDom.EFCore
{
    /// <summary>
    /// 表
    /// </summary>
    public class Table
    {
        /// <summary>
        /// 应用表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 数据库表名
        /// </summary>
        public string RealTableName { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        public Dictionary<string, Column> ColumnMap { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public List<Column> PrimaryKeyColumns { get; set; }

        /// <summary>
        /// 索引
        /// </summary>
        public List<Index> Indexes { get; set; }

        /// <summary>
        /// 外键
        /// </summary>
        public List<KeyColumnUsage> KeyColumnUsageList { get; set; }
    }

    /// <summary>
    /// TableExtension
    /// </summary>
    public static class TableExtension
    {
        /// <summary>
        /// SubjectEntity
        /// </summary>
        public static string TableEntity(this Table table)
        {
            return $"{table.TableName}Entity";
        }

        /// <summary>
        /// 添加一个属性
        /// </summary>
        public static Table AddColumn(this Table table, Column column)
        {
            if (table.ColumnMap == null)
            {
                table.ColumnMap = new Dictionary<string, Column>();
            }

            table.ColumnMap.Add(column.ColumnName, column);

            return table;
        }

        /// <summary>
        /// 添加一个属性
        /// </summary>
        public static Column AddColumn(this Table table, string columnName, string realColumnName = null)
        {
            if (table.ColumnMap == null)
            {
                table.ColumnMap = new Dictionary<string, Column>();
            }

            Column field = new Column();
            field.ColumnName = columnName;
            field.RealColumnName = realColumnName;

            table.ColumnMap.Add(columnName, field);

            return field;
        }
    }
}
