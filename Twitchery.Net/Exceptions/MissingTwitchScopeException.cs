using TwitcheryNet.Extensions;

namespace TwitcheryNet.Exceptions;

public class MissingTwitchScopeException : Exception
{
    public List<string> MissingScopes { get; }
    
    public override string Message => $"Missing required scopes: {string.Join(", ", MissingScopes)}";
    
    public MissingTwitchScopeException(params string[] missingScopes)
    {
        MissingScopes = missingScopes.ToList();
    }

    public static void ThrowIfMissing(List<string> scopes, params string[] required)
    {
        var missing = required.Where(scope => !scopes.Contains(scope)).ToList();
        if (missing.Count != 0)
        {
            throw new MissingTwitchScopeException(missing.ToArray());
        }
    }
    
    public static void ThrowIfMissing(List<string> scopes, params Enum[] required)
    {
        var requiredScopes = required
            .Select(e => e.GetValue() ?? string.Empty)
            .Where(e => e.Length > 0)
            .ToList();
        
        ThrowIfMissing(scopes, requiredScopes.ToArray());
    }
}