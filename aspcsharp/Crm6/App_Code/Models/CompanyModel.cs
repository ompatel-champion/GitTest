using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using Crm6.App_Code;
using Crm6.App_Code.Login;

namespace Models
{
    public class CompanyModel
    {
        public Company Company { get; set; }
        public DocumentModel CompanyLogo { get; set; }
        public bool CreateSession { get; set; }
    }

    public class CompanyExtended
    {
        public string SalesOwnerName;
        public Company Company;
    }

    public class CompanyContactModel
    {
        public Company Company { get; set; }
        public IEnumerable<Contact> Contacts { get; set; }
    }

    public class LinkCompanyRequest
    {
        public int UpdateUserId { get; set; }
        public int SubscriberId { get; set; }
        public int CompanyId { get; set; }
        public int LinkedCompanyId { get; set; }
        public string LinkType { get; set; }
    }

    public class CompanyImportModel
    {
        public int SubscriberId { get; set; }
        public string SalesRepName { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string PostalCode { get; set; }
        public string CountryName { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public string Fax { get; set; }
        public string Website { get; set; }
        public string Industry { get; set; }
        public string Source { get; set; }
        public string CampaignName { get; set; }
        public string Comments { get; set; }
        public IEnumerable<ContactImport> ContactImports { get; set; }

        public static bool IsHeaderRow(IXLRow row)
        {
            var val = row.Cell(1).Value.ToString().ToLower().Trim();
            return val == "sales rep name";
        }

        public static CompanyImportModel FromXlRow(IXLRow row, int subscriberId, GlobalUser user)
        {
            var xxx = row?.Cell("A").Value.ToString() ?? "Bob Dylan";

            return new CompanyImportModel
            {
                SubscriberId = subscriberId,
                SalesRepName = user.FullName,
                CompanyName = row?.Cell("B")?.Value?.ToString(),
                Address = row?.Cell("G")?.Value?.ToString(),
                City = row?.Cell("H")?.Value?.ToString(),
                StateProvince = row?.Cell("I")?.Value?.ToString(),
                PostalCode = row?.Cell("J")?.Value?.ToString(),
                CountryName = row?.Cell("K")?.Value?.ToString(),
                //CountryCode = codeLookupList.FirstOrDefault(c => c.CountryName.ToLower() == row?.Cell("K")?.Value?.ToString().ToLower())?.CountryCode ?? "US",
                Phone = row?.Cell("L")?.Value?.ToString(),
                Phone2 = row?.Cell("M")?.Value?.ToString(),
                Fax = row?.Cell("N")?.Value?.ToString(),
                Website = row?.Cell("O")?.Value?.ToString(),
                Industry = row?.Cell("P")?.Value?.ToString(),
                Source = row?.Cell("Q")?.Value?.ToString(),
                CampaignName = row?.Cell("R")?.Value?.ToString(),
                Comments = row?.Cell("S")?.Value?.ToString(),
            };
        }

        public static CompanyImportModel AddContacts(CompanyImportModel model, IXLWorksheet worksheet)
        {
            var companyRows = worksheet
                .Rows()
                .Where(i => i.Cell("B").Value.Equals(model.CompanyName))
                .ToList();

            model.ContactImports = companyRows
                .Select(i => ContactImport.FromRow(i, model.SubscriberId));
            return model;
        }
    }
}