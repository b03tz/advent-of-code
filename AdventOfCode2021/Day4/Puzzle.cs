using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day4
{
    public class Puzzle
    {
        private List<int> CalledNumbers = new List<int>();
        private List<BingoGrid[,]> BingoGrids = new List<BingoGrid[,]>();
        
        public Puzzle()
        {
            var content = File.ReadAllText("Day4\\input.txt");
            var lines = content.Split("\n").ToList();
            
            CalledNumbers.AddRange(lines.First().Split(',').Select(x => Convert.ToInt32(x)));

            ParseBingoGrids(lines);

            Part1(lines);
            Part2(lines);
        }
        
        private void Part1(List<string> lines)
        {
            foreach (var call in CalledNumbers)
            {
                CallNumber(call);
                
                var winningGrid = CheckFirstWinningGrid();

                if (winningGrid != null)
                {
                    Console.WriteLine($"Final score: {call * CalculateGridSum(winningGrid)}");
                    break;
                }
            }
        }
        
        private void Part2(List<string> lines)
        {
            var winningBoards = new List<BingoGrid[,]>();
            var lastCall = 0;
            
            foreach (var call in CalledNumbers)
            {
                CallNumber(call, winningBoards);

                foreach (BingoGrid[,] grid in BingoGrids)
                {
                    if (!winningBoards.Contains(grid) && CheckIfGridWon(grid))
                    {
                        lastCall = call;
                        winningBoards.Add(grid);
                    }
                }
            }

            var lastBoard = winningBoards.Last();
            Console.WriteLine($"Sum of last board to win: {lastCall * CalculateGridSum(lastBoard)}");            
        }

        private int CalculateGridSum(BingoGrid[,] grid)
        {
            var sum = 0;
            for (int i = 0; i < grid.GetLength(0); i++)
            for (int j = 0; j < grid.GetLength(0); j++)
                if (!grid[i, j].Called)
                    sum += grid[i, j].Number;

            return sum;
        }

        private void CallNumber(int number, List<BingoGrid[,]> gridsToSkip = null)
        {
            foreach (var grid in BingoGrids)
            {
                if (gridsToSkip != null && gridsToSkip.Contains(grid))
                    continue;
                
                for (var r = 0; r < 5; r++)
                    for (var c = 0; c < 5; c++)
                        if (grid[r, c].Number == number)
                            grid[r, c].Called = true;
            }
        }

        private BingoGrid[,] CheckFirstWinningGrid()
        {
            foreach (var grid in BingoGrids)
                if (CheckIfGridWon(grid))
                    return grid;

            return null;
        }

        private bool CheckIfGridWon(BingoGrid[,] grid)
        {
            // Check for winning column
            for (var r = 0; r < 5; r++)
            {
                var columnsCompleted = true;
                
                for (var c = 0; c < 5; c++)
                    if (grid[r, c].Called == false)
                        columnsCompleted = false;

                if (columnsCompleted)
                    return true;
            }
            
            // Check for winning row
            for (var r = 0; r < 5; r++)
            {
                var rowCompleted = true;
                
                for (var c = 0; c < 5; c++)
                    if (grid[c, r].Called == false)
                        rowCompleted = false;

                if (rowCompleted)
                    return true;
            }

            return false;
        }

        private void ParseBingoGrids(List<string> lines)
        {
            lines = lines.Skip(2).ToList();

            var currentBingoRow = 0;
            var currentBingoGrid = new BingoGrid[5,5];
            while (true)
            {
                if (!lines.Any())
                    break;

                var currentLine = lines.First().Trim();
                lines = lines.Skip(1).ToList();

                if (string.IsNullOrWhiteSpace(currentLine))
                {
                    currentBingoRow = 0;
                    BingoGrids.Add(currentBingoGrid);
                    currentBingoGrid = new BingoGrid[5,5];
                    continue;
                }

                var parsedColumns = currentLine.Replace("  ", " ").Split(' ').Select(x => Convert.ToInt32(x)).ToList();

                for (var i = 0; i < parsedColumns.Count(); i++)
                {
                    var foo = new BingoGrid
                    {
                        Number = parsedColumns[i],
                        Called = false
                    };
                    currentBingoGrid[currentBingoRow, i] = foo;
                }

                currentBingoRow += 1;
            }
            
            BingoGrids.Add(currentBingoGrid);
        }
    }

    public class BingoGrid
    {
        public int Number { get; set; }
        public bool Called { get; set; }
    }
}