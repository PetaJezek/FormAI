using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FormAI;

public partial class MainWindow : BaseWindow
{
    public MainWindow()
    {
        InitializeComponent();
        AppSession.CurrentUser = null;
        
    }
    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        var fadeOutStoryboard = new Storyboard();
        var fadeInStoryboard = new Storyboard();

        var fadeOutAnimation = new DoubleAnimation
        {
            From = 1.0,
            To = 0.0,
            Duration = TimeSpan.FromMilliseconds(300),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
        };

        Storyboard.SetTarget(fadeOutAnimation, StartButton);
        Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath("Opacity"));
        fadeOutStoryboard.Children.Add(fadeOutAnimation);

        fadeOutStoryboard.Completed += (s, args) =>
        {
            StartButton.Visibility = Visibility.Collapsed;
            AuthButtonsPanel.Visibility = Visibility.Visible;
            AuthButtonsPanel.Opacity = 0;

            var fadeInAnimation = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(400),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            Storyboard.SetTarget(fadeInAnimation, AuthButtonsPanel);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("Opacity"));
            fadeInStoryboard.Children.Add(fadeInAnimation);

            var slideAnimation = new DoubleAnimation
            {
                From = 30,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(400),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            var translateTransform = new TranslateTransform(0, 30);
            AuthButtonsPanel.RenderTransform = translateTransform;

            Storyboard.SetTarget(slideAnimation, AuthButtonsPanel);
            Storyboard.SetTargetProperty(slideAnimation, new PropertyPath("RenderTransform.Y"));
            fadeInStoryboard.Children.Add(slideAnimation);

            fadeInStoryboard.Begin();
        };

        fadeOutStoryboard.Begin();
    }

    private void Root_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            if (e.ClickCount == 2)
            {
                WindowState = WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
                return;
            }

            DragMove();
        }
    }
    


    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        var registerPage = new RegisterPage();
        registerPage.WindowState = this.WindowState;
        registerPage.Show();
        Close();
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        var loginPage = new LoginPage();
        loginPage.WindowState = this.WindowState;
        loginPage.Show();
        Close();
    }
}