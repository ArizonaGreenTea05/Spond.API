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
            }
        }
    }

    private static async Task<SpondGroup?> SelectGroup()
    {
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
        Console.WriteLine("Select a group:");
        for (var i = 0; i < subGroups.Count; i++)
        {
            var subGroup = subGroups[i];
            Console.WriteLine($"{i + 1}:\t- {subGroup.Name} (ID: {subGroup.Id})");
        }

        if (!int.TryParse(Console.ReadLine(), out var groupOption)) return null;
        return groupOption <= subGroups.Count && groupOption > 0 ? subGroups[groupOption - 1] : null;
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

    public static async Task<bool> Login(string emailPhone, string password)
    {
        if (await SpondClient.LoginWithEmail(emailPhone, password)) return true;
        if (await SpondClient.LoginWithPhoneNumber(emailPhone, password)) return true;
        return false;
    }
}
