using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Web.Http;
using Windows.Storage;
using Windows.Web.Http.Headers;
using HtmlAgilityPack;

using PublicTransportSofia.Utils;

namespace PublicTransportSofia
{
    public class HttpRequester
    {
        private static HttpRequester instance;
        private HttpClient client;
        private ApplicationDataContainer localSettings;

        private HttpRequester()
        {
            this.client = new HttpClient();
            this.localSettings = ApplicationData.Current.LocalSettings;
        }

        public static HttpRequester Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new HttpRequester();
                }

                return instance;
            }
        }

        private void SaveCookiesToLocalSettings(HttpResponseHeaderCollection headers)
        {
            var responseHeadersKeys = headers.Keys.ToList();
            var responseHeadersValues = headers.Values.ToList();
            var indexOfCoookies = responseHeadersKeys.IndexOf(StringResources.SetCookie);
            var debug = 7;
            var cookies = responseHeadersValues[indexOfCoookies];
            string pattern = " .*, *";//to replace " path=/, ";
            string replacement = " ";
            Regex regex = new Regex(pattern);
            string newCookies = regex.Replace(cookies, replacement);
            var currentSavedCookies = this.LoadCookiesFromLocalSettings();
            var newCookiesToBeSaved = "";
            if (string.IsNullOrEmpty(currentSavedCookies))
            {
                currentSavedCookies = newCookies;
            }
            else
            {
                var newCookiesList = newCookies.Split(' ');
                foreach (var newCookie in newCookiesList)
                {
                    pattern = ".*=";
                    regex = new Regex(pattern, RegexOptions.IgnoreCase);
                    Match matches = regex.Match(newCookie);
                    string cookieName = "exampleText";
                    while(matches.Success)
                    {
                        cookieName = matches.Value;
                        matches = matches.NextMatch();
                    }

                    if(currentSavedCookies.IndexOf(cookieName) == -1)
                    {
                        currentSavedCookies += "; " + newCookie;
                    }
                    else
                    {
                        pattern = cookieName + @"\w+";
                        regex = new Regex(pattern);
                        currentSavedCookies = regex.Replace(currentSavedCookies, newCookie);
                    }
                }
            }

            this.localSettings.Values[StringResources.CookieNameInLocalSettings] = currentSavedCookies;
            var again = this.LoadCookiesFromLocalSettings();
            //var cookies = responseHeadersKeys.Zip(responseHeadersValues, (k, v) => new { k, v })
            //    .Where(x => x.k == "Set-Cookie").Select(x => x.v).FirstOrDefault();

            // "PHPSESSID=je4ekdi3ak5842peto4rv3pn62; vjfmrii=89ea942e251ce5928a442d2c8f053d5c; alpocjengi=d98c07b1c96658a22074ec291e3163628c716843"

            var d = 1;
        }

        public string LoadCookiesFromLocalSettings()
        {
            var cookiesFromLocalSettings = "";
           // ApplicationDataCompositeValue cookies = (ApplicationDataCompositeValue) this.localSettings.Values[StringResources.CookieNameInLocalSettings];
           object cookies = localSettings.Values[StringResources.CookieNameInLocalSettings];
            if(cookies != null)
            {
                cookiesFromLocalSettings = cookies.ToString();
            }

            return cookiesFromLocalSettings;
        }

        public async Task<string> GetVirtualTablesMainPage()
        {
            var uri = new Uri(StringResources.VirtualTablesBaseURL);
          //  var httpClient = new HttpClient();
          //  this.localSettings.Values.Remove(StringResources.)
            this.client.DefaultRequestHeaders.Clear();
            this.SetDefaultHeaders();
            HttpResponseMessage aResponse = await this.client.GetAsync(uri);//PostAsync(new Uri("http://m.sofiatraffic.bg/vt"), theContent);
           // var responseHeadersKeys =  aResponse.
            var virtualTablesMainPage = await aResponse.Content.ReadAsStringAsync();//await this.client.GetStringAsync(uri);
            var headers = aResponse.Headers;
            SaveCookiesToLocalSettings(headers);
            //var virtualTablesMainPage = await this.client.GetStringAsync(uri);
            var captchaURL = "";
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();

            // There are various options, set as needed
            htmlDoc.OptionFixNestedTags = true;

            // filePath is a path to a file containing the html
            htmlDoc.LoadHtml(virtualTablesMainPage);

            // Use:  htmlDoc.LoadHtml(xmlString);  to load from a string (was htmlDoc.LoadXML(xmlString)

            // ParseErrors is an ArrayList containing any errors from the Load statement
            if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0)
            {
                // Handle any parse errors as required

            }
            else
            {
                if (htmlDoc.DocumentNode != null)
                {
                    var bodyNode = htmlDoc.DocumentNode.Descendants("img").Where(d =>
    d.Attributes.Contains("src") && d.Attributes["src"].Value.Contains("captcha")).FirstOrDefault();
                    captchaURL = bodyNode.Attributes.Where(a => a.Name == "src").Select(a => a.Value).FirstOrDefault();//Element("//body");//SelectSingleNode("//body");
                   // HtmlNode footer = htmlDoc.DocumentNode.Descendants().SingleOrDefault(x => x.c == "footer"); 
                    if (bodyNode != null)
                    {
                        // Do something with bodyNode
                    }
                }
            }
            var debug = 2;
          //  httpClient.Dispose();
            return  StringResources.SofiaTrafficBaseURL + captchaURL;
        }

        public void SetDefaultHeaders()
        {
            var headers = this.client.DefaultRequestHeaders;
            headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            headers.AcceptEncoding.ParseAdd("gzip, deflate");
            headers.AcceptLanguage.ParseAdd("bg-BG,bg;q=0.8,en;q=0.6,sq;q=0.4");
            headers.Add("Host", "m.sofiatraffic.bg");
            headers.Add("Proxy-Connection", "keep-alive");
            headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.65 Safari/537.36");
            var cookies = this.LoadCookiesFromLocalSettings();
            if (!string.IsNullOrEmpty(cookies))
            {
                headers.Cookie.ParseAdd(cookies);
            }
        }

        public void SetPostHeaders()
        {
            var headers = this.client.DefaultRequestHeaders;
            headers.CacheControl.ParseAdd("max-age=0");
            headers.Add("Origin", "http://m.sofiatraffic.bg");
            headers.TryAppendWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            headers.Add("Referer", "http://m.sofiatraffic.bg/vt");
        }

        public async void GetSchedulesForStation(string stationCode)
        {
            //var body = String.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=notify.windows.com", MyCredentials, MyCredentials2);

            var body = String.Format("stopCode=2326&o=1&sec=5&0432c17d1f9285738465d347d4fc76f3=cf69fb71725ebf137b903a2da6af719f&go=1&submit=Провери");
            HttpStringContent theContent = new HttpStringContent(body, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded");
            this.client.DefaultRequestHeaders.Clear();
            this.SetDefaultHeaders();
            this.SetPostHeaders();
            HttpResponseMessage aResponse = await client.PostAsync(new Uri("http://m.sofiatraffic.bg/vt"), theContent);
          
            //var content = aResponse.Content;
            //var con = await content.ReadAsStringAsync();
            //var headers = aResponse.Headers;
            //StringBuilder sb = new StringBuilder();
            //foreach (var header in responseHeaders)
            //{
            //    var h = String.Format("Header: {0} ", header);
            //    sb.Append(h + "\n");
            //}

            //var res = sb.ToString();
          //  var uri = new Uri("http://m.sofiatraffic.bg/vt");
          //  var httpClient = new HttpClient();
          ////  httpClient.DefaultRequestHeaders.
          //  var userAgnet = httpClient.DefaultRequestHeaders.UserAgent;
          //  string result;
          //  // Always catch network exceptions for async methods
          //  try
          //  {
          //      result = await httpClient.GetStringAsync(uri);
          //  }
          //  catch
          //  {
          //      // Details in ex.Message and ex.HResult.       
          //  }

          //  // Once your app is done using the HttpClient object call dispose to 
          //  // free up system resources (the underlying socket and memory used for the object)
          //  httpClient.Dispose();
            var debug = 1;
        }
    }
}
