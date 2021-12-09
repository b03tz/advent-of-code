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
            "1110111", "0010010", "1011101", "1011011", "0111010", // 0, 1, 2, 3, 4
            "1101011", "1101111", "1010010", "1111111", "1111011", // 5, 6, 7, 8, 9
        };
        
        private List<string> SignalPatterns = new List<string>();
        private List<string> OutputValues = new List<string>();
        private List<string> StandardDigits = new List<string>();
        
        public Puzzle()
        {
            var content = File.ReadAllText("Day8\\input.txt");
            Permutations = File.ReadAllText("Day8\\permutations.txt").Split("\n").Select(x => x.Trim()).ToList();
            StandardDigits = Numbers.Select(x => CreateDigitString("0123456", x)).ToList();

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
        
        private void Part1()
        {
            var instances = 0;              //1  4  7  8
            var uniqueInstances = new int[] { 2, 4, 3, 7 };
            
            foreach (var outputValue in OutputValues)
            {
                var characters = outputValue.Split(' ');
                var lengths = characters.Select(x => x.Length);

                foreach (var length in lengths)
                    if (uniqueInstances.Contains(length))
                        instances++;
            }
            
            Console.WriteLine($"Part 1 - unique instances: {instances}");
        }

        private void Part2()
        {
            Console.WriteLine("Part 2 - Calculating...");

            List<string> outputNumbers = new List<string>();
            
            for (var i = 0; i < SignalPatterns.Count; i++)
            {
                var letters = SignalPatterns[i].Split(' ');
                var outputLetters = OutputValues[i].Split(' ');
                
                // Find a known letter
                var known = outputLetters
                    .Where(x => new int[] { 2, 4, 3, 7 }
                    .Contains(x.Length))
                    .ToList();
                
                // Make a copy of all possible permutations
                var filteredPermutations = new List<string>(Permutations);    

                // Filter permutations based on known
                foreach (var knownNumber in known)
                {
                    switch (knownNumber.Length)
                    {
                        case 2:
                            // It's a one
                            filteredPermutations = filteredPermutations
                                .Where(x => knownNumber.Contains(x[2]))
                                .Where(x => knownNumber.Contains(x[5])).ToList();
                            break;
                        case 4:
                            // It's a 4
                            filteredPermutations = filteredPermutations
                                .Where(x => knownNumber.Contains(x[1]))
                                .Where(x => knownNumber.Contains(x[2]))
                                .Where(x => knownNumber.Contains(x[3]))
                                .Where(x => knownNumber.Contains(x[5])).ToList();
                            break;
                        case 3:
                            // It's a 7
                            filteredPermutations = filteredPermutations
                                .Where(x => knownNumber.Contains(x[0]))
                                .Where(x => knownNumber.Contains(x[2]))
                                .Where(x => knownNumber.Contains(x[5])).ToList();
                            break;
                    }
                }

                // Try to work out board logic based on known numbers
                var (permutation, numberSegment) = GetPermutation(letters, filteredPermutations);
                var stringOutput = "";
                
                foreach (var letter in outputLetters)
                {
                    var letterBinary = ToBinary(permutation, letter);
                    var createdDigit = CreateDigitString(numberSegment, letterBinary);
                    var digit = StandardDigits.FindIndex(x => x == createdDigit);

                    stringOutput += digit.ToString();
                }
                
                outputNumbers.Add(stringOutput);
            }
            
            Console.WriteLine($"Part 2 - Total added: {outputNumbers.Select(x => Convert.ToInt32(x)).Sum()}");
        }

        public (string, string) GetPermutation(string[] letters, List<string> permutations)
        {
            // Try to work out board logic based on known numbers
            var lastPermutation = "";
            var lastNumberSegment = "";
            foreach (var possiblePermutation in permutations)
            {
                var numberSegment = SegmentToNumbers(possiblePermutation);
                var foundPermutation = true;

                foreach (var letter in letters)
                {
                    var letterBinary = ToBinary(possiblePermutation, letter);
                    var createdDigit = CreateDigitString(numberSegment, letterBinary);

                    if (StandardDigits.All(x => x != createdDigit))
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
            return segment
                .Replace("a", "0")
                .Replace("b", "1")
                .Replace("c", "2")
                .Replace("d", "3")
                .Replace("e", "4")
                .Replace("f", "5")
                .Replace("g", "6");
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
        
        private string CreateDigitString(string numericSegment, string input)
        {
            var output = new StringBuilder("0000000");

            for (var i = 0; i < numericSegment.Length; i++)
            {
                if (Convert.ToInt32(input.Substring(i, 1)) == 0)
                    continue;

                output[i] = '1';
            }

            return output.ToString();
        }
    }
}