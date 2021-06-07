using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Panosen.CodeDom.CSharp;
using Panosen.CodeDom.CSharp.Engine;

namespace Panosen.CodeDom.EFCore.Engine
{
    /// <summary>
    /// SubjectEntityEngine
    /// </summary>
    public class TableEntityEngine
    {
        /// <summary>
        /// Generate
        /// </summary>
        /// <returns></returns>
        public void Generate(TableEntityFile tableEntity, CodeWriter codeWriter)
        {
            CodeFile codeFile = new CodeFile();

            var codespace = codeFile.AddNamespace($"{tableEntity.CSharpRootNamespace}.Entity");

            var codeClass = codespace.AddClass(tableEntity.Table.TableEntity(), summary: $"`{tableEntity.Table.RealTableName}`", accessModifiers: AccessModifiers.Public);
            codeClass.AddSystemUsing(SystemUsing.System);
            codeClass.AddSystemUsing(SystemUsing.SystemCollectionsGeneric);

            if (tableEntity.Table.ColumnMap != null && tableEntity.Table.ColumnMap.Count > 0)
            {
                foreach (var column in tableEntity.Table.ColumnMap.Values)
                {
                    codeClass.AddProperty(column.CSharpType, column.ColumnName, summary: $"{column.Comment}{Environment.NewLine}{column.ColumnType} `{tableEntity.Table.RealTableName}`.`{column.RealColumnName}`");
                }
            }

            PrepareForeignKey(tableEntity, codeClass);

            new CSharpCodeEngine().GenerateCodeFile(codeFile, codeWriter);
        }

        private void PrepareForeignKey(TableEntityFile tableEntity, CodeClass codeClass)
        {
            if (tableEntity.IgnoreForeignKey)
            {
                return;
            }

            var table = tableEntity.Table;

            if (table.KeyColumnUsageList == null || table.KeyColumnUsageList.Count <= 0)
            {
                return;
            }

            if (table.KeyColumnUsageList.Any(v => v.ReferencedTableName == tableEntity.Table.TableName))
            {
                codeClass.AddConstructor(Constructor(tableEntity.Table, tableEntity.TableMap));
            }

            foreach (var keyColumnUsage in table.KeyColumnUsageList)
            {
                if (keyColumnUsage.TableName != tableEntity.Table.TableName)
                {
                    continue;
                }

                if (keyColumnUsage.ReferencedTableName == null)
                {
                    continue;
                }

                var referencedTable = tableEntity.TableMap[keyColumnUsage.ReferencedTableName];

                codeClass.AddProperty($"{referencedTable.TableEntity()}", $"{referencedTable.TableName}")
                    .SetIsVirtual(true)
                    .SetSummary(keyColumnUsage.ConstraintName);
            }

            foreach (var keyColumnUsage in table.KeyColumnUsageList)
            {
                if (keyColumnUsage.ReferencedTableName != tableEntity.Table.TableName)
                {
                    continue;
                }

                var referenceTable = tableEntity.TableMap[keyColumnUsage.TableName];

                codeClass.AddProperty($"ICollection<{referenceTable.TableEntity()}>", $"{referenceTable.TableName}s")
                    .SetIsVirtual(true)
                    .SetSummary(keyColumnUsage.ConstraintName);
            }
        }

        private CodeMethod Constructor(Table table, Dictionary<string, Table> tableMap)
        {
            CodeMethod codeMethod = new CodeMethod();
            codeMethod.AccessModifiers = AccessModifiers.Public;
            codeMethod.Name = table.TableEntity();
            codeMethod.SetSummary(table.TableEntity());

            foreach (var keyColumnUsage in table.KeyColumnUsageList)
            {
                if (keyColumnUsage.ReferencedTableName != table.TableName)
                {
                    continue;
                }

                var tableName = keyColumnUsage.TableName;
                var table2 = tableMap[tableName];

                codeMethod.StepStatement($"{tableName}s = new HashSet<{table2.TableEntity()}>();");
            }

            return codeMethod;
        }
    }
}
