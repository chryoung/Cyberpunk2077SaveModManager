using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cyberpunk2077SaveModManager
{
    public sealed partial class SaveManager : Page
    {
        public SaveManager()
        {
            this.InitializeComponent();
            this.RefreshSaveList();
        }

        public string SaveDirectoryPath { get; private set; } = DefaultSaveDirectoryPath;

        public ObservableCollection<DataEntity.SaveFile> SaveFiles { get; private set; } = [];

        private static string DefaultSaveDirectoryPath => $"{Environment.GetEnvironmentVariable("UserProfile")}\\Saved Games\\CD Projekt Red\\Cyberpunk 2077";

        private void RefreshSaveList()
        {
            var saveFilePaths = Directory.GetDirectories(this.SaveDirectoryPath)
                .Select(Path.GetFullPath);
            List<DataEntity.SaveFile> saveFiles = [];
            foreach (var path in saveFilePaths) {
                try
                {
                    var saveFile = new DataEntity.SaveFile(path);
                    saveFiles.Add(saveFile);
                }
                catch (Exception ex)
                {
                    // TODO: Log the exception
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }

            foreach (var saveFile in saveFiles.OrderByDescending(sf => sf.Metadata.Data.metadata.timestamp))
            {
                SaveFiles.Add(saveFile);
            }
        }
    }
}
