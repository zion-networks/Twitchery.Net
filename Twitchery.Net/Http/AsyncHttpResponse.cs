namespace TwitcheryNet.Http;

public class AsyncHttpResponse<T> : AsyncHttpResponse where T : class
{
    public T? Body { get; set; }
    
    public AsyncHttpResponse(HttpResponseMessage response, string rawBody, T? body) : base(response, rawBody)
    {
        Body = body;
    }
    
    public static AsyncHttpResponse<T> FromBase(AsyncHttpResponse baseResponse)
    {
        return new AsyncHttpResponse<T>(baseResponse.Response, baseResponse.RawBody, null);
    }
    
    public static AsyncHttpResponse<T> FromBase(AsyncHttpResponse baseResponse, T body)
    {
        return new AsyncHttpResponse<T>(baseResponse.Response, baseResponse.RawBody, body);
    }
}

public class AsyncHttpResponse
{
    public HttpResponseMessage Response { get; }
    public string RawBody { get; }
    
    public AsyncHttpResponse(HttpResponseMessage response, string rawBody)
    {
        Response = response;
        RawBody = rawBody;
    }
}