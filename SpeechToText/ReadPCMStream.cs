using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SpeechToText
{
    public class ReadPCMStream : PullAudioInputStreamCallback
    {
        private Stream stream;
        private long streamLength = 0;
        private long nextStartPostion = 0;
        // Whether to automatically release the stream
        private bool autoDispose;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Audio stream</param>
        /// <param name="autoDispose">Automatically release audio stream，The default is true</param>
        public ReadPCMStream(Stream stream, bool autoDispose = true)
        {
            this.stream = stream;
            this.streamLength = stream.Length;
            this.autoDispose = autoDispose;
        }

        //public ReadPCMStream(string filePath, bool autoDispose = true)
        //{

        //}


        public override int Read(byte[] dataBuffer, uint size)
        {
            // Have finished reading, return 0
            if (nextStartPostion >= streamLength)
            {
                this.Close();
                return 0; 
            }

            // The read data minus the unread data indicates the remaining data length
            var remaining = streamLength - nextStartPostion;
            // The amount of data that can be read this time is equal to the remaining data length by default
            var readLength = (int)remaining;
            // If the remaining amount is greater than the length of the byte array,
            // then the amount read this time is the length of the array
            if (remaining > size)
            {
                readLength = (int)size;
            }


            // Set the start position of the stream
            stream.Seek(nextStartPostion, SeekOrigin.Begin);
            // Fill stream data into byte array
            stream.Read(dataBuffer, 0, readLength);
            // Cumulative total read amount
            nextStartPostion += readLength;

            

            return readLength;
        }


        public override void Close()
        {
            if (this.autoDispose)
            {
                stream?.Dispose();
            }

            base.Close();
        }
    }
}
