using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using RandoPi.Shared;

namespace RandoPi.Desktop.Providers.Sources;

public class FoxSource : IImageSource
{
    public ImageMode Mode => ImageMode.Foxes;
    
    private const string Url = "https://randomfox.ca/floof/";

    private readonly HttpClient _httpClient;
    
    public FoxSource()
    {
        _httpClient = new HttpClient();
    }

    public byte[] Get()
    {
        var result = Array.Empty<byte>();
        var task = Task.Run(async () =>
        {
            var resp = await _httpClient.GetAsync(Url);
            var data = await resp.Content.ReadAsStringAsync();
            var json = JsonNode.Parse(data);
            var imageLink = json["image"].ToString();
            
            resp = await _httpClient.GetAsync(imageLink);
            result = await resp.Content.ReadAsByteArrayAsync();
        });
        task.Wait();
        return result;
    }
}