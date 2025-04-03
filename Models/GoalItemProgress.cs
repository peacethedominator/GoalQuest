using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GoalQuest.Models
{
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

}
