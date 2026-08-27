# Spond.API

A user-friendly C# interface for the Spond API, providing easy access to Spond's group management, event scheduling, and member information features.

## 🚀 Features

- **Simple Authentication**: Login with email or phone number, with 2FA support
- **Group Management**: Retrieve and manage group information
- **Event Handling**: Query, update events with flexible filtering options
- **Event Responses**: Change a member's accept/decline response for events
- **Attendance Export**: Download event attendance as XLSX
- **Posts**: Retrieve group wall posts with optional comments
- **Chat**: List chat conversations and send messages
- **Club Finance**: Retrieve Spond Club financial transactions
- **Member Information**: Access user profiles and member details
- **Strongly Typed Models**: Full C# model support with IntelliSense
- **XML Documentation**: Complete API documentation included

## 📦 Installation

Install via NuGet Package Manager:

```bash
dotnet add package Spond.API
```

Or via Package Manager Console:

```powershell
Install-Package Spond.API
```

## 🔧 Quick Start

### Basic Usage

```csharp
using Spond.API.Services;
using Microsoft.Extensions.Logging;

// Create a client instance
var client = new SpondClient();

// Login with email
bool success = await client.LoginWithEmail("your@email.com", "yourpassword");

if (success)
{
    // Get all groups
    var groups = await client.GetGroups();
    
    // Get current user profile
    var profile = await client.GetCurrentUser();
    
    // Get events for a specific time range
    var events = await client.GetEvents(
        DateTime.Now,
        DateTime.Now.AddMonths(1)
    );
}
```

### With Dependency Injection and Logging

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();
services.AddLogging(builder => builder.AddConsole());

var serviceProvider = services.BuildServiceProvider();
var logger = serviceProvider.GetService<ILogger<SpondClient>>();

var client = new SpondClient(logger: logger);
```

## 📚 Core API Methods

### Authentication

- `LoginWithEmail(string email, string password, otpCallback?)` - Authenticate using email
- `LoginWithPhoneNumber(string phoneNumber, string password, otpCallback?)` - Authenticate using phone number

### Data Retrieval

- `GetGroups()` - Retrieve all groups
- `GetCurrentUser()` - Get the current user's profile
- `GetEvents(...)` - Retrieve events with various filtering options
  - Filter by time range (end timestamps and/or start timestamps)
  - Filter by group or subgroup
  - Include/exclude comments, hidden events
  - Sort ascending or descending
- `GetEvent(string id)` - Retrieve a single event by ID
- `GetPosts(groupId?, max, includeComments)` - Retrieve group wall posts
- `GetMessages(max?)` - Retrieve recent chat conversations
- `GetEventAttendance(string eventId)` - Download event attendance as XLSX bytes

### Write Operations

- `UpdateEvent(string eventId, SpondEventUpdateRequest updates)` - Update an existing event
- `ChangeResponse(string eventId, string memberId, bool accepted, declineMessage?)` - Change a member's event response (accept/decline)
- `SendMessage(string chatId, string text)` - Send a message to an existing chat thread
- `SendMessage(string recipientProfileId, string groupId, string text)` - Start a new chat with a member

### Spond Club Finance

- `GetTransactions(string clubId, int maxItems?)` - Retrieve Club financial transactions (paginated)

## 🛠️ Advanced Usage

### Filtering Events

```csharp
using static Spond.API.Enums;

// Get events for a specific group
var group = groups.First();
var groupEvents = await client.GetEvents(
    group, 
    DateTime.Now,
    DateTime.Now.AddDays(14),
    max: 50,
    order: Order.Descending,
    includeComments: true
);

// Get events for a subgroup
var subGroup = group.SubGroups.First();
var subGroupEvents = await client.GetEvents(
    group,
    subGroup,
    DateTime.Now,
    DateTime.Now.AddMonths(1)
);

// Filter by start time (flexible range)
var upcomingEvents = await client.GetEvents(
    minStartTime: DateTime.Now,
    maxStartTime: DateTime.Now.AddDays(7),
    max: 20
);
```

### Working with Posts

```csharp
// Get posts for all groups
var posts = await client.GetPosts();

// Get posts for a specific group
var groupPosts = await client.GetPosts(groupId: group.Id, max: 50);

foreach (var post in groupPosts)
{
    Console.WriteLine($"{post.CreatedTime}: {post.Text}");
    foreach (var comment in post.Comments)
        Console.WriteLine($"  └ {comment.Text}");
}
```

### Updating Events and Responses

```csharp
// Update an event
var updated = await client.UpdateEvent(eventId, new SpondEventUpdateRequest
{
    Description = "Updated description",
    MaxAccepted = 20
});

// Accept an event on behalf of a member
var responses = await client.ChangeResponse(eventId, memberId, accepted: true);

// Decline with a message
responses = await client.ChangeResponse(eventId, memberId, accepted: false, declineMessage: "Can't make it");

// Download attendance report
byte[]? xlsx = await client.GetEventAttendance(eventId);
if (xlsx is not null)
    File.WriteAllBytes($"{eventId}.xlsx", xlsx);
```

### Chat

```csharp
// List recent chats
var chats = await client.GetMessages(max: 50);
foreach (var chat in chats)
    Console.WriteLine($"Chat {chat.Id}: {chat.Message?.Text}");

// Send a message to an existing chat thread
await client.SendMessage(chatId: chats.First().Id, text: "Hello!");

// Start a new chat (use Profile.Id, not member Id)
var member = group.Members.First();
await client.SendMessage(
    recipientProfileId: member.Profile!.Id,
    groupId: group.Id,
    text: "Hello from the API!"
);
```

### Spond Club Transactions

```csharp
// Retrieve up to 200 transactions for a club
// clubId is found in the Spond Club web UI URL
var transactions = await client.GetTransactions("your-club-id", maxItems: 200);
foreach (var tx in transactions)
    Console.WriteLine($"{tx.PaidAt}: {tx.PaymentName} paid by {tx.PaidByName}");
```

## 📖 Documentation

- [User Guide](docs/user-guide.md) - Detailed usage examples and scenarios
- [Developer Guide](docs/developer-guide.md) - Architecture, contribution guidelines, and API details
- XML Documentation - Full IntelliSense support with method descriptions

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🔗 Links

- [GitHub Repository](https://github.com/ArizonaGreenTea05/Spond.API)
- [NuGet Package](https://www.nuget.org/packages/Spond.API)
- [Spond Website](https://spond.com)

## ⚠️ Disclaimer

This is an unofficial API client. Use at your own risk. The Spond API is not officially documented and may change without notice.