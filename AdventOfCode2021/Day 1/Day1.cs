using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day_1
{
    public class Day1
    {
        public Day1()
        {
            var content = File.ReadAllText("Day 1\\input.txt");
            var lines = content.Split("\n").Select(x => Convert.ToInt32(x)).ToList();
            
            Part1(lines);
            Part2(lines);
        }

        private static void Part1(List<int> lines)
        {
            int? previous = null;
            int increases = 0;
            
            foreach (var line in lines)
            {
                if (previous == null)
                {
                    previous = line;
                    continue;
                }

                if (line > previous)
                    increases++;
                
                previous = line;
            }
            
            Console.WriteLine($"Total number of increases: {increases}");
        }

        private static void Part2(List<int> lines)
        {
            int[] slidingWindow = { 0, 1, 2 };

            int? previous = null;
            int increases = 0;

            while (slidingWindow[2] < lines.Count)
            {
                var result = lines[slidingWindow[0]] + lines[slidingWindow[1]] + lines[slidingWindow[2]];

                if (previous == null)
                {
                    previous = result;
                    continue;
                }

                if (result > previous)
                    increases++;

                previous = result;
                slidingWindow[0]++;
                slidingWindow[1]++;
                slidingWindow[2]++;
            }
            
            Console.WriteLine($"Total number of sliding window increases: {increases}");
        }
    }
}