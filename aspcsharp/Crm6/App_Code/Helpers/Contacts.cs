using System;
using System.Linq;
using Models;
using System.Collections.Generic;
using Crm6.App_Code;
using Crm6.App_Code.Helpers;
using System.Web.Http;
using ClosedXML.Excel;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;


namespace Helpers
{
    public class Contacts
    {
        
        public string GetContactNameFromId(int contactId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Contacts.Where(t => t.ContactId == contactId).Select(t => t.ContactName).FirstOrDefault() ??
                   "";
        }

        public List<Contact> GetContactLists(ContactFilter filters)
        {
            var response = new ContactListResponse
            {
                Contacts = new List<Contact>()
            };

            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            var contacts = (from contact in context.Contacts
                            where (!contact.Deleted && contact.CompanyId > 0)
                                   && ((contact.FirstName != null && contact.FirstName != "") || (contact.LastName != null && contact.LastName != ""))
                            select contact).Distinct();

            // apply filters
            if (filters.SubscriberId > 0)
                contacts = contacts.Where(t => t.SubscriberId == filters.SubscriberId);

            IEnumerable<Contact> finalContacts = contacts;
            
            if (filters.UserId > 0)
            {
                var companyFilter = new CompanyFilters()
                {
                    SubscriberId = filters.SubscriberId,
                    UserId = filters.UserId,
                    FilterType = "ALL"
                };
                var companies = new Companies().GetCompaniesGlobal(companyFilter);
                finalContacts = (from t in companies.Companies
                                 join contact in contacts on t.CompanyId equals contact.CompanyId
                                 select contact).Distinct();
            }

            // apply filters 
            if (filters.CompanyId > 0)
                finalContacts = finalContacts.Where(t => t.CompanyId == filters.CompanyId);

            if (!string.IsNullOrEmpty(filters.Keyword))
            {
                filters.Keyword = filters.Keyword.ToLower();
                finalContacts = finalContacts.Where(t => (t.FirstName + "").ToLower().Contains(filters.Keyword)
                    || (t.LastName + "").ToLower().Contains(filters.Keyword)
                    || (t.FirstName + " " + t.LastName).ToLower().Contains(filters.Keyword)
                    || (t.CompanyName + "").ToLower().Contains(filters.Keyword)
                    || (t.MiddleName + "").ToLower().Contains(filters.Keyword)
                    );
            }

            // sort
            if (!string.IsNullOrEmpty(filters.SortBy))
            {
                switch (filters.SortBy.ToLower())
                {
                    case "createddate asc":
                        finalContacts = finalContacts.OrderBy(t => t.CreatedDate);
                        break;
                    case "createddate desc":
                        finalContacts = finalContacts.OrderByDescending(t => t.CreatedDate);
                        break;
                    case "contactname asc":
                        finalContacts = finalContacts.OrderBy(t => t.FirstName);
                        break;
                    case "contactname desc":
                        finalContacts = finalContacts.OrderByDescending(t => t.LastName);
                        break;
                    case "email asc":
                        finalContacts = finalContacts.OrderBy(t => t.Email);
                        break;
                    case "email desc":
                        finalContacts = finalContacts.OrderByDescending(t => t.Email);
                        break;
                    case "businesscity asc":
                        finalContacts = finalContacts.OrderBy(t => t.BusinessCity);
                        break;
                    case "businesscity desc":
                        finalContacts = finalContacts.OrderByDescending(t => t.BusinessCity);
                        break;
                    case "businesscountry asc":
                        finalContacts = finalContacts.OrderBy(t => t.BusinessCountry);
                        break;
                    case "businesscountry desc":
                        finalContacts = finalContacts.OrderByDescending(t => t.BusinessCountry);
                        break;
                    case "title asc":
                        finalContacts = finalContacts.OrderBy(t => t.Title);
                        break;
                    case "title desc":
                        finalContacts = finalContacts.OrderByDescending(t => t.Title);
                        break;
                    case "lastactivity asc":
                        finalContacts = finalContacts.OrderBy(t => t.LastActivityDate);
                        break;
                    case "lastactivity desc":
                        finalContacts = finalContacts.OrderByDescending(t => t.LastActivityDate);
                        break;
                    case "companyname asc":
                        finalContacts = finalContacts.OrderBy(t => t.CompanyName);
                        break;
                    case "companyname desc":
                        finalContacts = finalContacts.OrderByDescending(t => t.CompanyName);
                        break;
                    default:
                        break;
                }
            }

            // record count/ total pages 
            var recordCount = finalContacts.Count();
            var totalPages = 0;

            // apply paging
            if (filters.RecordsPerPage > 0 && filters.CurrentPage > 0)
            {
                finalContacts = finalContacts.Skip((filters.CurrentPage - 1) * filters.RecordsPerPage)
                             .Take(filters.RecordsPerPage);
                totalPages = recordCount % filters.RecordsPerPage == 0 ?
                                    (recordCount / filters.RecordsPerPage) :
                                  ((recordCount / filters.RecordsPerPage) + 1);
            }
            response.Contacts = finalContacts.ToList();
            

            return finalContacts.ToList();
        }

