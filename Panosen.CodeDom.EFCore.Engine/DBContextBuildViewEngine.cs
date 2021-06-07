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
    /// DBContextBuildViewEngine
    /// </summary>
    public class DBContextBuildViewEngine
    {
        /// <summary>
        /// Generate
        /// </summary>
        public void Generate(DBContextBuildViewFile dbContextBuildView, CodeWriter codeWriter)
        {
            CodeFile codeFile = new CodeFile();

            var codespace = codeFile.AddNamespace(dbContextBuildView.CSharpRootNamespace);

            var codeClass = codespace.AddClass(dbContextBuildView.ContextName).SetIsPartial(true);

            codeClass.AddSystemUsing(SystemUsing.SystemThreading);
            codeClass.AddSystemUsing(SystemUsing.SystemThreadingTasks);

            codeClass.AddNugetUsing("Microsoft.EntityFrameworkCore");
            codeClass.AddNugetUsing("Microsoft.EntityFrameworkCore.Metadata.Builders");

            codeClass.AddProjectUsing($"{dbContextBuildView.CSharpRootNamespace}.Entity");

            if (dbContextBuildView.View != null)
            {
                codeClass.AddMethod(BuildMethod_BuildView(dbContextBuildView.View));
            }

            new CSharpCodeEngine().GenerateCodeFile(codeFile, codeWriter);
        }

        private CodeMethod BuildMethod_BuildView(View view)
        {
            CodeMethod codeMethod = new CodeMethod();
            codeMethod.AccessModifiers = AccessModifiers.Private;
            codeMethod.Type = "void";
            codeMethod.Name = $"Build{view.Name}";

            codeMethod.AddParameter($"EntityTypeBuilder<{view.ViewEntity()}>", view.ViewEntity().ToLowerCamelCase());

            //表映射
            codeMethod.StepStatementChain().AddCallMethodExpression($"{view.ViewEntity().ToLowerCamelCase()}.ToView")
                .AddParameter(DataValue.DoubleQuotationString(view.ViewName));

            //主键
            codeMethod.StepEmpty();
            codeMethod.StepStatementChain().AddCallMethodExpression($"{view.ViewEntity().ToLowerCamelCase()}.HasNoKey");

            //配置字段
            if (view.ColumnMap != null && view.ColumnMap.Count > 0)
            {
                foreach (var column in view.ColumnMap.Values)
                {
                    codeMethod.StepEmpty();

                    codeMethod.StepStatement($"// `{view.ViewName}`.`{column.RealColumnName}`");

                    var chain = codeMethod.StepStatementChain();
                    chain.AddCallMethodExpression($"{view.ViewEntity().ToLowerCamelCase()}.Property").AddParameter($"p => p.{column.ColumnName}");
                    chain.AddCallMethodExpression("HasColumnName", true).AddParameter(DataValue.DoubleQuotationString(column.RealColumnName));
                }
            }

            return codeMethod;
        }
    }
}
