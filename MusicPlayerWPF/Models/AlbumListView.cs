using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MusicPlayerWPF.Models
{
    public class AlbumListView
    {
        public BitmapImage Cover { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
    }
}
