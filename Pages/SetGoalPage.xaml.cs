using System.Text.Json;

namespace GoalQuest
{
    public partial class SetGoalPage : BasePage
    {
        private Dictionary<string, List<GoalItem>> _goals;
        private List<GoalItem> _tempGoals;
        private readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GoalsData.json");

        public SetGoalPage()
        {
            InitializeComponent();
            _tempGoals = new List<GoalItem>();
            _goals = LoadGoals();
            LoadExistingGoals();
        }

        private void LoadExistingGoals()
        {
            string dateKey = DateTime.Now.ToString("yyyy-MM-dd");

            //_tempGoals.Clear();

            if (_goals.TryGetValue(dateKey, out var existingGoals))
            {
                _tempGoals.AddRange(existingGoals
                    .Select(g => new GoalItem(g.Goal, g.Points, RemoveGoal)));
            }
            DisplayGoals();
        }

        private void OnAddGoalClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(GoalEntry.Text) || PointPicker.SelectedItem == null) return;

            int points = int.Parse(PointPicker.SelectedItem.ToString());
            _tempGoals.Add(new GoalItem(GoalEntry.Text, points, RemoveGoal));

            GoalEntry.Text = string.Empty;
            DisplayGoals();
        }

        private void DisplayGoals()
        {
            GoalsCollection.ItemsSource = null; // Ensure proper UI refresh
            GoalsCollection.ItemsSource = _tempGoals;
            UpdateTotalPoints();
        }

        private void UpdateTotalPoints()
        {
            int totalPoints = _tempGoals.Sum(g => g.Points);
            TotalPointsLabel.Text = $"Total Points: {totalPoints}";
        }

        private void RemoveGoal(GoalItem goalItem)
        {
            _tempGoals.Remove(goalItem);
            DisplayGoals();
        }

        private async void OnSaveGoalsClicked(object sender, EventArgs e)
        {
            string dateKey = DateTime.Now.ToString("yyyy-MM-dd");

            _goals[dateKey] = _tempGoals
            .Select(g => new GoalItem(g.Goal, g.Points, RemoveGoal))
            .ToList();

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
                    var rawData = JsonSerializer.Deserialize<Dictionary<string, List<GoalItemDto>>>(json);

                    return rawData?.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Select(g => new GoalItem(g.Goal, g.Points, RemoveGoal)).ToList()
                    ) ?? new Dictionary<string, List<GoalItem>>();
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
        public string DisplayText => $"{Goal} \n{Points} Points";
        public Command RemoveCommand { get; }

        public GoalItem(string goal, int points, Action<GoalItem> removeAction)
        {
            Goal = goal;
            Points = points;
            RemoveCommand = new Command(() => removeAction(this));
        }
    }
    public class GoalItemDto
    {
        public string Goal { get; set; }
        public int Points { get; set; }
    }
}
