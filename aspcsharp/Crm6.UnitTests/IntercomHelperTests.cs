using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using ClosedXML.Excel;
using Xunit;

namespace Crm6.UnitTests
{
    public class CompanyContactImportTests
    {
        private const string ContainerRef = "temp";
        private const string BlobRef = "3e1d7e2b-df24-4dfa-a9cb-829e8aad7b17.xlsx";
        private const int SubscriberId = 100;
        private const string SharedConnection = "Data Source=ff-test.database.windows.net;Initial Catalog=CRM_Test_Shared;Persist Security Info=True;User ID=FfTest;Password=Ftest#Test!1";
        private const string Connection = "Data Source=ff-test.database.windows.net;Initial Catalog=CRM_Test;Persist Security Info=True;User ID=FfTest;Password=Ftest#Test!1";

        private static readonly Func<string, string, XLWorkbook> GetTestWorkbook = (b, c) =>
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = assembly
                    .GetManifestResourceNames()
                    .Single(str => str.EndsWith("companyImport.xlsx"));

                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    var wb = new XLWorkbook(stream);
                    return wb;
                }
            };

        [Fact]
        public void TestProcessCompanyImport()
        {
            var sut = new App_Code.Helpers.ImportCompanies(SharedConnection, Connection)
            {
                GetWorkbook = GetTestWorkbook
            };

            // override the excel getter 


            var companies = sut.GetCompanyImports(sut.GetWorkbook(ContainerRef, BlobRef), SubscriberId);
            Assert.Equal(185, companies.Count());
            Assert.Equal("AGC Middle East and Africa Office", companies.First().CompanyName);
            Assert.Equal("+971504291341", companies.First().Phone2);

        }

        [Fact]
        public void TestCompanyImport()
        {
            var sut = new App_Code.Helpers.ImportCompanies(SharedConnection, Connection);

            // override the excel getter 
            sut.GetWorkbook = GetTestWorkbook;
            var result = sut.ImportCompaniesContacts(SubscriberId, BlobRef, ContainerRef);

            Assert.False(true, "fix it, please");
        }
    }

    public class IntercomHelperTests
    {
        [Fact]
        public void Test1()
        {
            Assert.True(false);
        }
    }
}
