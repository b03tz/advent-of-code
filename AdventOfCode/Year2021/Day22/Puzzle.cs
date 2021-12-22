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
            this.Init(22, true);
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

            //Part1();
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
                
                Console.WriteLine(instruction.ToString());
                
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
            
            Console.WriteLine(pixels.LongCount());
        }
        
        private void Part2()
        {
            var groupedInstructions = new HashSet<HashSet<Instruction>>();
            var maxSize = 1000;

            Console.WriteLine("Splitting");
            Instructions = SplitInstructions(Instructions, maxSize);
            Console.WriteLine("Done splitting");
            while (Instructions.Any())
            {
                var first = Instructions.First();
                var instructionSet = new HashSet<Instruction>()
                {
                    first
                };
                var theRest = new HashSet<Instruction>(Instructions.Skip(1));
                
                // Find overlapping cubes
                foreach (var cube in theRest)
                {
                    if (cube.OverlapsWith(first))
                        instructionSet.Add(cube);
                }
                
                groupedInstructions.Add(instructionSet);
                theRest.RemoveWhere(x => instructionSet.Contains(x));
                Instructions = theRest;
                Console.WriteLine($"Adding {instructionSet.Count} instructions, {Instructions.Count} remaining (total groups: {groupedInstructions.Count})");

                // Try 100 instruction sets
                if (groupedInstructions.Count > 1)
                {
                    break;
                }
            }

            long totalOnCount = 0;
            foreach(var grouped in groupedInstructions)
            {
                var subUniverse = new bool[1000, 1000, 1000];
                
               // HashSet<(int x, int y, int z)> pixels = new HashSet<(int x, int y, int z)>();
               
                foreach (var instruction in grouped)
                {
                    var minX = Math.Abs(instruction.RangeX.start);
                    var minY = Math.Abs(instruction.RangeY.start);
                    var minZ = Math.Abs(instruction.RangeZ.start);
                    
                    Console.WriteLine(instruction.ToString());
                
                    for (var x = 0; x < 1000; x++)
                    for (var y = 0; y < 1000; y++)
                    for (var z = 0; z < 1000; z++)
                    {
                        var startX = Math.Abs(instruction.RangeX.start) - minX + x;
                        var startY = Math.Abs(instruction.RangeY.start) - minY + y;
                        var startZ = Math.Abs(instruction.RangeZ.start) - minZ + z;
                        subUniverse[startX, startY, startZ] = instruction.Action == Action.On;
                    }
                }

                for(var x = 0; x < subUniverse.GetLength(0); x++)
                {
                    for(var y = 0; y < subUniverse.GetLength(1); y++)
                    {
                        for(var z = 0; z < subUniverse.GetLength(2); z++)
                        {
                            if (subUniverse[x, y, z] == true)
                                totalOnCount++;
                        }
                    }
                }
            }
            Console.WriteLine(totalOnCount);
        }

        private HashSet<Instruction> SplitInstructions(HashSet<Instruction> instructions, int maxSize = 50)
        {
            var newInstructions = new HashSet<Instruction>();

            // Split instructions in X axis
            Console.WriteLine($"Split X {instructions.Count}");
            foreach (var instruction in instructions)
            {
                var xSize = instruction.RangeX.end - instruction.RangeX.start;

                if (xSize > maxSize)
                {
                    var i = instruction.RangeX.start;
                    for (; i < instruction.RangeX.end; i += maxSize)
                    {
                        var end = i + maxSize;
                        if (end > instruction.RangeX.end)
                            end = instruction.RangeX.end;
                        
                        var newInstruction = new Instruction()
                        {
                            RangeX = (i, end),
                            RangeY = instruction.RangeY,
                            RangeZ = instruction.RangeZ
                        };
                        
                        newInstructions.Add(newInstruction);
                    }

                    if (i < instruction.RangeX.end)
                    {
                        newInstructions.Add(new Instruction()
                        {
                            RangeX = (i, instruction.RangeX.end),
                            RangeY = instruction.RangeY,
                            RangeZ = instruction.RangeZ
                        });
                    }
                }
            }
            
            Console.WriteLine($"Split Y {newInstructions.Count}");
            // Split instructions in Y axis
            var splitYInstructions = new HashSet<Instruction>();
            foreach (var instruction in newInstructions)
            {
                var ySize = instruction.RangeY.end - instruction.RangeY.start;

                if (ySize > maxSize)
                {
                    var i = instruction.RangeY.start;
                    for (; i < instruction.RangeY.end; i += maxSize)
                    {
                        var end = i + maxSize;
                        if (end > instruction.RangeY.end)
                            end = instruction.RangeY.end;
                        
                        var newInstruction = new Instruction()
                        {
                            RangeX = instruction.RangeX,
                            RangeY = (i, end),
                            RangeZ = instruction.RangeZ
                        };
                        
                        splitYInstructions.Add(newInstruction);
                    }

                    if (i < instruction.RangeY.end)
                    {
                        splitYInstructions.Add(new Instruction()
                        {
                            RangeX = instruction.RangeX,
                            RangeY = (i, instruction.RangeY.end),
                            RangeZ = instruction.RangeZ
                        });
                    }
                }
            }

            newInstructions = splitYInstructions;
            
            Console.WriteLine($"Split Z {newInstructions.Count}");
            var splitZInstructions = new HashSet<Instruction>();
            foreach (var instruction in newInstructions)
            {
                var zSize = instruction.RangeZ.end - instruction.RangeZ.start;

                if (zSize > maxSize)
                {
                    var i = instruction.RangeZ.start;
                    for (; i < instruction.RangeZ.end; i += maxSize)
                    {
                        var end = i + maxSize;
                        if (end > instruction.RangeZ.end)
                            end = instruction.RangeZ.end;
                        
                        var newInstruction = new Instruction()
                        {
                            RangeX = instruction.RangeX,
                            RangeY = instruction.RangeY,
                            RangeZ = (i, end)
                        };
                        
                        splitZInstructions.Add(newInstruction);
                    }

                    if (i < instruction.RangeZ.end)
                    {
                        splitZInstructions.Add(new Instruction()
                        {
                            RangeX = instruction.RangeX,
                            RangeY = instruction.RangeY,
                            RangeZ = (i, instruction.RangeZ.end)
                        });
                    }
                }
            }

            return splitZInstructions;
        }
    }

    public struct Pixel
    {
        public Pixel(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public override string ToString()
        {
            return $"{X},{Y},{Z}";
        }
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
        public static bool OverlapsWith(this Instruction a, Instruction b)
        {
            if (
                // if b = inside a
                (b.RangeX.start > a.RangeX.start && b.RangeX.start < a.RangeX.end) ||
                (b.RangeX.end > a.RangeX.start && b.RangeX.end < a.RangeX.end) ||
                (b.RangeY.start > a.RangeY.start && b.RangeY.start < a.RangeY.end) ||
                (b.RangeY.end > a.RangeY.start && b.RangeY.end < a.RangeY.end) ||
                (b.RangeZ.start > a.RangeZ.start && b.RangeZ.start < a.RangeZ.end) ||
                (b.RangeZ.end > a.RangeZ.start && b.RangeZ.end < a.RangeY.end) ||
                
                // if a  inside b
                (a.RangeX.start > b.RangeX.start && a.RangeX.start < b.RangeX.end) ||
                (a.RangeX.end > b.RangeX.start && a.RangeX.end < b.RangeX.end) ||
                (a.RangeY.start > b.RangeY.start && a.RangeY.start < b.RangeY.end) ||
                (a.RangeY.end > b.RangeY.start && a.RangeY.end < b.RangeY.end) ||
                (a.RangeZ.start > b.RangeZ.start && a.RangeZ.start < b.RangeZ.end) ||
                (a.RangeZ.end > b.RangeZ.start && a.RangeZ.end < b.RangeY.end)
            )
            {
                return true;
            }
            
            return false;
        }
    }
}