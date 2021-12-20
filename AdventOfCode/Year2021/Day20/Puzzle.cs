using System;
using System.Collections.Generic;
using System.Linq;

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
        }

        private void Part1(string[,] image)
        {
            image = Enhance(image, 2);
            
            PrintImage(image);

            /*PrintImage(image);
            image = Enhance(image);
            PrintImage(image);*/
            var count = 0;
            for (var row = 1; row < image.GetLength(0); row++)
                for (var col = 1; col < image.GetLength(1); col++)
                    if (image[row, col] == "#")
                        count++;
            
            
            Console.WriteLine($"Total count: {count}");
        }

        private string[,] Enhance(string[,] image, int numberOfTimes)
        {
            string[,] outputImage = new string[0,0];
            
            for (var iteration = 0; iteration < numberOfTimes; iteration++)
            {
                var paddingSize = 6;
                var offset = iteration * paddingSize;
                image = PadArray(image, ".", paddingSize);
                outputImage = new string[image.GetLength(0), image.GetLength(1)];
            
                // Fill output image with darks
                for (var row = 0; row < outputImage.GetLength(0); row++)
                    for (var col = 0; col < outputImage.GetLength(1); col++)
                        outputImage[row, col] = ".";
                    
                for (var row = (paddingSize + offset - 2); row < image.GetLength(0) - (paddingSize + offset) + 1; row++)
                {
                    for (var col = (paddingSize + offset - 2); col < image.GetLength(1) - (paddingSize + offset) + 1; col++)
                    {
                        outputImage[row, col] = GetPixel(image, row, col);
                    }
                }
            }

            return outputImage;
        }

        private string GetPixel(string[,] outputImage, int row, int col)
        {
            var pixelString =
                outputImage[row - 1, col - 1] +
                outputImage[row - 1, col] +
                outputImage[row - 1, col + 1] +
                outputImage[row, col - 1] +
                outputImage[row, col] +
                outputImage[row, col + 1] +
                outputImage[row + 1, col - 1] +
                outputImage[row + 1, col] +
                outputImage[row + 1, col + 1];

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