namespace Spond.API.Models;

public class SpondGroup
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<SpondMember> Members { get; set; } = [];
}