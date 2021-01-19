using System;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using Crm6.App_Code;

namespace Models
{
    public class ContactModel
    {
        public Contact Contact { get; set; }
        public DocumentModel ProfilePicture { get; set; }
    }

    public class ContactImport
    {
        private static readonly Func<IXLRow, bool> _isHeader = row
            => row.Cell(1).Value.ToString().ToLower().Equals("company name");

        public int SubscriberId { get; set; }
        public string SalesRepName { get; set; }

        public string CompanyName { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Website { get; set; }
        public string Industry { get; set; }
        public string Source { get; set; }
        public string CampaignName { get; set; }
        public string Comments { get; set; }

        public List<Exception> ParsingExceptions { get; } = new List<Exception>();

        public static Contact ToContact(ContactImport import, Company co)
        {
            var contact = new Contact
            {
                SubscriberId = import.SubscriberId,
                CompanyName = co.CompanyName,
                CompanyId = co.CompanyId,
                FirstName = import.ContactFirstName,
                LastName = import.ContactLastName,
                ContactName = $"{import.ContactFirstName} {import.ContactLastName}",
                Email = import.Email,
                BusinessAddress = import.Address,
                BusinessCity = import.City,
                BusinessPostalCode = import.PostalCode,
                BusinessCountry = import.Country,
                BusinessPhone = import.Phone,
                Fax = import.Fax,
                Website = import.Website,
                ContactType = import.Industry,
                Source = import.Source,
                CreatedUserId = co.CreatedUserId,
                CreatedUserName = co.CreatedUserName,
                UpdateUserName = co.CreatedUserName,
                Comments = import.Comments,
                CreatedDate = DateTime.UtcNow,
                LastUpdate = DateTime.UtcNow,
                
            };
            return contact;
        }

        public static ContactImport FromRow(IXLRow row, int subscriberId)
        {
            var contactImport = new ContactImport();
            if (_isHeader(row))
                return contactImport;

            if (row.Cells(1, 15).All(i => string.IsNullOrEmpty(i.Value.ToString())))
            {
                contactImport.ParsingExceptions.Add(new Exception("Empty row"));
                return contactImport;
            }

            try
            {
                contactImport.SubscriberId = subscriberId;
                contactImport.CompanyName = row.Cell("B")?.Value.ToString();
                contactImport.ContactFirstName = row.Cell("C")?.Value.ToString();
                contactImport.ContactLastName = row.Cell("D")?.Value.ToString();
                contactImport.Email = row.Cell("F")?.Value.ToString();
                contactImport.Address = row.Cell("G")?.Value.ToString();
                contactImport.City = row.Cell("H")?.Value.ToString();
                contactImport.State = row.Cell("I")?.Value.ToString();
                contactImport.PostalCode = row.Cell("J")?.Value.ToString();
                contactImport.Country = row.Cell("K")?.Value.ToString();
                contactImport.Phone = row.Cell("L")?.Value.ToString();
                contactImport.Fax = row.Cell("N")?.Value.ToString();
                contactImport.Website = row.Cell("O")?.Value.ToString();
                contactImport.Industry = row.Cell("P")?.Value.ToString();
                contactImport.Source = row.Cell("Q")?.Value.ToString();
                contactImport.CampaignName = row.Cell("R")?.Value.ToString();
                contactImport.Comments = row.Cell("S")?.Value.ToString();
            }
            catch (Exception ex)
            {
                contactImport.ParsingExceptions.Add(ex);
            }

            return contactImport;
        }
    }
}