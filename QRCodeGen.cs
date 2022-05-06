using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Net.Codecrete.QrCodeGenerator;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

namespace QRCodeGen
{
    public static class QRCodeGen
    {
        [FunctionName("Form")]
        public static HttpResponseMessage Form(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ILogger log, ExecutionContext context)
        {
            // read in text from a hosted file
            string indexPage = File.ReadAllText(context.FunctionAppDirectory + "/www/index.html");

            // create a new HTTP Response Message
            var result = new HttpResponseMessage(HttpStatusCode.OK);

            // add a text/html header for browsers 
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            
            // get all the data, and add it to content 
            result.Content = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(indexPage));

            // send it 
            return result;
        }

        [FunctionName("GenerateQRCode")]
        public static async Task<IActionResult> GenerateQRCode(
         [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ILogger log)
        {
            // get QR text from query string 
            string qrtext = req.Query["qrtext"];

            // create the QR Code from our text
            var qr = QrCode.EncodeText(qrtext, QrCode.Ecc.Medium);
            // convert it into a byte array for PNG output
            var pngout = qr.ToPng(10, 1, SkiaSharp.SKColors.Black, SkiaSharp.SKColors.White);

            // create a new return object
            var ourResult = new ReturnObject { };

            // store our byte array as a string 
            ourResult.Image = Convert.ToBase64String(pngout);

            // send it as JSON
            return new JsonResult(ourResult);
        }
    }
    public class ReturnObject
    {
        // the only property here is a string for the PNG
        public string Image { get; set; }
    }
}
