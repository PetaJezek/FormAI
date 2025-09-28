using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace FormAI;

public struct Functions
{
    const string FolderName = "MarkdownMealPlans";

    public static string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
    public static string SaveFile(string fileContent, string fileName, string extension)
    {
        if (string.IsNullOrWhiteSpace(fileContent))
        {
            return "Error: Content was empty, file not saved.";
        }

        try
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            
            string targetDir = Path.Combine(documentsPath, "FormAI Meal Plans");

            Directory.CreateDirectory(targetDir);

            string fullFileName = $"{fileName}.{extension}";
            string filePath = Path.Combine(targetDir, fullFileName);

            File.WriteAllText(filePath, fileContent);
        
            return filePath;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to save the file. Error: {ex.Message}", "File Save Error");
            return $"Error: {ex.Message}";
        }
    }
    
    public static string ReadFile(string fileName, string extension, string folderName)
    {
        try
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string targetDir = Path.Combine(documentsPath, folderName);

            
            string filePath = Path.Combine(targetDir, $"{fileName}.{extension}");

            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            else
            {
                return $"Error: The file '{fileName}.{extension}' was not found in '{folderName}'.";
            }
        }
        catch (Exception ex)
        {
            return $"A critical error occurred while reading the file: {ex.Message}";
        }
    }
    


}