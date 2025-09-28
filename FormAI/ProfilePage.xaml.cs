using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FormAI;

public partial class ProfilePage : BaseWindow
{
    
    public ProfilePage()
    {
        InitializeComponent();
        //populate the comboboxes
        GenderComboBox.ItemsSource = Enum.GetValues<Gender>();
        GoalComboBox.ItemsSource = Enum.GetValues<Goal>();
        DataContext = AppSession.CurrentUser;
        LoadUserData();

    }

    private void LoadUserData()
    {
        var user = AppSession.CurrentUser;
        if (user.HasSubmitedBioInfo)
        {
            GenderComboBox.SelectedItem = user.Gender;
            GoalComboBox.SelectedItem = user.Goal;
        }
    }

    // Allow only numeric input for integer fields (From stackOverflow)
    private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !e.Text.All(char.IsDigit);
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        
        try
        {
            var user = AppSession.CurrentUser;
            user.Gender = (Gender)GenderComboBox.SelectedItem;
            user.Goal = (Goal)GoalComboBox.SelectedItem;
            user.Age = int.Parse(AgeBox.Text);
            user.Height = int.Parse(HeightBox.Text);
            user.Weight = int.Parse(WeightBox.Text); 
            user.HasSubmitedBioInfo = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Please fill out all fields correctly.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        string filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "FormAI_UserData.json");
        string jsonString = File.ReadAllText(filePath);
        var allUsers = JsonSerializer.Deserialize<List<User>>(jsonString);
        int userIndex = allUsers.FindIndex(user => user.Email.Equals(AppSession.CurrentUser.Email, StringComparison.OrdinalIgnoreCase));
        
        allUsers[userIndex] = AppSession.CurrentUser;
        string updatedJson = JsonSerializer.Serialize(allUsers, new JsonSerializerOptions { WriteIndented = true });
        
        File.WriteAllText(filePath, updatedJson);
        // rest of the code
        
        


    }
}