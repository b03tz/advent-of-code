using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Year2021.Day11
{
    public class Puzzle : PuzzleBase
    {
        private Octopus[,] OctopusGrid;
        
        public Puzzle()
        {
            this.Init(11, false);

            ReadGrid();
            Part1();
            
            ReadGrid();
            Part2();
        }

        private void Part1()
        {
            var totalFlashes = 0;
            for (var i = 0; i < 100; i++)
            {
                totalFlashes += Step();
            }
            
            Console.WriteLine($"Total flashes: {totalFlashes}");
        }

        private void Part2()
        {
            var step = 1;
            while (true)
            {
                Step();
                if (HaveAllFlashed())
                    break;

                step += 1;
            }
            
            Console.WriteLine($"All flashed at step {step}");
        }

        private int Step()
        {
            var flashes = 0;
            // Increase by one
            for (var row = 0; row < OctopusGrid.GetLength(0); row++)
                for (var column = 0; column < OctopusGrid.GetLength(1); column++)
                    OctopusGrid[row, column].Value += 1;

            // Flash
            for (var row = 0; row < OctopusGrid.GetLength(0); row++)
                for (var column = 0; column < OctopusGrid.GetLength(1); column++)
                    if (OctopusGrid[row, column].Value == 10)
                        flashes += Flash(OctopusGrid[row, column]);
            
            // Reset flashes
            for (var row = 0; row < OctopusGrid.GetLength(0); row++)
                for (var column = 0; column < OctopusGrid.GetLength(1); column++)
                    OctopusGrid[row, column].Flashed = false;

            return flashes;
        }

        private bool HaveAllFlashed()
        {
            var allHaveFlashed = true;
            for (var row = 0; row < OctopusGrid.GetLength(0); row++)
                for (var column = 0; column < OctopusGrid.GetLength(1); column++)
                    if (OctopusGrid[row, column].Value > 0)
                        allHaveFlashed = false;

            return allHaveFlashed;
        }

        private int Flash(Octopus octopus)
        {
            var flashes = 1;
            
            octopus.Flashed = true;
            octopus.Value = 0;

            var adjecant = GetAdjecant(octopus.Row, octopus.Column).Where(x => !x.Flashed && x.Value < 10).ToList();
            adjecant.ForEach(x => x.Value += 1);
            adjecant.Where(x => x.Value > 9).ToList().ForEach(x => flashes += Flash(x));

            return flashes;
        }

        private List<Octopus> GetAdjecant(int row, int column)
        {
            var result = new List<Octopus>();

            var locations = new List<(int x, int y)>
            {
                (row - 1, column - 1),
                (row - 1, column),
                (row - 1, column + 1),
                (row, column - 1),
                (row, column + 1),
                (row + 1, column - 1),
                (row + 1, column),
                (row + 1, column + 1)
            };

            foreach (var location in locations)
            {
                try
                {
                    result.Add(OctopusGrid[location.x, location.y]);
                }
                catch
                {
                    
                }
            }
            
            return result;
        }
        
        private void ReadGrid()
        {
            var lines = this.GetPuzzleLines();
            for (var row = 0; row < lines.Length; row++)
            {
                var line = lines[row].Trim();

                var rowNumbers = lines[row].ToList();
                for (var column = 0; column < rowNumbers.Count; column++)
                {
                    int value = Convert.ToInt32(rowNumbers[column].ToString());
                    
                    OctopusGrid ??= new Octopus[lines.Length, line.Length];

                    OctopusGrid[row, column] = new Octopus
                    {
                        Row = row,
                        Column = column,
                        Value = value,
                        Flashed = false
                    };
                }
            }
        }
        
        private void PrintGrid()
        {
            for (var row = 0; row < OctopusGrid.GetLength(0); row++)
            {
                var rowOutput = "";
                for (var column = 0; column < OctopusGrid.GetLength(1); column++)
                    rowOutput += OctopusGrid[row, column].Value + ".";
                
                Console.WriteLine(rowOutput);
            }
            Console.WriteLine("--");
        }
    }

    public class Octopus
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int Value { get; set; }
        public bool Flashed { get; set; }
    }
}