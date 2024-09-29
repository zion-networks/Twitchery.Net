using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
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
    public string ContentType { get; private set; } = MediaTypeNames.Application.Json;
    public Dictionary<string, string> Headers { get; } = new();
    public Dictionary<string, string> QueryParameters { get; } = new();
    public string? Body { get; set; }
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
    
    public HttpRequestBuilder AddHeader(string key, string value)
    {
        Headers[key] = value;
        return this;
    }
    
    public HttpRequestBuilder AddHeaderOptional(string? key, string? value)
    {
        if (key is not null && value is not null)
        {
            Headers[key] = value;
        }

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
        {
            QueryParameters[key] = value;
        }

        return this;
    }
    
    public HttpRequestBuilder SetQuery<TQuery>(TQuery? query) where TQuery : class, IQueryParameters
    {
        if (query is null)
        {
            return this;
        }

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
    
    public HttpRequestBuilder SetBody<T>(T? body) where T : class
    {
        if (body is null)
        {
            return this;
        }

        Body = JsonConvert.SerializeObject(body, Formatting.None);
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
        {
            var strContent = new StringContent(Body ?? string.Empty, Encoding.UTF8, ContentType);
            request.Content = strContent;
        }

        Request = request;
        
        return this;
    }
    
    public async Task<AsyncHttpResponse> SendAsync(CancellationToken token = default)
    {
        if (Request is null)
        {
            throw new InvalidOperationException("Request is not built.");
        }

        var result = Method.Method switch
        {
            "GET" => await AsyncHttpClient.GetAsync(this, token),
            "POST" => await AsyncHttpClient.PostAsync(this, token),
            _ => throw new NotImplementedException($"Method {Method.Method} is not implemented yet.")
        };
        
        return result;
    }
    
    public async Task<AsyncHttpResponse<T>> SendAsync<T>(CancellationToken token = default) where T : class
    {
        if (Request is null)
        {
            throw new InvalidOperationException("Request is not built.");
        }

        var result = Method.Method switch
        {
            "GET" => await AsyncHttpClient.GetAsync<T>(this, token),
            "POST" => await AsyncHttpClient.PostAsync<T>(this, token),
            _ => throw new NotImplementedException($"Method {Method.Method} is not implemented yet.")
        };

        return result;
    }

    public HttpRequestBuilder SetContentType(string type)
    {
        ContentType = type;
        
        return this;
    }
}