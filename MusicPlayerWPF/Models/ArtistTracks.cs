using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerWPF.Models
{
    class ArtistTracks
    {
        [Key]
        public int ID { get; set; }
        public int TrackID { get; set; }
        public int ArtistID { get; set; }
        public virtual Track Track { get; set; }
        public virtual Artist Artist { get; set; }
    }
}
