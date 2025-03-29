using System;
using Microsoft.Maui.Controls;
using System.IO;

namespace GoalQuest
{
    public partial class WelcomePage : ContentPage
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

                if (!string.IsNullOrEmpty(_profile.ImagePath) && File.Exists(_profile.ImagePath))
                {
                    ProfileImage.Source = ImageSource.FromFile(_profile.ImagePath);
                }
                else
                {
                    ProfileImage.Source = "placeholder.png"; // Ensure this file exists in Resources/Images
                }
            }
        }

        private async void GoToHome_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HomePage());
        }
    }
}
