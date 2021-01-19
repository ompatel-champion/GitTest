using Crm6.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers
{
    public class Districts
    {

        public District GetDistrict(int districtId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Districts.FirstOrDefault(t => t.DistrictId == districtId && t.SubscriberId == subscriberId);
        }


        public District GetDistrictFromCode(string districtCode, int subscriberId)
        {
            var connection = LoginUser.GetConnection( );
            var context = new DbFirstFreightDataContext(connection);
            return context.Districts.FirstOrDefault(t => t.DistrictCode == districtCode 
            && t.SubscriberId == subscriberId && !t.Deleted);
        }


        public string GetDistrictName(int districtId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Districts.Where(t => t.DistrictId == districtId).Select(t => t.DistrictName).FirstOrDefault();
        }


        public string GetDistrictNameFromCode(string districtCode, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Districts.Where(t => t.DistrictCode == districtCode).Select(t => t.DistrictName).FirstOrDefault();
        }


        public List<District> GetDistricts(int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Districts.Where(t => !t.Deleted && t.SubscriberId == subscriberId)
                .OrderBy(t => t.DistrictName).Select(t => t).ToList();
        }


        public int SaveDistrict(District districtDetails)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            // get the district by id or create new district object
            var district = context.Districts.FirstOrDefault(t => t.SubscriberId == districtDetails.SubscriberId &&
                                                                             t.DistrictId == districtDetails.DistrictId) ?? new District();
            // fill details
            district.CountryCode = new Countries().GetCountryCodeFromCountryName(districtDetails.CountryName);
            district.CountryName = districtDetails.CountryName;
            district.DistrictManagerUserId = 0;
            var districtCode = districtDetails.DistrictCode;
            if (districtCode.Length >= 5)
            {
                districtCode = districtCode.Substring(0, 5); // limit to 5 characters
            }
            district.DistrictCode = districtCode;
            district.DistrictName = districtDetails.DistrictName;
            district.LastUpdate = DateTime.UtcNow;
            district.UpdateUserId = districtDetails.UpdateUserId;
            district.UpdateUserName = new Users().GetUserFullNameById(districtDetails.UpdateUserId, districtDetails.SubscriberId);

            if (district.DistrictId < 1)
            {
                // new district - insert
                district.SubscriberId = districtDetails.SubscriberId;
                district.CreatedUserId = district.UpdateUserId;
                district.CreatedUserName = district.UpdateUserName;
                district.CreatedDate = DateTime.UtcNow;
                context.Districts.InsertOnSubmit(district);
            }
            context.SubmitChanges();
            // return the district id
            return district.DistrictId;
        }


        public bool DeleteDistrict(int districtId, int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var district = context.Districts.FirstOrDefault(t => t.DistrictId == districtId);
            if (district != null)
            {
                district.Deleted = true;
                district.DeletedUserId = userId;
                district.DeletedDate = DateTime.UtcNow;
                district.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);
                context.SubmitChanges();
                return true;
            }
            return false;
        }


        public District GetDistrictForManager(int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Districts.FirstOrDefault(a => a.DistrictManagerUserId == userId);
        }

    }

}

