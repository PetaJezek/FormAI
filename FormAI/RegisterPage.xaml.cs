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

public partial class RegisterPage : BaseWindow
{
    public RegisterPage()
    {
        InitializeComponent();
    }
    

    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        
        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FormAI_UserData.json");
        
        List<User> users = new List<User>();

        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);
            
            // if not empty
            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                users = JsonSerializer.Deserialize<List<User>>(jsonString);
            }
        }
        
        
        
        
        // Validate input
        if (string.IsNullOrWhiteSpace(FullNameBox.Text))
        {
            MessageBox.Show("Please enter your full name.", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

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

        if (PasswordBox.Password != ConfirmPasswordBox.Password)
        {
            MessageBox.Show("Passwords do not match.", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        
        //check if the user exists
        if (users.Any(user => user.Email.Equals(EmailBox.Text, StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show("This email address is already registered.", "Registration Error",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        // create the new user
        
        
        var user = new User()
        {
            Name = FullNameBox.Text,
            Email = EmailBox.Text,
            PasswordHash = HashPassword(PasswordBox.Password),
            HasSubmitedBioInfo = false
        };

        try
        {
            users.Add(user);
            // pretty json 
            
            var options = new JsonSerializerOptions { WriteIndented = true };
            string updatedJsonString = JsonSerializer.Serialize(users, options);
            
            File.WriteAllText(filePath, updatedJsonString);
            MessageBox.Show($"Registration successful for {FullNameBox.Text}! Please login using your credentials.",
                "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred while registering: {ex.Message}",
                "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        // Navigate back to main window
        NavigateToMain();
    }

    private void BackToLoginButton_Click(object sender, MouseButtonEventArgs e)
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