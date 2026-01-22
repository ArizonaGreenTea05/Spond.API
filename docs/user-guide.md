# Spond.API User Guide

A comprehensive guide to using the Spond.API library for interacting with the Spond platform.

## Table of Contents

1. [Introduction](#introduction)
2. [Getting Started](#getting-started)
3. [Authentication](#authentication)
4. [Working with Groups](#working-with-groups)
5. [Managing Events](#managing-events)
6. [Working with Members](#working-with-members)
7. [Best Practices](#best-practices)
8. [Common Scenarios](#common-scenarios)
9. [Troubleshooting](#troubleshooting)

---

## Introduction

Spond.API is a user-friendly C# library that provides programmatic access to the Spond platform. Spond is a team management and communication platform used by sports teams, clubs, and organizations to coordinate events, manage members, and facilitate communication.

### What Can You Do with Spond.API?

- **Authenticate** users via email or phone number
- **Retrieve group information** including members, subgroups, and roles
- **Query events** with flexible filtering options (date ranges, groups, subgroups)
- **Access member details** including profiles, guardians, and contact information
- **Manage event responses** and track attendance

### Key Features

- 🔐 Simple and secure authentication
- 📊 Strongly-typed models with full IntelliSense support
- 🎯 Flexible event filtering and querying
- 📝 Complete XML documentation
- 🔍 Comprehensive member and group management
- ⚡ Async/await support for modern C# applications

---

## Getting Started

### Prerequisites

- **.NET 10.0 or higher**
- A valid Spond account (with email/password or phone number/password)
- **Visual Studio 2026** or **Visual Studio Code** (recommended)
- Basic knowledge of C# and async/await patterns

### Installation

Install the package via NuGet Package Manager:

```bash
dotnet add package Spond.API
```

Or using the Package Manager Console in Visual Studio:

```powershell
Install-Package Spond.API
```

### Basic Setup

```csharp
using Spond.API.Services;
using Spond.API.Models;
using static Spond.API.Enums;

// Create a new client instance
var client = new SpondClient();
```

### With Dependency Injection (Recommended)

For production applications, use dependency injection with logging:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spond.API.Services;

// Configure services
var services = new ServiceCollection();
services.AddLogging(builder => 
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

var serviceProvider = services.BuildServiceProvider();
var logger = serviceProvider.GetService<ILogger<SpondClient>>();

// Create client with logger
var client = new SpondClient(logger: logger);
```

---

## Authentication

Authentication is the first step to using the Spond API. You must log in before making any data requests.

### Login with Email

Most users will authenticate using their email address:

```csharp
var client = new SpondClient();

string email = "your.email@example.com";
string password = "YourSecurePassword";

bool loginSuccess = await client.LoginWithEmail(email, password);

if (loginSuccess)
{
    Console.WriteLine("Successfully logged in!");
}
else
{
    Console.WriteLine("Login failed. Please check your credentials.");
}
```

### Login with Phone Number

Alternatively, you can authenticate using a phone number:

```csharp
var client = new SpondClient();

string phoneNumber = "+4712345678"; // Include country code
string password = "YourSecurePassword";

bool loginSuccess = await client.LoginWithPhoneNumber(phoneNumber, password);

if (loginSuccess)
{
    Console.WriteLine("Successfully logged in with phone number!");
}
else
{
    Console.WriteLine("Login failed. Please check your credentials.");
}
```

### Authentication Best Practices

- **Never hardcode credentials** in your source code
- Store credentials securely using environment variables or configuration files
- Use `appsettings.json` for configuration (exclude from version control)
- Consider using Azure Key Vault or similar services for production applications

Example using configuration:

```csharp
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

string email = configuration["Spond:Email"];
string password = configuration["Spond:Password"];

var client = new SpondClient();
await client.LoginWithEmail(email, password);
```

### Getting Current User Information

After successful authentication, retrieve the current user's profile:

```csharp
var currentUser = await client.GetCurrentUser();

if (currentUser != null)
{
    Console.WriteLine($"Logged in as: {currentUser.FirstName} {currentUser.LastName}");
    Console.WriteLine($"User ID: {currentUser.Id}");
}
```

---

## Working with Groups

Groups are the core organizational unit in Spond, representing teams, clubs, or organizations.

### Retrieving All Groups

Get all groups the authenticated user belongs to:

```csharp
var groups = await client.GetGroups();

Console.WriteLine($"You are a member of {groups.Count} group(s):");

foreach (var group in groups)
{
    Console.WriteLine($"- {group.Name} (ID: {group.Id})");
}
```

### Accessing Group Details

Each `SpondGroup` object contains comprehensive information:

```csharp
var group = groups.First();

// Basic information
Console.WriteLine($"Group Name: {group.Name}");
Console.WriteLine($"Group ID: {group.Id}");
Console.WriteLine($"Country: {group.CountryCode}");
Console.WriteLine($"Signup URL: {group.SignupUrl}");

// Settings
Console.WriteLine($"Contact Info Shared: {group.ShareContactInfo}");
Console.WriteLine($"Admins Can Add Members: {group.AdminsCanAddMembers}");
Console.WriteLine($"Contact Info Hidden: {group.ContactInfoHidden}");

// Counts
Console.WriteLine($"Total Members: {group.Members.Count}");
Console.WriteLine($"Subgroups: {group.SubGroups.Count}");
Console.WriteLine($"Roles: {group.Roles.Count}");
```

### Working with Subgroups

Subgroups allow teams to organize members into smaller divisions (e.g., age groups, skill levels):

```csharp
var group = groups.First();

if (group.SubGroups.Any())
{
    Console.WriteLine($"\nSubgroups in {group.Name}:");
    
    foreach (var subGroup in group.SubGroups)
    {
        Console.WriteLine($"- {subGroup.Name}");
        Console.WriteLine($"  ID: {subGroup.Id}");
        Console.WriteLine($"  Color: {subGroup.Color}");
    }
}
else
{
    Console.WriteLine("This group has no subgroups.");
}
```

### Finding a Specific Group

```csharp
var groups = await client.GetGroups();

// Find by name
var soccerTeam = groups.FirstOrDefault(g => 
    g.Name.Contains("Soccer", StringComparison.OrdinalIgnoreCase));

// Find by ID
string targetGroupId = "abc123xyz";
var specificGroup = groups.FirstOrDefault(g => g.Id == targetGroupId);

if (specificGroup != null)
{
    Console.WriteLine($"Found group: {specificGroup.Name}");
}
```

### Group Members Overview

```csharp
var group = groups.First();

Console.WriteLine($"\nMembers of {group.Name}:");

foreach (var member in group.Members)
{
    string name = $"{member.FirstName} {member.LastName}";
    string contact = member.Email ?? member.PhoneNumber ?? "No contact info";
    
    Console.WriteLine($"- {name} - {contact}");
    
    if (member.Guardians.Any())
    {
        Console.WriteLine($"  Guardians: {string.Join(", ", 
            member.Guardians.Select(g => $"{g.FirstName} {g.LastName}"))}");
    }
}
```

---

## Managing Events

Events are activities, practices, games, or meetings organized within groups.

### Basic Event Retrieval

Get all events within a date range:

```csharp
// Get events for the next 30 days
var startDate = DateTime.Now;
var endDate = DateTime.Now.AddDays(30);

var events = await client.GetEvents(startDate, endDate);

Console.WriteLine($"Found {events.Count} events:");

foreach (var evt in events)
{
    Console.WriteLine($"- {evt.Name}");
    Console.WriteLine($"  Start: {evt.StartTime:g}");
    Console.WriteLine($"  End: {evt.EndTime:g}");
}
```

### Filtering Events by Group

Retrieve events for a specific group:

```csharp
var groups = await client.GetGroups();
var group = groups.First();

var startDate = DateTime.Now;
var endDate = DateTime.Now.AddMonths(1);

// Get events for this group only
var groupEvents = await client.GetEvents(
    group,
    startDate,
    endDate
);

Console.WriteLine($"Events for {group.Name}: {groupEvents.Count}");
```

You can also use the group ID directly:

```csharp
string groupId = "your-group-id";
var groupEvents = await client.GetEvents(
    groupId,
    DateTime.Now,
    DateTime.Now.AddMonths(1)
);
```

### Filtering Events by Subgroup

For more granular filtering, retrieve events for specific subgroups:

```csharp
var group = groups.First();
var subGroup = group.SubGroups.First();

var subGroupEvents = await client.GetEvents(
    group,
    subGroup,
    DateTime.Now,
    DateTime.Now.AddMonths(1)
);

Console.WriteLine($"Events for {subGroup.Name}: {subGroupEvents.Count}");
```

Or using IDs:

```csharp
string groupId = "group-id";
string subGroupId = "subgroup-id";

var subGroupEvents = await client.GetEvents(
    groupId,
    subGroupId,
    DateTime.Now,
    DateTime.Now.AddWeeks(2)
);
```

### Advanced Event Filtering Options

The `GetEvents` method provides several optional parameters for fine-tuned control:

```csharp
var events = await client.GetEvents(
    minEndTime: DateTime.Now,
    maxEndTime: DateTime.Now.AddMonths(2),
    max: 50,                      // Limit to 50 events
    order: Order.Descending,      // Most recent first
    scheduled: true,              // Include scheduled events
    includeHidden: false,         // Exclude hidden events
    includeComments: true,        // Include event comments
    addProfileInfo: true          // Add profile information
);
```

#### Parameter Details

- **max**: Limit the number of events returned (useful for pagination)
- **order**: `Order.Ascending` or `Order.Descending` (based on start time)
- **scheduled**: Include or exclude scheduled events
- **includeHidden**: Whether to include events marked as hidden
- **includeComments**: Include comment data with events
- **addProfileInfo**: Add detailed profile information to event data

### Sorting Events

```csharp
// Get events in ascending order (earliest first)
var upcomingEvents = await client.GetEvents(
    DateTime.Now,
    DateTime.Now.AddMonths(1),
    order: Order.Ascending
);

// Get events in descending order (latest first)
var recentEvents = await client.GetEvents(
    DateTime.Now.AddMonths(-1),
    DateTime.Now,
    order: Order.Descending
);
```

### Working with Event Details

Access detailed information from event objects:

```csharp
var events = await client.GetEvents(DateTime.Now, DateTime.Now.AddDays(7));

foreach (var evt in events)
{
    Console.WriteLine($"\n=== {evt.Name} ===");
    Console.WriteLine($"ID: {evt.Id}");
    Console.WriteLine($"Start: {evt.StartTime:F}");
    Console.WriteLine($"End: {evt.EndTime:F}");
    Console.WriteLine($"Duration: {(evt.EndTime - evt.StartTime).TotalHours:F1} hours");
    
    // Check if cancelled
    if (evt.Cancelled == true)
    {
        Console.WriteLine("⚠️ This event has been cancelled");
    }
    
    // Event owners (organizers)
    Console.WriteLine($"\nOrganizers ({evt.Owners.Count}):");
    foreach (var owner in evt.Owners)
    {
        Console.WriteLine($"- {owner.FirstName} {owner.LastName} ({owner.Response})");
    }
    
    // Accepted owners
    var acceptedOwners = evt.AcceptedOwners;
    Console.WriteLine($"Accepted organizers: {acceptedOwners.Count}");
    
    // Accepted members
    var acceptedMembers = evt.AcceptedMembers;
    Console.WriteLine($"Accepted members: {acceptedMembers.Count}");
    
    foreach (var member in acceptedMembers.Take(5))
    {
        Console.WriteLine($"  ✓ {member.FirstName} {member.LastName}");
    }
}
```

### Filtering Events by Date Range

```csharp
// This week's events
var thisWeek = await client.GetEvents(
    DateTime.Now.StartOfWeek(),
    DateTime.Now.EndOfWeek()
);

// Next month's events
var nextMonth = await client.GetEvents(
    DateTime.Now.AddMonths(1).StartOfMonth(),
    DateTime.Now.AddMonths(1).EndOfMonth()
);

// Helper extension methods (you can create these)
public static class DateTimeExtensions
{
    public static DateTime StartOfWeek(this DateTime dt)
    {
        int diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
        return dt.AddDays(-diff).Date;
    }
    
    public static DateTime EndOfWeek(this DateTime dt)
    {
        return dt.StartOfWeek().AddDays(7).AddSeconds(-1);
    }
    
    public static DateTime StartOfMonth(this DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, 1);
    }
    
    public static DateTime EndOfMonth(this DateTime dt)
    {
        return dt.StartOfMonth().AddMonths(1).AddSeconds(-1);
    }
}
```

---

## Working with Members

Members represent individuals within groups, including players, coaches, parents, and administrators.

### Accessing Member Information

Members are retrieved as part of group data:

```csharp
var groups = await client.GetGroups();
var group = groups.First();

Console.WriteLine($"\nMembers of {group.Name}:");

foreach (var member in group.Members)
{
    Console.WriteLine($"\n{member.FirstName} {member.LastName}");
    Console.WriteLine($"  ID: {member.Id}");
    Console.WriteLine($"  Email: {member.Email ?? "N/A"}");
    Console.WriteLine($"  Phone: {member.PhoneNumber ?? "N/A"}");
    
    if (member.Birthday.HasValue)
    {
        Console.WriteLine($"  Birthday: {member.Birthday.Value:D}");
        var age = DateTime.Now.Year - member.Birthday.Value.Year;
        Console.WriteLine($"  Age: ~{age} years");
    }
}
```

### Working with Member Profiles

Members have associated user profiles:

```csharp
foreach (var member in group.Members)
{
    if (member.Profile != null)
    {
        Console.WriteLine($"{member.Profile.FirstName} {member.Profile.LastName}");
        Console.WriteLine($"  Profile ID: {member.Profile.Id}");
    }
    else
    {
        Console.WriteLine($"{member.FirstName} {member.LastName} (no profile)");
    }
}
```

### Working with Guardians

For youth sports or organizations, members often have guardians (parents or legal guardians):

```csharp
var group = groups.First();

Console.WriteLine("Members with Guardians:");

foreach (var member in group.Members.Where(m => m.Guardians.Any()))
{
    Console.WriteLine($"\n{member.FirstName} {member.LastName}");
    Console.WriteLine("  Guardians:");
    
    foreach (var guardian in member.Guardians)
    {
        Console.WriteLine($"    - {guardian.FirstName} {guardian.LastName}");
        Console.WriteLine($"      Email: {guardian.Email ?? "N/A"}");
        Console.WriteLine($"      Phone: {guardian.PhoneNumber ?? "N/A"}");
    }
}
```

### Finding Specific Members

```csharp
var group = groups.First();

// Find member by name
var member = group.Members.FirstOrDefault(m => 
    m.FirstName.Equals("John", StringComparison.OrdinalIgnoreCase) &&
    m.LastName.Equals("Smith", StringComparison.OrdinalIgnoreCase));

// Find member by ID
string memberId = "member-id-123";
var specificMember = group.Members.FirstOrDefault(m => m.Id == memberId);

// Find members by age range
var minors = group.Members.Where(m => 
    m.Birthday.HasValue && 
    DateTime.Now.Year - m.Birthday.Value.Year < 18);

Console.WriteLine($"Found {minors.Count()} minor members");
```

### Creating a Member Directory

```csharp
var group = groups.First();

Console.WriteLine($"=== {group.Name} Directory ===\n");

// Group members by last name
var sortedMembers = group.Members
    .OrderBy(m => m.LastName)
    .ThenBy(m => m.FirstName);

foreach (var member in sortedMembers)
{
    Console.WriteLine($"{member.LastName}, {member.FirstName}");
    
    if (!string.IsNullOrEmpty(member.Email))
        Console.WriteLine($"  📧 {member.Email}");
    
    if (!string.IsNullOrEmpty(member.PhoneNumber))
        Console.WriteLine($"  📱 {member.PhoneNumber}");
    
    if (member.Guardians.Any())
    {
        Console.WriteLine($"  👨‍👩‍👦 Guardians:");
        foreach (var guardian in member.Guardians)
        {
            Console.WriteLine($"     {guardian.FirstName} {guardian.LastName}");
            if (!string.IsNullOrEmpty(guardian.PhoneNumber))
                Console.WriteLine($"     📱 {guardian.PhoneNumber}");
        }
    }
    
    Console.WriteLine();
}
```

---

## Best Practices

### 1. Authentication Management

```csharp
public class SpondService
{
    private readonly SpondClient _client;
    private readonly ILogger<SpondService> _logger;
    private bool _isAuthenticated = false;
    
    public SpondService(ILogger<SpondService> logger)
    {
        _client = new SpondClient(logger: logger);
        _logger = logger;
    }
    
    public async Task<bool> EnsureAuthenticatedAsync(string email, string password)
    {
        if (_isAuthenticated)
            return true;
        
        _isAuthenticated = await _client.LoginWithEmail(email, password);
        
        if (!_isAuthenticated)
        {
            _logger.LogError("Failed to authenticate with Spond API");
        }
        
        return _isAuthenticated;
    }
}
```

### 2. Error Handling

Always implement proper error handling:

```csharp
try
{
    var groups = await client.GetGroups();
    
    if (groups == null || !groups.Any())
    {
        Console.WriteLine("No groups found or unable to retrieve groups.");
        return;
    }
    
    // Process groups...
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Network error: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```

### 3. Efficient Date Range Queries

Be mindful of the date ranges to avoid retrieving unnecessary data:

```csharp
// Good: Specific, reasonable date range
var upcomingEvents = await client.GetEvents(
    DateTime.Now,
    DateTime.Now.AddDays(14)
);

// Avoid: Very large date ranges that may timeout or return excessive data
var allEvents = await client.GetEvents(
    DateTime.Now.AddYears(-5),
    DateTime.Now.AddYears(5)
);
```

### 4. Caching Strategy

For frequently accessed data, implement caching:

```csharp
public class SpondCache
{
    private List<SpondGroup>? _cachedGroups;
    private DateTime _cacheExpiry = DateTime.MinValue;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(15);
    private readonly SpondClient _client;
    
    public SpondCache(SpondClient client)
    {
        _client = client;
    }
    
    public async Task<List<SpondGroup>> GetGroupsAsync()
    {
        if (_cachedGroups == null || DateTime.Now > _cacheExpiry)
        {
            _cachedGroups = await _client.GetGroups();
            _cacheExpiry = DateTime.Now.Add(_cacheDuration);
        }
        
        return _cachedGroups;
    }
    
    public void InvalidateCache()
    {
        _cachedGroups = null;
        _cacheExpiry = DateTime.MinValue;
    }
}
```

### 5. Using Logging Effectively

```csharp
var services = new ServiceCollection();
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddFilter("Spond.API", LogLevel.Information);
});

var logger = services.BuildServiceProvider()
    .GetService<ILogger<SpondClient>>();

var client = new SpondClient(logger: logger);
```

### 6. Null-Safety

Always check for null values:

```csharp
var currentUser = await client.GetCurrentUser();

if (currentUser == null)
{
    Console.WriteLine("Unable to retrieve current user information.");
    return;
}

// Safe to use currentUser here
Console.WriteLine($"Welcome, {currentUser.FirstName}!");
```

### 7. Disposing Resources

When using `SpondClient` in a long-running application, consider implementing `IDisposable`:

```csharp
public class SpondService : IDisposable
{
    private readonly SpondClient _client;
    
    public SpondService()
    {
        _client = new SpondClient();
    }
    
    public void Dispose()
    {
        // Cleanup if needed
    }
}

// Usage
using (var service = new SpondService())
{
    // Use service...
}
```

---

## Common Scenarios

### Scenario 1: Event Attendance Report

Generate a report of who's attending upcoming events:

```csharp
public async Task GenerateAttendanceReportAsync(SpondClient client)
{
    var groups = await client.GetGroups();
    var group = groups.First();
    
    var events = await client.GetEvents(
        group,
        DateTime.Now,
        DateTime.Now.AddDays(14),
        order: Order.Ascending
    );
    
    Console.WriteLine($"=== Attendance Report for {group.Name} ===\n");
    
    foreach (var evt in events)
    {
        Console.WriteLine($"\n{evt.Name}");
        Console.WriteLine($"Date: {evt.StartTime:D}");
        Console.WriteLine($"Time: {evt.StartTime:t} - {evt.EndTime:t}");
        
        var acceptedMembers = evt.AcceptedMembers;
        Console.WriteLine($"Attending: {acceptedMembers.Count}");
        
        foreach (var member in acceptedMembers)
        {
            Console.WriteLine($"  ✓ {member.FirstName} {member.LastName}");
        }
        
        Console.WriteLine($"---");
    }
}
```

### Scenario 2: Group Member Contact List

Export member contact information:

```csharp
public async Task ExportContactListAsync(SpondClient client, string groupName)
{
    var groups = await client.GetGroups();
    var group = groups.FirstOrDefault(g => 
        g.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase));
    
    if (group == null)
    {
        Console.WriteLine($"Group '{groupName}' not found.");
        return;
    }
    
    Console.WriteLine($"=== Contact List for {group.Name} ===\n");
    
    foreach (var member in group.Members.OrderBy(m => m.LastName))
    {
        Console.WriteLine($"{member.LastName}, {member.FirstName}");
        Console.WriteLine($"  Email: {member.Email ?? "Not provided"}");
        Console.WriteLine($"  Phone: {member.PhoneNumber ?? "Not provided"}");
        
        if (member.Guardians.Any())
        {
            Console.WriteLine("  Emergency Contacts:");
            foreach (var guardian in member.Guardians)
            {
                Console.WriteLine($"    {guardian.FirstName} {guardian.LastName}");
                Console.WriteLine($"    Phone: {guardian.PhoneNumber ?? "Not provided"}");
            }
        }
        
        Console.WriteLine();
    }
}
```

### Scenario 3: Weekly Schedule Display

Display this week's schedule:

```csharp
public async Task DisplayWeeklyScheduleAsync(SpondClient client)
{
    var startOfWeek = DateTime.Now.Date;
    while (startOfWeek.DayOfWeek != DayOfWeek.Monday)
    {
        startOfWeek = startOfWeek.AddDays(-1);
    }
    
    var endOfWeek = startOfWeek.AddDays(7);
    
    var events = await client.GetEvents(
        startOfWeek,
        endOfWeek,
        order: Order.Ascending
    );
    
    Console.WriteLine($"=== Schedule for Week of {startOfWeek:D} ===\n");
    
    var eventsByDay = events.GroupBy(e => e.StartTime.Date);
    
    foreach (var day in eventsByDay)
    {
        Console.WriteLine($"\n{day.Key:dddd, MMMM d}");
        Console.WriteLine(new string('-', 40));
        
        foreach (var evt in day.OrderBy(e => e.StartTime))
        {
            Console.WriteLine($"{evt.StartTime:t} - {evt.EndTime:t}: {evt.Name}");
            
            if (evt.Recipients?.Group != null)
            {
                Console.WriteLine($"  Group: {evt.Recipients.Group.Name}");
            }
        }
    }
}
```

### Scenario 4: Subgroup Member Lists

List members organized by subgroups:

```csharp
public async Task DisplayMembersBySubgroupAsync(SpondClient client, string groupName)
{
    var groups = await client.GetGroups();
    var group = groups.FirstOrDefault(g => g.Name.Contains(groupName));
    
    if (group == null || !group.SubGroups.Any())
    {
        Console.WriteLine("No subgroups found.");
        return;
    }
    
    Console.WriteLine($"=== {group.Name} - Members by Subgroup ===\n");
    
    foreach (var subGroup in group.SubGroups.OrderBy(sg => sg.Name))
    {
        Console.WriteLine($"\n{subGroup.Name} ({subGroup.Color})");
        Console.WriteLine(new string('-', 40));
        
        // Note: Subgroup membership isn't directly available in current models
        // This would require additional API calls or data structures
        Console.WriteLine("Members: [Requires additional implementation]");
    }
}
```

### Scenario 5: Event Reminder System

Check for upcoming events and send reminders:

```csharp
public async Task CheckUpcomingEventsAsync(SpondClient client)
{
    var tomorrow = DateTime.Now.Date.AddDays(1);
    var dayAfterTomorrow = tomorrow.AddDays(1);
    
    var upcomingEvents = await client.GetEvents(
        tomorrow,
        dayAfterTomorrow,
        order: Order.Ascending
    );
    
    if (!upcomingEvents.Any())
    {
        Console.WriteLine("No events scheduled for tomorrow.");
        return;
    }
    
    Console.WriteLine($"=== Events Tomorrow ({tomorrow:D}) ===\n");
    
    foreach (var evt in upcomingEvents)
    {
        Console.WriteLine($"⏰ REMINDER: {evt.Name}");
        Console.WriteLine($"   Time: {evt.StartTime:t} - {evt.EndTime:t}");
        
        var acceptedCount = evt.AcceptedMembers.Count;
        Console.WriteLine($"   Current attendance: {acceptedCount}");
        
        if (evt.Cancelled == true)
        {
            Console.WriteLine("   ⚠️ CANCELLED");
        }
        
        Console.WriteLine();
    }
}
```

### Scenario 6: Monthly Activity Summary

Generate a summary of activities for the month:

```csharp
public async Task GenerateMonthlySummaryAsync(SpondClient client, int year, int month)
{
    var startDate = new DateTime(year, month, 1);
    var endDate = startDate.AddMonths(1);
    
    var events = await client.GetEvents(startDate, endDate);
    
    Console.WriteLine($"=== Activity Summary for {startDate:MMMM yyyy} ===\n");
    Console.WriteLine($"Total Events: {events.Count}");
    
    var cancelledEvents = events.Where(e => e.Cancelled == true).Count();
    Console.WriteLine($"Cancelled Events: {cancelledEvents}");
    Console.WriteLine($"Active Events: {events.Count - cancelledEvents}");
    
    // Group by event type (if you have this in your model)
    var groupedByName = events.GroupBy(e => e.Name);
    Console.WriteLine($"\nEvent Breakdown:");
    
    foreach (var group in groupedByName.OrderByDescending(g => g.Count()))
    {
        Console.WriteLine($"  {group.Key}: {group.Count()} event(s)");
    }
    
    // Calculate total participation
    var totalAttendance = events.Sum(e => e.AcceptedMembers.Count);
    Console.WriteLine($"\nTotal Attendance (across all events): {totalAttendance}");
    
    if (events.Any())
    {
        var avgAttendance = totalAttendance / (double)events.Count;
        Console.WriteLine($"Average Attendance per Event: {avgAttendance:F1}");
    }
}
```

---

## Troubleshooting

### Authentication Issues

**Problem**: Login fails with valid credentials

**Solutions**:
1. Verify credentials are correct (check for typos, extra spaces)
2. Ensure you're using the correct login method (email vs. phone number)
3. Check if the account requires two-factor authentication
4. Verify network connectivity
5. Check if the Spond API is accessible from your location

```csharp
bool success = await client.LoginWithEmail(email, password);

if (!success)
{
    Console.WriteLine("Login failed. Please verify:");
    Console.WriteLine("1. Email address is correct");
    Console.WriteLine("2. Password is correct");
    Console.WriteLine("3. Network connection is stable");
    Console.WriteLine("4. Account doesn't have 2FA enabled");
}
```

### Empty Data Results

**Problem**: Methods return empty lists or null values

**Solutions**:
1. Ensure you're authenticated before making requests
2. Verify the date range is reasonable
3. Check if you actually have access to the data
4. Confirm the group/subgroup IDs are correct

```csharp
// Check authentication first
var currentUser = await client.GetCurrentUser();

if (currentUser == null)
{
    Console.WriteLine("Not authenticated. Please login first.");
    return;
}

var groups = await client.GetGroups();

if (groups == null || !groups.Any())
{
    Console.WriteLine("No groups found. This could mean:");
    Console.WriteLine("- You're not a member of any groups");
    Console.WriteLine("- Authentication expired");
    Console.WriteLine("- Network or API issue");
}
```

### Date Range Issues

**Problem**: Events not showing up as expected

**Solutions**:
1. Verify `minEndTime` is before `maxEndTime`
2. Check that the date range includes the events you expect
3. Remember that dates are based on event end time, not start time
4. Ensure times are in the correct timezone

```csharp
var startDate = DateTime.Now;
var endDate = startDate.AddDays(30);

Console.WriteLine($"Searching for events between:");
Console.WriteLine($"  Start: {startDate:F}");
Console.WriteLine($"  End: {endDate:F}");

var events = await client.GetEvents(startDate, endDate);
Console.WriteLine($"Found {events.Count} events");
```

### Network Timeouts

**Problem**: Requests timeout or take too long

**Solutions**:
1. Reduce the date range for event queries
2. Use the `max` parameter to limit results
3. Check network connectivity
4. Consider implementing retry logic

```csharp
// Use reasonable limits
var events = await client.GetEvents(
    DateTime.Now,
    DateTime.Now.AddDays(14),  // Smaller range
    max: 100                    // Limit results
);
```

### Null Reference Exceptions

**Problem**: `NullReferenceException` when accessing data

**Solutions**:
1. Always check for null before accessing properties
2. Use null-conditional operators (`?.`)
3. Verify that related data is populated

```csharp
// Safe approach
var groups = await client.GetGroups();

if (groups != null && groups.Any())
{
    var group = groups.First();
    
    // Check before accessing nested properties
    if (group.Members != null)
    {
        foreach (var member in group.Members)
        {
            // Use null-conditional operator
            var email = member.Email ?? "No email";
            
            // Check profile before accessing
            if (member.Profile != null)
            {
                Console.WriteLine($"{member.Profile.FirstName}");
            }
        }
    }
}
```

### Missing Event Details

**Problem**: Event objects missing expected data (comments, profile info, etc.)

**Solutions**:
1. Ensure optional parameters are set correctly
2. Use `includeComments: true` and `addProfileInfo: true`
3. Some data may not be available based on permissions

```csharp
// Request all available data
var events = await client.GetEvents(
    group,
    DateTime.Now,
    DateTime.Now.AddDays(7),
    includeComments: true,
    addProfileInfo: true,
    includeHidden: false
);
```

### API Version Issues

**Problem**: Unexpected behavior or missing features

**Solutions**:
1. Ensure you're using the latest version of Spond.API
2. Check the [GitHub repository](https://github.com/ArizonaGreenTea05/Spond.API) for updates
3. Review the changelog for breaking changes

```bash
# Update to the latest version
dotnet add package Spond.API --version latest
```

### Common Mistakes

1. **Forgetting to await async methods**
   ```csharp
   // Wrong
   var groups = client.GetGroups(); // Returns Task, not List
   
   // Correct
   var groups = await client.GetGroups();
   ```

2. **Not checking for authentication**
   ```csharp
   // Wrong
   var events = await client.GetEvents(...); // May fail if not logged in
   
   // Correct
   bool loggedIn = await client.LoginWithEmail(email, password);
   if (loggedIn)
   {
       var events = await client.GetEvents(...);
   }
   ```

3. **Using incorrect date formats**
   ```csharp
   // Wrong
   DateTime startDate = DateTime.Parse("01/15/2024"); // May fail with different cultures
   
   // Correct
   DateTime startDate = new DateTime(2024, 1, 15);
   ```

### Getting Help

If you continue to experience issues:

1. **Check the API documentation**: Review the XML documentation in IntelliSense
2. **Review examples**: Look at the code examples in this guide
3. **Check GitHub Issues**: Visit the [GitHub repository](https://github.com/ArizonaGreenTea05/Spond.API/issues)
4. **Enable logging**: Use logging to see detailed error messages
5. **Community support**: Ask questions in the project's discussion forum

```csharp
// Enable detailed logging for troubleshooting
var services = new ServiceCollection();
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Debug); // Detailed logging
});

var logger = services.BuildServiceProvider()
    .GetService<ILogger<SpondClient>>();

var client = new SpondClient(logger: logger);
```

---

## Additional Resources

- **API Documentation**: Full XML documentation available in IntelliSense
- **GitHub Repository**: [https://github.com/ArizonaGreenTea05/Spond.API](https://github.com/ArizonaGreenTea05/Spond.API)
- **NuGet Package**: [https://www.nuget.org/packages/Spond.API](https://www.nuget.org/packages/Spond.API)
- **Spond Official Website**: [https://spond.com](https://spond.com)
- **Developer Guide**: See `docs/developer-guide.md` for technical details

---

## Disclaimer

This is an **unofficial** API client for Spond. The Spond API is not officially documented and may change without notice. Use this library at your own risk. Always respect Spond's terms of service and rate limits.

---

*Last updated: 2024*
