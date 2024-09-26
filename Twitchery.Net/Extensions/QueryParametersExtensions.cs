using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Web;
using TwitcheryNet.Attributes;
using TwitcheryNet.Exceptions;
using TwitcheryNet.Models.Helix;

namespace TwitcheryNet.Extensions;

public static class QueryParametersExtensions
{
    public static string ToQueryString<T>(this T queryParameters) where T : IQueryParameters
    {
        var properties = queryParameters.GetType().GetProperties();
        var queryString = "?";

        foreach (var property in properties)
        {
            var attribute = property.GetCustomAttribute<QueryParameterAttribute>();
            if (attribute == null)
            {
                continue;
            }

            var value = property.GetValue(queryParameters) ?? attribute.DefaultValue;
            
            if (value == null)
            {
                if (attribute.Required)
                {
                    throw new QueryGenerationException<T>(attribute.Name);
                }
                
                continue;
            }
            
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(queryParameters);
            if (Validator.TryValidateObject(queryParameters, validationContext, validationResults, true) is false)
            {
                throw new QueryParameterValidationException<T>(validationResults);
            }
            
            if (value is string strVal)
            {
                var urlEncodedName = HttpUtility.UrlEncode(attribute.Name);
                var urlEncodedValue = HttpUtility.UrlEncode(strVal);
            
                queryString += $"{urlEncodedName}={urlEncodedValue}&";
            }
            else if (value is int intVal)
            {
                var urlEncodedName = HttpUtility.UrlEncode(attribute.Name);
                var urlEncodedValue = HttpUtility.UrlEncode(intVal.ToString());
            
                queryString += $"{urlEncodedName}={urlEncodedValue}&";
            }
            else if (value is List<string> listVal)
            {
                foreach (var val in listVal)
                {
                    var urlEncodedName = HttpUtility.UrlEncode(attribute.Name);
                    var urlEncodedValue = HttpUtility.UrlEncode(val);
                
                    queryString += $"{urlEncodedName}={urlEncodedValue}&";
                }
            }
            else
            {
                throw new QueryParameterTypeUnsupported(property.Name, property.PropertyType);
            }
        }
        
        queryString = queryString.TrimEnd('&');
        
        return queryString;
    }
}