using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day_2
{
    public class Day2
    {
        public Day2()
        {
            var content = File.ReadAllText("Day 2\\input.txt");
            var lines = content.Split("\n").Select(MapLine).ToList();

            Part1(lines);
            Part2(lines);
        }

        private (Direction, int) MapLine(string line)
        {
            var parts = line.Split(' ');
            var count = Convert.ToInt32(parts[1].Trim());
            
            Direction direction;
            switch (parts[0].Trim().ToLower())
            {
                case "forward":
                    return (Direction.Forward, count);
                case "up":
                    return (Direction.Up, count);
                case "down":
                    return (Direction.Down, count);
            }

            return (Direction.Down, 0);
        }

        public void Part1(List<(Direction, int)> input)
        {
            var x = 0;
            var y = 0;

            foreach (var line in input)
            {
                switch (line.Item1)
                {
                    case Direction.Forward:
                        x += line.Item2;
                        break;
                    case Direction.Down:
                        y += line.Item2;
                        break;
                    case Direction.Up:
                        y -= line.Item2;
                        break;
                }
            }
            
            Console.WriteLine($"Total x: {x}, total y: {y}, multiplied: {x * y}");
        }

        public void Part2(List<(Direction, int)> input)
        {
            var x = 0;
            var y = 0;
            var aim = 0;

            foreach (var line in input)
            {
                switch (line.Item1)
                {
                    case Direction.Forward:
                        x += line.Item2;
                        y += (aim * line.Item2);
                        break;
                    case Direction.Down:
                        aim += line.Item2;
                        break;
                    case Direction.Up:
                        aim -= line.Item2;
                        break;
                }
            }
            
            Console.WriteLine($"Total x: {x}, total y: {y}, multiplied: {x * y}");
        }
    }
}