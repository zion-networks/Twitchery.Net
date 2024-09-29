using System.Net;
using Microsoft.Extensions.Logging;
using TwitcheryNet.Http;

namespace TwitcheryNet.Extensions;

public static class LoggerExtensions
{
    public static bool Validate(this AsyncHttpResponse response, string url, HttpStatusCode expected = HttpStatusCode.OK, string? error = null, ILogger? logger = null, bool logAsError = true)
    {
        var msg = response.Response;
        if (msg.StatusCode == expected)
        {
            return true;
        }

        var errorMsg = error ?? $"Received HTTP status code {msg.StatusCode} != {expected} at route {url}{(error != null ? $": {error}" : "")}";
        
        if (logAsError)
        {
            if (logger is not null)
            {
                logger.LogError("{msg}", errorMsg);
            }
            else
            {
                Console.WriteLine("[ERROR] {0}", errorMsg);
            }
        }
        else
        {
            if (logger is not null)
            {
                logger.LogWarning("{msg}", errorMsg);
            }
            else
            {
                Console.WriteLine("[WARNING] {0}", errorMsg);
            }
        }
        
        return false;
    }
}