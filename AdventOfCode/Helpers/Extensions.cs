using System;

namespace AdventOfCode.Helpers
{
    public static class Extensions
    {
        public static int ToInt(this string input)
        {
            return Convert.ToInt32(input);
        }
        
        public static long ToLong(this string input)
        {
            return Convert.ToInt64(input);
        }
    }
}