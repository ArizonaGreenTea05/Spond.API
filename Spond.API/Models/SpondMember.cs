namespace Spond.API.Models;

public class SpondMember
{
    public string Id { get; set; } = string.Empty;
    public SpondUserProfile? Profile { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string CreatedTime { get; set; } = string.Empty;
    public string? DateOfBirth { get; set; }
    public DateTime? Birthday => DateTime.TryParse(DateOfBirth, out var dateTime) ? dateTime : null;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public List<SpondMember> Guardians { get; set; } = [];
}