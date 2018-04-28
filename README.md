[![NuGet](https://img.shields.io/nuget/v/Zooqle.Net.svg)](https://www.nuget.org/packages/Zooqle.Net)
[![GitHub license](https://img.shields.io/github/license/FurkanKambay/Zooqle.Net.svg)](https://github.com/FurkanKambay/Zooqle.Net/blob/master/LICENSE)
[![GitHub issues](https://img.shields.io/github/issues/FurkanKambay/Zooqle.Net.svg)](https://github.com/FurkanKambay/Zooqle.Net/issues)
[![CodeFactor](https://www.codefactor.io/repository/github/FurkanKambay/Zooqle.Net/badge)](https://www.codefactor.io/repository/github/FurkanKambay/Zooqle.Net)

# Zooqle.Net

A .NET Standard 1.1 library for searching torrents on [Zooqle](https://zooqle.com/).

## Installation

Package Manager
```
Install-Package Zooqle.Net
```

.NET CLI
```
dotnet add package Zooqle.Net 
```

## Usage

### Import namespace

```C#
using Zooqle.Net;
using Zooqle.Net.Advanced; // for advanced search
```

### Basic search

```C#
SearchResult firstPage = await ZooqleClient.SearchAsync("search terms");
SearchResult secondPage = await ZooqleClient.SearchAsync("search terms", page: 2);
```

### Advanced search (filters)

```C#
// Apps and games in English that are larger than 1GB and released in the last 2 weeks
AdvancedQuery query = new AdvancedQuery("search terms")
{
    Categories = Categories.Apps | Categories.Games,
    Language = Language.English,
    MinSize = new Size(1, SizeUnit.GB),
    Age = Age.NewerThan(2, TimeUnit.Week)
}

SearchResult result = await ZooqleClient.SearchAsync(query, page: 1);
```

### Search results

```C#
SearchResult page = await ZooqleClient.SearchAsync("search terms");
Torrent torrent = page.Results[0];

string title = torrent.Title;
int seedCount = torrent.SeedCount;
Uri magnetUri = torrent.MagnetUri;
```
