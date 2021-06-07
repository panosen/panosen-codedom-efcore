using Microsoft.VisualStudio.TestTools.UnitTesting;
using Panosen.CodeDom.CSharp.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panosen.CodeDom.EFCore.Engine.MSTest
{
    [TestClass]
    public class TableEntityEngineTest1
    {
        private Table bookTable;
        private Table tagTable;
        private Dictionary<string, Table> tableMap;
        private List<KeyColumnUsage> keyColumnUsageList;

        [TestInitialize]
        public void Setup()
        {
            this.bookTable = PrepareBook();
            this.tagTable = PrepareTag();
            this.keyColumnUsageList = PrepareKeyColumnUsageList();

            this.tableMap = new Dictionary<string, Table>();
            this.tableMap.Add(this.bookTable.TableName, this.bookTable);
            this.tableMap.Add(this.tagTable.TableName, this.tagTable);
        }

        [TestMethod]
        public void TestBook()
        {
            var tableEntityFile = new TableEntityFile();
            tableEntityFile.CSharpRootNamespace = "Savory.BookManage.Repository";
            tableEntityFile.TableMap = this.tableMap;
            tableEntityFile.Table = this.bookTable;
            tableEntityFile.Table.KeyColumnUsageList = this.keyColumnUsageList;

            var builder = new StringBuilder();
            new TableEntityEngine().Generate(tableEntityFile, new StringWriter(builder));
            var actual = builder.ToString();
            var expected = PrepareBookExpected();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTag()
        {
            var tableEntityFile = new TableEntityFile();
            tableEntityFile.CSharpRootNamespace = "Savory.BookManage.Repository";
            tableEntityFile.TableMap = this.tableMap;
            tableEntityFile.Table = this.tagTable;
            tableEntityFile.Table.KeyColumnUsageList = this.keyColumnUsageList;

            var builder = new StringBuilder();
            new TableEntityEngine().Generate(tableEntityFile, new StringWriter(builder));
            var actual = builder.ToString();
            var expected = PrepareTagExpected();
            Assert.AreEqual(expected, actual);
        }

        private Table PrepareBook()
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
                property.Comment = "Book.Id";
                property.CSharpType = "int";
                property.ColumnType = "int";

                table.ColumnMap.Add("Id", property);
                table.PrimaryKeyColumns.Add(property);
            }

            {
                var property = new Column();
                property.ColumnName = "Name";
                property.RealColumnName = "name";
                property.MaxLength = 100;
                property.Comment = "Book.Name";
                property.CSharpType = "string";
                property.ColumnType = "varchar(100)";

                table.ColumnMap.Add("Name", property);
            }

            {
                var property = new Column();
                property.ColumnName = "TagId";
                property.RealColumnName = "tag_id";
                property.Comment = "Book.TagId";
                property.CSharpType = "int";
                property.ColumnType = "int";

                table.ColumnMap.Add("TagId", property);
            }

            {
                table.Indexes = new List<Index>();
                table.Indexes.Add(new Index
                {
                    Name = "IX_book",
                    NONE_UNIQUE = 0,
                    Properties = new List<Column> { table.ColumnMap["TagId"] }
                });
            }

            return table;
        }

        private static List<KeyColumnUsage> PrepareKeyColumnUsageList()
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

            return keyColumnUsageList;
        }

        private Table PrepareTag()
        {
            var table = new Table();
            table.TableName = "Tag";
            table.RealTableName = "tag";
            table.ColumnMap = new Dictionary<string, Column>();
            table.PrimaryKeyColumns = new List<Column>();

            {
                var property = new Column();
                property.ColumnName = "Id";
                property.RealColumnName = "id";
                property.Comment = "Tag.Id";
                property.CSharpType = "int";
                property.ColumnType = "int";

                table.ColumnMap.Add("Id", property);
                table.PrimaryKeyColumns.Add(property);
            }

            return table;
        }

        private string PrepareBookExpected()
        {
            return @"using System;
using System.Collections.Generic;

namespace Savory.BookManage.Repository.Entity
{

    /// <summary>
    /// `book`
    /// </summary>
    public class BookEntity
    {

        /// <summary>
        /// Book.Id
        /// int `book`.`id`
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Book.Name
        /// varchar(100) `book`.`name`
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Book.TagId
        /// int `book`.`tag_id`
        /// </summary>
        public int TagId { get; set; }

        /// <summary>
        /// FK_book_tag_id__tag_id
        /// </summary>
        public virtual TagEntity Tag { get; set; }
    }
}
";
        }

        private string PrepareTagExpected()
        {
            return @"using System;
using System.Collections.Generic;

namespace Savory.BookManage.Repository.Entity
{

    /// <summary>
    /// `tag`
    /// </summary>
    public class TagEntity
    {

        /// <summary>
        /// Tag.Id
        /// int `tag`.`id`
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// FK_book_tag_id__tag_id
        /// </summary>
        public virtual ICollection<BookEntity> Books { get; set; }

        /// <summary>
        /// TagEntity
        /// </summary>
        public TagEntity()
        {
            Books = new HashSet<BookEntity>();
        }
    }
}
";
        }
    }
}
