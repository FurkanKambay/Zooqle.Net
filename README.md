# Zooqle.Net

A .NET Standard library for searching torrents on [Zooqle](https://zooqle.com/).

## Usage

```C#
var zooqle = new ZooqleClient();
ReadOnlyCollection<Torrent> searchResults = await zooqle.SearchAsync("search terms");
var torrent = searchResults[0];

var title = torrent.Title;
// Seeds, Peers, Size, MagnetUri and so on…
```