using System.ComponentModel.DataAnnotations;

namespace TwitcheryNet.Exceptions;

public class QueryParameterValidationException<T> : QueryParameterValidationException
{
    public override string Message { get; }

    public QueryParameterValidationException(List<ValidationResult> validationResults) : base(validationResults)
    {
        var resultsStr = ValidationResults.Select(r => $"{r.MemberNames.First()}: {r.ErrorMessage}");
        Message = $"Query parameter validation on {typeof(T).Name} failed for properties: {resultsStr}";
    }
}

public class QueryParameterValidationException : Exception
{
    public List<ValidationResult> ValidationResults { get; }
    public override string Message { get; }

    public QueryParameterValidationException(List<ValidationResult> validationResults)
    {
        ValidationResults = validationResults;

        var resultsStr = ValidationResults.Select(r => $"{r.MemberNames.First()}: {r.ErrorMessage}");
        Message = $"Query parameter validation failed for properties: {resultsStr}";
    }
}