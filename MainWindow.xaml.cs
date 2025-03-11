using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace FolderHabits
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Habit> _habits = new ObservableCollection<Habit>();
        private const string SaveFileName = "habits.json";

        public MainWindow()
        {
            InitializeComponent();
            HabitsListView.ItemsSource = _habits;
            LoadHabits();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleTextBox.Text.Trim();
            string folderPath = FolderTextBox.Text.Trim();

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(folderPath))
            {
                System.Windows.MessageBox.Show("Please enter both a title and folder path.");
                return;
            }

            if (!Directory.Exists(folderPath))
            {
                System.Windows.MessageBox.Show("The specified folder does not exist.");
                return;
            }

            _habits.Add(new Habit
            {
                Title = title,
                FolderPath = folderPath
            });

            TitleTextBox.Text = string.Empty;
            FolderTextBox.Text = string.Empty;

            SaveHabits();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {          
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select Folder to Monitor",
                CheckFileExists = false,
                FileName = "Select Folder", // Trick to use OpenFileDialog for folders
                CheckPathExists = true
            };

            bool? result = dialog.ShowDialog();
            if (result == true)
            {        
                string folderPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                if (!string.IsNullOrEmpty(folderPath))
                {
                    FolderTextBox.Text = folderPath;
                }
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
                System.Windows.MessageBox.Show($"Failed to save habits: {ex.Message}");
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
                        _habits.Add(habit);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to load habits: {ex.Message}");
            }
        }
    }
}