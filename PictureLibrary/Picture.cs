using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace PictureLibrary
{
    public class Picture
    {
        public string picName { get; set; }
        public string albumName { get; set; }
        public DateTime dateAdded { get; set; }
        public string path { get; set; }
        public BitmapImage picThumbnail {get;set;}
       
    }
}
