using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Panosen.CodeDom.CSharp;
using Panosen.CodeDom.CSharp.Engine;

namespace Panosen.CodeDom.EFCore.Engine
{
    /// <summary>
    /// DBContextBuildEntityEngine
    /// </summary>
    public class DBContextBuildTableEngine
    {
        /// <summary>
        /// Generate
        /// </summary>
        public void Generate(DBContextBuildTableFile dbContextBuildTable, CodeWriter codeWriter)
        {
            if (dbContextBuildTable == null)
            {
                return;
            }

            CodeFile codeFile = new CodeFile();

            codeFile.AddSystemUsing(SystemUsing.SystemThreading);
            codeFile.AddSystemUsing(SystemUsing.SystemThreadingTasks);

            codeFile.AddNugetUsing("Microsoft.EntityFrameworkCore");
            codeFile.AddNugetUsing("Microsoft.EntityFrameworkCore.Metadata.Builders");

            codeFile.AddProjectUsing($"{dbContextBuildTable.CSharpRootNamespace}.Entity");

            var codespace = codeFile.AddNamespace(dbContextBuildTable.CSharpRootNamespace);

            var codeClass = codespace.AddClass(dbContextBuildTable.ContextName).SetIsPartial(true);

            if (dbContextBuildTable.Table != null)
            {
                codeClass.AddMethod(BuildMethod_BuildTable(dbContextBuildTable, dbContextBuildTable.IgnoreForeignKey));
            }

            new CSharpCodeEngine().GenerateCodeFile(codeWriter, codeFile);
        }

