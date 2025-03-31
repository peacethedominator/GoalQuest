using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;
using System.Text.Json.Serialization;

namespace GoalQuest
{
    public partial class TrackProgressPage : BasePage
    {
        private Dictionary<string, List<GoalItemProgress>> _goals;
        private readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GoalsData.json");
        private string _dateKey;

        public ObservableCollection<GoalItemProgress> Goals { get; set; } = new(); // Use ObservableCollection

        public TrackProgressPage()
        {
            InitializeComponent();
            _dateKey = DateTime.Now.ToString("yyyy-MM-dd");
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _goals = LoadGoals();
            LoadExistingGoals();
        }

        private void LoadExistingGoals()
        {
            if (!_goals.TryGetValue(_dateKey, out var existingGoals))
            {
                existingGoals = new List<GoalItemProgress>();
                _goals[_dateKey] = existingGoals;
            }

            Goals.Clear(); 

            foreach (var goal in existingGoals)
            {
                goal.UpdateButtonProperties();

                goal.ToggleStatusCommand = new Command(() => ToggleGoalStatus(goal));

                Goals.Add(goal);
            }

            UpdateDailyProgress();
        }


        private void ToggleGoalStatus(GoalItemProgress goal)
        {
            goal.Status = goal.Status == GoalStatus.Incomplete ? GoalStatus.Completed : GoalStatus.Incomplete;
            goal.UpdateButtonProperties();
            SaveGoals();
            UpdateDailyProgress();
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

        [JsonIgnore]
        public Command ToggleStatusCommand { get; set; }

        public GoalItemProgress(string goal, int points)
        {
            Goal = goal;
            Points = points;
            Status = GoalStatus.Incomplete;
            UpdateButtonProperties();
            ToggleStatusCommand = new Command(() => { }); 
        }

        public void UpdateButtonProperties()
        {
            ButtonText = Status == GoalStatus.Completed ? "✔" : "✖";
            ButtonColor = Status == GoalStatus.Completed ? "Green" : "Red";
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
