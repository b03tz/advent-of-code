using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode.Year2021.Day25
{
    public class Puzzle : PuzzleBase
    {
        private char[,] map;
        private const char East = '>';
        private const char South = 'v';    
        
        public Puzzle()
        {
            this.Init(25, false);
            var lines = this.GetPuzzleLines();

            map = new char[lines.Count(),lines.First().Length];
            for (var row = 0; row < lines.Count(); row++)
            {
                var charRow = lines[row].ToCharArray();
                for (var col = 0; col < charRow.Length; col++)
                {
                    map[row, col] = charRow[col];
                }
            }

            Part1(map);
        }

        private void Part1(char[,] fishArray)
        {
            var iterations = 1;

            Console.WriteLine(" ");
            while (true)
            {
                var stepEast = StepEast(fishArray);
                var stepSouth = StepSouth(stepEast.newArray);
                
                if (!stepEast.changed && !stepSouth.changed)
                    break;

                fishArray = stepSouth.newArray;
                iterations++;
            }
            
            Console.WriteLine(iterations);
        }

        private (bool changed, char[,] newArray) StepEast(char[,] fishArray)
        {
            var newArray = new char[fishArray.GetLength(0), fishArray.GetLength(1)];
            
            var isUpdated = false;
            var movedEast = new HashSet<(int, int)>();
            for (var row = fishArray.GetLength(0) - 1; row >= 0; row--)
            {
                for (var col = fishArray.GetLength(1) - 1; col >= 0; col--)
                {
                    var contains = movedEast.Contains((row, col));
                    if (fishArray[row, col] != East && !contains)
                    {
                        newArray[row, col] = fishArray[row, col];
                        continue;
                    }

                    if (contains)
                        continue;

                    var r = CanMoveRight(fishArray, row, col);
                    if (r.canMove)
                    {
                        isUpdated = true;
                        newArray[row, col] = '.';
                        newArray[row, r.nextCol] = East;
                        movedEast.Add((row, r.nextCol));
                        continue;
                    }
                    
                    newArray[row, col] = fishArray[row, col];
                }
            }

            return (isUpdated, newArray);
        }

        public (bool changed, char[,] newArray) StepSouth(char[,] fishArray)
        {
            var isUpdated = false;
            var newArray = new char[fishArray.GetLength(0), fishArray.GetLength(1)];
            
            var movedSouth = new HashSet<(int, int)>();
            for (var row = fishArray.GetLength(0) - 1; row >= 0; row--)
            {
                for (var col = fishArray.GetLength(1) - 1; col >= 0; col--)
                {
                    var contains = movedSouth.Contains((row, col));
                    if (fishArray[row, col] != South && !contains)
                    {
                        newArray[row, col] = fishArray[row, col];
                        continue;
                    }

                    if (contains)
                        continue;

                    var r = CanMoveDown(fishArray, row, col);
                    if (r.canMove)
                    {
                        isUpdated = true;
                        newArray[row, col] = '.';
                        newArray[r.nextRow, col] = South;
                        movedSouth.Add((r.nextRow, col));
                        continue;
                    }

                    newArray[row, col] = fishArray[row, col];
                }
            }

            return (isUpdated, newArray);
        }

        private (bool canMove, int nextRow) CanMoveDown(char[,] fishArray, int row, int col)
        {
            var nextChar = ' ';
            var nextRow = 0;

            if (row < fishArray.GetLength(0) - 1)
            {
                nextRow = row + 1;
            }

            nextChar = fishArray[nextRow, col];
            return nextChar == '.' ? (true, nextRow) : (false, nextRow);
        }

        private (bool canMove, int nextCol) CanMoveRight(char[,] fishArray, int row, int col)
        {
            var nextChar = ' ';
            var nextCol = 0;
            
            if (col < fishArray.GetLength(1) - 1)
                nextCol = col + 1;

            nextChar = fishArray[row, nextCol];
            return nextChar == '.' ? (true, nextCol) : (false, nextCol);
        }

        private void PrintMap(char[,] fishArray)
        {
            for (var row = 0; row < fishArray.GetLength(0); row++)
            {
                for (var col = 0; col < fishArray.GetLength(1); col++)
                {
                    Console.Write(fishArray[row,col]);
                }
                Console.Write("\n");
            }
            Console.Write("\n");
        }
    }
}