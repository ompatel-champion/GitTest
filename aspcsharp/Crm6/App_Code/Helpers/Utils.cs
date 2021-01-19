using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Models;
using System.Web;
using Crm6.App_Code;
using System.Net;
using System.Net.Sockets;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Helpers
{
    public class Utils
    {

        public static string GetDisplayDate(DateTime? dt)
        {
            try
            {
                return dt.HasValue ? dt.Value.ToString("dd-MMM-yy HH:mm") : "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static bool IsValidEmail( string email)
        {
            const string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            var regex = new Regex(pattern, RegexOptions.IgnoreCase);

            return regex.IsMatch(email);
        }
                      
        public static string StripHtml(string html)
        {
            var result = Regex.Replace(html, @"<[^>]*>", String.Empty);
            result = result.Replace("&nbsp;", " ");
            result = HttpUtility.HtmlDecode(result);
            return result;
        }
        
        public LatitudeLongitude ConvertUnlocoCoordinatesToDecimal(string unLocodeCoordinates)
        {
            LatitudeLongitude decimalCoordinates = new LatitudeLongitude();
            // N +  S -
            // W -  E +

            // Decimal degrees = whole number of degrees, plus minutes divided by 60, plus seconds divided by 3600

            //Sacramento, CA
            //3834N 12129W
            //38.6954002380
            //-121.59100342

            //Monterey, CA
            //3636N 12154W
            //36.5870018005
            //-121.84300232

            int longitudeDegrees;
            double longitudeMinutes;
            string eastWest;
            var longitudeDecimal = 0.00;
            // Parse Unloco Coordinates String for
            var latLong = unLocodeCoordinates.Split(' ');

            // Get Decimal Latitude
            int latitudeDegrees;
            double latitudeMinutes;
            string northSouth;
            var latitudeDecimal = 0.00;
            if (latLong[0].Length == 5)
            {
                latitudeDegrees = int.Parse(latLong[0].Substring(1, 2));
                latitudeMinutes = double.Parse("." + latLong[0].Substring(3, 2));
                northSouth = latLong[0].Substring(5, 1);
            }
            else
            {
                latitudeDegrees = int.Parse(latLong[0].Substring(1, 3));
                latitudeMinutes = double.Parse("." + latLong[0].Substring(4, 2));
                northSouth = latLong[0].Substring(6, 1);
            }
            if (northSouth == "S")
            {
                latitudeDegrees = -latitudeDegrees;
            }
            latitudeDecimal = latitudeDegrees + latitudeMinutes;

            // Get Decimal Longitude
            if (latLong[1].Length == 5)
            {
                longitudeDegrees = int.Parse(latLong[1].Substring(1, 2));
                longitudeMinutes = double.Parse("." + latLong[1].Substring(3, 2));
                eastWest = latLong[1].Substring(5, 1);
            }
            else
            {
                longitudeDegrees = int.Parse(latLong[1].Substring(1, 3));
                longitudeMinutes = double.Parse("." + latLong[1].Substring(4, 2));
                eastWest = latLong[1].Substring(6, 1);
            }
            if (eastWest == "W")
            {
                longitudeDegrees = -longitudeDegrees;
            }
            longitudeDecimal = longitudeDegrees + longitudeMinutes;

            decimalCoordinates.Latitude = latitudeDecimal;
            decimalCoordinates.Longitude = longitudeDecimal;

            return decimalCoordinates;
        }
        
        /// <summary>
        /// changes the querystring values
        /// </summary>
        /// <param name="currentPageUrl"></param>
        /// <param name="paramToReplace"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static string ChangeQsValue(string currentPageUrl, string paramToReplace, string newValue)
        {
            string urlWithoutQuery = currentPageUrl.IndexOf('?') >= 0
                ? currentPageUrl.Substring(0, currentPageUrl.IndexOf('?'))
                : currentPageUrl;

            string queryString = currentPageUrl.IndexOf('?') >= 0
                ? currentPageUrl.Substring(currentPageUrl.IndexOf('?'))
                : null;

            var queryParamList = queryString != null
                ? HttpUtility.ParseQueryString(queryString)
                : HttpUtility.ParseQueryString(string.Empty);

            if (queryParamList[paramToReplace] != null)
            {
                queryParamList[paramToReplace] = newValue;
            }
            else
            {
                queryParamList.Add(paramToReplace, newValue);
            }
            return String.Format("{0}?{1}", urlWithoutQuery, queryParamList);
        }

        //encode a string as base64
        public static string Base64Encode(string str) {
            var bytes = System.Text.Encoding.UTF8.GetBytes(str);
            return System.Convert.ToBase64String(bytes);
        }
        
        public static String Base64StringToHexString(String base64String)
        {
            var bytes = Convert.FromBase64String(base64String);
            var sbHexString = new StringBuilder();
            for (var i = 0; i < bytes.Length; i++)
            {
                sbHexString.Append(bytes[i].ToString("X2"));
            }
            return sbHexString.ToString();
        }
        
        public static string GetElapsedTime(DateTime? startTime, DateTime? endTime)
        {
            var elapsedTime = "00:00:00";
            if (endTime != null && startTime != null)
            {
                var span = endTime.Value.Subtract(startTime.Value);
                var hours = PadString(Convert.ToString(span.Hours), 2, "0");
                var minutes = PadString(Convert.ToString(span.Minutes), 2, "0");
                var seconds = PadString(Convert.ToString(span.Seconds), 2, "0");
                elapsedTime = hours + ":" + minutes + ":" + seconds;
            }
            return elapsedTime;
        }
        
        public static string PadString(string textIn, int length, string padChar)
        {
            // Pad string number with zeros
            var textOut = "";
            for (var i = 1; i <= length; i++)
            {
                if (textIn.Length < length)
                {
                    textOut = padChar + textIn;
                }
                else
                {
                    textOut = textIn;
                }
            }
            return textOut;
        }
        
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }


        //8 bytes randomly selected for both the Key and the Initialization Vector
        //the IV is used to encrypt the first block of text so that any repetitive 
        //patterns are not apparent
        private static byte[] KEY_64 = { 42, 16, 93, 156, 78, 4, 218, 32 };
        private static byte[] IV_64 = { 55, 103, 246, 79, 36, 99, 167, 3 };

        //24 byte or 192 bit key and IV for TripleDES
        private static byte[] KEY_192 = {42, 16, 93, 156, 78, 250, 218, 32, 15,
                                        167, 44, 80, 26, 250, 155, 112,
                                        2, 94, 11, 204, 119, 35, 184, 197};
        private static byte[] IV_192 = {55, 103, 246, 79, 36, 99, 167, 3, 42,
                                       250, 62, 83, 184, 7, 209, 13,    145,
                                       23, 200, 58, 173, 10, 121, 222};


        /// <summary>
        /// Standard DES encryption
        /// </summary>
        /// <param name="val">Accepts value to be encrypted using DES</param>
        /// <returns>Returns value encrypted in DES</returns>
        public static string Encrypt(string val)
        {
            string encrypted = "";
            if (val != "")
            {
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateEncryptor(KEY_64, IV_64), CryptoStreamMode.Write);
                StreamWriter sw = new StreamWriter(cs);

                sw.Write(val);
                sw.Flush();
                cs.FlushFinalBlock();
                ms.Flush();

                //convert back to string - added explicit conversion to int32
                encrypted = Convert.ToBase64String(ms.GetBuffer(), 0, Convert.ToInt32(ms.Length));
            }
            return encrypted;
        }
        
        /// <summary>
        /// Standard DES decryption
        /// </summary>
        /// <param name="val">Value of decrypted</param>
        /// <returns>Returns decrypted value as string</returns>
        public static string Decrypt(string val)
        {
            string decrpted = "";
            if (val != "")
            {
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();

                //convert from string to byte
                byte[] buffer = Convert.FromBase64String(val);
                MemoryStream ms = new MemoryStream(buffer);
                CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateDecryptor(KEY_64, IV_64), CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cs);
                decrpted = sr.ReadToEnd();
            }
            return decrpted;
        }
               
        public static string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
            }
            catch (Exception)
            {
            }
            return "";
        }

        public static bool IsNumeric(object expression)
        {
            if (expression == null)
                return false;

            double testDouble;
            if (expression is string)
            {
                CultureInfo provider;
                if (((string)expression).StartsWith("$"))
                    provider = new CultureInfo("en-US");
                else
                    provider = CultureInfo.InvariantCulture;

                if (double.TryParse((string)expression, NumberStyles.Any, provider, out testDouble))
                    return true;
            }
            else
            {
                if (double.TryParse(expression.ToString(), out testDouble))
                    return true;
            }

            //VB's 'IsNumeric' returns true for any boolean value:
            bool testBool;
            if (bool.TryParse(expression.ToString(), out testBool))
                return true;

            return false;
        }

        public static string WriteLogFile(string message)
        {
            var filePath = System.Web.HttpContext.Current.Server.MapPath("log/" + DateTime.Now.ToString("dd-MMM-yy-hhmm") + ".txt");
            //var streamWriter = new StreamWriter("c:\\file.txt", append: true);
            var logFile = File.AppendText(filePath);
            logFile.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture) + Environment.NewLine + message);
            logFile.Close();
            return message;
        }

        //get profile image url
        //note: if an imageUrl is not provided then an SVG base64 data URL/image showcasing the users initials on a random background color is created and returned.
        //note: if both the imageUrl and fullName are not provided then the defImageUrl is returned (typically a placeholder image)
        public static string GetProfileImageUrl(string imageUrl=null, string fullName=null, string placeholderImgUrl=null, int colorIndex=0)
        {
            string url = !string.IsNullOrEmpty(placeholderImgUrl)?placeholderImgUrl:"_content/_img/no-pic.png";
            string[] BG_COLORS = new string[] {
                "#1184C3",  //medium dark blue
                "#3E4955",  //dark grey
                "#0C629C",  //dark blue
                "#1FC39F",  //medium green
                "#A2AFBD",  //light grey
                "#039280",  //dark green
                "#1874A8",  //medium darker blue
                "#6D7B8C",  //medium grey
                "#1E92D2"   //medium blue
            };
            if (!string.IsNullOrEmpty(imageUrl)) url = imageUrl;
            else if (!string.IsNullOrEmpty(fullName)) {
                //NOTE! Initials are limited to a-z A-Z, and 0-9
                string initials = (new Regex(@"(\b[a-zA-Z0-9])[a-zA-Z0-9]* ?")).Replace(fullName, "$1");
                initials = (new Regex(@"[^a-zA-Z0-9]")).Replace(initials, "");
                if (initials.Length>2) initials = initials.Substring(0,1)+initials.Substring(initials.Length-1);
                colorIndex = colorIndex>0?colorIndex%BG_COLORS.Length:(new Random().Next(0, BG_COLORS.Length-1));
                var bgColor = BG_COLORS[colorIndex];
                //create inline SVG (typically used as an HTML image tag src data URL)
                var svgStr = "<svg";
                    svgStr += " viewBox='0 0 100 100'";
                    svgStr += " xmlns='http://www.w3.org/2000/svg'";
                        svgStr += " style='";
                            svgStr += "color:white;";
                            svgStr += "text-transform:uppercase;";
                            svgStr += "background-color:" + bgColor + ";";
                        svgStr += "'";
                svgStr += ">";
                    svgStr += "<text";
                        svgStr += " x='50%'";
                        svgStr += " y='50 %'";
                        svgStr += " dominant-baseline='central'";
                        svgStr += " text-anchor='middle'";
                        svgStr += " fill='white'";
                        svgStr += " font-size='50'";
                        svgStr += " font-family='Arial, Helvetica, Sans-Serif'";
                    svgStr += ">"+initials+"</text>";
                svgStr += "</svg>";
                svgStr = Utils.Base64Encode(svgStr);		
		        url = "data:image/svg+xml;base64,"+svgStr;
            }
            return url;
        }

        public static string AddWordBreakOpportunities(string str)
        {
            var regex = new Regex(@"(\-|\@)", RegexOptions.IgnoreCase);
            return regex.Replace(str, "$1<wbr/>");
        }

        public static string getDateOrdinalDay(int day) {
            string ordinal;
            if (day>3 && day<21) ordinal = "th";
            else {
                switch (day%10) {
                    case 1:  ordinal = "st"; break;
                    case 2:  ordinal = "nd"; break;
                    case 3:  ordinal = "rd"; break;
                    default: ordinal = "th"; break;
                }
            }
            return day+ordinal;
        }

        public static string GetDateOrdinalMonthDay(int month, int day) {
            return GetDateMonthNameFromNum(month)+" "+getDateOrdinalDay(day);
        }

        public static string GetDateMonthNameFromNum(int month) {
            string[] months = new string[] {"January", "February", "March","April", "May", "June", "July", "August", "September", "October", "November", "December"};
            return months.GetValue(month-1).ToString();
        }
    }
}
