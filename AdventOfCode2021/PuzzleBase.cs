using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021
{
    public class PuzzleBase
    {
        public int Day = 0;
        public bool IsTesting = false;

        public void Init(int day, bool isTesting)
        {
            this.Day = day;
            this.IsTesting = isTesting;
        }
        
        public string GetPuzzleText()
        {
            return File.ReadAllText(GetPuzzleFilename());
        }

        public string[] GetPuzzleLines()
        {
            return File.ReadAllLines(GetPuzzleFilename());
        }

        public List<string[]> LinesToArray(IEnumerable<string> lines, string separator)
        {
            if (separator == "")
                return lines.Select(x => x.ToArray().Select(x => x.ToString()).ToArray()).ToList();
                
            return lines.Select(x => x.Split(separator)).ToList();
        }

        public int[] LinesToInts(IEnumerable<string> lines)
        {
            return lines.Select(x => Convert.ToInt32(x)).ToArray();
        }

        public List<int[]> ArrayLinesToInts(IEnumerable<string[]> lines)
        {
            return lines.Select(x => x.Select(y => Convert.ToInt32(y)).ToArray()).ToList();
        }
        
        public string GetPuzzleFilename()
        {
            var testPrefix = IsTesting ? "test" : "";
            
            return $"Day{Day}\\{testPrefix}input.txt";
        }
    }
}