namespace Spond.API.Interfaces;

public interface ICommonData
{
    public string LoginTokenPropertyName { get;  }
    public string BaseUrl { get;  }
    public string LoginUrl { get; }
    public string UserUrl { get; }
    public string GroupsUrl { get; }
    public string EventsUrl { get; }
}
