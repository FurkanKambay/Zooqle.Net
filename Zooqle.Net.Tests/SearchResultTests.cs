using Xunit;

namespace Zooqle.Net.Tests
{
    public class SearchResultTests
    {
        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(30, 1)]
        [InlineData(60, 2)]
        [InlineData(90, 3)]
        public void Total_Page_Count_Is_Calculated_Correctly(int totalResults, int expectedTotalPageCount)
        {
            var result = new SearchResult
            {
                ItemCountPerPage = 30,
                TotalResultCount = totalResults,
            };

            Assert.Equal(expectedTotalPageCount, result.TotalPageCount);
        }
    }
}
