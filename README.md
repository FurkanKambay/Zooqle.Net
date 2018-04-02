[![NuGet](https://img.shields.io/nuget/v/Zooqle.Net.svg)](https://www.nuget.org/packages/Zooqle.Net)
[![GitHub license](https://img.shields.io/github/license/FurkanKambay/Zooqle.Net.svg)](https://github.com/FurkanKambay/Zooqle.Net/blob/master/LICENSE)
[![GitHub issues](https://img.shields.io/github/issues/FurkanKambay/Zooqle.Net.svg)](https://github.com/FurkanKambay/Zooqle.Net/issues)
[![CodeFactor](https://www.codefactor.io/repository/github/FurkanKambay/Zooqle.Net/badge)](https://www.codefactor.io/repository/github/FurkanKambay/Zooqle.Net)

# Zooqle.Net

A .NET Standard 2.0 library for searching torrents on [Zooqle](https://zooqle.com/).

## Installation

Package Manager
```
Install-Package Zooqle.Net -Version 0.3.0 
```

.NET CLI
```
dotnet add package Zooqle.Net 
```

## Usage

### Import namespace

```C#
using Zooqle.Net;
```

### Basic search

```C#
SearchResult firstPage = await ZooqleClient.SearchAsync("search terms");
SearchResult secondPage = await ZooqleClient.SearchAsync("search terms", page: 2);
```

### Advanced search (filters)

```C#
// Apps and games in English that are larger than 1GB
SearchQuery query = SearchQuery.Create("search terms")
    .InCategories(Categories.Apps | Categories.Games)
    .InLanguage(Language.English)
    .LargerThan(1, SizeUnit.GB);

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
