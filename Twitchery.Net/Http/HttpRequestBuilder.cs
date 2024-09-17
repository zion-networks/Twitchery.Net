using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using TwitcheryNet.Extensions;
using TwitcheryNet.Models.Helix;

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
    
    public HttpRequestBuilder SetQueryString<TQuery>(TQuery query) where TQuery : class, IQueryParameters
    {
        Uri = new UriBuilder(Uri)
        {
            Query = query.ToQueryString()
        }.Uri;
        
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
        {
            var urlKey = HttpUtility.UrlEncode(key);
            var urlValue = HttpUtility.UrlEncode(value);
            
            query[urlKey] = urlValue;
        }
        
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
    
    public async Task<AsyncHttpResponse> SendAsync(CancellationToken cancellationToken = default)
    {
        if (Request is null)
            throw new InvalidOperationException("Request is not built.");
        
        switch (Method.Method)
        {
            case "GET":
                var getResult = await AsyncHttpClient.GetAsync(this, cancellationToken);

                if (RequiredStatusCode is not null && getResult.Response.StatusCode != RequiredStatusCode)
                    throw new HttpRequestException($"Expected status code {RequiredStatusCode} but received {getResult.Response.StatusCode} with the following body: {getResult.RawBody}");

                return getResult;
            
            case "POST":
                var postResult = await AsyncHttpClient.PostAsync(this, cancellationToken);
                
                if (RequiredStatusCode is not null && postResult.Response.StatusCode != RequiredStatusCode)
                    throw new HttpRequestException($"Expected status code {RequiredStatusCode} but received {postResult.Response.StatusCode} with the following body: {postResult.RawBody}");
                
                return postResult;
            
            //case HttpMethod.Put:
            //    return await SendPutAsync(cancellationToken);
            
            //case HttpMethod.Delete:
            //    return await SendDeleteAsync(cancellationToken);
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public async Task<AsyncHttpResponse<T>> SendAsync<T>(CancellationToken cancellationToken = default)
        where T : class
    {
        if (Request is null)
            throw new InvalidOperationException("Request is not built.");
        
        switch (Method.Method)
        {
            case "GET":
                var result = await AsyncHttpClient.GetAsync<T>(this, cancellationToken);
            
                if (RequiredStatusCode is not null && result.Response.StatusCode != RequiredStatusCode)
                    throw new HttpRequestException($"Expected status code {RequiredStatusCode} but received {result.Response.StatusCode} with the following body: {result.RawBody}");
                
                return result;
            
            case "POST":
                var postResult = await AsyncHttpClient.PostAsync<T>(this, cancellationToken);
                
                if (RequiredStatusCode is not null && postResult.Response.StatusCode != RequiredStatusCode)
                    throw new HttpRequestException($"Expected status code {RequiredStatusCode} but received {postResult.Response.StatusCode} with the following body: {postResult.RawBody}");
                
                return postResult;
            
            //case HttpMethod.Put:
            //    return await SendPutAsync(cancellationToken);
            
            //case HttpMethod.Delete:
            //    return await SendDeleteAsync(cancellationToken);
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}