using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;

namespace AdventOfCode.Year2021.Day17
{
    public class Puzzle : PuzzleBase
    {
        public Puzzle()
        {
            this.Init(17, false);
            var input = this.GetPuzzleText().Trim();

            var match = new Regex("(x=([\\-0-9]{1,})\\.\\.([\\-0-9]{1,})), (y=([\\-0-9]{1,})\\.\\.([\\-0-9]{1,}))").Match(input);
            
            var xS = Convert.ToInt32(match.Groups[2].Value);
            var xE = Convert.ToInt32(match.Groups[3].Value);
            var yS = Convert.ToInt32(match.Groups[6].Value);
            var yE = Convert.ToInt32(match.Groups[5].Value);
            
            Part1((xS, xE, yS, yE));
            Part2((xS, xE, yS, yE));
        }

        public void Part1((int xS, int xE, int yS, int yE) targetArea)
        {
            var currentX = MinimumXVelocity(targetArea.xS);
            var currentY = 1;
            var maxY = 0;
            var bestVelocity = (currentX, currentY);
            
            while (true)
            {
                var yTries = 0;
                while (true)
                {
                    var testVelocity = (currentX, currentY);
                    // Find heighest y for current x
                    if (DoesEnterTarget(testVelocity, targetArea))
                    {
                        var maxYVelocity = CalculateMaximumY(testVelocity);
                        if (maxYVelocity > maxY)
                        {
                            bestVelocity = testVelocity;
                            maxY = maxYVelocity;
                        }
                    }

                    yTries++;
                    currentY++;

                    if (yTries > 250)
                        break;
                }

                currentY = 1;
                currentX++;

                if (currentX >= targetArea.xE)
                    break;
            }
            
            Console.WriteLine($"Part 1 - The best shooting position is {bestVelocity.currentX}, {bestVelocity.currentY} with a maximum Y of {CalculateMaximumY(bestVelocity)}");
        }
        
        private void Part2((int xS, int xE, int yS, int yE) targetArea)
        {
            var velocities = new HashSet<(int vX, int vY)>();

            // Positive Y
            LoopVelocityRange(velocities, targetArea, "up");
            LoopVelocityRange(velocities, targetArea, "down");
            
            Console.WriteLine($"Part 2 - Total velocities reaching goal: {velocities.Count}");
        }

        private void LoopVelocityRange(
            HashSet<(int, int)> velocities, 
            (int xS, int xE, int yS, int yE) targetArea,
            string direction)
        {
            var currentX = MinimumXVelocity(targetArea.xS) - 3;
            var maxX = targetArea.xE;
            
            var currentY = 1;
            while (true)
            {
                var yTries = 0;
                while (true)
                {
                    var testVelocity = (currentX, currentY);
                    // Find heighest y for current x
                    if (DoesEnterTarget(testVelocity, targetArea))
                    {
                        velocities.Add(testVelocity);
                    }

                    yTries++;

                    if (direction == "up")
                        currentY++;
                    else
                        currentY--;

                    if (yTries > 250)
                        break;
                }

                if (currentX > maxX)
                    break;
                
                currentY = 0;
                currentX++;
            }
        }

        private bool DoesEnterTarget((int vX, int vY) velocity, (int xS, int xE, int yS, int yE) targetArea)
        {
            (int x, int y) position = (0, 0);

            while (true)
            {
                position.x += velocity.vX;
                position.y += velocity.vY;

                if (IsInTargetArea(position, targetArea))
                    return true;

                if (HasOvershotTargetArea(position, targetArea))
                    return false;

                if (velocity.vX > 0)
                    velocity.vX -= 1;
                velocity.vY -= 1;
            }
        }
        
        private int CalculateMaximumY((int vX, int vY) velocity)
        {
            (int x, int y) position = (0, 0);
            
            while (true)
            {
                position.x += velocity.vX;
                position.y += velocity.vY;

                if (velocity.vY == 0)
                    return position.y;

                if (velocity.vX > 0)
                    velocity.vX -= 1;
                velocity.vY -= 1;
            }
        }

        private int MinimumXVelocity(int targetX)
        {
            var minimumX = 2;
            while (true)
            {
                var result = 0;
                for (var i = minimumX; i > 0; i--)
                {
                    result += i;
                }

                if (result >= targetX)
                    return minimumX;

                minimumX += 1;
            }
        }

        private bool IsInTargetArea((int x, int y) position, (int xS, int xE, int yS, int yE) targetArea)
        {
            return (position.x >= targetArea.xS && position.x <= targetArea.xE) &&
                   (position.y <= targetArea.yS && position.y >= targetArea.yE);
        }

        private bool HasOvershotTargetArea((int x, int y) position, (int xS, int xE, int yS, int yE) targetArea)
        {
            return (position.x > targetArea.xE || position.y < targetArea.yE);
        }
    }
}