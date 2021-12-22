using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Helpers;

namespace AdventOfCode.Year2021.Day22
{
    public class Puzzle : PuzzleBase
    {
        private List<Instruction> Instructions = new List<Instruction>();
        private List<Cuboid> Cuboids = new List<Cuboid>();
        
        public Puzzle()
        {
            this.Init(22, true);
            var lines = this.GetPuzzleLines();

            foreach (var line in lines)
            {
                var matches = new Regex("^(on|off) x=([\\-0-9]{1,})..([\\-0-9]{1,}),y=([\\-0-9]{1,})..([\\-0-9]{1,}),z=([\\-0-9]{1,})..([\\-0-9]{1,})$").Match(line);

                Instructions.Add(new Instruction
                {
                    Action = matches.Groups[1].Value == "on" ? Action.On : Action.Off,
                    RangeX = (matches.Groups[2].Value.ToInt(), matches.Groups[3].Value.ToInt()),
                    RangeY = (matches.Groups[4].Value.ToInt(), matches.Groups[5].Value.ToInt()),
                    RangeZ = (matches.Groups[6].Value.ToInt(), matches.Groups[7].Value.ToInt()),
                });
            }

            Part1();
        }

        private void Part1()
        {
            var minY = Instructions.Select(x => x.RangeY.start).Min();
            var maxY = Instructions.Select(x => x.RangeY.end).Max();
            var minX = Instructions.Select(x => x.RangeX.start).Min();
            var maxX = Instructions.Select(x => x.RangeX.end).Max();
            var minZ = Instructions.Select(x => x.RangeZ.start).Min();
            var maxZ = Instructions.Select(x => x.RangeZ.end).Max();
            
            var xSize = maxX - minX;
            var ySize = maxY - minY;
            var zSize = maxZ - minZ;

            var xArray = new Pixel[xSize];
            var yArray = new Pixel[ySize];
            var zArray = new Pixel[zSize];

            var index = 0;
            for (var i = minX; i < maxX; i++)
                xArray[index++] = new Pixel(i);
            
            index = 0;
            for (var i = minY; i < maxY; i++)
                yArray[index++] = new Pixel(i);
            
            index = 0;
            for (var i = minZ; i < maxZ; i++)
                zArray[index++] = new Pixel(i);

            foreach (var instruction in Instructions)
            {
                switch (instruction.Action)
                {
                    case Action.On:
                        xArray.Where(x => x.Index >= instruction.RangeX.start && x.Index <= instruction.RangeX.end).ToList().ForEach(x => x.State = true);
                        yArray.Where(x => x.Index >= instruction.RangeX.start && x.Index <= instruction.RangeX.end).ToList().ForEach(x => x.State = true);
                        zArray.Where(x => x.Index >= instruction.RangeX.start && x.Index <= instruction.RangeX.end).ToList().ForEach(x => x.State = true);
                        break;
                    case Action.Off:
                        xArray.Where(x => x.Index >= instruction.RangeX.start && x.Index <= instruction.RangeX.end).ToList().ForEach(x => x.State = false);
                        yArray.Where(x => x.Index >= instruction.RangeX.start && x.Index <= instruction.RangeX.end).ToList().ForEach(x => x.State = false);
                        zArray.Where(x => x.Index >= instruction.RangeX.start && x.Index <= instruction.RangeX.end).ToList().ForEach(x => x.State = false);
                        break;
                }
            }
        }

        private class Pixel
        {
            public Pixel(int index)
            {
                Index = index;
            }
            public int Index { get; set; }
            public bool State { get; set; }
        }

        private void HandleInstruction(List<Range> onList, int start, int end, Action action)
        {
            // Check if there's an item already containing the supplied item
            var contained = onList.FirstOrDefault(range =>
                range.Start <= start && range.End >= end
            );

            if (contained != null)
            {
                // They are already on
                if (action == Action.On)
                    return;

                // The need to be split
                if (action == Action.Off)
                {
                    var newRange1 = new Range(contained.Start, start);
                    var newRange2 = new Range(end, contained.End);

                    onList.Remove(contained);
                    onList.Add(newRange1);
                    onList.Add(newRange2);

                    return;
                }
            }
            
            // Check if the item already contains a list item
            var containedExisting = onList.Where(range =>
                range.Start >= start && range.End <= end
            ).ToList();

            if (containedExisting.Any())
            {
                // They are already on
                if (action == Action.On)
                {
                    // Remove all
                    onList.RemoveAll(x => containedExisting.Contains(x));
                    
                    // Add this range
                    onList.Add(new Range(start, end));

                    return;
                }

                
                if (action == Action.Off)
                {
                    // Remove all that fall in this range
                    onList.RemoveAll(x => containedExisting.Contains(x));
                    return;
                }
            }
            
            // Check if we can join
            var joinStart = onList.FirstOrDefault(range =>
                range.Start <= start && range.End >= start
            );
            var joinEnd = onList.FirstOrDefault(range =>
                range.Start <= end && range.End >= end
            );
            if (joinStart != null && joinEnd != null)
            {
                // Join the ranges together
                joinStart.End = end;
                onList.Remove(joinStart);

                return;
            }
            
            // Check if we can extend left
            var extendLeft = onList.FirstOrDefault(range =>
                range.Start >= start && range.Start <= end);

            if (extendLeft != null)
            {
                extendLeft.Start = start;
                return;
            }
            
            // Check if we can extend right
            var extendRight = onList.FirstOrDefault(range =>
                range.Start <= start && range.End >= start);

            if (extendRight != null)
            {
                extendRight.End = end;
                return;
            }
            
            // Just add it
            onList.Add(new Range(start, end));
        }
    }

    
    
    public class Range
    {
        public Range(int start, int end)
        {
            Start = start;
            End = end;
        }
        public int Start { get; set; }
        public int End { get; set; }
    }

    public class Cuboid
    {
        public (int start, int end) X { get; set; }
        public (int start, int end) Y { get; set; }
        public (int start, int end) Z { get; set; }

        public long Calculate()
        {
            return Math.Abs(X.end - X.start) * Math.Abs(Y.end - Y.start) * Math.Abs(Z.end - Z.start);
        }
    }

    public class Instruction
    {
        public Action Action { get; set; }
        public (int start, int end) RangeX { get; set; }
        public (int start, int end) RangeY { get; set; }
        public (int start, int end) RangeZ { get; set; }
    }
    
    public enum Action {
        On,
        Off
    }
}