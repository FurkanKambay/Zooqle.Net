using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Zooqle.Net.Advanced
{
    // TODO SearchQuery documentation

    /// <summary>
    /// Advanced search filter query builder to be used in <see cref="ZooqleClient"/>.
    /// </summary>
    public class AdvancedQuery
    {
        private string searchTerms;
        private string excludedTerms;
        private string exactMatchTerms;
        private Size minSize;
        private Size maxSize;
        private Age age;
        private Categories categories;
        private Language language;
        private bool onlyInFiles;

        /// <summary>
        /// Indicates if a search can be made at this state.
        /// </summary>
        public bool IsReady => !string.IsNullOrEmpty(searchTerms) || !string.IsNullOrEmpty(exactMatchTerms);

        /// <summary>
        /// Creates a new <see cref="AdvancedQuery"/> with the given search terms.
        /// </summary>
        public AdvancedQuery(string terms, bool matchExactly = false)
        {
            if (matchExactly)
                ExactMatchTerms = terms;
            else
                SearchTerms = terms;
        }

        public string SearchTerms
        {
            get => searchTerms;
            set => searchTerms = RemoveWhitespace(value);
        }

        /// <summary>
        /// Space-delimited terms to be excluded from the results.
        /// </summary>
        public string ExcludedTerms
        {
            get => excludedTerms;
            set => excludedTerms = RemoveWhitespace(value);
        }

        public string ExactMatchTerms
        {
            get => exactMatchTerms;
            set => exactMatchTerms = RemoveWhitespace(value?.Replace('"', spaceC));
        }

        public Size MinSize
        {
            get => minSize;
            set
            {
                var isValid = value.IsValid && !maxSize.IsValid || value.Kilobytes < maxSize.Kilobytes;
                minSize = isValid ? value : default(Size);
            }
        }

        public Size MaxSize
        {
            get => maxSize;
            set
            {
                var isValid = value.IsValid && !minSize.IsValid || value.Kilobytes > minSize.Kilobytes;
                maxSize = isValid ? value : default(Size);
            }
        }

        public Age Age
        {
            get => age;
            set => age = value.IsValid ? value : default(Age);
        }

        public Categories Categories
        {
            get => categories;
            set
            {
                var isValid = value >= Categories.Any && (int)value < (int)Categories.Other << 1;
                categories = isValid ? value : default(Categories);
            }
        }

        public Language Language
        {
            get => language;
            set => language = Enum.IsDefined(typeof(Language), value) ? value : default(Language);
        }

        public bool OnlyInFiles
        {
            get => onlyInFiles;
            set => onlyInFiles = value;
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
                filters.Add('"' + exactMatchTerms.Trim() + '"');

            if (minSize.IsValid && maxSize.IsValid)
                filters.Add($"{minSize.Amount}{minSize.Unit}-{maxSize.Amount}{maxSize.Unit}");
            else if (minSize.IsValid)
                filters.Add($">{minSize.Amount}{minSize.Unit}");
            else if (maxSize.IsValid)
                filters.Add($"<{maxSize.Amount}{maxSize.Unit}");

            if (age.Amount > default(int))
                filters.Add((age.IsNewer ? "before:" : "after:") + age.Amount + age.Unit.ToString()[0]);

            if (categories != default(Categories))
                filters.Add("category:" + categories.ToString().Replace(spaceS, string.Empty));

            if (language != default(Language))
            {
                // Hexavigesimal decoding of enum values (magic number to 2-letter codes)
                var lang = (int)language - 1; // TODO decrease enum values by one
                filters.Add("+lang:" + (char)(96 + lang / 26) + (char)(97 + lang % 26));
            }

            if (onlyInFiles)
                filters.Add("!onlyFiles");

            return string.Join(spaceS, filters);
        }

        private static string RemoveWhitespace(string text) =>
            string.IsNullOrWhiteSpace(text) ? string.Empty : Regex.Replace(text, @"\s{2,}", spaceS).Trim();

        private const string spaceS = " ";
        private const char spaceC = ' ';
    }
}
