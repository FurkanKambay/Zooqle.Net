using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Zooqle.Net
{
    // TODO Documentation

    /// <summary>
    /// Advanced search filter query builder to be used in <see cref="ZooqleClient"/>.
    /// </summary>
    public class SearchQuery
    {
        private SearchQuery() { }

        /// <summary>
        /// Indicates if a search can be made at this state.
        /// </summary>
        public bool IsValid => !string.IsNullOrEmpty(searchTerms);

        /// <summary>
        /// Creates a new <see cref="SearchQuery"/> with the given search terms.
        /// </summary>
        /// <param name="searchTerms">
        /// The main search terms for the query. The query will not be valid if this is
        /// <see langword="null"/> or whitespace.
        /// </param>
        public static SearchQuery Create(string searchTerms) =>
            new SearchQuery().WithSearchTerms(searchTerms);

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
            this.exactMatchTerms = RemoveWhitespace(exactMatchTerms?.Replace('"', ' '));
            return this;
        }

        public SearchQuery LargerThan(int amount, SizeUnit unit) => InternalSetSize(amount, unit, false);

        public SearchQuery SmallerThan(int amount, SizeUnit unit) => InternalSetSize(amount, unit, true);

        public SearchQuery OlderThan(int amount, TimeUnit unit) => InternalSetAge(amount, unit, true);

        public SearchQuery NewerThan(int amount, TimeUnit unit) => InternalSetAge(amount, unit, false);

        public SearchQuery InCategory(Category category)
        {
            this.category = (category > default(Category) && (int)category < Category.Other.GetHashCode() << 1) ? category : default(Category);
            return this;
        }

        public SearchQuery InLanguage(Language language)
        {
            this.language = Enum.IsDefined(typeof(Language), language) ? language : default(Language);
            return this;
        }

        public SearchQuery OnlyInFiles(bool onlyInFiles = true)
        {
            onlyFiles = onlyInFiles;
            return this;
        }

        /// <summary>
        /// Returns the query string or <see cref="string.Empty"/> if the query is not valid.
        /// </summary>
        public override string ToString()
        {
            if (!IsValid)
                return string.Empty;

            var filters = new List<string>() { searchTerms };

            if (!string.IsNullOrEmpty(excludedTerms))
            {
                foreach (var term in excludedTerms.Split(' '))
                    filters.Add("-" + term);
            }

            if (!string.IsNullOrEmpty(exactMatchTerms))
                filters.Add("\"" + exactMatchTerms.Trim() + "\"");

            var minIsValid = minSizeAmount > default(int);
            var maxIsValid = maxSizeAmount > default(int);
            if (minIsValid || maxIsValid)
            {
                filters.Add(
                    minIsValid && maxIsValid ? minSizeAmount + minSizeUnit.ToString() + "-" + maxSizeAmount + maxSizeUnit
                    : minIsValid ? ">" + minSizeAmount + minSizeUnit
                    : "<" + maxSizeAmount + maxSizeUnit);
            }

            if (ageAmount > default(int))
                filters.Add((ageIsOlder ? "before:" : "after:") + ageAmount + ageUnit.ToString()[0]);

            if (category != default(Category))
                filters.Add("category:" + category.ToString().Replace(space, string.Empty));

            if (language != default(Language))
            {
                // Hexavigesimal decoding of enum values (magic number to 2-letter codes)
                var lang = (int)language - 1;
                filters.Add("+lang:" + (char)(96 + lang / 26) + (char)(97 + lang % 26));
            }

            if (onlyFiles)
                filters.Add("!onlyFiles");

            return RemoveWhitespace(string.Join(space, filters));
        }

        private SearchQuery InternalSetSize(int amount, SizeUnit unit, bool isMax)
        {
            var isValid = amount > default(int) && unit >= SizeUnit.KB && unit <= SizeUnit.GB;

            if (isValid && isMax)
                ValidateAndAssign(false, minSizeAmount, minSizeUnit, amount, unit, ref maxSizeAmount, ref maxSizeUnit);
            else if (isValid)
                ValidateAndAssign(true, amount, unit, maxSizeAmount, maxSizeUnit, ref minSizeAmount, ref minSizeUnit);
            else if (isMax)
                maxSizeAmount = default(int);
            else
                minSizeAmount = default(int);

            return this;
        }

        private SearchQuery InternalSetAge(int amount, TimeUnit unit, bool isOlder)
        {
            ageAmount = default(int);

            if (amount > default(int) && unit >= TimeUnit.Hour && unit <= TimeUnit.Month)
            {
                ageAmount = amount;
                ageUnit = unit;
                ageIsOlder = isOlder;
            }

            return this;
        }

        private static bool ValidateAndAssign(bool validatingMin, int minAmount, SizeUnit minUnit, int maxAmount, SizeUnit maxUnit, ref int amountToAssign, ref SizeUnit unitToAssign)
        {
            long minKB = minAmount;
            for (var i = 0; i < (int)minUnit; i++)
                minKB *= 1024;

            long maxKB = maxAmount;
            for (var i = 0; i < (int)maxUnit; i++)
                maxKB *= 1024;

            if (maxAmount == default(int) || minAmount == default(int) || minKB < maxKB)
            {
                amountToAssign = validatingMin ? minAmount : maxAmount;
                unitToAssign = validatingMin ? minUnit : maxUnit;
                return true;
            }
            return false;
        }

        private static string RemoveWhitespace(string text) =>
            string.IsNullOrWhiteSpace(text) ? string.Empty : Regex.Replace(text, @"\s{2,}", space).Trim();

        private string searchTerms;
        private string excludedTerms;
        private string exactMatchTerms;

        private int minSizeAmount;
        private SizeUnit minSizeUnit;
        private int maxSizeAmount;
        private SizeUnit maxSizeUnit;

        private int ageAmount;
        private TimeUnit ageUnit;
        private bool ageIsOlder;

        private Category category;
        private Language language;
        private bool onlyFiles;

        private const string space = " ";
    }
}
