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

### Advanced search (filters)

```C#
// Apps in English that are larger than 1GB
SearchQuery query = SearchQuery.Create("app name")
    .InCategory(Category.Apps)
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
