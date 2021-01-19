
using System.Collections.Generic;

namespace Models
{

    public class DealsReportFilters
    {
        // Deals Report Filters and Parameters
        public string ActiveSalesStage { get; set; }
        public string CompanyName { get; set; }
        public string ConsigneeName { get; set; }
        public string DateFrom { get; set; }
        public string DateLastActivityFrom { get; set; }
        public string DateLastActivityTo { get; set; }
        public string DateNextActivityFrom { get; set; }
        public string DateNextActivityTo { get; set; }
        public string DateReceivedFrom { get; set; }
        public string DateReceivedTo { get; set; }
        public string DateTo { get; set; }
        public string DateType { get; set; }
        public string DateWonFrom { get; set; }
        public string DateWonTo { get; set; }
        public int DealId { get; set; }
        public List<string> DealTypes { get; set; }
        public string DecisionDate { get; set; }
        public List<string> DestinationCountries { get; set; }   //IATA or Unloco
        public List<string> DestinationLocations { get; set; }
        public List<string> Industries { get; set; }
        public List<string> Campaigns { get; set; } 
        public List<string> LocationCodes { get; set; }
        public List<string> DistrictCodes { get; set; }
        public string ShippingFrquency { get; set; }

        // TODO: Is NOT Needed??  public string Mode { get; set; }   // Ocean / Air /Road / Logistics
        public string OrderBy { get; set; }
        public List<string> OriginCountries { get; set; }
        public List<string> OriginLocations { get; set; }       //IATA or Unloco
        public string RegionCode { get; set; }
        public string SalesRepCode { get; set; }
        public List<string> CountryCodes { get; set; }
        public string SalesRepDistrictCode { get; set; }
        public string SalesRepRegionCode { get; set; }
        public List<string> SalesRepLocationCodes { get; set; }
        public List<int> UserIds { get; set; }
        public List<string> SalesStages { get; set; }
        public List<string> ServiceTypes { get; set; }              // Ocean LCL, Ocean FCL, Air, LTL Expedited, LTL, FTL, Logistics
        public string ShipperName { get; set; }
        public string ShowLinkedDeals { get; set; }                 // False(Default)
        public string VolumeUnit { get; set; }                      // CBMs, Kgs, Lbs, TEUs, Tonnes
        public List<string> Competitors { get; set; }                     

        

        // User Settings
        public string CurrencyCode { get; set; }                    //"US" Default - Used for doing Currency Conversion from InputCurrency - based on what Currency the User wants to be displayed
        public string ReportTotals { get; set; }                    // Monthly or Yearly(Deafault)
        public int SubscriberId { get; set; }
        public int UserId { get; set; }                             // Logged-in user ID

        // Report Settings
        public int CurrentPage { get; set; }
        public string Keyword { get; set; }
        public int RecordsPerPage { get; set; }
        public string SortBy { get; set; }
        public List<string> CountryNames { get;  set; }
        public bool IsFromDashboard { get; set; }
        public bool IsSpotDeals { get; set; }
    }

    public class DealsReportItem
    {
        public int DealId { get; set; }
        public int SubscriberId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string ConsigneeNames { get; set; }
        public string DealName { get; set; }
        public string DealType { get; set; }
        public string DateContractEnd { get; set; }
        public string DateCreated { get; set; }
        public string DateUpdated { get; set; }
        public string LastActivityDate { get; set; }
        public string NextActivityDate { get; set; }
        public string UpdatedBy { get; set; }
        public string DateDecision { get; set; }
        public string DateFirstShipment { get; set; }
        public string DateLost { get; set; }
        public string DateWon { get; set; }
        public string DateProposalDue { get; set; }
        public string Industry { get; set; }
        public string LocationName { get; set; } 
        public string OriginCountries { get; set; }
        public string Origins { get; set; } 
        public string DestinationCountries { get; set; }
        public string Destinations { get; set; }
        public double Profit { get; set; }
        public double Revenue { get; set; }
        public double SpotProfit { get; set; }
        public double SpotRevenue { get; set; }
        public string SalesRepName { get; set; }
        public string SalesStage { get; set; }
        public string Services { get; set; }
        public string ShipperNames { get; set; }

        // Volumes
        public int VolumeOceanFclTeus { get; set; }
        public int VolumeOceanlclCbms { get; set; }
        public int VolumeOceanlclCubes { get; set; }
        public int VolumeAirTonnage { get; set; }
        public int VolumeRoadKgs { get; set; }
        public int VolumeRoadLbs { get; set; }
        public int VolumeLogisticsCbms { get; set; }
         
        public int UserId { get; set; }

        public string CurrencyCode { get; set; } 
        public string CurrencySymbol { get; set; }


        public int LBs { get; set; }
        public int CBMs { get; set; }
        public int TEUs { get; set; }
        public int KGs { get; set; }
        public int Tonnes { get; set; }

        public int SpotLBs { get; set; }
        public int SpotCBMs { get; set; }
        public int SpotTEUs { get; set; }
        public int SpotKGs { get; set; }
        public int SpotTonnes { get; set; }

        public string ReasonWonLost { get; set; }
        public string Comments { get; set; }
        public double ProfitRevenuePercentage { get; set; }

    }

    public class DealsReportResponse
    {
        public List<DealsReportItem> Deals { get; set; }
        public int RecordCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string ExcelUri { get; set; }
        // Report Totals
        public int TotalVolumeOceanFclTeus { get; set; }
        public int TotalVolumeOceanlclCbms { get; set; }
        public int TotalVolumeOceanlclCubes { get; set; }
        public int TotalVolumeAirTonnage { get; set; }
        public int TotalVolumeRoadKgs { get; set; }
        public int TotalVolumeRoadLbs { get; set; }
        public int TotalVolumeLogisticsCbms { get; set; }
    }

}
