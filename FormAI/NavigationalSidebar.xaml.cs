using System;
using System.Windows;
using System.Windows.Controls;

namespace FormAI
{
    public partial class NavigationSidebar : UserControl
    {
        public event EventHandler<string>? NavigationRequested;

        public NavigationSidebar()
        {
            InitializeComponent(); // required when the control has a XAML file
        }

        private void CalendarButton_Click(object sender, RoutedEventArgs e) => NavigateTo("Calendar");
        private void SettingsButton_Click(object sender, RoutedEventArgs e) => NavigateTo("Preferences");
        private void ProfileButton_Click(object sender, RoutedEventArgs e)  => NavigateTo("Profile");
        private void MealsButton_Click(object sender, RoutedEventArgs e)    => NavigateTo("Meals");
        private void ExerciseButton_Click(object sender, RoutedEventArgs e) => NavigateTo("Exercise");
        private void LogOutButton_Click(object sender, RoutedEventArgs e) => NavigateTo("LogOut");
       

        private void NavigateTo(string destination)
        {
            // from stackoverflow. 
            NavigationRequested?.Invoke(this, destination);
            
            
            Window? window = destination switch
            {
                "Calendar" => new CalendarPage(),
                "Preferences" => new PreferencePage(),
                "Profile"  => new ProfilePage(),
                //"Meals"    => new MealsPage(),
                //"Exercise" => new ExercisePage(),
                "LogOut" => new MainWindow(),
                _ => null
            };

            if (window != null)
            {
                
                var owner = Window.GetWindow(this);
                if (owner != null)
                {
                    window.WindowState = owner.WindowState;
                    window.Show();
                    owner.Close();
                }
                else
                {
                    window.Show();
                }
            }
            
        }

        
    }
}