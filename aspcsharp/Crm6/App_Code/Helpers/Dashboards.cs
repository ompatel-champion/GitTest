using System.Collections.Generic;
using Models;
using System.Linq;
using Crm6.App_Code;
using System.Data.SqlClient;
using Crm6.App_Code.Models;
using System;

namespace Helpers
{
    public class Dashboards
    {

        public List<SalesForecastByStage> GetSalesForecastByStage(DashboardDataRequest request)
        {
            var salesForcastByStages = new List<SalesForecastByStage>();

            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            var user = context.Users.FirstOrDefault(t => t.UserId == request.UserId);
            if (user != null)
            {
                var targetShippingFrequency = new Subscribers().GetShippingFrequency(user.SubscriberId) ?? "Per Month";
                var userCurrencySymbol = new Currencies().GetCurrencySymbolFromCode(user.CurrencyCode);
                //  var countryCode = user.CountryCode;

                var sql = "SELECT d.SalesStageName,";
                if (request.Status == "ACTIVE")
                    sql += "s.SortOrder,";
                sql += " Sum(d.RevenueUSD) AS RevenueUSD, Sum(d.ProfitUSD) AS ProfitUSD, Count(*)  AS DealCount ";
                sql += "FROM Deals AS d ";
                sql += "INNER JOIN Users As u ON (d.DealOwnerId = u.UserId) AND (d.SubscriberId = u.SubscriberId) ";
                if (request.Status == "ACTIVE")
                    sql += "INNER JOIN SalesStages As s ON (d.SalesStageName = s.SalesStageName) AND (d.SubscriberId = s.SubscriberId) ";
                sql += "WHERE (d.SubscriberId = " + request.SubscriberId + ") ";
                sql += "AND (d.Deleted = 0) ";
                sql += "AND (u.Deleted = 0) ";
                if (request.CountryCodes != null && request.CountryCodes.Count > 0)
                {
                    var strCountryCodes = "";
                    foreach (var countryCode in request.CountryCodes)
                    {
                        if (!string.IsNullOrEmpty(strCountryCodes))
                            strCountryCodes += ",";
                        strCountryCodes += "'" + countryCode + "'";
                    }
                    if (!string.IsNullOrEmpty(strCountryCodes))
                    {
                        sql += "AND (u.CountryCode IN (" + strCountryCodes + ")) ";
                    }
                }
                if (request.LocationCodes != null && request.LocationCodes.Count > 0)
                {
                    var strLocationCodes = "";
                    foreach (var locationCode in request.LocationCodes)
                    {
                        if (!string.IsNullOrEmpty(strLocationCodes))
                            strLocationCodes += ",";
                        strLocationCodes += "'" + locationCode + "'";
                    }
                    if (!string.IsNullOrEmpty(strLocationCodes))
                    {
                        sql += "AND (u.LocationCode IN (" + strLocationCodes + ")) ";
                    }
                }

                if (request.SalesRepIds != null && request.SalesRepIds.Count > 0)
                {
                    var strSalesRepIds = "";
                    foreach (var salesRepId in request.SalesRepIds)
                    {
                        if (!string.IsNullOrEmpty(strSalesRepIds))
                            strSalesRepIds += ",";
                        strSalesRepIds += "" + salesRepId + "";
                    }
                    if (!string.IsNullOrEmpty(strSalesRepIds))
                    {
                        sql += "AND (d.DealOwnerId IN (" + strSalesRepIds + ")) ";
                    }
                }

                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    sql += "AND (d.DealName = '" + request.Keyword + "') ";
                }
                if (request.Status == "ACTIVE")
                {
                    sql += "AND (d.SalesStageName) <> 'won' ";
                    sql += "And(d.SalesStageName) <> 'lost' ";
                    sql += "And(d.SalesStageName) <> 'stalled' ";
                }
                else if (request.Status == "INACTIVE")
                {
                    sql += "AND ((d.SalesStageName) = 'won' ";
                    sql += "OR (d.SalesStageName) = 'lost' ";
                    sql += "OR (d.SalesStageName) = 'stalled' )";
                }
                if (!string.IsNullOrEmpty(request.DateType))
                {
                    if (!string.IsNullOrEmpty(request.DateFrom))
                    {
                        var dateFrom = Convert.ToDateTime(request.DateFrom);

                        dateFrom = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, 0, 0, 0);
                        switch (request.DateType)
                        {
                            case "Proposal":
                                sql += "AND (d.EstimatedStartDate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "ContractEnd":
                                sql += "AND (d.ContractEndDate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "CreatedDate":
                                sql += "AND (d.CreatedDate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "UpdatedDate":
                                sql += "AND (d.LastUpdate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                        }
                    }
                    if (!string.IsNullOrEmpty(request.DateTo))
                    {
                        var dateTo = Convert.ToDateTime(request.DateTo);
                        dateTo = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day, 23, 59, 0);
                        switch (request.DateType)
                        {
                            case "Proposal":
                                sql += "AND (d.EstimatedStartDate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "ContractEnd":
                                sql += "AND (d.ContractEndDate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "CreatedDate":
                                sql += "AND (d.CreatedDate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "UpdatedDate":
                                sql += "AND (d.LastUpdate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                        }
                    }
                }

                //  sql += "AND d.SpotDeal = 0 ";
                sql += "GROUP BY d.SalesStageName";
                if (request.Status == "ACTIVE")
                    sql += ",s.SortOrder ORDER BY s.SortOrder";
                else
                    sql += " ORDER BY  d.SalesStageName";

                var dataCenter = user.DataCenter;
                var connectionString = LoginUser.GetConnection();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // populate dataReader
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader dataReader = cmd.ExecuteReader();

                    // check if no records
                    if (!dataReader.HasRows) { return salesForcastByStages; }

                    // iterate though salesStage totals records
                    while (dataReader.Read())
                    {
                        var salesStageName = dataReader.GetValue(0).ToString();
                        var revenue = 0.0;
                        var profit = 0.0;
                        var dealCount = 0;
                        if (request.Status == "ACTIVE")
                        {
                            revenue = (double)dataReader.GetValue(2);
                            profit = (double)dataReader.GetValue(3);
                            dealCount = (int)dataReader.GetValue(4);
                        }
                        else
                        {
                            revenue = (double)dataReader.GetValue(1);
                            profit = (double)dataReader.GetValue(2);
                            dealCount = (int)dataReader.GetValue(3);
                        }

                        // convert profit and revenue to user's currency
                        var userCurrencyProfit = profit;
                        var userCurrencyRevenue = revenue;
                        if (user.CurrencyCode != "USD")
                        {
                            userCurrencyProfit = new Currencies().GetCalculatedCurrencyExchangeValue("USD", user.CurrencyCode, profit);
                            userCurrencyRevenue = new Currencies().GetCalculatedCurrencyExchangeValue("USD", user.CurrencyCode, revenue);
                        }

                        // Per Year reporting
                        if (targetShippingFrequency == "Per Year")
                        {
                            userCurrencyProfit = userCurrencyProfit * 12;
                            userCurrencyRevenue = userCurrencyRevenue * 12;
                        }

                        // populate chart table
                        if (userCurrencyProfit > 0 || userCurrencyRevenue > 0)
                        {
                            salesForcastByStages.Add(new SalesForecastByStage
                            {
                                CurrencyCode = user.CurrencyCode,
                                CurrencySymbol = userCurrencySymbol,
                                Profit = (int)userCurrencyProfit,
                                Revenue = (int)userCurrencyRevenue,
                                SalesStage = salesStageName,
                                DealCount = dealCount
                            });
                        }
                    }
                    conn.Close();
                }

