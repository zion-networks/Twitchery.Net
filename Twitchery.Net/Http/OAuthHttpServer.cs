using System.Net;
using System.Text;
using Newtonsoft.Json;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Http;

public class OAuthHttpServer
{
    private string Prefix { get; }
    private string State { get; }
    
    public OAuthHttpServer(string prefix, string state)
    {
        if (HttpListener.IsSupported is false)
            throw new NotSupportedException("HttpListener is not supported on this platform.");

        Prefix = prefix;
        State = state;
    }


    public async Task<OAuthLoginRequest?> WaitForAuthentication(CancellationToken cancellationToken = default)
    {
        using var httpServer = new HttpListener();
        var keepRunning = true;
        
        OAuthLoginRequest? userLogin = null;
        
        httpServer.Prefixes.Add(Prefix);
        httpServer.Start();

        while (keepRunning && cancellationToken.IsCancellationRequested is false)
        {
            var ctx = await httpServer.GetContextAsync();
            var req = ctx.Request;
            
            if (req.HttpMethod != "GET" && req.HttpMethod != "POST")
                continue;

            if (req.Url is null)
                continue;

            switch (req.Url.AbsolutePath)
            {
                // GET PREFIX/#access_token=...&scope=...&state=...&token_type=...
                case "/":
                {
                    var response = Encoding.UTF8.GetBytes(ConstantHtml.OAuthHtml);
                    ctx.Response.ContentType = "text/html";
                    ctx.Response.ContentLength64 = response.Length;
                    await ctx.Response.OutputStream.WriteAsync(response, cancellationToken);
                    ctx.Response.Close();
                    break;
                }

                // POST PREFIX/auth
                case "/oauth":
                {
                    if (req.ContentType != "application/json")
                    {
                        await SendError(ctx, HttpStatusCode.BadRequest, "invalid_request", "The request must be application/json.", cancellationToken);
                        break;
                    }
                    
                    using var reader = new StreamReader(req.InputStream, req.ContentEncoding);
                    var body = await reader.ReadToEndAsync(cancellationToken);
                    var loginRequest = JsonConvert.DeserializeObject<OAuthLoginRequest>(body);

                    if (loginRequest is null)
                    {
                        await SendError(ctx, HttpStatusCode.BadRequest, "invalid_request", "The request body is invalid.", cancellationToken);
                        break;
                    }
                    
                    if (string.IsNullOrWhiteSpace(loginRequest.AccessToken))
                    {
                        await SendError(ctx, HttpStatusCode.BadRequest, "invalid_request", "The accessToken is missing.", cancellationToken);
                        break;
                    }
                    
                    if (string.IsNullOrWhiteSpace(loginRequest.Scope))
                    {
                        await SendError(ctx, HttpStatusCode.BadRequest, "invalid_request", "The scope is missing.", cancellationToken);
                        break;
                    }
                    
                    if (string.IsNullOrWhiteSpace(State) is false && string.IsNullOrWhiteSpace(loginRequest.State))
                    {
                        await SendError(ctx, HttpStatusCode.BadRequest, "invalid_request", "The state is missing.", cancellationToken);
                        break;
                    }
                    
                    if (string.IsNullOrWhiteSpace(loginRequest.TokenType) || loginRequest.TokenType != "bearer")
                    {
                        await SendError(ctx, HttpStatusCode.BadRequest, "invalid_request", "The tokenType is missing or invalid.", cancellationToken);
                        break;
                    }
                    
                    ctx.Response.StatusCode = 200;
                    ctx.Response.Close();
                    
                    userLogin = loginRequest;
                    keepRunning = false;
                    
                    break;
                }
                
                default:
                    continue;
            }
        }
        
        httpServer.Stop();
        
        return userLogin;
    }

    private async Task SendError(HttpListenerContext ctx, HttpStatusCode code, string errorType, string errorDescription,
        CancellationToken cancellationToken = default)
    {
        var response = JsonConvert.SerializeObject(new
        {
            error = errorType,
            error_description = errorDescription
        });
                        
        var responseBytes = Encoding.UTF8.GetBytes(response);
        ctx.Response.ContentType = "application/json";
        ctx.Response.ContentLength64 = responseBytes.Length;
        ctx.Response.StatusCode = (int)code;
                        
        await ctx.Response.OutputStream.WriteAsync(responseBytes, cancellationToken);
                        
        ctx.Response.Close();
    }
}