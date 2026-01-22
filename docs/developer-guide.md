# Developer Guide

## Table of Contents
1. [Architecture Overview](#architecture-overview)
2. [Project Structure](#project-structure)
3. [Core Components](#core-components)
4. [API Version Support](#api-version-support)
5. [Building the Project](#building-the-project)
6. [Testing](#testing)
7. [XML Documentation](#xml-documentation)
8. [Contributing Guidelines](#contributing-guidelines)
9. [Code Style and Standards](#code-style-and-standards)
10. [Release Process](#release-process)
11. [Extending the API](#extending-the-api)

---

## Architecture Overview

Spond.API is a C# library that provides a strongly-typed interface to the unofficial Spond API. The architecture follows a clean separation of concerns:

```
┌─────────────────┐
│  SpondClient    │  ← Main entry point for API operations
└────────┬────────┘
         │
         ├─→ HttpClient (with CookieContainer)
         ├─→ ICommonData (API version configuration)
         └─→ ILogger (optional logging)
                │
                ▼
         ┌──────────────┐
         │  Spond API   │
         │  (v2.1)      │
         └──────────────┘
```

### Key Design Principles

- **Dependency Injection Ready**: Optional logger support via `ILogger<SpondClient>`
- **Version Abstraction**: `ICommonData` interface allows support for multiple API versions
- **Strongly Typed Models**: All API responses are deserialized to C# classes
- **Extensibility**: Generic `GetData<T>` method for custom API calls
- **ISO 8601 Compliance**: DateTime handling via extension methods

### Authentication Flow

1. Client sends POST to `/api/2.1/login` with email/phone + password
2. API returns JSON containing `loginToken`
3. Token stored in `Authorization: Bearer {token}` header
4. All subsequent requests use Bearer authentication
5. HttpClient maintains cookies via `CookieContainer`

---

## Project Structure

```
Spond.API/
├── Services/
│   └── SpondClient.cs              # Main client implementation
├── Interfaces/
│   └── ICommonData.cs              # API version interface
├── Models/
│   ├── CommonData.2.1.cs           # API v2.1 implementation
│   ├── SpondEvent.cs               # Event model
│   ├── SpondGroup.cs               # Group model
│   ├── SpondMember.cs              # Member model
│   ├── SpondRole.cs                # Role model
│   ├── SpondSubGroup.cs            # SubGroup model
│   ├── SpondUserProfile.cs         # User profile model
│   ├── SpondEventOwner.cs          # Event owner model
│   └── SpondLoginInformation.cs    # Login response model
├── Extensions/
│   └── DateTimeExtensions.cs       # ISO 8601 conversion utilities
├── Enums.cs                        # Shared enumerations
└── Resources/
    └── spond-icon.png              # Package icon

Spond.API.Test/
└── Program.cs                      # Console test application
```

### Folder Descriptions

#### **Services/**
Contains the main API client implementation. `SpondClient` is the public-facing class that orchestrates HTTP requests, authentication, and data retrieval.

#### **Interfaces/**
Defines contracts for version-specific implementations. `ICommonData` abstracts API endpoint URLs and URL construction logic, enabling support for multiple API versions without changing client code.

#### **Models/**
Contains all data transfer objects (DTOs) representing Spond entities:
- **CommonData.2.1.cs**: Version-specific URL construction and endpoint definitions
- **Spond*.cs files**: Strongly-typed models matching API response structures

#### **Extensions/**
Utility methods extending built-in types. Currently contains `DateTimeExtensions` for ISO 8601 formatting required by the Spond API.

---

## Core Components

### SpondClient Class

The main entry point for all API operations.

**Constructor:**
```csharp
public SpondClient(ICommonData? commonData = null, ILogger<SpondClient>? logger = null)
```

- `commonData`: Optional version configuration (defaults to `CommonData_2_1`)
- `logger`: Optional logger for debugging and error tracking

**Key Fields:**
```csharp
private readonly HttpClient _client;           // Configured with BaseAddress and CookieContainer
private readonly ICommonData _commonData;      // Version-specific configuration
private readonly ILogger<SpondClient>? _logger; // Optional logger
```

**Authentication Methods:**
```csharp
public async Task<bool> LoginWithEmail(string email, string password)
public async Task<bool> LoginWithPhoneNumber(string phoneNumber, string password)
```

Both methods call the internal generic `Login<T>` method which:
1. Posts credentials to `_commonData.LoginUrl`
2. Parses `loginToken` from response
3. Sets `Authorization` header with Bearer token
4. Returns `true` on success, `false` on failure

**Data Retrieval Methods:**

- `GetData<T>(string url)`: Generic method for any API endpoint
- `GetGroups()`: Retrieves all groups
- `GetCurrentUser()`: Retrieves current user profile
- `GetEvents(...)`: Multiple overloads for flexible event querying

### ICommonData Interface and CommonData_2_1 Implementation

**ICommonData** defines the contract for API version implementations:

```csharp
public interface ICommonData
{
    string LoginTokenPropertyName { get; }  // Property name in login response
    string BaseUrl { get; }                 // API base URL
    string LoginUrl { get; }                // Authentication endpoint
    string UserUrl { get; }                 // User profile endpoint
    string GroupsUrl { get; }               // Groups endpoint
    
    // URL construction methods for events
    string GetEventsUrl(...);
}
```

**CommonData_2_1** implements this for Spond API v2.1:

```csharp
internal class CommonData_2_1 : ICommonData
{
    public string LoginTokenPropertyName => "loginToken";
    public string BaseUrl => "https://spond.com/";
    public string LoginUrl => "/api/2.1/login";
    public string UserUrl => "/api/2.1/profile";
    public string GroupsUrl => "/api/2.1/groups";
    
    public string GetEventsUrl(...)
    {
        // Constructs: /api/2.1/sponds?minEndTimestamp=...&maxEndTimestamp=...
    }
}
```

**URL Construction Logic:**

The `GetEventsUrl` methods build query strings with:
- `minEndTimestamp` / `maxEndTimestamp`: ISO 8601 formatted with milliseconds
- `max`: Number of results (auto-calculated: ~5 events per day in range)
- `order`: "asc" or "desc"
- Optional filters: `groupId`, `subGroupId`, `includeComments`, etc.

Example URL:
```
/api/2.1/sponds?minEndTimestamp=2024-01-01T00:00:00.000Z&maxEndTimestamp=2024-01-31T23:59:59.999Z&max=155&order=asc&groupId=abc123
```

### Model Classes

All models are plain C# classes with properties matching API responses:

**SpondGroup:**
```csharp
public class SpondGroup
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<SpondMember> Members { get; set; }
    public List<SpondSubGroup> SubGroups { get; set; }
    public List<SpondRole> Roles { get; set; }
    // ... other properties
}
```

**SpondEvent:**
- Uses `StartTimestamp` and `EndTimestamp` for JSON serialization
- Exposes `StartTime` and `EndTime` as `DateTime` properties
- Computed properties: `AcceptedOwners`, `AcceptedMembers`

**SpondMember:**
- Represents both full members and guardians
- `Birthday` computed from `DateOfBirth` string
- Recursive `Guardians` list for parent relationships

**Design Patterns:**
- **Newtonsoft.Json** for deserialization (configured in `GetData<T>`)
- Empty collections default to `[]` (C# 12 collection expressions)
- Nullable reference types enabled for null-safety

### DateTimeExtensions

Handles ISO 8601 format conversion required by Spond API.

```csharp
public static class DateTimeExtensions
{
    // Converts DateTime to ISO 8601 string
    public static string ToIso8601(this DateTime dateTime, bool useMilliseconds)
    // Parses ISO 8601 string to DateTime
    public static DateTime FromIso8601(string input, bool useMilliseconds)
}
```

**Formats:**
- Without milliseconds: `"yyyy-MM-dd'T'HH:mm:ss'Z'"`
- With milliseconds: `"yyyy-MM-dd'T'HH:mm:ss.fff'Z'"`

**Usage in API:**
- Event URLs use milliseconds: `minEndTime.ToIso8601(true)`
- Event timestamp properties use format without milliseconds
- All times converted to UTC for transmission, LocalTime for consumption

### Enums

**Order:** Sort direction for queries
```csharp
public enum Order { Ascending, Descending }
```

**EventVisibility:** Event privacy settings
```csharp
public enum EventVisibility { Undefined, Invitees }
```

**Permission:** Group role permissions with JSON serialization
```csharp
[JsonConverter(typeof(StringEnumConverter))]
public enum Permission 
{ 
    Members, Admins, Settings, Events, Posts, Polls, 
    Payments, Chat, Files, Fundraisers, 
    [EnumMember(Value = "coaches-corner")] CoachesCorner 
}
```

Note: `Permission` uses `StringEnumConverter` for proper JSON mapping.

---

## API Version Support

### Current Version: 2.1

The library currently supports Spond API version 2.1 through the `CommonData_2_1` class.

### Adding New API Versions

To support a new API version (e.g., 2.2):

1. **Create new implementation:**
```csharp
internal class CommonData_2_2 : ICommonData
{
    public string LoginTokenPropertyName => "token"; // May change
    public string BaseUrl => "https://spond.com/";
    public string LoginUrl => "/api/2.2/auth";       // New endpoint
    // ... implement all interface members
}
```

2. **Update SpondClient constructor:**
```csharp
public SpondClient(ICommonData? commonData = null, ILogger<SpondClient>? logger = null)
{
    _commonData = commonData ?? new CommonData_2_2(); // New default
    // ...
}
```

3. **Handle breaking changes:**
- Update model classes if response structures change
- Add version-specific model classes if needed
- Update XML documentation

4. **Test compatibility:**
- Ensure all existing tests pass
- Add version-specific tests if behavior differs

---

## Building the Project

### Prerequisites

- **.NET 10.0 SDK** or later
- Visual Studio 2022+ or JetBrains Rider (optional)

### Build Commands

**Restore dependencies:**
```bash
dotnet restore
```

**Build the library:**
```bash
dotnet build
```

**Build in Release mode:**
```bash
dotnet build --configuration Release
```

**Create NuGet package:**
```bash
dotnet pack --configuration Release
```

### Project Configurations

The `.csproj` file configures:

```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <Nullable>enable</Nullable>                    <!-- Null reference types -->
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <GeneratePackageOnBuild>True</GeneratePackageOnBuild>
  <DocumentationFile>bin\$(Configuration)\$(TargetFramework)\Spond.API.xml</DocumentationFile>
</PropertyGroup>
```

**NuGet Package Configuration:**
- **Package ID:** Spond.API
- **Authors:** Tobias Schälte
- **License:** MIT
- **Icon:** Resources/spond-icon.png
- **Readme:** Included from ../README.md

### Dependencies

```xml
<PackageReference Include="Microsoft.Extensions.Logging" Version="10.0.2" />
<PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
```

- **Microsoft.Extensions.Logging:** Optional logging infrastructure
- **Newtonsoft.Json:** JSON serialization/deserialization

---

## Testing

### Test Project Structure

The `Spond.API.Test` project is a console application that provides interactive testing:

```
Spond.API.Test/
├── Program.cs                  # Interactive test console
└── Spond.API.Test.csproj      # Test project file
```

### Running Tests

**Run the test application:**
```bash
cd Spond.API.Test
dotnet run
```

**With command-line arguments:**
```bash
dotnet run -- "email@example.com" "password"
```

### Test Application Features

The console app provides an interactive menu to:
1. Print all events across all groups
2. Print all groups the user belongs to
3. Print events for a specific group
4. Print events for a specific subgroup

**Code Flow:**
1. Authenticate with email or phone number
2. Retrieve current user profile
3. Display menu options
4. Execute selected operation
5. Display formatted results

### Manual Testing Scenarios

**Authentication Testing:**
- Email login with valid/invalid credentials
- Phone number login with valid/invalid credentials
- Error handling for network failures

**Data Retrieval Testing:**
- Groups with/without subgroups
- Events with various filter combinations
- Date range edge cases (same day, cross-month, etc.)
- Event response acceptance tracking

**Integration Testing:**
- Test against live Spond API (requires valid credentials)
- Verify model deserialization with real data
- Check DateTime conversion accuracy across timezones

### Automated Testing Recommendations

For contributors adding automated tests:

**Unit Tests:**
- URL construction in `CommonData_2_1`
- DateTime extension methods
- Model property calculations (e.g., `Birthday`, `AcceptedMembers`)

**Integration Tests:**
- Mock HttpClient with example responses
- Verify Bearer token header attachment
- Test deserialization of complex nested objects

**Tools:**
- xUnit or NUnit for test framework
- Moq for mocking HttpClient
- FluentAssertions for readable assertions

---

## XML Documentation

### Generation

XML documentation is automatically generated during build:

```xml
<GenerateDocumentationFile>true</GenerateDocumentationFile>
<DocumentationFile>bin\$(Configuration)\$(TargetFramework)\Spond.API.xml</DocumentationFile>
```

Output: `bin/Release/net10.0/Spond.API.xml`

### Documentation Standards

All public types and members must include:

**Classes:**
```csharp
/// <summary>
/// Brief description of the class purpose.
/// </summary>
public class ClassName
```

**Methods:**
```csharp
/// <summary>
/// Description of what the method does.
/// </summary>
/// <param name="paramName">Description of the parameter.</param>
/// <returns>Description of the return value.</returns>
public ReturnType MethodName(ParamType paramName)
```

**Properties:**
```csharp
/// <summary>
/// Description of the property purpose and value.
/// </summary>
public string PropertyName { get; set; }
```

**Enums:**
```csharp
/// <summary>
/// Description of the enumeration purpose.
/// </summary>
public enum EnumName
{
    /// <summary>
    /// Description of this enum value.
    /// </summary>
    Value1,
}
```

### IntelliSense Support

When the package is installed via NuGet, the XML file provides:
- Method signature tooltips
- Parameter descriptions
- Return value information
- Usage examples (via `<example>` tags)

### Documentation Testing

Verify documentation quality:

```bash
# Check for missing XML comments (requires StyleCop.Analyzers or similar)
dotnet build /p:TreatWarningsAsErrors=true /p:WarningsAsErrors=CS1591
```

CS1591: Missing XML comment for publicly visible type or member

---

## Contributing Guidelines

### Getting Started

1. **Fork the repository** on GitHub
2. **Clone your fork:**
   ```bash
   git clone https://github.com/YOUR_USERNAME/Spond.API.git
   cd Spond.API
   ```
3. **Create a feature branch:**
   ```bash
   git checkout -b feature/your-feature-name
   ```

### Development Workflow

1. **Make your changes** in the appropriate directory
2. **Add XML documentation** for public APIs
3. **Test your changes:**
   ```bash
   dotnet build
   cd Spond.API.Test
   dotnet run
   ```
4. **Commit with descriptive messages:**
   ```bash
   git commit -m "Add support for retrieving event details"
   ```
5. **Push to your fork:**
   ```bash
   git push origin feature/your-feature-name
   ```
6. **Create a Pull Request** on GitHub

### Pull Request Guidelines

**PR Title Format:**
- `feat: Add new endpoint for X`
- `fix: Correct DateTime parsing issue`
- `docs: Update developer guide`
- `refactor: Simplify URL construction`

**PR Description Should Include:**
- Summary of changes
- Motivation and context
- Breaking changes (if any)
- Testing performed
- Related issue numbers

**Before Submitting:**
- [ ] Code builds without errors
- [ ] All public APIs have XML documentation
- [ ] Changes tested with real API (if applicable)
- [ ] No breaking changes (or clearly documented)
- [ ] Updated README or docs if needed

### Code Review Process

1. Maintainers review code for quality and design
2. Automated builds run via GitHub Actions
3. Requested changes addressed in follow-up commits
4. Approval required before merge
5. Squash merge to main branch

---

## Code Style and Standards

### C# Style Guidelines

**Naming Conventions:**
- Classes, methods, properties: `PascalCase`
- Private fields: `_camelCase` with underscore prefix
- Parameters, local variables: `camelCase`
- Constants: `PascalCase`
- Interfaces: `IPascalCase` (prefix with I)

**Code Organization:**
```csharp
namespace Spond.API.Services;  // File-scoped namespace

/// <summary>...</summary>
public class ClassName
{
    // 1. Private fields
    private readonly HttpClient _client;
    
    // 2. Constructors
    public ClassName() { }
    
    // 3. Public properties
    public string Property { get; set; }
    
    // 4. Public methods
    public void PublicMethod() { }
    
    // 5. Private methods
    private void PrivateMethod() { }
}
```

**Modern C# Features:**
- File-scoped namespaces: `namespace Spond.API;`
- Null-coalescing: `var data = await GetData() ?? [];`
- Collection expressions: `new List<string> { }` → `[]`
- Pattern matching: `order switch { ... }`
- String interpolation: `$"Value: {variable}"`

**Nullable Reference Types:**
- Enabled project-wide: `<Nullable>enable</Nullable>`
- Use `?` suffix for nullable: `string? optionalValue`
- Use `!` operator carefully when null-checked: `value!.Property`

### Formatting

**Indentation:**
- 4 spaces (no tabs)
- Braces on new line

**Line Length:**
- Aim for 120 characters maximum
- Break long method signatures across lines

**Spacing:**
```csharp
// Good
public async Task<bool> MethodName(string param1, int param2)
{
    var result = await CallMethod(param1, param2);
    return result is not null;
}

// Avoid
public async Task<bool> MethodName(string param1,int param2){
    var result=await CallMethod(param1,param2);
    return result is not null;}
```

### Best Practices

**Async/Await:**
- Always use `async/await` for I/O operations
- Suffix async methods with `Async` (e.g., `GetDataAsync`) - *current code may not follow this*
- Return `Task<T>` or `Task` for async methods

**Error Handling:**
```csharp
// Log errors, don't throw exceptions for API failures
if (!response.IsSuccessStatusCode)
{
    _logger?.LogError($"Error: {response.StatusCode}");
    return null;  // or default value
}
```

**Dependency Injection:**
- Accept dependencies in constructor
- Store as `private readonly` fields
- Use interfaces for abstraction

**Immutability:**
- Use `readonly` for fields that don't change
- Consider init-only setters: `public string Name { get; init; }`
- Make collections read-only when appropriate

---

## Release Process

### Version Numbering

Follow **Semantic Versioning (SemVer)**:
- **Major (X.0.0):** Breaking changes
- **Minor (1.X.0):** New features, backward compatible
- **Patch (1.0.X):** Bug fixes, backward compatible

### Pre-Release Checklist

- [ ] All tests pass
- [ ] XML documentation complete
- [ ] README.md updated
- [ ] CHANGELOG updated (if maintained)
- [ ] Version number incremented in `.csproj`
- [ ] Release notes prepared

### GitHub Actions CI/CD

**Build and Test Workflow:**
```yaml
name: Build and Test .NET
on:
  push:
    branches: [ "main" ]
  pull_request:
    branches: [ "main" ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v4
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: 10.0.x
    - name: Restore dependencies
      run: dotnet restore
    - name: Build
      run: dotnet build --no-restore
    - name: Test
      run: dotnet test --no-build --verbosity normal
```

**Publishing Workflow:**
The `publish.yml` workflow handles NuGet package publishing (check `.github/workflows/publish.yml` for details).

### Manual Release Steps

1. **Update version in `.csproj`:**
   ```xml
   <Version>1.2.0</Version>
   ```

2. **Build release package:**
   ```bash
   dotnet pack --configuration Release
   ```

3. **Test package locally:**
   ```bash
   dotnet nuget push bin/Release/Spond.API.1.2.0.nupkg --source local-feed
   ```

4. **Create GitHub release:**
   - Tag: `v1.2.0`
   - Title: `Release 1.2.0`
   - Description: Release notes

5. **Publish to NuGet.org:**
   ```bash
   dotnet nuget push bin/Release/Spond.API.1.2.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
   ```

### Post-Release

- Monitor NuGet.org for successful publication
- Update GitHub issues/discussions
- Announce on relevant channels

---

## Extending the API

### Adding New Endpoints

**1. Add URL to ICommonData:**
```csharp
public interface ICommonData
{
    // ... existing members
    string MessagesUrl { get; }  // New endpoint
}
```

**2. Implement in CommonData_2_1:**
```csharp
public string MessagesUrl => "/api/2.1/messages";
```

**3. Create Model Class:**
```csharp
namespace Spond.API.Models;

/// <summary>
/// Represents a message in the Spond system.
/// </summary>
public class SpondMessage
{
    /// <summary>
    /// The unique identifier of the message.
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// The message text content.
    /// </summary>
    public string Text { get; set; } = string.Empty;
    
    /// <summary>
    /// The timestamp when the message was sent.
    /// </summary>
    public DateTime Timestamp { get; set; }
}
```

**4. Add Method to SpondClient:**
```csharp
/// <summary>
/// Retrieves all messages for the authenticated user.
/// </summary>
/// <returns>A list of <see cref="SpondMessage"/> objects, or an empty list if none found.</returns>
public async Task<List<SpondMessage>> GetMessages()
{
    return await GetData<List<SpondMessage>>(_commonData.MessagesUrl) ?? [];
}
```

**5. Test the Implementation:**
```csharp
var client = new SpondClient();
await client.LoginWithEmail("test@example.com", "password");
var messages = await client.GetMessages();
foreach (var message in messages)
{
    Console.WriteLine($"{message.Timestamp}: {message.Text}");
}
```

### Adding Complex Endpoints with Parameters

For endpoints requiring query parameters:

**1. Add method to ICommonData:**
```csharp
string GetMessagesUrl(DateTime since, int max);
```

**2. Implement URL construction:**
```csharp
public string GetMessagesUrl(DateTime since, int max)
{
    return $"/api/2.1/messages?since={since.ToIso8601(true)}&max={max}";
}
```

**3. Add overload to SpondClient:**
```csharp
/// <summary>
/// Retrieves messages since a specific time.
/// </summary>
/// <param name="since">The minimum timestamp for messages.</param>
/// <param name="max">Maximum number of messages to retrieve.</param>
/// <returns>A list of <see cref="SpondMessage"/> objects.</returns>
public async Task<List<SpondMessage>> GetMessages(DateTime since, int max = 100)
{
    return await GetData<List<SpondMessage>>(_commonData.GetMessagesUrl(since, max)) ?? [];
}
```

### Using GetData for Quick Prototyping

For testing new endpoints without formal integration:

```csharp
var client = new SpondClient();
await client.LoginWithEmail("test@example.com", "password");

// Direct API call
var response = await client.GetData<JsonElement>("/api/2.1/experimental");
Console.WriteLine(response.GetRawText());

// With custom model
public class ExperimentalData 
{ 
    public string Value { get; set; } = string.Empty; 
}

var data = await client.GetData<ExperimentalData>("/api/2.1/experimental");
```

### POST/PUT/DELETE Operations

Currently, the library only supports GET operations. To add mutation support:

**1. Add generic POST method:**
```csharp
/// <summary>
/// Posts data to the Spond API.
/// </summary>
/// <typeparam name="TRequest">The type of data to send.</typeparam>
/// <typeparam name="TResponse">The type of data expected in response.</typeparam>
/// <param name="url">The API endpoint URL.</param>
/// <param name="data">The data to post.</param>
/// <returns>The deserialized response, or null if the request failed.</returns>
public async Task<TResponse?> PostData<TRequest, TResponse>(string url, TRequest data) 
    where TResponse : class
{
    var response = await _client.PostAsJsonAsync(url, data);
    if (!response.IsSuccessStatusCode) return null;
    var json = await response.Content.ReadAsStringAsync();
    return JsonConvert.DeserializeObject<TResponse>(json);
}
```

**2. Use in specific methods:**
```csharp
public async Task<bool> SendMessage(string groupId, string text)
{
    var payload = new { groupId, text };
    var result = await PostData<object, SpondMessage>("/api/2.1/messages", payload);
    return result is not null;
}
```

### Debugging API Calls

**Enable logging:**
```csharp
var services = new ServiceCollection();
services.AddLogging(builder => 
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Debug);
});

var serviceProvider = services.BuildServiceProvider();
var logger = serviceProvider.GetService<ILogger<SpondClient>>();

var client = new SpondClient(logger: logger);
```

**Inspect raw responses:**
```csharp
public async Task<string?> GetRawJson(string url)
{
    var response = await _client.GetAsync(url);
    if (!response.IsSuccessStatusCode) return null;
    return await response.Content.ReadAsStringAsync();
}
```

**Monitor HTTP traffic:**
- Use tools like Fiddler or Postman
- Examine the Spond web app's Network tab in browser DevTools
- Reference unofficial API documentation (if available)

### Understanding the Spond API

Since the Spond API is not officially documented:

1. **Reverse Engineering:**
   - Use browser DevTools (F12) on spond.com
   - Monitor Network tab during actions
   - Analyze request/response structures

2. **Common Patterns:**
   - Most endpoints under `/api/2.1/`
   - Timestamps in ISO 8601 format
   - Bearer token authentication
   - JSON request/response bodies

3. **Error Handling:**
   - HTTP 401: Invalid/expired token (re-authenticate)
   - HTTP 404: Invalid endpoint or resource
   - HTTP 429: Rate limiting (implement backoff)

4. **Rate Limiting:**
   - Not officially documented
   - Implement exponential backoff if needed
   - Cache responses when appropriate

---

## Additional Resources

- **GitHub Repository:** https://github.com/ArizonaGreenTea05/Spond.API
- **NuGet Package:** https://www.nuget.org/packages/Spond.API
- **User Guide:** [docs/user-guide.md](user-guide.md)
- **Spond Website:** https://spond.com

## Questions and Support

For questions or issues:
1. Check existing [GitHub Issues](https://github.com/ArizonaGreenTea05/Spond.API/issues)
2. Create a new issue with detailed description
3. Join discussions in the repository

---

**Happy coding! 🚀**
