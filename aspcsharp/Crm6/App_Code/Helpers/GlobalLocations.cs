using System;
using System.Linq;
using Models;
using System.Collections.Generic;
using Crm6.App_Code.Shared;
using System.Web.SessionState;

namespace Helpers
{

    public class GlobalLocations : IRequiresSessionState
    {

        public GlobalLocationListResponse GetGlobalLocations(GlobalLocationFilter filters)
        {
            var response = new GlobalLocationListResponse
            {
                GlobalLocations = new List<GlobalLocation>()
            };

            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);

            // get global locations
            var globalLocations = (from globalLocation in sharedContext.GlobalLocations where !globalLocation.Deleted select globalLocation);

            // apply filters
            if (filters.GlobalLocationId > 0)
                globalLocations = globalLocations.Where(t => t.GlobalLocationId == filters.GlobalLocationId);

            if (!string.IsNullOrEmpty(filters.Keyword))
            {
                filters.Keyword = filters.Keyword.ToLower();
                globalLocations = globalLocations.Where(t => t.LocationCode.ToLower().Contains(filters.Keyword)
                                                 || t.LocationName.ToLower().Contains(filters.Keyword)
                                                 || t.CountryCode.ToLower().Contains(filters.Keyword)
                                                 || t.CountryName.ToLower().Contains(filters.Keyword));
            }

            // sort
            if (!string.IsNullOrEmpty(filters.SortBy))
            {
                switch (filters.SortBy.ToLower())
                {
                    case "locationcode asc":
                        globalLocations = globalLocations.OrderBy(t => t.LocationCode);
                        break;
                    case "locationcode desc":
                        globalLocations = globalLocations.OrderByDescending(t => t.LocationCode);
                        break;
                    case "locationname asc":
                        globalLocations = globalLocations.OrderBy(t => t.LocationName);
                        break;
                    case "locationname desc":
                        globalLocations = globalLocations.OrderByDescending(t => t.LocationName);
                        break;
                    case "countryname asc":
                        globalLocations = globalLocations.OrderBy(t => t.CountryName);
                        break;
                    case "countryname desc":
                        globalLocations = globalLocations.OrderByDescending(t => t.CountryName);
                        break;
                    default:
                        break;
                }
            }
            response.GlobalLocations = globalLocations.ToList();
            return response;
        }


        public int SaveGlobalLocation(GlobalLocationSaveRequest request)
        {
            var globalLocation = request.GlobalLocation;
            var sharedConnection = LoginUser.GetWritableSharedConnectionForSubscriberId(request.SubscriberId);
            var sharedContext = new DbSharedDataContext(sharedConnection);

            // check for location
            var objGlobalLocation = sharedContext.GlobalLocations.FirstOrDefault(d => d.GlobalLocationId == globalLocation.GlobalLocationId) ?? new GlobalLocation();
            // populate fields
            objGlobalLocation.Airport = globalLocation.Airport;
            objGlobalLocation.CountryCode = new Countries().GetCountryCodeFromCountryName(globalLocation.CountryName);
            objGlobalLocation.CountryName = globalLocation.CountryName;
            objGlobalLocation.InlandPort = globalLocation.InlandPort;
            objGlobalLocation.LastUpdate = DateTime.UtcNow;
            objGlobalLocation.LocationCode = globalLocation.LocationCode;
            objGlobalLocation.LocationName = globalLocation.LocationName;
            objGlobalLocation.MultiModal = globalLocation.MultiModal;
            objGlobalLocation.RailTerminal = globalLocation.RailTerminal;
            objGlobalLocation.RoadTerminal = globalLocation.RoadTerminal;
            objGlobalLocation.SeaPort = globalLocation.SeaPort;
            objGlobalLocation.UpdateUserIdGlobal = globalLocation.UpdateUserIdGlobal;
            // user name
            var userName = new Users().GetUserFullNameByUserIdGlobal(globalLocation.UpdateUserIdGlobal);
            objGlobalLocation.UpdateUserName = userName;

            // insert new global location
            if (objGlobalLocation.GlobalLocationId < 1)
            {
                objGlobalLocation.CreatedUserIdGlobal = globalLocation.UpdateUserIdGlobal;
                objGlobalLocation.CreatedDate = DateTime.UtcNow;
                objGlobalLocation.CreatedUserName = userName;
                sharedContext.GlobalLocations.InsertOnSubmit(objGlobalLocation);
            }
            sharedContext.SubmitChanges();

            // return global location Id
            return objGlobalLocation.GlobalLocationId;


        }


        public GlobalLocation GetGlobalLocation(int globalLocationId)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);

            // get the global location by id
            var globalLocation = sharedContext.GlobalLocations.FirstOrDefault(a => a.GlobalLocationId == globalLocationId);
            // set the return location object
            return globalLocation;
        }


        public GlobalLocation GetGlobalLocationByCode(string locationCode)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            // get the global location by code
            var globalLocation = sharedContext.GlobalLocations.FirstOrDefault(a => a.LocationCode == locationCode);
            // set the return global location object
            return globalLocation;
        }


        public bool DeleteGlobalLocation(int globalLocationId, int userIdGlobal, int subscriberId)
        {
            var sharedConnection = LoginUser.GetWritableSharedConnectionForSubscriberId(subscriberId);
            var sharedContext = new DbSharedDataContext(sharedConnection);

            var globalLocation = sharedContext.GlobalLocations.FirstOrDefault(t => t.GlobalLocationId == globalLocationId);
            if (globalLocation != null)
            {
                globalLocation.Deleted = true;
                globalLocation.DeletedUserIdGlobal = userIdGlobal;
                globalLocation.DeletedDate = DateTime.UtcNow;
                globalLocation.DeletedUserName = new Users().GetUserFullNameByUserIdGlobal(userIdGlobal);
                sharedContext.SubmitChanges();
                return true;
            }
            return false;
        }

    }

    public class GlobalLocationSaveRequest
    {
        public GlobalLocation GlobalLocation { get; set; }
        public int SubscriberId { get; set; }
    }

}
