using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2021.Day14
{
    public class Puzzle : PuzzleBase
    {
        private string template;
        private Dictionary<string, string> insertionRules = new Dictionary<string, string>();
        private Dictionary<string, long> pairCounts = new Dictionary<string, long>();
        
        public Puzzle()
        {
            this.Init(14, false);
            var lines = this.GetPuzzleLines();

            template = lines.First();
            foreach (var insertionRule in lines.Skip(2))
            {
                var rule = insertionRule.Split(" -> ");
                insertionRules[rule[0].Trim()] = rule[1].Trim();
            }

            Part1();
            Part2();
        }

        private void Part1()
        {
            var pairs = CreatePairs(template);

            for (var i = 0; i < 10; i++)
                pairs = Step(pairs);
            
            var result = pairCounts.OrderBy(x => x.Key).ToList();
            Console.WriteLine($"Result part 1: {result.Select(x => x.Value).Max() - result.Select(x => x.Value).Min()}");
        }
        
        private void Part2()
        {
            pairCounts.Clear();
            var pairs = CreatePairs(template);

            for (var i = 0; i < 40; i++)
                pairs = Step(pairs);

            var result = pairCounts.OrderBy(x => x.Key).ToList();
            Console.WriteLine($"Result part 2: {result.Select(x => x.Value).Max() - result.Select(x => x.Value).Min()}");
        }

        private Dictionary<string, long> CreatePairs(string input)
        {
            var output = new Dictionary<string, long>();

            for (var i = 0; i < input.Length; i++)
            {
                if (i + 1 == input.Length)
                    break;

                var newPair = $"{input[i]}{input[i + 1]}";

                if (!output.ContainsKey(newPair))
                    output[newPair] = 0;

                output[newPair] += 1;
            }
            
            foreach(var letter in input.ToList())
            {
                var insert = letter.ToString();
                if (!pairCounts.ContainsKey(insert))
                    pairCounts[insert] = 0;

                pairCounts[insert]++;
            }

            return output;
        }

        private Dictionary<string, long> Step(Dictionary<string, long> pairs)
        {
            var newPairs = new Dictionary<string, long>();

            foreach (var pair in pairs)
            {
                var insert = insertionRules[pair.Key];
                string newPair1 = $"{pair.Key[0]}{insert}";
                string newPair2 = $"{insert}{pair.Key[1]}";

                if (!pairCounts.ContainsKey(insert))
                    pairCounts[insert] = 0;

                var oldValue1 = newPairs.ContainsKey(newPair1) ? newPairs[newPair1] : 0;
                var oldValue2 = newPairs.ContainsKey(newPair2) ? newPairs[newPair2] : 0;
                newPairs[newPair1] = oldValue1 + pair.Value;
                newPairs[newPair2] = oldValue2 + pair.Value;

                pairCounts[insert] += pair.Value;
            }
            
            return newPairs;
        }
    }
}