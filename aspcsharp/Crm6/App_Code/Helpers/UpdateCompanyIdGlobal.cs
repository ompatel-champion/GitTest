using Crm6.App_Code.Shared;
using System;
using System.Linq;
using Crm6.App_Code;


namespace Helpers
{

    public class UpdateCompanyIdGlobal
    {

        // GlobalCalendarEvents
        // GlobalDeals
        // GlobalDocuments
        // GlobalNotes
        // GlobalTasks


        // update CompanyIdGlobal field in various tables
        public string CompanyIdGlobalUpdates(string connection, string dataCenter)
        {
            var returnMessage = "";

            // calendar events
            returnMessage += CompanyIdGlobal_UpdateCalendarEvents(connection, dataCenter);

            // companies
            returnMessage += CompanyIdGlobal_UpdateCompanies(connection, dataCenter);

            // deals
            returnMessage += CompanyIdGlobal_UpdateDeals(connection, dataCenter);

            // documents
            returnMessage += CompanyIdGlobal_UpdateDocuments(connection, dataCenter);

            // link contact to company
            returnMessage += CompanyIdGlobal_LinkContactToCompany(connection, dataCenter);

            // notes
            returnMessage += CompanyIdGlobal_UpdateNotes(connection, dataCenter);

            // tasks
          //  returnMessage += CompanyIdGlobal_UpdateTasks(connection, dataCenter);

            // add missing global companies for data center
            returnMessage += AddMissingGlobalCompanies(connection, dataCenter);

            // write to text file
            Utils.WriteLogFile(returnMessage + Environment.NewLine);

            return returnMessage;
        }


        private string CompanyIdGlobal_UpdateCalendarEvents(string connection, string dataCenter)
        {
            var startTime = DateTime.Now;
            var context = new DbFirstFreightDataContext(connection);

            var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(dataCenter);
            var gContext = new DbSharedDataContext(sharedConnection);
            var endTime = DateTime.Now;
            var elapsedTime = endTime - startTime;
            var recordCount = 0;
            startTime = DateTime.Now;
            //var events = context.CalendarEvents.Where(t => t.CompanyId > 0 && !t.Deleted)).ToList();
            // only records without CompanyIdGlobal
            //var events = context.CalendarEvents.Where(t => t.CompanyId > 0  && !t.Deleted && t.CompanyIdGlobal == 0).ToList();
            //foreach (var e in events)
            //{
            //    var gCompany = gContext.GlobalCompanies.FirstOrDefault(t => t.CompanyId == e.CompanyId
            //                                                                && t.SubscriberId == e.SubscriberId
            //                                                                && !t.Deleted);
            //    if (gCompany != null)
            //    {
            //        e.CompanyIdGlobal = gCompany.GlobalCompanyId;
            //        context.SubmitChanges();
            //        recordCount = recordCount + 1;
            //    }
            //}
            //endTime = DateTime.Now;
            //elapsedTime = endTime - startTime;
            var returnMessage = recordCount + " calendar events | " + elapsedTime + " | " + dataCenter.ToUpper() + Environment.NewLine;
            return returnMessage;
        }


        private string CompanyIdGlobal_UpdateCompanies(string connection, string dataCenter)
        {
            var startTime = DateTime.Now;
            var context = new DbFirstFreightDataContext(connection);
            var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(dataCenter);
            var gContext = new DbSharedDataContext(sharedConnection);
            var endTime = DateTime.Now;
            var elapsedTime = endTime - startTime;
            var recordCount = 0;
            //var companies = context.Companies.Where(t => t.CompanyId > 0 && !t.Deleted).ToList();
            // only records without CompanyIdGlobal
            var companies = context.Companies.Where(t => t.CompanyId > 0 && !t.Deleted && t.CompanyIdGlobal == 0).ToList();
            foreach (var company in companies)
            {
                var gCompany = gContext.GlobalCompanies.FirstOrDefault(t => t.CompanyId == company.CompanyId
                                                                            && t.SubscriberId == company.SubscriberId
                                                                            && !t.Deleted);
                if (gCompany != null)
                {
                    company.CompanyIdGlobal = gCompany.GlobalCompanyId;
                    context.SubmitChanges();
                    recordCount = recordCount + 1;
                }
            }
            endTime = DateTime.Now;
            elapsedTime = endTime - startTime;
            var returnMessage = recordCount + " companies | " + elapsedTime + " | " + dataCenter.ToUpper() + Environment.NewLine;
            return returnMessage;
        }


