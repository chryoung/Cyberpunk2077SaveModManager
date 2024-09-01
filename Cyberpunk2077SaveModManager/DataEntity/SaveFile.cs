using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Media.Imaging;
using Newtonsoft.Json;
using Windows.Storage.Streams;
using Windows.Storage;

namespace Cyberpunk2077SaveModManager.DataEntity
{
    public class SaveFile : INotifyPropertyChanged
    {
        private const string JsonMetadataFileName = "metadata.9.json";
        private const string ScreenshotFileName = "screenshot.png";
        private BitmapImage _screenshot;

        public SaveFile(string path)
        {
            this.Path = path;
            var jsonFilePath = System.IO.Path.Combine(path, JsonMetadataFileName);
            this.Metadata = JsonConvert.DeserializeObject<Save.SaveMetadata>(File.ReadAllText(jsonFilePath));
            LoadScreenshot();
        }

        public string Path { get; private set; }

        public Save.SaveMetadata Metadata { get; private set; }

        public long Size =>
            Directory.GetFiles(this.Path)
                .Select(f => new FileInfo(f))
                .Select(fi => fi.Length)
                .Sum();

        public string ReadableSize =>
            Utils.FormatSize(this.Size);

        public BitmapImage Screenshot
        {
            get => _screenshot;
            private set
            {
                _screenshot = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private string ScreenshotFilePath => System.IO.Path.Combine(this.Path, ScreenshotFileName);

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void LoadScreenshot()
        {
            // This method cannot be an async Task because it has to be on the UI thread to create
            // BitmapImage. Moving it to the background thread will result COM error RPC_E_WRONG_THREAD.
            try
            {
                if (string.IsNullOrEmpty(this.ScreenshotFilePath))
                {
                    return;
                }

                // Load the image
                BitmapImage bitmapImage = new();
                StorageFile file = await StorageFile.GetFileFromPathAsync(this.ScreenshotFilePath);
                using IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                await bitmapImage.SetSourceAsync(stream);
                this.Screenshot = bitmapImage;
            }
            catch (Exception ex)
            {
                // TODO: log exception
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}
