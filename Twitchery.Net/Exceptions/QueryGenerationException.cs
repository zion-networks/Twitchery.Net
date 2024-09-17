namespace TwitcheryNet.Exceptions;

public class QueryGenerationException<T> : QueryGenerationException
{
    public QueryGenerationException(string parameterName) : base(typeof(T).Name, parameterName)
    {
        
    }
}

public class QueryGenerationException : Exception
{
    public string QueryObjectTypeName { get; }
    public string ParameterName { get; }
    
    public QueryGenerationException(string queryObjectTypeName, string parameterName) : base($"{parameterName} is required.")
    {
        QueryObjectTypeName = queryObjectTypeName;
        ParameterName = parameterName;
    }
}