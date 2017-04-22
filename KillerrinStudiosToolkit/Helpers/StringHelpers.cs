using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KillerrinStudiosToolkit.Helpers
{
    public static class StringHelpers
    {
        public static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.-]+");
            return !regex.IsMatch(text);
        }

        public static string AddSpacesToSentence(string text, bool preserveAcronyms)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }

        public static double SimilarTo(this string string1, string string2)
        {
            string[] splitString1 = string1.Split(' ');
            string[] splitString2 = string2.Split(' ');
            var strCommon = splitString1.Intersect(splitString2);
            //Formula : Similarity (%) = 100 * (CommonItems * 2) / (Length of String1 + Length of String2)
            double Similarity = (double)(100 * (strCommon.Count() * 2)) / (splitString1.Length + splitString2.Length);
            return Similarity;
        }
    }
}
