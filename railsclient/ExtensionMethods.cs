using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RailsClient.ExtensionMethods
{
    public static class StringExtensions
    {
        public static string Pluralize(this string singular)
        {
            return singular + 's';
        }

        public static string Singularize(this string plural)
        {
            return plural.Remove(plural.Length - 1);
        }
    }
}
