using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace FormAI;

public partial class PreferencePage : BaseWindow
{
    public PreferencePage()
    {
        
        InitializeComponent();
        DataContext = AppSession.CurrentUser;
    }

    // Handles saving the user's preferences to the JSON file.
private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string folderPath;
            folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string fileName;
            fileName = "FormAI_UserData.json";

            string fullPath;
            fullPath = Path.Combine(folderPath, fileName);

            bool doesFileExist;
            doesFileExist = File.Exists(fullPath);

            if (doesFileExist == false)
            {
                MessageBox.Show("User data file not found. Cannot save.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string jsonStringFromFile;
            jsonStringFromFile = File.ReadAllText(fullPath);

            List<User> allUsers;
            allUsers = JsonSerializer.Deserialize<List<User>>(jsonStringFromFile);

            int targetUserIndex = -1;

            User userToFind = AppSession.CurrentUser;

            string emailToMatch = userToFind.Email;

            for (int i = 0; i < allUsers.Count; i = i + 1)
            {
                User userInList = allUsers[i];

                if (userInList.Email.Equals(emailToMatch, StringComparison.OrdinalIgnoreCase))
                {
                    targetUserIndex = i;
                    break;
                }
            }

            bool wasUserFound = targetUserIndex != -1;
            
            if (wasUserFound)
            {
                allUsers[targetUserIndex] = AppSession.CurrentUser;
            }
            else
            {
                MessageBox.Show("Could not find current user in the data file. Cannot save.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            JsonSerializerOptions formattingOptions = new JsonSerializerOptions();
            
            formattingOptions.WriteIndented = true;

            string updatedJsonString;
            updatedJsonString = JsonSerializer.Serialize(allUsers, formattingOptions);

            File.WriteAllText(fullPath, updatedJsonString);
            
            MessageBox.Show("Preferences saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            string baseMessage = "An error occurred while saving:";
            string exceptionMessage = ex.Message;
            string fullErrorMessage = baseMessage + " " + exceptionMessage;

            MessageBox.Show(fullErrorMessage, "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}