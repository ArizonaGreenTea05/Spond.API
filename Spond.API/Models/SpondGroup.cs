using static Spond.API.Enums;

namespace Spond.API.Models;

public class SpondGroup
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string SignupUrl { get; set; } = string.Empty;
    public EventVisibility EventVisibility { get; set; } = Enums.EventVisibility.Undefined;
    public bool ShareContactInfo { get; set; }
    public bool AdminsCanAddMembers { get; set; }
    public bool ContactInfoHidden { get; set; }
    public List<SpondMember> Members { get; set; } = [];
    public List<SpondSubGroup> SubGroups { get; set; } = [];
    public List<SpondRole> Roles { get; set; } = [];
    public List<string> AddressFormat { get; set; } = [];
}