                var salesStages = new List<SalesStage>();
                if (request.Status == "ACTIVE")
                {
                    salesStages = new SalesStages().GetSalesStages(request.SubscriberId)
                                                  .Where(t => !t.Won && !t.Lost && t.SalesStageName != "Stalled")
                                                  .OrderBy(t => t.SortOrder).ToList();
                }
                else
                {
                    salesStages = new List<SalesStage> {
                        new SalesStage { SalesStageName = "Won"},
                        new SalesStage { SalesStageName = "Lost"},
                        new SalesStage { SalesStageName = "Stalled"}
                    };
                }

                var finalSalesForcastByStages = new List<SalesForecastByStage>();

                foreach (var s in salesStages)
                {
                    var salesStageData = new SalesForecastByStage
                    {
                        SalesStage = s.SalesStageName
                    };

                    // check if the sales stage is in groupes sales stage list
                    var found = salesForcastByStages.FirstOrDefault(t => t.SalesStage.ToLower() == s.SalesStageName.ToLower());
                    if (found != null)
                    {
                        salesStageData.CurrencyCode = found.CurrencyCode;
                        salesStageData.CurrencySymbol = found.CurrencySymbol;
                        salesStageData.Profit = found.Profit;
                        salesStageData.Revenue = found.Revenue;
                        salesStageData.DealCount = found.DealCount;
                    }
                    else
                    {
                        salesStageData.CurrencyCode = user.CurrencyCode;
                        salesStageData.CurrencySymbol = userCurrencySymbol;
                        salesStageData.Profit = 0;
                        salesStageData.Revenue = 0;
                    }
                    finalSalesForcastByStages.Add(salesStageData);
                }
                return finalSalesForcastByStages;
            }

