using System;
using System.Collections.Generic;
using System.Text;
using Windows.Web.Http;
using System.IO;

namespace PublicTransportSofia
{
    public class Test
    {
        public string ImageSource { get; set; }


        public Test(string imageSource)
        {
            this.ImageSource = imageSource;
        }

        public Test()
            : this("Images/test.jpg")
        {

            //this.getImage();
        }

        private async void getImage() 
        {
            var uri = new Uri("http://m.sofiatraffic.bg/captcha/2e84ce45132bbd3dc188793bf1727d39");
            //var uri = new Uri("http://m.sofiatraffic.bg/vt");
            var httpClient = new HttpClient();
          //  httpClient.DefaultRequestHeaders.
           // InputStream result;
            string exception;
            // Always catch network exceptions for async methods
       //     try
       //     {
            var result = await httpClient.GetBufferAsync(uri);//GetStringAsync(uri);
          //  this.ImageSource = result;
         //   }
        //    catch(Exception ex)
         //   {
             //   exception = "Message: " +  ex.Message + "Data: " + ex.Data.ToString() + "Type: " + ex.GetType();
                // Details in ex.Message and ex.HResult.       
          //  }

            // Once your app is done using the HttpClient object call dispose to 
            // free up system resources (the underlying socket and memory used for the object)
            httpClient.Dispose();
            var debug = 1;
        }
    }
}
