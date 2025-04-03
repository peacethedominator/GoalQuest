using System.Collections.ObjectModel;
using System.Text.Json;
using GoalQuest.Models;

namespace GoalQuest
{
    public partial class TrackProgressPage : BasePage
    {
        private Dictionary<string, List<GoalItemProgress>> _goals;
        private readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GoalsData.json");
        private string _dateKey;

        public ObservableCollection<GoalItemProgress> Goals { get; set; } = new();

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
    
    public enum GoalStatus
    {
        Incomplete,
        Completed
    }
}