            return new List<SalesForecastByStage>();
        }




        public List<DealsByIndustry> GetDealsByIndustry(DashboardDataRequest request)
        {
            var dealsByIndustry = new List<DealsByIndustry>();
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var user = context.Users.FirstOrDefault(t => t.UserId == request.UserId);
            if (user != null)
            {
                var targetShippingFrequency = new Subscribers().GetShippingFrequency(user.SubscriberId) ?? "Per Month";
                var userCurrencySymbol = new Currencies().GetCurrencySymbolFromCode(user.CurrencyCode);


                // sum deal totals with join on users table
                var sql = "SELECT d.Industry, Sum(d.RevenueUSD) AS RevenueUSD, Sum(d.ProfitUSD) AS ProfitUSD ";
                sql += "FROM Deals AS d ";
                sql += "INNER JOIN Users As u ON (d.DealOwnerId = u.UserId) AND (d.SubscriberId = u.SubscriberId) ";
                sql += "WHERE (d.SubscriberId = " + request.SubscriberId + ") ";
                if (request.CountryCodes != null && request.CountryCodes.Count > 0)
                {
                    var strCountryCodes = "";
                    foreach (var countryCode in request.CountryCodes)
                    {
                        if (!string.IsNullOrEmpty(strCountryCodes))
                            strCountryCodes += ",";
                        strCountryCodes += "'" + countryCode + "'";
                    }
                    sql += "AND (u.CountryCode IN (" + strCountryCodes + ")) ";
                }
                if (request.LocationCodes != null && request.LocationCodes.Count > 0)
                {
                    var strLocationCodes = "";
                    foreach (var locationCode in request.LocationCodes)
                    {
                        if (!string.IsNullOrEmpty(strLocationCodes))
                            strLocationCodes += ",";
                        strLocationCodes += "'" + locationCode + "'";
                    }
                    sql += "AND (u.LocationCode IN (" + strLocationCodes + ")) ";
                }

                if (request.SalesRepIds != null && request.SalesRepIds.Count > 0)
                {
                    var strSalesRepIds = "";
                    foreach (var salesRepId in request.SalesRepIds)
                    {
                        if (!string.IsNullOrEmpty(strSalesRepIds))
                            strSalesRepIds += ",";
                        strSalesRepIds += "" + salesRepId + "";
                    }
                    sql += "AND (d.DealOwnerId IN (" + strSalesRepIds + ")) ";
                }

                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    sql += "AND (d.DealName = '" + request.Keyword + "') ";
                }
                if (request.Status == "ACTIVE")
                {
                    sql += "AND (d.SalesStageName) <> 'won' ";
                    sql += "And(d.SalesStageName) <> 'lost' ";
                    sql += "And(d.SalesStageName) <> 'stalled' ";
                }
                else if (request.Status == "INACTIVE")
                {
                    sql += "AND ((d.SalesStageName) = 'won' ";
                    sql += "OR (d.SalesStageName) = 'lost' ";
                    sql += "OR (d.SalesStageName) = 'stalled' )";
                }
                if (!string.IsNullOrEmpty(request.DateType))
                {
                    if (!string.IsNullOrEmpty(request.DateFrom))
                    {
                        var dateFrom = Convert.ToDateTime(request.DateFrom);

                        dateFrom = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, 0, 0, 0);
                        switch (request.DateType)
                        {
                            case "Proposal":
                                sql += "AND (d.EstimatedStartDate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "ContractEnd":
                                sql += "AND (d.ContractEndDate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "CreatedDate":
                                sql += "AND (d.CreatedDate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "UpdatedDate":
                                sql += "AND (d.LastUpdate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                        }
                    }
                    if (!string.IsNullOrEmpty(request.DateTo))
                    {
                        var dateTo = Convert.ToDateTime(request.DateTo);
                        dateTo = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day, 23, 59, 0);
                        switch (request.DateType)
                        {
                            case "Proposal":
                                sql += "AND (d.EstimatedStartDate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "ContractEnd":
                                sql += "AND (d.ContractEndDate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "CreatedDate":
                                sql += "AND (d.CreatedDate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "UpdatedDate":
                                sql += "AND (d.LastUpdate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                        }
                    }
                }
                sql += "AND (d.Deleted = 0) ";
                sql += "AND (u.Deleted = 0) ";
                //  sql += "AND d.SpotDeal = 0 ";
                sql += "GROUP BY d.Industry ";
                sql += "ORDER BY d.Industry";

                var dataCenter = user.DataCenter;
                var connectionString = LoginUser.GetConnection();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // populate dataReader
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader dataReader = cmd.ExecuteReader();

                    // check if no records
                    if (!dataReader.HasRows) { return dealsByIndustry; }

                    // iterate though industry totals records
                    while (dataReader.Read())
                    {
                        var industry = dataReader.GetValue(0).ToString();
                        var revenue = (double)dataReader.GetValue(1);
                        var profit = (double)dataReader.GetValue(2);

                        // convert profit and revenue to user's currency
                        var userCurrencyProfit = profit;
                        var userCurrencyRevenue = revenue;
                        if (user.CurrencyCode != "USD")
                        {
                            userCurrencyProfit = new Currencies().GetCalculatedCurrencyExchangeValue("USD", user.CurrencyCode, profit);
                            userCurrencyRevenue = new Currencies().GetCalculatedCurrencyExchangeValue("USD", user.CurrencyCode, revenue);
                        }

                        // Per Year reporting
                        if (targetShippingFrequency == "Per Year")
                        {
                            userCurrencyProfit = userCurrencyProfit * 12;
                            userCurrencyRevenue = userCurrencyRevenue * 12;
                        }

                        // populate chart table
                        if (userCurrencyProfit > 0 || userCurrencyRevenue > 0)
                        {
                            dealsByIndustry.Add(new DealsByIndustry
                            {
                                CurrencyCode = user.CurrencyCode,
                                CurrencySymbol = userCurrencySymbol,
                                Industry = industry,
                                Profit = userCurrencyProfit,
                                Revenue = userCurrencyRevenue
                            });
                        }
                    }
                    conn.Close();
                }
            }
            return dealsByIndustry;
        }


        public List<SalesForecastByLocation> GetSalesForecastByLocation(DashboardDataRequest request)
        {
            var salesForecastByLocation = new List<SalesForecastByLocation>();

            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var user = context.Users.FirstOrDefault(t => t.UserId == request.UserId);
            if (user != null)
            {
                var targetShippingFrequency = new Subscribers().GetShippingFrequency(user.SubscriberId) ?? "Per Month";
                var userCurrencySymbol = new Currencies().GetCurrencySymbolFromCode(user.CurrencyCode);

                var sql = "SELECT u.LocationName,u.LocationCode, Sum(d.RevenueUSD) AS RevenueUSD, Sum(d.ProfitUSD) AS ProfitUSD ";
                sql += "FROM Deals AS d ";
                sql += "INNER JOIN Users As u ON(d.DealOwnerId = u.UserId) AND(d.SubscriberId = u.SubscriberId) ";
                sql += "WHERE (d.SubscriberId = " + request.SubscriberId + ") ";
                if (request.CountryCodes != null && request.CountryCodes.Count > 0)
                {
                    var strCountryCodes = "";
                    foreach (var countryCode in request.CountryCodes)
                    {
                        if (!string.IsNullOrEmpty(strCountryCodes))
                            strCountryCodes += ",";
                        strCountryCodes += "'" + countryCode + "'";
                    }
                    sql += "AND (u.CountryCode IN (" + strCountryCodes + ")) ";
                }
                if (request.LocationCodes != null && request.LocationCodes.Count > 0)
                {
                    var strLocationCodes = "";
                    foreach (var locationCode in request.LocationCodes)
                    {
                        if (!string.IsNullOrEmpty(strLocationCodes))
                            strLocationCodes += ",";
                        strLocationCodes += "'" + locationCode + "'";
                    }
                    sql += "AND (u.LocationCode IN (" + strLocationCodes + ")) ";
                }

                if (request.SalesRepIds != null && request.SalesRepIds.Count > 0)
                {
                    var strSalesRepIds = "";
                    foreach (var salesRepId in request.SalesRepIds)
                    {
                        if (!string.IsNullOrEmpty(strSalesRepIds))
                            strSalesRepIds += ",";
                        strSalesRepIds += "" + salesRepId + "";
                    }
                    sql += "AND (d.DealOwnerId IN (" + strSalesRepIds + ")) ";
                }

                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    sql += "AND (d.DealName = '" + request.Keyword + "') ";
                }
                if (request.Status == "ACTIVE")
                {
                    sql += "AND (d.SalesStageName) <> 'won' ";
                    sql += "And(d.SalesStageName) <> 'lost' ";
                    sql += "And(d.SalesStageName) <> 'stalled' ";
                }
                else if (request.Status == "INACTIVE")
                {
                    sql += "AND ((d.SalesStageName) = 'won' ";
                    sql += "OR (d.SalesStageName) = 'lost' ";
                    sql += "OR (d.SalesStageName) = 'stalled' )";
                }
                if (!string.IsNullOrEmpty(request.DateType))
                {
                    if (!string.IsNullOrEmpty(request.DateFrom))
                    {
                        var dateFrom = Convert.ToDateTime(request.DateFrom);

                        dateFrom = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, 0, 0, 0);
                        switch (request.DateType)
                        {
                            case "Proposal":
                                sql += "AND (d.EstimatedStartDate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "ContractEnd":
                                sql += "AND (d.ContractEndDate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "CreatedDate":
                                sql += "AND (d.CreatedDate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "UpdatedDate":
                                sql += "AND (d.LastUpdate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                        }
                    }
                    if (!string.IsNullOrEmpty(request.DateTo))
                    {
                        var dateTo = Convert.ToDateTime(request.DateTo);
                        dateTo = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day, 23, 59, 0);
                        switch (request.DateType)
                        {
                            case "Proposal":
                                sql += "AND (d.EstimatedStartDate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "ContractEnd":
                                sql += "AND (d.ContractEndDate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "CreatedDate":
                                sql += "AND (d.CreatedDate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "UpdatedDate":
                                sql += "AND (d.LastUpdate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                        }
                    }
                }
                sql += "AND (d.Deleted = 0) ";
                sql += "AND (u.Deleted = 0) ";
                //  sql += "AND d.SpotDeal = 0 ";
                sql += "GROUP BY u.LocationName, u.LocationCode ";
                sql += "ORDER BY u.LocationName";


                var dataCenter = user.DataCenter;
                var connectionString = LoginUser.GetConnection();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // populate dataReader
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader dataReader = cmd.ExecuteReader();

                    // check if no records
                    if (!dataReader.HasRows) { return salesForecastByLocation; }

                    // iterate though industry totals records
                    while (dataReader.Read())
                    {
                        var locationName = dataReader.GetValue(0).ToString();
                        var locationCode = dataReader.GetValue(1).ToString();
                        var revenue = (double)dataReader.GetValue(2);
                        var profit = (double)dataReader.GetValue(3);

                        // convert profit and revenue to user's currency
                        var userCurrencyProfit = profit;
                        var userCurrencyRevenue = revenue;
                        if (user.CurrencyCode != "USD")
                        {
                            userCurrencyProfit = new Currencies().GetCalculatedCurrencyExchangeValue("USD", user.CurrencyCode, profit);
                            userCurrencyRevenue = new Currencies().GetCalculatedCurrencyExchangeValue("USD", user.CurrencyCode, revenue);
                        }

                        // Per Year reporting
                        if (targetShippingFrequency == "Per Year")
                        {
                            userCurrencyProfit = userCurrencyProfit * 12;
                            userCurrencyRevenue = userCurrencyRevenue * 12;
                        }

                        // populate chart table
                        if (userCurrencyProfit > 0 || userCurrencyRevenue > 0)
                        {
                            salesForecastByLocation.Add(new SalesForecastByLocation()
                            {
                                CurrencyCode = user.CurrencyCode,
                                CurrencySymbol = userCurrencySymbol,
                                LocationName = locationName,
                                LocationCode = locationCode,
                                Profit = userCurrencyProfit,
                                Revenue = userCurrencyRevenue
                            });
                        }
                    }
                    conn.Close();
                }

            }
            return salesForecastByLocation.OrderByDescending(t => t.Revenue).ToList();
        }


        public List<SalesForecastBySalesRep> GetSalesForecastBySalesReps(DashboardDataRequest request)
        {

            var salesForecastBySalesRep = new List<SalesForecastBySalesRep>();

            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var user = context.Users.FirstOrDefault(t => t.UserId == request.UserId);
            if (user != null)
            {
                var targetShippingFrequency = new Subscribers().GetShippingFrequency(user.SubscriberId) ?? "Per Month";
                var userCurrencySymbol = new Currencies().GetCurrencySymbolFromCode(user.CurrencyCode);

                var sql = "SELECT u.UserId, u.FullName, u.LocationName, Sum(d.RevenueUSD) AS RevenueUSD, Sum(d.ProfitUSD) AS ProfitUSD ";
                sql += "FROM Deals AS d ";
                sql += "INNER JOIN Users As u ON(d.DealOwnerId = u.UserId) AND(d.SubscriberId = u.SubscriberId) ";
                sql += "WHERE (d.SubscriberId = " + request.SubscriberId + ") ";
                if (request.CountryCodes != null && request.CountryCodes.Count > 0)
                {
                    var strCountryCodes = "";
                    foreach (var countryCode in request.CountryCodes)
                    {
                        if (!string.IsNullOrEmpty(strCountryCodes))
                            strCountryCodes += ",";
                        strCountryCodes += "'" + countryCode + "'";
                    }
                    sql += "AND (u.CountryCode IN (" + strCountryCodes + ")) ";
                }
                if (request.LocationCodes != null && request.LocationCodes.Count > 0)
                {
                    var strLocationCodes = "";
                    foreach (var locationCode in request.LocationCodes)
                    {
                        if (!string.IsNullOrEmpty(strLocationCodes))
                            strLocationCodes += ",";
                        strLocationCodes += "'" + locationCode + "'";
                    }
                    sql += "AND (u.LocationCode IN (" + strLocationCodes + ")) ";
                }

                if (request.SalesRepIds != null && request.SalesRepIds.Count > 0)
                {
                    var strSalesRepIds = "";
                    foreach (var salesRepId in request.SalesRepIds)
                    {
                        if (!string.IsNullOrEmpty(strSalesRepIds))
                            strSalesRepIds += ",";
                        strSalesRepIds += "" + salesRepId + "";
                    }
                    sql += "AND (d.DealOwnerId IN (" + strSalesRepIds + ")) ";
                }

                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    sql += "AND (d.DealName = '" + request.Keyword + "') ";
                }
                if (request.Status == "ACTIVE")
                {
                    sql += "AND (d.SalesStageName) <> 'won' ";
                    sql += "And(d.SalesStageName) <> 'lost' ";
                    sql += "And(d.SalesStageName) <> 'stalled' ";
                }
                else if (request.Status == "INACTIVE")
                {
                    sql += "AND ((d.SalesStageName) = 'won' ";
                    sql += "OR (d.SalesStageName) = 'lost' ";
                    sql += "OR (d.SalesStageName) = 'stalled' )";
                }
                if (!string.IsNullOrEmpty(request.DateType))
                {
                    if (!string.IsNullOrEmpty(request.DateFrom))
                    {
                        var dateFrom = Convert.ToDateTime(request.DateFrom);

                        dateFrom = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, 0, 0, 0);
                        switch (request.DateType)
                        {
                            case "Proposal":
                                sql += "AND (d.EstimatedStartDate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "ContractEnd":
                                sql += "AND (d.ContractEndDate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "CreatedDate":
                                sql += "AND (d.CreatedDate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "UpdatedDate":
                                sql += "AND (d.LastUpdate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                        }
                    }
                    if (!string.IsNullOrEmpty(request.DateTo))
                    {
                        var dateTo = Convert.ToDateTime(request.DateTo);
                        dateTo = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day, 23, 59, 0);
                        switch (request.DateType)
                        {
                            case "Proposal":
                                sql += "AND (d.EstimatedStartDate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "ContractEnd":
                                sql += "AND (d.ContractEndDate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "CreatedDate":
                                sql += "AND (d.CreatedDate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "UpdatedDate":
                                sql += "AND (d.LastUpdate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                        }
                    }
                }
                sql += "AND (d.Deleted = 0) ";
                sql += "AND (u.Deleted = 0) ";
                //sql += "AND d.SpotDeal = 0 ";
                sql += "GROUP BY u.UserId, u.FullName, u.LocationName ";
                sql += "ORDER BY u.FullName";

                var dataCenter = user.DataCenter;
                var connectionString = LoginUser.GetConnection();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // populate dataReader
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader dataReader = cmd.ExecuteReader();

                    // check if no records
                    if (!dataReader.HasRows) { return salesForecastBySalesRep; }

                    // iterate though industry totals records
                    while (dataReader.Read())
                    {
                        var salesRepId = (int)dataReader.GetValue(0);
                        var salesRep = dataReader.GetValue(1).ToString();
                        var locationName = dataReader.GetValue(2).ToString();
                        var revenue = (double)dataReader.GetValue(3);
                        revenue = (int)revenue; 
                        var profit = (double)dataReader.GetValue(4);

                        // convert profit and revenue to user's currency
                        var userCurrencyProfit = profit;
                        var userCurrencyRevenue = revenue;
                        if (user.CurrencyCode != "USD")
                        {
                            userCurrencyProfit = new Currencies().GetCalculatedCurrencyExchangeValue("USD", user.CurrencyCode, profit);
                            userCurrencyRevenue = new Currencies().GetCalculatedCurrencyExchangeValue("USD", user.CurrencyCode, revenue);
                        }

                        // Per Year reporting
                        if (targetShippingFrequency == "Per Year")
                        {
                            userCurrencyProfit = userCurrencyProfit * 12;
                            userCurrencyRevenue = userCurrencyRevenue * 12;
                        }

                        // populate chart table
                        if (userCurrencyProfit > 0 || userCurrencyRevenue > 0)
                        {
                            salesForecastBySalesRep.Add(new SalesForecastBySalesRep
                            {
                                CurrencyCode = user.CurrencyCode,
                                CurrencySymbol = userCurrencySymbol,
                                Profit = userCurrencyProfit,
                                Revenue = (int)userCurrencyRevenue,
                                SalesRep = salesRep,
                                UserId = salesRepId
                            });
                        }
                    }
                    conn.Close();
                }
            }
            return salesForecastBySalesRep.OrderByDescending(t => t.Revenue).ToList();
        }

        //sales forcast by sales rep stage
        public List<SalesForecastBySalesRepStage> GetSalesForecastBySalesRepStage(DashboardDataRequest request)
        {
            var salesForecastBySalesRepStage = new List<SalesForecastBySalesRepStage>();
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var user = context.Users.FirstOrDefault(t => t.UserId == request.UserId);
            if (user != null)
            {
                var targetShippingFrequency = new Subscribers().GetShippingFrequency(user.SubscriberId) ?? "Per Month";
                var userCurrencySymbol = new Currencies().GetCurrencySymbolFromCode(user.CurrencyCode);

                var sql = "SELECT u.UserId, u.FullName, s.SalesStageName, u.LocationName, Sum(d.RevenueUSD) AS RevenueUSD, Sum(d.ProfitUSD) AS ProfitUSD ";
                sql += "FROM Deals AS d ";
                sql += "INNER JOIN Users As u ON(d.DealOwnerId = u.UserId) AND(d.SubscriberId = u.SubscriberId) ";
                sql += "INNER JOIN SalesStages As s ON (d.SalesStageName = s.SalesStageName) AND (d.SubscriberId = s.SubscriberId) ";
                sql += "WHERE (d.SubscriberId = " + request.SubscriberId + ") ";
                if (request.CountryCodes != null && request.CountryCodes.Count > 0)
                {
                    var strCountryCodes = "";
                    foreach (var countryCode in request.CountryCodes)
                    {
                        if (!string.IsNullOrEmpty(strCountryCodes))
                            strCountryCodes += ",";
                        strCountryCodes += "'" + countryCode + "'";
                    }
                    sql += "AND (u.CountryCode IN (" + strCountryCodes + ")) ";
                }
                if (request.LocationCodes != null && request.LocationCodes.Count > 0)
                {
                    var strLocationCodes = "";
                    foreach (var locationCode in request.LocationCodes)
                    {
                        if (!string.IsNullOrEmpty(strLocationCodes))
                            strLocationCodes += ",";
                        strLocationCodes += "'" + locationCode + "'";
                    }
                    sql += "AND (u.LocationCode IN (" + strLocationCodes + ")) ";
                }

                if (request.SalesRepIds != null && request.SalesRepIds.Count > 0)
                {
                    var strSalesRepIds = "";
                    foreach (var salesRepId in request.SalesRepIds)
                    {
                        if (!string.IsNullOrEmpty(strSalesRepIds))
                            strSalesRepIds += ",";
                        strSalesRepIds += "" + salesRepId + "";
                    }
                    sql += "AND (d.DealOwnerId IN (" + strSalesRepIds + ")) ";
                }

                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    sql += "AND (d.DealName = '" + request.Keyword + "') ";
                }
                if (request.Status == "ACTIVE")
                {
                    sql += "AND (d.SalesStageName) <> 'won' ";
                    sql += "And(d.SalesStageName) <> 'lost' ";
                    sql += "And(d.SalesStageName) <> 'stalled' ";
                }
                else if (request.Status == "INACTIVE")
                {
                    sql += "AND ((d.SalesStageName) = 'won' ";
                    sql += "OR (d.SalesStageName) = 'lost' ";
                    sql += "OR (d.SalesStageName) = 'stalled' )";
                }
                if (!string.IsNullOrEmpty(request.DateType))
                {
                    if (!string.IsNullOrEmpty(request.DateFrom))
                    {
                        var dateFrom = Convert.ToDateTime(request.DateFrom);

                        dateFrom = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, 0, 0, 0);
                        switch (request.DateType)
                        {
                            case "Proposal":
                                sql += "AND (d.EstimatedStartDate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "ContractEnd":
                                sql += "AND (d.ContractEndDate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "CreatedDate":
                                sql += "AND (d.CreatedDate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "UpdatedDate":
                                sql += "AND (d.LastUpdate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                        }
                    }
                    if (!string.IsNullOrEmpty(request.DateTo))
                    {
                        var dateTo = Convert.ToDateTime(request.DateTo);
                        dateTo = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day, 23, 59, 0);
                        switch (request.DateType)
                        {
                            case "Proposal":
                                sql += "AND (d.EstimatedStartDate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "ContractEnd":
                                sql += "AND (d.ContractEndDate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "CreatedDate":
                                sql += "AND (d.CreatedDate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "UpdatedDate":
                                sql += "AND (d.LastUpdate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                        }
                    }
                }
                sql += "AND (d.Deleted = 0) ";
                sql += "AND (u.Deleted = 0) ";
                sql += "GROUP BY u.UserId, u.FullName, s.SalesStageName, u.LocationName ";
                sql += "ORDER BY u.FullName, s.SalesStageName";

                var dataCenter = user.DataCenter;
                var connectionString = LoginUser.GetConnection();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // populate dataReader
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader dataReader = cmd.ExecuteReader();

                    // check if no records
                    if (!dataReader.HasRows) { return salesForecastBySalesRepStage; }

                    // iterate though industry totals records
                    while (dataReader.Read())
                    {
                        var salesRepId = (int)dataReader.GetValue(0);
                        var salesRep = dataReader.GetValue(1).ToString();
                        var salesStage = dataReader.GetValue(2).ToString();
                        var locationName = dataReader.GetValue(3).ToString();
                        var revenue = (double)dataReader.GetValue(4);
                        revenue = (int)revenue; 
                        var profit = (double)dataReader.GetValue(5);

                        // convert profit and revenue to user's currency
                        var userCurrencyProfit = profit;
                        var userCurrencyRevenue = revenue;
                        if (user.CurrencyCode != "USD")
                        {
                            userCurrencyProfit = new Currencies().GetCalculatedCurrencyExchangeValue("USD", user.CurrencyCode, profit);
                            userCurrencyRevenue = new Currencies().GetCalculatedCurrencyExchangeValue("USD", user.CurrencyCode, revenue);
                        }

                        // Per Year reporting
                        if (targetShippingFrequency == "Per Year")
                        {
                            userCurrencyProfit = userCurrencyProfit * 12;
                            userCurrencyRevenue = userCurrencyRevenue * 12;
                        }

                        // populate chart table
                        if (userCurrencyProfit > 0 || userCurrencyRevenue > 0)
                        {
                            salesForecastBySalesRepStage.Add(new SalesForecastBySalesRepStage
                            {
                                CurrencyCode = user.CurrencyCode,
                                CurrencySymbol = userCurrencySymbol,
                                Profit = userCurrencyProfit,
                                Revenue = (int)userCurrencyRevenue,
                                SalesRep = salesRep,
                                UserId = salesRepId,
                                SalesStage = salesStage
                            });
                        }
                    }
                    conn.Close();
                }
            }
            return salesForecastBySalesRepStage;
        }

        public List<SalesForecastByCountry> GetSalesForecastByCountry(DashboardDataRequest request)
        {
            var salesForecastByCountry = new List<SalesForecastByCountry>();

            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var user = context.Users.FirstOrDefault(t => t.UserId == request.UserId);
            if (user != null)
            {
                var targetShippingFrequency = new Subscribers().GetShippingFrequency(user.SubscriberId) ?? "Per Month";
                var userCurrencySymbol = new Currencies().GetCurrencySymbolFromCode(user.CurrencyCode);

                var sql = "SELECT u.CountryName,u.CountryCode, Sum(d.RevenueUSD) AS RevenueUSD, Sum(d.ProfitUSD) AS ProfitUSD ";
                sql += "FROM Deals AS d ";
                sql += "INNER JOIN Users As u ON(d.DealOwnerId = u.UserId) AND(d.SubscriberId = u.SubscriberId) ";
                sql += "WHERE (d.SubscriberId = " + request.SubscriberId + ") ";
                if (request.CountryCodes != null && request.CountryCodes.Count > 0)
                {
                    var strCountryCodes = "";
                    foreach (var countryCode in request.CountryCodes)
                    {
                        if (!string.IsNullOrEmpty(strCountryCodes))
                            strCountryCodes += ",";
                        strCountryCodes += "'" + countryCode + "'";
                    }
                    sql += "AND (u.CountryCode IN (" + strCountryCodes + ")) ";
                }
                if (request.LocationCodes != null && request.LocationCodes.Count > 0)
                {
                    var strLocationCodes = "";
                    foreach (var locationCode in request.LocationCodes)
                    {
                        if (!string.IsNullOrEmpty(strLocationCodes))
                            strLocationCodes += ",";
                        strLocationCodes += "'" + locationCode + "'";
                    }
                    sql += "AND (u.LocationCode IN (" + strLocationCodes + ")) ";
                }

                if (request.SalesRepIds != null && request.SalesRepIds.Count > 0)
                {
                    var strSalesRepIds = "";
                    foreach (var salesRepId in request.SalesRepIds)
                    {
                        if (!string.IsNullOrEmpty(strSalesRepIds))
                            strSalesRepIds += ",";
                        strSalesRepIds += "" + salesRepId + "";
                    }
                    sql += "AND (d.DealOwnerId IN (" + strSalesRepIds + ")) ";
                }

                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    sql += "AND (d.DealName = '" + request.Keyword + "') ";
                }
                if (request.Status == "ACTIVE")
                {
                    sql += "AND (d.SalesStageName) <> 'won' ";
                    sql += "And(d.SalesStageName) <> 'lost' ";
                    sql += "And(d.SalesStageName) <> 'stalled' ";
                }
                else if (request.Status == "INACTIVE")
                {
                    sql += "AND ((d.SalesStageName) = 'won' ";
                    sql += "OR (d.SalesStageName) = 'lost' ";
                    sql += "OR (d.SalesStageName) = 'stalled' )";
                }
                if (!string.IsNullOrEmpty(request.DateType))
                {
                    if (!string.IsNullOrEmpty(request.DateFrom))
                    {
                        var dateFrom = Convert.ToDateTime(request.DateFrom);

                        dateFrom = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, 0, 0, 0);
                        switch (request.DateType)
                        {
                            case "Proposal":
                                sql += "AND (d.EstimatedStartDate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "ContractEnd":
                                sql += "AND (d.ContractEndDate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "CreatedDate":
                                sql += "AND (d.CreatedDate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "UpdatedDate":
                                sql += "AND (d.LastUpdate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                        }
                    }
                    if (!string.IsNullOrEmpty(request.DateTo))
                    {
                        var dateTo = Convert.ToDateTime(request.DateTo);
                        dateTo = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day, 23, 59, 0);
                        switch (request.DateType)
                        {
                            case "Proposal":
                                sql += "AND (d.EstimatedStartDate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "ContractEnd":
                                sql += "AND (d.ContractEndDate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "CreatedDate":
                                sql += "AND (d.CreatedDate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "UpdatedDate":
                                sql += "AND (d.LastUpdate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                        }
                    }
                }
                sql += "AND (d.Deleted = 0) ";
                sql += "AND (u.Deleted = 0) ";
                //   sql += "AND d.SpotDeal = 0 ";
                sql += "GROUP BY u.CountryName, u.CountryCode ";
                sql += "ORDER BY u.CountryName";


                var dataCenter = user.DataCenter;
                var connectionString = LoginUser.GetConnection();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // populate dataReader
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader dataReader = cmd.ExecuteReader();

                    // check if no records
                    if (!dataReader.HasRows) { return salesForecastByCountry; }

                    // iterate though industry totals records
                    while (dataReader.Read())
                    {
                        var countryName = dataReader.GetValue(0).ToString();
                        var countryCode = dataReader.GetValue(1).ToString();
                        var revenue = (double)dataReader.GetValue(2);
                        var profit = (double)dataReader.GetValue(3);

                        // convert profit and revenue to user's currency
                        var userCurrencyProfit = profit;
                        var userCurrencyRevenue = revenue;
                        if (user.CurrencyCode != "USD")
                        {
                            userCurrencyProfit = new Currencies().GetCalculatedCurrencyExchangeValue("USD", user.CurrencyCode, profit);
                            userCurrencyRevenue = new Currencies().GetCalculatedCurrencyExchangeValue("USD", user.CurrencyCode, revenue);
                        }

                        // Per Year reporting
                        if (targetShippingFrequency == "Per Year")
                        {
                            userCurrencyProfit = userCurrencyProfit * 12;
                            userCurrencyRevenue = userCurrencyRevenue * 12;
                        }

                        // populate chart table
                        if (userCurrencyProfit > 0 || userCurrencyRevenue > 0)
                        {
                            salesForecastByCountry.Add(new SalesForecastByCountry()
                            {
                                CurrencyCode = user.CurrencyCode,
                                CurrencySymbol = userCurrencySymbol,
                                CountryName = countryName,
                                CountryCode = countryCode,
                                Profit = userCurrencyProfit,
                                Revenue = userCurrencyRevenue
                            });
                        }
                    }
                    conn.Close();
                }

            }
            return salesForecastByCountry.OrderByDescending(t => t.Revenue).ToList();
        }


        public List<VolumesByLocation> GetVolumesByLocation(DashboardDataRequest request)
        {
            var countryVolumesByLocation = new List<VolumesByLocation>();

            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var user = context.Users.FirstOrDefault(t => t.UserId == request.UserId);
            if (user != null)
            {
                var targetShippingFrequency = new Subscribers().GetShippingFrequency(user.SubscriberId) ?? "Per Month";
                var userCurrencySymbol = new Currencies().GetCurrencySymbolFromCode(user.CurrencyCode);

                // TODO: Sum by the locationName field in Users table
                var sql = "SELECT  u.LocationName, u.LocationCode, ";
                sql += "Sum(d.TEUs + (d.CBMs / 35)) AS OceanTeus, ";
                sql += "Sum(d.Tonnes + (d.Kgs * 0.001) + (d.Lbs * 0.000453592)) AS AirTonnage ";
                sql += "FROM Deals AS d ";
                sql += "INNER JOIN Users As u ON(d.DealOwnerId = u.UserId) AND(d.SubscriberId = u.SubscriberId) ";
                sql += "WHERE (d.SubscriberId = " + request.SubscriberId + ") ";
                if (request.CountryCodes != null && request.CountryCodes.Count > 0)
                {
                    var strCountryCodes = "";
                    foreach (var countryCode in request.CountryCodes)
                    {
                        if (!string.IsNullOrEmpty(strCountryCodes))
                            strCountryCodes += ",";
                        strCountryCodes += "'" + countryCode + "'";
                    }
                    sql += "AND (u.CountryCode IN (" + strCountryCodes + ")) ";
                }
                if (request.LocationCodes != null && request.LocationCodes.Count > 0)
                {
                    var strLocationCodes = "";
                    foreach (var locationCode in request.LocationCodes)
                    {
                        if (!string.IsNullOrEmpty(strLocationCodes))
                            strLocationCodes += ",";
                        strLocationCodes += "'" + locationCode + "'";
                    }
                    sql += "AND (u.LocationCode IN (" + strLocationCodes + ")) ";
                }

                if (request.SalesRepIds != null && request.SalesRepIds.Count > 0)
                {
                    var strSalesRepIds = "";
                    foreach (var salesRepId in request.SalesRepIds)
                    {
                        if (!string.IsNullOrEmpty(strSalesRepIds))
                            strSalesRepIds += ",";
                        strSalesRepIds += "" + salesRepId + "";
                    }
                    sql += "AND (d.DealOwnerId IN (" + strSalesRepIds + ")) ";
                }

                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    sql += "AND (d.DealName = '" + request.Keyword + "') ";
                }
                if (request.Status == "ACTIVE")
                {
                    sql += "AND (d.SalesStageName) <> 'won' ";
                    sql += "And(d.SalesStageName) <> 'lost' ";
                    sql += "And(d.SalesStageName) <> 'stalled' ";
                }
                else if (request.Status == "INACTIVE")
                {
                    sql += "AND ((d.SalesStageName) = 'won' ";
                    sql += "OR (d.SalesStageName) = 'lost' ";
                    sql += "OR (d.SalesStageName) = 'stalled' )";
                }
                if (!string.IsNullOrEmpty(request.DateType))
                {
                    if (!string.IsNullOrEmpty(request.DateFrom))
                    {
                        var dateFrom = Convert.ToDateTime(request.DateFrom);

                        dateFrom = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, 0, 0, 0);
                        switch (request.DateType)
                        {
                            case "Proposal":
                                sql += "AND (d.EstimatedStartDate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "ContractEnd":
                                sql += "AND (d.ContractEndDate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "CreatedDate":
                                sql += "AND (d.CreatedDate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "UpdatedDate":
                                sql += "AND (d.LastUpdate >= '" + dateFrom.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                        }
                    }
                    if (!string.IsNullOrEmpty(request.DateTo))
                    {
                        var dateTo = Convert.ToDateTime(request.DateTo);
                        dateTo = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day, 23, 59, 0);
                        switch (request.DateType)
                        {
                            case "Proposal":
                                sql += "AND (d.EstimatedStartDate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "ContractEnd":
                                sql += "AND (d.ContractEndDate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "CreatedDate":
                                sql += "AND (d.CreatedDate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                            case "UpdatedDate":
                                sql += "AND (d.LastUpdate <= '" + dateTo.ToString("yyyy-MM-dd HH:mm:000") + "') ";
                                break;
                        }
                    }
                }
                sql += "AND (d.Deleted = 0) ";
                sql += "AND (u.Deleted = 0) ";
                //  sql += "AND d.SpotDeal = 0 ";
                sql += "AND d.Services IN ('Ocean FCL', 'Ocean LCL', 'Air') ";
                sql += "GROUP BY u.LocationName, u.LocationCode ";
                sql += "ORDER BY u.LocationName";


                var dataCenter = user.DataCenter;
                var connectionString = LoginUser.GetConnection();

                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // populate dataReader
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader dataReader = cmd.ExecuteReader();

                    // check if no records
                    if (!dataReader.HasRows) { return countryVolumesByLocation; }

                    // iterate though industry totals records
                    while (dataReader.Read())
                    {
                        var locationName = dataReader.GetValue(0).ToString();
                        var locationCode = dataReader.GetValue(1).ToString();
                        var oceanTeus = (double)dataReader.GetValue(2);
                        var airTonnage = (double)dataReader.GetValue(3);

                        // Per Year reporting
                        if (targetShippingFrequency == "Per Year")
                        {
                            oceanTeus = oceanTeus * 12;
                            airTonnage = airTonnage * 12;
                        }

                        // populate chart table
                        if (oceanTeus > 0 || airTonnage > 0)
                        {
                            countryVolumesByLocation.Add(new VolumesByLocation
                            {
                                AirTonnage = (int)airTonnage,
                                LocationName = locationName,
                                LocationCode = locationCode,
                                OceanTeus = (int)oceanTeus
                            });
                        }
                    }
                    conn.Close();
                }

            }
            return countryVolumesByLocation;
        }

    }
}