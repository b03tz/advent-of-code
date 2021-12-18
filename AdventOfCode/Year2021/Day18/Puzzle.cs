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
        private readonly List<Pair> Pairs = new List<Pair>();
        
        public class Pair
        {
            public object LeftValue { get; set; }
            
            public object RightValue { get; set; }

            public bool IsRegularPair => LeftValue is PairValue && RightValue is PairValue;
            
            public Pair? Parent { get; set; }
        }

        public class PairValue
        {
            public PairValue(int value)
            {
                this.Value = value;
            }
            
            public int Value { get; set; }
        }

        public Puzzle()
        {
            this.Init(18, false);
            var lines = this.GetPuzzleLines();

            Part1(lines);
            Part2(lines);
        }

        public void Part2(string[] lines)
        {
            var largestMagnitude = 0;
            foreach (var line1 in lines)
            {
                foreach (var line2 in lines)
                {
                    if (line1 == line2)
                        continue;

                    var pair = ParsePairs(JArray.Parse(line1));
                    var otherPair = ParsePairs(JArray.Parse(line2));
                    var addition = new Pair();
                    pair.Parent = addition;
                    otherPair.Parent = addition;
                    addition.LeftValue = pair;
                    addition.RightValue = otherPair;
                    
                    addition = Reduce(addition);

                    var magnitude = CalculateMagnitude(addition);

                    if (magnitude > largestMagnitude)
                        largestMagnitude = magnitude;
                }
            }
            
            Console.WriteLine($"Part 2 - Largest magnitude: {largestMagnitude}");
        }

        public void Part1(string[] lines)
        {
            foreach (var line in lines)
            {
                Pairs.Add(ParsePairs(JArray.Parse(line)));
            }
            
            Pair leftNumber = Pairs.First();

            for (var i = 1; i < Pairs.Count; i++)
            {
                Pair second = Pairs.Skip(i).First();

                var addition = new Pair();
                addition.LeftValue = leftNumber;
                addition.RightValue = second;
                leftNumber.Parent = addition;
                second.Parent = addition;
                leftNumber = Reduce(addition);
            }
            
            leftNumber = Reduce(leftNumber);

            Console.WriteLine(GetPuzzlePairString(leftNumber));
            Console.WriteLine($"Part 1 - Magnitude: {CalculateMagnitude(leftNumber)}");
        }
        
        private int CalculateMagnitude(Pair input)
        {
            var total = 0;
            if (input.LeftValue is PairValue leftValue)
            {
                total += 3 * leftValue.Value;
            }
            else if (input.LeftValue is Pair leftPair)
            {
                total += 3 * CalculateMagnitude(leftPair);
            }

            if (input.RightValue is PairValue rightValue)
            {
                total += 2 * rightValue.Value;
            } else if (input.RightValue is Pair rightPair)
            {
                total += 2 * CalculateMagnitude(rightPair);
            }

            return total;
        }

        private Pair Reduce(Pair input)
        {
            while (true)
            {
                var explodeResult = Explode(input);
                if (explodeResult != null)
                {
                    input = explodeResult;
                    continue;
                }

                var splitResult = Split(input);
                if (splitResult != null)
                {
                    input = splitResult;
                    continue;
                }

                if (splitResult == null && explodeResult == null)
                    break;
            }

            return input;
        }

        private Pair? Split(Pair input)
        {
            var pairToSplit = GetPairToSplit(input);

            if (pairToSplit == null)
                return null;

            if (pairToSplit.LeftValue is PairValue leftPair && leftPair.Value >= 10)
            {
                var valueToSplit = (float) leftPair.Value / 2;
                var leftValue = Convert.ToInt32(Math.Floor(valueToSplit));
                var rightValue = Convert.ToInt32(Math.Ceiling(valueToSplit));

                pairToSplit.LeftValue = new Pair()
                {
                    LeftValue = new PairValue(leftValue),
                    RightValue = new PairValue(rightValue),
                    Parent = pairToSplit
                };

                return input;
            }

            if (pairToSplit.RightValue is PairValue rightPair && rightPair.Value >= 10)
            {
                var valueToSplit = (float) rightPair.Value / 2;
                var leftValue = Convert.ToInt32(Math.Floor(valueToSplit));
                var rightValue = Convert.ToInt32(Math.Ceiling(valueToSplit));

                pairToSplit.RightValue = new Pair()
                {
                    LeftValue = new PairValue(leftValue),
                    RightValue = new PairValue(rightValue),
                    Parent = pairToSplit
                };

                return input;
            }

            return input;
        }

        private string GetPuzzlePairString(Pair output)
        {
            string returnValue = "";

            returnValue += "[";
            if (output.LeftValue is Pair leftPair)
            {
                returnValue += GetPuzzlePairString(leftPair);
            }
            else
            {
                returnValue += ((PairValue)output.LeftValue).Value.ToString();
            }

            returnValue += ",";

            if (output.RightValue is Pair rightPair)
            {
                returnValue += GetPuzzlePairString(rightPair);
            }
            else
            {
                returnValue += ((PairValue)output.RightValue).Value.ToString();
            }

            returnValue += "]";

            return returnValue;
        }

        private Pair? Explode(Pair input)
        {
            var nodeToExplode = GetPairToExplode(input);

            if (nodeToExplode == null)
                return null;

            var leftValue = (PairValue)nodeToExplode.LeftValue;
            var rightValue = (PairValue)nodeToExplode.RightValue;

            var toReplaceLeft = FindClosestLeft(nodeToExplode);
            var toReplaceRight = FindClosestRight(nodeToExplode);

            if (toReplaceLeft != null)
                toReplaceLeft.Value += leftValue.Value;

            if (toReplaceRight != null)
                toReplaceRight.Value += rightValue.Value;
            
            var parent = nodeToExplode.Parent;
            if (parent == null)
                return input;
            
            if (parent.LeftValue == nodeToExplode)
            {
                parent.LeftValue = new PairValue(0);
            }

            if (parent.RightValue == nodeToExplode)
            {
                parent.RightValue = new PairValue(0);
            }
            
            return input;
        }

        private PairValue? FindClosestLeft(Pair child)
        {
            var parent = child.Parent;

            while (parent is not null)
            {
                if (parent.LeftValue != child)
                {
                    var rootToGoRight = parent.LeftValue;
                    while (rootToGoRight is not PairValue)
                    {
                        rootToGoRight = ((Pair)rootToGoRight).RightValue;
                    }

                    return (PairValue)rootToGoRight;
                }

                child = parent;
                parent = parent.Parent;
            }

            return null;
        }
        
        private PairValue? FindClosestRight(Pair child)
        {
            var parent = child.Parent;

            while (parent is not null)
            {
                if (parent.RightValue != child)
                {
                    var rootToGoLeft = parent.RightValue;
                    while (rootToGoLeft is not PairValue)
                    {
                        rootToGoLeft = ((Pair)rootToGoLeft).LeftValue;
                    }

                    return (PairValue)rootToGoLeft;
                }

                child = parent;
                parent = parent.Parent;
            }

            return null;
        }
        
        private Pair? GetPairToSplit(Pair startNode)
        {
            if (startNode.LeftValue is PairValue leftValue && leftValue.Value >= 10)
                return startNode;
            
            if (startNode.LeftValue is Pair leftPair)
            {
                var leftToSplit = GetPairToSplit(leftPair);
                if (leftToSplit != null)
                    return leftToSplit; 
            }

            if (startNode.RightValue is PairValue rightValue && rightValue.Value >= 10)
                return startNode;
            
            if (startNode.RightValue is Pair rightPair)
            {
                var rightToSplit = GetPairToSplit(rightPair);
                if (rightToSplit != null)
                    return rightToSplit;
            }

            return null;
        }

        private Pair? GetPairToExplode(Pair startNode, int? currentDepth = null)
        {
            if (currentDepth == null)
                currentDepth = 1;
            
            if (startNode.IsRegularPair && currentDepth > 4)
                return startNode;

            if (!startNode.IsRegularPair)
            {
                if (startNode.LeftValue is Pair leftPair)
                {
                    var leftExplode = GetPairToExplode(leftPair, currentDepth + 1);
                    if (leftExplode != null)
                        return leftExplode;
                }
                
                if (startNode.RightValue is Pair rightPair)
                {
                    var rightExplode = GetPairToExplode(rightPair, currentDepth + 1);
                    if (rightExplode != null)
                        return rightExplode;
                }
            }

            return null;
        }

        public Pair ParsePairs(JArray input, Pair? parent = null)
        {
            var newPair = new Pair();

            if (input[0] is JArray child1Array)
                newPair.LeftValue = ParsePairs(child1Array, newPair);
            else
                newPair.LeftValue = new PairValue(input[0].Value<int>());
            
            if (input[1] is JArray child2Array)
                newPair.RightValue = ParsePairs(child2Array, newPair);
            else
                newPair.RightValue = new PairValue(input[1].Value<int>());

            newPair.Parent = parent;
            
            return newPair;
        }
    }
}