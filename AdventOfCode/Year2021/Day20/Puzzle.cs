using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Helpers;

namespace AdventOfCode.Year2021.Day20
{
    public class Puzzle : PuzzleBase
    {
        private string Algorhithm;
        private string[,] Image;
        
        public Puzzle()
        {
            this.Init(20, false);
            var lines = this.GetPuzzleLines();

            Algorhithm = lines.First();

            var stringArray = LinesToArray(lines.Skip(2), "");
            Image = new string[stringArray.Count(), stringArray.First().Length];
            
            for(var row = 0; row < stringArray.Count; row++)
            for (var col = 0; col < stringArray[row].Length; col++)
                Image[row, col] = stringArray[row][col];

            Part1(Image);
            Part2(Image);
        }

        private void Part1(string[,] image)
        {
            image = Enhance(image, 2);
            Console.WriteLine($"Part 1 - Total count: {image.CountOccurences("#")}");
        }
        
        private void Part2(string[,] image)
        {
            image = Enhance(image, 50);
            Console.WriteLine($"Part 2 - Total count: {image.CountOccurences("#")}");
        }
        
        private string[,] Enhance(string[,] image, int numberOfTimes)
        {
            Console.WriteLine($"Enhancing {numberOfTimes} times...");
            var unknownUniverse = ".";
            for (var iteration = 0; iteration < numberOfTimes; iteration++)
            {
                Console.Write(iteration + 1+ "...");
                var paddingSize = 1;
                image = image.PadArray(unknownUniverse, paddingSize);
                var outputImage = new string[image.GetLength(0), image.GetLength(1)];
            
                // Fill output image with unknownUniverse
                for (var row = 0; row < outputImage.GetLength(0); row++)
                    for (var col = 0; col < outputImage.GetLength(1); col++)
                        outputImage[row, col] = unknownUniverse;
                    
                for (var row = 0; row < outputImage.GetLength(0); row++)
                {
                    for (var col = 0; col < outputImage.GetLength(1); col++)
                    {
                        outputImage[row, col] = GetPixel(image, row, col, unknownUniverse);
                    }
                }

                image = outputImage;

                if (unknownUniverse == ".")
                    unknownUniverse = Algorhithm.Substring(0, 1);
                else
                    unknownUniverse = ".";
            }
            
            Console.Write("\n");

            return image;
        }

        private (int xS, int xE, int yS, int yE) GetImageBounds(string[,] image)
        {
            var xS = Int32.MaxValue;
            var xE = 0;
            var yS = Int32.MaxValue;
            var yE = 0;
            
            for (var row = 0; row < image.GetLength(0); row++)
                for (var col = 0; col < image.GetLength(1); col++)
                {
                    if (image[row, col] == "#")
                    {
                        if (col < xS)
                            xS = col;

                        if (col > xE)
                            xE = col;

                        if (row < yS)
                            yS = row;

                        if (row > yS)
                            yE = row;
                    }
                }

            return (xS, xE, yS, yE);
        }

        private string GetPixel(string[,] outputImage, int row, int col, string unknownUniverse)
        {
            var pixelString =
                outputImage.GetFromArray(row - 1, col - 1, unknownUniverse) +
                outputImage.GetFromArray(row - 1, col, unknownUniverse) +
                outputImage.GetFromArray(row - 1, col + 1, unknownUniverse) +
                outputImage.GetFromArray(row, col - 1, unknownUniverse) +
                outputImage.GetFromArray(row, col, unknownUniverse) +
                outputImage.GetFromArray(row, col + 1, unknownUniverse) +
                outputImage.GetFromArray(row + 1, col - 1, unknownUniverse) +
                outputImage.GetFromArray(row + 1, col, unknownUniverse) +
                outputImage.GetFromArray(row + 1, col + 1, unknownUniverse);

            pixelString = pixelString
                .Replace(".", "0")
                .Replace("#", "1");
            
            var binaryResult = Convert.ToInt32(pixelString, 2);

            return Algorhithm.Substring(binaryResult, 1);
        }

        private void PrintImage(string[,] image)
        {
            for (var row = 0; row < image.GetLength(0); row++)
            {
                for (var col = 0; col < image.GetLength(1); col++)
                {
                    Console.Write(image[row,col]);
                }
                
                Console.Write("\n");
            }
            
            Console.Write("\n");
            Console.Write("\n");
        }
    }
}