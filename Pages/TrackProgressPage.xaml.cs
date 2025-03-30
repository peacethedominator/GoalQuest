using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;

namespace GoalQuest
{
    public partial class TrackProgressPage : ContentPage
    {
        private Dictionary<string, List<GoalItemProgress>> _goals;
        private readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GoalsData.json");
        private string _dateKey;

        public List<GoalItemProgress> Goals { get; set; } = new();

        public TrackProgressPage()
        {
            InitializeComponent();
            _dateKey = DateTime.Now.ToString("yyyy-MM-dd");
            _goals = LoadGoals();
            LoadExistingGoals();
            BindingContext = this;
        }

        private void LoadExistingGoals()
        {
            if (!_goals.TryGetValue(_dateKey, out var existingGoals))
            {
                existingGoals = new List<GoalItemProgress>();
                _goals[_dateKey] = existingGoals;
            }

            Goals = existingGoals;

            foreach (var goal in Goals)
            {
                goal.UpdateButtonProperties();
                goal.ToggleStatusCommand = new Command(() => ToggleGoalStatus(goal));
            }

            RefreshGoals();
            UpdateDailyProgress();
        }

        private void ToggleGoalStatus(GoalItemProgress goal)
        {
            if (goal.Status == GoalStatus.Incomplete)
            {
                goal.Status = GoalStatus.Completed;
            }
            else
            {
                goal.Status = GoalStatus.Incomplete;
            }

            goal.UpdateButtonProperties(); // Ensure UI updates
            SaveGoals();
            RefreshGoals();
            UpdateDailyProgress();
        }

        private void RefreshGoals()
        {
            GoalsCollection.ItemsSource = null;
            GoalsCollection.ItemsSource = Goals;
        }

        private void UpdateDailyProgress()
        {
            if (_goals.TryGetValue(_dateKey, out var goals))
            {
                int totalPoints = goals.Sum(g => g.Points);
                int completedPoints = goals.Where(g => g.Status == GoalStatus.Completed).Sum(g => g.Points);
                DailyProgressLabel.Text = $"Daily Progress: {completedPoints}/{totalPoints} points";
            }
        }

        private void SaveGoals()
        {
            try
            {
                string json = JsonSerializer.Serialize(_goals, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving goals: {ex.Message}");
            }
        }

        private Dictionary<string, List<GoalItemProgress>> LoadGoals()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    string json = File.ReadAllText(_filePath);
                    return JsonSerializer.Deserialize<Dictionary<string, List<GoalItemProgress>>>(json) ?? new Dictionary<string, List<GoalItemProgress>>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading goals: {ex.Message}");
                }
            }
            return new Dictionary<string, List<GoalItemProgress>>();
        }
    }

    public class GoalItemProgress : INotifyPropertyChanged
    {
        private GoalStatus _status;
        private string _buttonText;
        private string _buttonColor;

        public string Goal { get; set; }
        public int Points { get; set; }

        public GoalStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
                UpdateButtonProperties();
            }
        }

        public string ButtonText
        {
            get => _buttonText;
            private set
            {
                _buttonText = value;
                OnPropertyChanged();
            }
        }

        public string ButtonColor
        {
            get => _buttonColor;
            private set
            {
                _buttonColor = value;
                OnPropertyChanged();
            }
        }

        public Command ToggleStatusCommand { get; set; }

        public GoalItemProgress(string goal, int points)
        {
            Goal = goal;
            Points = points;
            Status = GoalStatus.Incomplete;
            UpdateButtonProperties();
            ToggleStatusCommand = new Command(() => { }); // Placeholder, will be assigned in page
        }

        public void UpdateButtonProperties()
        {
            if (Status == GoalStatus.Completed)
            {
                ButtonText = "✔";
                ButtonColor = "Green";
            }
            else
            {
                ButtonText = "✖";
                ButtonColor = "Red";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum GoalStatus
    {
        Incomplete,
        Completed
    }
}
