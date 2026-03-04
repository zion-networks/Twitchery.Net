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
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static async Task<AsyncHttpResponse<T>> GetAsync<T>(Uri uri, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await GetAsync(uri, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static async Task<AsyncHttpResponse<T>> GetAsync<T>(HttpRequestBuilder builder, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await GetAsync(builder, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

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
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static async Task<AsyncHttpResponse<T>> PostAsync<T>(Uri uri, HttpContent content, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await PostAsync(uri, content, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static async Task<AsyncHttpResponse<T>> PostAsync<T>(HttpRequestBuilder builder, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await PostAsync(builder, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

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

    public static async Task<AsyncHttpResponse> DeleteAsync(string uri, CancellationToken cancellationToken = default)
    {
        return await DeleteAsync(new Uri(uri), cancellationToken);
    }

    public static async Task<AsyncHttpResponse> DeleteAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();
        using var response = await client.DeleteAsync(uri, cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        OnResponse?.Invoke(response, responseBody);

        return new AsyncHttpResponse(response, responseBody);
    }

    public static async Task<AsyncHttpResponse> DeleteAsync(HttpRequestBuilder builder, CancellationToken cancellationToken = default)
    {
        var request = builder.Request ?? throw new ArgumentNullException(nameof(builder.Request));

        using var client = new HttpClient();
        using var response = await client.SendAsync(request, cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        OnResponse?.Invoke(response, responseBody);

        return new AsyncHttpResponse(response, responseBody);
    }

    public static async Task<AsyncHttpResponse<T>> DeleteAsync<T>(string uri, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await DeleteAsync(uri, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static async Task<AsyncHttpResponse<T>> DeleteAsync<T>(Uri uri, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await DeleteAsync(uri, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static async Task<AsyncHttpResponse<T>> DeleteAsync<T>(HttpRequestBuilder builder, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await DeleteAsync(builder, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static HttpRequestBuilder StartDelete(string baseUri)
    {
        return StartDelete(new Uri(baseUri));
    }

    public static HttpRequestBuilder StartDelete(Uri baseUri)
    {
        return new HttpRequestBuilder(HttpMethod.Delete, baseUri);
    }

    public static async Task<AsyncHttpResponse> PutAsync(string uri, HttpContent content, CancellationToken cancellationToken = default)
    {
        return await PutAsync(new Uri(uri), content, cancellationToken);
    }

    public static async Task<AsyncHttpResponse> PutAsync(Uri uri, HttpContent content, CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();
        using var response = await client.PutAsync(uri, content, cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        OnResponse?.Invoke(response, responseBody);

        return new AsyncHttpResponse(response, responseBody);
    }

    public static async Task<AsyncHttpResponse> PutAsync(HttpRequestBuilder builder, CancellationToken cancellationToken = default)
    {
        var request = builder.Request ?? throw new ArgumentNullException(nameof(builder.Request));

        using var client = new HttpClient();
        using var response = await client.SendAsync(request, cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        OnResponse?.Invoke(response, responseBody);

        return new AsyncHttpResponse(response, responseBody);
    }

    public static async Task<AsyncHttpResponse<T>> PutAsync<T>(string uri, HttpContent content, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await PutAsync(uri, content, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static async Task<AsyncHttpResponse<T>> PutAsync<T>(Uri uri, HttpContent content, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await PutAsync(uri, content, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static async Task<AsyncHttpResponse<T>> PutAsync<T>(HttpRequestBuilder builder, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await PutAsync(builder, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static HttpRequestBuilder StartPut(string baseUri)
    {
        return StartPut(new Uri(baseUri));
    }

    public static HttpRequestBuilder StartPut(Uri baseUri)
    {
        return new HttpRequestBuilder(HttpMethod.Put, baseUri);
    }

    public static async Task<AsyncHttpResponse> PatchAsync(string uri, HttpContent content, CancellationToken cancellationToken = default)
    {
        return await PatchAsync(new Uri(uri), content, cancellationToken);
    }

    public static async Task<AsyncHttpResponse> PatchAsync(Uri uri, HttpContent content, CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();
        using var response = await client.PatchAsync(uri, content, cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        OnResponse?.Invoke(response, responseBody);

        return new AsyncHttpResponse(response, responseBody);
    }

    public static async Task<AsyncHttpResponse> PatchAsync(HttpRequestBuilder builder, CancellationToken cancellationToken = default)
    {
        var request = builder.Request ?? throw new ArgumentNullException(nameof(builder.Request));

        using var client = new HttpClient();
        using var response = await client.SendAsync(request, cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        OnResponse?.Invoke(response, responseBody);

        return new AsyncHttpResponse(response, responseBody);
    }

    public static async Task<AsyncHttpResponse<T>> PatchAsync<T>(string uri, HttpContent content, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await PatchAsync(uri, content, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static async Task<AsyncHttpResponse<T>> PatchAsync<T>(Uri uri, HttpContent content, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await PatchAsync(uri, content, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static async Task<AsyncHttpResponse<T>> PatchAsync<T>(HttpRequestBuilder builder, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await PatchAsync(builder, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static HttpRequestBuilder StartPatch(string baseUri)
    {
        return StartPatch(new Uri(baseUri));
    }

    public static HttpRequestBuilder StartPatch(Uri baseUri)
    {
        return new HttpRequestBuilder(HttpMethod.Patch, baseUri);
    }

    public static async Task<AsyncHttpResponse> HeadAsync(string uri, CancellationToken cancellationToken = default)
    {
        return await HeadAsync(new Uri(uri), cancellationToken);
    }

    public static async Task<AsyncHttpResponse> HeadAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Head, uri);
        using var response = await client.SendAsync(request, cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        OnResponse?.Invoke(response, responseBody);

        return new AsyncHttpResponse(response, responseBody);
    }

    public static async Task<AsyncHttpResponse> HeadAsync(HttpRequestBuilder builder, CancellationToken cancellationToken = default)
    {
        var request = builder.Request ?? throw new ArgumentNullException(nameof(builder.Request));

        using var client = new HttpClient();
        using var response = await client.SendAsync(request, cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        OnResponse?.Invoke(response, responseBody);

        return new AsyncHttpResponse(response, responseBody);
    }

    public static async Task<AsyncHttpResponse<T>> HeadAsync<T>(string uri, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await HeadAsync(uri, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static async Task<AsyncHttpResponse<T>> HeadAsync<T>(Uri uri, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await HeadAsync(uri, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static async Task<AsyncHttpResponse<T>> HeadAsync<T>(HttpRequestBuilder builder, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await HeadAsync(builder, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static HttpRequestBuilder StartHead(string baseUri)
    {
        return StartHead(new Uri(baseUri));
    }

    public static HttpRequestBuilder StartHead(Uri baseUri)
    {
        return new HttpRequestBuilder(HttpMethod.Head, baseUri);
    }

    public static async Task<AsyncHttpResponse> OptionsAsync(string uri, CancellationToken cancellationToken = default)
    {
        return await OptionsAsync(new Uri(uri), cancellationToken);
    }

    public static async Task<AsyncHttpResponse> OptionsAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Options, uri);
        using var response = await client.SendAsync(request, cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        OnResponse?.Invoke(response, responseBody);

        return new AsyncHttpResponse(response, responseBody);
    }

    public static async Task<AsyncHttpResponse> OptionsAsync(HttpRequestBuilder builder, CancellationToken cancellationToken = default)
    {
        var request = builder.Request ?? throw new ArgumentNullException(nameof(builder.Request));

        using var client = new HttpClient();
        using var response = await client.SendAsync(request, cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        OnResponse?.Invoke(response, responseBody);

        return new AsyncHttpResponse(response, responseBody);
    }

    public static async Task<AsyncHttpResponse<T>> OptionsAsync<T>(string uri, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await OptionsAsync(uri, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static async Task<AsyncHttpResponse<T>> OptionsAsync<T>(Uri uri, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await OptionsAsync(uri, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static async Task<AsyncHttpResponse<T>> OptionsAsync<T>(HttpRequestBuilder builder, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await OptionsAsync(builder, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static HttpRequestBuilder StartOptions(string baseUri)
    {
        return StartOptions(new Uri(baseUri));
    }

    public static HttpRequestBuilder StartOptions(Uri baseUri)
    {
        return new HttpRequestBuilder(HttpMethod.Options, baseUri);
    }

    public static async Task<AsyncHttpResponse> TraceAsync(string uri, CancellationToken cancellationToken = default)
    {
        return await TraceAsync(new Uri(uri), cancellationToken);
    }

    public static async Task<AsyncHttpResponse> TraceAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Trace, uri);
        using var response = await client.SendAsync(request, cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        OnResponse?.Invoke(response, responseBody);

        return new AsyncHttpResponse(response, responseBody);
    }

    public static async Task<AsyncHttpResponse> TraceAsync(HttpRequestBuilder builder, CancellationToken cancellationToken = default)
    {
        var request = builder.Request ?? throw new ArgumentNullException(nameof(builder.Request));

        using var client = new HttpClient();
        using var response = await client.SendAsync(request, cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        OnResponse?.Invoke(response, responseBody);

        return new AsyncHttpResponse(response, responseBody);
    }

    public static async Task<AsyncHttpResponse<T>> TraceAsync<T>(string uri, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await TraceAsync(uri, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static async Task<AsyncHttpResponse<T>> TraceAsync<T>(Uri uri, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await TraceAsync(uri, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static async Task<AsyncHttpResponse<T>> TraceAsync<T>(HttpRequestBuilder builder, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await TraceAsync(builder, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static HttpRequestBuilder StartTrace(string baseUri)
    {
        return StartTrace(new Uri(baseUri));
    }

    public static HttpRequestBuilder StartTrace(Uri baseUri)
    {
        return new HttpRequestBuilder(HttpMethod.Trace, baseUri);
    }

    public static async Task<AsyncHttpResponse> ConnectAsync(string uri, CancellationToken cancellationToken = default)
    {
        return await ConnectAsync(new Uri(uri), cancellationToken);
    }

    public static async Task<AsyncHttpResponse> ConnectAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Connect, uri);
        using var response = await client.SendAsync(request, cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        OnResponse?.Invoke(response, responseBody);

        return new AsyncHttpResponse(response, responseBody);
    }

    public static async Task<AsyncHttpResponse> ConnectAsync(HttpRequestBuilder builder, CancellationToken cancellationToken = default)
    {
        var request = builder.Request ?? throw new ArgumentNullException(nameof(builder.Request));

        using var client = new HttpClient();
        using var response = await client.SendAsync(request, cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        OnResponse?.Invoke(response, responseBody);

        return new AsyncHttpResponse(response, responseBody);
    }

    public static async Task<AsyncHttpResponse<T>> ConnectAsync<T>(string uri, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await ConnectAsync(uri, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static async Task<AsyncHttpResponse<T>> ConnectAsync<T>(Uri uri, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await ConnectAsync(uri, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static async Task<AsyncHttpResponse<T>> ConnectAsync<T>(HttpRequestBuilder builder, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = AsyncHttpResponse<T>.FromBase(await ConnectAsync(builder, cancellationToken));

        if (string.IsNullOrWhiteSpace(response.RawBody))
        {
            return response;
        }

        response.Body = JsonConvert.DeserializeObject<T>(response.RawBody);

        return response;
    }

    public static HttpRequestBuilder StartConnect(string baseUri)
    {
        return StartConnect(new Uri(baseUri));
    }

    public static HttpRequestBuilder StartConnect(Uri baseUri)
    {
        return new HttpRequestBuilder(HttpMethod.Connect, baseUri);
    }
}