using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Helpers;

namespace AdventOfCode.Year2020.Day5
{
    public class Puzzle : PuzzleBase
    {
        public Seat[,] AllSeats;
        public Seat?[,] ValidSeats;

        public Puzzle()
        {
            this.Init(5, false);

            var lines = GetPuzzleLines();

            AllSeats = new Seat[128, 8];
            ValidSeats = new Seat[128, 8];
            for (var row = 0; row < 128; row++)
            {
                for (var column = 0; column < 8; column++)
                    AllSeats[row, column] = new Seat
                    {
                        Row = row,
                        Column = column
                    };
            }

            Part1(lines);
            Part2();
        }

        private void Part1(string[] lines)
        {
            List<Seat> validSeats = new List<Seat>();

            foreach (var space in lines)
            {
                Seat[,] seats = (Seat[,])AllSeats.Clone();

                var spaceInput = space;
                while (spaceInput.Length > 0)
                {
                    var action = spaceInput.Substring(0, 1);
                    spaceInput = spaceInput.Substring(1);

                    seats = action switch
                    {
                        "F" => seats.CutArrayY(0, seats.GetLength(0) / 2),
                        "B" => seats.CutArrayY(seats.GetLength(0) / 2, seats.GetLength(0) / 2),
                        "R" => seats.CutArrayX(seats.GetLength(1) / 2, seats.GetLength(1) / 2),
                        "L" => seats.CutArrayX(0, seats.GetLength(1) / 2),
                        _ => seats
                    };
                }

                // Finally we are left with 1 seat
                var validSeat = seats[0, 0];
                validSeats.Add(validSeat);

                // Add the valid seat
                ValidSeats[validSeat.Row, validSeat.Column] = new Seat
                {
                    Row = validSeat.Row,
                    Column = validSeat.Column
                };
            }

            var highestSeat = validSeats.OrderByDescending(x => x.Column + (x.Row * 8)).First();

            Console.WriteLine($"Part 1 - The highest seat ID = {highestSeat.Column + (highestSeat.Row * 8)}");
        }

        private void Part2()
        {
            var ids = new List<int>();
            for (var row = 0; row < ValidSeats.GetLength(0); row++)
            for (var column = 0; column < AllSeats.GetLength(1); column++)
            {
                var validSeat = ValidSeats[row, column];
                if (validSeat == null)
                    continue;

                validSeat.Id = column + (row * 8);
                ids.Add(validSeat.Id);
            }

            ids.Sort();

            int? lastSeat = null;
            foreach (var id in ids)
            {
                if (lastSeat == null)
                {
                    lastSeat = id;
                    continue;
                }

                if (id - lastSeat > 1)
                {
                    Console.WriteLine($"Part 2 - Missing seat: {id - 1}");
                    break;
                }

                lastSeat = id;
            }
        }
    }

    public class Seat
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int Id { get; set; }
    }
}