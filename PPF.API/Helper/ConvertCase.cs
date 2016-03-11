using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace PPF.API.Helper
{
    public static class ConvertCase
    {
        /// <summary>
        /// Convert the passed string phrase into provided Case type
        /// </summary>
        /// <param name="phrase">string which need to convert</param>
        /// <param name="cases">target case to which passing string need to convert</param>
        /// <returns></returns>
        public static string To(string phrase, Case cases)
        {
            return To(phrase, cases, new[] { ' ', '-', '.' });
        }
        /// <summary>
        /// Convert the passed string phrase into provided Case type
        /// </summary>
        /// <param name="phrase">String which need to convert</param>
        /// <param name="cases">Target case to which passing string need to convert</param>
        /// <param name="separator">Array of char separator. If do not passed it will considered as array of ' ', '-' & '.' by defaults</param>
        /// <returns></returns>
        public static string To(string phrase, Case cases, char[] separator)
        {
            var index = 0;
            string formatString = phrase;
            separator = separator == null || separator.Length == 0 ? new[] { ' ', '-', '.' } : separator;

            string[] splittedPhrase = phrase.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            var convertedString = new List<string>();
            foreach (string piece in splittedPhrase)
            {
                // Make a formating string to finally maintain the original string structure
                formatString = new Regex(piece).Replace(formatString, "{" + index++ + "}", 1);

                char[] splittedPhraseChars = piece.ToCharArray();
                if (cases == Case.CamelCase)
                {
                    splittedPhraseChars[0] = Char.ToLower(splittedPhraseChars[0]);
                }
                else if (cases == Case.PascalCase)
                {
                    splittedPhraseChars[0] = Char.ToUpper(splittedPhraseChars[0]);
                }
                convertedString.Add(new string(splittedPhraseChars));
            }
            return string.Format(formatString, convertedString.ToArray());

        }

        public enum Case
        {
            PascalCase,
            CamelCase
        }
    }


}