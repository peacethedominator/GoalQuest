
namespace GoalQuest
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private async void OnSetGoalClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SetGoalPage());
        }

        private async void OnTrackProgressClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TrackProgressPage());
        }

        private async void OnUserProfileClicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new UserProfilePage());
            var userProfilePage = new UserProfilePage();
            await userProfilePage.LoadProfileDataAsync(); 
            await Navigation.PushAsync(userProfilePage);
        }

        private async void OnRewardsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RewardsPage());
        }

        private async void OnGoalHistoryClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GoalHistory());
        }
        
    }
}
