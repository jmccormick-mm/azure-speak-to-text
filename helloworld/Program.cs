//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

// <code>
using Microsoft.CognitiveServices.Speech;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace helloworld
{
    class Program
    {
        public static async Task RecognizeSpeechAsync()
        {
            try
            {

                string textFile = "test.txt";
                var config = SpeechConfig.FromSubscription("7424749d204a427fa279184390a92787", "eastus");
                var speechRecognizer = new SpeechRecognizer(config);
                var stopRecognition = new TaskCompletionSource<int>();

                FileStream fileStream = File.OpenWrite(textFile);
                StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);


                speechRecognizer.Recognized += (s, e) =>
                {
                    switch (e.Result.Reason)
                    {
                        case ResultReason.RecognizedSpeech:
                            streamWriter.WriteLine(e.Result.Text);
                            break;
                        case ResultReason.NoMatch:
                            Console.WriteLine("Speech could not be recognized.");
                            break;
                    }
                };

                speechRecognizer.Canceled += (s, e) =>
                {
                    if (e.Reason != CancellationReason.EndOfStream)
                    {
                        Console.WriteLine("Speech recognition canceled.");
                    }
                    stopRecognition.TrySetResult(0);
                    streamWriter.Close();
                };

                speechRecognizer.SessionStopped += (s, e) =>
                {
                    Console.WriteLine("Speech recognition stopped.");
                    stopRecognition.TrySetResult(0);
                    streamWriter.Close();
                };

                Console.WriteLine("Speech recognition started.");
                await speechRecognizer.StartContinuousRecognitionAsync();
                Task.WaitAny(new[] { stopRecognition.Task });
                await speechRecognizer.StopContinuousRecognitionAsync();


            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static async Task Main()
        {
            await RecognizeSpeechAsync();
            Console.WriteLine("Please press <Return> to continue.");
            Console.ReadLine();
        }
    }
}
// </code>
