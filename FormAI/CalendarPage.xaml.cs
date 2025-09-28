using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FormAI
{
    public partial class CalendarPage : BaseWindow
    {
        private List<CalendarDay> _calendarDays = new();
        private CalendarDay? _selectedDay;

        public CalendarPage()
        {
            InitializeComponent();
            InitializeCalendar();
        }

        // Sets up the 31-day calendar view, marking days with existing meal plans.
        private void InitializeCalendar()
        {
            DateTime startDate = DateTime.Today;
            var existingMealPlanDates = LoadExistingMealPlanDates(AppSession.CurrentUser.Email);
            DayOfWeek startOfWeek = DayOfWeek.Monday;
            int emptyCells = ((int)startDate.DayOfWeek - (int)startOfWeek + 7) % 7;

            var calendarSource = new List<object>();

            for (int i = 0; i < emptyCells; i++)
            {
                calendarSource.Add(null);
            }

            for (int i = 0; i < 31; i++)
            {
                var dayDate = startDate.AddDays(i);
                var calendarDay = new CalendarDay(dayDate);

                if (existingMealPlanDates.Contains(dayDate.Date))
                {
                    calendarDay.HasMealPlan = true;
                }
                _calendarDays.Add(calendarDay);
                calendarSource.Add(calendarDay);
            }

            CalendarItemsControl.ItemsSource = calendarSource;
        }

        // Scans the user's documents for existing meal plan files to identify dates.
        private HashSet<DateTime> LoadExistingMealPlanDates(string userEmail)
        {
            var existingDates = new HashSet<DateTime>();
            try
            {
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string targetDir = Path.Combine(documentsPath, "FormAI Meal Plans");

                if (!Directory.Exists(targetDir))
                {
                    return existingDates;
                }

                string userKey = SimpleUserKey(userEmail);
                string[] mealPlanFiles = Directory.GetFiles(targetDir, $"MealPlan_*_{userKey}.md");

                foreach (var filePath in mealPlanFiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    const string prefix = "MealPlan_";

                    if (!fileName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string remainder = fileName.Substring(prefix.Length);
                    int underscoreIndex = remainder.IndexOf('_');

                    string dateString = (underscoreIndex > 0)
                        ? remainder.Substring(0, underscoreIndex)
                        : remainder;

                    if (DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                    {
                        existingDates.Add(date.Date);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading existing meal plans: {ex.Message}", "File Read Error");
            }

            return existingDates;
        }

        // Handles clicks on a calendar day, showing the action panel for the selected date.
        private void DayButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: DateTime selectedDate })
            {
                _selectedDay = _calendarDays.FirstOrDefault(day => day.Date.Date == selectedDate.Date);
                if (_selectedDay == null) return;

                SelectedDateText.Text = _selectedDay.Date.ToString("dddd, MMMM dd");
                SelectedDatePanel.Visibility = Visibility.Visible;
            }
        }

        // Hides the action panel when the user clicks the background.
        private void Background_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectedDatePanel.Visibility = Visibility.Collapsed;
            _selectedDay = null;
        }

        // Prevents the panel from closing when the user clicks directly on it.
        private void SelectedDatePanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        // Hides the action panel when the Escape key is pressed.
        private void CalendarPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                SelectedDatePanel.Visibility = Visibility.Collapsed;
                _selectedDay = null;
            }
        }

        // Initiates the meal plan generation for the single selected day.
        private async void GenerateForDay_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button) button.Content = "Generating...";
            
            await GenerateMealPlansAsync(1);
            
            if (sender is Button buttonAfter) buttonAfter.Content = "Generate for Day";

            SelectedDatePanel.Visibility = Visibility.Collapsed;
            _selectedDay = null;
        }

        // Initiates meal plan generation for the remainder of the week from the selected day.
        private async void GenerateForWeek_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button) button.Content = "Generating...";
            
            int daysUntilSunday = ((int)DayOfWeek.Sunday - (int)_selectedDay.Date.DayOfWeek + 7) % 7 + 1;
            await GenerateMealPlansAsync(daysUntilSunday);
            
            if (sender is Button buttonAfter) buttonAfter.Content = "Generate for Week";

            SelectedDatePanel.Visibility = Visibility.Collapsed;
            _selectedDay = null;
        }

        // Calls the AI service to generate meal plans and saves the results to files.
        private async Task GenerateMealPlansAsync(int numberOfMealPlans)
        {
            try
            {
                string markdown = await MealGeneration.GenerateMealPlansAsync(AppSession.CurrentUser, numberOfMealPlans);
                if (markdown.StartsWith("An error occurred"))
                {
                    MessageBox.Show(markdown, "AI Service Error");
                    return;
                }

                string[] dailyPlans = markdown.Split(new[] { "--- ---" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < dailyPlans.Length; i++)
                {
                    DateTime currentDate = _selectedDay.Date.AddDays(i);
                    string singleDayMarkdown = dailyPlans[i].Trim();
                    var dayForFileName = _calendarDays.FirstOrDefault(d => d.Date.Date == currentDate.Date);
                    
                    if (dayForFileName != null)
                    {
                        string fileName = BuildMealPlanFileName(dayForFileName);
                        Functions.SaveFile(singleDayMarkdown, fileName, "md");
                        dayForFileName.HasMealPlan = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"A critical error occurred:\n{ex.Message}", "Error");
            }
        }

        // Creates a short, consistent user key from an email for file naming.
        private static string SimpleUserKey(string email)
        {
            using var sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(email.Trim().ToLowerInvariant()));
            return Convert.ToHexString(hash).Substring(0, 8).ToLowerInvariant();
        }

        // Constructs the standardized file name for a meal plan based on the date and user.
        private static string BuildMealPlanFileName(CalendarDay? selectedDay)
        {
            var key = SimpleUserKey(AppSession.CurrentUser.Email);
            var date = (selectedDay?.Date ?? DateTime.Today).Date;
            var stamp = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            return $"MealPlan_{stamp}_{key}";
        }

        // Opens the meal plan viewer for a selected day if a plan exists.
        private void ViewMealPlan_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDay == null)
            {
                MessageBox.Show("Please select a day first.", "Information");
                return;
            }

            if (_selectedDay.HasMealPlan)
            {
                string fileName = BuildMealPlanFileName(_selectedDay);
                string markdownContent = Functions.ReadFile(fileName, "md", "FormAI Meal Plans");

                var viewerWindow = new MealPlanPage($"Meal Plan for {_selectedDay.Date:MMMM dd}", markdownContent);
                viewerWindow.WindowState = WindowState;
                viewerWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("No meal plan has been generated for this day.", "Information");
            }
        }
    }
}