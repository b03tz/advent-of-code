using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdventOfCode.Year2021.Day18
{
    public class Puzzle : PuzzleBase
    {
        public class Pair
        {
            public int Number { get; set; }
            public int Depth { get; set; }
        }

        public Puzzle()
        {
            this.Init(18, false);
            var lines = this.GetPuzzleLines();

            TestExpected(CalculateMagnitude(ParsePairs(JArray.Parse("[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]"))).ToString(), "3488");
            TestExpected(CalculateMagnitude(ParsePairs(JArray.Parse("[[1,2],[[3,4],5]]"))).ToString(), "143");
            TestExpected(CalculateMagnitude(ParsePairs(JArray.Parse("[[[[0,7],4],[[7,8],[6,0]]],[8,1]]"))).ToString(), "1384");
            TestExpected(CalculateMagnitude(ParsePairs(JArray.Parse("[[[[1,1],[2,2]],[3,3]],[4,4]]"))).ToString(), "445");
            TestExpected(CalculateMagnitude(ParsePairs(JArray.Parse("[[[[3,0],[5,3]],[4,4]],[5,5]]"))).ToString(), "791");

            return;
            List<Pair> currentNumber = ParsePairs(JArray.Parse(lines.First()));

            for (var i = 1; i < lines.Length; i++)
            {
                List<Pair> second = ParsePairs(JArray.Parse(lines.Skip(i).First()));
                var input = Add(currentNumber, second);
                
                currentNumber = Reduce(input);
                
                PrintNumber(currentNumber);
            }
            
            PrintNumber(currentNumber);

            Console.WriteLine($"Magnitude: {CalculateMagnitude(currentNumber)}");
        }

        private int CalculateMagnitude(List<Pair> parsePairs)
        {
            var intList = parsePairs.Select(x => x.Number).ToList();
            while (intList.Count() > 1)
            {
                var loop = 0;

                var newList = new List<int>();
                // Calculate magnitude
                int i = 0;
                var offset = 0;
                for (; i < intList.Count / 2; i++)
                {
                    var first = intList[offset];
                    var second = intList[offset + 1];
                    
                    newList.Add((3 * first) + (2 * second));
                    loop++;
                    offset += 2;
                }

                var end = offset + 1;
                if (end == intList.Count)
                    newList.Add(intList.Last());

                intList = newList;
            }
            
            return intList.First();
        }

        private List<Pair> Reduce(List<Pair> input)
        {
            while (true)
            {
                var explodeResult = Explode(input);
                if (explodeResult != null)
                {
                    //Console.Write("EXPLODE: ");
                    //PrintNumber(explodeResult);
                    input = explodeResult;
                    //break;
                    continue;
                }

                var splitResult = Split(input);
                if (splitResult != null)
                {
                    //Console.Write("SPLIT: ");
                    //PrintNumber(splitResult);
                    input = splitResult;
                    continue;
                }

                if (explodeResult == null && splitResult == null)
                    break;
            }

            return input;
        }

        private void PrintNumber(List<Pair> input)
        {
            var color = Console.ForegroundColor;

            foreach (var number in input)
            {
                Console.Write(number.Number);
                Console.ForegroundColor = ConsoleColor.Green;
                //Console.Write("("+number.Depth+")");
                Console.Write(",");
                Console.ForegroundColor = color;
            }
            Console.Write("\n");
        }

        private List<Pair> Add(List<Pair> firstPairs, List<Pair> secondPairs, bool increaseDepth = true)
        {
            var pairs = new List<Pair>();
            pairs.AddRange(firstPairs);
            pairs.AddRange(secondPairs);
            pairs.ForEach(x => x.Depth += 1);

            return pairs;
        }

        private List<Pair>? Split(List<Pair> pairs)
        {
            var pairsToSplit = pairs.Where(x => x.Number >= 10).ToList();

            if (!pairsToSplit.Any())
                return null;

            var pair = pairsToSplit.First();
            var index = pairs.FindIndex(x => x == pair);

            var newPair1 = new Pair()
            {
                Depth = pair.Depth + 1,
                Number = Convert.ToInt32(Math.Floor((float) pair.Number / 2))
            };
            
            var newPair2 = new Pair()
            {
                Depth = pair.Depth + 1,
                Number = Convert.ToInt32(Math.Ceiling((float) pair.Number / 2))
            };

            var firstPartOfList = pairs.Take(index);
            var lastPartOfList = pairs.Skip(1).Skip(index);
            var newList = new List<Pair>();
            
            newList.AddRange(firstPartOfList);
            newList.Add(newPair1);
            newList.Add(newPair2);
            newList.AddRange(lastPartOfList);

            return newList;
        }

        private List<Pair>? Explode(List<Pair> pairs)
        {
            Pair? first = null;
            Pair? second = null;
            int firstIndex = 0;
            int secondIndex = 0;
            var pairFound = false;
            for (var i = 0; i < pairs.Count - 1; i++)
            {
                first = pairs[i];
                second = pairs[i + 1];
                firstIndex = i;
                secondIndex = i + 1;

                if (first.Depth == second.Depth && first.Depth > 4)
                {
                    pairFound = true;
                    break;
                }
            }

            if (!pairFound || first == null || second == null)
                return null;
            
            /*var pairToExplode = pairs.Where(x => x.Depth > 4).ToList();

            if (pairToExplode.Count() < 2)
                return null;
            
            var first = pairToExplode.First();
            var second = pairToExplode.Skip(1).First();
            var firstIndex = pairs.FindIndex(x => x == first);
            var secondIndex = pairs.FindIndex(x => x == second);*/
            
            // Get number left of first
            var left = firstIndex - 1;
            var right = secondIndex + 1;

            if (left >= 0)
            {
                pairs[left].Number += first.Number;
            }/*
            else
            {
                // Replace it with a 0
                pairs[firstIndex] = new Pair()
                {
                    Number = 0,
                    Depth = first.Depth - 1
                };
            }*/

            if (right < pairs.Count())
            {
                pairs[right].Number += second.Number;
            }/*
            else
            {
                pairs[secondIndex] = new Pair()
                {
                    Number = 0,
                    Depth = second.Depth - 1
                };
            }*/
            
            // Place a new 0 
            if (left > 0)
            {
                // Replace the left
                pairs[pairs.FindIndex(x => x == first)].Number = 0;
                pairs[pairs.FindIndex(x => x == first)].Depth = first.Depth - 1;
                
                // Remove the second
                pairs.Remove(second);
                return pairs;
            }
            
            // Replace the right
            pairs[pairs.FindIndex(x => x == second)].Number = 0;
            pairs[pairs.FindIndex(x => x == second)].Depth = first.Depth - 1;
                
            // Remove the first
            pairs.Remove(first);
            return pairs;
            
            /*
            // Remove the pairs

            var toTest = 0;
            if (left > 0)
                if (pairs[left].Depth == first.Depth - 1)
                {
                    pairs.Remove(first);
                    toTest = 1;
                }
                else
                {
                    first.Depth -= 1;
                    first.Number = 0;
                }

            if (right < pairs.Count() + toTest)
                if (pairs[right - toTest].Depth == second.Depth - 1)
                    pairs.Remove(second);
                else
                {
                    second.Depth -= 1;
                    second.Number = 0;
                }

            return pairs;*/
        }

        public bool IsNumberPair(JToken value)
        {
            return value.Count() == 2 && value.First() is JToken && value.Skip(1).First() is JToken;
        }

        public List<Pair> ParsePairs(JArray input, int level = 1)
        {
            var result = new List<Pair>();
            JArray toExplode = null;
            JArray toSplit = null;

            foreach (var value in input)
            {
                if (value is JArray subArray)
                {
                    result.AddRange(ParsePairs(subArray, level + 1));
                    continue;
                }

                if (value is JToken)
                {
                    result.Add(new Pair()
                    {
                        Depth = level,
                        Number = value.Value<int>()
                    });
                }
            }

            return result;
        }
    }
}