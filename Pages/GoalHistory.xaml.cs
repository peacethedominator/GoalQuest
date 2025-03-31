using System.Text.Json;

namespace GoalQuest
{
    public partial class GoalHistory : BasePage
    {
        private readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GoalsData.json");

        public List<GoalGroup> GroupedGoals { get; set; }

        public GoalHistory()
        {
            InitializeComponent();
            LoadGoalHistory();
            BindingContext = this;
        }

        private void LoadGoalHistory()
        {
            var goalsData = LoadGoalsFromStorage(); 

            if (goalsData == null || goalsData.Count == 0)
            {
                GroupedGoals = new List<GoalGroup>();
                return;
            }

            GroupedGoals = goalsData
                .Select(entry => new
                {
                    Date = DateTime.TryParse(entry.Key, out DateTime parsedDate) ? parsedDate : DateTime.MinValue,
                    Goals = entry.Value
                })
                .OrderByDescending(entry => entry.Date)
                .Select(entry => new GoalGroup
                {
                    Date = $"Date: {entry.Date:dd/MM/yyyy}",
                    Goals = entry.Goals.Select(g => new GoalHistoryItem
                    {
                        GoalTitle = g.Goal,
                        StatusColor = g.Status == GoalStatus.Completed ? "#28A745" : "#DC3545"
                    }).ToList()
                })
                .ToList();
        }

        private Dictionary<string, List<GoalItemProgress>> LoadGoalsFromStorage()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    string json = File.ReadAllText(_filePath);
                    var goalsData = JsonSerializer.Deserialize<Dictionary<string, List<GoalItemProgress>>>(json);

                    return goalsData ?? new Dictionary<string, List<GoalItemProgress>>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading goal history: {ex.Message}");
                }
            }
            return new Dictionary<string, List<GoalItemProgress>>(); 
        }
    }

    public class GoalGroup
    {
        public string Date { get; set; }
        public List<GoalHistoryItem> Goals { get; set; }
    }

    public class GoalHistoryItem
    {
        public string GoalTitle { get; set; }
        public string StatusColor { get; set; }
    }
}
