using Microsoft.VisualStudio.TestTools.UnitTesting;
using Panosen.CodeDom.CSharp.Engine;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Panosen.CodeDom.EFCore.Engine.MSTest
{
    [TestClass]
    public class DBContextBuildTableEngineTenantTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var dBContextBuildTable = new DBContextBuildTableFile();
            dBContextBuildTable.ContextName = "BookDBContext";
            dBContextBuildTable.CSharpRootNamespace = "Savory.BookManage.Repository";
            dBContextBuildTable.TenantServiceName = "tenantService";
            dBContextBuildTable.TenantServicePropertyName = "TenantIdB";
            dBContextBuildTable.TenantTablePropertyName = "TenantIdA";

            {
                var table = new Table();
                table.TableName = "Book";
                table.RealTableName = "book";
                table.ColumnMap = new Dictionary<string, Column>();
                table.PrimaryKeyColumns = new List<Column>();

                {
                    var property = new Column();
                    property.ColumnName = "Id";
                    property.RealColumnName = "id";

                    table.ColumnMap.Add("Id", property);
                    table.PrimaryKeyColumns.Add(property);
                }


                {
                    var property = new Column();
                    property.ColumnName = "Name";
                    property.RealColumnName = "name";
                    property.MaxLength = 100;

                    table.ColumnMap.Add("Name", property);
                }

                {
                    var keyColumnUsageList = new List<KeyColumnUsage>();
                    keyColumnUsageList.Add(new KeyColumnUsage
                    {
                        TableName = "Book",
                        RealTableName = "book",
                        ColumnName = "TagId",
                        RealColumnName = "tag_id",
                        ReferencedTableName = "Tag",
                        RealReferencedTableName = "tag",
                        ReferencedColumnName = "Id",
                        RealReferencedColumnName = "id",
                        ConstraintName = "FK_book_tag_id__tag_id"
                    });

                    table.KeyColumnUsageList = keyColumnUsageList;
                }

                dBContextBuildTable.Table = table;
            }

            var builder = new StringBuilder();

            new DBContextBuildTableEngine().Generate(dBContextBuildTable, builder);

            var actual = builder.ToString();

            var expected = PrepareExpected();

            Assert.AreEqual(expected, actual);
        }

        private string PrepareExpected()
        {
            return @"using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Savory.BookManage.Repository.Entity;

namespace Savory.BookManage.Repository
{

    partial class BookDBContext
    {

        private void BuildBook(EntityTypeBuilder<BookEntity> bookEntity)
        {
            bookEntity.ToTable(""book"");

            bookEntity.HasQueryFilter(table => table.TenantIdA == tenantService.TenantIdB);

            bookEntity.HasKey(table => table.Id)
                .HasName(""PRIMARY"");

            // `book`.`id`
            bookEntity.Property(p => p.Id)
                .HasColumnName(""id"");

            // `book`.`name`
            bookEntity.Property(p => p.Name)
                .HasColumnName(""name"")
                .HasMaxLength(100);

            // FK_book_tag_id__tag_id
            bookEntity.HasOne(v => v.Tag)
                .WithMany(v => v.Books)
                .HasForeignKey(v => v.TagId);
        }
    }
}
";
        }
    }
}
