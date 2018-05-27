[![Build status](https://ci.appveyor.com/api/projects/status/fs435q89drf083mc?svg=true)](https://ci.appveyor.com/project/FurkanKambay/zooqle-net)
[![CodeFactor](https://www.codefactor.io/repository/github/FurkanKambay/Zooqle.Net/badge)](https://www.codefactor.io/repository/github/FurkanKambay/Zooqle.Net)
[![NuGet package](https://img.shields.io/nuget/v/Zooqle.Net.svg)](https://www.nuget.org/packages/Zooqle.Net)
[![GitHub issues](https://img.shields.io/github/issues/FurkanKambay/Zooqle.Net.svg)](https://github.com/FurkanKambay/Zooqle.Net/issues)
[![GitHub license](https://img.shields.io/github/license/FurkanKambay/Zooqle.Net.svg)](https://github.com/FurkanKambay/Zooqle.Net/blob/master/LICENSE)

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
SearchResult firstPage = await ZooqleClient.SearchTorrentAsync("search terms");
SearchResult secondPage = await ZooqleClient.SearchTorrentAsync("search terms", page: 2);
```

### Find with info hash

```C#
// Info hash: base-16 or base-32 string
Uri pageUri32 = await ZooqleClient.FindTorrentByInfoHashAsync("0123456789ABCDEF000000000000000000000000");
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
};

SearchResult result = await ZooqleClient.SearchTorrentAsync(query, page: 1);
```

### Search results

```C#
SearchResult page = await ZooqleClient.SearchTorrentAsync("search terms");
Torrent torrent = page.Results[0];

string title = torrent.Title;
int seedCount = torrent.SeedCount;
Uri magnetUri = torrent.MagnetUri;
```
