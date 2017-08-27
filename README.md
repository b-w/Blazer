# Blazer

Blazer is a tiny, high-performance object mapper that serves as an extension to ADO.NET.

You can get it on [NuGet](https://www.nuget.org/packages/Blazer/).

Blazer's API consists of a couple of useful extension methods on `System.Data`'s provider-agnostic `IDbConnection` interface. These take care of a lot of the boilerplate code that's normally involved when using ADO.NET directly.

So, instead of doing stuff like this:

```csharp
using (var cmd = m_conn.CreateCommand())
{
    cmd.CommandText = "SELECT * FROM [Production].[TransactionHistory] WHERE [TransactionID] = @Id";

    var dbParam = cmd.CreateParameter();
    dbParam.Direction = ParameterDirection.Input;
    dbParam.DbType = DbType.Int32;
    dbParam.ParameterName = "@Id";
    dbParam.Value = 42;
    cmd.Parameters.Add(dbParam);

    using (var reader = cmd.ExecuteReader())
    {
        if (reader.Read())
        {
            var transaction = new TransactionHistory();

            transaction.TransactionID = reader.GetInt32(0);
            transaction.ProductID = reader.GetInt32(1);
            transaction.ReferenceOrderID = reader.GetInt32(2);
            transaction.ReferenceOrderLineID = reader.GetInt32(3);
            transaction.TransactionDate = reader.GetDateTime(4);
            transaction.TransactionType = reader.GetString(5);
            transaction.Quantity = reader.GetInt32(6);
            transaction.ActualCost = reader.GetDecimal(7);
            transaction.ModifiedDate = reader.GetDateTime(8);

            return transaction;
        }
        else
        {
            return null;
        }
    }
}
```

...Blazer lets you do this:

```csharp
return m_conn.QuerySingle<TransactionHistory>(
      "SELECT * FROM [Production].[TransactionHistory] WHERE [TransactionID] = @Id",
      new { Id = 42 }
    );
```

Or, if your project is `>= .Net 4.6` or `>= .Net Standard 1.3`, you can use string interpolation to do this:

```csharp
return m_conn.QuerySingle<TransactionHistory>(
      $"SELECT * FROM [Production].[TransactionHistory] WHERE [TransactionID] = {42}"
    );
```

## Design Goals

Blazer's primary features are:

1. **Speed**. Blazer's performance is on level with that of hand-crafted ADO.NET. (also see *Performance* section)
2. **Simplicity**. Blazer has a minimal API and does exactly what you would expect. Nothing more, nothing less. (also see *API Functions* section)
3. **Lightweight**. One assembly. One namespace. No external dependencies.
4. **Provider-agnostic**. There is no separate code for SQL Server, SQLite, MySql, etc... As long as there is an ADO.NET provider for it, Blazer just works.
5. **Configuration-less**. Blazer requires zero configuration and does not depend on any pre-generated code or models.

There are also a lot of things that Blazer explicitly *does not* do:

1. Blazer will not write your SQL for you.
2. Blazer will not create, migrate, or otherwise manage your database.
3. Blazer will not generate code from your database schema, or vice versa.
4. Blazer will not perform change tracking on loaded entities.
5. Blazer will not load anything that's not a flat object (i.e. no child entities, collections, relations, etc).
6. Blazer will never threaten to stab you and, in fact, cannot speak.

In short, **Blazer is not an ORM**. It lacks a lot of fancy features that come standard with libraries like Entity Framework. But the things it does do, it aims to do really well.

And while it's not a fully-fledged Object *Relational* Mapper, Blazer does perform convention-based object mapping when loading query results. See the *Mapping conventions* section for more on this.

## Quickstart

1. Reference `Blazer.dll` (get it on [NuGet](https://www.nuget.org/packages/Blazer/)).
2. Import the `Blazer` namespace.
3. Open an `IDbConnection`.
4. Use the Blazer extension methods on `IDbConnection`.

Minimal code example: (uses the [Adventure Works 2014 database](https://msftdbprodsamples.codeplex.com/releases/view/125550))

```csharp
namespace Quickstart
{
    using System.Data;
    using System.Data.SqlClient;
    using Blazer;
    
    class Program
    {
        static void Main(string[] args)
        {
            dynamic product;
            using (IDbConnection conn = new SqlConnection(@"Server=.\LOCALSQL;Database=AdventureWorks;Integrated Security=True"))
            {
                conn.Open();
                product = conn.QuerySingle("SELECT * FROM [Production].[Product] WHERE [ProductID] = @Id", new { Id = 328 });
                conn.Close();
            }
            Console.WriteLine(product.Name);
        }
    }
}
```

## API Functions

Here's all the different things Blazer can do for you.

### Querying

For typed query results, use:

```csharp
IEnumerable<T> IDbConnection.Query<T>(string command, object parameters = null, CommandConfiguration config = null) where T : new()
```

For dynamic query results, use:

```csharp
IEnumerable<dynamic> IDbConnection.Query(string command, object parameters = null, CommandConfiguration config = null)
```

For retrieving single results, use:

```csharp
T IDbConnection.QuerySingle<T>(string command, object parameters = null, CommandConfiguration config = null) where T : new()
```

and:

```csharp
dynamic IDbConnection.QuerySingle(string command, object parameters = null, CommandConfiguration config = null)
```

### Scalar queries

To execute a scalar query, use:

```csharp
T IDbConnection.Scalar<T>(string command, object parameters = null, CommandConfiguration config = null)
```

Note that the singular result of the scalar query is casted directly to `T`.

### Commands / Non-queries

To execute a command (or non-query), use:

```csharp
int IDbConnection.Command(string command, object parameters = null, CommandConfiguration config = null)
```

### Stored procedures

A stored procedure can be executed in two different ways:

1. As a regular query using the `CommandType.StoredProcedure` option.
2. As a non-query using input/output parameters.

For the second option, use:

```csharp
void IDbConnection.StoredProcedure(string command, SpParameters parameters = null, CommandConfiguration config = null)
```

The `SpParameters` object contains all input-, output-, and return parameters.

To add an input parameter, use:

```csharp
void SpParameters.AddInput(string name, object value, DbType? dbType = null, int? size = null, byte? precision = null, byte? scale = null)
```

To add an output parameter, use:

```csharp
void SpParameters.AddOutput(string name, DbType dbType, int? size = null, byte? precision = null, byte? scale = null)
```

To add an input/output parameter, use:

```csharp
void SpParameters.AddInputOutput(string name, object value, DbType? dbType = null, int? size = null, byte? precision = null, byte? scale = null)
```

To set the return parameter, use:

```csharp
void SpParameters.SetReturn(string name, DbType dbType, int? size = null, byte? precision = null, byte? scale = null)
```

After executing the stored procedure using `IDbConnection.StoredProcedure`, the output- and return values are available from the `SpParameters` object.

To get the value of an output parameter, use:

```csharp
T SpParameters.GetOutputValue<T>(string name)
```

To get the return value, use:

```csharp
T SpParameters.GetReturnValue<T>()
```

Note that values of the output- and return parameters are casted directly to `T`.

### Command parameters and configuration

With the exception of `StoredProcedure`, all Blazer `IDbConnection` functions have arguments of shape `(string command, object parameters = null, CommandConfiguration config = null)`.

`command` is pretty self-explanatory: it contains the command text.

*(optional)* `parameters` contains the command parameters. You'll most likely just want to use an anonymous type for this, although this is not a requirement. Any type can be used, though only `Public Instance` properties of the type can be used as parameter values. For the conventions Blazer uses to map properties of the parameter object to `IDataParameter`s on a command, see the *Mapping conventions* section.

*(optional)* `config` contains some configuration options to use for the command. It can contain things like the command timeout or `IDbTransaction` to use. If `null` is passed here, Blazer falls back to its default configuration options. The defaults can be accessed through `CommandConfiguration.Default`. When a `config` value is provided, not all options on it are necessarily set. A second option, `CommandConfiguration.OnUnsetConfigurationOption`, controls how Blazer deals with this situation.

### async/await

For any target platform that supports `async/await`, Blazer's various functions contain a matching `...Async()` version to support asynchonous programming patterns.

### String interpolation

Starting with .Net 4.6 and .Net Standard 1.3, Blazer supports string interpolation as an alternative way of passing parameters to queries. For this, Blazer's `IDbConnection` functions contain an overload of shape `(FormattableString commandString, CommandConfiguration config = null)`, combining the command string and the parameters in one argument.

So, instead of doing something like this:

```csharp
someConnection.Query("SELECT * FROM Foo WHERE Bar = @Bar", new { Bar = 42 });
```

...you can just do this:

```csharp
someConnection.Query($"SELECT * FROM Foo WHERE Bar = {42}");
```

Blazer will intercept the interpolated string and its argument values, and uses them to produce a parameterized SQL query (as it would for regular string queries).

In the SQL profiler, this first query produces the following:

```sql
exec sp_executesql N'SELECT * FROM Foo WHERE Bar = @Bar',N'@Bar int',@Bar=42
```

...while the second query produces this:

```sql
exec sp_executesql N'SELECT * FROM Foo WHERE Bar = @p__blazer__0',N'@p__blazer__0 int',@p__blazer__0=42
```

## Performance

Blazer is blazingly fast. Here are some performance benchmarks comparing Blazer to other popular data access libraries.

### Selecting many records (typed results)

In this test, the top *N* records are selected in a single query and are mapped to a strongly-typed POCO.  
Results are averaged over 10 runs. The values are formatted `average (stdev)`.

<table>
  <tr>
    <th>Provider</th>
    <th>N = 500</th>
    <th>N = 5.000</th>
    <th>N = 50.000</th>
  </tr>
  <tr>
    <td>Hand-Crafted ADO.NET</td>
    <td>0,5ms (0,4ms)</td>
    <td>7,2ms (0,4ms)</td>
    <td>81,2ms (2,96ms)</td>
  </tr>
  <tr>
    <td>Blazer v0.1.0</td>
    <td><strong>0,8ms (0,4ms)</strong></td>
    <td><strong>8,3ms (0,46ms)</strong></td>
    <td><strong>93,1ms (2,91ms)</strong></td>
  </tr>
  <tr>
    <td>Linq2SQL .NET 4.6.1</td>
    <td>1,1ms (0,3ms)</td>
    <td>11,9ms (0,54ms)</td>
    <td>120,4ms (5,31ms)</td>
  </tr>
  <tr>
    <td>Linq2SQL .NET 4.6.1 (change tracking)</td>
    <td>1,4ms (0,66ms)</td>
    <td>12,9ms (0,7ms)</td>
    <td>132,3ms (2,57ms)</td>
  </tr>
  <tr>
    <td>Entity Framework v6.1.3</td>
    <td>1,2ms (0,6ms)</td>
    <td>12ms (0,77ms)</td>
    <td>139,6ms (8,26ms)</td>
  </tr>
  <tr>
    <td>Entity Framework v6.1.3 (change tracking)</td>
    <td>6,9ms (0,54ms)</td>
    <td>90,4ms (6,81ms)</td>
    <td>1009,2ms (38,21ms)</td>
  </tr>
  <tr>
    <td>Dapper v1.42.0</td>
    <td>1,1ms (0,13ms)</td>
    <td>10ms (0,2ms)</td>
    <td>118,1ms (2,02ms)</td>
  </tr>
  <tr>
    <td>OrmLite v4.0.56</td>
    <td>1ms (0ms)</td>
    <td>9ms (0,7ms)</td>
    <td>106,3ms (2,05ms)</td>
  </tr>
  <tr>
    <td>PetaPoco v5.1.1.171</td>
    <td>1,2ms (0,08ms)</td>
    <td>9,9ms (0,3ms)</td>
    <td>98,5ms (1,75ms)</td>
  </tr>
</table>

Blazer takes the gold, but it's a photo finish. In fact, all tested providers perform great in this category, except for Entity Framework with change tracking enabled.

### Selecting many records (dynamic results)

In this test, the top *N* records are selected in a single query and are provided as `dynamic` results.  
Results are averaged over 10 runs. The values are formatted `average (stdev)`.

<table>
  <tr>
    <th>Provider</th>
    <th>N = 500</th>
    <th>N = 5.000</th>
    <th>N = 50.000</th>
  </tr>
  <tr>
    <td>Blazer v0.1.0</td>
    <td>2,7ms (1,76ms)</td>
    <td>10,6ms (0,66ms)</td>
    <td>182,4ms (3,17ms)</td>
  </tr>
  <tr>
    <td>Dapper v1.42.0</td>
    <td><strong>1,3ms (0,9ms)</strong></td>
    <td><strong>10,4ms (0,49ms)</strong></td>
    <td><strong>126,5ms (1,2ms)</strong></td>
  </tr>
  <tr>
    <td>PetaPoco v5.1.1.171</td>
    <td>2,9ms (0,54ms)</td>
    <td>22,3ms (0,46ms)</td>
    <td>322,2ms (4,24ms)</td>
  </tr>
  <tr>
    <td>Simple.Data v0.19.0</td>
    <td>4,9ms (2,7ms)</td>
    <td>34,9ms (2,47ms)</td>
    <td>334,9ms (1,81ms)</td>
  </tr>
  <tr>
    <td>Massive v2.0</td>
    <td>9,8ms (1,47ms)</td>
    <td>30,7ms (0,9ms)</td>
    <td>248,5ms (1,8ms)</td>
  </tr>
</table>

Dapper wins at this particular test, with Blazer at an undisputed second place. Not too shabby!

### Selecting single records (typed results)

In this test, single, random records are selected *N* times and are mapped to a strongly-typed POCO.  
Results are averaged over 10 runs. The values are formatted `average (stdev)`.

<table>
  <tr>
    <th>Provider</th>
    <th>N = 500</th>
    <th>N = 5.000</th>
    <th>N = 50.000</th>
  </tr>
  <tr>
    <td>Hand-Crafted ADO.NET</td>
    <td>35,5ms (1,63ms)</td>
    <td>349,8ms (2,14ms)</td>
    <td>3512,9ms (14,46ms)</td>
  </tr>
  <tr>
    <td>Blazer v0.1.0</td>
    <td><strong>37ms (4,49ms)</strong></td>
    <td><strong>366,7ms (2,49ms)</strong></td>
    <td><strong>3674,5ms (14,53ms)</strong></td>
  </tr>
  <tr>
    <td>Linq2SQL .NET 4.6.1</td>
    <td>414,9ms (5,03ms)</td>
    <td>4143,7ms (15,79ms)</td>
    <td>41686,2ms (407,96ms)</td>
  </tr>
  <tr>
    <td>Linq2SQL .NET 4.6.1 (change tracking)</td>
    <td>421,5ms (15,09ms)</td>
    <td>4194,2ms (62,36ms)</td>
    <td>35444,9ms (411,42ms)</td>
  </tr>
  <tr>
    <td>Entity Framework v6.1.3</td>
    <td>218,1ms (7,62ms)</td>
    <td>2248,7ms (50,05ms)</td>
    <td>21956,8ms (112,96ms)</td>
  </tr>
  <tr>
    <td>Entity Framework v6.1.3 (change tracking)</td>
    <td>228,8ms (9,83ms)</td>
    <td>2310ms (24,35ms)</td>
    <td>22811,6ms (105,37ms)</td>
  </tr>
  <tr>
    <td>Dapper v1.42.0</td>
    <td>39,5ms (3,98ms)</td>
    <td>379ms (2,68ms)</td>
    <td>3783,3ms (10,61ms)</td>
  </tr>
  <tr>
    <td>OrmLite v4.0.56</td>
    <td>67,6ms (3,26ms)</td>
    <td>661ms (2,76ms)</td>
    <td>6587,3ms (21,18ms)</td>
  </tr>
  <tr>
    <td>PetaPoco v5.1.1.171</td>
    <td>47,1ms (1,92ms)</td>
    <td>473,5ms (15,24ms)</td>
    <td>4612,4ms (28,9ms)</td>
  </tr>
</table>

Blazer wins at this one, with Dapper and PetaPoco closely behind. Microsoft's own data access tech falls a bit short here, and is about an order of magnitude slower.

Everything scales linearly over *N*, as expected.

### Selecting single records (dynamic results)

In this test, single, random records are selected *N* times and provided as `dynamic` results.  
Results are averaged over 10 runs. The values are formatted `average (stdev)`.

<table>
  <tr>
    <th>Provider</th>
    <th>N = 500</th>
    <th>N = 5.000</th>
    <th>N = 50.000</th>
  </tr>
  <tr>
    <td>Blazer v0.1.0</td>
    <td><strong>36,8ms (1,99ms)</strong></td>
    <td><strong>340,4ms (21,06ms)</strong></td>
    <td><strong>3480,7ms (284,05ms)</strong></td>
  </tr>
  <tr>
    <td>Dapper v1.42.0</td>
    <td>45,1ms (5,43ms)</td>
    <td>420,9ms (7,46ms)</td>
    <td>4280,1ms (98,7ms)</td>
  </tr>
  <tr>
    <td>PetaPoco v5.1.1.171</td>
    <td>59,4ms (3,9ms)</td>
    <td>541,3ms (41,77ms)</td>
    <td>5022,7ms (176,1ms)</td>
  </tr>
  <tr>
    <td>Simple.Data v0.19.0</td>
    <td>134,5ms (6,34ms)</td>
    <td>1470,6ms (16,49ms)</td>
    <td>14851,8ms (221,19ms)</td>
  </tr>
  <tr>
    <td>Massive v2.0</td>
    <td>132,6ms (9,26ms)</td>
    <td>1436,2ms (14,12ms)</td>
    <td>14371,5ms (28,6ms)</td>
  </tr>
</table>

Although Dapper won when selecting many `dynamic` results in a single query, Blazer is faster when selecting single records as `dynamic`.

## Mapping conventions

Here's how Blazer does its mapping.

### Input parameters

For input parameters, the name of the parameter property will become the name of the `IDbDataParameter` added to the command, prefixed with an "@".

So for example, take the following input parameter object:

```csharp
{
  CategoryId = 42,
  YearStart = 2016
}
```

In this case two parameters named `@CategoryId` and `@YearStart` will be added to the command.

Blazer does not attempt to parse the command string to see if each parameter is actually used. All properties in the input parameter object will be blindly added as command parameters.

**Note**: when using Blazer's string interpolation functions, none of the above is applicable as Blazer just generates its own parameter names, and adds parameters automatically for each string interpolation argument. So in that case you don't have to worry about any of this :)

### Output parameters

In the case of typed query results, Blazer performs mapping from the columns of the query result set to fields on the result type. To do this it attempts to find a matching field for each column of the result set, using a couple of simple matching rules that are attempted in sequence.

First, it looks for any public instance member (field or property) which has a `System.ComponentModel.DataAnnotations.Schema.ColumnAttribute` where the `ColumnAttribute.Name` value equals the name of the result set column.

Secondly, it looks for any member (again, public instance field or property) where the name (`MemberInfo.Name` property) equals the name of the result set column.

`StringComparison.OrdinalIgnoreCase` is used for all string matching.

The first member found is matched to the result set column. Columns not matched to any member are simply ignored.

## License

See [LICENSE.txt](https://github.com/b-w/Blazer/blob/master/LICENSE.txt)

## Copyright

Blazer is copyright (c) 2017 Bart Wolff, [www.bartwolff.com]()
