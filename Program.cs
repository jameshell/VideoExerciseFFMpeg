using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Exceptions;

namespace ConcatenateVideos
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var videoDirectoryPath = @"F:\test";
            var video1Path = Path.Combine(videoDirectoryPath, "video1.mp4");
            var video2Path = Path.Combine(videoDirectoryPath, "video2.mp4");
            var outputVideoPath = Path.Combine(videoDirectoryPath, "output.mp4");

            var videoList = new List<string> { video1Path, video2Path };

            var conversion = FFmpeg.Conversions.New();

            foreach (var videoPath in videoList)
            {
                var mediaInfo = await FFmpeg.GetMediaInfo(videoPath);
                var videoStream = mediaInfo.VideoStreams.FirstOrDefault();
                var audioStream = mediaInfo.AudioStreams.FirstOrDefault();

                if (videoStream != null)
                {
                    conversion.AddStream(videoStream);
                }
                if (audioStream != null)
                {
                    conversion.AddStream(audioStream);
                }
                //conversion.AddStream(videoStream).AddStream(audioStream);
            }

            conversion.OnProgress += (sender, args) =>
            {
                Console.WriteLine($"Concatenating videos... {args.Duration} / {args.TotalLength}");
            };

            try
            {
                await conversion.Start(outputVideoPath);

                Console.WriteLine($"Concatenation completed. Output video saved to {outputVideoPath}");
            }
            catch (ConversionException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
