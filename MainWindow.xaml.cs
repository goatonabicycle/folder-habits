using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Win32;

namespace FolderHabits
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Habit> _habits = new ObservableCollection<Habit>();

        public MainWindow()
        {
            InitializeComponent();
            HabitsListView.ItemsSource = _habits;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleTextBox.Text.Trim();
            string folderPath = FolderTextBox.Text.Trim();

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(folderPath))
            {
                MessageBox.Show("Please enter both a title and folder path.");
                return;
            }

            _habits.Add(new Habit
            {
                Title = title,
                FolderPath = folderPath
            });
            
            TitleTextBox.Text = string.Empty;
            FolderTextBox.Text = string.Empty;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select Folder to Monitor"
            };

            if (dialog.ShowDialog() == true)
            {
                FolderTextBox.Text = dialog.FolderName;
            }
        }
    }
}