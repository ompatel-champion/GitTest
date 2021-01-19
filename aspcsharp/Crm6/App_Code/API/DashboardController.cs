using System.Web.Http;
using Models;
using Helpers;
using System.Collections.Generic;
using Crm6.App_Code.Models;

namespace API
{
    public class DashboardController : ApiController
    {

        [AcceptVerbs("POST")]
        public List<SalesForecastByStage> GetSalesForecastByStage([FromBody] DashboardDataRequest request)
        {
            return new Dashboards().GetSalesForecastByStage(request);
        }


        [AcceptVerbs("POST")]
        public List<DealsByIndustry> GetDealsByIndustry([FromBody] DashboardDataRequest request)
        {
            return new Dashboards().GetDealsByIndustry(request);
        }

        [AcceptVerbs("POST")]
        public List<SalesForecastByLocation> GetSalesForecastByLocation([FromBody] DashboardDataRequest request)
        {
            return new Dashboards().GetSalesForecastByLocation(request);
        }


        [AcceptVerbs("POST")]
        public List<SalesForecastBySalesRep> GetSalesForecastBySalesReps([FromBody] DashboardDataRequest request)
        {
            return new Dashboards().GetSalesForecastBySalesReps(request);
        }

        [AcceptVerbs("POST")]
        public List<SalesForecastBySalesRepStage> GetSalesForecastBySalesRepStage([FromBody] DashboardDataRequest request)
        {
            return new Dashboards().GetSalesForecastBySalesRepStage(request);
        }

        [AcceptVerbs("POST")]
        public List<SalesForecastByCountry> GetSalesForecastByCountry([FromBody] DashboardDataRequest request)
        {
            return new Dashboards().GetSalesForecastByCountry(request);
        }

        [AcceptVerbs("POST")]
        public List<VolumesByLocation> GetVolumesByLocation([FromBody] DashboardDataRequest request)
        {
            return new Dashboards().GetVolumesByLocation(request);
        }

    }
}
