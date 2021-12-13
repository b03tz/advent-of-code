using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2021.Day11;

namespace AdventOfCode2021.Day12
{
    public class Puzzle : PuzzleBase
    {
        private Dictionary<string, Cave> CaveSystem = new Dictionary<string, Cave>();
        private List<string> FoundPaths = new List<string>();
        
        public Puzzle()
        {
            this.Init(12, true);
            var lines = LinesToArray(this.GetPuzzleLines(), "-");

            foreach (var connection in lines)
            {
                if (!CaveSystem.ContainsKey(connection[0]))
                    CaveSystem[connection[0]] = new Cave
                    {
                        Letter = connection[0],
                        IsBig = connection[0].ToUpper() == connection[0]
                    };

                if (!CaveSystem.ContainsKey(connection[1]))
                    CaveSystem[connection[1]] = new Cave
                    {
                        Letter = connection[1],
                        IsBig = connection[1].ToUpper() == connection[1]
                    };
                
                if (!CaveSystem[connection[0]].Connections.Contains(connection[1]))
                    CaveSystem[connection[0]].Connections.Add(connection[1]);
                
                if (!CaveSystem[connection[1]].Connections.Contains(connection[0]))
                    CaveSystem[connection[1]].Connections.Add(connection[0]);
            }
            
            //Part1();
            Part2();
        }

        private void Part1()
        {
            var paths = new List<string>();

            // Loop through all start connections
            foreach (var cave in CaveSystem["start"].Connections)
            {
                FindPaths(cave, "start," + cave + ",");
            }
            
            Console.WriteLine($"Total paths found: {FoundPaths.Count}");
        }
        
        private void Part2()
        {
            FoundPaths.Clear();
            var paths = new List<string>();

            // Loop through all start connections
            foreach (var cave in CaveSystem["start"].Connections)
            {
                FindPathsPart2(cave, "start," + cave + ",", false);
            }
            
            Console.WriteLine(FoundPaths.Count());
        }

        private void FindPaths(string start, string currentPath)
        {
            foreach (var cave in CaveSystem[start].Connections)
            {
                if (cave == currentPath.Substring(currentPath.Length - (cave.Length + 1), cave.Length))
                    continue;

                // Can only visit small cave once
                if (cave.ToLower() == cave && currentPath.Contains(cave + ","))
                    continue;
                
                var newPath = currentPath + cave + ",";

                if (cave == "end" && !FoundPaths.Contains(newPath))
                {
                    // Check if it exists
                    FoundPaths.Add(newPath);
                    continue;
                }

                FindPaths(cave, newPath);
            }
        }
        
        private void FindPathsPart2(string start, string currentPath, bool smallCaveVisited = false)
        {
            foreach (var cave in CaveSystem[start].Connections)
            {
                if (cave == currentPath.Substring(currentPath.Length - (cave.Length + 1), cave.Length))
                    continue;

                // Can only visit small cave once
                if (cave.ToLower() == cave && currentPath.Contains(cave + ","))
                {
                    if (!smallCaveVisited)
                        smallCaveVisited = HasSmallCaveBeenVisitedTwice(currentPath);
                    
                    if (cave == "start" || cave == "end" || smallCaveVisited)
                        continue;
                }
                
                var newPath = currentPath + cave + ",";

                if (cave == "end" && !FoundPaths.Contains(newPath))
                {
                    // Check if it exists
                    FoundPaths.Add(newPath);
                    continue;
                }

                FindPathsPart2(cave, newPath, smallCaveVisited);
            }
        }

        private bool HasSmallCaveBeenVisitedTwice(string currentPath)
        {
            var splitted = currentPath
                .Replace("start,", "")
                .Replace("end,", "")
                .Trim()
                .Split(",")
                .GroupBy(x => x)
                .Where(x => x.Key == x.Key.ToLower())
                .ToList();

            return splitted.Any(x => x.Count() > 1);
        }

        private int Occurences(string currentPath, string cave)
        {
            var list = currentPath.Split(",", StringSplitOptions.TrimEntries);

            return list.Count(x => x == cave);
        }
    }

    public class Cave
    {
        public bool IsBig { get; set; }
        public List<string> Connections { get; set; } = new List<string>();

        public bool Visited { get; set; }
        
        public string Letter { get; set; }
    }
}