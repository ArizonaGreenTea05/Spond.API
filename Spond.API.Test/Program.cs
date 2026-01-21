using Spond.API.Services;

namespace Spond.API.Test;

internal class Program
{
    private static readonly SpondClient _spondClient = new();

    static async Task Main(string[] args)
    {
        Console.Write("Email or Phone Number: ");
        var emailPhone = Console.ReadLine();
        Console.Write("Password: ");
        var password = Console.ReadLine();

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

        var currentUser = await _spondClient.GetCurrentUser();
        Console.WriteLine(currentUser is null
            ? "The current user is null."
            : $"Current User: {currentUser.FirstName} {currentUser.LastName} (ID: {currentUser.Id})");

        var events = await _spondClient.GetEvents(DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));
        Console.WriteLine();
        Console.WriteLine("Events from the last month to the next month:");
        foreach (var spondEvent in events)
        {
            Console.WriteLine($"- {spondEvent.Name} (ID: {spondEvent.Id}) from {spondEvent.StartTime} to {spondEvent.EndTime}");
        }
    }

    public static async Task<bool> Login(string emailPhone, string password)
    {
        if (await _spondClient.LoginWithEmail(emailPhone, password)) return true;
        if (await _spondClient.LoginWithPhoneNumber(emailPhone, password)) return true;
        return false;
    }
}
