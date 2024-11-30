using System;
using System.Windows;

namespace DungeonDiceQuest
{
    public partial class MainWindow : Window
    {
        private int health = 100; // Shared health pool

        public MainWindow()
        {
            InitializeComponent();
        }

        private void RollDice_Click(object sender, RoutedEventArgs e)
        {
            // Simulate a dice roll (6-sided)
            Random rand = new Random();
            int diceRoll = rand.Next(1, 7); // 1 to 6
            DiceResult.Text = $"Result: {diceRoll}";

            // Generate a random challenge
            string challenge = GenerateChallenge(diceRoll);
            GameLog.Items.Add(challenge);

            // Update health based on challenge outcome
            if (diceRoll < 3) // Example condition for losing health
            {
                health -= 10;
                GameLog.Items.Add("Lost 10 health!");
            }
            else
            {
                GameLog.Items.Add("Safe for now...");
            }

            // Update health bar
            HealthBar.Value = health;

            // Check for loss condition
            if (health <= 0)
            {
                MessageBox.Show("Game Over! You lost all your health.");
                ResetGame();
            }
        }

        private string GenerateChallenge(int diceRoll)
        {
            // Randomly decide between monster, trap, or treasure
            string[] challenges = { "Monster Encountered!", "Trap Activated!", "Found Treasure!" };
            Random rand = new Random();
            return challenges[rand.Next(challenges.Length)];
        }

        private void ResetGame()
        {
            health = 100;
            HealthBar.Value = health;
            GameLog.Items.Clear();
            DiceResult.Text = "Result: ";
        }
    }
}