        public ContactListResponse GetContacts(ContactFilter filters)
        {
            
            var response = new ContactListResponse
            {
                Contacts = new List<Contact>()
            };

            // record count/ total pages 
            var recordCount = GetContactLists(filters).Count();
            var totalPages = 0;
            IEnumerable<Contact> finalContacts = Enumerable.Empty<Contact>();

            // apply paging
            if (filters.RecordsPerPage > 0 && filters.CurrentPage > 0)
            {
                finalContacts = GetContactLists(filters).Skip((filters.CurrentPage - 1) * filters.RecordsPerPage)
                             .Take(filters.RecordsPerPage);
                totalPages = recordCount % filters.RecordsPerPage == 0 ?
                                    (recordCount / filters.RecordsPerPage) :
                                  ((recordCount / filters.RecordsPerPage) + 1);
            }

            response.Contacts = GetContactLists(filters);

            // set total pages and records 
            if (response.Contacts.Count > 0)
            {
                response.TotalPages = totalPages;
                response.Records = recordCount;
                response.CurrentPage = filters.CurrentPage;
            }

            // set the return contact list
            return response;
        }


        public string GetContactSalesTeam(int contactId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var salesTeamUsers = context.LinkUserToContacts.Where(t => t.ContactId == contactId && !t.Deleted)
                                    .Select(t => t.UserName).ToList();

            return string.Join(", ", salesTeamUsers);
        }


