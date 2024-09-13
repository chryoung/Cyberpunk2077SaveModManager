using Microsoft.UI.Xaml.Media.Imaging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using IOPath = System.IO.Path;
using Serilog;

namespace Cyberpunk2077SaveModManager.DataEntity
{
    public class SaveFile
    {
        private const string JsonMetadataFileName = "metadata.9.json";
        private const string ScreenshotFileName = "screenshot.png";
        private readonly object _initLock = new();
        private bool _isLoaded = false;

        public SaveFile(string path)
        {
            this.Path = path;
            this.Name = IOPath.GetFileName(path);
        }

        public string Path { get; private set; }

        public Save.SaveMetadata Metadata { get; private set; }

        public string Name { get; private set; }

        public long Size { get; private set; }

        public string Level => Metadata?.Data.metadata.levelString ?? string.Empty;

        public string Timestamp => Metadata?.Data.metadata.timestampString ?? string.Empty;

        public string ReadableSize => Utils.FormatSize(this.Size);

        public BitmapImage Screenshot { get; private set; }

        public bool IsLoaded
        {
            get
            {
                lock (_initLock)
                {
                    return _isLoaded;
                }
            }

            private set
            {
                lock (_initLock)
                {
                    _isLoaded = value;
                }
            }
        }

        public async Task<SaveFile> LoadAsync()
        {
            if (!this.IsLoaded)
            {
                SaveFile loadedSaveFile = new();
                loadedSaveFile.Path = this.Path;
                loadedSaveFile.Name = this.Name;
                await loadedSaveFile.LoadMetadataAsync();
                await loadedSaveFile.LoadScreenshotAsync();
                loadedSaveFile.Size = loadedSaveFile.GetSaveSize();
                loadedSaveFile.IsLoaded = true;

                return loadedSaveFile;
            }
            else
            {
                return this;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is SaveFile file &&
                   Path == file.Path;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Path);
        }

        private SaveFile() {}

        private string ScreenshotFilePath => System.IO.Path.Combine(this.Path, ScreenshotFileName);

        private async Task LoadMetadataAsync()
        {
            try
            {
                var jsonFilePath = IOPath.Combine(this.Path, JsonMetadataFileName);
                var jsonContent = await File.ReadAllTextAsync(jsonFilePath);
                this.Metadata = JsonConvert.DeserializeObject<Save.SaveMetadata>(jsonContent);
                this.Name = this.Metadata.Data.metadata.name;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to load metadata for {path}: {errorMessage}", this.Path, ex.Message);
            }
        }

        private async Task LoadScreenshotAsync()
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
                Log.Logger.Error("Failed to load screenshot for {path}: {errorMessage}", this.Path, ex.Message);
            }
        }

        private long GetSaveSize() =>
            Directory.GetFiles(this.Path)
                .Select(f => new FileInfo(f))
                .Select(fi => fi.Length)
                .Sum();
    }
}
