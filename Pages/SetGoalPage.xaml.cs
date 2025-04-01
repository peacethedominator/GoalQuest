using System.Text.Json;
using System.Text.Json.Serialization;

namespace GoalQuest
{
    public partial class SetGoalPage : BasePage
    {
        private Dictionary<string, List<GoalItem>> _goals;
        private readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GoalsData.json");

        public SetGoalPage()
        {
            InitializeComponent();
            _goals = LoadGoals();
            LoadExistingGoals();
        }

        private void LoadExistingGoals()
        {
            DateTime today = DateTime.Today;
            var filteredGoals = _goals
                .Where(g => DateTime.Parse(g.Key) >= today)
                .OrderBy(g => g.Key)
                .SelectMany(g => g.Value)
                .ToList();

            GoalsCollection.ItemsSource = filteredGoals;
            UpdateTotalPoints();
        }

        private async void OnAddGoalClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(GoalEntry.Text) || PointPicker.SelectedItem == null) return;

            DateTime selectedDate = GoalDatePicker.Date;
            if (selectedDate < DateTime.Today)
            {
                await DisplayAlert("Invalid Date", "You cannot add goals for past dates.", "OK");
                return;
            }

            string selectedDateString = selectedDate.ToString("yyyy-MM-dd");
            int points = int.Parse(PointPicker.SelectedItem.ToString());

            var newGoal = new GoalItem(GoalEntry.Text, points, selectedDateString, RemoveGoal, EditGoal);

            if (!_goals.ContainsKey(selectedDateString))
            {
                _goals[selectedDateString] = new List<GoalItem>();
            }
            _goals[selectedDateString].Add(newGoal);

            GoalEntry.Text = string.Empty;
            LoadExistingGoals();
        }

        private void UpdateTotalPoints()
        {
            if (GoalsCollection.ItemsSource is List<GoalItem> currentGoals)
            {
                int totalPoints = currentGoals.Sum(g => g.Points);
                TotalPointsLabel.Text = $"Total Points: {totalPoints}";
            }
        }

        private void RemoveGoal(GoalItem goalItem)
        {
            if (goalItem == null || string.IsNullOrEmpty(goalItem.Date)) return;

            string goalDateKey = goalItem.Date;

            if (_goals.ContainsKey(goalDateKey))
            {
                _goals[goalDateKey].Remove(goalItem);
                if (_goals[goalDateKey].Count == 0)
                {
                    _goals.Remove(goalDateKey);
                }
            }
            LoadExistingGoals();
        }

        private void EditGoal(GoalItem goalItem)
        {
            if (goalItem == null || string.IsNullOrEmpty(goalItem.Date)) return;

            GoalEntry.Text = goalItem.Goal;

            if (DateTime.TryParse(goalItem.Date, out DateTime parsedDate))
            {
                GoalDatePicker.Date = parsedDate;
            }
            else
            {
                Console.WriteLine($"Invalid date format: {goalItem.Date}");
            }

            PointPicker.SelectedItem = goalItem.Points.ToString();
            RemoveGoal(goalItem);
        }

        private async void OnSaveGoalsClicked(object sender, EventArgs e)
        {
            SaveGoals();
            await DisplayAlert("Success", "Goals saved successfully!", "OK");
            await Navigation.PushAsync(new HomePage());
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

        private Dictionary<string, List<GoalItem>> LoadGoals()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    string json = File.ReadAllText(_filePath);
                    var loadedGoals = JsonSerializer.Deserialize<Dictionary<string, List<GoalItem>>>(json) ?? new Dictionary<string, List<GoalItem>>();
                    var result = new Dictionary<string, List<GoalItem>>();

                    DateTime today = DateTime.Today;
                    foreach (var dateEntry in loadedGoals)
                    {
                        if (DateTime.Parse(dateEntry.Key) >= today)
                        {
                            var date = dateEntry.Key;
                            var goalList = dateEntry.Value;
                            foreach (var goal in goalList)
                            {
                                goal.Date = date;
                                goal.InitializeCommands(RemoveGoal, EditGoal);
                            }
                            result[date] = goalList;
                        }
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading goals: {ex.Message}");
                }
            }
            return new Dictionary<string, List<GoalItem>>();
        }
    }

    public class GoalItem
    {
            public string Goal { get; set; }
            public int Points { get; set; }
            public string Date { get; set; }

            [JsonIgnore]
            public Command RemoveCommand { get; private set; }

            [JsonIgnore]
            public Command EditCommand { get; private set; }

            public int Status { get; set; }
            public string ButtonText { get; set; }
            public string ButtonColor { get; set; }
            public GoalItem() { }

            public GoalItem(string goal, int points, string date, Action<GoalItem> removeAction, Action<GoalItem> editAction)
            {
                Goal = goal;
                Points = points;
                Date = date;
                InitializeCommands(removeAction, editAction);
            }

            public void InitializeCommands(Action<GoalItem> removeAction, Action<GoalItem> editAction)
            {
                RemoveCommand = new Command(() => removeAction(this));
                EditCommand = new Command(() => editAction(this));
            }
    }
}