        public int SaveContact(ContactSaveRequest request)
        {
            try
            {
                var securityContext = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection());
                var dataCenter = securityContext.GlobalSubscribers.Where(t => t.SubscriberId == request.Contact.SubscriberId).Select(t => t.DataCenter).FirstOrDefault();
                var connection = LoginUser.GetConnectionForDataCenter(dataCenter); 
                //var connection = LoginUser.GetConnection( );
                var context = new DbFirstFreightDataContext(connection);

                // save contact
                var contact = request.Contact;
                // check for contact
                var objContact = context.Contacts.FirstOrDefault(d => d.ContactId == contact.ContactId) ?? new Contact();
                // populate fields
                objContact.FirstName = (contact.FirstName + "").Trim();
                objContact.LastName = (contact.LastName + "").Trim();
                objContact.MiddleName = (contact.MiddleName + "").Trim();
                objContact.ContactName = (contact.FirstName + "").Trim() + (string.IsNullOrEmpty(contact.MiddleName) ? "" : (" " + contact.MiddleName)) + " " + (contact.LastName + "").Trim();
                objContact.Title = contact.Title;
                objContact.Hobbies = contact.Hobbies;
                objContact.Comments = contact.Comments;
                objContact.Email = contact.Email;
                objContact.MobilePhone = contact.MobilePhone;
                objContact.BusinessPhone = contact.BusinessPhone;
                objContact.BusinessAddress = contact.BusinessAddress;
                objContact.BusinessCity = contact.BusinessCity;
                objContact.BusinessCountry = contact.BusinessCountry;
                objContact.BusinessPostalCode = contact.BusinessPostalCode;
                objContact.Website = contact.Website;
                objContact.ContactType = contact.ContactType;

                // set company
                objContact.CompanyId = contact.CompanyId;
                if (contact.CompanyIdGlobal == 0)
                { 
                    var company = context.Companies.FirstOrDefault(c => c.CompanyId == contact.CompanyId && c.SubscriberId == contact.SubscriberId);
                    if (company != null)
                    {
                        objContact.CompanyIdGlobal = company.CompanyIdGlobal;
                        objContact.CompanyName = company.CompanyName;
                    }
                }
                 
                objContact.LastUpdate = DateTime.UtcNow;
                objContact.UpdateUserId = contact.UpdateUserId; 
                objContact.ReceiveEmail = contact.ReceiveEmail;
                objContact.Married = contact.Married;
                objContact.HasChildern = contact.HasChildern;
                objContact.OkToCall = contact.OkToCall;
                objContact.HolidayCards = contact.HolidayCards;
                objContact.FormerEmployee = contact.FormerEmployee;
                objContact.BusinessStateProvince = contact.BusinessStateProvince;
                objContact.PreviousEmployees = contact.PreviousEmployees;
                objContact.BirthdayDay = contact.BirthdayDay;
                objContact.BirthdayMonth = contact.BirthdayMonth;

                // user name
                var userName = new Users().GetUserFullNameById(contact.UpdateUserId, contact.SubscriberId);
                objContact.UpdateUserName = userName;
                // insert new contact
                var userActivityType = "Saved";
                if (objContact.ContactId < 1)
                {
                    objContact.SubscriberId = contact.SubscriberId;
                    objContact.ContactOwnerUserId = contact.UpdateUserId;
                    objContact.CreatedUserId = contact.UpdateUserId;
                    objContact.CreatedDate = DateTime.UtcNow;
                    objContact.CreatedUserName = userName;
                    context.Contacts.InsertOnSubmit(objContact);
                    userActivityType = "Created";
                }
                //update the contact owner if not already defined
                if (objContact.ContactOwnerUserId<1) objContact.ContactOwnerUserId = contact.ContactOwnerUserId;
                context.SubmitChanges();

                // save profile pic
                var profilePic = request.ProfilePic;
                if (profilePic != null)
                {
                    profilePic.ContactId = objContact.ContactId;
                    profilePic.SubscriberId = objContact.SubscriberId;
                    profilePic.DocumentTypeId = Convert.ToInt32(DocumentTypeEnum.ContactProfilePic);
                    profilePic.UploadedBy = objContact.UpdateUserId;
                    profilePic.UploadedByName = objContact.UpdateUserName;
                    new Documents().SaveDocument(profilePic);
                }

                // add link contact company
                var contactLinkCompany = context.LinkContactToCompanies
                        .FirstOrDefault(t => t.CompanyId == objContact.CompanyId && t.ContactId == objContact.ContactId && !t.Deleted);
                if (contactLinkCompany == null)
                {
                    context.LinkContactToCompanies.InsertOnSubmit(new LinkContactToCompany
                    {
                        ContactId = objContact.ContactId,
                        ContactName = new Contacts().GetContactNameFromId(objContact.ContactId, objContact.SubscriberId),
                        CompanyId = objContact.CompanyId,
                        CompanyName = new Companies().GetCompanyNameFromId(objContact.CompanyId, objContact.SubscriberId),
                        SubscriberId = objContact.SubscriberId,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdate = DateTime.UtcNow,
                        UpdateUserName = objContact.UpdateUserName,
                        UpdateUserId = objContact.UpdateUserId,
                        CreatedUserId = objContact.UpdateUserId,
                        CreatedUserName = objContact.UpdateUserName,
                        LinkType = "Company Contact"
                    });
                    context.SubmitChanges();
                }

                // add link contact user - created user
                var createdContactUser = context.LinkUserToContacts
                        .FirstOrDefault(t => t.ContactId == objContact.ContactId && t.UserId == objContact.CreatedUserId && !t.Deleted);
                if (createdContactUser == null)
                {
                    context.LinkUserToContacts.InsertOnSubmit(new LinkUserToContact
                    {
                        UserId = objContact.UpdateUserId,
                        UserName = new Users().GetUserFullNameById(objContact.UpdateUserId, objContact.SubscriberId),
                        ContactId = objContact.ContactId,
                        ContactName = objContact.ContactName,
                        SubscriberId = objContact.SubscriberId,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdate = DateTime.UtcNow,
                        UpdateUserName = objContact.UpdateUserName,
                        UpdateUserId = objContact.UpdateUserId,
                        CreatedUserId = objContact.UpdateUserId,
                        CreatedUserName = objContact.UpdateUserName,
                        LinkType = "",
                    });
                    context.SubmitChanges();
                }

                // sales team
                objContact.SalesTeam = GetContactSalesTeam(objContact.ContactId, objContact.SubscriberId);
                context.SubmitChanges();

                // Log Event 
                new Logging().LogUserAction(new UserActivity
                {
                    UserId = objContact.UpdateUserId,
                    ContactId = objContact.ContactId,
                    ContactName = objContact.ContactName,
                    UserActivityMessage = userActivityType+" Contact: " + contact.ContactName
                });

                //DONE: intercom Journey Step event
                var eventName = "Saved contact";
                var intercomHelper = new IntercomHelper();
                intercomHelper.IntercomTrackEvent(objContact.CreatedUserId, objContact.SubscriberId, eventName);

                //if (request.CreateSession)
                //{
                   // LoginUser.CreateQuickAddContactSession(contact);
                //}

                // update company last activity date
                new Companies().UpdateCompanyLastActivityDate(contact.CompanyId, contact.SubscriberId);

                // return contact Id
                return objContact.ContactId;

            }
            catch (Exception ex)
            {
                var error = new Crm6.App_Code.Shared.WebAppError
                {
                    ErrorCallStack = ex.StackTrace,
                    ErrorDateTime = DateTime.UtcNow,
                    RoutineName = "SaveContact",
                    PageCalledFrom = "Helper/Contacts",
                    SubscriberName = "",
                    ErrorMessage = ex.ToString(),
                    UserId = request.Contact.UpdateUserId
                };
                new Logging().LogWebAppError(error);
            }
            return 0;
        }


