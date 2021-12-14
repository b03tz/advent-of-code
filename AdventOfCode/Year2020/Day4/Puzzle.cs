using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Year2020.Day4
{
    public class Puzzle : PuzzleBase
    {
        private List<Passport> Passports = new List<Passport>();
        
        public Puzzle()
        {
            this.Init(4, false);

            var lines = GetPuzzleLines();

            MapPassports(lines);

            Part1();
            Part2();
        }

        private void Part1()
        {
            Console.WriteLine($"Part 1 - Number of valid passports: {Passports.Count(x => x.IsValid)}");
        }

        private void Part2()
        {
            Console.WriteLine($"Part 2 - Total passports: {Passports.Count}");
            Console.WriteLine($"Part 2 - Valid passports: {Passports.Count(x => x.IsValid && x.IsValidated)}");
        }
        
        private void MapPassports(string[] lines)
        {
            var passportData = "";
            foreach (var line in lines)
            {
                if (line.Trim() == "")
                {
                    Passports.Add(TextToPassword(passportData));
                    passportData = "";
                    continue;
                }

                passportData += " "+line;
            }
        }

        private Passport TextToPassword(string passportData)
        {
            var input = passportData
                .Trim()
                .Split(" ")
                .Select(x => x.Split(":"));
                
                
                var data = input.ToDictionary(x => x[0].Trim(), x => x[1].Trim());
            
            return new Passport
            {
                Ecl = data.ContainsKey("ecl") ? data["ecl"] : "",
                Pid = data.ContainsKey("pid") ? data["pid"] : "",
                Eyr = data.ContainsKey("eyr") ? data["eyr"] : "",
                Hcl = data.ContainsKey("hcl") ? data["hcl"] : "",
                Byr = data.ContainsKey("byr") ? data["byr"] : "",
                Iyr = data.ContainsKey("iyr") ? data["iyr"] : "",
                Cid = data.ContainsKey("cid") ? data["cid"] : "",
                Hgt = data.ContainsKey("hgt") ? data["hgt"] : "",
            };
        }
    }

    public class Passport
    {
        public string Ecl { get; set; } = string.Empty;
        public string Pid { get; set; } = string.Empty;
        public string Eyr { get; set; } = string.Empty;
        public string Hcl { get; set; } = string.Empty;
        public string Byr { get; set; } = string.Empty;
        public string Iyr { get; set; } = string.Empty;
        public string Cid { get; set; } = string.Empty;
        public string Hgt { get; set; } = string.Empty;

        public bool IsValid
        {
            get
            {
                if (Ecl != "" && Pid != "" && Eyr != "" && Hcl != "" && Byr != "" && Iyr != "" && Hgt != "")
                    return true;
                
                return false;
            }
        }

        public bool IsValidated
        {
            get
            {
                // Validate birth year
                if (!ValidateYear(Byr, 1920, 2002) || !ValidateYear(Iyr, 2010, 2020) || !ValidateYear(Eyr, 2020, 2030))
                    return false;

                if (!ValidateHeight(Hgt))
                    return false;

                if (!ValidateColor(Hcl))
                    return false;

                if (!ValidateEyeColor(Ecl))
                    return false;

                if (!ValidatePasspordId(Pid))
                    return false;

                return true;
            }
        }

        private bool ValidatePasspordId(string pid)
        {
            return new Regex("^[0-9]{9}$").Match(pid).Success;
        }

        private bool ValidateEyeColor(string ecl)
        {
            return new[] { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" }.Contains(ecl);
        }

        private bool ValidateColor(string hcl)
        {
            return new Regex("^\\#[0-9a-f]{6}$").Match(hcl).Success;
        }

        private bool ValidateHeight(string height)
        {
            var regex = new Regex("^([0-9]+)([a-z]+)$").Match(height);

            if (regex.Groups.Count != 3)
                return false;

            var size = Convert.ToInt32(regex.Groups[1].Value);
            var unit = regex.Groups[2].Value;

            switch (unit)
            {
                default:
                    return false;
                case "in":
                    if (size < 59 || size > 76)
                        return false;
                    break;
                case "cm":
                    if (size < 150 || size > 193)
                        return false;
                    break;
            }

            return true;
        }

        private bool ValidateYear(string year, int min, int max)
        {
            if (year.Length != 4)
                return false;

            var converted = Convert.ToInt32(year);
            
            if (converted < min || converted > max)
                return false;

            return true;
        }
    }
}