using Crm6.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Crm6.App_Code.Helpers
{
    public class QuotesHelper
    {
        public int SaveQuote(Quote quoteDetails, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            var quote = context.Quotes.FirstOrDefault(t => t.SubscriberId == quoteDetails.SubscriberId &&
                                                                             t.QuoteId == quoteDetails.QuoteId) ?? new Quote();
            // fill details
            quote.CompanyId = quoteDetails.CompanyId;
            quote.DealId = quoteDetails.DealId;
            quote.UserId = quoteDetails.UserId;
            quote.BranchCode = quoteDetails.BranchCode;
            quote.IncotermText = quoteDetails.IncotermText;
            quote.QuoteCode = quoteDetails.QuoteCode;
            quote.TotalPackages = quoteDetails.TotalPackages;

            if (quote.QuoteId < 1)
            {
                quote.SubscriberId = quoteDetails.SubscriberId;
                quote.CreatedUserId = quoteDetails.UpdateUserId;
                quote.CreatedUserName = quoteDetails.UpdateUserName;
                quote.CreatedDate = DateTime.UtcNow;
                context.Quotes.InsertOnSubmit(quote);
            } 

            context.SubmitChanges();

            return quote.QuoteId;
        }

        public Quote GetQuote(int quoteId, int subscriberId)
        {
            var connection = LoginUser.GetConnection( );
            var context = new DbFirstFreightDataContext(connection);
            return context.Quotes.FirstOrDefault(t => t.QuoteId == quoteId);
        }
    }
}