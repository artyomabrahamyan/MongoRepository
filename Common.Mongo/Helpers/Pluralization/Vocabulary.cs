using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Common.Mongo.Helpers.Pluralization
{
    /// <summary>
    /// A container for exceptions to simple pluralization/singularization rules.
    /// Vocabularies.Default contains an extensive list of rules for US English.
    /// At this time, multiple vocabularies and removing existing rules are not supported.
    /// </summary>
    internal class Vocabulary
    {
        private readonly List<Rule> _plurals = new List<Rule>();
        private readonly List<Rule> _singulars = new List<Rule>();
        private readonly List<string> _unCountables = new List<string>();

        /// <summary>
        /// Adds a word to the vocabulary which cannot easily be pluralized/singularized by RegEx, e.g. "person" and "people".
        /// </summary>
        /// <param name="singular">The singular form of the irregular word, e.g. "person".</param>
        /// <param name="plural">The plural form of the irregular word, e.g. "people".</param>
        /// <param name="matchEnding">True to match these words on their own as well as at the end of longer words. False, otherwise.</param>
        public void AddIrregular(string singular, string plural, bool matchEnding = true)
        {
            if (matchEnding)
            {
                AddPlural("(" + singular[0] + ")" + singular.Substring(1) + "$", "$1" + plural.Substring(1));
                AddSingular("(" + plural[0] + ")" + plural.Substring(1) + "$", "$1" + singular.Substring(1));
                return;
            }

            AddPlural($"^{singular}$", plural);
            AddSingular($"^{plural}$", singular);
        }

        /// <summary>
        /// Adds an uncountable word to the vocabulary, e.g. "fish".  Will be ignored when plurality is changed.
        /// </summary>
        /// <param name="word">Word to be added to the list of uncountables.</param>
        public void AddUncountable(string word)
        {
            _unCountables.Add(word.ToLower());
        }

        /// <summary>
        /// Adds a rule to the vocabulary that does not follow trivial rules for pluralization, e.g. "bus" -> "buses".
        /// </summary>
        /// <param name="rule">RegEx to be matched, case insensitive, e.g. "(bus)es$".</param>
        /// <param name="replacement">RegEx replacement  e.g. "$1".</param>
        public void AddPlural(string rule, string replacement)
        {
            _plurals.Add(new Rule(rule, replacement));
        }

        /// <summary>
        /// Adds a rule to the vocabulary that does not follow trivial rules for singularization, e.g. "vertices/indices -> "vertex/index".
        /// </summary>
        /// <param name="rule">RegEx to be matched, case insensitive, e.g. ""(vert|ind)ices$"".</param>
        /// <param name="replacement">RegEx replacement  e.g. "$1ex".</param>
        public void AddSingular(string rule, string replacement)
        {
            _singulars.Add(new Rule(rule, replacement));
        }

        /// <summary>
        /// Pluralizes the provided input considering irregular words.
        /// </summary>
        /// <param name="word">Word to be pluralized.</param>
        /// <param name="inputIsKnownToBeSingular">Normally you call Pluralize on singular words; but if you're unsure call it with false.</param>
        /// <returns>Result string.</returns>
        public string Pluralize(string word, bool inputIsKnownToBeSingular = true)
        {
            var result = ApplyRules(_plurals, word);

            if (inputIsKnownToBeSingular)
            {
                return result;
            }

            var asSingular = ApplyRules(_singulars, word);
            var asSingularAsPlural = ApplyRules(_plurals, asSingular);
            if (asSingular != null && asSingular != word && asSingular + "s" != word && asSingularAsPlural == word &&
                result != word)
            {
                return word;
            }

            return result;
        }

        /// <summary>
        /// Singularization of the provided input considering irregular words.
        /// </summary>
        /// <param name="word">Word to be singularized.</param>
        /// <param name="inputIsKnownToBePlural">Normally you call Singularize on plural words; but if you're unsure call it with false.</param>
        /// <returns>Result string.</returns>
        public string Singularize(string word, bool inputIsKnownToBePlural = true)
        {
            var result = ApplyRules(_singulars, word);

            if (inputIsKnownToBePlural)
            {
                return result;
            }

            // the Plurality is unknown so we should check all possibilities
            var asPlural = ApplyRules(_plurals, word);
            var asPluralAsSingular = ApplyRules(_singulars, asPlural);
            if (asPlural != word && word + "s" != asPlural && asPluralAsSingular == word && result != word)
            {
                return word;
            }

            return result ?? word;
        }

        private string ApplyRules(IList<Rule> rules, string word)
        {
            if (word == null)
            {
                return null;
            }

            if (IsUncountable(word))
            {
                return word;
            }

            var result = word;
            for (var i = rules.Count - 1; i >= 0; i--)
            {
                if ((result = rules[i].Apply(word)) != null)
                {
                    break;
                }
            }

            return result;
        }

        private bool IsUncountable(string word)
        {
            return _unCountables.Contains(word.ToLower());
        }

        private class Rule
        {
            private readonly Regex _regex;
            private readonly string _replacement;

            public Rule(string pattern, string replacement)
            {
                _regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                _replacement = replacement;
            }

            public string Apply(string word)
            {
                if (!_regex.IsMatch(word))
                {
                    return null;
                }

                return _regex.Replace(word, _replacement);
            }
        }
    }
}