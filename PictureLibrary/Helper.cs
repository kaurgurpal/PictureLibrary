using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Search;
namespace PictureLibrary
{
    class Helper
    {


       // public ObservableCollection<Picture> allPicsList;
        public static StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        
        public async Task<IReadOnlyList<Picture>> GetPicturesByAlbum(string albumName)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            var allPicsList = new List<Picture>();
            // Set options for file type and sort order.
            List<string> fileTypeFilter = new List<string>();
            fileTypeFilter.Add(".jpg");
            fileTypeFilter.Add(".png");
            QueryOptions queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, fileTypeFilter);
            // Get the JPG files in the user's Pictures folder
            // and its subfolders and sort them by date.
            StorageFileQueryResult results = storageFolder.CreateFileQueryWithOptions(queryOptions);

            IReadOnlyList<StorageFile> files = await results.GetFilesAsync();


            foreach (StorageFile file in files.OrderBy(a => a.DateCreated))
            {
                var pic = new Picture()
                {
                    picName = file.Name,
                    albumName = null,
                    path = file.Path,
                    dateAdded = file.DateCreated.LocalDateTime
                };
                allPicsList.Add(pic);
            }
            return allPicsList;
        }

        public async Task uploadCameraPic()
        {
            CameraCaptureUI cameraCapture = new CameraCaptureUI();
            cameraCapture.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            StorageFile pic = await cameraCapture.CaptureFileAsync(CameraCaptureUIMode.Photo);
            await pic.CopyAsync(ApplicationData.Current.LocalFolder, pic.Name, NameCollisionOption.GenerateUniqueName);

        }

        public async Task GetAllAlbums()
        {

            var allAlbumsList = new ObservableCollection<Album>();

            IReadOnlyList<StorageFolder> albums = await storageFolder.GetFoldersAsync();

            foreach (StorageFolder album in albums.OrderBy(a => a.Name))
            {
                var albumItem = new Album()
                {
                    albumName = album.Name,
                    coverPicPath = null,
                    dateAlbumCreated = album.DateCreated.LocalDateTime,
                    totalItems = 0

                };
                allAlbumsList.Add(albumItem);
            }


        }


    }
}
