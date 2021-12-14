using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Year2020.Day1
{
    public class Puzzle : PuzzleBase
    {
        public Puzzle()
        {
            this.Init(1, false);

            var lines = LinesToInts(GetPuzzleLines()).ToList();

            Part1(lines);
            Part2(lines);
        }

        private static void Part1(List<int> lines)
        {
            foreach (var source in lines)
            {
                var result = lines.FirstOrDefault(x => x != source && x + source == 2020);

                if (result != 0)
                {
                    Console.WriteLine($"2 entries: {source} * {result} = {source * result}");
                    break;
                }
            }
        }

        private static void Part2(List<int> lines)
        {
            var found = false;
            foreach (var x in lines)
            {
                foreach (var y in lines)
                {
                    var result = lines.FirstOrDefault(z => z != x && z != y && x + y + z == 2020);

                    if (result != 0)
                    {
                        Console.WriteLine($"3 entries: {x}, {y}, {result} = {x * y * result}");
                        found = true;
                        break;
                    }
                }

                if (found)
                    break;
            }
        }
    }
}