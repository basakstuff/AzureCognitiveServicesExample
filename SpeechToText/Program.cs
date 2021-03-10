using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.IO;
using System.Threading.Tasks;


namespace SpeechToText
{
    class Program
    {
        // Official document address：https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/quickstart-csharp-dotnetcore-windows

        // subscription key
        static string apiKey = "";
        // Region
        static string region = "westeurope";
        // Language
        static string speechRecognitionLanguage = "tr-tr";

        // Audio stream formatting configuration
        static AudioStreamFormat AudioStreamFormat;

        static void Main()
        {
            //// Real-time short speech recognition
            ////RecognizeSortSpeechAsync().Wait();

            //// Long speech real-time recognition
            //RecognizeLongSpeechAsync().Wait();

            // Recognize from audio file
            var filePath = @"voice3.wav";
            //filePath = @"abcdh.wav";
            //FormFile(filePath).Wait();

            // Recognize from audio file stream
            /* Audio formatting configuration
             * This configuration cannot be changed, the official currently only supports this standard
             * Official document link:：https://docs.microsoft.com/tr-tr/azure/cognitive-services/speech-service/how-to-use-audio-input-streams
             * */
            if (AudioStreamFormat == null)
            {
                byte channels = 2;
                byte bitsPerSample = 16;
                uint samplesPerSecond = 16000;
                AudioStreamFormat = AudioStreamFormat.GetWaveFormatPCM(samplesPerSecond, bitsPerSample, channels);
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            FormStream(fileStream).Wait();


            Console.WriteLine("Press any key to end the program..");
            Console.ReadKey();
        }

        /// <summary>
        /// Short speech recognition (real time)
        /// </summary>
        /// <returns></returns>
        public static async Task RecognizeSortSpeechAsync()
        {
            var config = SpeechConfig.FromSubscription(apiKey, region);
            config.SpeechRecognitionLanguage = speechRecognitionLanguage; // language settings

            // Create analyzer
            using (var recognizer = new SpeechRecognizer(config))
            {

                Console.WriteLine("Say something...");

                /*
                 * RecognizeOnceAsync() returns after the first voice is recognized, so it is only suitable for single recognition such as commands or queries,
                 * For long-running identification, use StartContinuousRecognitionAsync() instead.
                 */
                var result = await recognizer.RecognizeOnceAsync();

                // test result
                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"Recognition result: {result.Text}");
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"Recognition failed: the language cannot be recognized.");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    Console.WriteLine($"Recognition cancelled: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"Recognition has been cancelled: Error code ={cancellation.ErrorCode}");
                        Console.WriteLine($"Recognition cancelled: Error details ={cancellation.ErrorDetails}");
                        Console.WriteLine($"Recognition cancelled: Please check if the subscription is normal");
                    }
                }
            }
        }


        public static async Task RecognizeLongSpeechAsync()
        {
            var config = SpeechConfig.FromSubscription(apiKey, region);
            config.SpeechRecognitionLanguage = speechRecognitionLanguage; 

            var stopRecognition = new TaskCompletionSource<int>();

          
            using (var recognizer = new SpeechRecognizer(config))
            {

                Console.WriteLine("Say something...");

               
                recognizer.Recognizing += (s, e) =>
                {
                };

                recognizer.Recognized += (s, e) =>
                {
                    if (e.Result.Reason == ResultReason.RecognizedSpeech)
                    {
                        Console.WriteLine(e.Result.Text);

                        if (e.Result.Text == "Stop recognition")
                        {
                            stopRecognition.TrySetResult(0);
                        }
                    }
                    else if (e.Result.Reason == ResultReason.NoMatch)
                    {
                        Console.WriteLine($"Did not match: ");
                    }
                };

                recognizer.Canceled += (s, e) =>
                {
                    Console.WriteLine($"Cancel identification: Reason ={e.Reason}");

                    if (e.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"Cancel recognition: Error code={e.ErrorCode}");
                        Console.WriteLine($"ancel Recognition: Error Details={e.ErrorDetails}");
                        Console.WriteLine($"ancel identification: Please check your subscription information");
                    }

                    stopRecognition.TrySetResult(0);
                };

                recognizer.SessionStarted += (s, e) =>
                {
                    Console.WriteLine("\n    Recognition begins.");
                };

                recognizer.SessionStopped += (s, e) =>
                {
                    Console.WriteLine("\n    End of recognition.");
                };

                // Start continuous recognition. Use stopcontinuousrecognition() to stop recognition.
                await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                // Wait for completion
                Task.WaitAny(new[] { stopRecognition.Task });
            }
        }


        /// <summary>
        /// Read audio from file, only test wav file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task FormFile(string filePath)
        {
            using (var audioConfig = AudioConfig.FromWavFileInput(filePath)) 
            {
                await SpeechRecognizer(audioConfig);
            }
        }


        /// <summary>
        /// Identify from the audio stream
        /// 
        /// Only specific audio is currently supported, check for details:
        /// https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/how-to-use-audio-input-streams
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task FormStream(Stream stream)
        {
            using (var audioConfig = AudioConfig.FromStreamInput(new ReadPCMStream(stream), AudioStreamFormat)) // 从文件读取
            {
                await SpeechRecognizer(audioConfig);
            }
        }


        // 
        private static async Task SpeechRecognizer(AudioConfig audioConfig)
        {
            var config = SpeechConfig.FromSubscription(apiKey, region);
            config.SpeechRecognitionLanguage = speechRecognitionLanguage;

            var stopRecognition = new TaskCompletionSource<int>();

            using (var recognizer = new SpeechRecognizer(config, audioConfig))
            {
                recognizer.Recognizing += (s, e) =>
                {
                    //Console.WriteLine(e.Result.Text);
                };

                recognizer.Recognized += (s, e) =>
                {
                    if (e.Result.Reason == ResultReason.RecognizedSpeech)
                    {
                        Console.WriteLine(e.Result.Text);
                    }
                    else if (e.Result.Reason == ResultReason.NoMatch)
                    {
                        Console.WriteLine($"Data not recognized: ");
                    }
                };

                recognizer.Canceled += (s, e) =>
                {
                    Console.WriteLine($"Recognition cancellation: Reason ={e.Reason}");

                    if (e.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"ecognition cancellation: error code={e.ErrorCode}");
                        Console.WriteLine($"Recognition cancellation: Error Details={e.ErrorDetails}");
                        Console.WriteLine($"Recognition cancellation: Please check if your Azure subscription is updated");
                    }

                    stopRecognition.TrySetResult(0);
                };

                recognizer.SessionStarted += (s, e) =>
                {
                    Console.WriteLine("\n    Start to recognize.");
                };

                recognizer.SessionStopped += (s, e) =>
                {
                    Console.WriteLine("\n    End recognition.");
                    stopRecognition.TrySetResult(0);
                };

          

                await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                Task.WaitAny(new[] { stopRecognition.Task });

                await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
            }
        }


    }

}