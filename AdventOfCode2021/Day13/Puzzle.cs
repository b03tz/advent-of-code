using System;
using System.Linq;

namespace AdventOfCode2021.Day13
{
    public class Puzzle : PuzzleBase
    {
        private bool[,] Grid;
        
        public Puzzle()
        {
            this.Init(13, false);
            var lines = ArrayLinesToInts(LinesToArray(this.GetPuzzleLines(), ","));

            var maxX = lines.Select(x => x[0]).Max();
            var maxY = lines.Select(x => x[1]).Max();

            Grid = new bool[maxY + 1, maxX + 1];

            foreach (var coordinate in lines)
                Grid[coordinate[1], coordinate[0]] = true;

            Part1();
            Part2();
        }

        private void Part2()
        {
            var newArray = FoldX(655, Grid);
            newArray = FoldY(447, newArray);
            newArray = FoldX(327, newArray);
            newArray = FoldY(223, newArray);
            newArray = FoldX(163, newArray);
            newArray = FoldY(111, newArray);
            newArray = FoldX(81, newArray);
            newArray = FoldY(55, newArray);
            newArray = FoldX(40, newArray);
            newArray = FoldY(27, newArray);
            newArray = FoldY(13, newArray);
            newArray = FoldY(6, newArray);
            
            Console.WriteLine("Part 2 code:");
            PrintGrid(newArray);
        }

        private void Part1()
        {
            var newArray = FoldX(655, Grid);

            var dots = 0;
            for (var i = 0; i < newArray.GetLength(0); i++)
                for(var x = 0; x < newArray.GetLength(1); x++)
                    if (newArray[i, x])
                        dots++;
            
            Console.WriteLine($"Part 1: total number of dots: {dots}");
        }

        private bool[,] FoldY(int location, bool[,] array)
        {
            var newArray = GetRows(array, location);
            var belowFoldArray = GetRows(array, array.GetLength(0) - location - 1, location + 1);

            for (var i = belowFoldArray.GetLength(0) - 1; i >= 0; i--)
            {
                var mirroredRow = newArray.GetLength(0) - i - 1;
                for (var col = 0; col < belowFoldArray.GetLength(1); col++)
                    if (belowFoldArray[i, col])
                        newArray[mirroredRow, col] = true;
            }
            
            return newArray;
        }
        
        private bool[,] FoldX(int location, bool[,] array)
        {
            var newArray = GetColumns(array, location);
            var rightOfFoldArray = GetColumns(array, array.GetLength(1) - location - 1, location + 1);

            for (var row = 0; row < rightOfFoldArray.GetLength(0); row++)
                for (var col = rightOfFoldArray.GetLength(1) - 1; col >= 0; col--)
                {
                    var mirroredColumn = newArray.GetLength(1) - col - 1;
                    if (rightOfFoldArray[row, col])
                        newArray[row, mirroredColumn] = true;
                }

            return newArray;
        }

        private bool[,] GetRows(bool[,] array, int rows, int startRow = 0)
        {
            var newArray = new bool[rows, array.GetLength(1)];

            var rowToAdd = startRow;
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < array.GetLength(1); col++)
                    newArray[row, col] = array[rowToAdd, col];

                rowToAdd++;
            }

            return newArray;
        }
        
        private bool[,] GetColumns(bool[,] array, int columns, int startColumn = 0)
        {
            var newArray = new bool[array.GetLength(0), columns];

            for (var row = 0; row < array.GetLength(0); row++)
            {
                var columnToAdd = startColumn;
                for (var col = 0; col < columns; col++)
                {
                    newArray[row, col] = array[row, columnToAdd];
                    columnToAdd++;
                }
            }

            return newArray;
        }

        public void PrintGrid(bool[,] grid)
        {
            for (var y = 0; y < grid.GetLength(0); y++)
            {
                var line = "";
                for (var x = 0; x < grid.GetLength(1); x++)
                {
                    if (grid[y, x])
                    {
                        line += "█";
                        continue;
                    }

                    line += " ";
                }
                
                Console.WriteLine(line);
            }
        }
    }
}