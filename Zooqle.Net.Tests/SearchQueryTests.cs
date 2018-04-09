using Xunit;

namespace Zooqle.Net.Tests
{
    public class SearchQueryTests
    {
        [Fact]
        public void Whitespace_And_Illegal_Characters_Are_Removed()
        {
            var expected = @"search something -exclude -these -words ""match these exactly""";
            var actual = SearchQuery.Create(" search   something ")
                .ExcludingTerms(" exclude  these  words ")
                .MatchingExactly("match\"these\"exactly")
                .ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_Overwrite_Search_Terms()
        {
            var expected = "new";
            var actual = SearchQuery.Create("old")
                .WithSearchTerms(expected)
                .ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_Overwrite_Exact_Matches()
        {
            var expected = "\"new\"";
            var actual = SearchQuery.Create("old", true)
                .MatchingExactly("new")
                .ToString();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("aa", Language.Afar)]
        [InlineData("az", Language.Azerbaijani)]
        [InlineData("ba", Language.Bashkir)]
        [InlineData("es", Language.Spanish)]
        [InlineData("zh", Language.Chinese)]
        [InlineData("zu", Language.Zulu)]
        public void Language_Codes_Are_Decoded_Correctly(string languageCode, Language language)
        {
            var expected = terms + " +lang:" + languageCode;
            var actual = SearchQuery.Create(terms)
                .InLanguage(language)
                .ToString();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("TV", Categories.TV)]
        [InlineData("Movies,TV,Other", Categories.TV | Categories.Movies | Categories.Other)]
        [InlineData("TV,Anime", Categories.TV | Categories.Anime)]
        [InlineData("Games,Apps", Categories.Apps | Categories.Games)]
        public void Category_Strings_Are_Correct(string categoryText, Categories categories)
        {
            var expected = terms + " category:" + categoryText;
            var actual = SearchQuery.Create(terms)
                .InCategories(categories)
                .ToString();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(50, SizeUnit.MB, 20, SizeUnit.MB)]
        [InlineData(2, SizeUnit.MB, 2040, SizeUnit.KB)]
        [InlineData(1052, SizeUnit.MB, 1, SizeUnit.GB)]
        [InlineData(3000, SizeUnit.KB, 2, SizeUnit.MB)]
        public void Invalid_Size_Amounts_Do_Not_Affect_Query(int minAmount, SizeUnit minUnit, int maxAmount, SizeUnit maxUnit)
        {
            var expected1 = terms + " >" + minAmount + minUnit;
            var actual1 = SearchQuery.Create(terms)
                .LargerThan(minAmount, minUnit)
                .SmallerThan(maxAmount, maxUnit)
                .ToString();

            var expected2 = terms + " <" + maxAmount + maxUnit;
            var actual2 = SearchQuery.Create(terms)
                .SmallerThan(maxAmount, maxUnit)
                .LargerThan(minAmount, minUnit)
                .ToString();

            Assert.Equal(new[] { expected1, expected2 }, new[] { actual1, actual2 });
        }

        [Theory]
        [InlineData(0, -1, ">1KB")]
        [InlineData(0, 3, ">1KB")]
        [InlineData(-1, 0, "<2KB")]
        [InlineData(3, 0, "<2KB")]
        [InlineData(-2, 5, null)]
        [InlineData(4, -3, null)]
        public void Invalid_Size_Units_Do_Not_Affect_Query(int minUnit, int maxUnit, string sizeText)
        {
            var expected = (terms + " " + sizeText).TrimEnd();
            var actual = SearchQuery.Create(terms)
                .LargerThan(1, (SizeUnit)minUnit)
                .SmallerThan(2, (SizeUnit)maxUnit)
                .ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Default_Filters_Do_Not_Affect_Query()
        {
            var actual = SearchQuery.Create(terms)
                .ExcludingTerms("")
                .MatchingExactly("")
                .LargerThan(0, SizeUnit.MB)
                .SmallerThan(0, SizeUnit.GB)
                .NewerThan(0, TimeUnit.Day)
                .OlderThan(0, TimeUnit.Day)
                .InCategories(Categories.Any)
                .InLanguage(Language.Any)
                .OnlyInFiles(false)
                .ToString();

            Assert.Equal(terms, actual);
        }

        private const string terms = "terms";
    }
}
