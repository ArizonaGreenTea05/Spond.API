using static Spond.API.Enums;

namespace Spond.API.Models;

public class SpondRole
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<Permission> Permissions { get; set; } = [];
}
