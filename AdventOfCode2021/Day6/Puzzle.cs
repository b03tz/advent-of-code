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

            foreach (var fishTimer in startingFishes)
            {
                if (!LanternFishes.ContainsKey(fishTimer))
                    LanternFishes[fishTimer] = 0;
                    
                LanternFishes[fishTimer] += 1;
            }

            Part1();
        }

        private void Part1()
        {
            const int totalDays = 256;
            
            for (var i = 1; i <= totalDays; i++)
                Pass1Day();

            BigInteger result = 0;
            foreach (KeyValuePair<int, BigInteger> fish in LanternFishes)
                result += fish.Value;
            
            Console.WriteLine($"{totalDays} days passed {result} fish spawned");
        }
        
        private void Pass1Day()
        {
            // Decrease the timers into a new dictionary
            var newFishDictionary = new Dictionary<int, BigInteger>();
            
            foreach (var (timer, numberOfFish) in LanternFishes)
                newFishDictionary[timer - 1] = numberOfFish;
            
            LanternFishes = newFishDictionary;

            // If no timers expired; nothing needs to be done
            if (!LanternFishes.ContainsKey(-1))
                return;
            
            // If there are no fishes currently on timer 6 just set it to 0
            if (!LanternFishes.ContainsKey(6))
                LanternFishes[6] = 0;
                
            // Reset the number of fishes that are currently timer value -1 to 6 (they just spawned a fish)
            LanternFishes[6] += LanternFishes[-1];

            // If there are no new fishes set the 8 timer to 0
            if (!LanternFishes.ContainsKey(8))
                LanternFishes[8] = 0;
            
            // Add to the new fishes the number of new spawned fishes (the number in the -1 timer slot)
            LanternFishes[8] += LanternFishes[-1];
            
            // Remove the -1 timer slot, the fish have already been reset to the 6 timer slot above
            LanternFishes.Remove(-1);
        }
    }
}