using System.Collections.Generic;
using System.Linq;
using Models;
using System;
using Crm6.App_Code;
using Crm6.App_Code.Helpers;

namespace Helpers
{
    public class Lanes
    {


        public LaneModel GetLane(int laneId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var lane = context.Lanes.FirstOrDefault(t => t.LaneId == laneId);
            if (lane != null)
            {
                var laneModel = new LaneModel
                {
                    Lane = lane
                };

                // origin location
                var originUnlocoOrIataLocation = new SelectList();
                if (!string.IsNullOrEmpty(lane.OriginIataCode))
                {
                    if (lane.OriginIataCode == "Various")
                    {
                        originUnlocoOrIataLocation.SelectValue = "Various";
                        originUnlocoOrIataLocation.SelectText = "Various";
                    }
                    else
                    {
                        // get IATA code display value
                        originUnlocoOrIataLocation.SelectValue = lane.OriginIataCode;
                        originUnlocoOrIataLocation.SelectText = new Deals().GetGlobalLocationDisplayValue(lane.OriginIataCode, lane.OriginCountryName, lane.Service);
                    }
                }
                else if (!string.IsNullOrEmpty(lane.OriginUnlocoCode))
                {
                    // get UNLOCO code display value
                    originUnlocoOrIataLocation.SelectValue = lane.OriginUnlocoCode;
                    originUnlocoOrIataLocation.SelectText = new Deals().GetGlobalLocationDisplayValue(lane.OriginUnlocoCode, lane.OriginCountryName, lane.Service);
                }
                laneModel.OriginLocation = originUnlocoOrIataLocation;

                // destination location
                var destinationUnlocoOrIataLocation = new SelectList();
                if (!string.IsNullOrEmpty(lane.DestinationIataCode))
                {
                    if (lane.DestinationIataCode == "Various")
                    {
                        destinationUnlocoOrIataLocation.SelectValue = "Various";
                        destinationUnlocoOrIataLocation.SelectText = "Various";
                    }
                    else
                    {
                        // get IATA code display value
                        destinationUnlocoOrIataLocation.SelectValue = lane.DestinationIataCode;
                        destinationUnlocoOrIataLocation.SelectText = new Deals().GetGlobalLocationDisplayValue(lane.DestinationIataCode, lane.DestinationCountryName, lane.Service);
                    }
                }
                else if (!string.IsNullOrEmpty(lane.DestinationUnlocoCode))
                {
                    // get UNLOCO code display value
                    destinationUnlocoOrIataLocation.SelectValue = lane.DestinationUnlocoCode;
                    destinationUnlocoOrIataLocation.SelectText = new Deals().GetGlobalLocationDisplayValue(lane.DestinationUnlocoCode, lane.DestinationCountryName, lane.Service);
                }
                laneModel.DestinationLocation = destinationUnlocoOrIataLocation;

                return laneModel;
            }
            return null;
        }


        public LaneListResponse GetLanes(LaneFilter filters)
        {
            var response = new LaneListResponse
            {
                Lanes = new List<Lane>(),
                UserCurrencyCode = new Users().GetUserCurrencyCode(filters.UserId, filters.SubscriberId)
            };
            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());
            var userShippingFrequency = context.Subscribers.Where(t => t.SubscriberId == filters.SubscriberId)
                                               .Select(t => t.DefaultShippingFrequency).FirstOrDefault() ?? "Per Year";

            var lanes = context.Lanes.Where(t => !t.Deleted).ToList();
            if (filters.SubscriberId > 0)
                lanes = lanes.Where(t => t.SubscriberId == filters.SubscriberId).ToList();
            if (filters.DealId > 0)
                lanes = lanes.Where(t => t.DealId == filters.DealId).ToList();

            // convert revenue to the user currency
            lanes.ForEach(t =>
            {
                var sourceCurrencyCode = t.CurrencyCode;
                var revenue = t.Revenue;
                var profit = t.TotalLaneProfit;
                if (sourceCurrencyCode != response.UserCurrencyCode)
                {
                    revenue = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode,
                        response.UserCurrencyCode, revenue);
                    profit = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode,
                        response.UserCurrencyCode, profit);
                }

