# NoSQLite

## Install

[![Nuget](https://img.shields.io/nuget/v/NoSQLite?label=nuget&style=for-the-badge)](https://www.nuget.org/packages/NoSQLite/)

## Usage

```csharp
public class User
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public string[]? Tags { get; set; }
}

var db = Database.Create("account.db");
var users = db.GetOrCreateCollection<User>();

// Create 
var user= await users.AddAsync(new User
{
    Name = "alex",
    Age = 23,
    Tags = ["a", "b"]
});

// Update
user.Data.Age = 32;
users.UpdateAsync(user);

// Get
user = await users.GetByIdAsync(user.Id);

// Query
var filteredUsers= users.Query.Where("$.Age", Comparison.Greater, 20).TakeAsync();

///Delete
users.Delete(user.Id)
```