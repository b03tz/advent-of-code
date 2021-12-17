using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Year2021.Day15
{
    public class Puzzle : PuzzleBase
    {
        public Tile[,] Map;
        
        public Puzzle()
        {
            this.Init(15, false);
            var lines = ArrayLinesToInts(LinesToArray(this.GetPuzzleLines(), ""));

            Map = new Tile[lines.Count, lines.First().Length];

            for (var row = 0; row < lines.Count; row++)
            for (var col = 0; col < lines[row].Length; col++)
                Map[row, col] = new Tile(col, row, lines[row][col]);

            Part1();
            Part2();
        }

        private void Part1()
        {
            var start = Map[0, 0];
            var finish = Map[Map.GetLength(0) - 1, Map.GetLength(1) - 1];
            
            var path = AStar(start, finish);
            var totalCost = 0;
            List<(int X, int Y)> locations = new List<(int x, int y)>();
            
            while (path != null)
            {
                totalCost += Map[path.Y, path.X].Cost;
                locations.Add((path.Y, path.X));
                path = path.Parent;
            }
            
            //PrintMap(locations);

            Console.WriteLine($"Total cost: {totalCost - Map[0,0].Cost} !");
        }
        
        private void Part2()
        { 
            // Duplicate the map
            var originalXLength = Map.GetLength(1);
            var originalYLength = Map.GetLength(0);
            
            for (var i = 0; i < 4; i++)
                Map = RepeatArrayRows(Map, originalXLength);
            
            for (var i = 0; i < 4; i++)
                Map = RepeatArrayColumns(Map, originalYLength);
             
            var start = Map[0, 0];
            var finish = Map[Map.GetLength(0) - 1, Map.GetLength(1) - 1];
            
            var path = AStar(start, finish);
            var totalCost = 0;
            while (path != null)
            {
                totalCost += Map[path.Y, path.X].Cost;
                path = path.Parent;
            }

            Console.WriteLine($"Total cost: {totalCost - Map[0,0].Cost} !");
            PrintTimers();
        }

        private Tile? AStar(Tile start, Tile finish)
        {
            var openList = new Dictionary<string, Tile> { {start.Location, new Tile(
                start.X,
                start.Y,
                start.Cost
            )} };
            var closedList = new Dictionary<string, Tile>();

            while (openList.Any())
            {
                var leastCost = openList.OrderBy(x => x.Value.Cost).First();
                openList.Remove(leastCost.Key);

                if (leastCost.Value.Location == finish.Location)
                    return leastCost.Value;

                var adjecantTiles = GetAdjecant(leastCost.Value);

                foreach (var successor in adjecantTiles)
                {
                    if (successor == finish)
                        return successor;
                    
                    if (closedList.ContainsKey(successor.Location))
                        continue;
                    
                    if (openList.ContainsKey(successor.Location))
                    {
                        var existing = openList[successor.Location];
                        if (existing.Cost > successor.Cost)
                            openList[successor.Location] = successor;
                        continue;
                    }
                    
                    openList[successor.Location] = successor;
                }

                closedList[leastCost.Value.Location] = leastCost.Value;
            }

            return null;
        }

        private List<Tile> GetAdjecant(Tile tile)
        {
            var positions = new List<(int x, int y)>
            {
                (tile.X - 1, tile.Y),
                (tile.X + 1, tile.Y),
                (tile.X, tile.Y - 1),
                (tile.X, tile.Y + 1)
            }.Where(position => position.x >= 0 && position.x < Map.GetLength(1) && position.y >= 0 && position.y < Map.GetLength(0));

            var result = new List<Tile>();

            foreach (var pos in positions)
            {
                var newTile = new Tile(
                    pos.x,
                    pos.y,
                    tile.Cost + Map[pos.y, pos.x].Cost,
                    tile
                );
                
                result.Add(newTile);
            }

            return result;
        }

        public void PrintMap(List<(int x, int y)> markedTiles)
        {
            var color = Console.ForegroundColor;
            var path = "";
            var totalCost = 0;
            for (var row = 0; row < Map.GetLength(0); row++)
            {
                for (var col = 0; col < Map.GetLength(1); col++)
                {
                    var tile = Map[row, col];

                    if (markedTiles.Contains((tile.X, tile.Y)))
                    {
                        totalCost += tile.Cost;
                        path += tile.Cost;
                        Console.ForegroundColor = ConsoleColor.Red;
                    }

                    Console.Write(Map[row, col].Cost + " ");

                    Console.ForegroundColor = color;
                }

                Console.Write("\n");
            }
            
            Console.WriteLine($"PATH: {path}");
            Console.WriteLine($"EXP.: 112136511511323211");
            Console.WriteLine($"Total cost: {totalCost - Map[0,0].Cost}");
            Console.WriteLine($"Exp.  cost: 40");
        }
        
        private Tile[,] RepeatArrayRows(Tile[,] input, int howManyRows = 11)
        {
            var newArray = new Tile[input.GetLength(0) + howManyRows, input.GetLength(1)];

            for (var row = 0; row < newArray.GetLength(0); row++)
            {
                var actualRow = row;
                var shouldAdd = false;
                if (row >= input.GetLength(0))
                {
                    shouldAdd = true;
                    actualRow = row - howManyRows;
                }
                
                for (var column = 0; column < newArray.GetLength(1); column++)
                {
                    var costValue = input[actualRow, column].Cost;
                    if (shouldAdd)
                    {
                        if (costValue == 9)
                            costValue = 0;

                        costValue++;
                    }

                    newArray[row, column] = new Tile(
                        column,
                        row,
                        costValue
                    );
                }
            }

            return newArray;
        }
        
        private Tile[,] RepeatArrayColumns(Tile[,] input, int howManyColumns = 11)
        {
            var newArray = new Tile[input.GetLength(0), input.GetLength(1) + howManyColumns];

            for (var row = 0; row < newArray.GetLength(0); row++)
            {
                for (var column = 0; column < newArray.GetLength(1); column++)
                {
                    var actualColumn = column;
                    var shouldAdd = false;
                    if (column >= input.GetLength(1))
                    {
                        shouldAdd = true;
                        actualColumn = column - howManyColumns;
                    }
                    
                    var costValue = input[row, actualColumn].Cost;
                    if (shouldAdd)
                    {
                        if (costValue == 9)
                            costValue = 0;

                        costValue++;
                    }

                    newArray[row, column] = new Tile(
                        column,
                        row,
                        costValue
                    );
                }
            }

            return newArray;
        }
    }
    


    public record Tile(int X, int Y, int Cost, Tile? Parent = null)
    {
        public string Location => $"{Y}{X}";
    }
}