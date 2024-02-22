using Azure.Storage.Blobs.Models;
using Microsoft.Win32;
using RemoteStorage.ViewModels;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RemoteStorage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ItemsViewModel itemsViewModel;
        //JPEG, TIFF, PNG, SVG, GIF
        private readonly string filter = "Graphic files|*.jpg;*.jpeg;*.tiff;*.png;*.svg;*.gif";

        public MainWindow()
        {
            InitializeComponent();
            itemsViewModel = new ItemsViewModel();
            cbDirectories.ItemsSource = itemsViewModel.Directories;
            lbItems.ItemsSource = itemsViewModel.Items;
        }

        private void LbItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataContext = lbItems.SelectedItem as BlobItem;
        }

        private void CbDirectories_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                itemsViewModel.Directory = cbDirectories.Text.Trim();
                cbDirectories.SelectedItem = itemsViewModel.Directory;
            }
        }

        private void CbDirectories_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (itemsViewModel.Directories.Contains(cbDirectories.Text))
            {
                itemsViewModel.Directory = cbDirectories.Text;
                cbDirectories.SelectedItem = itemsViewModel.Directory;
            }
        }

        private async void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (lbItems.SelectedItem is not BlobItem item)
            {
                return;
            }

            var dialog = new SaveFileDialog
            {
                FileName = item.Name[(item.Name.LastIndexOf(ItemsViewModel.ForwardSlash) + 1)..],
                Filter = filter
            };

            if (dialog.ShowDialog() == true)
            {
                await itemsViewModel.DownloadAsync(item, dialog.FileName);
            }
            cbDirectories.Text = itemsViewModel.Directory;
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lbItems.SelectedItem is not BlobItem item)
            {
                return;
            }

            await itemsViewModel.DeleteAsync(item);
            cbDirectories.Text = itemsViewModel.Directory;
        }

        private async void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = filter
            };
            if (dialog.ShowDialog() == true)
            {
                itemsViewModel.Directory = GetFileExtension(dialog.FileName);
                await itemsViewModel.UploadAsync(dialog.FileName, itemsViewModel.Directory);
            }
            cbDirectories.Text = itemsViewModel.Directory;
        }

        private string? GetFileExtension(string fileName)
        {
            return System.IO.Path.GetExtension(fileName).Substring(1).ToLower();
        }
    }
}
