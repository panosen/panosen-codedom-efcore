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
    public class DBContextEngineTenantTest
    {
        [TestMethod]
        public void Test()
        {
            DBContextFile dbContext = new DBContextFile(); ;
            dbContext.DBName = "Bookdb";
            dbContext.ContextName = "BookDBContext";
            dbContext.CSharpRootNamespace = "Savory.BookManage.Repository";
            dbContext.TableMap = new Dictionary<string, Table>();
            dbContext.TenantServiceNamespace = "Sample.MultiTenant";
            dbContext.TenantServiceInterface = "ITenantService";
            dbContext.TenantServiceName = "tenantService";

            {
                var subject = new Table();
                subject.TableName = "Book";
                subject.RealTableName = "book";

                dbContext.TableMap.Add("Book", subject);
            }

            var builder = new StringBuilder();
            new DBContextEngine().Generate(dbContext, builder);
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

using Sample.MultiTenant;
using Savory.BookManage.Repository.Entity;

namespace Savory.BookManage.Repository
{

    /// <summary>
    /// BookDBContext
    /// </summary>
    public partial class BookDBContext : DbContext
    {

        /// <summary>
        /// ITenantService
        /// </summary>
        private readonly ITenantService tenantService;

        /// <summary>
        /// table `book`
        /// </summary>
        public DbSet<BookEntity> Books { get; set; }

        /// <summary>
        /// BookDBContext
        /// </summary>
        public BookDBContext(DbContextOptions<BookDBContext> options, ITenantService tenantService) : base(options)
        {
            this.tenantService = tenantService;
        }

        /// <summary>
        /// OnModelCreating
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<BookEntity>(BuildBook);
        }
    }
}
";
        }
    }
}
