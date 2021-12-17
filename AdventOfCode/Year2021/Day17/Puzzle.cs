using System;
using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;

namespace AdventOfCode.Year2021.Day17
{
    public class Puzzle : PuzzleBase
    {
        public Puzzle()
        {
            this.Init(17, true);
            var input = this.GetPuzzleText().Trim();

            var match = new Regex("(x=([\\-0-9]{1,})\\.\\.([\\-0-9]{1,})), (y=([\\-0-9]{1,})\\.\\.([\\-0-9]{1,}))").Match(input);
            
            var xS = Convert.ToInt32(match.Groups[2].Value);
            var xE = Convert.ToInt32(match.Groups[3].Value);
            var yS = Convert.ToInt32(match.Groups[6].Value);
            var yE = Convert.ToInt32(match.Groups[5].Value);
            
            Part1((xS, xE, yS, yE));
        }

        public void Part1((int xS, int xE, int yS, int yE) targetArea)
        {
            var currentPosition = (0, 0);
            var velocity = (0, 0);

            var test1 = DoesEnterTarget((6, 2), targetArea);
            var test2 = DoesEnterTarget((6, 3), targetArea);
            var test3 = DoesEnterTarget((9, 0), targetArea);
            var test4 = DoesEnterTarget((17, -4), targetArea);
            
            // Determine horizontal velocity
            if (targetArea.xS > 0 && targetArea.xE > 0)
            {
                // Shooting to the right
            }
            else
            {
                // Shooting to the left
            }

            if (targetArea.yS < 0 && targetArea.yE < 0)
            {
                // Shooting below
            }
            else
            {
                // Shooting up
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

                velocity.vX -= 1;
                velocity.vY -= 1;
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