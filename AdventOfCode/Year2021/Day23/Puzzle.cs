using System;
using System.Data;
using System.Linq;

namespace AdventOfCode.Year2021.Day23
{
    public class Puzzle : PuzzleBase
    {
        private int[] validHallwayPositions = new[]
        {
            0, 1, 3, 5, 7, 9, 10
        };

        private int[] columnEntrances = new[]
        {
            2, 4, 6, 8
        };
        
        public Puzzle()
        {
            this.Init(23, true);


            foreach (var startingPosition in new[] {0, 1, 2, 3})
            {
                var startingState = new[,]
                {
                    {'A', 'B', 'D', 'C'},
                    {'B', 'A', 'C', 'D'}
                };

                var emptyHallway = new[]
                {
                    ' ',
                    ' ',
                    ' ',
                    ' ',
                    ' ',
                    ' ',
                    ' ',
                    ' ',
                    ' ',
                    ' ',
                    ' '
                };
                
                var puzzle = (amphipodRooms: startingState, hallway: emptyHallway);
                var maxIterations = 2500;
                while (true)
                {
                    maxIterations--;

                    if (maxIterations <= 0)
                    {
                        Console.WriteLine($"{maxIterations} its");
                        puzzle.Print();
                        break;
                        
                    }
                    
                    // Try every valid hallway position
                    // Try to move something into this position
                    var tryRoom = startingPosition;
                    for (var room = 0; room < 3; room++)
                    {
                        foreach (var validHallwayPosition in validHallwayPositions)
                        {
                            puzzle.MoveFromRoomToHallway(tryRoom, validHallwayPosition);

                            if (puzzle.amphipodRooms.IsValidGameState())
                            {
                                puzzle.Print();
                            }
                        }
                        
                        tryRoom++;
                        if (tryRoom > 3)
                            tryRoom = 0;
                    }
                    
                    // Try to move stuff into rooms
                    tryRoom = startingPosition;
                    for (var room = 0; room < 3; room++)
                    {
                        foreach (var validHallwayPosition in validHallwayPositions)
                        {
                            puzzle.MoveFromHallwayToRoom(tryRoom, validHallwayPosition);

                            if (puzzle.amphipodRooms.IsValidGameState())
                            {
                                puzzle.Print();
                            }
                        }

                        tryRoom++;
                        if (tryRoom > 3)
                            tryRoom = 0;
                    }
                    
                    // Try to move rooms into rooms
                    tryRoom = startingPosition;
                    for (var sourceRoom = 0; sourceRoom < 3; sourceRoom++)
                    {
                        var otherTryRoom = startingPosition;
                            
                        for (var targetRoom = 0; targetRoom < 3; targetRoom++)
                        {
                            if (tryRoom == otherTryRoom)
                                continue;
                                
                            // Don't touch good rooms
                            if (puzzle.amphipodRooms.IsRoomComplete(sourceRoom))
                                continue;
                                
                            puzzle.MoveFromRoomToRoom(tryRoom, otherTryRoom);
                                
                            if (puzzle.amphipodRooms.IsValidGameState())
                            {
                                puzzle.Print();
                            }
                            //puzzle.Print();
                                
                            otherTryRoom++;
                            if (otherTryRoom > 3)
                                otherTryRoom = 0;
                        }
                            
                        tryRoom++;
                        if (tryRoom > 3)
                            tryRoom = 0;
                    }
                    
                    if (puzzle.amphipodRooms.IsValidGameState())
                    {
                        puzzle.Print();
                    }
                }
            }
            
        }




    }

    public static class PuzzleExtensions
    {
        private static int[] validHallwayPositions = new[]
        {
            0, 1, 3, 5, 7, 9, 10
        };

        private static int[] columnEntrances = new[]
        {
            2, 4, 6, 8
        };

        private static char[] validColumnEntries = new[]
        {
            'A', 'B', 'C', 'D'
        };
        
