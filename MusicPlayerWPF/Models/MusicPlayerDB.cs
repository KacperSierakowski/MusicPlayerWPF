using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerWPF.Models
{
    class MusicPlayerDB : DbContext
    {
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<ArtistAlbums> ArtistAlbums { get; set; }
        public DbSet<ArtistTracks> ArtistTracks { get; set; }

    }
}
