using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Year2020.Day6
{
    public class Puzzle : PuzzleBase
    {
        private List<List<Person>> Groups = new List<List<Person>>();
        public Puzzle()
        {
            this.Init(6, false);
            
            var lines = GetPuzzleLines();

            var currentGroup = new List<Person>();
            foreach (var line in lines)
            {
                if (line.Trim() == "")
                {
                    Groups.Add(currentGroup);
                    currentGroup = new List<Person>();
                    continue;
                }

                currentGroup.Add(new Person()
                {
                    QuestionsAnswered = line.Select(x => x.ToString()).ToList()
                });
            }
            
            Groups.Add(currentGroup);

            Part1();
            Part2();
        }

        private void Part1()
        {
            var total = 0;
            foreach (var group in Groups)
            {
                var result = group
                    .SelectMany(x => x.QuestionsAnswered)
                    .GroupBy(x => x);

                total += result.Count();
            }
            
            Console.WriteLine($"Part 1 - Sum of questions: {total}");
        }

        private void Part2()
        {
            var total = 0;
            foreach (var group in Groups)
            {
                var result = group
                    .SelectMany(x => x.QuestionsAnswered)
                    .GroupBy(x => x)
                    .ToList();

                total += result.Count(x => x.Count() == group.Count);
            }
            
            Console.WriteLine($"Part 2 - Sum of questions to which all answered yes: {total}");
        }
    }

    public class Person
    {
        public List<string> QuestionsAnswered { get; set; } = new List<string>();
    }
}