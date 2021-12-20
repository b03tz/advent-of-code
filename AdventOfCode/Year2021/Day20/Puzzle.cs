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
                Console.Write(iteration + 1 + "...");

                image = image.PadArray(unknownUniverse, 1);
                var outputImage = new string[image.GetLength(0), image.GetLength(1)];
            
                // Fill output image with unknownUniverse
                for (var row = 0; row < outputImage.GetLength(0); row++)
                    for (var col = 0; col < outputImage.GetLength(1); col++)
                        outputImage[row, col] = unknownUniverse;
                    
                for (var row = 0; row < outputImage.GetLength(0); row++)
                    for (var col = 0; col < outputImage.GetLength(1); col++)
                        outputImage[row, col] = ConvertPixel(image, row, col, unknownUniverse);

                image = outputImage;

                unknownUniverse = unknownUniverse == "." ? Algorhithm.Substring(0, 1) : ".";
            }
            
            Console.Write("\n");

            return image;
        }

        private string ConvertPixel(string[,] outputImage, int row, int col, string unknownUniverse)
        {
            var pixelString =
                (outputImage.GetFromArray(row - 1, col - 1, unknownUniverse) == "#" ? "1" : "0") +
                (outputImage.GetFromArray(row - 1, col, unknownUniverse) == "#" ? "1" : "0") +
                (outputImage.GetFromArray(row - 1, col + 1, unknownUniverse) == "#" ? "1" : "0") +
                (outputImage.GetFromArray(row, col - 1, unknownUniverse) == "#" ? "1" : "0") +
                (outputImage.GetFromArray(row, col, unknownUniverse) == "#" ? "1" : "0") +
                (outputImage.GetFromArray(row, col + 1, unknownUniverse) == "#" ? "1" : "0") +
                (outputImage.GetFromArray(row + 1, col - 1, unknownUniverse) == "#" ? "1" : "0") +
                (outputImage.GetFromArray(row + 1, col, unknownUniverse) == "#" ? "1" : "0") +
                (outputImage.GetFromArray(row + 1, col + 1, unknownUniverse) == "#" ? "1" : "0");
            
            return Algorhithm.Substring(Convert.ToInt32(pixelString, 2), 1);
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