using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using GoalQuest.Models;

namespace GoalQuest
{
    public partial class UserProfilePage : BasePage
    {
        private readonly string userFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UserData");
        private readonly string filePath;
        private string _profileImagePath;
        private readonly string defaultImagePath = "default_profile.jpg";

        public UserProfilePage()
        {
            InitializeComponent();

            if (!Directory.Exists(userFolder))
            {
                Directory.CreateDirectory(userFolder);
            }

            filePath = Path.Combine(userFolder, "profile.json");
            LoadProfileDataAsync();
        }

        public async Task LoadProfileDataAsync()
        {
            if (File.Exists(filePath))
            {
                var data = await File.ReadAllTextAsync(filePath);
                var profile = JsonSerializer.Deserialize<Profile>(data);

                if (profile != null)
                {
                    NameEntry.Text = profile.Name;
                    DOBEntry.Date = DateTime.Parse(profile.DateOfBirth);
                    MotivationEntry.Text = profile.Motivation;
                    _profileImagePath = string.IsNullOrWhiteSpace(profile.ImagePath) ? defaultImagePath : profile.ImagePath;
                    ProfileImageButton.Source = ImageSource.FromFile(_profileImagePath);
                }
            }
            else
            {
                _profileImagePath = defaultImagePath;
                ProfileImageButton.Source = ImageSource.FromFile(defaultImagePath);
            }
        }

        private async Task SaveProfileData(string name, string dob, string aim, string imagePath)
        {
            string destinationPath = string.IsNullOrWhiteSpace(imagePath) ? defaultImagePath : Path.Combine(userFolder, Path.GetFileName(imagePath));
            
            try
            {
                if (!Directory.Exists(userFolder))
                {
                    Directory.CreateDirectory(userFolder);
                }

                Profile existingProfile = null;
                if (File.Exists(filePath))
                {
                    string existingData = await File.ReadAllTextAsync(filePath);
                    existingProfile = JsonSerializer.Deserialize<Profile>(existingData);
                }

                if (!string.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath))
                {
                    if (existingProfile == null || existingProfile.ImagePath != destinationPath)
                    {
                        File.Copy(imagePath, destinationPath, overwrite: true);
                    }
                    else
                    {
                        Console.WriteLine("No changes in image. Skipping copy.");
                    }
                }

                var profile = new Profile
                {
                    Name = name,
                    DateOfBirth = dob,
                    Motivation = aim,
                    ImagePath = destinationPath
                };
                
                var jsonData = JsonSerializer.Serialize(profile, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(filePath, jsonData);

                Console.WriteLine("Profile data saved.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving profile: {ex.Message}");
            }
        }

        private async void OnImageButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Pick a profile picture"
                });

                if (result != null)
                {
                    _profileImagePath = result.FullPath;
                    ProfileImageButton.Source = ImageSource.FromFile(_profileImagePath);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to pick image: {ex.Message}", "OK");
            }
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                string name = NameEntry.Text;
                string dob = DOBEntry.Date.ToString("yyyy-MM-dd");
                string aim = MotivationEntry.Text;

                await SaveProfileData(name, dob, aim, _profileImagePath);

                await DisplayAlert("Success", "Profile saved successfully!", "OK");
                await Navigation.PushAsync(new HomePage());
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
                string.IsNullOrWhiteSpace(MotivationEntry.Text))
            {
                DisplayAlert("Error", "Please fill in all fields.", "OK");
                return false;
            }

            return true;
        }
    }

}