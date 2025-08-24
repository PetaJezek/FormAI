using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FormAI
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer moleculeTimer;
        private Random random = new Random();
        private List<Ellipse> activeMolecules = new List<Ellipse>();

        
        
        private void InitializeMoleculeAnimation()
        {
            // Timer to create new molecules periodically
            moleculeTimer = new DispatcherTimer();
            moleculeTimer.Interval = TimeSpan.FromMilliseconds(800); // Create molecule every 800ms
            moleculeTimer.Tick += CreateMolecule;
            moleculeTimer.Start();
            
            // Create initial molecules
            for (int i = 0; i < 15; i++)
            {
                CreateMolecule(null, null);
            }
        }
        private void Root_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                // Optional: double-click to toggle maximize/restore
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

        
        private void CreateMolecule(object sender, EventArgs e)
        {
            // Remove old molecules to prevent memory buildup
            if (activeMolecules.Count > 40)
            {
                var oldMolecule = activeMolecules[0];
                MoleculesCanvas.Children.Remove(oldMolecule);
                activeMolecules.RemoveAt(0);
            }
            
            // Create new molecule
            var molecule = new Ellipse();
            
            // Randomly choose molecule type (Protein, Carb, or Fat)
            string[] moleculeTypes = { "Protein", "Carb", "Fat" };
            string moleculeType = moleculeTypes[random.Next(moleculeTypes.Length)];
            
            // Apply appropriate style
            switch (moleculeType)
            {
                case "Protein":
                    molecule.Style = (Style)FindResource("ProteinMolecule");
                    break;
                case "Carb":
                    molecule.Style = (Style)FindResource("CarbMolecule");
                    break;
                case "Fat":
                    molecule.Style = (Style)FindResource("FatMolecule");
                    break;
            }
            
            // Position molecule randomly at bottom of screen
            double startX = random.NextDouble() * ActualWidth;
            double startY = ActualHeight + 20; // Start just below visible area
            
            Canvas.SetLeft(molecule, startX);
            Canvas.SetTop(molecule, startY);
            
            // Add to canvas
            MoleculesCanvas.Children.Add(molecule);
            activeMolecules.Add(molecule);
            
            // Create floating animation
            AnimateMolecule(molecule);
        }
        
        private void AnimateMolecule(Ellipse molecule)
        {
            // Create floating animation
            var storyboard = new Storyboard();
            
            // Y-axis movement (floating up)
            var translateYAnimation = new DoubleAnimation();
            translateYAnimation.From = Canvas.GetTop(molecule);
            translateYAnimation.To = -50; // End above visible area
            translateYAnimation.Duration = TimeSpan.FromSeconds(random.NextDouble() * 8 + 10); // 10-18 seconds
            translateYAnimation.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };
            
            // X-axis movement (slight drift)
            var translateXAnimation = new DoubleAnimation();
            translateXAnimation.From = Canvas.GetLeft(molecule);
            translateXAnimation.To = Canvas.GetLeft(molecule) + (random.NextDouble() * 200 - 100); // Drift ±100 pixels
            translateXAnimation.Duration = translateYAnimation.Duration;
            translateXAnimation.EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut };
            
            // Rotation animation
            var rotateTransform = new RotateTransform();
            molecule.RenderTransform = rotateTransform;
            molecule.RenderTransformOrigin = new Point(0.5, 0.5);
            
            var rotationAnimation = new DoubleAnimation();
            rotationAnimation.From = 0;
            rotationAnimation.To = 360;
            rotationAnimation.Duration = TimeSpan.FromSeconds(random.NextDouble() * 15 + 10); // 10-25 seconds
            rotationAnimation.RepeatBehavior = RepeatBehavior.Forever;
            
            // Opacity animation (fade in/out)
            var opacityAnimation = new DoubleAnimationUsingKeyFrames
            {
                Duration = translateYAnimation.Duration,
                KeyFrames =
                {
                    // At 0 seconds, opacity is 0 (fade in)
                    new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))),
        
                    // At 1 second, opacity is 0.8
                    new LinearDoubleKeyFrame(0.8, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1))),
        
                    // Hold at 0.8 until 1 second before the end
                    new LinearDoubleKeyFrame(0.8, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(translateYAnimation.Duration.TimeSpan.TotalSeconds - 1))),
        
                    // At the very end, fade out to 0
                    new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(translateYAnimation.Duration.TimeSpan))
                }
            };
            opacityAnimation.Duration = translateYAnimation.Duration;
            
            
            
            // Scale animation (subtle pulsing)
            var scaleTransform = new ScaleTransform();
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(rotateTransform);
            molecule.RenderTransform = transformGroup;
            // Scale animation (subtle pulsing) - Compact Version
            var scaleAnimation = new DoubleAnimationUsingKeyFrames
            {
                Duration = TimeSpan.FromSeconds(9),
                RepeatBehavior = RepeatBehavior.Forever,
                KeyFrames =
                {
                    new EasingDoubleKeyFrame(1,   KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))),
                    new EasingDoubleKeyFrame(1.2, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(3))),
                    new EasingDoubleKeyFrame(0.8, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(6))),
                    new EasingDoubleKeyFrame(1,   KeyTime.FromTimeSpan(TimeSpan.FromSeconds(9)))
                }
            };
            scaleAnimation.Duration = TimeSpan.FromSeconds(9);
            scaleAnimation.RepeatBehavior = RepeatBehavior.Forever;
            
            // Set up animation targets
            Storyboard.SetTarget(translateYAnimation, molecule);
            Storyboard.SetTargetProperty(translateYAnimation, new PropertyPath("(Canvas.Top)"));
            
            Storyboard.SetTarget(translateXAnimation, molecule);
            Storyboard.SetTargetProperty(translateXAnimation, new PropertyPath("(Canvas.Left)"));
            
            Storyboard.SetTarget(rotationAnimation, molecule);
            Storyboard.SetTargetProperty(rotationAnimation, new PropertyPath("RenderTransform.Children[1].Angle"));
            
            Storyboard.SetTarget(opacityAnimation, molecule);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
            
            Storyboard.SetTarget(scaleAnimation, molecule);
            Storyboard.SetTargetProperty(scaleAnimation, new PropertyPath("RenderTransform.Children[0].ScaleX"));
            
            var scaleYAnimation = scaleAnimation.Clone();
            Storyboard.SetTarget(scaleYAnimation, molecule);
            Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("RenderTransform.Children[0].ScaleY"));
            
            // Add animations to the storyboard
            storyboard.Children.Add(translateYAnimation);
            storyboard.Children.Add(translateXAnimation);
            storyboard.Children.Add(rotationAnimation);
            storyboard.Children.Add(opacityAnimation);
            storyboard.Children.Add(scaleAnimation);
            storyboard.Children.Add(scaleYAnimation);
            
            // Start animation
            storyboard.Begin();
        }
        
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // Handle login/guest selection logic here
            
            // Check if user is logged in (you'll implement this logic)
            bool isLoggedIn = false; // Replace with actual login check
            
            if (!isLoggedIn)
            {
                // Change button to show login options
                ShowLoginOptions();
            }
            else
            {
                // User is logged in, proceed to main app
                ProceedToMainApp();
            }
        }
        
        private void ShowLoginOptions()
        {
            // Create a simple popup or change the UI to show login/guest options
            var result = MessageBox.Show("Would you like to log in or continue as a guest?", 
                                       "Welcome to FormAI", 
                                       MessageBoxButton.YesNoCancel, 
                                       MessageBoxImage.Question);
            
            switch (result)
            {
                case MessageBoxResult.Yes:
                    // Show login form (you'll implement this)
                    ShowLoginForm();
                    break;
                case MessageBoxResult.No:
                    // Continue as guest
                    ContinueAsGuest();
                    break;
                case MessageBoxResult.Cancel:
                    // Do nothing, stay on welcome screen
                    break;
            }
        }
        
        private void ShowLoginForm()
        {
            // TODO: Implement login form
            MessageBox.Show("Login form would be shown here");
            // After successful login, call ProceedToMainApp()
        }
        
        private void ContinueAsGuest()
        {
            // TODO: Set guest mode
            MessageBox.Show("Continuing as guest...");
            ProceedToMainApp();
        }
        
        private void ProceedToMainApp()
        {
            // TODO: Navigate to main application window
            MessageBox.Show("Proceeding to main FormAI application...");
            
            // Example: Open main window and close this one
            // var mainAppWindow = new MainAppWindow();
            // mainAppWindow.Show();
            // this.Close();
        }
        
        protected override void OnClosed(EventArgs e)
        {
            moleculeTimer?.Stop();
            base.OnClosed(e);
        }
    }
}