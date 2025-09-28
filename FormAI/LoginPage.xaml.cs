using System.Windows;
using System.Windows.Input;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Windows.Shapes;
using System.IO;
using System;
using Path = System.IO.Path;
using static FormAI.Functions;


namespace FormAI;

public partial class LoginPage : BaseWindow
{
    public LoginPage()
    {
        InitializeComponent();
    }
    

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
       

        if (string.IsNullOrWhiteSpace(EmailBox.Text))
        {
            MessageBox.Show("Please enter your email.", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(PasswordBox.Password))
        {
            MessageBox.Show("Please enter a password.", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }


        string email = EmailBox.Text;
        string password = PasswordBox.Password;

        try
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FormAI_UserData.json");
            List<User> users = new List<User>();

            if (!File.Exists(filePath))
            {
                MessageBox.Show("No user data found. Please register first.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string jsonString = File.ReadAllText(filePath);

            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                users = JsonSerializer.Deserialize<List<User>>(jsonString);
            }

            if (users.Any(user => user.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                // if found a match in the list, check if password is correct

                string hashedPassword = HashPassword(password);

                if (hashedPassword == users.First(user => user.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                        .PasswordHash)
                {
                    AppSession.CurrentUser = users.First(user => user.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
                    
                    // Check if user created their bio or not?

                    if (!AppSession.CurrentUser.HasSubmitedBioInfo)
                    {
                        var profilePage = new ProfilePage();
                        profilePage.WindowState = this.WindowState;
                        profilePage.Show();
                        this.Close();
                        return;
                    }
                    var calendarPage = new CalendarPage();
                    calendarPage.WindowState = this.WindowState;
                    calendarPage.Show();
                    this.Close();
                    


                }
                else
                {
                    MessageBox.Show("Incorrect password.", "Login Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


            }
            else
            {
                MessageBox.Show("No user found with that email address.", "Login Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred while logging in: {ex.Message}",
                "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BackToRegisterButton_Click(object sender, MouseButtonEventArgs e)
    {
        NavigateToMain();
    }

    private void NavigateToMain()
    {
        var mainWindow = new MainWindow();
        mainWindow.WindowState = this.WindowState;
        mainWindow.Show();
        this.Close();
    }
}