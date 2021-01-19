using Helpers;
using System.Collections.Generic;
using System.Web.Http;
using Models;
using Crm6.App_Code;

namespace API
{

    public class DropdownController : ApiController
    {

        [AcceptVerbs("GET")]
        public List<SelectList> GetAllSalesReps([FromUri]int subscriberId)
        {
            return new DropdownHelper().GetAllSalesReps(subscriberId);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetCommodities([FromUri]int subscriberId)
        {
            return new DropdownHelper().GetCommodities(subscriberId);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetUserCompanies([FromUri]int subscriberId, int userId, string keyword)
        {
            return new DropdownHelper().GetUserCompanies(subscriberId, userId, keyword);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetCompanies([FromUri]int subscriberId, string keyword)
        {
            return new DropdownHelper().GetCompanies(subscriberId, keyword);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetCompanySegments([FromUri]int subscriberId)
        {
            return new DropdownHelper().GetCompanySegments(subscriberId);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetContacts([FromUri]int subscriberId, int companyId, string keyword)
        {
            return new DropdownHelper().GetContacts(subscriberId, companyId, keyword);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetContacts([FromUri]int subscriberId, int companyId)
        {
            return new DropdownHelper().GetContacts(subscriberId, companyId, "");
        }


        // TODO: move to other class???
        [AcceptVerbs("GET")]
        public string GetCurrencySymbolFromCode([FromUri]string currencyCode)
        {
            return new Currencies().GetCurrencySymbolFromCode(currencyCode);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetDeals([FromUri]int subscriberId, int companyId)
        {
            return new DropdownHelper().GetDeals(subscriberId, companyId);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetDistricts([FromUri]int subscriberId, int userId, string countryNames)
        {
            return new DropdownHelper().GetDistricts(subscriberId, userId, countryNames);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetGlobalCompanies([FromUri]int subscriberId, string keyword)
        {
            return new DropdownHelper().GetGlobalCompanies(subscriberId, keyword);
        }


        [AcceptVerbs("GET")]
        public string GetLatestMonday()
        {
            return new DropdownHelper().GetLatestMonday();
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetLocations([FromUri]int subscriberId, string keyword, string countryNames, string districtCodes)
        {
            return new DropdownHelper().GetLocations(subscriberId, keyword, countryNames, districtCodes);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetLostReasons([FromUri]int subscriberId)
        {
            return new DropdownHelper().GetLostReasons(subscriberId);
        }


        [AcceptVerbs("GET")]
        public MondaysResponse GetMondays([FromUri]int year)
        {
            return new DropdownHelper().GetMondays(year);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetSalesReps([FromUri]int subscriberId)
        {
            return new DropdownHelper().GetSalesReps(subscriberId);
        }

        [AcceptVerbs("GET")]
        public List<SelectList> GetSalesRepGlobalUserIds([FromUri]int subscriberId)
        {
            return new DropdownHelper().GetSalesRepGlobalUserIds(subscriberId);
        }

        [AcceptVerbs("GET")]
        public List<SelectList> GetLinkedSubsciberSalesRepGlobalUserIds([FromUri]int subscriberId)
        {
            return new DropdownHelper().GetLinkedSubsciberSalesRepGlobalUserIds(subscriberId);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetLinkedSubsciberSalesRepGlobalUserIds([FromUri]int subscriberId, string prefix)
        {
            return new DropdownHelper().GetLinkedSubsciberSalesRepGlobalUserIds(subscriberId, prefix);
        }




        [AcceptVerbs("GET")]
        public List<SelectList> GetSalesRepsByLocation([FromUri]int subscriberId, string locationCode)
        {
            return new DropdownHelper().GetSalesRepsByLocation(subscriberId, locationCode);
        }


        [AcceptVerbs("POST")]
        public List<SelectList> GetAllSalesReps([FromBody]GlobalSalesRepSearchRequest request)
        {
            return new DropdownHelper().GetAllSalesReps(request);
        }




        [AcceptVerbs("GET")]
        public List<OriginDestinationLoction> GetServiceLocations([FromUri]string countryCodes, string service, string keyword)
        {
            return new DropdownHelper().GetServiceLocations(countryCodes, service, keyword);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetTimeZones(int userId, int subscriberId)
        {
            return new DropdownHelper().GetTimeZones(userId, subscriberId);
        }



        [AcceptVerbs("POST")]
        public List<SelectList> GetUserLocations([FromBody]UserLocationRequest request)
        {
            return new DropdownHelper().GetUserLocations(request);
        }


        [AcceptVerbs("GET")]
        public List<Location> GetUserLocations([FromUri]int userId, int subscriberId, string countryCode)
        {
            return new Users().GetUserLocations(userId, subscriberId, countryCode);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetUsers([FromUri]int subscriberId, string keyword)
        {
            return new DropdownHelper().GetUsers(subscriberId, keyword);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetWonReasons([FromUri]int subscriberId)
        {
            return new DropdownHelper().GetWonReasons(subscriberId);
        }

    }

}
