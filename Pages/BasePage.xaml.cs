namespace GoalQuest 
{
    public partial class BasePage : ContentPage
    {
        public BasePage()
        {
            Background = new LinearGradientBrush
            {
                GradientStops =
                {
                    new GradientStop { Color = Color.FromArgb("#FFB677"), Offset = 0.0f },
                    new GradientStop { Color = Color.FromArgb("#FF3CAC"), Offset = 1.0f }
                }
            };
        }
    }
}
