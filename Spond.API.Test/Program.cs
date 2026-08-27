using Spond.API.Models;
using Spond.API.Services;

namespace Spond.API.Test;

internal class Program
{
    private static readonly SpondClient SpondClient = new();

    private static async Task Main(string[] args)
    {
        string? emailPhone;
        string? password;
        if (args.Length >= 1)
        {
            emailPhone = args[0];
        }
        else
        {
            Console.Write("Email or Phone Number: ");
            emailPhone = Console.ReadLine();
        }

        if (args.Length >= 2)
        {
            password = args[1];
        }
        else
        {
            Console.Write("Password: ");
            password = Console.ReadLine();
        }

        if (emailPhone is null)
        {
            Console.WriteLine("Email or Phone Number cannot be null.");
            return;
        }
        if (password is null)
        {
            Console.WriteLine("Password cannot be null.");
            return;
        }
        var success = await Login(emailPhone, password);
        if (!success)
        {
            Console.WriteLine("Login failed.");
            return;
        }

        Console.WriteLine("Login successful!");

        var currentUser = await SpondClient.GetCurrentUser();
        Console.WriteLine(currentUser is null
            ? "The current user is null."
            : $"Current User: {currentUser.FirstName} {currentUser.LastName} (ID: {currentUser.Id})");

        var exit = false;
        while (!exit)
        {
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("\t0: Exit");
            Console.WriteLine("\t1: Print events");
            Console.WriteLine("\t2: Print groups");
            Console.WriteLine("\t3: Print events of group");
            Console.WriteLine("\t4: Print events of subgroup");
            Console.WriteLine("\t5: Print members of group");
            Console.WriteLine("\t6: Print members of subgroup");
            Console.WriteLine("\t7: Print posts");
            Console.WriteLine("\t8: Print posts of group");
            Console.WriteLine("\t9: Print chats");
            Console.WriteLine("\t10: Change event response");

            if (!int.TryParse(Console.ReadLine(), out var option))
            {
                Console.WriteLine("Invalid option. Please enter a number.");
                continue;
            }

            Console.WriteLine();

            switch (option)
            {
                case 0:
                    Console.WriteLine("Exiting...");
                    exit = true;
                    break;
                case 1:
                    await PrintEvents();
                    break;
                case 2:
                    await PrintGroups();
                    break;
                case 3:
                {
                    var group = await SelectGroup();
                    if (group is null)
                    {
                        Console.WriteLine("No group selected.");
                        break;
                    }
                    Console.WriteLine();
                    await PrintEvents(group.Id);
                    break;
                }
                case 4:
                {
                    var group = await SelectGroup();
                    if (group is null)
                    {
                        Console.WriteLine("No group selected.");
                        break;
                    }
                    Console.WriteLine();
                    var subGroup = SelectSubGroup(group);
                    if (subGroup is null)
                    {
                        Console.WriteLine("No sub-group selected.");
                        break;
                    }
                    Console.WriteLine();
                    await PrintEvents(group.Id, subGroup.Id);
                    break;
                }
                case 5:
                {
                    var group = await SelectGroup();
                    if (group is null)
                    {
                        Console.WriteLine("No group selected.");
                        break;
                    }
                    Console.WriteLine();
                    PrintMembers(group);
                    break;
                }
                case 6:
                {
                    var group = await SelectGroup();
                    if (group is null)
                    {
                        Console.WriteLine("No group selected.");
                        break;
                    }
                    Console.WriteLine();
                    var subGroup = SelectSubGroup(group);
                    if (subGroup is null)
                    {
                        Console.WriteLine("No sub-group selected.");
                        break;
                    }
                    Console.WriteLine();
                    PrintMembers(group, subGroup);
                    break;
                }
                case 7:
                    await PrintPosts();
                    break;
                case 8:
                {
                    var group = await SelectGroup();
                    if (group is null)
                    {
                        Console.WriteLine("No group selected.");
                        break;
                    }
                    Console.WriteLine();
                    await PrintPosts(group.Id);
                    break;
                }
                case 9:
                    await PrintChats();
                    break;
                case 10:
                    await ChangeEventResponse();
                    break;
            }
        }
    }

    private static void PrintMembers(SpondGroup group)
    {
        var members = group.Members.GroupBy(m => m.Respondent).ToDictionary(g => g.Key, g => g.ToList());

        if (members.TryGetValue(true, out var respondents))
        {
            Console.WriteLine($"{respondents.Count} respondents of group {group.Name} (ID: {group.Id}):");
            foreach (var member in respondents.OrderBy(m => m.FirstName))
            {
                Console.WriteLine($"\t- {member.FirstName} {member.LastName}, {member.Birthday?.ToShortDateString()} (ID: {member.Id})");
            }
            Console.WriteLine();
        }

        if (!members.TryGetValue(false, out var admins)) return;
        
        Console.WriteLine($"{admins.Count} admins of group {group.Name} (ID: {group.Id}):");
        foreach (var member in admins.OrderBy(m => m.FirstName))
        {
            Console.WriteLine($"\t- {member.FirstName} {member.LastName}, {member.Birthday?.ToShortDateString()} (ID: {member.Id})");
        }
    }

    private static void PrintMembers(SpondGroup group, SpondSubGroup subGroup)
    {
        var members = group.Members.Where(m => m.SubGroups.Contains(subGroup.Id)).GroupBy(m => m.Respondent).ToDictionary(g => g.Key, g => g.ToList());

        if (members.TryGetValue(true, out var respondents))
        {
            Console.WriteLine($"{respondents.Count} respondents of sub-group {subGroup.Name} (ID: {subGroup.Id}):");
            foreach (var member in respondents.OrderBy(m => m.FirstName))
            {
                Console.WriteLine($"\t- {member.FirstName} {member.LastName}, {member.Birthday?.ToShortDateString()} (ID: {member.Id})");
            }
            Console.WriteLine();
        }

        if (!members.TryGetValue(false, out var admins)) return;

        Console.WriteLine($"{admins.Count} admins of sub-group {subGroup.Name} (ID: {subGroup.Id}):");
        foreach (var member in admins.OrderBy(m => m.FirstName))
        {
            Console.WriteLine($"\t- {member.FirstName} {member.LastName}, {member.Birthday?.ToShortDateString()} (ID: {member.Id})");
        }
    }

