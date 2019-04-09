using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerWPF.Models
{
    public class Artist
    {
        [Key]
        public int ArtistID { get; set; }
        public string Name { get; set; }
        public string Nickname { get; set; }
        public virtual ICollection<ArtistTracks> TrackArtist { get; set; }
        public virtual ICollection<ArtistAlbums> AlbumArtists { get; set; }
    }
}
