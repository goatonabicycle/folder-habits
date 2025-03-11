using System;
using System.IO;
using System.Text.Json.Serialization;

namespace FolderHabits
{
    public class Habit
    {
        public string Title { get; set; } = string.Empty;
        public string FolderPath { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonIgnore]
        public int FileCount { get; set; }

        [JsonIgnore]
        public int FolderCount { get; set; }

        [JsonIgnore]
        public string DisplayPath
        {
            get
            {
                if (string.IsNullOrEmpty(FolderPath))
                    return string.Empty;

                return FolderPath.Length > 40
                    ? $"...{FolderPath.Substring(FolderPath.Length - 40)}"
                    : FolderPath;
            }
        }

        public void UpdateCounts()
        {
            if (!Directory.Exists(FolderPath))
            {
                FileCount = 0;
                FolderCount = 0;
                return;
            }

            try
            {
                string[] files = Directory.GetFiles(FolderPath);
                string[] folders = Directory.GetDirectories(FolderPath);

                FileCount = files.Length;
                FolderCount = folders.Length;
            }
            catch (Exception)
            {
                FileCount = 0;
                FolderCount = 0;
            }
        }
    }
}