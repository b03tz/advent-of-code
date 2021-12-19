using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace AdventOfCode.Year2021.Day19
{
    public class Puzzle : PuzzleBase
    {
        private List<Scanner> scanners = new List<Scanner>();
        
        public Puzzle()
        {
            this.Init(19, false);
            var lines = this.GetPuzzleLines();

            var currentScanner = 0;
            foreach (var line in lines)
            {
                if (line.Trim() == "" || line.Length < 3)
                    continue;
                
                if (line.Substring(0, 3) == "---")
                {
                    // New scanner
                    var split = line.Split("--- scanner ");
                    split = split[1].Split(" ");
                    currentScanner = Convert.ToInt32(split[0]);
                    
                    scanners.Add(new Scanner()
                    {
                        ScannerNumber = currentScanner
                    });
                    continue;
                }

                var coordinates = line.Split(",");
                scanners[currentScanner].Axis1.Add(Convert.ToInt32(coordinates[0]));
                scanners[currentScanner].Axis2.Add(Convert.ToInt32(coordinates[1]));
                scanners[currentScanner].Axis3.Add(Convert.ToInt32(coordinates[2]));
            }
            
            Part1();
            Part2();
        }

        private void Part1()
        {
            var firstScanner = scanners[0];
            firstScanner.Calibrated = true;
            
            Console.WriteLine($"Calibrating {scanners.Count} scanners...");
            
            // Calibrate all scanners to 0 if possible
            foreach (var scanner in scanners)
            {
                if (scanner == firstScanner)
                    continue;
                
                CalibrateScanner(firstScanner, scanner);

                if (scanner.Calibrated)
                {
                    Console.Write(scanners.Count(x => !x.Calibrated)+"...");
                }
            }
            
            // Calibrate all uncalibrated scanners to calibrated scanners?
            while (scanners.Any(x => !x.Calibrated))
            {
                foreach (var scanner in scanners.Where(x => !x.Calibrated))
                {
                    if (scanner == firstScanner)
                        continue;

                    foreach (var calibratedScanner in scanners.Where(x => x.Calibrated))
                    {
                        CalibrateScanner(calibratedScanner, scanner);

                        if (scanner.Calibrated)
                        {
                            Console.Write(scanners.Count(x => !x.Calibrated)+"...");
                            break;
                        }
                    }
                }
            }

            var uniqueCoordinates = scanners.SelectMany(x => x.AbsoluteCoordinates)
                .OrderBy(x => x.Axis1)
                .GroupBy(x => (x.Axis1, x.Axis2, x.Axis3));
            Console.WriteLine($"\nPart 1 - Unique coordinates: {uniqueCoordinates.Count()}");
        }
        
        private void Part2()
        {
            var largestDistance = 0;
            foreach (var scanner1 in scanners)
            {
                foreach (var scanner2 in scanners)
                {
                    var manhattenDistance =
                        Math.Abs(scanner1.Axis1Offset - scanner2.Axis1Offset) +
                        Math.Abs(scanner1.Axis2Offset - scanner2.Axis2Offset) +
                        Math.Abs(scanner1.Axis3Offset - scanner2.Axis3Offset);

                    if (manhattenDistance > largestDistance)
                        largestDistance = manhattenDistance;
                }
            }
            
            Console.WriteLine($"Part 2 - Largest manhattan distance: {largestDistance}");
        }

        private void CalibrateScanner(Scanner firstScanner, Scanner scanner)
        {
            // Find first axis
            (int matches, int diff, bool flip)? x = null;
            (int matches, int diff, bool flip)? y = null;
            (int matches, int diff, bool flip)? z = null;
            var axis1 = 0;
            var axis2 = 0;
            var axis3 = 0;

            for (var i = 1; i <= 3; i++)
            {
                x = CompareAxis(firstScanner, scanner, 1, i);

                if (x != null)
                {
                    if (x.Value.matches > 11)
                    {
                        axis1 = i;
                        break;
                    }
                }
            }
            
            for (var i = 1; i <= 3; i++)
            {
                if (axis1 > 0 && i == axis1)
                    continue;
                
                y = CompareAxis(firstScanner, scanner, 2, i);

                if (y != null)
                {
                    if (y.Value.matches > 11)
                    {
                        axis2 = i;
                        break;
                    }
                }
            }
            
            for (var i = 1; i <= 3; i++)
            {
                if ((axis1 > 0 && i == axis1) || (axis2 > 0 && i == axis2))
                    continue;
                
                z = CompareAxis(firstScanner, scanner, 3, i);

                if (z != null)
                {
                    if (z.Value.matches > 11)
                    {
                        axis3 = i;
                        break;
                    }
                }
            }

            if (x != null && y != null && z != null)
            {
                scanner.Axis1Is = axis1;
                scanner.Axis2Is = axis2;
                scanner.Axis3Is = axis3;
                scanner.Axis1Offset = x.Value.diff;
                scanner.Axis2Offset = y.Value.diff;
                scanner.Axis3Offset = z.Value.diff;
                scanner.Axis1Flipped = x.Value.flip;
                scanner.Axis2Flipped = y.Value.flip;
                scanner.Axis3Flipped = z.Value.flip;
                scanner.Calibrated = true;
            }
        }

        public (int matches, int diff, bool flip)? CompareAxis(Scanner firstScanner, Scanner secondScanner, int inputAxis, int checkAxis)
        {
            int[]? inputList = null;
            int[]? checkList = null;
            
            switch (inputAxis)
            {
                case 1:
                    inputList = firstScanner.AbsoluteCoordinates.Select(x => x.Axis1).ToArray();
                    break;
                case 2:
                    inputList = firstScanner.AbsoluteCoordinates.Select(x => x.Axis2).ToArray();
                    break;
                case 3:
                    inputList = firstScanner.AbsoluteCoordinates.Select(x => x.Axis3).ToArray();
                    break;
            }
            
            switch (checkAxis)
            {
                case 1:
                    checkList = secondScanner.AbsoluteCoordinates.Select(x => x.Axis1).ToArray();
                    break;
                case 2:
                    checkList = secondScanner.AbsoluteCoordinates.Select(x => x.Axis2).ToArray();
                    break;
                case 3:
                    checkList = secondScanner.AbsoluteCoordinates.Select(x => x.Axis3).ToArray();
                    break;
            }

            if (inputList == null || checkList == null)
                throw new Exception("Le impossible");

            foreach (var input in inputList)
            {
                foreach (var check in checkList)
                {
                    var matches = 0;
                    var negativeDiff = input - check;

                    foreach (var inputTest in inputList)
                        foreach (var test in checkList)
                            if (inputTest - test == negativeDiff)
                                matches += 1;

                    if (matches > 11)
                        return (matches, negativeDiff, false);
                    
                    matches = 0;
                    var positiveDiff = input + check;

                    foreach (var inputTest in inputList)
                        foreach (var test in checkList)
                            if (inputTest + test == positiveDiff)
                                matches += 1;

                    if (matches > 11)
                        return (matches, positiveDiff, true);
                }
            }
            
            return null;
        }
    }

    public class Scanner
    {
        public int ScannerNumber { get; set; }

        public List<int> Axis1 { get; set; } = new List<int>();
        public List<int> Axis2 { get; set; } = new List<int>();
        public List<int> Axis3 { get; set; } = new List<int>();

        public bool Calibrated { get; set; } = false;
        
        public bool Axis1Flipped { get; set; }
        public bool Axis2Flipped { get; set; }
        public bool Axis3Flipped { get; set; }
        
        public int Axis1Is { get; set; } // If axis 3 is actually scanner 0's axis 1 this value will be 3
        public int Axis2Is { get; set; }
        public int Axis3Is { get; set; }
        
        public int Axis1Offset { get; set; }
        
        public int Axis2Offset { get; set; }
        
        public int Axis3Offset { get; set; }

        private List<Coordinate> _absolute = new List<Coordinate>();

        public List<Coordinate> AbsoluteCoordinates
        {
            get
            {
                if (_absolute.Any())
                    return _absolute;
                
                var result = new List<Coordinate>();

                for (var i = 0; i < ActualAxis1.Count; i++)
                {
                    var xValue = ActualAxis1[i];
                    var yValue = ActualAxis2[i];
                    var zValue = ActualAxis3[i];
                    
                    if (Axis1Flipped)
                        xValue = -xValue;
                
                    if (Axis2Flipped)
                        yValue = -yValue;

                    if (Axis3Flipped)
                        zValue = -zValue;
                    
                    result.Add(new Coordinate(
                        xValue + Axis1Offset, 
                        yValue + Axis2Offset,
                        zValue + Axis3Offset
                    ));
                }

                if (Calibrated)
                    _absolute = result;

                return result;
            }
        }

        public List<int> ActualAxis1 => Axis1Is switch
        {
            1 => Axis1,
            2 => Axis2,
            3 => Axis3,
            _ => Axis1
        };
        
        public List<int> ActualAxis2 => Axis2Is switch
        {
            1 => Axis1,
            2 => Axis2,
            3 => Axis3,
            _ => Axis2
        };
        
        public List<int> ActualAxis3 => Axis3Is switch
        {
            1 => Axis1,
            2 => Axis2,
            3 => Axis3,
            _ => Axis3
        };
        
        public Coordinate? Location { get; set; }
    }

    public class Coordinate
    {
        public Coordinate(int axis1, int axis2, int axis3)
        {
            Axis1 = axis1;
            Axis2 = axis2;
            Axis3 = axis3;
        }
        
        public int Axis1 { get; }
        public int Axis2 { get; }
        public int Axis3 { get; }
    }
}