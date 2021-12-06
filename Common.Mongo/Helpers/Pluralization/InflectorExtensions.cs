using System.Text.RegularExpressions;

namespace Common.Mongo.Helpers.Pluralization
{
    /// <summary>
    /// Inflector extensions.
    /// </summary>
    internal static class InflectorExtensions
    {
        /// <summary>
        /// Pluralizes the provided input considering irregular words.
        /// </summary>
        /// <param name="word">Word to be pluralized.</param>
        /// <param name="inputIsKnownToBeSingular">Normally you call Pluralize on singular words; but if you're unsure call it with false.</param>
        /// <returns>Result string.</returns>
        public static string Pluralize(this string word, bool inputIsKnownToBeSingular = true)
        {
            return Vocabularies.Default.Pluralize(word, inputIsKnownToBeSingular);
        }

        /// <summary>
        /// Singularization of the provided input considering irregular words.
        /// </summary>
        /// <param name="word">Word to be singularized.</param>
        /// <param name="inputIsKnownToBePlural">Normally you call Singularize on plural words; but if you're unsure call it with false.</param>
        /// <returns>Result string.</returns>
        public static string Singularize(this string word, bool inputIsKnownToBePlural = true)
        {
            return Vocabularies.Default.Singularize(word, inputIsKnownToBePlural);
        }

        /// <summary>
        /// By default, pascalize converts strings to UpperCamelCase also removing underscores.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Result string.</returns>
        public static string Pascalize(this string input)
        {
            return Regex.Replace(input, "(?:^|_)(.)", match => match.Groups[1].Value.ToUpper());
        }

        /// <summary>
        /// Same as Pascalize except that the first character is lower case.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Result string.</returns>
        public static string Camelize(this string input)
        {
            var word = input.Pascalize();
            return word.Substring(0, 1).ToLower() + word.Substring(1);
        }

        /// <summary>
        /// Separates the input words with underscore.
        /// </summary>
        /// <param name="input">The string to be underscored.</param>
        /// <returns>Result string.</returns>
        public static string Underscore(this string input)
        {
            return Regex.Replace(
                Regex.Replace(
                    Regex.Replace(input, @"([A-Z]+)([A-Z][a-z])", "$1_$2"), @"([a-z\d])([A-Z])", "$1_$2"), @"[-\s]",
                "_").ToLower();
        }

        /// <summary>
        /// Replaces underscores with dashes in the string.
        /// </summary>
        /// <param name="underscoredWord">Underscored string.</param>
        /// <returns>Result string.</returns>
        public static string Dasherize(this string underscoredWord)
        {
            return underscoredWord.Replace('_', '-');
        }

        /// <summary>
        /// Replaces underscores with hyphens in the string.
        /// </summary>
        /// <param name="underscoredWord">Underscored string.</param>
        /// <returns>Result string.</returns>
        public static string Hyphenate(this string underscoredWord)
        {
            return underscoredWord.Dasherize();
        }
    }
}