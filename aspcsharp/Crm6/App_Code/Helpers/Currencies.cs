using System;
using System.Linq;
using Crm6.App_Code.Shared;

namespace Helpers
{
    public class Currencies
    {

        public double GetCurrencyExchangeRate(string currencyCode)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);

            var exRate = sharedContext.CurrencyExchangeRates.Where(t => t.CurrencyCode.Equals(currencyCode)).OrderByDescending(t => t.ExchangeDate).FirstOrDefault();
            if (exRate != null)
            {
                return exRate.ExchangeRate ?? 0.0;
            }
            return 0.0;
        }


        public double GetCurrencyExchangeRate(string currencyCode, DateTime exchangeDate)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            string strExchangeDate = exchangeDate.ToString();
            if (!string.IsNullOrEmpty(currencyCode) & strExchangeDate != "#12:00:00 AM#")
            {
                var exRate = sharedContext.CurrencyExchangeRates.Where(ce => ce.CurrencyCode.Equals(currencyCode) && ce.ExchangeDate == exchangeDate)
                     .Select(ce => ce.ExchangeRate).FirstOrDefault();
                if (exRate.HasValue)
                    return exRate.Value;
                else
                {
                    // If ExchangeRate Does Not Exist for Currency and Date - Get most Recent Exchange Rate
                    return GetMostRecentCurrencyExchangeRate(currencyCode);
                }
            }
            return 0;
        }


        public double GetMostRecentCurrencyExchangeRate(string currencyCode)
        {
            if (!string.IsNullOrEmpty(currencyCode))
            {
                var sharedConnection = LoginUser.GetSharedConnection();
                var sharedContext = new DbSharedDataContext(sharedConnection);
                var exRate = sharedContext.CurrencyExchangeRates.Where(ce => ce.CurrencyCode.Equals(currencyCode)).OrderByDescending(ce => ce.ExchangeDate)
                     .Select(ce => ce.ExchangeRate).FirstOrDefault();
                return (exRate.HasValue ? exRate.Value : 0);
            }
            return 0;
        }


        public string RenderCurrencyFromCurrencyCode(double amount, string currencyCode, int decimalPlaces)
        {
            // get currency symbol
            var currencySymbol = GetCurrencySymbolFromCode(currencyCode);
            // set string
            return amount > 0 ? (currencySymbol + Math.Round(amount, decimalPlaces).ToString("n")) : ""; 
        }


        public string GetCurrencySymbolFromCode(string currencyCode)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var currencySymbol = sharedContext.Currencies.Where(c => c.CurrencyCode.Equals(currencyCode)).Select(t => t.CurrencySymbol).FirstOrDefault() ?? "";
            return currencySymbol;}


        public double GetCalculatedCurrencyExchangeValue(string sourceCurrencyCode, string targetCurrencyCode, double amount)
        {
            double calculatedAmount = 0;
            if (!string.IsNullOrEmpty(sourceCurrencyCode) & !string.IsNullOrEmpty(targetCurrencyCode))
            {
                // Get Source and Target Exchange Rates on the Exchange Date
                double sourceExchangeRate = GetCurrencyExchangeRate(sourceCurrencyCode);
                double dblTargetExchangeRate = GetCurrencyExchangeRate(targetCurrencyCode);
                // Translate Source Amount to USD
                double sourceAmountUsd = 0.0;
                if (sourceExchangeRate > 0)
                {
                    try
                    {
                        sourceAmountUsd = amount / sourceExchangeRate;
                    }
                    catch (Exception) { }
                }
                // Round to four decimals
                sourceAmountUsd = Math.Round(sourceAmountUsd, 4);
                // Translate USD Amount to Target Currency
                calculatedAmount = sourceAmountUsd * dblTargetExchangeRate;
                // Round to four decimals
                calculatedAmount = Math.Round(calculatedAmount, 4);
            }
            return calculatedAmount;
        }
    }
}
