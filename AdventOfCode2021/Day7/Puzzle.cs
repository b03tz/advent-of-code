using System;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day7
{
    public class Puzzle
    {
        private int[] CrabPositions;
        
        public Puzzle()
        {
            var content = File.ReadAllText("Day7\\input.txt");
            
            CrabPositions = content.Split(',').Select(x => Convert.ToInt32(x)).ToArray();

            Part1();
            Part2();
        }

        public void Part1()
        {
            var heighest = CrabPositions.Max();
            var lowest = CrabPositions.Min();

            // Find the cheapest goto position
            int leastCost = Int32.MaxValue;
            int gotoPos = 0;
            for (var i = lowest; i <= heighest; i++)
            {
                var cost = CrabPositions
                    .Where(x => x != i)
                    .Select(x => Math.Abs(i - x))
                    .Sum();

                if (cost < leastCost)
                {
                    gotoPos = i;
                    leastCost = cost;
                }
            }
            
            Console.WriteLine($"The cheapest position = {gotoPos} for {leastCost} fuel");
        }
        
        public void Part2()
        {
            var heighest = CrabPositions.Max();
            var lowest = CrabPositions.Min();

            // Find the cheapest goto position
            int leastCost = Int32.MaxValue;
            int gotoPos = 0;
            for (var i = lowest; i <= heighest; i++)
            {
                var cost = CrabPositions
                    .Where(x => x != i)
                    .Select(x => CalculateCost(Math.Abs(i - x)))
                    .Sum();

                if (cost < leastCost)
                {
                    gotoPos = i;
                    leastCost = cost;
                }
            }
            
            Console.WriteLine($"The cheapest position = {gotoPos} for {leastCost} fuel");
        }

        private int CalculateCost(int travelDistance)
        {
            var cost = 0;
            for (var i = 1; i <= travelDistance; i++)
                cost += i;

            return cost;
        }
    }
}