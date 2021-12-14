using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Year2020.Day3
{
    public class Puzzle : PuzzleBase
    {
        private static bool[,] Trees;
        
        public Puzzle()
        {
            this.Init(3, false);

            var lines = LinesToArray(GetPuzzleLines(), "");

            Trees = new bool[lines.Count, lines.First().Length];
            
            for(var row = 0; row < lines.Count; row++)
                for (var column = 0; column < lines[row].Length; column++)
                    if (lines[row][column] == "#")
                        Trees[row, column] = true;
            
            Part1();
            Part2();
        }

        private void Part1()
        {
            Console.WriteLine($"Part 1 - Encountered {GetNumberOfTrees(3, 1)} trees");
        }

        private void Part2()
        {
            var slopes = new[]
            {
                (1, 1),
                (3, 1),
                (5, 1),
                (7, 1),
                (1, 2)
            };

            var treesEncountered = new List<int>();
            foreach (var slope in slopes)
            {
                var result = GetNumberOfTrees(slope.Item1, slope.Item2);
                
                treesEncountered.Add(result);
                Console.WriteLine($"Encountered {result} trees on {slope.Item1} - {slope.Item2}");
            }
            
            Console.WriteLine($"Part 2 - Multiplied answer: {treesEncountered.Aggregate(1, (x,y) => x * y)}");
        }

        private int GetNumberOfTrees(int stepX, int stepY)
        {
            var numberOfTrees = 0;
            var (y, x) = (0, 0);
            
            while (y < Trees.GetLength(0))
            {
                // Grow if neccesary
                if (x > Trees.GetLength(1))
                    Trees = RepeatArrayColumns(Trees, Trees.GetLength(1));

                if (Trees[y, x])
                    numberOfTrees++;
                
                y = y + stepY;
                x = x + stepX;
            }

            return numberOfTrees;
        }

        private bool[,] RepeatArrayColumns(bool[,] input, int originalSize = 11)
        {
            var newArray = new bool[input.GetLength(0), input.GetLength(1) + originalSize];

            for (var row = 0; row < newArray.GetLength(0); row++)
            {
                for (var column = 0; column < newArray.GetLength(1); column++)
                {
                    var actualColumn = column;
                    if (column >= input.GetLength(1))
                        actualColumn = column - originalSize;
                    
                    newArray[row, column] = input[row, actualColumn];
                }
            }

            return newArray;
        }
    }
}