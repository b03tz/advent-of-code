using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;

namespace AdventOfCode.Year2021.Day24
{
    public class Puzzle : PuzzleBase
    {
        public Puzzle()
        {
            this.Init(24, false);
            var lines = this.GetPuzzleLines();

            var unit = new LogicUnit(lines);
            
            // This puzzle is not finished.
            // I wrote the machine but it was just a boring puzzle.
            // Yesterday was a puzzle as well; and I didn't feel like finishing this
            // because I was really up for some die hard programming and not figuring out
            // a formula.
            // Had a friend calculate these for me! Good day sir!
        }

        public void Solve(long from, long to, LogicUnit logicUnit)
        {
            //var modelNumber = 13579246899999;
            var modelNumber = from;
            var iteration = 0;
            var perIteration = 50000;
            while(modelNumber >= to)
            {
                if (iteration % perIteration == 0)
                {
                    Console.WriteLine($"Iterated {perIteration} ({modelNumber}) ({from} / {to})");
                }

                iteration++;
                var modelString = modelNumber.ToString();
                if (modelString.IndexOf("0") != -1)
                {
                    modelNumber--;
                    continue;
                }

                var modelList = modelString.ToList();
                for (var i = 0; i < modelList.Count; i++)
                {
                    logicUnit.Execute(modelList[i].ToString(), i);
                    //logicUnit.PrintMemory();
                }

                if (logicUnit.IsValid())
                {
                    Console.WriteLine(modelNumber);
                    logicUnit.PrintMemory();
                }

                if (logicUnit.GetZ() < 5000)
                {                    
                    Console.WriteLine(modelNumber);
                    logicUnit.PrintMemory();
                }
                
                logicUnit.Reset();
                modelNumber--;
            }
        }
    }

    public class LogicUnit
    {
        private readonly bool debug = false;
        private readonly List<Instruction>[] instructions = new List<Instruction>[14];
        private readonly Dictionary<string, long> Memory = new Dictionary<string, long>
        {
            {"w", 0},
            {"x", 0},
            {"y", 0},
            {"z", 0}
        };
        
        public LogicUnit(string[] inputInstructions)
        {
            ParseInstructions(inputInstructions);
        }

        public long GetZ()
        {
            return Memory["z"];
        }

        public bool IsValid()
        {
            return Memory["z"] == 0;
        }

        public void Reset()
        {
            Memory["w"] = 0;
            Memory["x"] = 0;
            Memory["y"] = 0;
            Memory["z"] = 0;
        }

        public void PrintMemory()
        {
            foreach (var mem in Memory)
                Console.WriteLine(mem.Key + ": " + mem.Value);
        }

        public void PrintInstructions(int index)
        {
            foreach (var op in instructions[index])
            {
                Console.Write(op.Type.ToString().ToLower() + " ");
                Console.Write(op.LeftHand + " ");
                
                if (op.Type != InstructionType.Inp)
                    if (op.RightHandIsVariable)
                        Console.Write(op.RightHand);
                    else
                        Console.Write(op.RightHandIntValue);
                
                Console.Write("\n");
            }
        }

        public void Execute(string inputStringNumber, int instructionIndex)
        {
            var input = Convert.ToInt32(inputStringNumber);

            foreach (var instruction in instructions[instructionIndex])
            {
                long operationResult = 0;
                long bValue = instruction.RightHandIntValue;
                if (instruction.RightHandIsVariable)
                    bValue = Memory[instruction.RightHand];

                switch (instruction.Type)
                {
                    case InstructionType.Inp:
                        operationResult = input;
                        break;
                    case InstructionType.Add:
                        operationResult = Memory[instruction.LeftHand] + bValue;
                        break;
                    case InstructionType.Mul:
                        operationResult = Memory[instruction.LeftHand] * bValue;
                        break;
                    case InstructionType.Div:
                        // Do not divide by zero
                        if (bValue == 0)
                            continue;
                        
                        operationResult = Convert.ToInt64(Math.Floor((double)Memory[instruction.LeftHand] / bValue));
                        break;
                    case InstructionType.Eql:
                        operationResult = Memory[instruction.LeftHand] == bValue ? 1 : 0;
                        break;
                    case InstructionType.Mod:
                        // Do not execute invalid params
                        if (Memory[instruction.LeftHand] < 0 || bValue <= 0)
                            continue;

                        operationResult = Memory[instruction.LeftHand] % bValue;
                        break;
                }

                Memory[instruction.LeftHand] = operationResult;
                
                if (debug)
                    Deb($"*** Setting {instruction.LeftHand} to {operationResult}");
            }

            /*Memory["w"] = 0;
            Memory["x"] = 0;
            Memory["y"] = 0;*/
        }

        public void Deb(string input)
        {
            if (this.debug)
                Console.WriteLine(input);
        }
        
        private void ParseInstructions(string[] instructions)
        {
            var currentOperation = 0;
            List<Instruction> currentList = new List<Instruction>();
            foreach (var operation in instructions)
            {
                var op = operation.Split(" ");

                string? rightHand = op.Length > 2 ? op[2] : null;
                bool rightHandVariable = false;
                int rightHandIntValue = 0;
                if (rightHand != null)
                {
                    rightHandVariable = true;
                    
                    if (!Memory.ContainsKey(rightHand))
                    {
                        rightHandVariable = false;
                        rightHandIntValue = Convert.ToInt32(rightHand);
                    }
                }

                var newOperation = new Instruction()
                {
                    Type = Instruction.MapType(op[0]),
                    LeftHand = op[1],
                    RightHand = op.Length > 2 ? op[2] : null,
                    RightHandIsVariable = rightHandVariable,
                    RightHandIntValue = rightHandIntValue
                };

                if (newOperation.Type == InstructionType.Inp)
                {
                    if (!currentList.Any())
                    {
                        // This is the first
                        currentList.Add(newOperation);
                        continue;
                    }

                    this.instructions[currentOperation] = new List<Instruction>(currentList);
                    currentList.Clear();
                    currentOperation++;
                }
                
                currentList.Add(newOperation);
            }

            this.instructions[currentOperation] = currentList;
        }

        public long? Calculate(long modelNumber)
        {
            var modelString = modelNumber.ToString();
            if (modelString.IndexOf("0", StringComparison.Ordinal) != -1)
            {
                return null;
            }

            var modelList = modelString.ToList();
            for (var i = 0; i < modelList.Count; i++)
            {
                this.Execute(modelList[i].ToString(), i);
                this.Memory["x"] = 0;
                this.Memory["w"] = 0;
                this.Memory["y"] = 0;
            }
            
            var result = this.Memory["z"];
            this.Reset();
            return result;
        }
    }

    public class Instruction
    {
        public InstructionType Type { get; set; }
        public string LeftHand { get; set; } = "";
        public string RightHand { get; set; } = "";
        
        public bool RightHandIsVariable { get; set; }
        
        public long RightHandIntValue { get; set; }

        public static InstructionType MapType(string input)
        {
            switch (input.ToLower())
            {
                case "inp":
                    return InstructionType.Inp;
                case "add":
                    return InstructionType.Add;
                case "mul":
                    return InstructionType.Mul;
                case "div":
                    return InstructionType.Div;
                case "mod":
                    return InstructionType.Mod;
                case "eql":
                    return InstructionType.Eql;
                default:
                    throw new Exception("Invalid input!");
            }
        }
    }

    public enum InstructionType
    {
        Inp,
        Add,
        Mul,
        Div,
        Mod,
        Eql
    }
}