using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;

namespace FolderHabits
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Habit> _habits = new ObservableCollection<Habit>();
        private const string SaveFileName = "habits.json";
        private DispatcherTimer _refreshTimer;
        private Habit _currentHabit;
        private DateTime _selectedDate;

        public MainWindow()
        {
            InitializeComponent();

            HabitsListView.ItemsSource = _habits;
            LoadHabits();

            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();
            
            ChangesTitle.Text = "No day selected";
            ChangesListBox.ItemsSource = new List<string> { "Click on a day to view changes" };
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            RefreshAllCounts();
        }

        private void RefreshAllCounts()
        {
            foreach (var habit in _habits)
            {
                habit.UpdateCounts();
            }

            HabitsListView.Items.Refresh();

            if (_currentHabit != null)
            {
                ShowChangesForDate(_currentHabit, _selectedDate);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleTextBox.Text.Trim();
            string folderPath = FolderTextBox.Text.Trim();
            bool monitorSubdirectories = SubdirectoriesCheckBox.IsChecked ?? true;

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(folderPath))
            {
                MessageBox.Show("Please enter both a title and folder path.", "Missing Information",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show("The specified folder does not exist.", "Invalid Folder",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var habit = new Habit
            {
                Title = title,
                FolderPath = folderPath,
                MonitorSubdirectories = monitorSubdirectories
            };

            habit.UpdateCounts();
            _habits.Add(habit);

            TitleTextBox.Text = string.Empty;
            FolderTextBox.Text = string.Empty;
            SubdirectoriesCheckBox.IsChecked = true;

            SaveHabits();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select Folder to Monitor"
            };

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                string folderPath = dialog.FolderName;
                if (!string.IsNullOrEmpty(folderPath))
                {
                    FolderTextBox.Text = folderPath;
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Habit habit)
            {
                if (MessageBox.Show(
                    $"Are you sure you want to delete '{habit.Title}'?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _habits.Remove(habit);
                    SaveHabits();
                    
                    if (habit == _currentHabit)
                    {
                        _currentHabit = null;
                        ChangesTitle.Text = "No day selected";
                        ChangesListBox.ItemsSource = new List<string> { "Click on a day to view changes" };
                    }
                }
            }
        }

        private void ActivityDay_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is DateTime date && border.DataContext is ActivityDay activityDay)
            {                
                foreach (var habit in _habits)
                {
                    if (habit.ActivityDays.Contains(activityDay))
                    {
                        _currentHabit = habit;
                        _selectedDate = date;
                        ShowChangesForDate(habit, date);
                        break;
                    }
                }
            }
        }

        private void ShowChangesForDate(Habit habit, DateTime date)
        {
            var changes = habit.GetChangedFilesForDate(date);

            if (changes.Count > 0)
            {
                ChangesTitle.Text = $"Changes for {habit.Title} on {date:dd MMM yyyy}";
                
                var formattedChanges = new List<string>();
                foreach (var path in changes)
                {
                    string displayPath = path;

                    if (path.StartsWith(habit.FolderPath))
                    {
                        displayPath = path.Substring(habit.FolderPath.Length).TrimStart('\\', '/');
                        if (string.IsNullOrEmpty(displayPath))
                        {
                            displayPath = "(Root folder)";
                        }
                    }

                    formattedChanges.Add(displayPath);
                }

                ChangesListBox.ItemsSource = formattedChanges;
            }
            else
            {
                ChangesTitle.Text = $"No changes for {habit.Title} on {date:dd MMM yyyy}";
                ChangesListBox.ItemsSource = new List<string> { "No files were modified on this day" };
            }
        }

        private void SaveHabits()
        {
            try
            {
                string json = JsonSerializer.Serialize(_habits, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SaveFileName, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save habits: {ex.Message}");
            }
        }

        private void LoadHabits()
        {
            if (!File.Exists(SaveFileName))
                return;

            try
            {
                string json = File.ReadAllText(SaveFileName);
                var habits = JsonSerializer.Deserialize<List<Habit>>(json);
                if (habits != null)
                {
                    _habits.Clear();
                    foreach (var habit in habits)
                    {
                        habit.UpdateCounts();
                        _habits.Add(habit);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load habits: {ex.Message}");
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshAllCounts();
        }
    }
    
    public class OpenFolderDialog
    {
        public string Title { get; set; }
        public string FolderName { get; private set; }

        public bool? ShowDialog()
        {
            var dialog = new OpenFileDialog
            {
                Title = this.Title,
                CheckFileExists = false,
                FileName = "Select Folder",
                CheckPathExists = true,
                ValidateNames = false
            };

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                FolderName = Path.GetDirectoryName(dialog.FileName);
                return true;
            }

            return false;
        }
    }
}