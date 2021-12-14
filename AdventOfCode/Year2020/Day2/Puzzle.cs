using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Year2020.Day2
{
    public class Puzzle : PuzzleBase
    {
        public Puzzle()
        {
            this.Init(2, false);

            var lines = GetPuzzleLines();

            var policies = lines.Select(MapPolicy).ToList();
            
            Part1(policies);
            Part2(policies);
        }

        private static void Part1(List<Policy> policies)
        {
            foreach (var policy in policies)
            {
                var matches = Regex.Matches(policy.Password, policy.Substring).Count;

                if (matches <= policy.Max && matches >= policy.Min)
                    policy.IsCorrect = true;
            }
            
            Console.WriteLine($"Correct policies: {policies.Count(x => x.IsCorrect)}");
        }

        private static void Part2(List<Policy> policies)
        {
            // Reset
            policies.ForEach(x => x.IsCorrect = false);
            
            foreach (var policy in policies)
            {
                var char0 = policy.Password.Substring(policy.Min - 1, 1);
                var char1 = policy.Password.Substring(policy.Max - 1, 1);

                if ((char0 == policy.Substring || char1 == policy.Substring) && char0 != char1)
                    policy.IsCorrect = true;
            }
            
            Console.WriteLine($"Correct policies: {policies.Count(x => x.IsCorrect)}");
        }

        private Policy MapPolicy(string input)
        {
            var splittedInput = input.Split(": ");
            
            var splitted = splittedInput[0].Split(' ');
            var minMax = splitted[0].Split('-');
            return new Policy
            {
                Min = Convert.ToInt32(minMax[0]),
                Max = Convert.ToInt32(minMax[1]),
                Substring = splitted[1],
                Password = splittedInput[1]
            };
        }
    }

    public class Policy
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public string Substring { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsCorrect { get; set; } = false;
    }
}