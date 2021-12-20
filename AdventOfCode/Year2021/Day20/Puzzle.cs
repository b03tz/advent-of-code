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
            this.Init(20, true);
            var lines = this.GetPuzzleLines();

            Algorhithm = lines.First();

            var stringArray = LinesToArray(lines.Skip(2), "");
            Image = new string[stringArray.Count(), stringArray.First().Length];
            
            for(var row = 0; row < stringArray.Count; row++)
            for (var col = 0; col < stringArray[row].Length; col++)
                Image[row, col] = stringArray[row][col];

            Part1(Image);
        }

        private void Part1(string[,] image)
        {
            //PrintImage(image);
            image = Enhance(image, 2);
            PrintImage(image);
            
            Console.WriteLine($"Total count: {image.CountOccurences("#")}");
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

        private string[,] Enhance(string[,] image, int numberOfTimes)
        {
            for (var iteration = 0; iteration < numberOfTimes; iteration++)
            {
                var paddingSize = 2;
                image = PadArray(image, ".", paddingSize);
                var outputImage = new string[image.GetLength(0), image.GetLength(1)];
            
                // Fill output image with darks
                for (var row = 0; row < outputImage.GetLength(0); row++)
                    for (var col = 0; col < outputImage.GetLength(1); col++)
                        outputImage[row, col] = ".";
                    
                for (var row = 1; row < outputImage.GetLength(0) - 1; row++)
                {
                    for (var col = 1; col < outputImage.GetLength(1) - 1; col++)
                    {
                        outputImage[row, col] = GetPixel(image, row, col);
                    }
                }

                image = outputImage;
                image = image.CutArrayX(2, image.GetLength(1) - 4);
                image = image.CutArrayY(2, image.GetLength(0) - 4);
                
                //Console.WriteLine($"After iteration: {iteration}");
               // PrintImage(image);
            }

            

            return image;
        }

        private string GetFromArray(string[,] input, int row, int col, string defaultValue)
        {
            if (row < 0 || row > input.GetLength(0) - 1)
                return defaultValue;

            if (col < 0 || col > input.GetLength(1) - 1)
                return defaultValue;

            return input[row, col];
        }

        private string GetPixel(string[,] outputImage, int row, int col)
        {
            var pixelString =
                outputImage.GetFromArray(row - 1, col - 1, ".") +
                outputImage.GetFromArray(row - 1, col, ".") +
                outputImage.GetFromArray(row - 1, col + 1, ".") +
                outputImage.GetFromArray(row, col - 1, ".") +
                outputImage.GetFromArray(row, col, ".") +
                outputImage.GetFromArray(row, col + 1, ".") +
                outputImage.GetFromArray(row + 1, col - 1, ".") +
                outputImage.GetFromArray(row + 1, col, ".") +
                outputImage.GetFromArray(row + 1, col + 1, ".");

            pixelString = pixelString
                .Replace(".", "0")
                .Replace("#", "1");
            
            var binaryResult = Convert.ToInt32(pixelString, 2);

            /*if (binaryResult == 0)
                return ".";*/

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

        public string[,] PadArray(string[,] inputArray, string character, int paddingSize)
        {
            paddingSize = paddingSize * 2;
            var newArray = new string[inputArray.GetLength(0) + paddingSize, inputArray.GetLength(1) + paddingSize];

            for (var row = 0; row < inputArray.GetLength(0) + paddingSize; row++)
                for (var col = 0; col < inputArray.GetLength(1) + paddingSize; col++)
                {
                    if (row < (paddingSize / 2) || 
                        (row - (paddingSize / 2)) > inputArray.GetLength(0) - 1 || 
                        col < (paddingSize / 2) || 
                        (col - (paddingSize / 2)) > inputArray.GetLength(1) - 1)
                    {
                        newArray[row, col] = character;
                        continue;
                    }

                    newArray[row, col] = inputArray[row - (paddingSize / 2), col - (paddingSize / 2)];
                }
            
            return newArray;
        }
    }
}