using System;

public static void Run(Stream outputBlob, string myQueueItem, ILogger log)
{
    //myQueueItem = "https://www.nasa.gov/sites/default/files/thumbnails/image/cf-1280.jpg";
    log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
    byte[] imageData = null;
    using (var wc = new System.Net.WebClient())
    {
        imageData = wc.DownloadData(myQueueItem);

    }

}