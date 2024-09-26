using Newtonsoft.Json;

namespace TwitcheryNet.Http;

public static class AsyncHttpClient
{
    public static Action<HttpResponseMessage, string>? OnResponse { get; set; }
    
    public static async Task<AsyncHttpResponse> GetAsync(string uri, CancellationToken cancellationToken = default)
    {
        return await GetAsync(new Uri(uri), cancellationToken);
    }
    
    public static async Task<AsyncHttpResponse> GetAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync(uri, cancellationToken);
        
        var reponseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        OnResponse?.Invoke(response, reponseBody);

        return new AsyncHttpResponse(response, reponseBody);
    }
    
    public static async Task<AsyncHttpResponse> GetAsync(HttpRequestBuilder builder, CancellationToken cancellationToken = default)
    {
        var request = builder.Request ?? throw new ArgumentNullException(nameof(builder.Request));
        using var client = new HttpClient();
        using var response = await client.SendAsync(request, cancellationToken);
        
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        OnResponse?.Invoke(response, responseBody);

        return new AsyncHttpResponse(response, responseBody);
    }

    public static async Task<AsyncHttpResponse<T>> GetAsync<T>(string uri, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await GetAsync(uri, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
            return response;

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);
        
        return response;
    }
    
    public static async Task<AsyncHttpResponse<T>> GetAsync<T>(Uri uri, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await GetAsync(uri, cancellationToken));
        
        if (string.IsNullOrWhiteSpace(response.RawBody))
            return response;
        
        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);
        
        return response;
    }
    
    public static async Task<AsyncHttpResponse<T>> GetAsync<T>(HttpRequestBuilder builder, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await GetAsync(builder, cancellationToken));
        
        if (string.IsNullOrWhiteSpace(response.RawBody))
            return response;
        
        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);
        
        return response;
    }
    
    public static HttpRequestBuilder StartGet(string baseUri)
    {
        return StartGet(new Uri(baseUri));
    }
    
    public static HttpRequestBuilder StartGet(Uri baseUri)
    {
        return new HttpRequestBuilder(HttpMethod.Get, baseUri);
    }
    
    public static async Task<AsyncHttpResponse> PostAsync(string uri, HttpContent content, CancellationToken cancellationToken = default)
    {
        return await PostAsync(new Uri(uri), content, cancellationToken);
    }
    
    public static async Task<AsyncHttpResponse> PostAsync(Uri uri, HttpContent content, CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();
        using var response = await client.PostAsync(uri, content, cancellationToken);
        
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        OnResponse?.Invoke(response, responseBody);

        return new AsyncHttpResponse(response, responseBody);
    }
    
    public static async Task<AsyncHttpResponse> PostAsync(HttpRequestBuilder builder, CancellationToken cancellationToken = default)
    {
        var request = builder.Request ?? throw new ArgumentNullException(nameof(builder.Request));
        
        using var client = new HttpClient();
        using var response = await client.SendAsync(request, cancellationToken);
        
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        OnResponse?.Invoke(response, responseBody);

        return new AsyncHttpResponse(response, responseBody);
    }
    
    public static async Task<AsyncHttpResponse<T>> PostAsync<T>(string uri, HttpContent content, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await PostAsync(uri, content, cancellationToken));
        
        if (string.IsNullOrWhiteSpace(response.RawBody))
            return response;
        
        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);
        
        return response;
    }
    
    public static async Task<AsyncHttpResponse<T>> PostAsync<T>(Uri uri, HttpContent content, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await PostAsync(uri, content, cancellationToken));
        
        if (string.IsNullOrWhiteSpace(response.RawBody))
            return response;
        
        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);
        
        return response;
    }
    
    public static async Task<AsyncHttpResponse<T>> PostAsync<T>(HttpRequestBuilder builder, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await PostAsync(builder, cancellationToken));
        
        if (string.IsNullOrWhiteSpace(response.RawBody))
            return response;
        
        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);
        
        return response;
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