using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Year2021.Day21
{
    public class Puzzle : PuzzleBase
    {
        private Dictionary<(int, int, long, long), (long, long)> GameStates = new Dictionary<(int, int, long, long), (long, long)>();
        
        public Puzzle()
        {
            this.Init(21, false);

            Part1();
            Part2();
        }

        private void Part1()
        {
            // We play until a player has 1000 or more 
            var players = new[]
            {
                new Player(1, 3),
                new Player(2, 7)
            };
            
            var currentPlayer = 0;
            var dice = 0;
            
            // Keep playing the game until someone has a score of 1000
            while (!players.Any(x => x.Score >= 1000))
            {
                // Reset to player 0
                if (currentPlayer >= players.Length)
                    currentPlayer = 0;

                // Calculate thrown score
                var toMove = (dice + 1) + (dice + 2) + (dice + 3);

                // Move pawn (current + remainder of thrown score)
                var currentPosition = players[currentPlayer].Position + (toMove % 10);
                if (currentPosition > 10)
                    currentPosition -= 10;
                
                // Increase dice
                dice += 3;

                // Add score
                players[currentPlayer].Position = currentPosition;
                players[currentPlayer].Score += currentPosition;
                currentPlayer += 1;
            }
            
            Console.WriteLine($"Part 1 - Final score: {players.Select(x => x.Score).Min() * dice}");
        }
        
        private void Part2()
        {
            var result = CalculatePlayerWins(3, 7, 0, 0);
            Console.WriteLine($"Part 2 - Player 1 wins: {result.p1Wins}, Player 2 wins: {result.p2Wins}");
            // After some calculating...it seems you can better be player 1...
            // since he wins 94% of the time in ANY given start position for either player or player 2 \o/
        }

        private (long p1Wins, long p2Wins) CalculatePlayerWins(int currentPlayerPos, int waitingPlayerPos, long currentPlayerScore, long waitingPlayerScore)
        {
            // If any player has more then 21 points they win
            if (currentPlayerScore >= 21)
                return (1, 0);

            if (waitingPlayerScore >= 21)
                return (0, 1);

            // Return cached gamestate if it exists, the score calculation isn't hard...but the recursion is ;p
            if (GameStates.ContainsKey((currentPlayerPos, waitingPlayerPos, currentPlayerScore, waitingPlayerScore)))
                return GameStates[(currentPlayerPos, waitingPlayerPos, currentPlayerScore, waitingPlayerScore)];
            
            (long player1Wins, long player2Wins) result = (0, 0); 

            // Calculate each possible outcome in each die roll the next die rolls every outcome again
            for(var r1 = 1; r1 <= 3; r1++)
            {
                for(var r2 = 1; r2 <= 3; r2++)
                {
                    for(var r3 = 1; r3 <= 3; r3++)
                    {
                        var newPositionCurrentPlayer = (currentPlayerPos + r1 + r2 + r3);
                        
                        // Loop back the position (since it's never higher then 19 we can just subtract the 10)
                        if (newPositionCurrentPlayer > 10)
                            newPositionCurrentPlayer -= 10;
                        
                        var newScoreCurrentPlayer = currentPlayerScore + newPositionCurrentPlayer;
                        
                        // Switch positions around (waitingPlayer's turn now) and calculate their answers going recursively deeper and deeper
                        // meaning they keep switching around
                        var (p1Wins, p2Wins) = CalculatePlayerWins(
                            waitingPlayerPos, 
                            newPositionCurrentPlayer, 
                            waitingPlayerScore, 
                            newScoreCurrentPlayer
                        );
                        
                        // The result for this method will return the wins for currentPlayer being the waitingPlayer in the initial call
                        result.player1Wins += p2Wins;
                        result.player2Wins += p1Wins;
                    }
                }
            }
            
            // Store result of this state so it won't be calculated again
            GameStates[(currentPlayerPos, waitingPlayerPos, currentPlayerScore, waitingPlayerScore)] = result;
            
            return result;
        }
    }

    public class Player
    {
        public Player(int playerNumber, int position)
        {
            PlayerNumber = playerNumber;
            Position = position;
        }
        
        public int PlayerNumber { get; set; }
        public int Position { get; set; }
        public int Score { get; set; }
    }
}