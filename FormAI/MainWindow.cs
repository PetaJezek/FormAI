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
        private DispatcherTimer bubbleTimer;
        private Random random = new Random();
        private List<Ellipse> activeBubbles = new List<Ellipse>();



        private void InitializeBubbleAnimation()
        {
            // Timer to create new bubbles periodically
            bubbleTimer = new DispatcherTimer();
            bubbleTimer.Interval = TimeSpan.FromMilliseconds(800); // Create bubble every 800ms
            bubbleTimer.Tick += CreateBubble;
            bubbleTimer.Start();
            
            // Create initial bubbles
            for (int i = 0; i < 15; i++)
            {
                CreateBubble(null, null);
            }
        }
        
        private void CreateBubble(object sender, EventArgs e)
        {
            // Remove old bubbles to prevent memory buildup
            if (activeBubbles.Count > 40)
            {
                var oldBubble = activeBubbles[0];
                BubblesCanvas.Children.Remove(oldBubble);
                activeBubbles.RemoveAt(0);
            }
            
            // Create new bubble
            var bubble = new Ellipse();
            bubble.Style = (Style)FindResource("BubbleStyle");
            
            // Vary bubble sizes slightly for more visual interest
            double size = 8 + random.NextDouble() * 8; // Size between 8-16
            bubble.Width = size;
            bubble.Height = size;
            
            // Position bubble randomly at bottom of screen
            double startX = random.NextDouble() * ActualWidth;
            double startY = ActualHeight + 20; // Start just below visible area
            
            Canvas.SetLeft(bubble, startX);
            Canvas.SetTop(bubble, startY);
            
            // Add to canvas
            BubblesCanvas.Children.Add(bubble);
            activeBubbles.Add(bubble);
            
            // Create floating animation
            AnimateBubble(bubble);
        }

        private void AnimateBubble(Ellipse bubble)
        {
            // Create floating animation
            var storyboard = new Storyboard();
            
            // Y-axis movement (floating up)
            var translateYAnimation = new DoubleAnimation();
            translateYAnimation.From = Canvas.GetTop(bubble);
            translateYAnimation.To = -50; // End above visible area
            translateYAnimation.Duration = TimeSpan.FromSeconds(random.NextDouble() * 8 + 10); // 10-18 seconds
            translateYAnimation.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };
            
            // X-axis movement (slight drift)
            var translateXAnimation = new DoubleAnimation();
            translateXAnimation.From = Canvas.GetLeft(bubble);
            translateXAnimation.To = Canvas.GetLeft(bubble) + (random.NextDouble() * 200 - 100); // Drift ±100 pixels
            translateXAnimation.Duration = translateYAnimation.Duration;
            translateXAnimation.EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut };
            
            // Rotation animation
            var rotateTransform = new RotateTransform();
            bubble.RenderTransform = rotateTransform;
            bubble.RenderTransformOrigin = new Point(0.5, 0.5);
            
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
            bubble.RenderTransform = transformGroup;
            
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
            Storyboard.SetTarget(translateYAnimation, bubble);
            Storyboard.SetTargetProperty(translateYAnimation, new PropertyPath("(Canvas.Top)"));
            
            Storyboard.SetTarget(translateXAnimation, bubble);
            Storyboard.SetTargetProperty(translateXAnimation, new PropertyPath("(Canvas.Left)"));
            
            Storyboard.SetTarget(rotationAnimation, bubble);
            Storyboard.SetTargetProperty(rotationAnimation, new PropertyPath("RenderTransform.Children[1].Angle"));
            
            Storyboard.SetTarget(opacityAnimation, bubble);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
            
            Storyboard.SetTarget(scaleAnimation, bubble);
            Storyboard.SetTargetProperty(scaleAnimation, new PropertyPath("RenderTransform.Children[0].ScaleX"));
            
            var scaleYAnimation = scaleAnimation.Clone();
            Storyboard.SetTarget(scaleYAnimation, bubble);
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
        
        // Buttons Functions
        
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // Create smooth transition animation
            var fadeOutStoryboard = new Storyboard();
            var fadeInStoryboard = new Storyboard();

            // Fade out the start button
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

            // When fade out completes, hide start button and show auth buttons
            fadeOutStoryboard.Completed += (s, args) =>
            {
                StartButton.Visibility = Visibility.Collapsed;
                AuthButtonsPanel.Visibility = Visibility.Visible;
                AuthButtonsPanel.Opacity = 0; // Start invisible

                // Fade in the auth buttons
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

                // Add slide-up animation for auth buttons
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

            // Start the fade out animation
            fadeOutStoryboard.Begin();
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

        protected override void OnStateChanged(EventArgs e)
        {
            UpdateMaximizeButton();
            base.OnStateChanged(e);
        }

        private void UpdateMaximizeButton()
        {
            if (MaximizeButton != null)
            {
                MaximizeButton.Content = WindowState == WindowState.Maximized ? "\uE923" : "\uE922";
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            bubbleTimer?.Stop();
            base.OnClosed(e);
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount == 2)
                {
                    WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                    return;
                }

                DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Open the register page
            var registerPage = new RegisterPage();
            registerPage.Show();
            this.Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Open your login flow here
            MessageBox.Show("Login clicked.");
        }
    }
}
    
