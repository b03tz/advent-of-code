using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day10
{
    public class Puzzle
    {
        private readonly Dictionary<string, string> validPairs = new Dictionary<string, string>
        {
            {"(", ")"},
            {"[", "]"},
            {"{", "}"},
            {"<", ">"}
        };
        
        private readonly Dictionary<string, int> scoresPart1 = new Dictionary<string, int>
        {
            {")", 3},
            {"]", 57},
            {"}", 1197},
            {">", 25137}
        };

        private readonly Dictionary<string, int> scoresPart2 = new Dictionary<string, int>
        {
            {")", 1},
            {"]", 2},
            {"}", 3},
            {">", 4}
        };
        
        public List<string> IncompleteLines = new List<string>();
        
        public Puzzle()
        {
            var lines = File.ReadAllLines("Day10\\input.txt");

            Part1(lines);
            Part2();
        }

        private void Part1(string[] lines)
        {
            var invalidTagList = new List<string>();
            foreach (var line in lines)
            {
                var tags = line.ToList().Select(x => x.ToString());
                var tagList = new List<string>();
                var isCorrupted = false;
                
                foreach (var tag in tags)
                {
                    if (IsOpenTag(tag))
                    {
                        tagList.Add(tag);
                        continue;
                    }
                    
                    // It's a close tag so match it's pair
                    var pair = tagList.Last();
                    tagList.RemoveAt(tagList.Count() - 1);

                    if (IsCorrectTag(pair, tag)) 
                        continue;
                    
                    invalidTagList.Add(tag);
                    isCorrupted = true;
                    break;
                }
                
                if (!isCorrupted)
                    IncompleteLines.Add(line);
            }

            var score = 0;
            foreach (var scorePair in scoresPart1)
                score += invalidTagList.Count(x => x == scorePair.Key) * scoresPart1[scorePair.Key];
            
            Console.WriteLine($"Syntax error score: {score}");
        }

        public void Part2()
        {
            var scoreList = new List<long>();
            foreach (var line in IncompleteLines)
            {
                var tags = line.ToList().Select(x => x.ToString());
                var tagList = new List<string>();

                foreach (var tag in tags)
                {
                    if (IsOpenTag(tag))
                    {
                        tagList.Add(tag);
                        continue;
                    }
                    
                    // It's a close tag
                    var pair = tagList.Last();
                    tagList.RemoveAt(tagList.Count - 1);
                }
                
                // We are left with some tags
                tagList.Reverse();
                scoreList.Add(CalculateLineScore(tagList));
            }

            scoreList.Sort();

            Console.WriteLine($"Middlescore: {scoreList[(scoreList.Count - 1) / 2]}");
        }

        public long CalculateLineScore(List<string> tagList)
        {
            long lineScore = 0;

            foreach (var tag in tagList)
                lineScore = lineScore * 5 + scoresPart2[GetCloseTag(tag)];

            return lineScore;
        }
        
        public bool IsOpenTag(string tag)
        {
            return validPairs.ContainsKey(tag);
        }

        public bool IsCorrectTag(string tag, string comparison)
        {
            if (validPairs.ContainsKey(tag))
                if (validPairs[tag] == comparison)
                    return true;

            return validPairs.FirstOrDefault(x => x.Value == tag).Key == tag;
        }
        
        public string GetCloseTag(string openTag)
        {
            return validPairs[openTag];
        }
    }
}