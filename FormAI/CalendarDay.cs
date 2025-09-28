using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace FormAI
{
    // A simple class to hold a day's date and whether it has a meal plan.'
    public class CalendarDay : INotifyPropertyChanged
    {
        public DateTime Date { get; }
        public string DayOfMonth => Date.ToString("dd", CultureInfo.CurrentCulture);
        public string DayOfWeek => Date.ToString("ddd", CultureInfo.CurrentCulture);

        private bool _hasMealPlan;
        public bool HasMealPlan
        {
            get => _hasMealPlan;
            set
            {
                if (_hasMealPlan != value)
                {
                    _hasMealPlan = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(BackgroundColor));
                }
            }
        }

        public bool IsToday => Date.Date == DateTime.Today;

        public string BackgroundColor
        {
            get
            {
                if (HasMealPlan)
                {
                    return "#2E7D32";
                }
                if (IsToday)
                {
                    return "#6B46C1";
                }
                return "#2D3748";
            }
        }

        public CalendarDay(DateTime date)
        {
            Date = date;
            _hasMealPlan = false;
        }
        
        // From stack overflow. Property change handler, which invokes methods on change, because some buttons didnt "catch the change"
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}