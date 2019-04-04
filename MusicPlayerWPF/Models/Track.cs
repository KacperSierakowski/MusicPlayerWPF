using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerWPF.Models
{
    class Track
    {
        [Key]
        public int TrackID { get; set; }
        [Required]
        [Display(Name = "Track title")]
        public string Title { get; set; }
        public string FilePath { get; set; }
        public virtual ICollection<ArtistTracks> ArtistTracks { get; set; }
        public int AlbumID { get; set; }
        public virtual Album Album { get; set; }
    }
}
