using System;

namespace AdventOfCode2021
{
    class Program
    {
        private const int CurrentDay = 2;
        
        static void Main(string[] args)
        {
            for (var i = 1; i <= 2; i++)
            {
                OutputDay(i);
            }
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
            
            Activator.CreateInstance(type);
            Console.WriteLine("\n");
        }
    }
}