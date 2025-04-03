using System.Text.Json;
using GoalQuest.Models;

namespace GoalQuest;

public partial class MainPage : ContentPage
{
    private readonly string userFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UserData");
    private readonly string filePath;

    private readonly List<string> _quotes = new List<string>
    {
        "The journey of a thousand miles begins with one step.",
        "Set your goals high, and don't stop until you get there.",
        "You are never too old to set another goal or to dream a new dream.",
        "Success is not final, failure is not fatal: It is the courage to continue that counts.",
        "Discipline is the bridge between goals and accomplishment.",
        "Be loyal to your fututre, not your past.",
        "A lot will go wrong before everything goes right.",
        "Nothing is scarier than avoiding your full potential",
        "Step out of your comfort zone, and face your fears.",
        "Doubts kill more dreams than failure ever will.",
        "The happiness of your life depends on the quality of your thoughts."
    };

    public MainPage()
    {
        InitializeComponent();
        filePath = Path.Combine(userFolder, "profile.json");
        ShowRandomQuote();
    }

    private void ShowRandomQuote()
    {
        var random = new Random();
        int index = random.Next(_quotes.Count);
        QuoteLabel.Text = _quotes[index];
    }

    private async void StartButton_Clicked(object sender, EventArgs e)
    {
        if (File.Exists(filePath))
        {
            var data = await File.ReadAllTextAsync(filePath);
            var profile = JsonSerializer.Deserialize<Profile>(data);

            if (profile != null)
            {
                await Navigation.PushAsync(new WelcomePage(profile));
            }
            else
            {
                StartButton.IsVisible = true;
            }
        }
        else
        {
            StartButton.IsVisible = true;
            await Navigation.PushAsync(new UserProfilePage());
        }
    }

}
