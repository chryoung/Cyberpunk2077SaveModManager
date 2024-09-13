using Cyberpunk2077SaveModManager.DataSource;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;
using Serilog;

namespace Cyberpunk2077SaveModManager
{
    public sealed partial class SaveManager : Page
    {
        private static string DefaultSaveDirectoryPath() => $"{Environment.GetEnvironmentVariable("UserProfile")}\\Saved Games\\CD Projekt Red\\Cyberpunk 2077";

        public SaveManager()
        {
            this.InitializeComponent();
        }

        public string SaveDirectoryPath { get; private set; } = DefaultSaveDirectoryPath();

        public VirtualSaveFileDataSource SaveFiles { get; private set; } = new();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.RefreshSaveList();
        }

        private void RefreshSaveList()
        {
            this.SaveFiles.Clear();
            var saveFilePaths = Directory
                .GetDirectories(this.SaveDirectoryPath)
                .Select(Path.GetFullPath);
            foreach (var path in saveFilePaths)
            {
                try
                {
                    var saveFile = new DataEntity.SaveFile(path);
                    this.SaveFiles.Add(saveFile);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error("Failed to refresh save list for {path}: {errorMessage}", path, ex.Message);
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = this.SaveDataListView.SelectedItems.Cast<DataEntity.SaveFile>().ToList();

            foreach (var selectedItem in selectedItems)
            {
                var saveFile = (DataEntity.SaveFile)selectedItem;
                try
                {
                    this.SaveFiles.Remove(selectedItem);
                    FileSystem.DeleteDirectory(saveFile.Path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error("Failed to delete save file {saveFilePath}: {errorMessage}", saveFile.Path, ex.Message);
                }
            }
        }

        private void SaveDataListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SaveDataListView.SelectedItems.Count > 0)
            {
                this.DeleteButton.IsEnabled = true;
            }
            else
            {
                this.DeleteButton.IsEnabled = false;
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            this.RefreshSaveList();
        }
    }
}
