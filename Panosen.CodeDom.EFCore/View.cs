using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panosen.CodeDom.EFCore
{
    /// <summary>
    /// 视图
    /// </summary>
    public class View
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string ViewName { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        public Dictionary<string, Column> ColumnMap { get; set; }
    }

    /// <summary>
    /// ViewExtension
    /// </summary>
    public static class ViewExtension
    {
        /// <summary>
        /// SubjectEntity
        /// </summary>
        public static string ViewEntity(this View view)
        {
            return $"{view.Name}Entity";
        }

        /// <summary>
        /// 添加一个属性
        /// </summary>
        public static View AddColumn(this View view, Column column)
        {
            if (view.ColumnMap == null)
            {
                view.ColumnMap = new Dictionary<string, Column>();
            }

            view.ColumnMap.Add(column.ColumnName, column);

            return view;
        }

        /// <summary>
        /// 添加一个属性
        /// </summary>
        public static Column AddColumn(this View view, string columnName, string realColumnName = null)
        {
            if (view.ColumnMap == null)
            {
                view.ColumnMap = new Dictionary<string, Column>();
            }

            Column column = new Column();
            column.ColumnName = columnName;
            column.RealColumnName = realColumnName;

            view.ColumnMap.Add(column.ColumnName, column);

            return column;
        }
    }
}
