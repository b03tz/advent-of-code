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
                    Map[row, col] = new Tile
                    {
                        X = col,
                        Y = row,
                        Cost = lines[row][col]
                    };


            Part1();
            Part2();
        }

        private void Part1()
        {
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
        }

        private Tile MapNewTile(Tile input)
        {
            return new Tile
            {
                X = input.X,
                Y = input.Y,
                Cost = input.Cost
            };
        }

        private string TileLocation(Tile tile)
        {
            return $"{tile.Y}{tile.X}";
        }

        private Tile? AStar(Tile start, Tile finish)
        {
            var openList = new Dictionary<string, Tile> { {TileLocation(start), MapNewTile(start)} };
            var closedList = new Dictionary<string, Tile>();

            while (openList.Any())
            {
                var leastCost = openList.OrderBy(x => x.Value.CalculateCost).First();
                openList.Remove(leastCost.Key);

                if (TileLocation(leastCost.Value) == TileLocation(finish))
                {
                    return leastCost.Value;
                }

                var adjecantTiles = GetAdjecant(leastCost.Value);

                foreach (var successor in adjecantTiles)
                {
                    if (successor == finish)
                        return successor;
                    
                    if (closedList.ContainsKey(TileLocation(successor)))
                        continue;
                    
                    if (openList.ContainsKey(TileLocation(successor)))
                    {
                        var existing = openList[TileLocation(successor)];
                        if (existing.CalculateCost > successor.CalculateCost)
                        {
                            // Replace it
                            openList[TileLocation(successor)] = successor;
                        }
                        continue;
                    }
                    
                    openList[TileLocation(successor)] = successor;
                }

                closedList[TileLocation(leastCost.Value)] = leastCost.Value;
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
                var newTile = MapNewTile(Map[pos.y, pos.x]);
                newTile.Cost = tile.Cost + newTile.Cost;
                newTile.Parent = tile;
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
                    
                    newArray[row, column] = MapNewTile(input[actualRow, column]);
                    newArray[row, column].Cost = costValue;
                    newArray[row, column].X = column;
                    newArray[row, column].Y = row;
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
                    
                    newArray[row, column] = MapNewTile(input[row, actualColumn]);
                    newArray[row, column].Cost = costValue;
                    newArray[row, column].X = column;
                    newArray[row, column].Y = row;
                }
            }

            return newArray;
        }
    }
    


    public class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Cost { get; set; }
        public int CalculateCost => Cost;
        public Tile? Parent { get; set; }
    }
}