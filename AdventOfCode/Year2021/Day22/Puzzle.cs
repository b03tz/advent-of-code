using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;
using AdventOfCode.Helpers;

namespace AdventOfCode.Year2021.Day22
{
    public class Puzzle : PuzzleBase
    {
        private HashSet<Instruction> Instructions = new HashSet<Instruction>();
        private List<Cuboid> Cuboids = new List<Cuboid>();

        public Puzzle()
        {
            this.Init(22, false);
            var lines = this.GetPuzzleLines();

            foreach (var line in lines)
            {
                var matches =
                    new Regex(
                            "^(on|off) x=([\\-0-9]{1,})..([\\-0-9]{1,}),y=([\\-0-9]{1,})..([\\-0-9]{1,}),z=([\\-0-9]{1,})..([\\-0-9]{1,})$")
                        .Match(line);

                Instructions.Add(new Instruction
                {
                    Action = matches.Groups[1].Value == "on" ? Action.On : Action.Off,
                    RangeX = (matches.Groups[2].Value.ToInt(), matches.Groups[3].Value.ToInt()),
                    RangeY = (matches.Groups[4].Value.ToInt(), matches.Groups[5].Value.ToInt()),
                    RangeZ = (matches.Groups[6].Value.ToInt(), matches.Groups[7].Value.ToInt()),
                });
            }

            Part1();
            Part2();
        }

        private void Part1()
        {
            HashSet<(int x, int y, int z)> pixels = new HashSet<(int x, int y, int z)>();

            foreach (var instruction in Instructions)
            {
                if (
                    instruction.RangeX.start < -50 || instruction.RangeX.end > 50 ||
                    instruction.RangeY.start < -50 || instruction.RangeY.end > 50 ||
                    instruction.RangeZ.start < -50 || instruction.RangeZ.end > 50
                )
                {
                    continue;
                }

                switch (instruction.Action)
                {
                    case Action.On:
                        for (var x = instruction.RangeX.start; x <= instruction.RangeX.end; x++)
                        for (var y = instruction.RangeY.start; y <= instruction.RangeY.end; y++)
                        for (var z = instruction.RangeZ.start; z <= instruction.RangeZ.end; z++)
                            pixels.Add((x, y, z));
                        break;
                    case Action.Off:
                        for (var x = instruction.RangeX.start; x <= instruction.RangeX.end; x++)
                        for (var y = instruction.RangeY.start; y <= instruction.RangeY.end; y++)
                        for (var z = instruction.RangeZ.start; z <= instruction.RangeZ.end; z++)
                            pixels.Remove((x, y, z));
                        break;
                }
            }

            Console.WriteLine($"Part 1 result - {pixels.LongCount()}");
        }

        private void Part2()
        {
            var input = new HashSet<Cuboid>();
            foreach (var instruction in Instructions)
            {
                input.Add(new Cuboid
                {
                    X = (instruction.RangeX.start, instruction.RangeX.end),
                    Y = (instruction.RangeY.start, instruction.RangeY.end),
                    Z = (instruction.RangeZ.start, instruction.RangeZ.end),
                    IsOn = instruction.Action == Action.On 
                });
            }

            var result = new HashSet<Cuboid>();
            foreach (var cube in input)
            {
                var newCubes = new HashSet<Cuboid>();
                
                // Store cubes that are on always
                if (cube.IsOn)
                    newCubes.Add(cube);

                // Compare to other cubes in current result set
                foreach (var existingCube in result)
                {
                    // Create the intersection if it applies...
                    // The intersection is in opposite state of the intersected cube...
                    // This creates unnecessary cubes...but it's better than bruteforcing xD
                    var newCuboid = cube.CreateNewIfIntersect(existingCube, !existingCube.IsOn);
                    if (newCuboid == null)
                        continue;
                    
                    newCubes.Add(newCuboid);
                }

                // Add the new cubes back into the result
                foreach (var c in newCubes)
                    result.Add(c);
            }
            Console.WriteLine($"Total cubes: {result.Count}");
            Console.WriteLine($"Part 2 result - {result.Aggregate(0L, (totalVolume, c) => totalVolume + CalculateVolume(c))}");
        }

        public long CalculateVolume(Cuboid cube)
        {
            if (!cube.IsOn)
                return -(cube.X.end - cube.X.start + 1L) * (cube.Y.end - cube.Y.start + 1L) * (cube.Z.end - cube.Z.start + 1L);   
            
            return (cube.X.end - cube.X.start + 1L) * (cube.Y.end - cube.Y.start + 1L) * (cube.Z.end - cube.Z.start + 1L);
        }
    }
    

    public class Cuboid
    {
        public (int start, int end) X { get; set; }
        public (int start, int end) Y { get; set; }
        public (int start, int end) Z { get; set; }
        
        public bool IsOn { get; set; }

        public Cuboid? CreateNewIfIntersect(Cuboid b, bool isOn)
        {
            if (!OverlapsWith(b))
                return null;

            return new Cuboid
            {
                X = (Math.Max(X.start, b.X.start), Math.Min(X.end, b.X.end)),
                Y = (Math.Max(Y.start, b.Y.start), Math.Min(Y.end, b.Y.end)),
                Z = (Math.Max(Z.start, b.Z.start), Math.Min(Z.end, b.Z.end)),
                IsOn = isOn
            };
        }
        
        public bool OverlapsWith(Cuboid b)
        {
            if (
                X.start > b.X.end || X.end < b.X.start ||
                Y.start > b.Y.end || Y.end < b.Y.start ||
                Z.start > b.Z.end || Z.end < b.Z.start)
            {
                return false;
            }
            
            return true;
        }
    }

    public class Instruction
    {
        public Action Action { get; set; }
        public (int start, int end) RangeX { get; set; }
        public (int start, int end) RangeY { get; set; }
        public (int start, int end) RangeZ { get; set; }

        public override string ToString()
        {
            return $"x={RangeX.start}..{RangeX.end} y={RangeY.start}..{RangeY.end} z={RangeZ.start}..{RangeZ.end}";
        }
    }

    public enum Action
    {
        On,
        Off
    }

    public static class Extensions
    {

    }
}