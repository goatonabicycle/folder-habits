using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public List<ActivityDay> ActivityDays { get; set; } = new List<ActivityDay>();

        public void UpdateCounts()
        {
            if (!Directory.Exists(FolderPath))
            {
                FileCount = 0;
                FolderCount = 0;
                ActivityDays.Clear();
                return;
            }

            try
            {                
                string[] files = Directory.GetFiles(FolderPath);
                string[] folders = Directory.GetDirectories(FolderPath);

                FileCount = files.Length;
                FolderCount = folders.Length;
             
                DateTime today = DateTime.Today;
                ActivityDays.Clear();
              
                var allPaths = new List<string>();
                allPaths.Add(FolderPath); 
                allPaths.AddRange(files);
                
                for (int i = 9; i >= 0; i--)
                {
                    DateTime date = today.AddDays(-i);
                    bool hasActivity = false;

                    foreach (var path in allPaths)
                    {
                        DateTime lastModified = File.GetLastWriteTime(path);

                        if (lastModified.Date == date)
                        {
                            hasActivity = true;
                            break;
                        }
                    }

                    ActivityDays.Add(new ActivityDay
                    {
                        Date = date,
                        HasActivity = hasActivity
                    });
                }
            }
            catch (Exception)
            {
                FileCount = 0;
                FolderCount = 0;
                ActivityDays.Clear();
            }
        }
    }
}