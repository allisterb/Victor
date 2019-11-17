using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Victor
{
    public static class StringExtensions
    {
        public static bool IsEmpty(this string s) => s == "";

        public static bool IsNotEmpty(this string s) => s != "";

        public static string ToAlphaNumeric(this string s) => new string(s.Where(c => Char.IsLetterOrDigit(c)).ToArray());

        
    }
}