        public static (char amphipod, int moveCount)? MoveFromRoomToRoom(this (char[,] puzzle, char[] hallway) input, int fromRoom, int toRoom)
        {
            // We're not touching a complete room
            if (input.puzzle.IsRoomComplete(fromRoom) || input.puzzle.IsRoomComplete(toRoom))
                return null;
            
            // We're not moving out a correct char if all chars are correct that are in here 
            var expected = validColumnEntries[fromRoom];
            var allExpected = true;
            for (var row = input.puzzle.GetLength(0) - 1; row >= 0; row--)
            {
                if (input.puzzle[row, fromRoom] != expected && input.puzzle[row, fromRoom] != ' ')
                {
                    allExpected = false;
                }
            }

            if (allExpected)
                return null;
            
            // Check if hallway is clear
            if (!CanMoveInHallway(input.hallway, columnEntrances[fromRoom], columnEntrances[toRoom]))
                return null;
                //throw new Exception($"Cannot move from room {fromRoom} to {toRoom} because entrance is blocked!");
            
            // Get the amphipod closest to hallway
            char sourceAmphipod = ' ';
            int sourceAmphipodRow = 0;
            for (var row = input.puzzle.GetLength(0) - 1; row >= 0; row--)
            {
                if (input.puzzle[row, fromRoom] != ' ')
                {
                    sourceAmphipod = input.puzzle[row, fromRoom];
                    sourceAmphipodRow = row;
                }
            }

            if (!input.puzzle.IsEntranceValid(toRoom, sourceAmphipod))
                return null;
                //throw new Exception($"Cannot move {sourceAmphipod} into room {toRoom}");
            
            int targetRow = 0;
            for (var row = input.puzzle.GetLength(0) - 1; row >= 0; row--)
            {
                if (input.puzzle[row, toRoom] == ' ')
                {
                    targetRow = row;
                    break;
                }
            }

            int moveUpToHallwayCost = sourceAmphipodRow + 1;
            int moveToCorrectPositionCost = Math.Abs(columnEntrances[fromRoom] - columnEntrances[toRoom]);
            int moveDownToTargetCost = targetRow + 1;
            int totalMoveCost = moveUpToHallwayCost + moveToCorrectPositionCost + moveDownToTargetCost;

            input.puzzle[sourceAmphipodRow, fromRoom] = ' '; // replace the current with nothing
            input.puzzle[targetRow, toRoom] = sourceAmphipod; // replace the current with amphipod

            return (sourceAmphipod, totalMoveCost);
        }

        public static (char amphipod, int moveCount)? MoveFromRoomToHallway(this (char[,] puzzle, char[] hallway) input, int amphipodRoom, int hallwayPosition)
        {
            if (!validHallwayPositions.Contains(hallwayPosition))
                return null;
                //throw new Exception($"Tried to move amphipod to invalid hallway position: {hallwayPosition}");
            
            // Check if the move is possible
            if (!CanMoveInHallway(input.hallway, columnEntrances[amphipodRoom], hallwayPosition))
                return null;
                //throw new Exception($"Tried to move amphipod to blocked hallway position: {hallwayPosition}");

            // We're not touching a complete room
            if (input.puzzle.IsRoomComplete(amphipodRoom))
                return null;
            
            // We're not moving out a correct char if all chars are correct that are in here 
            var expected = validColumnEntries[amphipodRoom];
            var allExpected = true;
            for (var row = input.puzzle.GetLength(0) - 1; row >= 0; row--)
            {
                if (input.puzzle[row, amphipodRoom] != expected && input.puzzle[row, amphipodRoom] != ' ')
                {
                    allExpected = false;
                }
            }

            if (allExpected)
                return null;
            
            // Get the amphipod closest to hallway
            char amphiPod = ' ';
            int amphiPodRow = 0;
            for (var row = input.puzzle.GetLength(0) - 1; row >= 0; row--)
            {
                if (input.puzzle[row, amphipodRoom] != ' ')
                {
                    amphiPod = input.puzzle[row, amphipodRoom];
                    amphiPodRow = row;
                }
            }

            int moveUpToHallwayCost = amphiPodRow + 1;
            int moveToCorrectPositionCost = Math.Abs(hallwayPosition - columnEntrances[amphipodRoom]);
            int totalMoveCost = moveUpToHallwayCost + moveToCorrectPositionCost;

            input.puzzle[amphiPodRow, amphipodRoom] = ' '; // replace the current with nothing
            input.hallway[hallwayPosition] = amphiPod;

            return (amphiPod, totalMoveCost);
        }
        
