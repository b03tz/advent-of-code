using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace AdventOfCode2021.Day5
{
    public class Puzzle
    {
        public List<Coordinate> Coordinates = new List<Coordinate>();
        
        public Puzzle()
        {
            var content = File.ReadAllText("Day5\\input.txt");
            var lines = content.Split("\n").ToList();
            
            ParseInput(lines);
            
            var maxX = Math.Max(Coordinates.Select(x => x.SourceX).Max(), Coordinates.Select(x => x.DestinationX).Max()) + 1;
            var maxY = Math.Max(Coordinates.Select(x => x.SourceY).Max(), Coordinates.Select(x => x.DestinationY).Max()) + 1;
            
            int[,] grid = new int[maxX, maxY];
            Part1(grid);
            
            grid = new int[maxX, maxY];
            Part2(grid);
        }

        private void Part1(int[,] grid)
        {
            DrawLines(grid);
            
            //PrintGrid(grid); // only do this for the test grid ;p
            Console.WriteLine($"Number of overlaps: {CountOverlaps(grid)}");
        }
        
        private void Part2(int[,] grid)
        {
            DrawLines(grid, true);

            //PrintGrid(grid); // only do this for the test grid ;p
            Console.WriteLine($"Number of overlaps: {CountOverlaps(grid)}");
        }

        private int CountOverlaps(int[,] grid)
        {
            
            var overlaps = 0;
            for (var y = 0; y < grid.GetLength(1); y++)
            for (var x = 0; x < grid.GetLength(0); x++)
                if (grid[x, y] > 1)
                    overlaps++;

            return overlaps;
        }

        private void DrawLines(int[,] grid, bool includeDiagonal = false)
        {
            Coordinates.ForEach((c) =>
            {
                if (!includeDiagonal && c.SourceX != c.DestinationX && c.SourceY != c.DestinationY)
                    return;

                int from, to;
                
                // Vertical lines
                if (c.SourceX == c.DestinationX)
                {
                    from = c.SourceY;
                    to = c.DestinationY;
                    
                    if (c.DestinationY < c.SourceY)
                    {
                        from = c.DestinationY;
                        to = c.SourceY;
                    }
                    
                    for (var i = from; i <= to; i++)
                        grid[c.SourceX, i] += 1;
                    
                    return;
                }

                // Horizontal lines
                if (c.SourceY == c.DestinationY)
                {
                    from = c.SourceX;
                    to = c.DestinationX;
                    
                    if (c.DestinationX < c.SourceX)
                    {
                        from = c.DestinationX;
                        to = c.SourceX;
                    }
                    
                    for (var i = from; i <= to; i++)
                        grid[i, c.SourceY] += 1;

                    return;
                }
                
                // Diagonal lines
                var fromX = c.SourceX;
                var fromY = c.SourceY;
                var toX = c.DestinationX;
                var toY = c.DestinationY;
                
                if (c.SourceX > c.DestinationX)
                {
                    fromX = c.DestinationX;
                    fromY = c.DestinationY;
                    toX = c.SourceX;
                    toY = c.SourceY;
                }

                // Down-right
                if (fromY < toY)
                {
                    while (fromX <= toX)
                    {
                        grid[fromX, fromY]++;
                        
                        fromX++;
                        fromY++;
                    }

                    return;
                }
                
                // Up right
                while (fromX <= toX)
                {
                    grid[fromX, fromY]++;
                        
                    fromX++;
                    fromY--;
                }
            });
        }

        private void ParseInput(List<string> lines)
        {
            Coordinates = lines
            .Select(x => x.Replace(" -> ", ",").Split(','))
            .Select(x => new Coordinate(
                Convert.ToInt32(x[0]),
                Convert.ToInt32(x[1]),
                Convert.ToInt32(x[2]),
                Convert.ToInt32(x[3])
            )).ToList();
        }
        
        private void PrintGrid(int[,] grid)
        {
            for (var y = 0; y < grid.GetLength(1); y++)
            {
                var lineOutput = "";
                
                for (var x = 0; x < grid.GetLength(0); x++)
                    lineOutput += grid[x,y] == 0 ? "." : grid[x,y];
                
                Console.WriteLine(lineOutput);
            }
        }
    }

    public record Coordinate(int SourceX, int SourceY, int DestinationX, int DestinationY);
}