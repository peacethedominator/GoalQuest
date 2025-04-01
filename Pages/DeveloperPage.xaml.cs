using Microsoft.Maui.Controls;

namespace GoalQuest
{
    public partial class DeveloperPage : BasePage
    {
        public DeveloperPage()
        {
            InitializeComponent();
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
