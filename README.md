[![Build status](https://ci.appveyor.com/api/projects/status/fs435q89drf083mc?svg=true)](https://ci.appveyor.com/project/FurkanKambay/zooqle-net)
[![CodeFactor status](https://www.codefactor.io/repository/github/FurkanKambay/Zooqle.Net/badge)](https://www.codefactor.io/repository/github/FurkanKambay/Zooqle.Net)
[![NuGet package](https://img.shields.io/nuget/v/Zooqle.Net.svg)](https://www.nuget.org/packages/Zooqle.Net)
[![GitHub issues](https://img.shields.io/github/issues/FurkanKambay/Zooqle.Net.svg)](https://github.com/FurkanKambay/Zooqle.Net/issues)
[![GitHub license](https://img.shields.io/github/license/FurkanKambay/Zooqle.Net.svg)](https://github.com/FurkanKambay/Zooqle.Net/blob/master/LICENSE)

# Zooqle.Net

A .NET Standard 1.1 library for searching torrents on [Zooqle](https://zooqle.com/).

## Installation

Package Manager
```powershell
Install-Package Zooqle.Net
```

.NET CLI
```
dotnet add package Zooqle.Net
```

## Class Diagrams

- **Zooqle.Net**

![Zooqle.Net diagram](https://i.imgur.com/7rhvAGR.png)

- **Zooqle.Net.Advanced**

![Zooqle.Net.Advanced diagram](https://i.imgur.com/rN29h1m.png)

## Usage

### Namespaces

```csharp
using Zooqle.Net;
using Zooqle.Net.Advanced; // for advanced search
```

### Basic torrent search

```csharp
SearchResult result = await ZooqleClient.SearchTorrentAsync("search terms", page: 1);
```

### Find torrent with an info hash

```csharp
// Info hash: base-16 or base-32 string
Uri torrentWebpage = await ZooqleClient.FindTorrentByInfoHashAsync("0123456789ABCDEF000000000000000000000000");
```

### Advanced torrent search (filters)

```csharp
// Apps and games in English that are larger than 1 GB and
// released in the last 2 weeks that match the search terms
AdvancedQuery query = new AdvancedQuery("search terms")
{
    Categories = Categories.Apps | Categories.Games,
    Language = Language.English,
    MinSize = new Size(1, SizeUnit.GB),
    Age = Age.NewerThan(2, TimeUnit.Week)
};

SearchResult result = await ZooqleClient.SearchTorrentAsync(query, page: 1);
```

### Searching for movies, TV shows, actors

```csharp
ReadOnlyCollection<ZooqleItem> items = await ZooqleClient.SearchItemAsync("The IT Crowd");
```

### Getting movies or TV shows by IMDb IDs

```csharp
ZooqleItem item = await ZooqleClient.GetItemFromImdbIdAsync("tt0106179");
```
