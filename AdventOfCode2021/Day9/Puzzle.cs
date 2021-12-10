using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day9
{
    public class Puzzle
    {
        private readonly Location[,] locations;
        private List<Location> lowPoints = null!;
        
        public Puzzle()
        {
            var lines = File.ReadAllLines("Day9\\input.txt");
            for (var row = 0; row < lines.Length; row++)
            {
                var line = lines[row].Trim();

                var rowNumbers = lines[row].ToList();
                for (var column = 0; column < rowNumbers.Count; column++)
                {
                    int depth = Convert.ToInt32(rowNumbers[column].ToString());
                    
                    locations ??= new Location[lines.Length, line.Length];

                    locations[row, column] = new Location
                    {
                        Row = row,
                        Column = column,
                        Depth = depth,
                        Counted = false
                    };
                }
            }

            Part1();
            Part2();

            // Locations can't be null exiting constructor
            locations ??= new Location[1, 1];
        }
        
        public void Part1()
        {
            lowPoints = new List<Location>();
            for (var row = 0; row < locations.GetLength(0); row++)
                for (var column = 0; column < locations.GetLength(1); column++)
                    if (GetAdjecantNonDiagonals(row, column).All(x => x.Depth > locations[row, column].Depth))
                        lowPoints.Add(locations[row, column]);
            
            Console.Write($"The sum of all lowpoints is {lowPoints.Select(x => x.Depth + 1).Sum()}\n");
        }

        private void Part2()
        {
            var basinSizes = lowPoints
                .Select(lowPoint => GetBasinSize(lowPoint.Row, lowPoint.Column))
                .ToList();

            basinSizes.Sort();
            basinSizes.Reverse();

            var multipliedSize = basinSizes.First();
            foreach (var size in basinSizes.Skip(1).Take(2))
                multipliedSize *= size;
            
            Console.Write($"Multiplied basin size: {multipliedSize}\n");
        }

        private int GetBasinSize(int row, int column)
        {
            var size = 0;

            var adjecent = GetAdjecantNonDiagonals(row, column)
                .Where(x => !x.Counted && x.Depth != 9)
                .ToList();

            if (!adjecent.Any())
                return 0;
            
            size += adjecent.Count;

            foreach (var adjecentLocation in adjecent)
                adjecentLocation.Counted = true;

            size += adjecent.Sum(adjecentLocation => GetBasinSize(adjecentLocation.Row, adjecentLocation.Column));

            return size;
        }

        private List<Location> GetAdjecantNonDiagonals(int row, int column)
        {
            var result = new List<Location>();
            
            if (row > 0)
                result.Add(locations[row - 1, column]);

            if (row < locations.GetLength(0) - 1)
                result.Add(locations[row + 1, column]);
            
            if (column > 0)
                result.Add(locations[row, column -1]);
            
            if (column < locations.GetLength(1) - 1)
                result.Add(locations[row, column + 1]);
            
            return result;
        }
    }

    public class Location
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int Depth { get; set; }
        public bool Counted { get; set; }
    }
}