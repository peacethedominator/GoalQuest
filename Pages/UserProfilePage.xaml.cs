using System.Text.Json;

namespace GoalQuest
{
    public partial class UserProfilePage : BasePage
    {
        private readonly string userFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UserData");
        private readonly string filePath;
        private string _profileImagePath;

        public UserProfilePage()
        {
            InitializeComponent();

            if (!Directory.Exists(userFolder))
            {
                Directory.CreateDirectory(userFolder);
            }

            filePath = Path.Combine(userFolder, "profile.json");
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
                    DOBEntry.Text = profile.DateOfBirth;
                    MotivationEntry.Text = profile.Motivation;
                    ProfileImageButton.Source = ImageSource.FromFile(profile.ImagePath);
                    _profileImagePath = profile.ImagePath;
                }
            }
        }

        private async void SaveProfileData(string name, string dob, string aim, string imagePath)
        {
            string imageFileName = Path.GetFileName(imagePath);
            string destinationPath = Path.Combine(userFolder, imageFileName);

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
                string dob = DOBEntry.Text;
                string aim = MotivationEntry.Text;

                SaveProfileData(name, dob, aim, _profileImagePath);

                await DisplayAlert("Success", "Profile saved successfully!", "OK");
                await Navigation.PushAsync(new HomePage());
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
                string.IsNullOrWhiteSpace(DOBEntry.Text) ||
                string.IsNullOrWhiteSpace(MotivationEntry.Text))
            {
                DisplayAlert("Error", "Please fill in all fields.", "OK");
                return false;
            }

            if (!DateTime.TryParseExact(DOBEntry.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _))
            {
                DisplayAlert("Error", "Invalid Date of Birth format.", "OK");
                return false;
            }

            return true;
        }
    }

    public class Profile
    {
        public required string Name { get; set; }
        public required string DateOfBirth { get; set; }
        public required string Motivation { get; set; }
        public required string ImagePath { get; set; }
    }
}
