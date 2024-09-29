namespace TwitcheryNet.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class RequiresTokenAttribute : Attribute
{
    public TokenType TokenType { get; }
    
    public RequiresTokenAttribute(TokenType tokenType) => TokenType = tokenType;
}