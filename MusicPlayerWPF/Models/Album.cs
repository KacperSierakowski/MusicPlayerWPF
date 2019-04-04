using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerWPF.Models
{
    class Album
    {
        [Key]
        public int AlbumID { get; set; }
        [Required]
        [Display(Name = "Album title")]
        public string Title { get; set; }
        public virtual ICollection<ArtistAlbums> ArtistAlbums { get; set; }
        public virtual ICollection<Track> Tracks { get; set; }
    }
}