        private string CompanyIdGlobal_UpdateDeals(string connection, string dataCenter)
        {
            var startTime = DateTime.Now;
            var context = new DbFirstFreightDataContext(connection);
            var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(dataCenter);
            var gContext = new DbSharedDataContext(sharedConnection);
            var endTime = DateTime.Now;
            var elapsedTime = endTime - startTime;
            var recordCount = 0;
            //var deals = context.Deals.Where(t => t.CompanyId > 0 && !t.Deleted)).ToList();
            // only records without CompanyIdGlobal
            var deals = context.Deals.Where(t => t.CompanyId > 0  && !t.Deleted && t.CompanyIdGlobal == 0).ToList();
            foreach (var deal in deals)
            {
                var gCompany = gContext.GlobalCompanies.FirstOrDefault(t => t.CompanyId == deal.CompanyId 
                                                                            && t.SubscriberId == deal.SubscriberId 
                                                                            && !t.Deleted);
                if (gCompany != null)
                {
                    deal.CompanyIdGlobal = gCompany.GlobalCompanyId;
                    context.SubmitChanges();
                    recordCount = recordCount + 1;
                }
            }
            endTime = DateTime.Now;
            elapsedTime = endTime - startTime;
            var returnMessage = recordCount + " deals | " + elapsedTime + " | " + dataCenter.ToUpper() + Environment.NewLine;
            return returnMessage;
        }


        private string CompanyIdGlobal_UpdateDocuments(string connection, string dataCenter)
        {
            var startTime = DateTime.Now;
            var context = new DbFirstFreightDataContext(connection);
            var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(dataCenter);
            var gContext = new DbSharedDataContext(sharedConnection);
            var endTime = DateTime.Now;
            var elapsedTime = endTime - startTime;
            var recordCount = 0;
            //var documents = context.Documents.Where(t => t.CompanyId > 0 && !t.Deleted)).ToList();
            // only records without CompanyIdGlobal
            var documents = context.Documents.Where(t => t.CompanyId > 0  && !t.Deleted && t.CompanyIdGlobal == 0).ToList();
            foreach (var document in documents)
            {
                var gCompany = gContext.GlobalCompanies.FirstOrDefault(t => t.CompanyId == document.CompanyId 
                                                                            && t.SubscriberId == document.SubscriberId 
                                                                            && !t.Deleted);
                if (gCompany != null)
                {
                    document.CompanyIdGlobal = gCompany.GlobalCompanyId;
                    context.SubmitChanges();
                    recordCount = recordCount + 1;
                }
            }
            endTime = DateTime.Now;
            elapsedTime = endTime - startTime;
            var returnMessage = recordCount + " documents | " + elapsedTime + " | " + dataCenter.ToUpper() + Environment.NewLine;
            return returnMessage;
        }


        private string CompanyIdGlobal_LinkContactToCompany(string connection, string dataCenter)
        {
            var startTime = DateTime.Now;
            var context = new DbFirstFreightDataContext(connection);
            var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(dataCenter);
            var gContext = new DbSharedDataContext(sharedConnection);
            var endTime = DateTime.Now;
            var elapsedTime = endTime - startTime;
            var recordCount = 0;
            //var links = context.LinkContactToCompanies.Where(t => t.CompanyId > 0).ToList();
            // only records without CompanyIdGlobal
            var links = context.LinkContactToCompanies.Where(t => t.CompanyId > 0 && t.CompanyIdGlobal == 0).ToList();
            foreach (var link in links)
            {
                var gCompany = gContext.GlobalCompanies.FirstOrDefault(t => t.CompanyId == link.CompanyId 
                                                                            && t.SubscriberId == link.SubscriberId 
                                                                            && !t.Deleted);
                if (gCompany != null)
                {
                    link.CompanyIdGlobal = gCompany.GlobalCompanyId;
                    context.SubmitChanges();
                    recordCount = recordCount + 1;
                }
            }
            endTime = DateTime.Now;
            elapsedTime = endTime - startTime;
            var returnMessage = recordCount + " link contact to company | " + elapsedTime + " | " + dataCenter.ToUpper() + Environment.NewLine;
            return returnMessage;
        }


        private string CompanyIdGlobal_UpdateNotes(string connection, string dataCenter)
        {
            var startTime = DateTime.Now;
            var context = new DbFirstFreightDataContext(connection);
            var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(dataCenter);
            var gContext = new DbSharedDataContext(sharedConnection);
            var endTime = DateTime.Now;
            var elapsedTime = endTime - startTime;
            var recordCount = 0;
            //var notes = context.Notes.Where(t => t.CompanyId > 0 && !t.Deleted)).ToList();
            // only records without CompanyIdGlobal
            var notes = context.Notes.Where(t => t.CompanyId > 0  && !t.Deleted && t.CompanyIdGlobal == 0).ToList();
            foreach (var note in notes)
            {
                var gCompany = gContext.GlobalCompanies.FirstOrDefault(t => t.CompanyId == note.CompanyId 
                                                                            && t.SubscriberId == note.SubscriberId 
                                                                            && !t.Deleted);
                if (gCompany != null)
                {
                    note.CompanyIdGlobal = gCompany.GlobalCompanyId;
                    context.SubmitChanges();
                    recordCount = recordCount + 1;
                }
            }
            endTime = DateTime.Now;
            elapsedTime = endTime - startTime;
            var returnMessage = recordCount + " notes | " + elapsedTime + " | " + dataCenter.ToUpper() + Environment.NewLine;
            return returnMessage;
        }


