# Fluent Sql Builder

Welcome to the readme of Fluent Sql Builder. This is a library that allows developers to build a Sql Command or Sql Statement with Parameters using a strongly typed system similiar to LINQ without the hassle of creating/maintaining a db context.

## Example:
All values supplied will be transformed to parameters. Therefore you won't have to worry about SQL Injection.
```cs
using (var connection = new SqlConnection())
{
    var command = connection.Build()
                    .From<Person>()
                    .Join<Work>((person,work) => person.WorkId == work.WorkId)
                    .Join<Person>((person, work,person2)=> person.Id == person2.Id)
                    .Where((person, work, person2) => person2.Age.In(new int[] { 1, 2 }))
                    .Select((person, work, person2) => new { person.Id, person.Name })
                    .ToSqlCommand();
}
```
> SELECT [Person].[Id], [Person].[Name] FROM [Person] JOIN [Work] AS [Work] ON [Person].[WorkId] = [Work].[WorkId] JOIN [Person] AS [Person1] ON [Person].[Id] = [Person1].[Id] WHERE [Person1].[Age] IN (@P1,@P2)

Currently it only supports SQL Server. All the functions can be concatenated with eachother to freely buld your own query.

You can install Sql Builder through nuget.

## How to use Fluent Sql Builder?

Building a query always starts with the From<T>() function and ends with the Select() function similiar to LINQ.

### Basic Select query.
```cs
using (var connection = new SqlConnection())
{
    var command = connection.Build()
                    .From<Person>()
                    .Select(person => new { person.Id })
                    .ToSqlCommand();
}
```
> SELECT [Person].[Id] FROM [Person]


You can add more columns to the select function by simply expanding the anonymous object within the Select function.

```cs
using (var connection = new SqlConnection())
{
    var command = connection.Build()
                    .From<Person>()
                    .Select(person => new { person.Id, person.Name })
                    .ToSqlCommand();
}
```
> SELECT [Person].[Id], [Person].[Name] FROM [Person]


### Adding a Where Clause.
Parameters used within the where clause will automatically be added to the command with its appropriate value to prevent SQL Injection.
```cs
using (var connection = new SqlConnection())
{
    var command = connection.Build()
                    .From<Person>()
                    .Where((person) => person.Salary > 5000)
                    .Select(person => new { person.Id, person.Name })
                    .ToSqlCommand();
}
```
> SELECT [Person].[Id], [Person].[Name] FROM [Person] WHERE [Person].[Salary] > @P1


There's also support for In, Not In, Like, Not Like, Between, Not Between
```cs
using (var connection = new SqlConnection())
{
    IEnumerable<int> ages = new int[] { 1, 2 };
    var command = connection.Build()
                    .From<Person>()
                    .Where((person) => person.Age.In(ages))
                    .Select(person => new { person.Id, person.Name })
                    .ToSqlCommand();
}
```
> SELECT [Person].[Id], [Person].[Name] FROM [Person] WHERE [Person].[Age] IN (@P1,@P2)


```cs
using (var connection = new SqlConnection())
{
    var command = connection.Build()
                    .From<Person>()
                    .Where((person) => person.Name.Like("base%"))
                    .Select(person => new { person.Id, person.Name })
                    .ToSqlCommand();
}
```
> SELECT [Person].[Id], [Person].[Name] FROM [Person] WHERE [Person].[Age] LIKE @P1


```cs
using (var connection = new SqlConnection())
{
    var command = connection.Build()
                    .From<Person>()
                    .Where((person) => person.Age.Between(1,2))
                    .Select(person => new { person.Id, person.Name })
                    .ToSqlCommand();
}
```
> SELECT [Person].[Id], [Person].[Name] FROM [Person] WHERE [Person].[Age] BETWEEN @P1 AND @P2

### Joining other tables
```cs
using (var connection = new SqlConnection())
{
    var command = connection.Build()
                    .From<Person>()
                    .Join<Work>((person,work) => person.WorkId == work.WorkId)
                    .Select((person, work) => new { person.Id, person.Name, work.Address })
                    .ToSqlCommand();
}
```
> SELECT [Person].[Id], [Person].[Name], [Work].[Address] FROM [Person] JOIN [Work] AS [Work] ON [Person].[WorkId] = [Work].[WorkId]


There's also support for self-joining
```cs
using (var connection = new SqlConnection())
{
    var command = connection.Build()
                    .From<Person>()
                    .Join<Person>((person, person2) => person.WorkId == person2.WorkId)
                    .Select((person, person2) => new { person.Id, person2.Name })
                    .ToSqlCommand();
}
```
> SELECT [Person].[Id], [Person1].[Name] FROM [Person] JOIN [Person] AS [Person1] ON [Person].[WorkId] = [Person1].[WorkId]


You can join multiple tables aswell by using the Join function over and over.
```cs
using (var connection = new SqlConnection())
{
    var command = connection.Build()
                    .From<Person>()
                    .Join<Person>((person, person2) => person.WorkId == person2.WorkId)
                    .Join<Person>((person, person2,person3) => person.Name == person3.Name)
                    .Select((person, person2, person3) => new { person.Id, person2.Name , person3.Age })
                    .ToSqlCommand();
}
```
> SELECT [Person].[Id], [Person1].[Name], [Person2].[Age] FROM [Person] JOIN [Person] AS [Person1] ON [Person].[WorkId] = [Person1].[WorkId] JOIN [Person] AS [Person2] ON [Person].[Name] = [Person2].[Name]


### Adding a Group By Clause

```cs
using (var connection = new SqlConnection())
{
    var command = connection.Build()
                    .From<Person>()
                    .GroupBy((person) => new { person.Age })
                    .Select(person => new { person.Id, person.Name })
                    .ToSqlCommand();
}
```
> SELECT [Person].[Id], [Person].[Name] FROM [Person] GROUP BY [Person].[Age]


There's also support to do Group By on multiple columns.
```cs
using (var connection = new SqlConnection())
{
    var command = connection.Build()
                    .From<Person>()
                    .GroupBy((person) => new { person.Age , person.Name })
                    .Select(person => new { person.Id, person.Name })
                    .ToSqlCommand();
}
```
> SELECT [Person].[Id], [Person].[Name] FROM [Person] GROUP BY [Person].[Age], [Person].[Name]


### Adding an Order By Clause
```cs
using (var connection = new SqlConnection())
{
    var command = connection.Build()
                    .From<Person>()
                    .GroupBy((person) => new { person.Age , person.Name })
                    .Select(person => new { person.Id, person.Name })
                    .ToSqlCommand();
}
```
> SELECT [Person].[Id], [Person].[Name] FROM [Person] ORDER BY [Person].[Age]


There's also support to do Order By on multiple columns.
```cs
using (var connection = new SqlConnection())
{
    var command = connection.Build()
                    .From<Person>()
                    .OrderBy((person) => new { person.Age , person.Salary })
                    .Select(person => new { person.Id, person.Name })
                    .ToSqlCommand();
}
```
> SELECT [Person].[Id], [Person].[Name] FROM [Person] ORDER BY [Person].[Age], [Person].[Salary]
