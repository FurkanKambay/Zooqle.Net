using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Zooqle.Net
{
    // TODO SearchQuery documentation

    /// <summary>
    /// Advanced search filter query builder to be used in <see cref="ZooqleClient"/>.
    /// </summary>
    public class SearchQuery
    {
        private SearchQuery() { }

        /// <summary>
        /// Indicates if a search can be made at this state.
        /// </summary>
        public bool IsReady => !string.IsNullOrEmpty(searchTerms) || !string.IsNullOrEmpty(exactMatchTerms);

        /// <summary>
        /// Creates a new <see cref="SearchQuery"/> with the given search terms.
        /// </summary>
        /// <param name="terms">The main search terms for the query.</param>
        /// <param name="matchExactly">Whether the given terms are matched exactly or not.</param>
        public static SearchQuery Create(string terms, bool matchExactly = false)
        {
            var query = new SearchQuery();
            return matchExactly ? query.MatchingExactly(terms) : query.WithSearchTerms(terms);
        }

        /// <summary>
        /// Overwrites the current search terms.
        /// </summary>
        /// <param name="searchTerms">The new search terms for the query.</param>
        public SearchQuery WithSearchTerms(string searchTerms)
        {
            this.searchTerms = RemoveWhitespace(searchTerms);
            return this;
        }

        /// <param name="excludedTerms">Space-delimited terms.</param>
        public SearchQuery ExcludingTerms(string excludedTerms)
        {
            this.excludedTerms = RemoveWhitespace(excludedTerms);
            return this;
        }

        public SearchQuery MatchingExactly(string exactMatchTerms)
        {
            this.exactMatchTerms = RemoveWhitespace(exactMatchTerms?.Replace('"', spaceC));
            return this;
        }

        public SearchQuery LargerThan(int amount, SizeUnit unit)
        {
            minSize = GetSizeValues(false, amount, unit);
            return this;
        }

        public SearchQuery SmallerThan(int amount, SizeUnit unit)
        {
            maxSize = GetSizeValues(true, amount, unit);
            return this;
        }

        public SearchQuery OlderThan(int amount, TimeUnit unit)
        {
            age = GetAgeValues(true, amount, unit);
            return this;
        }

        public SearchQuery NewerThan(int amount, TimeUnit unit)
        {
            age = GetAgeValues(false, amount, unit);
            return this;
        }

        public SearchQuery InCategories(Categories categories)
        {
            var isValid = categories >= default(Categories) && (int)categories < (int)Categories.Other << 1;
            this.categories = isValid ? categories : default(Categories);
            return this;
        }

        public SearchQuery InLanguage(Language language)
        {
            var isValid = Enum.IsDefined(typeof(Language), language);
            this.language = isValid ? language : default(Language);
            return this;
        }

        public SearchQuery OnlyInFiles(bool onlyInFiles = true)
        {
            this.onlyInFiles = onlyInFiles;
            return this;
        }

        /// <summary>
        /// Returns the query string or <see cref="string.Empty"/> if the query is not ready.
        /// </summary>
        public override string ToString()
        {
            if (!IsReady)
                return string.Empty;

            var filters = new List<string>();

            if (!string.IsNullOrEmpty(searchTerms))
                filters.Add(searchTerms);

            if (!string.IsNullOrEmpty(excludedTerms))
                filters.AddRange(excludedTerms.Split(spaceC).Select(term => "-" + term));

            if (!string.IsNullOrEmpty(exactMatchTerms))
                filters.Add("\"" + exactMatchTerms.Trim() + "\"");

            var minIsValid = minSize.amount > default(int);
            var maxIsValid = maxSize.amount > default(int);
            
            if (minIsValid && maxIsValid)
                filters.Add($"{minSize.amount}{minSize.unit}-{maxSize.amount}{maxSize.unit}");
            else if (minIsValid)
                filters.Add($">{minSize.amount}{minSize.unit}");
            else if (maxIsValid)
                filters.Add($"<{maxSize.amount}{maxSize.unit}");

            if (age.amount > default(int))
                filters.Add((age.isOlder ? "before:" : "after:") + age.amount + age.unit.ToString()[0]);

            if (categories != default(Categories))
                filters.Add("category:" + categories.ToString().Replace(spaceS, string.Empty));

            if (language != default(Language))
            {
                // Hexavigesimal decoding of enum values (magic number to 2-letter codes)
                var lang = (int)language - 1;
                filters.Add("+lang:" + (char)(96 + lang / 26) + (char)(97 + lang % 26));
            }

            if (onlyInFiles)
                filters.Add("!onlyFiles");

            return string.Join(spaceS, filters);
        }

        private (int amount, SizeUnit unit) GetSizeValues(bool isMax, int amount, SizeUnit unit)
        {
            var isUnitValid = amount > default(int) && unit >= SizeUnit.KB && unit <= SizeUnit.GB;
            var isAmountValid = isMax && (minSize.amount == default(int) || IsLessThan(minSize, (amount, unit)))
                || !isMax && (maxSize.amount == default(int) || IsLessThan((amount, unit), maxSize));

            return (isUnitValid && isAmountValid) ? (amount, unit) : (default(int), default(SizeUnit));
        }

        private (int amount, TimeUnit unit, bool isOlder) GetAgeValues(bool isOlder, int amount, TimeUnit unit)
        {
            return amount > default(int) && unit >= TimeUnit.Hour && unit <= TimeUnit.Month
                ? (amount, unit, isOlder)
                : (default(int), default(TimeUnit), default(bool));
        }

        private static bool IsLessThan((int amount, SizeUnit unit) minSize, (int amount, SizeUnit unit) maxSize) =>
            ConvertToKB(minSize) < ConvertToKB(maxSize);

        private static long ConvertToKB((int amount, SizeUnit unit) size) =>
            (long)size.amount << 10 * (int)size.unit;

        private static string RemoveWhitespace(string text) =>
            string.IsNullOrWhiteSpace(text) ? string.Empty : Regex.Replace(text, @"\s{2,}", spaceS).Trim();

        private string searchTerms;
        private string excludedTerms;
        private string exactMatchTerms;

        private (int amount, SizeUnit unit) minSize;
        private (int amount, SizeUnit unit) maxSize;

        private (int amount, TimeUnit unit, bool isOlder) age;

        private Categories categories;
        private Language language;
        private bool onlyInFiles;

        private const string spaceS = " ";
        private const char spaceC = ' ';
    }
}
