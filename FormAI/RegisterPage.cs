using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace FormAI;

public partial class RegisterPage : Window
{
    private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
            DragMove();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        // TODO: Add your registration logic here
        
            
        // Your code goes here
    }

    private void BackToLoginButton_Click(object sender, RoutedEventArgs e)
    {
        // TODO: Navigate back to login or main window
        Close();
        
    }
}