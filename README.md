# NoSQLite

## Install

[![Nuget](https://img.shields.io/nuget/v/NoSQLite?label=nuget&style=for-the-badge)](https://www.nuget.org/packages/NoSQLite/)

## Usage

```
public class User
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public string[]? Tags { get; set; }
}

var db = IDatabase.Create("account.db");
var users = db.GetOrCreateCollection<User>();
var doc = new NewDocument<User>
{
    Data = new User
    {
        Name = "alex",
        Age = 23,
        Tags = ["a", "b"]
    }
};
await users.AddAsync(doc);
var docOnDb = await users.GetRequiredByIdAsync(doc.Id);
```