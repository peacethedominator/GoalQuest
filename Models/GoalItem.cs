using System.Text.Json.Serialization;

namespace GoalQuest.Models
{
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
