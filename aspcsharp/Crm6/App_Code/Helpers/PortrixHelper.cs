using Crm6.App_Code.Models;
using Crm6.App_Code.Shared;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Crm6.App_Code.Helpers
{
    public class PortrixHelper
    {
        private const string baseAddress = "https://gpm-pls-demo.portrix-ls.de/";
        //private HttpClient client = new HttpClient();

        //public PortrixHelper()
        //{
        //    client.BaseAddress = new Uri(baseAddress);
        //    client.DefaultRequestHeaders.Accept.Clear();
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //}

        //private HttpContent GetContent(PortrixClientModel model)
        //{
        //    var myContent = JsonConvert.SerializeObject(model);
        //    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
        //    var byteContent = new ByteArrayContent(buffer);
        //    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        //    return byteContent;
        //}

        //public async Task<int> CreateClient(PortrixClientModel model)
        //{
        //    using (HttpResponseMessage response = await client.PostAsync("apis/1.1/clients", GetContent(model)).ConfigureAwait(false))
        //    {
        //        if (response.IsSuccessStatusCode)
        //        {
        //            using (HttpContent content = response.Content)
        //            {
        //                string result = await content.ReadAsStringAsync();

        //                if (string.IsNullOrWhiteSpace(result) == false)
        //                {
        //                    var jsonResponse = JsonConvert.DeserializeObject<PortrixClientResponseModel>(result);

        //                    if (jsonResponse != null)
        //                    {
        //                        return jsonResponse.id;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return 0;
        //}

        //public async Task<int> EditClient(PortrixClientModel model, int clientId)
        //{
        //    using (HttpResponseMessage response = await client.PutAsync($"apis/1.1/clients/{clientId}", GetContent(model)).ConfigureAwait(false))
        //    {
        //        if (response.IsSuccessStatusCode)
        //        {
        //            using (HttpContent content = response.Content)
        //            {
        //                string result = await content.ReadAsStringAsync();

        //                if (string.IsNullOrWhiteSpace(result) == false)
        //                {
        //                    var jsonResponse = JsonConvert.DeserializeObject<PortrixClientResponseModel>(result);

        //                    if (jsonResponse != null)
        //                    {
        //                        return jsonResponse.id;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return 0;
        //}

        //public async void DeleteClient(int clientId)
        //{
        //    await client.DeleteAsync($"apis/1.1/clients/{clientId}").ConfigureAwait(false);
        //}

        //public async void CreateCrmQuote(QuoteModel model)
        //{
        //    var connection = LoginUser.GetSharedConnectionForSubscriberId(model.SubscriberId);
        //    using (var context = new DbSharedDataContext(connection))
        //    {
        //        //Checks if the quote already exists for this quote provider.
        //        var existingQuote = (from t in context.Quotes where t.QuoteProvider.ToLower() == model.QuoteProvider.ToLower() && t.QuoteProviderId == model.QuoteProviderId select t)?.FirstOrDefault();

        //        //If quote exists, update it in the database.  Otherwise create new quote.
        //        if (existingQuote != null)
        //        {
        //            existingQuote.SubscriberId = model.SubscriberId;
        //            existingQuote.AllInFormat = model.AllInFormat;
        //            existingQuote.BranchCode = model.BranchCode;
        //            existingQuote.BranchName = model.BranchName;
        //            existingQuote.CarrierVisibility = model.CarrierVisibility;
        //            existingQuote.CompanyCode = model.CompanyCode;
        //            existingQuote.CompanyDescription = model.CompanyDescription;
        //            existingQuote.CompanyId = model.CompanyId;
        //            existingQuote.CompanyName = model.CompanyName;
        //            existingQuote.ContactId = model.ContactId;
        //            existingQuote.ContactName = model.ContactName;
        //            existingQuote.CreatedDate = model.CreatedDate;
        //            existingQuote.CreatedUserId = model.CreatedUserId;
        //            existingQuote.CreatedUserName = model.CreatedUserName;
        //            existingQuote.CurrencyCode = model.CurrencyCode;
        //            existingQuote.CustomerCode = model.CustomerCode;
        //            existingQuote.CustomerName = model.CustomerName;
        //            existingQuote.DealId = model.DealId;
        //            existingQuote.Deleted = model.Deleted;
        //            existingQuote.DeletedDate = model.DeletedDate;
        //            existingQuote.DeletedUserId = model.DeletedUserId;
        //            existingQuote.DeletedUserName = model.DeletedUserName;
        //            existingQuote.DeliveryDate = model.DeliveryDate;
        //            existingQuote.Destination = model.Destination;
        //            existingQuote.DestinationIataCode = model.DestinationIataCode;
        //            existingQuote.Incoterm = model.Incoterm;
        //            existingQuote.IncotermText = model.IncotermText;
        //            existingQuote.InternalNotes = model.InternalNotes;
        //            existingQuote.Language = model.Language;
        //            existingQuote.LastUpdate = model.LastUpdate;
        //            existingQuote.Mode = model.Mode;
        //            existingQuote.NatureOfGoods = model.NatureOfGoods;
        //            existingQuote.Origin = model.Origin;
        //            existingQuote.OriginIataCode = model.OriginIataCode;
        //            existingQuote.OriginSystem = model.OriginSystem;
        //            existingQuote.PaymentConditions = model.PaymentConditions;
        //            existingQuote.PdfLink = model.PdfLink;
        //            existingQuote.ProfitGlobal = model.ProfitGlobal;
        //            existingQuote.ProfitType = model.ProfitType;
        //            existingQuote.QuoteAmount = model.QuoteAmount;
        //            existingQuote.QuoteCode = model.QuoteCode;
        //            existingQuote.QuoteConditions = model.QuoteConditions;
        //            existingQuote.QuoteDescription = model.QuoteDescription;
        //            existingQuote.QuoteDetails = model.QuoteDetails;
        //            existingQuote.QuoteNotes = model.QuoteNotes;
        //            existingQuote.QuoteNumber = model.QuoteNumber;
        //            existingQuote.QuoteStatus = model.QuoteStatus;
        //            existingQuote.QuoteSubject = model.QuoteSubject;
        //            existingQuote.TotalCbms = model.TotalCbms;
        //            existingQuote.TotalGrossWeight = model.TotalGrossWeight;
        //            existingQuote.TotalVolumetricWeight = model.TotalVolumetricWeight;
        //            existingQuote.TotalWeight = model.TotalWeight;
        //            existingQuote.TransitTime = model.TransitTime;
        //            existingQuote.UpdateUserId = model.UpdateUserId;
        //            existingQuote.UpdateUserName = model.UpdateUserName;
        //            existingQuote.UserCode = model.UserCode;
        //            existingQuote.UserId = model.UserId;
        //            existingQuote.UserEmail = model.UserEmail;
        //            existingQuote.UserIndoorEmail = model.UserIndoorEmail;
        //            existingQuote.UserIndoorCode = model.UserIndoorCode;
        //            existingQuote.UserOutdoorEmail = model.UserOutdoorEmail;
        //            existingQuote.UserOutdoorCode = model.UserOutdoorCode;
        //            existingQuote.ValidFom = model.ValidFom;
        //            existingQuote.ValidTo = model.ValidTo;
        //            existingQuote.WonOption = model.WonOption;
        //        }
        //        else
        //        {
        //            Shared.Quote newQuote = new Shared.Quote
        //            {
        //                SubscriberId = model.SubscriberId,
        //                AllInFormat = model.AllInFormat,
        //                BranchCode = model.BranchCode,
        //                BranchName = model.BranchName,
        //                CarrierVisibility = model.CarrierVisibility,
        //                CompanyCode = model.CompanyCode,
        //                CompanyDescription = model.CompanyDescription,
        //                CompanyId = model.CompanyId,
        //                CompanyName = model.CompanyName,
        //                ContactId = model.ContactId,
        //                ContactName = model.ContactName,
        //                CreatedDate = model.CreatedDate,
        //                CreatedUserId = model.CreatedUserId,
        //                CreatedUserName = model.CreatedUserName,
        //                CurrencyCode = model.CurrencyCode,
        //                CustomerCode = model.CustomerCode,
        //                CustomerName = model.CustomerName,
        //                DealId = model.DealId,
        //                Deleted = model.Deleted,
        //                DeletedDate = model.DeletedDate,
        //                DeletedUserId = model.DeletedUserId,
        //                DeletedUserName = model.DeletedUserName,
        //                DeliveryDate = model.DeliveryDate,
        //                Destination = model.Destination,
        //                DestinationIataCode = model.DestinationIataCode,
        //                Incoterm = model.Incoterm,
        //                IncotermText = model.IncotermText,
        //                InternalNotes = model.InternalNotes,
        //                Language = model.Language,
        //                LastUpdate = model.LastUpdate,
        //                Mode = model.Mode,
        //                NatureOfGoods = model.NatureOfGoods,
        //                Origin = model.Origin,
        //                OriginIataCode = model.OriginIataCode,
        //                OriginSystem = model.OriginSystem,
        //                PaymentConditions = model.PaymentConditions,
        //                PdfLink = model.PdfLink,
        //                ProfitGlobal = model.ProfitGlobal,
        //                ProfitType = model.ProfitType,
        //                QuoteAmount = model.QuoteAmount,
        //                QuoteCode = model.QuoteCode,
        //                QuoteConditions = model.QuoteConditions,
        //                QuoteDescription = model.QuoteDescription,
        //                QuoteDetails = model.QuoteDetails,
        //                QuoteNotes = model.QuoteNotes,
        //                QuoteNumber = model.QuoteNumber,
        //                QuoteProvider = model.QuoteProvider,
        //                QuoteProviderId = model.QuoteProviderId,
        //                QuoteStatus = model.QuoteStatus,
        //                QuoteSubject = model.QuoteSubject,
        //                TotalCbms = model.TotalCbms,
        //                TotalGrossWeight = model.TotalGrossWeight,
        //                TotalVolumetricWeight = model.TotalVolumetricWeight,
        //                TotalWeight = model.TotalWeight,
        //                TransitTime = model.TransitTime,
        //                UpdateUserId = model.UpdateUserId,
        //                UpdateUserName = model.UpdateUserName,
        //                UserCode = model.UserCode,
        //                UserId = model.UserId,
        //                UserEmail = model.UserEmail,
        //                UserIndoorEmail = model.UserIndoorEmail,
        //                UserIndoorCode = model.UserIndoorCode,
        //                UserOutdoorEmail = model.UserOutdoorEmail,
        //                UserOutdoorCode = model.UserOutdoorCode,
        //                ValidFom = model.ValidFom,
        //                ValidTo = model.ValidTo,
        //                WonOption = model.WonOption
        //            };

        //            context.Quotes.InsertOnSubmit(newQuote);
        //        }

        //        context.SubmitChanges();
        //    }
        //}
    }
}