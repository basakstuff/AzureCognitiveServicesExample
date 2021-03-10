using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;



namespace ComputerVisionOCR
{

    // Document address：https://docs.microsoft.com/en-us/azure/cognitive-services/computer-vision/


    /// <summary>
    /// OCR image text recognition
    /// </summary>

    static class Program
    {


        // Replace <Subscription Key> with your valid subscription key.
        const string apiKey = "";
        // api address
        const string uriBase = "https://westeurope.api.cognitive.microsoft.com/vision/v2.0/ocr";

        static void Main()
        {

            string imageFilePath = @"imgs/1.jpg";
            string imgUrl = "https://leizhangstorage.blob.core.chinacloudapi.cn/azureblog/ocr.jpg";


            HttpClient client = new HttpClient();
            var result = string.Empty;


            Console.WriteLine("Identify from file:");

            result = AzureOcrHelper.OcrFromFile(client, uriBase, apiKey, imageFilePath, "en").Result;
            Console.WriteLine("\r\nresponse:\n\n{0}\n", JToken.Parse(result).ToString());
            result = AzureOcrHelper.SimpleFormattedText(result);
            Console.WriteLine("\r\nFormat the output result:\n\n{0}\n", result);

            Console.WriteLine("\r\n===================================\r\n");


            //Console.WriteLine("Identify from url:");

            //result = AzureOcrHelper.OcrFromUrl(client, uriBase, apiKey, imgUrl).Result;
            //Console.WriteLine("\r\nresponse:\n\n{0}\n", JToken.Parse(result).ToString());



            //result = AzureOcrHelper.SimpleFormattedText(result);
            //Console.WriteLine("\r\nFormat the output result:\n\n{0}\n", result);



            Console.WriteLine("\nPress any key to exit...");
            Console.ReadLine();
        }
    }
}
