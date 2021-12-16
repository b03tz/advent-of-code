using System;
using System.Diagnostics;

namespace AdventOfCode
{
    class Program
    {
        public static int Year = 2020;
        public static int SkipDays = 4;
        public static int CurrentDay = 5;
        
        static void Main(string[] args)
        {
            var timer = new Stopwatch();
            timer.Start();

            if (args.Length > 0)
                Year = Convert.ToInt32(args[0]);
            
            if (args.Length > 1)
                SkipDays = Convert.ToInt32(args[1]);
            
            if (args.Length > 2)
                CurrentDay = Convert.ToInt32(args[2]);
            
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

            var type = Type.GetType($"AdventOfCode.Year{Year}.Day{dayNumber}.Puzzle");
            
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