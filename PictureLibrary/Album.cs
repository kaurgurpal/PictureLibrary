using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace PictureLibrary
{
    public class Album
    {
        public string albumName { get; set; } 
        public uint totalItems { get; set; }
        public DateTime dateAlbumCreated { get; set; }
        public string coverPicPath { get; set; }

        
    }
}
