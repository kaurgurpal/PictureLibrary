using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using Windows.Storage.Search;
using System.Threading.Tasks;
using Windows.Storage.FileProperties;
using Windows.UI.Popups;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PictureLibrary
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Picture> allPicsList;
        public ObservableCollection<Album> allAlbumsList;
        const ThumbnailMode thumbnailMode = ThumbnailMode.PicturesView;
        const uint size = 300;
        public  List<Picture> lstDelete = new List<Picture>();
        Helper helperObj;

        public MainPage()
        {
            MaximizeWindowOnLoad();
            this.InitializeComponent();
            allPicsList = new ObservableCollection<Picture>();
            allAlbumsList = new ObservableCollection<Album>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            helperObj = new Helper();
            await GetAllPictures();
            
        }
        /// <summary>
        /// Gets all the pictures from the Local state storage folder
        /// </summary>
        /// <returns></returns>
        public async Task GetAllPictures()
        {
            // Set options for file type 
            List<string> fileTypeFilter = new List<string>();
            fileTypeFilter.Add(".jpg");
            fileTypeFilter.Add(".png");
            QueryOptions queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, fileTypeFilter);
            StorageFileQueryResult results = Helper.storageFolder.CreateFileQueryWithOptions(queryOptions);
            //Get all the pictures asynchronously
            IReadOnlyList<StorageFile> files = await results.GetFilesAsync();
            foreach (StorageFile file in files)
            {
                //Get thumbnail for the fetched picture
                var thumb = await file.GetThumbnailAsync(thumbnailMode, size);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.SetSource(thumb);
                var pic = new Picture()
                {
                    picName = file.Name,
                    albumName = null,
                    path = file.Path,
                    dateAdded = file.DateCreated.LocalDateTime,
                    picThumbnail = bitmapImage
                };
                //add to observable collection
                allPicsList.Add(pic);
            }
        }

        private static void MaximizeWindowOnLoad()
        {
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Maximized;
        }


        private void Home_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
        /// <summary>
        /// Add pictures to the storage folder and the observable collection to update the gridview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddPicture_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            //Set options for file open picker
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".png");

            var fileList = await openPicker.PickMultipleFilesAsync();
            if (fileList != null)
            {
                foreach (var file in fileList)
                {
                    var stream = await file.CopyAsync(storageFolder, file.Name, NameCollisionOption.GenerateUniqueName);
                    var thumb = await stream.GetThumbnailAsync(thumbnailMode, size);
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(thumb);
                    allPicsList.Add(
                       new Picture
                       {
                           albumName = "",
                           dateAdded = stream.DateCreated.LocalDateTime,
                           path = stream.Path,
                           picName = stream.Name,
                           picThumbnail = bitmapImage
                       }
                       );
                }
            }
            else
            {
                //  
            }
        }

        private async void AddAlbum_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var result = await AddAlbumContentDialog.ShowAsync();

        }

        private void AppBarToggleButton_Click(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private async void AddAlbumContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                var appFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(txtAlbumName.Text, CreationCollisionOption.FailIfExists);
            }
            catch (Exception)
            {

                args.Cancel = true;
                txtError.Text = "Album Name Already Exists!";
            }
        }

        private void ItemsGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Here I'm calculating the number of columns I want based on
            // the width of the page
            var columns = Math.Ceiling(ActualWidth / 350);
            ((ItemsWrapGrid)itemsGridView.ItemsPanelRoot).ItemWidth = e.NewSize.Width / columns;
        }

        private async void BtnCamera_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                helperObj = new Helper();
                await helperObj.uploadCameraPic();
                this.Frame.Navigate(typeof(MainPage));
            }
            catch (Exception ex)
            {
                await new MessageDialog("Error Occurred : " + ex.Message.ToString(), "Error").ShowAsync();
            }
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var tb = (Image)e.OriginalSource;
            flipView1.SelectedItem = tb.DataContext;
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void ChkSelect_Click(object sender, RoutedEventArgs e)
        {
            var chktb = (CheckBox)e.OriginalSource;
            var dataCxtx = (Picture)chktb.DataContext;
            if (chktb.IsChecked==true)
            {
                lstDelete.Add(dataCxtx);
            }
            else
            {
                lstDelete.Remove(dataCxtx);
            }
        }

        private async void BtnDelete_ClickAsync(object sender, RoutedEventArgs e)
        {
            foreach (var item in lstDelete.ToList())
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(item.path).AsTask();
                await file.DeleteAsync(StorageDeleteOption.Default);
                allPicsList.Remove(item);
                lstDelete.Remove(item);
            }
        }
    }
}
