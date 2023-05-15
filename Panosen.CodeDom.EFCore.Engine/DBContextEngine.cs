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
    /// DBContextEngine
    /// </summary>
    public class DBContextEngine
    {
        /// <summary>
        /// Generate
        /// </summary>
        /// <returns></returns>
        public void Generate(DBContextFile dbContext, CodeWriter codeWriter)
        {
            CodeFile codeFile = new CodeFile();

            codeFile.AddSystemUsing(SystemUsing.SystemThreading);
            codeFile.AddSystemUsing(SystemUsing.SystemThreadingTasks);

            codeFile.AddNugetUsing("Microsoft.EntityFrameworkCore");
            codeFile.AddNugetUsing("Microsoft.EntityFrameworkCore.Metadata.Builders");

            codeFile.AddProjectUsing($"{dbContext.CSharpRootNamespace}.Entity");

            var codespace = codeFile.AddNamespace(dbContext.CSharpRootNamespace);

            var codeClass = codespace.AddClass(dbContext.ContextName, accessModifiers: AccessModifiers.Public)
                .SetIsPartial(true)
                .SetBaseClass("DbContext")
                .SetSummary(dbContext.ContextName);

            if (dbContext.TableMap != null && dbContext.TableMap.Count > 0)
            {
                foreach (var table in dbContext.TableMap.Values)
                {
                    codeClass.AddProperty($"DbSet<{table.TableEntity()}>", $"{table.TableName}s")
                        .SetSummary($"table `{table.RealTableName}`");
                }
            }

            if (dbContext.ViewList != null && dbContext.ViewList.Count > 0)
            {
                foreach (var view in dbContext.ViewList)
                {
                    codeClass.AddProperty($"DbSet<{view.ViewEntity()}>", $"{view.Name}s")
                        .SetSummary($"view `{dbContext.DBName}`.`{view.ViewName}`");
                }
            }

            codeClass.AddConstructor(BuildMethod_Constructor(dbContext));

            codeClass.AddMethod(BuildMethod_OnModelCreating(dbContext));

            new CSharpCodeEngine().GenerateCodeFile(codeWriter, codeFile);
        }

        private CodeMethod BuildMethod_Constructor(DBContextFile dbContext)
        {
            CodeMethod codeMethod = new CodeMethod();
            codeMethod.AccessModifiers = AccessModifiers.Public;
            codeMethod.Name = dbContext.ContextName;
            codeMethod.AddParameter($"DbContextOptions<{dbContext.ContextName}>", "options");
            codeMethod.SetSummary(dbContext.ContextName);

            codeMethod.BaseConstructor = "base(options)";

            codeMethod.StepCollection = new StepCollection();

            return codeMethod;
        }

        private CodeMethod BuildMethod_OnModelCreating(DBContextFile dbContext)
        {
            CodeMethod codeMethod = new CodeMethod();
            codeMethod.AccessModifiers = AccessModifiers.Protected;
            codeMethod.Type = "void";
            codeMethod.Name = "OnModelCreating";
            codeMethod.SetSummary("OnModelCreating");
            codeMethod.SetIsOverride(true);

            codeMethod.AddParameter("ModelBuilder", "builder");

            codeMethod.StepCollection = new StepCollection();

            if (dbContext.TableMap != null && dbContext.TableMap.Count > 0)
            {
                foreach (var table in dbContext.TableMap.Values)
                {
                    codeMethod.StepStatement($"builder.Entity<{table.TableEntity()}>(Build{table.TableName});");
                }
            }

            if (dbContext.ViewList != null && dbContext.ViewList.Count > 0)
            {
                foreach (var view in dbContext.ViewList)
                {
                    codeMethod.StepStatement($"builder.Entity<{view.ViewEntity()}>(Build{view.Name});");
                }
            }

            return codeMethod;
        }
    }
}
