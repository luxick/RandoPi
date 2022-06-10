using System.Net;
using System.Text;
using System.Text.Json;
using luxick.Result;

namespace RandoPi.Client;

public class RandoClient
{
    /// <summary>   Options for controlling the serializer. </summary>
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    
    public RandoClient(string serverUrl)
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(serverUrl);
    }

    public async Task<Result<string>> Echo(string message) =>
        await ApiRequest<string, string>("/debug/echo", HttpMethod.Post, message);

    private async Task<Result<TResult>> ApiRequest<TResult, TArgs>(string url, HttpMethod method, TArgs args)
    {
        try
        {
            var request = new HttpRequestMessage(method, url);

            // Serialize the parameter class and add it to the request
            request.Content = new StringContent(JsonSerializer.Serialize(args), Encoding.UTF8, "application/json");

            // perform the http request to the API
            var response = await _httpClient.SendAsync(request);
                
            // The user has no valid login credentials
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return new Error<TResult>("Unauthorized");
            }

            var json = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(json) || json == "{}")
                return new Error<TResult>("Invalid response", "The server returned no or empty data");
                
            // Deserialize the returned object 
            var result = JsonSerializer.Deserialize<Result<TResult>>(json, _serializerOptions);
            return result;
        }
        catch (Exception e)
        {
            return new Error<TResult>(e);
        }
    }
}