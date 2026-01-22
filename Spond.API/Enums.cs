using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Spond.API;

public static class Enums
{
    public enum Order
    {
        Ascending,
        Descending
    }

    public enum EventVisibility
    {
        Undefined,
        Invitees
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Permission
    {
        Members,
        Admins,
        Settings,
        Events,
        Posts,
        Polls,
        Payments,
        Chat,
        Files,
        Fundraisers,
        [EnumMember(Value = "coaches-corner")]
        CoachesCorner
    }
}
