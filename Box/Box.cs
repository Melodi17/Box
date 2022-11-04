using System;
using System.IO;
using System.Net;
using Spectre.Console;

namespace Box
{
    public class Box
    {
        public static bool IsAvailable(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 1000 * 5;
            request.Method = "HEAD";
            try
            {
                using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static void Download(string url, string localPath)
        {
            ProgressBarColumn barColumn = new();
            barColumn.RemainingStyle = new(Color.Grey, Color.Grey);
            barColumn.CompletedStyle = new(Color.Orange1, Color.Orange1);
            barColumn.FinishedStyle = new(Color.Green, Color.Grey);
            barColumn.IndeterminateStyle = new(Color.Blue, Color.Blue);

            var col = new ProgressColumn[]
                {
                    new TaskDescriptionColumn(),    // Task description
                    barColumn,                      // Progress bar
                    new PercentageColumn(),         // Percentage
                    new RemainingTimeColumn(),      // Remaining time
                    new SpinnerColumn()             // Spinner
                };

            AnsiConsole.Progress()
                .AutoRefresh(true) // Turn on auto refresh
                .AutoClear(true)   // Do not remove the task list when done
                .HideCompleted(false)   // Hide tasks as they are completed
                .Columns(col)
                .Start(ctx =>
                {
                    var task = ctx.AddTask($"Downloading {Path.GetFileName(localPath)}");

                    WebClient client = new();
                    client.DownloadProgressChanged += (_, o) =>
                    {
                        task.MaxValue = (double)(o.TotalBytesToReceive) / (double)(1024 * 1024);
                        task.Value = (double)(o.BytesReceived) / (double)(1024 * 1024);
                    };
                    client.DownloadFileCompleted += (_, o) =>
                    {
                        task.MaxValue = 1;
                        task.Value = task.MaxValue;
                    };

                    try
                    {
                        client.DownloadFileAsync(new(url), localPath);
                    }
                    catch (Exception)
                    {
                        task.MaxValue = 1;
                        task.Value = task.MaxValue;
                    }

                    while (!ctx.IsFinished) { }
                });
        }
    }
}