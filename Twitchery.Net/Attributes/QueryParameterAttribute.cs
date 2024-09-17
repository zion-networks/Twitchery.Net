namespace TwitcheryNet.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class QueryParameterAttribute : Attribute
{
    public string Name { get; }
    public bool Required { get; }
    public bool ListAsSeparate { get; }
    public string? DefaultValue { get; }
    
    public QueryParameterAttribute(string name, bool required = false, bool listAsSeparate = true, string? defaultValue = null)
    {
        Name = name;
        Required = required;
        ListAsSeparate = listAsSeparate;
        DefaultValue = defaultValue;
    }
}