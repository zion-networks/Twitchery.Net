namespace TwitcheryNet.Exceptions;

public class QueryParameterTypeUnsupported : Exception
{
    public string ParameterName { get; }
    public Type ParameterType { get; }
    
    public QueryParameterTypeUnsupported(string parameterName, Type parameterType) : base($"Query parameter type unsupported: {parameterName} ({parameterType.Name})")
    {
        ParameterName = parameterName;
        ParameterType = parameterType;
    }
}