# Spond.API

A user-friendly C# interface for the Spond API, providing easy access to Spond's group management, event scheduling, and member information features.

## 🚀 Features

- **Simple Authentication**: Login with email or phone number
- **Group Management**: Retrieve and manage group information
- **Event Handling**: Query events with flexible filtering options
- **Member Information**: Access user profiles and member details
- **Strongly Typed Models**: Full C# model support with IntelliSense
- **XML Documentation**: Complete API documentation included

## 📦 Installation

Install via NuGet Package Manager:

```bash
dotnet add package SpondSharp
```

Or via Package Manager Console:

```powershell
Install-Package SpondSharp
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

- `LoginWithEmail(string email, string password)` - Authenticate using email
- `LoginWithPhoneNumber(string phoneNumber, string password)` - Authenticate using phone number

### Data Retrieval

- `GetGroups()` - Retrieve all groups
- `GetCurrentUser()` - Get the current user's profile
- `GetEvents(...)` - Retrieve events with various filtering options
  - Filter by time range
  - Filter by group or subgroup
  - Include/exclude comments, hidden events
  - Sort ascending or descending

## 🛠️ Advanced Usage

### Filtering Events

```csharp
using static Spond.API.Enums;

// Get events for a specific group
var group = groups.First();
var groupEvents = await client.GetEvents(
    group, 
    DateTime.Now, 
    DateTime.Now.AddWeeks(2),
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
- [NuGet Package](https://www.nuget.org/packages/SpondSharp)
- [Spond Website](https://spond.com)

## ⚠️ Disclaimer

This is an unofficial API client. Use at your own risk. The Spond API is not officially documented and may change without notice.