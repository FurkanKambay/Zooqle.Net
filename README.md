# Zooqle.Net

A .NET Standard library for searching torrents on [Zooqle](https://zooqle.com/).

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

### Search results

```C#
SearchResult page = await ZooqleClient.SearchAsync("search terms");
Torrent torrent = page.Results[0];

string title = torrent.Title;
int seedCount = torrent.SeedCount;
Uri magnetUri = torrent.MagnetUri;
```
