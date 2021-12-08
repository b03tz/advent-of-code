using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

namespace AdventOfCode2021.Day8
{
    public class Puzzle
    {
        private readonly Dictionary<int, int> DigitCount = new Dictionary<int, int>
        {
            {0, 6},
            {1, 2},
            {2, 5},
            {3, 5},
            {4, 4},
            {5, 5},
            {6, 6},
            {7, 3},
            {8, 7},
            {9, 6},
        };
        
        private List<string> SignalPatterns = new List<string>();
        private List<string> OutputValues = new List<string>();
        
        public Puzzle()
        {
            var content = File.ReadAllText("Day8\\testinput.txt");

            var lines = content.Split("\n");

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Split(" | ");
                SignalPatterns.Add(line[0].Trim());
                OutputValues.Add(line[1].Trim());
            }

            Part1();
            Part2();
        }
        
            //                        7                 4
        //acedgfb cdfbe gcdfa fbcad dab cefabd cdfgeb eafb cagedb ab | cdfeb fcadb cdfeb cdbaf
        private void Part1()
        {
            var instances = 0;              //1  4  7  8
            var uniqueInstances = new int[] { 2, 4, 3, 7 };
            
            foreach (var outputValue in OutputValues)
            {
                var characters = outputValue.Split(' ');
                var lengths = characters.Select(x => x.Length);

                foreach (var length in lengths)
                {
                    if (uniqueInstances.Contains(length))
                        instances++;
                }
            }
            
            Console.WriteLine($"Unique instances: {instances}");
        }

        private void Part2()
        {
            var segment = "0123456";
            var input = "1011011";
            
            PrintNumber(segment, input);
        }

        private void PrintNumber(string segment, string input)
        {
            var output = new bool[7, 7];

            for (var i = 0; i < segment.Length; i++)
            {
                if (Convert.ToInt32(input.Substring(i, 1)) == 0)
                    continue;
                
                switch (Convert.ToInt32(segment.Substring(i, 1)))
                {
                    case 0:
                        output[0, 1] = true;
                        output[0, 2] = true;
                        output[0, 3] = true;
                        output[0, 4] = true;
                        output[0, 5] = true;
                        break;
                    case 1:
                        output[1, 0] = true;
                        output[2, 0] = true;
                        break;
                    case 2:
                        output[1, 6] = true;
                        output[2, 6] = true;
                        break;
                    case 3:
                        output[3, 1] = true;
                        output[3, 2] = true;
                        output[3, 3] = true;
                        output[3, 4] = true;
                        output[3, 5] = true;
                        break;
                    case 4:
                        output[4, 0] = true;
                        output[5, 0] = true;
                        break;
                    case 5:
                        output[4, 6] = true;
                        output[5, 6] = true;
                        break;
                    case 6:
                        output[6, 1] = true;
                        output[6, 2] = true;
                        output[6, 3] = true;
                        output[6, 4] = true;
                        output[6, 5] = true;
                        break;
                }
            }

            var outputString = "";
            for (int i = 0; i < output.GetLength(0); i++)
            {
                for (int j = 0; j < output.GetLength(1); j++)
                {
                    outputString += output[i, j] ? "X" : ".";
                }

                outputString += "\n";
            }

            Console.WriteLine(outputString);
        }
    }
}