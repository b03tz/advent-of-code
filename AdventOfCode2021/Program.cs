using System;
using System.Diagnostics;

namespace AdventOfCode2021
{
    class Program
    {
        private const int SkipDays = 13;
        private const int CurrentDay = 14;
        
        static void Main(string[] args)
        {
            var timer = new Stopwatch();
            timer.Start();
            
            for (var i = 1 + SkipDays; i <= CurrentDay; i++)
            {
                OutputDay(i);
            }

            Console.WriteLine($"****** Solving day {SkipDays + 1} to {CurrentDay} took {timer.ElapsedMilliseconds} ms");
        }

        static void OutputDay(int dayNumber)
        {
            var additionalSpace = " ";
            if (dayNumber > 9)
            {
                additionalSpace = "";
            }
            
            Console.WriteLine("+___________________________________________+");
            Console.WriteLine($"| Output for day {dayNumber}                         {additionalSpace}|");
            Console.WriteLine("+___________________________________________+");

            var type = Type.GetType($"AdventOfCode2021.Day{dayNumber}.Puzzle");
            
            if (type == null)
                throw new ApplicationException($"Day {dayNumber} does not exist!");

            var s = new Stopwatch();
            s.Start();
            Activator.CreateInstance(type);
            Console.WriteLine($"*** Solution found in {s.ElapsedMilliseconds} ms");
            Console.WriteLine("\n");
        }
    }
}