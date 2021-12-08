using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace AdventOfCode2021.Day8
{
    public class Puzzle
    {
        private readonly List<string> Permutations = new List<string>();

        private readonly List<string> Numbers = new List<string>
        {
            "1110111",
            "0010010",
            "1011101",
            "1011011",
            "0111010",
            "1101011",
            "1101111",
            "1010010",
            "1111111",
            "1111011",
        };
        
        private List<string> SignalPatterns = new List<string>();
        private List<string> OutputValues = new List<string>();
        
        public Puzzle()
        {
            var content = File.ReadAllText("Day8\\input.txt");
            Permutations = File.ReadAllText("Day8\\permutations.txt").Split("\n").Select(x => x.Trim()).ToList();

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
            var standardDigits = Numbers.Select(x => CreateDigit("0123456", x).Cast<bool>()).ToList();

            List<string> outputNumbers = new List<string>();
            for (var i = 0; i < SignalPatterns.Count; i++)
            {
                var signal = SignalPatterns[i];
                var output = OutputValues[i];

                // First convert to dictionary
                var letters = signal.Split(' ');

                // Try to work out board logic based on known numbers
                var (permutation, numberSegment) = GetPermutation(letters);

                var outputLetters = output.Split(' ');
                var stringOutput = "";
                foreach (var letter in outputLetters)
                {
                    var letterBinary = ToBinary(permutation, letter);
                    var createdDigit = CreateDigit(numberSegment, letterBinary).Cast<bool>();
                    var digit = standardDigits.FindIndex(x => x.SequenceEqual(createdDigit));

                    stringOutput += digit.ToString();
                }
                
                outputNumbers.Add(stringOutput);
                Console.WriteLine(stringOutput);
            }
            
            Console.WriteLine($"Total added: {outputNumbers.Select(x => Convert.ToInt32(x)).Sum()}");
        }

        public (string, string) GetPermutation(string[] letters)
        {
            var standardDigits = Numbers.Select(x => CreateDigit("0123456", x).Cast<bool>()).ToList();

            // Try to work out board logic based on known numbers
            var lastPermutation = "";
            var lastNumberSegment = "";
            foreach (var possiblePermutation in Permutations)
            {
                var numberSegment = SegmentToNumbers(possiblePermutation);
                var foundPermutation = true;

                foreach (var letter in letters)
                {
                    var letterBinary = ToBinary(possiblePermutation, letter);
                    var createdDigit = CreateDigit(numberSegment, letterBinary).Cast<bool>();

                    if (!standardDigits.Any(x => x.SequenceEqual(createdDigit)))
                    {
                        foundPermutation = false;
                        break;
                    }
                }

                if (foundPermutation)
                {
                    lastPermutation = possiblePermutation;
                    lastNumberSegment = numberSegment;

                    break;
                }
            }

            return (lastPermutation, lastNumberSegment);
        }

        private string SegmentToNumbers(string segment)
        {
            return segment.Replace("a", "0").Replace("b", "1").Replace("c", "2").Replace("d", "3").Replace("e", "4").Replace("f", "5").Replace("g", "6");
        }

        private string ToBinary(string letterSegment, string letter)
        {
            var output = new StringBuilder("0000000");
            for (var i = 0; i < letterSegment.Length; i++)
            {
                if (letter.Contains(letterSegment[i]))
                {
                    output[letterSegment.IndexOf(letterSegment[i])] = '1';
                }
            }

            return output.ToString();
        }

        private bool[,] CreateDigit(string numericSegment, string input)
        {
            var output = new bool[5, 3];

            for (var i = 0; i < numericSegment.Length; i++)
            {
                if (Convert.ToInt32(input.Substring(i, 1)) == 0)
                    continue;
                
                /*
                 * This function generates an array that maps to segments
                 * Segments are numbered 0 to 6 from top to bottom -> left to right
                 *
                 * 000
                 * 1.2
                 * 333
                 * 4.5
                 * 666
                 * 
                 *  ███    ..█    ███    ███    █.█    ███    ███ 
                 *  █.█    ..█    ..█    ..█    █.█    █..    █.. 
                 *  █.█    ..█    ███    ███    ███    ███    ███ 
                 *  █.█    ..█    █..    ..█    ..█    ..█    █.█ 
                 *  ███    ..█    ███    ███    ..█    ███    ███ 
                 */
                
                switch (i)
                {
                    case 0:
                        output[0, 0] = true;
                        output[0, 1] = true;
                        output[0, 2] = true;
                        break;
                    case 1:
                        output[1, 0] = true;
                        break;
                    case 2:
                        output[1, 1] = true;
                        break;
                    case 3:
                        output[2, 0] = true;
                        output[2, 1] = true;
                        output[2, 2] = true;
                        break;
                    case 4:
                        output[3, 0] = true;
                        break;
                    case 5:
                        output[3, 2] = true;
                        break;
                    case 6:
                        output[4, 0] = true;
                        output[4, 1] = true;
                        output[4, 2] = true;
                        break;
                }
            }

            return output;
        }

        private void PrintNumber(string segment, string input)
        {
            var output = CreateDigit(segment, input);

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