using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


namespace ComputerVisionImgAnalyze
{
    // Document address：https://docs.microsoft.com/en-us/azure/cognitive-services/computer-vision/

  
    class Program
    {
        // subscriptionKey = "0123456789abcdef0123456789ABCDEF"
        private const string apiKey = "";

        // localImagePath = @"C:\Documents\LocalImage.jpg"
        private const string localImagePath = @"imgs/2.jpg";

        private const string remoteImageUrl = "https://i.sozcu.com.tr/wp-content/uploads/2019/11/12/iecrop/shutterstock_758018137_16_9_1573564118-880x495.jpg";

        // Specify the characteristics to be returned
        private static readonly List<VisualFeatureTypes> features =
            new List<VisualFeatureTypes>()
        {
            VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
            VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
            VisualFeatureTypes.Tags
        };

        static void Main(string[] args)
        {
            var computerVision = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(apiKey),
                new System.Net.Http.DelegatingHandler[] { });

            // Specify Azure region
            computerVision.Endpoint = "https://westeurope.api.cognitive.microsoft.com";

            Console.WriteLine("Analyzing the image ...");

            // Remote picture
            AnalyzeRemoteAsync(computerVision, remoteImageUrl).GetAwaiter().GetResult();

            // Local picture
            //AnalyzeLocalAsync(computerVision, localImagePath).GetAwaiter().GetResult();


            Console.WriteLine("press any key to exit...");
            Console.ReadLine();
        }

        // Analyze remote images
        private static async Task AnalyzeRemoteAsync(
            ComputerVisionClient computerVision, string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                Console.WriteLine("\nInvalid image link:\n{0} \n", imageUrl);
                return;
            }

            ImageAnalysis analysis = await computerVision.AnalyzeImageAsync(imageUrl, features);

            DisplayResults(analysis, imageUrl);
            DisplayImgTag(analysis);
        }

        // Analyze local images
        private static async Task AnalyzeLocalAsync(
            ComputerVisionClient computerVision, string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                Console.WriteLine("\nUnable to open or read the local image path:\n{0} \n", imagePath);
                return;
            }

            using (Stream imageStream = File.OpenRead(imagePath))
            {
                ImageAnalysis analysis = await computerVision.AnalyzeImageInStreamAsync(
                    imageStream, features);

                DisplayResults(analysis, imagePath);
                DisplayImgTag(analysis);
            }
        }

        // Display the most relevant titles of the images
        private static void DisplayResults(ImageAnalysis analysis, string imageUri)
        {
           // Console.WriteLine("\r\n\r\n{0}", imageUri);
            foreach (var caption in analysis.Description.Captions)
            {
                Console.WriteLine("\r\n{0}\r\n", caption.Text);
            }
           
        }

        // Display label
        private static void DisplayImgTag(ImageAnalysis analysis)
        {
            foreach (var tag in analysis.Tags)
            {
                Console.WriteLine("Label name:{0}       {1}%", tag.Name, Math.Round(tag.Confidence * 100, 2));
            }
        }
    }
}