        public string GetContactProfilePicUrl(int contactId, int subscriberId)
        {
            return new Users().GetUserProfilePicUrl(contactId, subscriberId, "contact");
        }


        public ContactModel GetContact(int contactId, int subscriberId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                                 .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                                 .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
           // var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            // get the contact by id
            var contact = context.Contacts.FirstOrDefault(a => a.ContactId == contactId);
            if (contact != null)
            {
                // get profile picture
                var profilePic = new Documents().GetDocumentsByDocType(Convert.ToInt32(DocumentTypeEnum.ContactProfilePic), contactId, subscriberId).ToList();
                // set the retuen contact object
                return new ContactModel
                {
                    Contact = contact,
                    ProfilePicture = profilePic.Count > 0 ? profilePic[0] : null
                };
            }
            return null;
        }


        public bool DeleteContact(int contactId, int userId, int subscriberId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                                  .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                                  .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter); 
            var context = new DbFirstFreightDataContext(connection);
            var contact = context.Contacts.FirstOrDefault(t => t.ContactId == contactId);
            if (contact != null)
            {
                contact.Deleted = true;
                contact.DeletedUserId = userId;
                contact.DeletedDate = DateTime.UtcNow;
                contact.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);
                context.SubmitChanges();
                return true;
            }
            return false;
        }

        public List<Contact> GetCompanyContacts(int companyId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var contacts = context.Contacts.Where(t => t.CompanyId == companyId && !t.Deleted);

            return contacts.Any() ? contacts.ToList() : null;
        }


        public int GetCompanyContactCount(int companyId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var contactCount = context.Contacts.Count(t => t.CompanyId == companyId && !t.Deleted);
            return contactCount;
        }


        public int GetContactCompanyId(int contactId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var companyId = context.Contacts.Where(t => t.ContactId == contactId)
                                   .Select(t => t.CompanyId)
                                   .FirstOrDefault();
            return companyId;
        }


        public List<ContactSalesTeamMember> GetContactUsers(int contactId, int subscriberId)
        {
            var salesTeamMembers = new List<ContactSalesTeamMember>();
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var linkedUsers = context.LinkUserToContacts.Where(t => t.ContactId == contactId && !t.Deleted);

            foreach (var linkedUser in linkedUsers) {
                var user = new Users().GetUser(linkedUser.UserId, subscriberId);
                if (user!=null && !user.User.Deleted) 
                {
                    salesTeamMembers.Add(new ContactSalesTeamMember
                    {
                        SalesTeamMemberData = linkedUser,
                        User = user.User,
                        ProfilePicture = user.ProfilePicture
                    });
                }
            }
            return salesTeamMembers;
        }


        public bool AddContactUser(AddContactUserRequest request)
        {
            var loginConnection = LoginUser.GetLoginConnection();
            var loginContext = new Crm6.App_Code.Login.DbLoginDataContext(loginConnection);
            var globalUser = loginContext.GlobalUsers.FirstOrDefault(i => i.GlobalUserId == request.GlobalUserId);
            if (globalUser != null) {
                var connection = LoginUser.GetConnection( );
                var context = new DbFirstFreightDataContext(connection);
                var updatedUserName = new Users().GetUserFullNameById(request.UpdatedBy, request.ContactSubscriberId);
                var found = context.LinkUserToContacts.FirstOrDefault(i => i.ContactId == request.ContactId && i.UserId == globalUser.UserId && !i.Deleted);
                if (found == null)
                {
                    var contactUser = new LinkUserToContact();
                    contactUser.UserName = new Users().GetUserFullNameById(request.UserId, request.ContactSubscriberId);
                    contactUser.CreatedUserId = request.UpdatedBy;
                    contactUser.CreatedUserName = updatedUserName;
                    contactUser.CreatedDate = DateTime.UtcNow;
                    contactUser.UpdateUserId = request.UpdatedBy;
                    contactUser.UpdateUserName = updatedUserName;
                    contactUser.LastUpdate = DateTime.UtcNow;
                    contactUser.LinkType = "";
                    contactUser.ContactId = request.ContactId;
                    contactUser.UserId = globalUser.UserId;
                    contactUser.SalesTeamRole = request.SalesTeamRole;
                    contactUser.ContactName = GetContactNameFromId(request.ContactId, request.ContactSubscriberId);
                    context.LinkUserToContacts.InsertOnSubmit(contactUser);
                }
                else
                {
                    found.SalesTeamRole = request.SalesTeamRole;
                    found.LastUpdate = DateTime.UtcNow;
                    found.UpdateUserName = updatedUserName;
                    found.UpdateUserId = request.UpdatedBy;
                }
                
                // update company last activity date 
                var comapnyId = GetContactCompanyId(request.ContactId, request.ContactSubscriberId);
                if (comapnyId > 0) new Companies().UpdateCompanyLastActivityDate(comapnyId, request.ContactSubscriberId);
                // contact sales team
                var contact = context.Contacts.FirstOrDefault(t => t.ContactId == request.ContactId);
                if (contact != null) contact.SalesTeam = GetContactSalesTeam(contact.ContactId, contact.SubscriberId);
                    
                context.SubmitChanges();                
                return true;
            }
            return false;
        }


        public bool DeleteContactUser(int contactId, int userId, int deleteUserId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var contactUser = context.LinkUserToContacts.FirstOrDefault(t => t.UserId == deleteUserId && t.ContactId == contactId);
            if (contactUser != null)
            {
                contactUser.DeletedUserId = userId;
                contactUser.Deleted = true;
                contactUser.DeletedDate = DateTime.UtcNow;
                context.SubmitChanges();

                // sales team
                var contact = context.Contacts.FirstOrDefault(t => t.ContactId == contactUser.ContactId);
                if (contact != null)
                {
                    contact.SalesTeam = GetContactSalesTeam(contact.ContactId, contact.SubscriberId);
                    context.SubmitChanges();
                }
                return true;
            }
            return false;
        }

        public void UpdateContactLastActivityDate(int contactId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var contact = context.Contacts.FirstOrDefault(t => t.ContactId == contactId);
            if (contact != null)
            {
                contact.LastActivityDate = DateTime.UtcNow;
                context.SubmitChanges();
            }
        }

        public void UpdateContactLastUpdateDate(int contactId, int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            var contact = context.Contacts.Where(t => t.ContactId == contactId).FirstOrDefault();
            if (contact != null)
            {
                contact.LastUpdate = DateTime.UtcNow;
                contact.UpdateUserId = userId;
                contact.UpdateUserName = new Users().GetUserFullNameById(userId, subscriberId);
                context.SubmitChanges();
            }
        }

    }


    public class AddContactUserRequest
    {
        public int ContactId { get; set; }
        public int ContactSubscriberId { get; set; }
        public string LinkType { get; set; }
        public string SalesTeamRole { get; set; }
        public int UpdatedBy { get; set; }
        public int UserId { get; set; }
        public int GlobalUserId { get; set; }
        public int UserSubscriberId { get; set; }
    }

    public class ContactSalesTeamMember
    {
        public User User { get; set; }
        public DocumentModel ProfilePicture { get; set; }
        public LinkUserToContact SalesTeamMemberData { get; set; }
    }
}
