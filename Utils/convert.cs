using System;
using System.Diagnostics;
using System.IO;

namespace AutoVideoConverter.Utils
{
    public class Converter
    {

        private static Converter Instancia { get; set; }

        public static Converter GetInstance()
        {
            if (Instancia == null)
            {
                Instancia = new Converter();
            }
            return Instancia;
        }
        public void ConvertToMp4WithSubtitles(string mkvFilePath)
        {
            // Extract subtitles from MKV
            string subtitlesFilePath = ExtractSubtitles(mkvFilePath);

            // Convert MKV to MP4
            ConvertToMp4(mkvFilePath, subtitlesFilePath);
        }

        private string ExtractSubtitles(string mkvFilePath)
        {
            // Create a temporary directory to store the extracted subtitles
            string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            // Set the output file path for the extracted subtitles
            string subtitlesFilePath = Path.Combine(tempDir, "subtitles.srt");

            // Use FFmpeg to extract the subtitles from the MKV file
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i \"{mkvFilePath}\" -map 0:s:0 \"{subtitlesFilePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new Exception("Failed to extract subtitles from MKV file.");
                }
            }

            return subtitlesFilePath;
        }

        private async void ConvertToMp4(string mkvFilePath, string subtitlesFilePath)
        {
            // Set the output file path for the MP4 file
            string mp4FilePath = Path.ChangeExtension(mkvFilePath, ".mp4");

            // Use FFmpeg to convert the MKV file to MP4
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i \"{mkvFilePath}\" -i \"{subtitlesFilePath}\" -c copy -c:s mov_text \"{mp4FilePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new Exception("Failed to convert MKV file to MP4.");
                }
                else if (process.ExitCode == 0)
                {
                    // Vamos a enviar una api al servicion de ntfs
                    // para que marque el archivo como "listo"

                    var name = Path.GetFileName(mkvFilePath);
                    // Vamos ha hacer una peticion api a https://antonio-ntfs.duckdns.org
                    // Va a tener este formato
                    // Create the HTTP client
                    using (HttpClient client = new HttpClient())
                    {
                        // Set the API endpoint URL
                        string apiUrl = "https://antonio-ntfs.duckdns.org";

                        // Create the request body
                        string requestBody = "File: " + name + " has been converted to MP4";

                        // Create the request headers
                        client.DefaultRequestHeaders.Add("Title", "File conversion completed");
                        client.DefaultRequestHeaders.Add("low", "urgent");


                        // Send the POST request
                        HttpResponseMessage response = await client.PostAsync(apiUrl, new StringContent(requestBody));

                        // Check if the request was successful
                        if (response.IsSuccessStatusCode)
                        {
                            // Request was successful
                            Console.WriteLine("API request sent successfully.");
                        }
                        else
                        {
                            // Request failed
                            Console.WriteLine("Failed to send API request.");
                        }
                    }
                }
            }
        }
    }
}