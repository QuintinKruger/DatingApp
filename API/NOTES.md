# Enity Framework 
An ORM with the following features:
1. Querying 
2. Change Tracking
3. Saving 
4. Concurrency 
5. Transactions 
6. Caching 
7. Built-in conventions 
8. Configurations 
9. Migrations - create DB using C# classes

## Packages Required
To make use of migrations, the `Microsoft.EntityFrameworkCore.Design` NuGet package is required, otherwise to simply use the ORM, install the `Microsoft.EntityFrameworkCore.<Provider>` for the database provider of choice 

## `dotnet-ef` tool 
The command line tool `dotnet-ef` can be used for a migrations, to manage Db contexts as well as the database itself 
``` bash
dotnet tool install -g dotnet-ef
```

## Running Migrations 
Create a new migration with a specific name `InitialCreate` and store it in `Data/Migrations`:
```bash
dotnet ef migrations add InitialCreate -o Data/Migrations
```
This created 3 new files in [DataMigrations](./API/Data/). Only one of note is [20261016095031_InitialCreate.cs](./API/Data/Migrations/20261016095031_InitialCreate.cs) which contains 2 methods:
1. Up - executed when migration is applied to DB
2. Down - used to perform a rollback of this migration

With the migration created we can apply them to the database using the command 
``` bash
dotnet ef database update
```

Alternatively, simly starting the application will also perform the migration

To check whether there are migrations needed we can run the following 
``` bash
dotnet ef migrations has-pending-model-changes
```

## Navigation Properties
Navigations are layered over foreign keys - meaning this is the abstraction EF uses to provide simple classes for relationships that exist between entities

Reference navigations and collection navigations represent respectively the one side and many side to one-to-many and many-to-one relationships in a DB 

see [Navigations](https://learn.microsoft.com/en-us/ef/core/modeling/relationships/navigations) for more.

see [Member.cs](./Entities/Member.cs#25) for code snippet of how a navigation is setup

A navigation property is typed as another entity, while its foreign key is a plain scalar (e.g. `string`) stored as a real column. EF pairs them by naming convention - a property named `<NavigationName>Id` (or `<PrincipalTypeName>Id`) whose type matches the principal's key becomes the foreign key, with no configuration needed. If the foreign key property is left out, EF creates it as a shadow property (a column with no matching class property); declaring it lets you set or read the relationship without loading the related entity. A non-nullable foreign key makes the relationship required, which defaults to cascade delete.

see [Photo.cs](./Entities/Photo.cs#13) for a foreign key property paired with its navigation

## Deferred Execution (Pagination Implementation)
Building up a tree of expressions of type `IQueryable`:
```csharp
IQueryable<User> query = context.Users
                .Where(x => x.Gender == gender)
                .Take(5) // for pagination
                .Skip(5) // for pagination
                .AsQueryable() // optional - this is explicit however
```

This query is not executed - hence a deferred execution. Only once we we call an execution on the query like `query.ToListAsync()`, `query.ToArrayAsync()`, `query.ToDictionary()`, `query.Count()`

This can be used to implement pagination:

# `dotnet` Commands 
`dotnet restore` - restore packages according to your csproj file (project file)

# Middleware 
We can provide middleware through which requests will pass before our app services it. For more on middleware and how it flows through our application see [.NET Middleware](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-10.0).

We can define our own middleware as has been in [ExceptionMiddleware.cs](./Middleware/ExceptionMiddleware.cs) and we can hook it up to our ASP.NET application in [Program.cs](./Program.cs#46) - in this case, provided as the first middleware component as we want it to apply to everything in our code base. 

# Extensions 


# FAQ
## What is the `!` called when assigning `null!` to a non-nullable property?
It's the null-forgiving operator (also called the "dammit" or "bang" operator). With nullable reference types enabled, it tells the compiler to treat the value as non-null and suppresses the warning - it has no effect at runtime, so a real `null` will still throw a `NullReferenceException` when used.
``` csharp
public string Name { get; set; } = null!;
```