        //private string CompanyIdGlobal_UpdateTasks(string connection, string dataCenter)
        //{
        //    var startTime = DateTime.Now;
        //    var context = new DbFirstFreightDataContext(connection);
        //    var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(dataCenter);
        //    var gContext = new DbSharedDataContext(sharedConnection);
        //    var endTime = DateTime.Now;
        //    var elapsedTime = endTime - startTime;
        //    var recordCount = 0;
        //    //var tasks = context.Tasks.Where(t => t.CompanyId > 0 && !t.Deleted)).ToList();
        //    // only records without CompanyIdGlobal
        //    var tasks = context.Tasks.Where(t => t.CompanyId > 0 && !t.Deleted && t.CompanyIdGlobal == 0).ToList();

        //    foreach (var task in tasks)
        //    {
        //        var gCompany = gContext.GlobalCompanies.FirstOrDefault(t => t.CompanyId == task.CompanyId 
        //                                                                    && t.SubscriberId == task.SubscriberId 
        //                                                                    && !t.Deleted);
        //        if (gCompany != null)
        //        {
        //            task.CompanyIdGlobal = gCompany.GlobalCompanyId;
        //            context.SubmitChanges();
        //            recordCount = recordCount + 1;
        //        }
        //    }
        //    endTime = DateTime.Now;
        //    elapsedTime = endTime - startTime;
        //    var returnMessage = recordCount + " tasks | " + elapsedTime + " | " + dataCenter.ToUpper() + Environment.NewLine;
        //    return returnMessage;
        //}


