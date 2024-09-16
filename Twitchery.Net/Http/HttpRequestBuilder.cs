using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace TwitcheryNet.Http;

public class HttpRequestBuilder
{
    public HttpMethod Method { get; private set; }
    public Uri Uri { get; private set; }
    public Dictionary<string, string> Headers { get; } = new();
    public Dictionary<string, string> QueryParameters { get; } = new();
    public string? Body { get; set; }
    public HttpStatusCode? RequiredStatusCode { get; set; }
    public HttpRequestMessage? Request { get; private set; }
    
    public HttpRequestBuilder(HttpMethod method, Uri uri)
    {
        Method = method;
        Uri = uri;
    }
    
    public HttpRequestBuilder SetPath(string path)
    {
        Uri = new Uri(Uri, path);
        return this;
    }
    
    public HttpRequestBuilder RequireStatusCode(HttpStatusCode statusCode)
    {
        RequiredStatusCode = statusCode;
        return this;
    }
    
    public HttpRequestBuilder RequireStatusCode(int statusCode)
    {
        RequiredStatusCode = (HttpStatusCode) statusCode;
        return this;
    }
    
    public HttpRequestBuilder AddHeader(string key, string value)
    {
        Headers[key] = value;
        return this;
    }
    
    public HttpRequestBuilder AddHeaderOptional(string? key, string? value)
    {
        if (key is not null && value is not null)
            Headers[key] = value;
        
        return this;
    }
    
    public HttpRequestBuilder AddQueryParameter(string key, string value)
    {
        QueryParameters[key] = value;
        return this;
    }

    public HttpRequestBuilder AddQueryParameterOptional(string? key, string? value)
    {
        if (key is not null && value is not null)
            QueryParameters[key] = value;
        
        return this;
    }
    
    public HttpRequestBuilder SetBody(string body)
    {
        Body = body;
        return this;
    }
    
    public HttpRequestBuilder SetBodyFromJsonObject<T>(T body) where T : class
    {
        Body = JsonConvert.SerializeObject(body);
        return this;
    }
    
    public HttpRequestBuilder Build()
    {
        var request = new HttpRequestMessage(Method, Uri);
        
        foreach (var (key, value) in Headers)
            request.Headers.Add(key, value);
        
        var query = HttpUtility.ParseQueryString(Uri.Query);
        
        foreach (var (key, value) in QueryParameters)
            query[key] = value;
        
        Uri = new UriBuilder(Uri)
        {
            Query = query.ToString()
        }.Uri;
        
        request.RequestUri = Uri;
        
        if (Body is not null)
            request.Content = new StringContent(Body ?? string.Empty, Encoding.UTF8, Headers.GetValueOrDefault("Content-Type", "application/json"));
        
        Request = request;
        
        return this;
    }
    
    public async Task<string?> SendAsync(CancellationToken cancellationToken = default)
    {
        if (Request is null)
            throw new InvalidOperationException("Request is not built.");
        
        switch (Method.Method)
        {
            case "GET":
                var result = await AsyncHttpClient.GetAsync(this, cancellationToken);
                
                if (result is null)
                    return null;
                
                if (RequiredStatusCode is not null && result.Value.Item1 != RequiredStatusCode)
                    throw new HttpRequestException($"Expected status code {RequiredStatusCode} but received {result.Value.Item1}.");

                return result.Value.Item2;
            
            case "POST":
                var postResult = await AsyncHttpClient.PostAsync(this, cancellationToken);
                
                if (postResult is null)
                    return null;
                
                if (RequiredStatusCode is not null && postResult.Value.Item1 != RequiredStatusCode)
                    throw new HttpRequestException($"Expected status code {RequiredStatusCode} but received {postResult.Value.Item1}.");

                return postResult.Value.Item2;
            
            //case HttpMethod.Put:
            //    return await SendPutAsync(cancellationToken);
            
            //case HttpMethod.Delete:
            //    return await SendDeleteAsync(cancellationToken);
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public async Task<T?> SendAsync<T>(CancellationToken cancellationToken = default)
        where T : class
    {
        if (Request is null)
            throw new InvalidOperationException("Request is not built.");
        
        switch (Method.Method)
        {
            case "GET":
                var result = await AsyncHttpClient.GetAsync<T>(this, cancellationToken);
            
                if (result is null)
                    return null;
            
                if (RequiredStatusCode is not null && result.Value.Item1 != RequiredStatusCode)
                    throw new HttpRequestException($"Expected status code {RequiredStatusCode} but received {result.Value.Item1}.");

                return result.Value.Item2;
            
            case "POST":
                var postResult = await AsyncHttpClient.PostAsync<T>(this, cancellationToken);
                
                if (postResult is null)
                    return null;
                
                if (RequiredStatusCode is not null && postResult.Value.Item1 != RequiredStatusCode)
                    throw new HttpRequestException($"Expected status code {RequiredStatusCode} but received {postResult.Value.Item1}.");

                return postResult.Value.Item2;
            
            //case HttpMethod.Put:
            //    return await SendPutAsync(cancellationToken);
            
            //case HttpMethod.Delete:
            //    return await SendDeleteAsync(cancellationToken);
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}