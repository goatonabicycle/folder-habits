using System;
using System.Collections.Generic;

namespace FolderHabits.Models
{
    public class HabitConfiguration
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string FolderPath { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public List<string> FileExtensionsToTrack { get; set; } = new List<string>();
    }
}