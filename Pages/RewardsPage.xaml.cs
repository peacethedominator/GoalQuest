using System.Text.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GoalQuest.Models;

namespace GoalQuest
{
    public partial class RewardsPage : BasePage, INotifyPropertyChanged
    {
        private readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GoalsData.json");
        private Dictionary<string, List<GoalItemProgress>> _goals;
        private string _dateKey;

        private int _dailyPoints;
        private int _allTimePoints;
        private int _level;
        private double _levelThreshold;

        public int DailyPoints
        {
            get => _dailyPoints;
            set { _dailyPoints = value; OnPropertyChanged(); UpdateProgress(); }
        }

        public int AllTimePoints
        {
            get => _allTimePoints;
            set { _allTimePoints = value; OnPropertyChanged(); UpdateProgress(); }
        }

        public int Level
        {
            get => _level;
            set { _level = value; OnPropertyChanged(); }
        }

        public double LevelThreshold
        {
            get => _levelThreshold;
            set { _levelThreshold = value; OnPropertyChanged(); }
        }

        public RewardsPage()
        {
            InitializeComponent();
            _dateKey = DateTime.Now.ToString("yyyy-MM-dd");
            _goals = LoadGoals();
            CalculatePoints();
            ApplyDailyPenalty();
            CalculateLevel();
            BindingContext = this;
        }

        private void CalculatePoints()
        {
            _dailyPoints = 0;
            _allTimePoints = 0;

            foreach (var goalEntry in _goals.Values)
            {
                foreach (var goal in goalEntry)
                {
                    if (goal.Status == GoalStatus.Completed)
                    {
                        _allTimePoints += goal.Points;
                    }
                }
            }

            if (_goals.ContainsKey(_dateKey))
            {
                foreach (var goal in _goals[_dateKey])
                {
                    if (goal.Status == GoalStatus.Completed)
                    {
                        _dailyPoints += goal.Points;
                    }
                }
            }

            DailyPoints = _dailyPoints;
            AllTimePoints = _allTimePoints;
        }

        private void ApplyDailyPenalty()
        {
            if (!_goals.ContainsKey(_dateKey) || _goals[_dateKey].All(g => g.Status == GoalStatus.Incomplete))
            {
                int penalty = (int)(AllTimePoints * 0.1);
                if (penalty > 0)
                {
                    AllTimePoints -= penalty;
                    SaveGoals();
                }
            }
        }

        private void CalculateLevel()
        {
            Level = 1;
            LevelThreshold = 100;
            int remainingPoints = AllTimePoints;

            while (remainingPoints >= LevelThreshold)
            {
                remainingPoints -= (int)LevelThreshold;
                Level++;
                LevelThreshold *= 1.05;
            }

            CurrentLevelLabel.Text = $"Level: {Level}";
            NextLevelLabel.Text = $"Next level after: {(int)(LevelThreshold - remainingPoints)} pts.";
        }


        private void UpdateProgress()
        {
            int totalDailyPoints = _goals.ContainsKey(_dateKey)
                ? _goals[_dateKey].Sum(g => g.Points)
                : 0;

            int totalAllTimePoints = _goals.Values
                .SelectMany(g => g)
                .Sum(g => g.Points);

            double dailyProgress = (totalDailyPoints > 0) ? (double)DailyPoints / totalDailyPoints : 0;
            double allTimeProgress = (totalAllTimePoints > 0) ? (double)AllTimePoints / totalAllTimePoints : 0;

            if (DailyProgressBar != null)
            {
                DailyProgressBar.Progress = Math.Min(dailyProgress, 1.0);
            }

            if (AllTimeProgressBar != null)
            {
                AllTimeProgressBar.Progress = Math.Min(allTimeProgress, 1.0);
            }

            DailyPointsLabel.Text = $"{DailyPoints}/{totalDailyPoints} pts";
            DailyProgressPercentageLabel.Text = $"{(dailyProgress * 100):0}% Complete";

            AllTimePointsLabel.Text = $"{AllTimePoints}/{totalAllTimePoints} pts";
            AllTimeProgressPercentageLabel.Text = $"{(allTimeProgress * 100):0}% Complete";

            int streak = CalculateStreak();
            StreakLabel.Text = $"{streak}-day streak!";
        }


        private Dictionary<string, List<GoalItemProgress>> LoadGoals()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    string json = File.ReadAllText(_filePath);
                    var data = JsonSerializer.Deserialize<Dictionary<string, List<GoalItemProgress>>>(json);
                    return data ?? new Dictionary<string, List<GoalItemProgress>>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading goals: {ex.Message}");
                }
            }
            return new Dictionary<string, List<GoalItemProgress>>();
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
        private int CalculateStreak()
        {
            int streak = 0;
            DateTime currentDate = DateTime.Now;

            while (true)
            {
                string dateKey = currentDate.ToString("yyyy-MM-dd");

                if (_goals.ContainsKey(dateKey) && _goals[dateKey].Any(g => g.Status == GoalStatus.Completed))
                {
                    streak++;
                    currentDate = currentDate.AddDays(-1);
                }
                else
                {
                    break; 
                }
            }

            return streak;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
