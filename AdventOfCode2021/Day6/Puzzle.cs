using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;

namespace AdventOfCode2021.Day6
{
    public class Puzzle
    {
        public Dictionary<int, BigInteger> LanternFishes = new Dictionary<int, BigInteger>();
        
        public Puzzle()
        {
            var content = File.ReadAllText("Day6\\input.txt");

            var startingFishes = content.Split(",").Select(x => Convert.ToInt32(x)).ToArray();

            for (var i = 0; i < startingFishes.Length; i++)
            {
                var fishTimer = startingFishes[i];

                if (!LanternFishes.ContainsKey(fishTimer))
                    LanternFishes[fishTimer] = 0;
                    
                LanternFishes[fishTimer] += 1;
            }

            Part1();
        }

        private void Part1()
        {
            var totalDays = 256;
            
            for (var i = 1; i <= totalDays; i++)
                Pass1Day();

            BigInteger result = 0;
            foreach (KeyValuePair<int, BigInteger> fish in LanternFishes)
                result += fish.Value;
            
            Console.WriteLine($"{totalDays} days passed {result} fish spawned");
        }
        
        private void Pass1Day()
        {
            var newFishDictionary = new Dictionary<int, BigInteger>();
            
            foreach (KeyValuePair<int, BigInteger> fish in LanternFishes)
                newFishDictionary[fish.Key - 1] = fish.Value;
            
            LanternFishes = newFishDictionary;

            if (!LanternFishes.ContainsKey(-1))
                return;
            
            if (!LanternFishes.ContainsKey(6))
                LanternFishes[6] = 0;
                
            LanternFishes[6] += LanternFishes[-1];

            if (!LanternFishes.ContainsKey(8))
                LanternFishes[8] = 0;
            
            LanternFishes[8] += LanternFishes[-1];
            
            LanternFishes.Remove(-1);
        }
    }
}