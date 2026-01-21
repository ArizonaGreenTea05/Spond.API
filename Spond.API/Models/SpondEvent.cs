using Spond.API.Extensions;
using static Spond.API.Extensions.DateTimeExtensions;

namespace Spond.API.Models;

public class SpondEvent
{
    public string Id { get; set; } = string.Empty;
    public string Heading { get; set; } = string.Empty;
    public string Name => Heading;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public string StartTimestamp
    {
        get => StartTime.ToIso8601(false);
        set => StartTime = FromIso8601(value, false);
    }

    public string EndTimestamp
    {
        get => EndTime.ToIso8601(false);
        set => EndTime = FromIso8601(value, false);
    }

    public List<SpondEventOwner> Owners { get; set; } = [];

    public List<SpondEventOwner> AcceptedOwners => Owners.Where(o => o.Response == "accepted").ToList();

    public List<SpondMember> AcceptedMembers => Responses?.AcceptedIds
        .Select(id => Recipients?.Group?.Members.FirstOrDefault(m => m.Id == id)).Where(m => m is not null).Cast<SpondMember>().ToList() ?? [];

    public SpondEventRecipients? Recipients { get; set; }

    public SpondEventResponses? Responses { get; set; }

    public bool? Cancelled { get; set; }
}

public class SpondEventRecipients
{
    public SpondGroup? Group { get; set; }
}

public class SpondEventResponses
{
    public List<string> AcceptedIds { get; set; } = [];
}