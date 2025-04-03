using GoalQuest.Models;

namespace GoalQuest
{
    public partial class WelcomePage : BasePage
    {
        private readonly Profile _profile;

        public WelcomePage(Profile profile)
        {
            InitializeComponent();
            _profile = profile;
            SetProfileData();
        }

        private void SetProfileData()
        {
            if (_profile != null)
            {
                WelcomeLabel.Text = $"Welcome back, {_profile.Name}!";
                MotivationLabel.Text = $"Pumped up to be: {_profile.Motivation}?";

                string profileImagePath = _profile.ImagePath;

                if (string.IsNullOrEmpty(profileImagePath) || !File.Exists(profileImagePath))
                {
                    profileImagePath = "default_profile.jpg";
                }

                ProfileImage.Source = ImageSource.FromFile(profileImagePath);
            }
        }

        private async void GoToHome_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HomePage());
        }
    }
}
