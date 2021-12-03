﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day2
{
    public class Puzzle
    {
        public Puzzle()
        {
            var content = File.ReadAllText("Day2\\input.txt");
            var lines = content.Split("\n").Select(MapLine).ToList();

            Part1(lines);
            Part2(lines);
        }

        private (string, int) MapLine(string line)
        {
            var parts = line.Split(' ');
            var count = Convert.ToInt32(parts[1].Trim());

            return (parts[0].Trim().ToLower(), count);
        }

        public static void Part1(List<(string, int)> input)
        {
            var x = 0;
            var y = 0;

            foreach (var line in input)
            {
                switch (line.Item1)
                {
                    case "forward":
                        x += line.Item2;
                        break;
                    case "down":
                        y += line.Item2;
                        break;
                    case "up":
                        y -= line.Item2;
                        break;
                }
            }
            
            Console.WriteLine($"Total x: {x}, total y: {y}, multiplied: {x * y}");
        }

        public static void Part2(List<(string, int)> input)
        {
            var x = 0;
            var y = 0;
            var aim = 0;

            foreach (var line in input)
            {
                switch (line.Item1)
                {
                    case "forward":
                        x += line.Item2;
                        y += (aim * line.Item2);
                        break;
                    case "down":
                        aim += line.Item2;
                        break;
                    case "up":
                        aim -= line.Item2;
                        break;
                }
            }
            
            Console.WriteLine($"Total x: {x}, total y: {y}, multiplied: {x * y}");
        }
    }
}