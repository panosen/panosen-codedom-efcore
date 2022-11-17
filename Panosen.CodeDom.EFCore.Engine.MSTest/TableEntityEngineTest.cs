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
    public class TableEntityEngineTest
    {
        [TestMethod]
        public void Test()
        {
            var tableEntity = new TableEntityFile();
            tableEntity.CSharpRootNamespace = "Savory.BookManage.Repository";
            tableEntity.TableMap = new Dictionary<string, Table>();

            {
                var table = new Table();
                table.TableName = "Book";
                table.RealTableName = "book";
                table.Comment = "图书";
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
                    property.ColumnName = "BookId";
                    property.RealColumnName = "book_id";
                    property.Comment = "Book.BookId";
                    property.CSharpType = "int?";
                    property.ColumnType = "int";

                    table.ColumnMap.Add("BookId", property);
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
                    property.ColumnName = "DataStatus";
                    property.RealColumnName = "data_status";
                    property.NotNullable = true;
                    property.Comment = "Book.DataStatus";
                    property.CSharpType = "int";
                    property.ColumnType = "int";

                    table.ColumnMap.Add("DataStatus", property);
                }

                tableEntity.Table = table;
                tableEntity.TableMap.Add("Book", table);
            }

            var builder = new StringBuilder();

            new TableEntityEngine().Generate(tableEntity, builder);

            var actual = builder.ToString();

            var expected = PrepareExpected();

            Assert.AreEqual(expected, actual);
        }

        private string PrepareExpected()
        {
            return @"using System;
using System.Collections.Generic;

namespace Savory.BookManage.Repository.Entity
{

    /// <summary>
    /// `book` 图书
    /// </summary>
    public class BookEntity
    {

        /// <summary>
        /// Book.Id
        /// int `book`.`id`
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Book.BookId
        /// int `book`.`book_id`
        /// </summary>
        public int? BookId { get; set; }

        /// <summary>
        /// Book.Name
        /// varchar(100) `book`.`name`
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Book.DataStatus
        /// int `book`.`data_status`
        /// </summary>
        public int DataStatus { get; set; }
    }
}
";
        }
    }
}
