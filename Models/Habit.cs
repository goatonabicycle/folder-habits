using System;
using System.Text.Json.Serialization;

namespace FolderHabits
{
    public class Habit
    {
        public string Title { get; set; } = string.Empty;
        public string FolderPath { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

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
    }
}