using System;
using System.Collections.Generic;
using System.Windows;

namespace DungeonDiceQuest
{
    public partial class MainWindow : Window
    {
        // Game state variables
        private List<Player> players;
        private int currentPlayerIndex;
        private Random random;
        private int healthPool;
        private const int initialHealthPool = 100;

        // Dungeon variables
        private int dungeonSize = 10;
        private List<Room> dungeon;
        private int currentRoomIndex;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            players = new List<Player>();
            currentPlayerIndex = 0;
            random = new Random();
            healthPool = initialHealthPool;

            // Dungeon generation
            dungeon = GenerateDungeon(dungeonSize);
            currentRoomIndex = 0;

            // Show Player Selection UI
            PlayerSelectionGrid.Visibility = Visibility.Visible;
            GameUIGrid.Visibility = Visibility.Collapsed;
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            // Gather selected players
            if (KnightCheckbox.IsChecked == true)
                players.Add(new Player("Knight", new List<string> { "Shield Block", "Sword Strike", "Courage" }));
            if (WizardCheckbox.IsChecked == true)
                players.Add(new Player("Wizard", new List<string> { "Fireball", "Mana Shield", "Lightning" }));
            if (ThiefCheckbox.IsChecked == true)
                players.Add(new Player("Thief", new List<string> { "Backstab", "Stealth", "Evasion" }));
            if (ClericCheckbox.IsChecked == true)
                players.Add(new Player("Cleric", new List<string> { "Heal", "Holy Light", "Protection" }));

            if (players.Count == 0)
            {
                MessageBox.Show("Please select at least one player to start the game.", "Player Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Switch to Game UI
            PlayerSelectionGrid.Visibility = Visibility.Collapsed;
            GameUIGrid.Visibility = Visibility.Visible;

            StartGame();
        }

        private void StartGame()
        {
            // Initialize game logic
            UpdateUI();
        }

        private List<Room> GenerateDungeon(int size)
        {
            List<Room> dungeon = new List<Room>();
            for (int i = 0; i < size; i++)
            {
                dungeon.Add(new Room
                {
                    Type = (RoomType)random.Next(3), // Randomly generate room type
                    Difficulty = random.Next(1, 11) // Difficulty level from 1 to 10
                });
            }

            // Add an exit room as the last room
            dungeon.Add(new Room { Type = RoomType.Exit });
            return dungeon;
        }

        private void UpdateUI()
        {
            // Update health, current player, and room details on the UI
            HealthTextBlock.Text = $"Health Pool: {healthPool}";
            PlayerTextBlock.Text = $"Current Player: {players[currentPlayerIndex].Name}";
            RoomTextBlock.Text = $"Room {currentRoomIndex + 1}/{dungeon.Count}: {dungeon[currentRoomIndex].Type}";
        }

        private void RollDiceButton_Click(object sender, RoutedEventArgs e)
        {
            int roll = RollDice(6); // Roll a 6-sided die
            DiceResultTextBlock.Text = $"You rolled: {roll}";

            HandleRoomChallenge(roll);
        }

        private int RollDice(int sides)
        {
            return random.Next(1, sides + 1);
        }

        private void HandleRoomChallenge(int roll)
        {
            Room currentRoom = dungeon[currentRoomIndex];

            switch (currentRoom.Type)
            {
                case RoomType.Monster:
                    HandleMonsterChallenge(roll, currentRoom);
                    break;
                case RoomType.Trap:
                    HandleTrapChallenge(roll, currentRoom);
                    break;
                case RoomType.Treasure:
                    HandleTreasureChallenge(roll, currentRoom);
                    break;
                case RoomType.Exit:
                    MessageBox.Show("Congratulations! You have found the exit and won the game!", "Victory");
                    Close();
                    return;
            }

            // Update health pool and check for game over
            if (healthPool <= 0)
            {
                MessageBox.Show("You have run out of health. Game over!", "Defeat");
                Close();
                return;
            }

            // Move to the next player and room
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            currentRoomIndex++;

            if (currentRoomIndex >= dungeon.Count)
            {
                MessageBox.Show("No more rooms left. Game over!", "Game Over");
                Close();
            }
            else
            {
                UpdateUI();
            }
        }

        private void HandleMonsterChallenge(int roll, Room room)
        {
            if (roll >= room.Difficulty)
            {
                MessageBox.Show("You defeated the monster!", "Victory");
            }
            else
            {
                int damage = room.Difficulty - roll;
                healthPool -= damage;
                MessageBox.Show($"You were injured! Lost {damage} health.", "Defeat");
            }
        }

        private void HandleTrapChallenge(int roll, Room room)
        {
            if (roll >= room.Difficulty)
            {
                MessageBox.Show("You avoided the trap!", "Success");
            }
            else
            {
                int damage = room.Difficulty - roll;
                healthPool -= damage;
                MessageBox.Show($"You triggered the trap! Lost {damage} health.", "Trap Activated");
            }
        }

        private void HandleTreasureChallenge(int roll, Room room)
        {
            if (roll >= room.Difficulty)
            {
                int reward = roll - room.Difficulty;
                healthPool += reward;
                MessageBox.Show($"You found treasure! Gained {reward} health.", "Treasure");
            }
            else
            {
                MessageBox.Show("The treasure was too difficult to obtain.", "Failure");
            }
        }
    }

    public class Player
    {
        public string Name { get; set; }
        public List<string> Abilities { get; set; }

        public Player(string name, List<string> abilities)
        {
            Name = name;
            Abilities = abilities;
        }
    }

    public class Room
    {
        public RoomType Type { get; set; }
        public int Difficulty { get; set; }
    }

    public enum RoomType
    {
        Monster,
        Trap,
        Treasure,
        Exit
    }
}
