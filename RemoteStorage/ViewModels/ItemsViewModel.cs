using Azure.Storage.Blobs.Models;
using RemoteStorage.DAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteStorage.ViewModels
{
    public class ItemsViewModel
    {
        public const string ForwardSlash = "/";

        public ObservableCollection<BlobItem> Items { get; }

        public ObservableCollection<string> Directories { get; }

        private string? directory;

        public string? Directory { 
            get => directory;
            set
            {
                directory = value;
                Refresh();
            }
        }

        public ItemsViewModel()
        {
            Items = new ObservableCollection<BlobItem>();
            Directories = new ObservableCollection<string>();
            Refresh();
        }

        private void Refresh()
        {
            Directories.Clear();
            Items.Clear();
            //prvo punim direktorije

            Repository.Container.GetBlobs().ToList().ForEach(item =>
            {
                if (item.Name.Contains(ForwardSlash))
                {
                    var dir = item.Name[..item.Name.LastIndexOf(ForwardSlash)];
                    if (!Directories.Contains(dir))
                    {
                        Directories.Add(dir);
                    }
                }
                //sada rjesavam iteme
                if (string.IsNullOrEmpty(Directory) && !item.Name.Contains(ForwardSlash))
                {
                    Items.Add(item);
                }
                //sada ubacujem iteme koji pripadaju selektiranom direktoriju
                else if (!string.IsNullOrEmpty(Directory) && item.Name.StartsWith($"{Directory}{ForwardSlash}")) 
                {
                    Items.Add(item);
                }
            });
        }

        public async Task UploadAsync(string path, string directory)
        {
            var filename = path[(path.LastIndexOf(Path.DirectorySeparatorChar) + 1)..];

            if (!string.IsNullOrEmpty(directory))
            {
                filename = $"{Directory}{ForwardSlash}{filename}";
            }
            using var fs = File.OpenRead(path);
            await Repository.Container.GetBlobClient(filename)
                .UploadAsync(fs, true);
            Refresh();
        }

        public async Task DownloadAsync(BlobItem item, string path)
        {


            using var fs = File.OpenWrite(path);
            await Repository.Container.GetBlobClient(item.Name)
                .DownloadToAsync(fs);
            Refresh();
        }
        public async Task DeleteAsync(BlobItem item)
        {
            await Repository.Container.GetBlobClient(item.Name)
                .DeleteAsync();
            Refresh();
        }

    }
}
