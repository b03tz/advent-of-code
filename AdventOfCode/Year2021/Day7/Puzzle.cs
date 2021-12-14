using System;
using System.IO;
using System.Linq;

namespace AdventOfCode.Year2021.Day7
{
    public class Puzzle : PuzzleBase
    {
        private readonly int[] crabPositions;
        
        public Puzzle()
        {
            this.Day = 7;
            var content = File.ReadAllText(GetPuzzleFilename());
            crabPositions = content.Split(',').Select(x => Convert.ToInt32(x)).ToArray();

            Part1();
            Part2();
        }

        public void Part1()
        {
            var heighest = crabPositions.Max();
            var lowest = crabPositions.Min();

            // First calculate the cheapest starting position by increments of 50 and then the actual accurate position
            var (startingValue, uselessCost) = CalculateCheapestPosition(50, lowest, heighest);
            var (cheapestPosition, cost) = CalculateCheapestPosition(1, startingValue - 50, heighest);
            
            Console.WriteLine($"The cheapest position = {cheapestPosition} for {cost} fuel");
        }
        
        public void Part2()
        {
            var heighest = crabPositions.Max();
            var lowest = crabPositions.Min();
            
            // First calculate the cheapest starting position by increments of 50 and then the actual accurate position
            var (startingPosition, uselessCost) = CalculateCheapestPosition(50, lowest, heighest, true);
            var (cheapestPosition, cost) = CalculateCheapestPosition(1, startingPosition - 50, heighest, true);
            
            Console.WriteLine($"The cheapest position = {cheapestPosition} for {cost} fuel");
        }

        private (int, int) CalculateCheapestPosition(int increment, int lowest, int heighest, bool fuelCostIncreases = false)
        {
            var cheapestCost = Int32.MaxValue;
            var cheapestPosition = 0;
            
            for (var s = lowest; s <= (heighest + increment); s += increment)
            {
                var cost = crabPositions
                    .Where(x => x != s)
                    .Select(x => CalculateCost(Math.Abs(s - x), fuelCostIncreases))
                    .Sum();

                if (cost < cheapestCost)
                {
                    cheapestCost = cost;
                    cheapestPosition = s;
                    continue;
                }
                
                // It's getting more expensive so break
                break;
            }
            
            return (cheapestPosition, cheapestCost);
        }

        private static int CalculateCost(int travelDistance, bool fuelCostIncreases)
        {
            if (!fuelCostIncreases)
                return travelDistance;
            
            var cost = 0;
            for (var i = 1; i <= travelDistance; i++)
                cost += i;

            return cost;
        }
    }
}