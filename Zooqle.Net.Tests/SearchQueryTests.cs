using Xunit;

namespace Zooqle.Net.Tests
{
    public class SearchQueryTests
    {
        [Fact]
        public void Whitespace_And_Illegal_Characters_Are_Removed()
        {
            var expected = @"search something -exclude -these -words ""match these exactly""";
            var query = SearchQuery.Create(" search   something ")
                .ExcludingTerms(" exclude  these  words ")
                .MatchingExactly("match\"these\"exactly");

            Assert.Equal(expected, query.ToString());
        }

        [Fact]
        public void Can_Overwrite_Search_Terms()
        {
            var expected1 = "new terms";
            var query1 = SearchQuery.Create("old terms")
                .WithSearchTerms("new terms");

            var expected2 = "new terms !onlyFiles";
            var query2 = SearchQuery.Create("old terms")
                .OnlyInFiles()
                .WithSearchTerms("new terms");

            Assert.Equal(expected1, query1.ToString());
            Assert.Equal(expected2, query2.ToString());
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
            var expected = "terms +lang:" + languageCode;
            var query = SearchQuery.Create("terms").InLanguage(language);

            Assert.Equal(expected, query.ToString());
        }

        [Fact]
        public void Correct_Query_When_All_Filters_Applied()
        {
            var expected = @"search something else -exclude -these -too ""match exactly"" 50KB-10MB before:9M category:Apps +lang:es !onlyFiles";
            var query = SearchQuery.Create(" search something else")
                .ExcludingTerms("exclude these too")
                .MatchingExactly("match exactly")
                .LargerThan(50, SizeUnit.KB)
                .SmallerThan(10, SizeUnit.MB)
                .OlderThan(9, TimeUnit.Month)
                .InCategory(Category.Apps)
                .InLanguage(Language.Spanish)
                .OnlyInFiles();

            Assert.Equal(expected, query.ToString());
        }

        [Theory]
        [InlineData(50, SizeUnit.MB, 20, SizeUnit.MB)]
        [InlineData(2, SizeUnit.MB, 2040, SizeUnit.KB)]
        [InlineData(1052, SizeUnit.MB, 1, SizeUnit.GB)]
        [InlineData(3000, SizeUnit.KB, 2, SizeUnit.MB)]
        public void Invalid_Size_Amounts_Do_Not_Affect_Query(int minAmount, SizeUnit minUnit, int maxAmount, SizeUnit maxUnit)
        {
            var expected1 = "terms >" + minAmount + minUnit;
            var query1 = SearchQuery.Create("terms")
                .LargerThan(minAmount, minUnit)
                .SmallerThan(maxAmount, maxUnit);

            var expected2 = "terms <" + maxAmount + maxUnit;
            var query2 = SearchQuery.Create("terms")
                .SmallerThan(maxAmount, maxUnit)
                .LargerThan(minAmount, minUnit);

            Assert.Equal(expected1, query1.ToString());
            Assert.Equal(expected2, query2.ToString());
        }

        [Theory]
        [InlineData((SizeUnit)(-1))]
        [InlineData((SizeUnit)(3))]
        public void Invalid_Size_Units_Do_Not_Affect_Query(SizeUnit unit)
        {
            var expected1 = "terms";
            var query1 = SearchQuery.Create("terms")
                .LargerThan(1, unit)
                .SmallerThan(2, unit);

            var expected2 = "terms <1MB";
            var query2 = SearchQuery.Create("terms")
                .SmallerThan(1, SizeUnit.MB)
                .LargerThan(2, unit);

            var expected3 = "terms >1MB";
            var query3 = SearchQuery.Create("terms")
                .LargerThan(1, SizeUnit.MB)
                .SmallerThan(2, unit);

            Assert.Equal(expected1, query1.ToString());
            Assert.Equal(expected2, query2.ToString());
            Assert.Equal(expected3, query3.ToString());
        }

        [Fact]
        public void Default_Filters_Do_Not_Affect_Query()
        {
            var expected = "terms";
            var query = SearchQuery.Create(" terms ")
                .ExcludingTerms("  ")
                .MatchingExactly(" \" \" ")
                .LargerThan(0, SizeUnit.MB)
                .SmallerThan(0, SizeUnit.GB)
                .NewerThan(0, TimeUnit.Day)
                .InCategory(Category.Any)
                .InLanguage(Language.Any)
                .OnlyInFiles(false);

            Assert.Equal(expected, query.ToString());
        }
    }
}
