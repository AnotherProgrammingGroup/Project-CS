using System;
using System.Collections.Generic;
using System.Threading;

namespace hacksoiHelloC
{
    class RockPaperScissors
    {
        public void go()
        {
            int userScore = 0, computerScore = 0;
            string[] choices = new string[] { "rock", "paper", "scissors" };
            Random computer = new Random();
            while(true)
            {
                Console.WriteLine("rock, paper, or scissors?");

                string userStrChoice = Console.ReadLine().ToLower();
                RPSChoice userChoice = RockPaperScissorsHandler.getChoice(userStrChoice);
                if(userChoice is NoChoice)
                {
                    Console.WriteLine("invalid input");
                    Console.WriteLine();
                    continue;
                }

                string computerStrChoice = choices[computer.Next(0, choices.Length)];
                RPSChoice computerChoice = RockPaperScissorsHandler.getChoice(computerStrChoice);

                RockPaperScissorsHandler.Outcome outcome = userChoice.duel(computerChoice);

                string result = "";
                switch (outcome)
                {
                    case RockPaperScissorsHandler.Outcome.TIE:
                        userScore++;
                        computerScore++;
                        result = "you tied!";
                        break;
                    case RockPaperScissorsHandler.Outcome.WIN:
                        userScore++;
                        result = "you won!";
                        break;
                    case RockPaperScissorsHandler.Outcome.LOSS:
                        computerScore++;
                        result = "you fucking suck!";
                        break;
                    default:
                        // can never reach here
                        break;
                }

                printResult(userStrChoice, computerStrChoice, result, userScore, computerScore);

                Console.WriteLine("play again? (anything for yes / no for no)");
                string playAgainStr = Console.ReadLine();
                if (playAgainStr.Equals("no"))
                    break;
                Console.WriteLine();
            }
        }

        private void printResult(string userStrChoice, string computerStrChoice, string result, int userScore, int computerScore)
        {
            Console.WriteLine();
            Console.Write("your choice: ");
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine(userStrChoice);

            System.Threading.Thread.Sleep(1000);

            Console.Write("computer's choice: ");
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine(computerStrChoice);

            System.Threading.Thread.Sleep(1000);

            Console.WriteLine(result);
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine();

            Console.WriteLine("your score: " + userScore);
            Console.WriteLine("computer's score: " + computerScore);
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine();
        }
    }

    class RockPaperScissorsHandler
    {
        public enum Outcome { LOSS, TIE, WIN, NULL };

        public static RPSChoice getChoice(string choice)
        {
            if (choice.Equals("rock"))
                return new Rock();
            if (choice.Equals("paper"))
                return new Paper();
            if (choice.Equals("scissors"))
                return new Scissors();
            return new NoChoice();
        }
    }

    interface RPSChoice
    {
        RockPaperScissorsHandler.Outcome duel(RPSChoice other);
    }

    class Rock : RPSChoice
    {
        public RockPaperScissorsHandler.Outcome duel(RPSChoice other)
        {
            if (other is Rock)
                return RockPaperScissorsHandler.Outcome.TIE;
            if (other is Paper)
                return RockPaperScissorsHandler.Outcome.LOSS;
            if (other is Scissors)
                return RockPaperScissorsHandler.Outcome.WIN;
            return RockPaperScissorsHandler.Outcome.NULL;
        }
    }

    class Paper : RPSChoice
    {
        public RockPaperScissorsHandler.Outcome duel(RPSChoice other)
        {
            if (other is Rock)
                return RockPaperScissorsHandler.Outcome.WIN;
            if (other is Paper)
                return RockPaperScissorsHandler.Outcome.TIE;
            if (other is Scissors)
                return RockPaperScissorsHandler.Outcome.LOSS;
            return RockPaperScissorsHandler.Outcome.NULL;
        }
    }

    class Scissors : RPSChoice
    {
        public RockPaperScissorsHandler.Outcome duel(RPSChoice other)
        {
            if (other is Rock)
                return RockPaperScissorsHandler.Outcome.LOSS;
            if (other is Paper)
                return RockPaperScissorsHandler.Outcome.WIN;
            if (other is Scissors)
                return RockPaperScissorsHandler.Outcome.TIE;
            return RockPaperScissorsHandler.Outcome.NULL;
        }
    }

    class NoChoice : RPSChoice
    {
        public RockPaperScissorsHandler.Outcome duel(RPSChoice other)
        {
            return RockPaperScissorsHandler.Outcome.NULL;
        }
    }

}
