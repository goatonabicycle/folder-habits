using System;

namespace FolderHabits
{
    public class Habit
    {
        public string Title { get; set; } = string.Empty;
        public string FolderPath { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}