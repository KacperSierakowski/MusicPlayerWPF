using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerWPF.Models
{
    class ArtistAlbums
    {
        [Key]
        public int ID { get; set; }
        public int AlbumID { get; set; }
        public int ArtistID { get; set; }
        public virtual Artist Artist { get; set; }
        public virtual Album Album { get; set; }
    }
}