        public static (char amphipod, int moveCount)? MoveFromHallwayToRoom(this (char[,] puzzle, char[] hallway) input, int amphipodRoom, int hallwayPosition)
        {
            if (!validHallwayPositions.Contains(hallwayPosition))
                return null;
                //throw new Exception($"Tried to move amphipod from invalid hallway position: {hallwayPosition}");
            
            // Check if hallway is clear
            if (!CanMoveInHallway(input.hallway, hallwayPosition, columnEntrances[amphipodRoom]))
                return null;
                //throw new Exception($"Cannot move from hallway position {hallwayPosition} to room {amphipodRoom} hallway is blocked!");
            
            if (!input.puzzle.IsEntranceValid(amphipodRoom, input.hallway[hallwayPosition]))
                return null;
                //throw new Exception($"Tried to move amphipod {hallway[hallwayPosition]} into invalid room {amphipodRoom} from hallway position: {hallwayPosition}");

            // Get number of filled slots
            int row;
            for (row = input.puzzle.GetLength(0) - 1; row >= 0; row--)
            {
                if (input.puzzle[row, amphipodRoom] == ' ')
                    break;
            }
            
            int moveDownIntoSlotCost = 1 + row;
            int moveToCorrectPositionCost = Math.Abs(hallwayPosition - columnEntrances[amphipodRoom]);
            int totalMoveCost = moveDownIntoSlotCost + moveToCorrectPositionCost;

            input.puzzle[row, amphipodRoom] = input.hallway[hallwayPosition]; // replace the current with moved amphipod
            input.hallway[hallwayPosition] = ' '; // clear hallway position

            return (input.puzzle[row, amphipodRoom], totalMoveCost);
        }
        
        public static bool CanMoveInHallway(char[] hallway, int from, int to)
        {
            // Trying to move it left
            if (to < from)
            {
                for (var col = from - 1; col >= to; col--)
                {
                    if (hallway[col] != ' ')
                        return false;
                }

                return true;
            }

            // Trying to move it right
            if (to > from)
            {
                for (var col = from + 1; col <= to; col++)
                {
                    if (hallway[col] != ' ')
                        return false;
                }

                return true;
            }

            return true;
        }

        public static bool IsRoomComplete(this char[,] puzzle, int amphipodRoom)
        {
            // Check the column bottom up if it's valid (if there's a different amphipod below, it cannot enter an invalid room)
            for (var row = puzzle.GetLength(0) - 1; row >= 0; row--)
            {
                if (puzzle[row, amphipodRoom] == ' ' || puzzle[row, amphipodRoom] != validColumnEntries[amphipodRoom])
                    return false;
            }

            return true;
        }
        
        public static bool IsEntranceValid(this char[,] puzzle, int amphipodRoom, char amphipod)
        {
            // If this amphipod can enter this entrance at all
            if (amphipod != validColumnEntries[amphipodRoom])
                return false;
            
            // If it's empty then he can enter
            var allEmpty = true;
            for (var row = 0; row < puzzle.GetLength(0); row++)
            {
                if (puzzle[row, amphipodRoom] != ' ')
                    allEmpty = false;
            }

            if (allEmpty)
                return true;
            
            // Check the column bottom up if it's valid (if there's a different amphipod below, it cannot enter an invalid room)
            for (var row = puzzle.GetLength(0) - 1; row >= 0; row--)
            {
                if (puzzle[row, amphipodRoom] != validColumnEntries[amphipodRoom] && puzzle[row, amphipodRoom] != ' ')
                    return false;
            }

            return true;
        }
        
        public static bool IsValidGameState(this char[,] puzzle)
        {
            var validCols = new[] { 'A', 'B', 'C', 'D' };
            
            for (var row = 0; row < puzzle.GetLength(0); row++)
            for (var col = 0; col < puzzle.GetLength(1); col++)
                if (puzzle[row, col] != validCols[col])
                    return false;

            return true;
        }
        
        public static void Print(this (char[,] puzzle, char[] hallway) input)
        {
            Console.WriteLine("▄▄▄▄▄▄▄▄▄▄▄▄▄");
            Console.Write("█");
            foreach(var x in input.hallway)
                Console.Write(x);
            Console.Write("█\n");
            
            for(var row = 0; row < input.puzzle.GetLength(0); row++)
                Console.WriteLine($"███{input.puzzle[row,0]}█{input.puzzle[row,1]}█{input.puzzle[row,2]}█{input.puzzle[row,3]}███");
            Console.WriteLine("▀▀▀▀▀▀▀▀▀▀▀▀▀");
            Console.WriteLine("");
        }
    }
}