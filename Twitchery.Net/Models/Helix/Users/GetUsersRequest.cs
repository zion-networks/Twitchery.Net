using System.ComponentModel.DataAnnotations;
using TwitcheryNet.Attributes;

namespace TwitcheryNet.Models.Helix.Users;

public class GetUsersRequest : IQueryParameters
{
    [QueryParameter("id", false, true)]
    [MaxLength(100)]
    public List<string>? Id { get; set; }
    
    [QueryParameter("login", false, true)]
    [MaxLength(100)]
    public List<string>? Login { get; set; }
}