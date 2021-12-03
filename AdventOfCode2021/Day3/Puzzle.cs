using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day3
{
    public class Puzzle
    {
        public Puzzle()
        {
            var content = File.ReadAllText("Day3\\input.txt");
            var lines = content.Split("\n").ToList();

            Part1(lines);
            Part2(lines);
        }

        public void Part1(List<string> lines)
        {
            List<string> gammaRateInput = new List<string>();            
            for (var i = 0; i < lines[0].Length - 1; i++)
                gammaRateInput.Add(lines.GetCommonBit(i).ToString());
            string gammaRate = String.Join("", gammaRateInput.ToArray());
            
            List<string> epsilonRateInput = new List<string>();            
            for (var i = 0; i < lines[0].Length - 1; i++)
                epsilonRateInput.Add(lines.GetCommonBit(i, true).ToString());
            string epsilonRate = String.Join("", epsilonRateInput.ToArray());

            var gammaRateInt = Convert.ToInt32(gammaRate, 2);
            var epsilonRateInt = Convert.ToInt32(epsilonRate, 2);
            
            Console.WriteLine($"Gamma rate: {gammaRateInt}");
            Console.WriteLine($"Epsilon rate: {epsilonRateInt}");
            Console.WriteLine($"Power consumption: {gammaRateInt * epsilonRateInt}");
        }

        private void Part2(List<string> lines)
        {
            var oxygenGenResult = new List<string>(lines);
            for (var i = 0; i < lines[0].Length - 1; i++)
            {
                oxygenGenResult = oxygenGenResult.Where(x =>
                {
                    return x[i].ToString() == oxygenGenResult.MostCommonBit(1, i, 1).ToString();
                }).ToList();
                
                if (oxygenGenResult.Count == 1)
                    break;
            }

            var oxygenRating = Convert.ToInt32(oxygenGenResult.First().Trim(), 2);
            
            var scrubberRatingResult = new List<string>(lines);
            for (var i = 0; i < lines[0].Length - 1; i++)
            {
                scrubberRatingResult = scrubberRatingResult.Where(
                    x => x[i].ToString() == scrubberRatingResult.MostCommonBit(0, i, 0).ToString()
                ).ToList();

                if (scrubberRatingResult.Count == 1)
                    break;
            }

            var scrubberRating = Convert.ToInt32(scrubberRatingResult.First().Trim(), 2);
            
            Console.WriteLine($"Oxygen rating: {oxygenRating}");
            Console.WriteLine($"Scrubber rating: {scrubberRating}");
            Console.WriteLine($"Life support rating: {oxygenRating * scrubberRating}");
        }
    }

    public static class ListHelper
    {
        public static int MostCommonBit(this List<string> lines, int bitToCheck, int inPosition, int keepIfEqual)
        {
            var result = (double)lines.Sum(x => Convert.ToInt32(x.Substring(inPosition, 1))) / lines.Count;
            
            if (bitToCheck == 1)
                return result == 0.5 ? keepIfEqual : Convert.ToInt32(result > 0.5);
            
            return result == 0.5 ? keepIfEqual : Convert.ToInt32(result < 0.5);
        }
        
        public static int GetCommonBit(this List<string> lines, int inPosition, bool getLeastCommon = false)
        {
            var result = (double)lines.Sum(x => Convert.ToInt32(x.Substring(inPosition, 1))) / lines.Count;
            return getLeastCommon ? Convert.ToInt32(result < 0.5) : Convert.ToInt32(result >= 0.5);
        }
    }
    
}