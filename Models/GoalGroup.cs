using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoalQuest.Models
{
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