        private string AddMissingGlobalCompanies(string connection, string dataCenter)
        {
            var startTime = DateTime.Now;
            var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(dataCenter);
            var endTime = DateTime.Now;
            var elapsedTime = endTime - startTime;
            var recordCount = 0;

            // data center connection
            var dataCenterContext = new DbFirstFreightDataContext(connection);

            // shared connection
            var sharedContext = new DbSharedDataContext(sharedConnection);

            // login connection
            var user = LoginUser.GetLoggedInUser();
            var subscriberId = user.Subscriber.SubscriberId;

            var loginConnection = LoginUser.GetLoginConnection();
            var loginContext = new Crm6.App_Code.Login.DbLoginDataContext(loginConnection);

            // get companies from data center without CompanyIdGlobal
            var companies = dataCenterContext.Companies.Where(t => !t.Deleted && t.CompanyIdGlobal == 0).ToList();

            // loop through data center companies and add to GlobalCompanies
            foreach (var company in companies)
            {
                // get the global company by CompanyId
                var globalCompany = sharedContext.GlobalCompanies.FirstOrDefault(t => t.CompanyId == company.CompanyId 
                                                                                      && t.SubscriberId == company.SubscriberId 
                                                                                      && !t.Deleted);

                // check if global company exists for data center CompanyId
                if (globalCompany == null)
                {
                    // Add global company record
                    globalCompany = new GlobalCompany();

                    globalCompany.Active = company.Active;
                    globalCompany.Address = string.IsNullOrEmpty(company.Address) ? "" : company.Address;
                    globalCompany.City = string.IsNullOrEmpty(company.City) ? "" : company.City;
                    globalCompany.CompanyCode = company.CompanyCode;
                    globalCompany.CompanyId = company.CompanyId;
                    globalCompany.CompanyName = string.IsNullOrEmpty(company.CompanyName) ? "" : company.CompanyName;

                    //globalCompany.CompanyOwnerUserId = company.CompanyOwnerUserId;

                    globalCompany.CompanyTypes = company.CompanyTypes;
                    globalCompany.CountryName = string.IsNullOrEmpty(company.CountryName) ? "" : company.CountryName;
                    globalCompany.CreatedDate = company.CreatedDate;
                    globalCompany.CreatedUserId = company.CreatedUserId;
                    globalCompany.CreatedUserName = company.CreatedUserName;
                    globalCompany.DataCenter = new Subscribers().GetDataCenter(company.SubscriberId); ;
                    globalCompany.Division = company.Division;
                    globalCompany.IsCustomer = company.IsCustomer;
                    globalCompany.LastActivityDate = company.LastActivityDate ?? DateTime.UtcNow;
                    globalCompany.LastUpdate = DateTime.UtcNow;
                    globalCompany.Phone = string.IsNullOrEmpty(company.Phone) ? "" : company.Phone;
                    globalCompany.PostalCode = string.IsNullOrEmpty(company.PostalCode) ? "" : company.PostalCode;
                    globalCompany.SalesTeam = company.SalesTeam;
                    globalCompany.StateProvince = string.IsNullOrEmpty(company.StateProvince) ? "" : company.StateProvince;
                    globalCompany.SubscriberId = company.SubscriberId;
                    globalCompany.UpdateUserId = company.UpdateUserId;
                    globalCompany.UpdateUserName = company.UpdateUserName;

                    // add/update verify method
                    sharedContext.GlobalCompanies.InsertOnSubmit(globalCompany);
                    sharedContext.SubmitChanges();

                    // increment recordCount for global companies created
                    recordCount += 1;

                    if (company.CompanyOwnerUserId > 0)
                    {
                        var globalUser = loginContext.GlobalUsers.FirstOrDefault(t => t.UserId == company.CreatedUserId 
                                                                                      && t.SubscriberId == company.SubscriberId);
                        if (globalUser != null)
                        {
                            // check if LinkGlobalCompanyGlobalUsers record exists
                            var linkGlobalCompanyUser = sharedContext.LinkGlobalCompanyGlobalUsers
                                    .FirstOrDefault(c => c.GlobalCompanyId == globalCompany.GlobalCompanyId 
                                                         && c.GlobalUserId == globalUser.GlobalUserId 
                                                         && !c.Deleted);
                            if (linkGlobalCompanyUser == null)
                            {
                                // create CompanyOwnerUserId LinkGlobalCompanyGlobalUsers record
                                var companyUser = new LinkGlobalCompanyGlobalUser();
                                companyUser.GlobalUserName = globalUser.FullName;
                                companyUser.UserSubscriberId = globalUser.SubscriberId;
                                companyUser.GlobalUserId = globalUser.GlobalUserId;
                                companyUser.CreatedBy = globalUser.GlobalUserId;
                                companyUser.CreatedByName = globalUser.FullName;
                                companyUser.CreatedDate = DateTime.UtcNow;
                                companyUser.LinkType = "";
                                companyUser.GlobalCompanyName = globalCompany.CompanyName;
                                companyUser.GlobalCompanyId = globalCompany.GlobalCompanyId;
                                companyUser.CompanySubscriberId = globalCompany.SubscriberId;
                                sharedContext.LinkGlobalCompanyGlobalUsers.InsertOnSubmit(companyUser);
                                sharedContext.SubmitChanges();
                            }
                        }
                    }

                    // add LinkGlobalCompanyGlobalUsers records for GlobalCompany
                    var linkedUsers = dataCenterContext.LinkUserToCompanies.Where(t => t.CompanyId == company.CompanyId 
                                                                                       && !t.Deleted).ToList();

                    foreach (var linkedUser in linkedUsers)
                    {
                        // get global user
                        var globaluser = loginContext.GlobalUsers.FirstOrDefault(t => t.SubscriberId == linkedUser.SubscriberId 
                                                                                      && t.UserId == linkedUser.UserId);
                        if (globaluser != null)
                        {
                            // check if linked user to company record already exists
                            var found = sharedContext.LinkGlobalCompanyGlobalUsers
                                                     .FirstOrDefault(t => t.GlobalCompanyId == globalCompany.GlobalCompanyId
                                                                    && t.GlobalUserId == globaluser.GlobalUserId
                                                                    && !t.Deleted);
                            if (found == null)
                            {
                                // add link global user to global company record
                                var globalLinkUser = new LinkGlobalCompanyGlobalUser
                                {
                                    GlobalCompanyId = globalCompany.GlobalCompanyId,
                                    GlobalUserId = globaluser.GlobalUserId,
                                    CreatedDate = linkedUser.CreatedDate,
                                    CreatedBy = linkedUser.CreatedUserId,
                                    CreatedByName = linkedUser.CreatedUserName,
                                    GlobalCompanyName = linkedUser.CompanyName,
                                    GlobalUserName = linkedUser.UserName,
                                    CompanySubscriberId = globalCompany.SubscriberId,
                                    UserSubscriberId = globaluser.SubscriberId
                                };
                                sharedContext.LinkGlobalCompanyGlobalUsers.InsertOnSubmit(globalLinkUser);
                                sharedContext.SubmitChanges();
                            }
                        }
                    }
                }
            }
            endTime = DateTime.Now;
            elapsedTime = endTime - startTime;
            var returnMessage = recordCount + " global companies created | " + elapsedTime + " | " + dataCenter.ToUpper() + Environment.NewLine;
            return returnMessage;
        }

    }
}