    private static async Task<SpondGroup?> SelectGroup()    {
        var groups = await SpondClient.GetGroups();
        Console.WriteLine("Select a group:");
        for (var i = 0; i < groups.Count; i++)
        {
            var spondGroup = groups[i];
            Console.WriteLine($"{i + 1}:\t- {spondGroup.Name} (ID: {spondGroup.Id})");
        }

        if (!int.TryParse(Console.ReadLine(), out var groupOption)) return null;
        return groupOption <= groups.Count && groupOption > 0 ? groups[groupOption - 1] : null;
    }

    private static SpondSubGroup? SelectSubGroup(SpondGroup group)
    {
        var subGroups = group.SubGroups;
        Console.WriteLine("Select a sub-group:");
        for (var i = 0; i < subGroups.Count; i++)
        {
            var subGroup = subGroups[i];
            Console.WriteLine($"{i + 1}:\t- {subGroup.Name} (ID: {subGroup.Id})");
        }

        if (!int.TryParse(Console.ReadLine(), out var subGroupOption)) return null;
        return subGroupOption <= subGroups.Count && subGroupOption > 0 ? subGroups[subGroupOption - 1] : null;
    }

    private static async Task PrintEvents(string? groupId = null, string? subGroupId = null)
    {
        var events = groupId is null 
            ? await SpondClient.GetEvents(DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1))
            : subGroupId is null
                ? await SpondClient.GetEvents(groupId, DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1))
                : await SpondClient.GetEvents(groupId, subGroupId, DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));
        Console.WriteLine($"Events from the last month to the next month ({nameof(groupId)}: {groupId}, {nameof(subGroupId)}: {subGroupId}):");
        foreach (var spondEvent in events)
        {
            Console.WriteLine($"\t- {spondEvent.Name} (ID: {spondEvent.Id})" 
                              + $" from {spondEvent.StartTime.ToShortDateString()} - {spondEvent.StartTime.ToShortTimeString()}" 
                              + $" to {(spondEvent.StartTime.Date == spondEvent.EndTime.Date ? string.Empty : $"{spondEvent.EndTime.ToShortDateString()} - ")}{spondEvent.EndTime.ToShortTimeString()}");
        }
    }

    private static async Task PrintGroups()
    {
        var groups = await SpondClient.GetGroups();
        Console.WriteLine("Groups this user is a member of:");
        foreach (var spondGroup in groups)
        {
            Console.WriteLine($"\t- {spondGroup.Name} (ID: {spondGroup.Id})");
        }
    }

    private static async Task PrintPosts(string? groupId = null)
    {
        var posts = await SpondClient.GetPosts(groupId);
        Console.WriteLine($"Posts{(groupId is null ? string.Empty : $" for group {groupId}")}:");
        foreach (var post in posts)
        {
            var preview = post.Text is not null ? post.Text.Substring(0, Math.Min(80, post.Text.Length)) : string.Empty;
            Console.WriteLine($"\t- [{post.CreatedTime}] {preview}... (ID: {post.Id}, Comments: {post.Comments.Count})");
        }
    }

    private static async Task PrintChats()
    {
        var chats = await SpondClient.GetMessages();
        Console.WriteLine("Recent chats:");
        foreach (var chat in chats)
        {
            var lastMsg = chat.Message?.Text ?? "(no message)";
            Console.WriteLine($"\t- Chat ID: {chat.Id}, Last message: {lastMsg?.Substring(0, Math.Min(60, lastMsg.Length))}");
        }
    }

    private static async Task ChangeEventResponse()
    {
        var events = await SpondClient.GetEvents(DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));
        Console.WriteLine("Select an event:");
        for (var i = 0; i < events.Count; i++)
            Console.WriteLine($"{i + 1}:\t- {events[i].Name} (ID: {events[i].Id})");
        if (!int.TryParse(Console.ReadLine(), out var eventOption) || eventOption < 1 || eventOption > events.Count) return;
        var selectedEvent = events[eventOption - 1];

        Console.Write("Enter member ID: ");
        var memberId = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(memberId)) return;

        Console.Write("Accept? (y/n): ");
        var accepted = Console.ReadLine()?.Trim().ToLower() == "y";

        string? declineMessage = null;
        if (!accepted)
        {
            Console.Write("Decline message (optional): ");
            declineMessage = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(declineMessage)) declineMessage = null;
        }

        var result = await SpondClient.ChangeResponse(selectedEvent.Id, memberId, accepted, declineMessage);
        Console.WriteLine(result is null
            ? "Failed to change response."
            : $"Response changed. Accepted: {result.AcceptedIds.Count}, Declined: {result.DeclinedIds.Count}, Unanswered: {result.UnansweredIds.Count}");
    }

    public static async Task<bool> Login(string emailPhone, string password)
    {
        static Task<string> OtpCallback(string maskedPhone)
        {
            Console.Write($"Enter the one-time password sent to {maskedPhone}: ");
            return Task.FromResult(Console.ReadLine() ?? string.Empty);
        }

        if (await SpondClient.LoginWithEmail(emailPhone, password, OtpCallback)) return true;
        if (await SpondClient.LoginWithPhoneNumber(emailPhone, password, OtpCallback)) return true;
        return false;
    }
}