        private CodeMethod BuildMethod_BuildTable(DBContextBuildTableFile dbContextBuildTable, bool ignoreForeignKey)
        {
            var table = dbContextBuildTable.Table;

            CodeMethod codeMethod = new CodeMethod();
            codeMethod.AccessModifiers = AccessModifiers.Private;
            codeMethod.Type = "void";
            codeMethod.Name = $"Build{table.TableName}";

            codeMethod.AddParameter($"EntityTypeBuilder<{table.TableEntity()}>", table.TableEntity().ToLowerCamelCase());

            //表映射
            codeMethod.StepStatementChain().AddCallMethodExpression($"{table.TableEntity().ToLowerCamelCase()}.ToTable")
                .AddParameter(DataValue.DoubleQuotationString(table.RealTableName));

            //多租户
            if (!string.IsNullOrEmpty(dbContextBuildTable.TenantServiceName)
                && !string.IsNullOrEmpty(dbContextBuildTable.TenantServicePropertyName)
                && !string.IsNullOrEmpty(dbContextBuildTable.TenantTablePropertyName))
            {
                codeMethod.StepEmpty();
                codeMethod.StepStatementChain().AddCallMethodExpression($"{table.TableEntity().ToLowerCamelCase()}.HasQueryFilter")
                    .AddParameterOfLamdaExpression()
                    .SetParameter("table")
                    .SetExpression($"table.{dbContextBuildTable.TenantTablePropertyName} == {dbContextBuildTable.TenantServiceName}.{dbContextBuildTable.TenantServicePropertyName}");
            }

            //主键
            codeMethod.StepEmpty();
            var stepBuilderOfPrimaryKey = codeMethod.StepStatementChain();
            if (table.PrimaryKeyColumns != null && table.PrimaryKeyColumns.Count > 0)
            {
                if (table.PrimaryKeyColumns.Count == 1)
                {
                    stepBuilderOfPrimaryKey
                        .AddCallMethodExpression($"{table.TableEntity().ToLowerCamelCase()}.HasKey")
                        .AddParameter($"table => table.{table.PrimaryKeyColumns[0].ColumnName}");
                }
                else
                {
                    stepBuilderOfPrimaryKey
                        .AddCallMethodExpression($"{table.TableEntity().ToLowerCamelCase()}.HasKey")
                        .AddParameter($"table => new {{ {string.Join(", ", table.PrimaryKeyColumns.Select(v => $"table.{v.ColumnName}"))} }}");
                }

                stepBuilderOfPrimaryKey.AddCallMethodExpression("HasName", true).AddParameter(DataValue.DoubleQuotationString("PRIMARY"));
            }
            else
            {
                stepBuilderOfPrimaryKey.AddCallMethodExpression($"{table.TableEntity().ToLowerCamelCase()}.HasNoKey");
            }

            //索引
            if (table.Indexes != null && table.Indexes.Count > 0)
            {
                var indexes = table.Indexes.OrderBy(v => v.Name).ToList();
                foreach (var index in indexes)
                {
                    codeMethod.StepEmpty();

                    codeMethod.StepStatement($"// {index.Name}");

                    var stepBuilder = codeMethod.StepStatementChain();
                    if (index.Properties.Count == 1)
                    {
                        stepBuilder.AddCallMethodExpression($"{table.TableEntity().ToLowerCamelCase()}.HasIndex")
                            .AddParameter($"table => table.{index.Properties[0].ColumnName}");
                    }
                    else
                    {
                        stepBuilder.AddCallMethodExpression($"{table.TableEntity().ToLowerCamelCase()}.HasIndex")
                            .AddParameter($"table => new {{ {string.Join(", ", index.Properties.Select(v => $"table.{v.ColumnName}"))} }}");
                    }

                    stepBuilder.AddCallMethodExpression("HasDatabaseName", true).AddParameter(DataValue.DoubleQuotationString(index.Name));
                    if (index.NONE_UNIQUE == 0)
                    {
                        stepBuilder.AddCallMethodExpression($"IsUnique", true);
                    }
                }
            }

            //配置字段
            if (table.ColumnMap != null && table.ColumnMap.Count > 0)
            {
                foreach (var field in table.ColumnMap.Values)
                {
                    codeMethod.StepEmpty();

                    codeMethod.StepStatement($"// `{table.RealTableName}`.`{field.RealColumnName}`");

                    var stepBuilder = codeMethod.StepStatementChain();
                    stepBuilder.AddCallMethodExpression($"{table.TableEntity().ToLowerCamelCase()}.Property").AddParameter($"p => p.{field.ColumnName}");
                    stepBuilder.AddCallMethodExpression("HasColumnName", true).AddParameter(DataValue.DoubleQuotationString(field.RealColumnName));
                    if (field.NotNullable && (table.PrimaryKeyColumns == null || table.PrimaryKeyColumns.Count == 0 || !table.PrimaryKeyColumns.Contains(field)))
                    {
                        stepBuilder.AddCallMethodExpression("IsRequired", true);
                    }
                    if (field.MaxLength != null && field.MaxLength != uint.MaxValue)
                    {
                        stepBuilder.AddCallMethodExpression("HasMaxLength", true).AddParameter(field.MaxLength.Value);
                    }
                }
            }

            //配置外键
            if (!ignoreForeignKey && table.KeyColumnUsageList != null && table.KeyColumnUsageList.Count > 0)
            {
                foreach (var keyColumnUsage in table.KeyColumnUsageList)
                {
                    if (keyColumnUsage.RealReferencedTableName == null)
                    {
                        continue;
                    }

                    if (!keyColumnUsage.RealTableName.Equals(table.RealTableName))
                    {
                        continue;
                    }

                    codeMethod.StepEmpty();

                    codeMethod.StepStatement($"// {keyColumnUsage.ConstraintName}");

                    var stepBuilder = codeMethod.StepStatementChain();
                    stepBuilder.AddCallMethodExpression($"{table.TableEntity().ToLowerCamelCase()}.HasOne")
                        .AddParameter($"v => v.{keyColumnUsage.ReferencedTableName}");
                    stepBuilder.AddCallMethodExpression("WithMany", true).AddParameter($"v => v.{table.TableName}s");
                    stepBuilder.AddCallMethodExpression("HasForeignKey", true).AddParameter($"v => v.{keyColumnUsage.ColumnName}");
                }
            }

            return codeMethod;
        }
    }
}
