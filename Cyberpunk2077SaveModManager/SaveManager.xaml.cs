using Cyberpunk2077SaveModManager.DataSource;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Linq;

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

        private void Page_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            this.RefreshSaveList();
        }

        private void RefreshSaveList()
        {
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
                    // TODO: Log the exception
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }
    }
}