                t.Revenue = revenue;
                t.TotalLaneProfit = profit;
                t.CurrencyCode = response.UserCurrencyCode;
            });

            response.UserCurrencySymbol = new Currencies().GetCurrencySymbolFromCode(response.UserCurrencyCode);
            if (string.IsNullOrEmpty(response.UserCurrencySymbol))
            {
                response.UserCurrencySymbol = response.UserCurrencyCode;
            }
            response.Lanes = lanes;
            return response;
        }


        public int SaveLane(Lane lane)
        {
            try
            {
                var connection = LoginUser.GetConnection();
                var context = new DbFirstFreightDataContext(connection);
                // check for lane
                var objLane = context.Lanes.FirstOrDefault(d => d.LaneId == lane.LaneId) ?? new Lane();

                // populate basic lane fields
                objLane.DealId = lane.DealId;
                objLane.Service = lane.Service;
                objLane.Revenue = lane.Revenue;

                // convert revenue to USD
                objLane.RevenueUSD = new Currencies().GetCalculatedCurrencyExchangeValue(lane.CurrencyCode, "USD", lane.Revenue);

                objLane.CurrencyCode = lane.CurrencyCode;
                objLane.VolumeAmount = lane.VolumeAmount;
                objLane.VolumeUnit = lane.VolumeUnit;
                objLane.ShippingFrequency = lane.ShippingFrequency;
                objLane.ProfitUnitOfMeasure = lane.ProfitUnitOfMeasure;
                objLane.ProfitPercent = lane.ProfitPercent;

                // calculate profit amount
                if (!string.IsNullOrEmpty(objLane.ProfitUnitOfMeasure))
                {
                    if (objLane.ProfitUnitOfMeasure == "Percentage")
                    {
                        objLane.ProfitPercent = lane.ProfitPercent;
                    }
                    else
                    {
                        objLane.ProfitAmount = lane.ProfitAmount;
                    }
                }

                // profit
                var profit = 0.0;
                var volumeAmount = objLane.VolumeAmount;
                var revenue = objLane.Revenue;
                var profitPercent = objLane.ProfitPercent;
                var profitAmount = objLane.ProfitAmount ?? 0.0;

                switch (objLane.ProfitUnitOfMeasure)
                {
                    case "Percentage":
                        profit = revenue * profitPercent / 100;
                        objLane.ProfitAmount = profit;
                        break;
                    case "Per KG":
                        if (objLane.VolumeUnit == "KGs")
                        {
                            profit = volumeAmount * profitAmount;
                            objLane.ProfitAmount = profit;
                        }
                        else if (objLane.VolumeUnit == "LBs")
                        {
                            profit = volumeAmount * profitAmount * 0.453592;
                            objLane.ProfitAmount = profit;
                        }
                        else if (objLane.VolumeUnit == "Tonnes")
                        {
                            profit = volumeAmount * 1000 * profitAmount;
                            objLane.ProfitAmount = profit;
                        }
                        else
                            profit = 0;
                        break;
                    case "Per LB":
                        if (objLane.VolumeUnit == "KGs")
                        {
                            profit = volumeAmount * profitAmount * 2.20462;
                            objLane.ProfitAmount = profit;
                        }
                        else if (objLane.VolumeUnit == "LBs")
                        {
                            profit = volumeAmount * profitAmount;
                            objLane.ProfitAmount = profit;
                        }
                        else
                            profit = 0;
                        break;
                    case "Per Container":
                        if (objLane.VolumeUnit == "TEUs")
                        {
                            profit = volumeAmount * profitAmount;
                            objLane.ProfitAmount = profit;
                        }
                        else
                            profit = 0.0;
                        break;
                    case "Per Truck":
                        if (objLane.VolumeUnit == "Trucks")
                        {
                            profit = volumeAmount * profitAmount;
                            objLane.ProfitAmount = profit;
                        }
                        else
                            profit = 0.0;
                        break;
                    case "Per CBMs":
                        if (objLane.VolumeUnit == "CBMs")
                        {
                            profit = volumeAmount * profitAmount;
                            objLane.ProfitAmount = profit;
                        }
                        else
                            profit = 0.0;
                        break;
                    case "Flat Rate":
                        // KGs / Tonnes / TEUs / CBMs
                        profit = profitAmount;
                        break;
                }

                // profit in data entered currency
                objLane.TotalLaneProfit = profit;

                // convert profit to USD
                objLane.TotalLaneProfitUSD = new Currencies().GetCalculatedCurrencyExchangeValue(lane.CurrencyCode, "USD", profit);

                // origin
                objLane.OriginCountryCode = lane.OriginCountryCode;
                if (lane.OriginCountryCode == "Various" || string.IsNullOrEmpty(lane.OriginCountryCode))
                {
                    objLane.OriginCountryName = "Various";
                    objLane.OriginCountryCode = "Various";
                }
                else
                    objLane.OriginCountryName = new Countries().GetCountryNameFromCountryCode(objLane.OriginCountryCode);
 
                objLane.OriginRegionName = lane.OriginRegionName; 

                objLane.OriginIataCode = "";
                if (!string.IsNullOrEmpty(lane.OriginIataCode))
                {
                    var iataCode = lane.OriginIataCode.Split(new[] { "|" }, StringSplitOptions.None)[0];
                    objLane.OriginIataCode = iataCode;
                    if (iataCode == "Various")
                    {
                        objLane.OriginName = "Various";
                    }
                    else
                        objLane.OriginName = GetIataName(iataCode, objLane.OriginCountryName);
                }

                objLane.OriginUnlocoCode = "";
                if (!string.IsNullOrEmpty(lane.OriginUnlocoCode))
                {
                    var unclocoCode = lane.OriginUnlocoCode.Split(new[] { "|" }, StringSplitOptions.None)[0];
                    objLane.OriginUnlocoCode = unclocoCode;
                    objLane.OriginName = GetUnlocoName(unclocoCode, objLane.OriginCountryName);
                }

                objLane.PartnerAtOrigin = lane.PartnerAtOrigin;

                // destination
                objLane.DestinationCountryCode = lane.DestinationCountryCode;
                if (lane.DestinationCountryCode == "Various" || string.IsNullOrEmpty(lane.DestinationCountryCode))
                {
                    objLane.DestinationCountryCode = "Various";
                    objLane.DestinationCountryName = "Various";
                }
                else
                    objLane.DestinationCountryName = new Countries().GetCountryNameFromCountryCode(objLane.DestinationCountryCode);
                 
                objLane.DestinationRegionName = lane.DestinationRegionName; 

                objLane.DestinationIataCode = "";
                if (!string.IsNullOrEmpty(lane.DestinationIataCode))
                {
                    var iataCode = lane.DestinationIataCode.Split(new[] { "|" }, StringSplitOptions.None)[0];
                    objLane.DestinationIataCode = iataCode;
                    if (iataCode == "Various")
                    {
                        objLane.DestinationName = "Various";
                    }
                    else
                        objLane.DestinationName = GetIataName(iataCode, objLane.DestinationCountryName);
                }

                objLane.DestinationUnlocoCode = "";
                if (!string.IsNullOrEmpty(lane.DestinationUnlocoCode))
                {
                    var unclocoCode = lane.DestinationUnlocoCode.Split(new[] { "|" }, StringSplitOptions.None)[0];
                    objLane.DestinationUnlocoCode = unclocoCode;
                    objLane.DestinationName = GetUnlocoName(unclocoCode, objLane.DestinationCountryName);
                }

                objLane.PartnerAtDestination = lane.PartnerAtDestination;
                // other fields
                objLane.ShipperCompany = lane.ShipperCompany;
                objLane.ConsigneeCompany = lane.ConsigneeCompany;
                objLane.Comments = lane.Comments;

                objLane.ReceiveFrom3PL = lane.ReceiveFrom3PL;
                objLane.SpecialRequirements = lane.SpecialRequirements;
                objLane.ServiceLocation = lane.ServiceLocation;
                objLane.RequiresBarcode = lane.RequiresBarcode;
                objLane.TrackAndTrace = lane.TrackAndTrace;
                objLane.CustomerPickUpAtWarehouse = lane.CustomerPickUpAtWarehouse;

                objLane.LastUpdate = DateTime.UtcNow;
                objLane.UpdateUserId = lane.UpdateUserId;
                objLane.UpdateUserName = new Users().GetUserFullNameById(lane.UpdateUserId, lane.SubscriberId);


                // insert new lane
                if (objLane.LaneId < 1)
                {
                    objLane.SubscriberId = lane.SubscriberId;
                    objLane.CreatedUserId = lane.UpdateUserId;
                    objLane.CreatedDate = DateTime.UtcNow;
                    objLane.CreatedUserName = objLane.UpdateUserName;
                    context.Lanes.InsertOnSubmit(objLane);
                }
                context.SubmitChanges();

                // TODO: should be async

                // log event to intercom
                var eventName = "Saved lane";
                var intercomHelper = new IntercomHelper();
                intercomHelper.IntercomTrackEvent(lane.UpdateUserId, lane.SubscriberId, eventName);

                // log user action
                new Logging().LogUserAction(new UserActivity
                {
                    UserId = lane.CreatedUserId,
                    DealId = lane.DealId,
                    UserActivityMessage = "Saved Lane: " + lane.OriginName + " to " + lane.DestinationUnlocoCode
                });

                // update company last activity date
                var dealCompanyId = context.Deals.Where(t => t.DealId == lane.DealId).Select(t => t.CompanyId).FirstOrDefault();
                if (dealCompanyId > 0)
                {
                    new Companies().UpdateCompanyLastActivityDate(dealCompanyId, lane.SubscriberId);
                }

                // update deal totals for lanes
                UpdateDealForLanes(lane.DealId, lane.SubscriberId);

                // return lane Id
                return objLane.LaneId;
            }
            catch (Exception ex)
            {
                var error = new Crm6.App_Code.Shared.WebAppError
                {
                    ErrorCallStack = ex.StackTrace,
                    ErrorDateTime = DateTime.UtcNow,
                    RoutineName = "SaveLane",
                    PageCalledFrom = "Helper/Lanes",
                    SubscriberName = "",
                    ErrorMessage = ex.ToString(),
                    UserId = lane.UpdateUserId
                };
                new Logging().LogWebAppError(error);
            }

            return 0;
        }


        public string GetIataName(string iataCode, string countryName)
        {
            var countryCode = new Countries().GetCountryCodeFromCountryName(countryName);
            var sharedConnection = LoginUser.GetSharedConnection();
            var context = new Crm6.App_Code.Shared.DbSharedDataContext(sharedConnection);

            var location = context.GlobalLocations.FirstOrDefault(t => t.Airport && t.LocationCode.Equals(iataCode) && t.CountryCode.ToLower().Equals(countryCode.ToLower())); 
            if (location != null)
                return location.LocationName;
            return "";
        }


        public string GetUnlocoName(string unlocoCode, string countryName)
        {
            var countryCode = new Countries().GetCountryCodeFromCountryName(countryName);
            var sharedConnection = LoginUser.GetSharedConnection();
            var context = new Crm6.App_Code.Shared.DbSharedDataContext(sharedConnection);
            var location = context.GlobalLocations.FirstOrDefault(t => !t.Airport && t.LocationCode.Equals(unlocoCode) && t.CountryCode.ToLower().Equals(countryCode.ToLower())); 
            if (location != null)
                return location.LocationName;
            return "";
        }


        public bool DeleteLane(int laneId, int userId, int laneSubscriberId)
        {
            //var connection = LoginUser.GetConnection();
            // get deal subscriber connection
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                   .GlobalSubscribers.Where(t => t.SubscriberId == laneSubscriberId)
                                                   .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);

            var context = new DbFirstFreightDataContext(connection);
            var lane = context.Lanes.FirstOrDefault(t => t.LaneId == laneId);
            if (lane != null)
            {
                lane.Deleted = true;
                lane.DeletedById = userId;
                lane.DateDeleted = DateTime.UtcNow;
                lane.DeletedUserName = new Users().GetUserFullNameById(userId, laneSubscriberId);
                context.SubmitChanges();

                // update deal totals for lanes
                UpdateDealForLanes(lane.DealId, lane.SubscriberId);

                return true;
            }
            return false;
        }


        public bool UpdateDealForLanes(int dealId, int laneSubscriberId)
        {
            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());

            var currencyCode = "";
            var cbms = 0.00;
            var kgs = 0.00;
            var lbs = 0.00;
            var teus = 0.00;
            var tonnes = 0.00;
            var cbmsSpot = 0.00;
            var kgsSpot = 0.00;
            var lbsSpot = 0.00;
            var teusSpot = 0.00;
            var tonnesSpot = 0.00;
            var profit = 0.00;
            var spotProfit = 0.0;
            var profitUsd = 0.00;
            var spotProfitUsd = 0.00;
            var revenue = 0.00;
            var revenueUsd = 0.00;
            var spotRevenue = 0.00;
            var spotRevenueUsd = 0.00;
            var services = new List<string>();
            var volumeUnit = "";

            var deal = context.Deals.FirstOrDefault(d => d.DealId == dealId);
            if (deal != null)
            {
                // get list of lanes for deal
                var lanes = GetLanes(dealId, laneSubscriberId);


                // Consignee Names
                var consigneeList = lanes.Where(t => !string.IsNullOrEmpty(t.ConsigneeCompany))
                                         .Select(t => t.ConsigneeCompany).Distinct();
                deal.ConsigneeNames = string.Join(", ", consigneeList);
                // Shipper Names
                var shipperNamesList = lanes.Where(t => !string.IsNullOrEmpty(t.ShipperCompany))
                                            .Select(t => t.ShipperCompany).Distinct();
                deal.ShipperNames = string.Join(", ", shipperNamesList);


                // set the origins/destination locations
                var originLocationList = new List<string>();
                var destLocationList = new List<string>();
                var originCountryList = new List<string>();
                var destCountryList = new List<string>();


                // loop thru lanes to sum services, cbms, kgs, lbs, profit, profitUsd, revenue, revenueUsd, teus, tonnes
                var isSpotDeal = false;
                foreach (var lane in lanes)
                {
                    var isSpotLane = !lane.ShippingFrequency.Equals("Per Month") && !lane.ShippingFrequency.Equals("Per Year") && !lane.ShippingFrequency.Equals("Per Week");

                    currencyCode = lane.CurrencyCode;

                    // service
                    if (!services.Contains(lane.Service + ""))
                    {
                        services.Add(lane.Service + "");
                    }

                    volumeUnit = (lane.VolumeUnit + "").ToLower();

                    // determine volumeAmount from volumeUnit - store in monthly frequency
                    switch (volumeUnit)
                    {
                        case "cbms":
                            switch (lane.ShippingFrequency)
                            {
                                case "Per Month":
                                    cbms += lane.VolumeAmount;
                                    break;
                                case "Per Week":
                                    cbms += lane.VolumeAmount * 52 / 12;
                                    break;
                                case "Per Year":
                                    cbms += lane.VolumeAmount / 12;
                                    break;
                                case "Spot":
                                    cbmsSpot += lane.VolumeAmount;
                                    break;
                            }
                            break;

                        case "kgs":
                            switch (lane.ShippingFrequency)
                            {
                                case "Per Month":
                                    kgs += lane.VolumeAmount;
                                    break;
                                case "Per Week":
                                    kgs += lane.VolumeAmount * 52 / 12;
                                    break;
                                case "Per Year":
                                    kgs += lane.VolumeAmount / 12;
                                    break;
                                case "Spot":
                                    kgsSpot += lane.VolumeAmount;
                                    break;
                            }
                            break;

                        case "lbs":
                            switch (lane.ShippingFrequency)
                            {
                                case "Per Month":
                                    lbs += lane.VolumeAmount;
                                    break;
                                case "Per Week":
                                    lbs += lane.VolumeAmount * 52 / 12;
                                    break;
                                case "Per Year":
                                    lbs += lane.VolumeAmount / 12;
                                    break;
                                case "Spot":
                                    lbsSpot += lane.VolumeAmount;
                                    break;
                            }
                            break;

                        case "teus":
                            switch (lane.ShippingFrequency)
                            {
                                case "Per Month":
                                    teus += lane.VolumeAmount;
                                    break;
                                case "Per Week":
                                    teus += lane.VolumeAmount * 52 / 12;
                                    break;
                                case "Per Year":
                                    teus += lane.VolumeAmount / 12;
                                    break;
                                case "Spot":
                                    teusSpot += lane.VolumeAmount;
                                    break;
                            }
                            break;

                        case "tonnes":
                            switch (lane.ShippingFrequency)
                            {
                                case "Per Month":
                                    tonnes += lane.VolumeAmount;
                                    break;
                                case "Per Week":
                                    tonnes += lane.VolumeAmount * 52 / 12;
                                    break;
                                case "Per Year":
                                    tonnes += lane.VolumeAmount / 12;
                                    break;
                                case "Spot":
                                    tonnesSpot = lane.VolumeAmount;
                                    break;
                            }
                            break;
                    }

                    // Revenue - store in monthly frequency
                    switch (lane.ShippingFrequency)
                    {
                        case "Per Month":
                            revenue = lane.Revenue;
                            break;
                        case "Per Week":
                            revenue = lane.Revenue * 52 / 12;
                            break;
                        case "Per Year":
                            revenue = lane.Revenue / 12;
                            break;
                        case "Spot":
                            spotRevenue = lane.Revenue;
                            break;
                    }

                    // RevenueUSD - convert from CurrencyCode - store in monthly frequency
                    revenueUsd += new Currencies().GetCalculatedCurrencyExchangeValue(currencyCode, "USD", revenue);
                    spotRevenueUsd += new Currencies().GetCalculatedCurrencyExchangeValue(currencyCode, "USD", spotRevenue);

                    // Profit - store in monthly frequency
                    switch (lane.ShippingFrequency)
                    {
                        case "Per Month":
                            profit = lane.TotalLaneProfit;
                            break;
                        case "Per Week":
                            profit = lane.TotalLaneProfit * 52 / 12;
                            break;
                        case "Per Year":
                            profit = lane.TotalLaneProfit / 12;
                            break;
                        case "Spot":
                            spotProfit = lane.TotalLaneProfit;
                            break;
                    }

                    // ProfitUSD - convert from CurrencyCode - store in monthly frequency
                    profitUsd += new Currencies().GetCalculatedCurrencyExchangeValue(currencyCode, "USD", profit);
                    spotProfitUsd += new Currencies().GetCalculatedCurrencyExchangeValue(currencyCode, "USD", spotProfit);


                    // origins
                    var strLocation = !string.IsNullOrEmpty(lane.OriginUnlocoCode) ?
                        (lane.OriginUnlocoCode) : (!string.IsNullOrEmpty(lane.OriginIataCode) ? lane.OriginIataCode : "");
                    if (!string.IsNullOrEmpty(strLocation))
                        strLocation = strLocation + " | " + lane.OriginName;
                    if (!originLocationList.Contains(strLocation) && !string.IsNullOrEmpty(strLocation))
                        originLocationList.Add(strLocation);

                    // destinations 
                    strLocation = "";
                    strLocation = !string.IsNullOrEmpty(lane.DestinationIataCode) ? lane.DestinationIataCode :
                                     (!string.IsNullOrEmpty(lane.DestinationUnlocoCode) ? lane.DestinationUnlocoCode : "");
                    if (!string.IsNullOrEmpty(strLocation))
                        strLocation = strLocation + " | " + lane.DestinationName;
                    if (!destLocationList.Contains(strLocation) && !string.IsNullOrEmpty(strLocation))
                        destLocationList.Add(strLocation);

                    // origin countries
                    if (!string.IsNullOrEmpty(lane.OriginCountryName) && !originCountryList.Contains(lane.OriginCountryName))
                        originCountryList.Add(lane.OriginCountryName);

                    // destination countries  
                    if (!string.IsNullOrEmpty(lane.DestinationCountryName) && !destCountryList.Contains(lane.DestinationCountryName))
                        destCountryList.Add(lane.DestinationCountryName);



                }

                // update Deal lane
                deal.CurrencyCode = currencyCode;
                deal.CBMs = cbms;
                deal.Kgs = kgs;
                deal.Lbs = lbs;
                deal.TEUs = teus;
                deal.Tonnes = tonnes;

                deal.CBMsSpot = cbmsSpot;
                deal.KgsSpot = kgsSpot;
                deal.LbsSpot = lbsSpot;
                deal.TEUsSpot = teusSpot;
                deal.TonnesSpot = tonnesSpot;


                deal.ProfitUSD = profitUsd;
                deal.RevenueUSD = revenueUsd;

                deal.ProfitUSDSpot = spotProfitUsd;
                deal.RevenueUSDSpot = spotRevenueUsd;

                deal.Services = string.Join(", ", services);
                deal.SpotDeal = isSpotDeal;

                // set origin/destination location and countries
                var originCountryStr = string.Join(", ", originCountryList);
                var destCountryStr = string.Join(", ", destCountryList);
                var originLocationStr = string.Join(", ", originLocationList);
                var destLocationStr = string.Join(", ", destLocationList);

                deal.OrignLocations = originLocationStr;
                deal.DestinationLocations = destLocationStr;

                deal.OrignCountries = originCountryStr;
                deal.DestinationCountries = destCountryStr;


                context.SubmitChanges();
                return true;
            }
            return false;

        }


        public List<Lane> GetLanes(int dealId, int subscriberId)
        {
            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());

            return context.Lanes.Where(t => !t.Deleted
                                            && t.SubscriberId == subscriberId
                                            && t.DealId == dealId).Select(t => t).ToList();
        }

    }
}