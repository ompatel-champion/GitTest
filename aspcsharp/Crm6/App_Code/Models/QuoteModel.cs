using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crm6.App_Code.Models
{
    public class QuoteModel
    {
        public int SubscriberId { get; set; }
        public int AllInFormat { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string CarrierVisibility { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyDescription { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int ContactId { get; set; }
        public string ContactName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int CreatedUserId { get; set; }
        public string CreatedUserName { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public int DealId { get; set; }
        public string DealName { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int DeletedUserId { get; set; }
        public string DeletedUserName { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string Destination { get; set; }
        public string DestinationIataCode { get; set; }
        public string Incoterm { get; set; }
        public string IncotermText { get; set; }
        public string InternalNotes { get; set; }
        public string Language { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string Mode { get; set; }
        public string NatureOfGoods { get; set; }
        public string Origin { get; set; }
        public string OriginIataCode { get; set; }
        public string OriginSystem { get; set; }
        public string PaymentConditions { get; set; }
        public string PdfLink { get; set; }
        public string ProfitGlobal { get; set; }
        public string ProfitType { get; set; }
        public decimal QuoteAmount { get; set; }
        public string QuoteCode { get; set; }
        public string QuoteConditions { get; set; }
        public string QuoteDescription { get; set; }
        public string QuoteDetails { get; set; }
        public string QuoteNotes { get; set; }
        public int QuoteNumber { get; set; }
        public string QuoteProvider { get; set; }
        public int QuoteProviderId { get; set; }
        public string QuoteStatus { get; set; }
        public string QuoteSubject { get; set; }
        public string ServiceType { get; set; }
        public decimal TotalCbms { get; set; }
        public decimal TotalGrossWeight { get; set; }
        public decimal TotalPackages { get; set; }
        public decimal TotalVolumetricWeight { get; set; }
        public decimal TotalWeight { get; set; }
        public string TransitTime { get; set; }
        public int UpdateUserId { get; set; }
        public string UpdateUserName { get; set; }
        public string UserCode { get; set; }
        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserIndoorEmail { get; set; }
        public string UserIndoorCode { get; set; }
        public string UserOutdoorEmail { get; set; }
        public string UserOutdoorCode { get; set; }
        public DateTime? ValidFom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string WonOption { get; set; }
    }
}