using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ComputerVisionOCR
{
    public static class AzureOcrHelper
    {
        /// <summary>
        /// Recognize text from url picture
        /// </summary>
        /// <param name="client"></param>
        /// <param name="baseUrl"></param>
        /// <param name="apiKey"></param>
        /// <param name="imageUrl"></param>
        /// <param name="language"></param>
        /// <param name="detectOrientation"></param>
        /// <returns></returns>
        public static async Task<string> OcrFromUrl(HttpClient client, string baseUrl, string apiKey, string imageUrl, string language = "unk", bool detectOrientation = true)
        {
            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
            var url = $"{baseUrl}?language={language}&detectOrientation={detectOrientation.ToString()}";


            HttpResponseMessage response;

            //var byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { url = imageUrl }));
            var dataStr = JsonConvert.SerializeObject(new { url = imageUrl });

            using (StringContent content = new StringContent(dataStr))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                // 
                response = await client.PostAsync(url, content);
            }

            // Asynchronously get the JSON response.
            var result = await response.Content.ReadAsStringAsync();

            client.DefaultRequestHeaders.Remove("Ocp-Apim-Subscription-Key");
            return result;

        }

        /// <summary>
        /// Recognize text from local pictures
        /// </summary>
        /// <param name="client"></param>
        /// <param name="baseUrl"></param>
        /// <param name="apiKey"></param>
        /// <param name="imageFilePath"></param>
        /// <param name="language"></param>
        /// <param name="detectOrientation"></param>
        /// <returns></returns>
        public static async Task<string> OcrFromFile(HttpClient client, string baseUrl, string apiKey, string imageFilePath, string language = "unk", bool detectOrientation = true)
        {
            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
            var url = $"{baseUrl}?language={language}&detectOrientation={detectOrientation.ToString()}";


            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                // 
                response = await client.PostAsync(url, content);
            }

            // Asynchronously get the JSON response.
            var result = await response.Content.ReadAsStringAsync();

            client.DefaultRequestHeaders.Remove("Ocp-Apim-Subscription-Key");


            return result;
        }


        /// <summary>
        /// Convert the local picture into a byte array
        /// </summary>
        /// <param name="imageFilePath">Picture path</param>
        /// <returns></returns>
        public static byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (var fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                var binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }

        /// <summary>
        /// Simply convert the Ocr recognition result Json string into a text paragraph
        /// </summary>
        /// <param name="ocrJsonResult"></param>
        /// <returns></returns>
        public static string SimpleFormattedText(string ocrJsonResult)
        {
            var objResult = JsonConvert.DeserializeObject<dynamic>(ocrJsonResult);

            if (objResult.regions == null)
            {
                return string.Empty;
            }


            var sb = new StringBuilder();
            foreach (var item in objResult.regions)
            {
                foreach (var item2 in item.lines)
                {
                    foreach (var item3 in item2.words)
                    {
                        sb.Append(item3.text);
                    }

                    sb.Append("\r\n");
                }
            }

            return sb.ToString();
        }
    }
}
