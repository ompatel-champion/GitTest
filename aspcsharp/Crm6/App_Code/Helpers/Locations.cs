using System;
using System.Linq;
using Models;
using System.Collections.Generic;
using Crm6.App_Code;

namespace Helpers
{

    public class Locations
    {

        public LocationListResponse GetLocations(LocationFilter filters)
        {
            var response = new LocationListResponse
            {
                Locations = new List<Location>()
            };

            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            // get locations
            var locations = (from location in context.Locations where !location.Deleted select location);

            // apply filters
            if (filters.SubscriberId > 0)
                locations = locations.Where(t => t.SubscriberId == filters.SubscriberId);
            if (filters.LocationId > 0)
                locations = locations.Where(t => t.LocationId == filters.LocationId);

            if (!string.IsNullOrEmpty(filters.Keyword))
            {
                filters.Keyword = filters.Keyword.ToLower();
                locations = locations.Where(t => t.LocationName.ToLower().Contains(filters.Keyword)
                                                 || t.LocationName.ToLower().Contains(filters.Keyword)
                                                 || t.StateProvince.ToLower().Contains(filters.Keyword));
            }

            // sort
            if (!string.IsNullOrEmpty(filters.SortBy))
            {
                switch (filters.SortBy.ToLower())
                {
                    case "createddate asc":
                        locations = locations.OrderBy(t => t.CreatedDate);
                        break;
                    case "createddate desc":
                        locations = locations.OrderByDescending(t => t.CreatedDate);
                        break;
                    case "locationname asc":
                        locations = locations.OrderBy(t => t.LocationName);
                        break;
                    case "locationname desc":
                        locations = locations.OrderByDescending(t => t.LocationName);
                        break;
                    default:
                        break;
                }
            }
            response.Locations = locations.ToList();
            return response;
        }


        public int SaveLocation(LocationSaveRequest request)
        {
            var subscriberId = request.Location.SubscriberId;
            var connection = LoginUser.GetConnection( );
            var context = new DbFirstFreightDataContext(connection);

            // save location
            var location = request.Location;
            // check for location
            var objLocation = context.Locations.FirstOrDefault(d => d.LocationId == location.LocationId) ?? new Location();
            // populate fields
            objLocation.Address = location.Address;
            objLocation.City = location.City;
            objLocation.Comments = location.Comments;
            objLocation.CountryCode = new Countries().GetCountryCodeFromCountryName(location.CountryName);
            objLocation.CountryName = location.CountryName;
            // district code
            if (string.IsNullOrEmpty(location.DistrictCode) || location.DistrictCode == "0")
            {
                objLocation.DistrictCode = "";
                objLocation.DistrictName = "";
            }
            else
            {
                objLocation.DistrictCode = location.DistrictCode;
                objLocation.DistrictName = new Districts().GetDistrictNameFromCode(location.DistrictCode, subscriberId);
            }
             
            objLocation.Email = location.Email;
            objLocation.Fax = location.Fax;
            objLocation.LastUpdate = DateTime.UtcNow;
            objLocation.Latitude = location.Latitude;
            objLocation.LocationManagerUserId = 0;
            objLocation.LocationName = location.LocationName; 
            objLocation.LocationCode = location.LocationCode;
            objLocation.LocationType = location.LocationType;
            objLocation.Longitude = location.Longitude;
            objLocation.Phone = location.Phone;
            objLocation.Phone2 = location.Phone2;
            objLocation.Phone3 = location.Phone3;
            objLocation.PostalCode = location.PostalCode;
            objLocation.RegionName = location.RegionName; 
            objLocation.StateProvince = location.StateProvince ?? "";
            objLocation.UpdateUserId = location.UpdateUserId;
            // user name
            var userName = new Users().GetUserFullNameById(location.UpdateUserId, location.SubscriberId);
            objLocation.UpdateUserName = userName;

            // insert new location
            if (objLocation.LocationId < 1)
            {
                objLocation.SubscriberId = location.SubscriberId;
                objLocation.CreatedUserId = location.UpdateUserId;
                objLocation.CreatedDate = DateTime.UtcNow;
                objLocation.CreatedUserName = userName;
                context.Locations.InsertOnSubmit(objLocation);
            }
            context.SubmitChanges();

            // Save Location Pic
            var locationPic = request.LocationPic;
            if (locationPic != null)
            {
                locationPic.LocationId = objLocation.LocationId;
                locationPic.SubscriberId = objLocation.SubscriberId;
                locationPic.DocumentTypeId = Convert.ToInt32(DocumentTypeEnum.LocationPic);
                locationPic.UploadedBy = objLocation.UpdateUserId;
                locationPic.UploadedByName = objLocation.UpdateUserName;
                new Documents().SaveDocument(locationPic);
            }
            else
            {
                // delete the profile picture if existing
                new Documents().DeleteFilesForDocType(new DocumentModel
                {
                    DocumentTypeId = Convert.ToInt32(DocumentTypeEnum.LocationPic),
                    LocationId = objLocation.LocationId,
                    UploadedBy = location.UpdateUserId
                });
            }
            // return location Id
            return objLocation.LocationId;
        }


        public string GetLocationPicUrl(int locationId)
        {
            var locationPic = new Documents().GetDocumentsByDocType(Convert.ToInt32(DocumentTypeEnum.LocationPic), locationId).ToList();
            if (locationPic.Count > 0)
                return locationPic[0].DocumentUrl;
            return "";
        }


        public Location GetLocation(int locationId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            // get the location by id  
            var location = context.Locations.FirstOrDefault(a => a.LocationId == locationId);
            // set the return location object
            return location;
        }


        public Location GetLocationByCode(string locationCode, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            // get the location by code  
            var location = context.Locations.FirstOrDefault(a => a.LocationCode == locationCode); 
            // set the return location object
            return location;
        }


        public bool DeleteLocation(int locationId, int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var location = context.Locations.FirstOrDefault(t => t.LocationId == locationId);
            if (location != null)
            {
                location.Deleted = true;
                location.DeletedUserId = userId;
                location.DeletedDate = DateTime.UtcNow;
                location.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);
                context.SubmitChanges();
                return true;
            }
            return false;
        }

    }

}
