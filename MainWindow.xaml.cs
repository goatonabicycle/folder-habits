using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;

namespace FolderHabits
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Habit> _habits = new ObservableCollection<Habit>();
        private const string SaveFileName = "habits.json";
        private DispatcherTimer _refreshTimer;

        public MainWindow()
        {
            InitializeComponent();
            HabitsListView.ItemsSource = _habits;
            LoadHabits();

            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10)
            };
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();
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

            var habit = new Habit
            {
                Title = title,
                FolderPath = folderPath
            };

            habit.UpdateCounts();
            _habits.Add(habit);

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
                FileName = "Select Folder",
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
                        habit.UpdateCounts();
                        _habits.Add(habit);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to load habits: {ex.Message}");
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshAllCounts();
        }
    }
}