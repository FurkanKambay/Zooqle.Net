using Xunit;
using Zooqle.Net.Advanced;

namespace Zooqle.Net.Tests
{
    public class AdvancedQueryTests
    {
        [Fact]
        public void Whitespace_and_illegal_characters_are_removed()
        {
            var expected = @"search something -exclude -these -words ""match these exactly""";
            var actual = new AdvancedQuery(" search   something ")
            {
                ExcludedTerms = " exclude  these  words ",
                ExactMatchTerms = "match\"these\"exactly"
            }.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_overwrite_search_terms()
        {
            var expected = "new";
            var actual = new AdvancedQuery("old")
            {
                SearchTerms = expected
            }.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_overwrite_exact_matches()
        {
            var expected = "\"new\"";
            var actual = new AdvancedQuery("old", true)
            {
                ExactMatchTerms = expected.Trim('"')
            }.ToString();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("aa", Language.Afar)]
        [InlineData("az", Language.Azerbaijani)]
        [InlineData("ba", Language.Bashkir)]
        [InlineData("es", Language.Spanish)]
        [InlineData("zh", Language.Chinese)]
        [InlineData("zu", Language.Zulu)]
        public void Language_codes_are_decoded_correctly(string languageCode, Language language)
        {
            var expected = terms + " +lang:" + languageCode;
            var actual = new AdvancedQuery(terms)
            {
                Language = language
            }.ToString();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("TV", Categories.TV)]
        [InlineData("Movies,TV,Other", Categories.TV | Categories.Movies | Categories.Other)]
        [InlineData("TV,Anime", Categories.TV | Categories.Anime)]
        [InlineData("Games,Apps", Categories.Apps | Categories.Games)]
        public void Category_strings_are_correct(string categoryText, Categories categories)
        {
            var expected = terms + " category:" + categoryText;
            var actual = new AdvancedQuery(terms)
            {
                Categories = categories
            }.ToString();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(50, SizeUnit.MB, 20, SizeUnit.MB)]
        [InlineData(2, SizeUnit.MB, 2040, SizeUnit.KB)]
        [InlineData(1052, SizeUnit.MB, 1, SizeUnit.GB)]
        [InlineData(3000, SizeUnit.KB, 2, SizeUnit.MB)]
        public void Invalid_size_amounts_do_not_affect_query(int minAmount, SizeUnit minUnit, int maxAmount, SizeUnit maxUnit)
        {
            var expected1 = terms + " >" + minAmount + minUnit;
            var actual1 = new AdvancedQuery(terms)
            {
                MinSize = new Size(minAmount, minUnit),
                MaxSize = new Size(maxAmount, maxUnit)
            }.ToString();

            var expected2 = terms + " <" + maxAmount + maxUnit;
            var actual2 = new AdvancedQuery(terms)
            {
                MaxSize = new Size(maxAmount, maxUnit),
                MinSize = new Size(minAmount, minUnit)
            }.ToString();

            Assert.Equal(new[] { expected1, expected2 }, new[] { actual1, actual2 });
        }

        [Theory]
        [InlineData(0, -1, ">1KB")]
        [InlineData(0, 3, ">1KB")]
        [InlineData(-1, 0, "<2KB")]
        [InlineData(3, 0, "<2KB")]
        [InlineData(-2, 5, null)]
        [InlineData(4, -3, null)]
        public void Invalid_size_units_do_not_affect_query(int minUnit, int maxUnit, string sizeText)
        {
            var expected = (terms + " " + sizeText).TrimEnd();
            var actual = new AdvancedQuery(terms)
            {
                MinSize = new Size(1, (SizeUnit)minUnit),
                MaxSize = new Size(2, (SizeUnit)maxUnit)
            }.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Default_filters_do_not_affect_query()
        {
            var actual = new AdvancedQuery(terms)
            {
                ExcludedTerms = "",
                ExactMatchTerms = "",
                MinSize = new Size(0, SizeUnit.MB),
                MaxSize = new Size(0, SizeUnit.GB),
                Age = new Age(0, TimeUnit.Day, false),
                Categories = Categories.Any,
                Language = Language.Any,
                OnlyInFiles = false
            }.ToString();

            Assert.Equal(terms, actual);
        }

        private const string terms = "terms";
    }
}
