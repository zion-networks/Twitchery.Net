using System.Net;
using Newtonsoft.Json;

namespace TwitcheryNet.Http;

public static class AsyncHttpClient
{
    public static Action<HttpResponseMessage, string>? OnResponse { get; set; }
    
    public static async Task<(HttpStatusCode, string?)?> GetAsync(string uri, CancellationToken cancellationToken = default)
    {
        return await GetAsync(new Uri(uri), cancellationToken);
    }
    
    public static async Task<(HttpStatusCode, string?)?> GetAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync(uri, cancellationToken);
        
        var reponseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        OnResponse?.Invoke(response, reponseBody);

        return (
            response.StatusCode,
            reponseBody
        );
    }
    
    public static async Task<(HttpStatusCode, string?)?> GetAsync(HttpRequestBuilder builder, CancellationToken cancellationToken = default)
    {
        var request = builder.Request;
        
        if (request is null)
            return null;
        
        using var client = new HttpClient();
        using var response = await client.SendAsync(request, cancellationToken);
        
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        OnResponse?.Invoke(response, responseBody);

        return (
            response.StatusCode,
            responseBody
        );
    }

    public static async Task<(HttpStatusCode, T?)?> GetAsync<T>(string uri, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = await GetAsync(uri, cancellationToken);

        if (response?.Item2 is null)
            return null;

        return (
            response.Value.Item1,
            JsonConvert.DeserializeObject<T>(response.Value.Item2)
        );
    }
    
    public static async Task<(HttpStatusCode, T?)?> GetAsync<T>(Uri uri, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = await GetAsync(uri, cancellationToken);

        if (response?.Item2 is null)
            return null;

        return (
            response.Value.Item1,
            JsonConvert.DeserializeObject<T>(response.Value.Item2)
        );
    }
    
    public static async Task<(HttpStatusCode, T?)?> GetAsync<T>(HttpRequestBuilder builder, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = await GetAsync(builder, cancellationToken);

        if (response?.Item2 is null)
            return null;

        return (
            response.Value.Item1,
            JsonConvert.DeserializeObject<T>(response.Value.Item2)
        );
    }
    
    public static HttpRequestBuilder StartGet(string baseUri)
    {
        return StartGet(new Uri(baseUri));
    }
    
    public static HttpRequestBuilder StartGet(Uri baseUri)
    {
        return new HttpRequestBuilder(HttpMethod.Get, baseUri);
    }
    
    public static async Task<(HttpStatusCode, string?)?> PostAsync(string uri, HttpContent content, CancellationToken cancellationToken = default)
    {
        return await PostAsync(new Uri(uri), content, cancellationToken);
    }
    
    public static async Task<(HttpStatusCode, string?)?> PostAsync(Uri uri, HttpContent content, CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();
        using var response = await client.PostAsync(uri, content, cancellationToken);
        
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        OnResponse?.Invoke(response, responseBody);

        return (
            response.StatusCode,
            responseBody
        );
    }
    
    public static async Task<(HttpStatusCode, string?)?> PostAsync(HttpRequestBuilder builder, CancellationToken cancellationToken = default)
    {
        var request = builder.Request;
        
        if (request is null)
            return null;
        
        using var client = new HttpClient();
        using var response = await client.SendAsync(request, cancellationToken);
        
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        OnResponse?.Invoke(response, responseBody);

        return (
            response.StatusCode,
            responseBody
        );
    }
    
    public static async Task<(HttpStatusCode,  T?)?> PostAsync<T>(string uri, HttpContent content, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = await PostAsync(uri, content, cancellationToken);

        if (response?.Item2 is null)
            return null;

        return (
            response.Value.Item1,
            JsonConvert.DeserializeObject<T>(response.Value.Item2)
        );
    }
    
    public static async Task<(HttpStatusCode, T?)?> PostAsync<T>(Uri uri, HttpContent content, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = await PostAsync(uri, content, cancellationToken);

        if (response?.Item2 is null)
            return null;

        return (
            response.Value.Item1,
            JsonConvert.DeserializeObject<T>(response.Value.Item2)
        );
    }
    
    public static async Task<(HttpStatusCode, T?)?> PostAsync<T>(HttpRequestBuilder builder, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = await PostAsync(builder, cancellationToken);

        if (response?.Item2 is null)
            return null;

        return (
            response.Value.Item1,
            JsonConvert.DeserializeObject<T>(response.Value.Item2)
        );
    }
    
    public static HttpRequestBuilder StartPost(string baseUri)
    {
        return StartPost(new Uri(baseUri));
    }
    
    public static HttpRequestBuilder StartPost(Uri baseUri)
    {
        return new HttpRequestBuilder(HttpMethod.Post, baseUri);
    }
}