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
        public bool MonitorSubdirectories { get; set; } = true;

        [JsonIgnore]
        public int FileCount { get; set; }

        [JsonIgnore]
        public int FolderCount { get; set; }

        [JsonIgnore]
        public List<ActivityDay> ActivityDays { get; set; } = new List<ActivityDay>();

        [JsonIgnore]
        public Dictionary<DateTime, List<string>> ChangedFilesByDay { get; set; } = new Dictionary<DateTime, List<string>>();

        public void UpdateCounts()
        {
            if (!Directory.Exists(FolderPath))
            {
                FileCount = 0;
                FolderCount = 0;
                ActivityDays.Clear();
                ChangedFilesByDay.Clear();
                return;
            }

            try
            {                
                var allFiles = GetFiles(FolderPath, MonitorSubdirectories);
                var allFolders = GetFolders(FolderPath, MonitorSubdirectories);

                FileCount = allFiles.Count;
                FolderCount = allFolders.Count;

                DateTime today = DateTime.Today;
                ActivityDays.Clear();
                ChangedFilesByDay.Clear();

                var allPaths = new List<string>();
                allPaths.Add(FolderPath);
                allPaths.AddRange(allFiles);
                allPaths.AddRange(allFolders);
             
                for (int i = 9; i >= 0; i--)
                {
                    DateTime date = today.AddDays(-i);
                    bool hasActivity = false;
                    var changedFiles = new List<string>();

                    foreach (var path in allPaths)
                    {
                        DateTime lastModified = File.GetLastWriteTime(path);

                        if (lastModified.Date == date)
                        {
                            hasActivity = true;
                            changedFiles.Add(path);
                        }
                    }

                    ActivityDays.Add(new ActivityDay
                    {
                        Date = date,
                        HasActivity = hasActivity
                    });

                    if (changedFiles.Count > 0)
                    {
                        ChangedFilesByDay[date] = changedFiles;
                    }
                }
            }
            catch (Exception)
            {
                FileCount = 0;
                FolderCount = 0;
                ActivityDays.Clear();
                ChangedFilesByDay.Clear();
            }
        }

        public List<string> GetChangedFilesForDate(DateTime date)
        {
            if (ChangedFilesByDay.TryGetValue(date, out var files))
            {
                return files;
            }
            return new List<string>();
        }

        private List<string> GetFiles(string directory, bool includeSubdirectories)
        {
            try
            {
                var filesList = new List<string>();
                
                filesList.AddRange(Directory.GetFiles(directory));
                
                if (includeSubdirectories)
                {
                    foreach (var subDir in Directory.GetDirectories(directory))
                    {
                        try
                        {
                            filesList.AddRange(GetFiles(subDir, true));
                        }
                        catch
                        {
                            // Skip directories we can't access
                        }
                    }
                }

                return filesList;
            }
            catch
            {
                return new List<string>();
            }
        }

        private List<string> GetFolders(string directory, bool includeSubdirectories)
        {
            try
            {
                var foldersList = new List<string>();
                
                foldersList.AddRange(Directory.GetDirectories(directory));

                if (includeSubdirectories)
                {
                    foreach (var subDir in Directory.GetDirectories(directory))
                    {
                        try
                        {
                            foldersList.AddRange(GetFolders(subDir, true));
                        }
                        catch
                        {
                        }
                    }
                }

                return foldersList;
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}