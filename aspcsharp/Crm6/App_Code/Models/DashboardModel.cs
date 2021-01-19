
using System.Collections.Generic;

namespace Models
{

    public class DashboardDataRequest
    {
        public int UserId { get; set; }
        public int SubscriberId { get; set; }
        public List<string> CountryCodes { get; set; }
        public List<string> LocationCodes { get; set; }
        public List<int> SalesRepIds { get; set; }
        public string Keyword { get; set; }
        public string Status { get; set; }
        public string DateType { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }

    public class SalesForecastByStage
    {
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public double Profit { get; set; }
        public double Revenue { get; set; }
        public string SalesStage { get; set; }
        public int SortOrder { get; set; }
        public int DealCount { get; set; }
    }

    public class DealsByIndustry
    {
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public string Industry { get; set; }
        public double Profit { get; set; }
        public double Revenue { get; set; }
    }

    public class SalesForecastBySalesRep
    {
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public double Profit { get; set; }
        public double Revenue { get; set; }
        public string SalesRep { get; set; }
        public int UserId { get; set; }  // Sales Rep UserId
    }

    public class SalesForecastBySalesRepStage
    {
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public double Profit { get; set; }
        public double Revenue { get; set; }
        public string SalesRep { get; set; }
        public int UserId { get; set; }
        public string SalesStage { get; set; }
    }

    public class SalesForecastByLocation
    {
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public double Profit { get; set; }
        public double Revenue { get; set; }
    }


    public class SalesForecastByCountry
    {
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public double Profit { get; set; }
        public double Revenue { get; set; }
    }


    public class VolumesByLocation
    {
        public double AirTonnage { get; set; }
        public string LocationName { get; set; }
        public double OceanTeus { get; set; }
        public string LocationCode { get; set; }
    }

}
