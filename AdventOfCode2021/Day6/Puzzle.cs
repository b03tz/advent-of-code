using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day6
{
    public class Puzzle
    {
        public Dictionary<long, long> LanternFishes = new Dictionary<long, long>();
        
        public Puzzle()
        {
            var content = File.ReadAllText("Day6\\input.txt");

            var startingFishes = content.Split(",").Select(x => Convert.ToInt32(x)).ToArray();

            for (var i = 0; i < startingFishes.Length; i++)
            {
                var fishTimer = startingFishes[i];

                if (LanternFishes.ContainsKey(fishTimer))
                {
                    LanternFishes[fishTimer] += 1;
                    continue;
                }
                    
                LanternFishes[fishTimer] = 1;
            }

            var s = new Stopwatch();
            s.Start();
            Part1();
            
            Console.WriteLine($"Solution found in {s.ElapsedMilliseconds} ms");
        }

        private void Part1()
        {
            var totalDays = 256;
            
            for (var i = 1; i <= totalDays; i++)
            {
                Pass1Day();
            }

            long result = 0;
            foreach (KeyValuePair<long, long> pair in LanternFishes)
            {
                result += pair.Value;
            }
            
            Console.WriteLine($"256 days passed {result} fish spawned");
        }
        
        private void Pass1Day()
        {
            var newFishDictionary = new Dictionary<long, long>();
            
            foreach (KeyValuePair<long, long> fish in LanternFishes)
            {
                newFishDictionary[fish.Key - 1] = fish.Value;
            }

            if (newFishDictionary.ContainsKey(-1))
            {
                var newFishCount = newFishDictionary[-1];
                
                if (newFishDictionary.ContainsKey(6))
                    newFishDictionary[6] += newFishDictionary[-1];
                else
                    newFishDictionary[6] = newFishDictionary[-1];
                
                if (newFishDictionary.ContainsKey(8))
                    newFishDictionary[8] += newFishCount;
                else                     
                    newFishDictionary[8] = newFishCount;
                
                newFishDictionary.Remove(-1);
            }
            
            LanternFishes = newFishDictionary;
        }
    }
}