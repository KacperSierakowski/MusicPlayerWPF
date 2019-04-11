using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using MusicPlayerWPF.Models;
using TagLib;
//Icons from https://www.flaticon.com/free-icon/pencil-edit-button_61456
//https://docs.microsoft.com/pl-pl/dotnet/framework/wpf/graphics-multimedia/how-to-control-a-mediaelement-play-pause-stop-volume-and-speed
namespace MusicPlayerWPF
{
    public partial class MainWindow : Window
    {
        MusicPlayerDB db = new MusicPlayerDB();
        DispatcherTimer dispatcherTimer;
        bool isDragging = false;
        String[] FileName;
        public MainWindow()
        {
            InitializeComponent();
            db.Database.Initialize(true);
            try
            {
                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
                dispatcherTimer.Tick += new EventHandler(timer_Tick);
            }
            catch (Exception ee)
            {
                string errorTimer = "Błąd z timmerem: " + ee.Data.ToString();
                MessageBox.Show(errorTimer);
            }
        }
        #region Play media onClick

        // The Play method will begin the media if it is not currently active or 
        // resume media if it is paused. This has no effect if the media is
        // already running.
        void OnMouseDownPlayMedia(object sender, MouseButtonEventArgs args)
        {
            myMediaElement.Play();
            ImagePlayMedia.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/UI_play_Yellow.png"));
            InitializeVolumeValue();
        }
        private void OnMouseUpPlayMedia(object sender, MouseButtonEventArgs e)
        {
            ImagePlayMedia.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/UI_play.png"));
            HidePlayShowPauseButtons();
        }

        #endregion
        #region Pause media OnClick

        // The Pause method pauses the media if it is currently running.
        // The Play method can be used to resume.
        void OnMouseDownPauseMedia(object sender, MouseButtonEventArgs args)
        {
            myMediaElement.Pause();
            ImagePauseMedia.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/UI_pause_Yellow.png"));
        }
        private void OnMouseUpPauseMedia(object sender, MouseButtonEventArgs e)
        {
            ImagePauseMedia.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/UI_pause.png"));
            HidePauseShowPlayButtons();
        }
        #endregion
        #region Stop and Replay media onClick
        // The StopReplay method stops and resets the media to be played from
        // the beginning.
        public void OnMouseDownStopAndReplayMedia(object sender, MouseButtonEventArgs args)
        {
            dispatcherTimer.Stop();
            myMediaElement.Stop();
            timelineSlider.Value = 0;
            myMediaElement.Source = null;
            TrackCover.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/TrackCoverUknown.png"));
            ImageStopMedia.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/UI_replay_Yellow.png"));

            if (FileName != null)
            {
                if (FileName.Length > 0)
                {
                    String FilePath = FileName[0].ToString();

                    if (CheckMP3Extension(FilePath))
                    {
                        try
                        {
                            TagLib.File tagFile = TagLib.File.Create(FilePath);
                            try
                            {
                                MemoryStream ms = new MemoryStream(tagFile.Tag.Pictures[0].Data.Data);
                                System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                                ms.Seek(0, SeekOrigin.Begin);
                                BitmapImage bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.StreamSource = ms;
                                bitmap.EndInit();
                                System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                                img.Source = bitmap;
                                TrackCover.Source = img.Source;
                            }
                            catch (Exception ee)
                            {
                                string errorNoCover = ee.Data.ToString();
                                TrackCover.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/TrackCoverUknown.png"));
                            }
                            myMediaElement.Source = new Uri(FilePath);
                            myMediaElement.Play();
                        }
                        catch (Exception ee)
                        {
                            MessageBox.Show("Plik nie znajduję się we wskazanej lokalizacji! " + ee.ToString());
                        }

                    }
                    else
                    {
                        MessageBox.Show("Wybrano zły format pliku!");
                    }
                }
                HidePlayShowPauseButtons();
            }
            else
            {
                string filePath = "O:/Pas Oriona/Magiczne Brzemia/Nowy folder (13)/21 Savage - a lot ft. J. Cole.mp3";
                TagLib.File tagFile = TagLib.File.Create(filePath);
                try
                {
                    MemoryStream ms = new MemoryStream(tagFile.Tag.Pictures[0].Data.Data);
                    System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                    img.Source = bitmap;
                    TrackCover.Source = img.Source;
                    Artist.Text = "Artist: " + tagFile.Tag.Performers[0].ToString();
                    TrackTitle.Text = "Title: " + tagFile.Tag.Title.ToString();
                    AlbumTitle.Text = "Album: " + tagFile.Tag.Album.ToString();
                }
                catch (Exception ee)
                {
                    string errorNoCover = ee.Data.ToString();
                    TrackCover.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/TrackCoverUknown.png"));
                }
                myMediaElement.Source = new Uri(filePath);
                myMediaElement.Play();
                HidePlayShowPauseButtons();
            }
        }
        private void OnMouseUpStopAndReplayMedia(object sender, MouseButtonEventArgs e)
        {
            ImageStopMedia.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/UI_replay.png"));
        }
        #endregion
        #region Change media volume 
        // Change the volume of the media.
        private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            myMediaElement.Volume = (double)volumeSlider.Value;
        }
        private void InitializeVolumeValue()
        {
            // Set the media's starting Volume and SpeedRatio to the current value of the
            // their respective slider controls.

            myMediaElement.Volume = (double)volumeSlider.Value;
        }
        #endregion
        #region MediaOpened and MediaEnded
        // When the media opens, initialize the "Seek To" slider maximum value
        // to the total number of miliseconds in the length of the media clip.
        private void Element_MediaOpened(object sender, EventArgs e)
        {
            try
            {
                if (myMediaElement.NaturalDuration.HasTimeSpan)
                {
                    timelineSlider.Maximum = myMediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                    CurrentPosition.Text = String.Format("00:00:00");
                    Duration.Text = String.Format("{0:00}:{1:00}:{2:00}",
                        myMediaElement.NaturalDuration.TimeSpan.Hours,
                        myMediaElement.NaturalDuration.TimeSpan.Minutes,
                        myMediaElement.NaturalDuration.TimeSpan.Seconds);
                    ImageEditTrack.Visibility = Visibility.Visible;
                }
                dispatcherTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void Element_MediaEnded(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            myMediaElement.Stop();
            myMediaElement.Source = null;
            timelineSlider.Value = 0;
            TrackCover.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/TrackCoverUknown.png"));

            ImagePauseMedia.Visibility = Visibility.Hidden;
            ImagePlayMedia.Visibility = Visibility.Hidden;
        }
        #endregion
        #region TimeSlider 
        void timer_Tick(object sender, EventArgs e)
        {
            if (!isDragging)
            {
                timelineSlider.Value = myMediaElement.Position.TotalSeconds;
            }
        }
        private void sliderPosition_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TimeSpan ts = TimeSpan.FromSeconds(e.NewValue);
            CurrentPosition.Text =
                String.Format("{0:00}:{1:00}:{2:00}",
                ts.Hours, ts.Minutes, ts.Seconds);
        }
        private void sliderPosition_DragStarted(object sender, DragStartedEventArgs e)
        {
            isDragging = true;
        }
        private void sliderPosition_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            isDragging = false;
            myMediaElement.Position = TimeSpan.FromSeconds(timelineSlider.Value);
            ////int SliderValue = (int)timelineSlider.Value;
            ////// Overloaded constructor takes the arguments days, hours, minutes, seconds, miniseconds.
            ////// Create a TimeSpan with miliseconds equal to the slider value.
            ////TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
            ////myMediaElement.Position = ts;
        }
        #endregion
        #region  AddFile to Player
        String FilePath;
        TagLib.File tagFile;

        private void Window_Drop(object sender, DragEventArgs e)
        {
            try
            {
                FileName = (String[])e.Data.GetData(DataFormats.FileDrop, true);
                if (FileName.Length > 0)
                {
                    FilePath = FileName[0].ToString();
                    if (CheckMP3Extension(FilePath))
                    {
                        tagFile = TagLib.File.Create(FilePath);
                        try
                        {
                            MemoryStream memoryStream = new MemoryStream(tagFile.Tag.Pictures[0].Data.Data);
                            System.Drawing.Image image = System.Drawing.Image.FromStream(memoryStream);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.StreamSource = memoryStream;
                            bitmapImage.EndInit();
                            TrackCover.Source = bitmapImage;
                        }
                        catch (Exception ee)
                        {
                            string errorNoCover = ee.Data.ToString();
                            //MessageBox.Show("Brak okładki! " + errorNoCover);
                            TrackCover.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/TrackCoverUknown.png"));
                        }
                        try
                        {
                            Artist.Text = "" + tagFile.Tag.Performers[0].ToString();
                            TrackTitle.Text = "" + tagFile.Tag.Title.ToString();
                            AlbumTitle.Text = "" + tagFile.Tag.Album.ToString();
                        }
                        catch (Exception ee)
                        {
                            string errorNoArtist = ee.Data.ToString();
                            MessageBox.Show("Brak Artysty! " + errorNoArtist);
                        }
                        myMediaElement.Source = new Uri(FilePath);
                        myMediaElement.Play();
                        AddToDatabase(tagFile, FilePath);
                    }
                    else
                    {
                        MessageBox.Show("Wybrano zły plik!");
                    }
                }
                e.Handled = true;
                HidePlayShowPauseButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private Boolean CheckMP3Extension(String FilePath)
        {
            Boolean Flag = false;
            try
            {
                String Extension = System.IO.Path.GetExtension(FilePath);

                if (Extension != String.Empty)
                {
                    if (Extension == ".mp3")
                    {
                        Flag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Flag;
        }
        #endregion
        #region Edit track
        String CoverFilePath;
        private void OnMouseDownEditTrack(object sender, MouseButtonEventArgs e)
        {
            HideAddFileWindowDropShowEditGrid();
            ImageEditTrack.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/UI_edit_Yellow.png"));
            AddExistingMetadataFromFileToEditGrid();
            OriginalTrackCover.Source = TrackCover.Source;
        }
        private void AddExistingMetadataFromFileToEditGrid()
        {
            if (tagFile.Tag.Performers[0] != null)
            {
                EditArtist.Text = tagFile.Tag.Performers[0].ToString();
            }
            if (tagFile.Tag.Album != null)
            {
                EditAlbumTitle.Text = tagFile.Tag.Album.ToString();
            }
            if (tagFile.Tag.Title != null)
            {
                EditTrackTitle.Text = tagFile.Tag.Title.ToString();
            }
            if (tagFile.Tag.AlbumArtists[0] != null)
            {
                EditArtistAlbum.Text = tagFile.Tag.AlbumArtists[0].ToString();
            }
        }

        private bool SomethingWasDropped = false;
        private void Cover_Drop(object sender, DragEventArgs e)
        {
            try
            {
                String[] FileName = (String[])e.Data.GetData(DataFormats.FileDrop, true);
                if (FileName.Length > 0)
                {
                    CoverFilePath = FileName[0].ToString();
                    try
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri(CoverFilePath, UriKind.Absolute);
                        bitmapImage.EndInit();
                        OriginalTrackCover.Source = bitmapImage;
                        SomethingWasDropped = true;
                    }
                    catch (Exception ee)
                    {
                        string errorNoCover = ee.Data.ToString();
                        MessageBox.Show("Coś nie tak z okładką: " + errorNoCover);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void EditPerformersOfATrack(TagLib.File tagFile, string newArtist)
        {
            try
            {
                tagFile.Tag.Performers = null;
                tagFile.Tag.Performers = new[] { newArtist };
                tagFile.Save();
            }
            catch (Exception ee)
            {
                string errorData = "Błąd przy dodawaniu danych artysty do pliku! " + ee.Data.ToString();
                MessageBox.Show(errorData);
            }
        }
        private void EditTitleOfATrack(TagLib.File tagFile, string newTitle)
        {
            try
            {
                tagFile.Tag.Title = null;
                tagFile.Tag.Title = newTitle;
                tagFile.Save();
            }
            catch (Exception ee)
            {
                string errorData = "Błąd przy dodawaniu nazwy piosenki do pliku! " + ee.Data.ToString();
                MessageBox.Show(errorData);
            }
        }
        private void EditAlbumTitleOfATrack(TagLib.File tagFile, string newAlbumTitle)
        {
            try
            {
                tagFile.Tag.Album = null;
                tagFile.Tag.Album = newAlbumTitle;
                tagFile.Save();
            }
            catch (Exception ee)
            {
                string errorData = "Błąd przy dodawaniu nazwy ALBUMU do pliku! " + ee.Data.ToString();
                MessageBox.Show(errorData);
            }
        }
        private void EditCoverOfATrack(TagLib.File tagFile, string coverFilePath)
        {
            if (SomethingWasDropped == true)
            {
                try
                {
                    IPicture albumArt = new TagLib.Picture(coverFilePath);
                    tagFile.Tag.Pictures = new TagLib.IPicture[] { albumArt };
                    tagFile.Save();
                }
                catch (Exception ee)
                {
                    string errorData = "Błąd przy dodawaniu OKŁADKI do pliku! " + ee.Data.ToString();
                    MessageBox.Show(errorData);
                }
            }
        }
        private void EditArtistOfAnAlbum(TagLib.File tagFile, string newArtistOfAnAlbum)
        {
            try
            {
                tagFile.Tag.AlbumArtists[0] = newArtistOfAnAlbum;
                tagFile.Save();
            }
            catch (Exception ee)
            {
                string errorData = "Błąd przy dodawaniu wykonawcy ALBUMU do pliku! " + ee.Data.ToString();
                MessageBox.Show(errorData);
            }
        }
        #endregion
        #region Edit database
        private void EditDb(TagLib.File tagFile, string filePath, string newArtistOfATrack, string newAlbumTitle, string newTrackTitle, string newAlbumPerformer)
        {
            string tmpOldArtistName = tagFile.Tag.Performers[0];
            string tmpOldAalbumTitle = tagFile.Tag.Album.ToString();
            Models.Track track = db.Tracks.Where(p => p.FilePath.Equals(filePath)).FirstOrDefault();
            Artist artist = db.Artists.Where(k => k.Name.Contains(tmpOldArtistName)).FirstOrDefault();
            Album album = db.Albums.Where(k => k.Title.Contains(tmpOldAalbumTitle)).FirstOrDefault();

            //Change title of a track in database
            track.Title = newTrackTitle;
            db.Entry(track).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            //Delete old-existing artist of edited track
            DeleteArtistOfATrackInDb(track, artist);
            CheckIfArtitExistAddIfNot(tagFile, artistId: out int artistId);


            if (album.Tracks.Count() > 1)
            {
                DeleteArtistAlbumConnectionFromArtistAlbumTable(album, artist);
            }
            else
            {
                DeleteAlbumOfATrackInDB(album, artist);
            }

            CheckIfAlbumExistAddIfNot(tagFile,  out albumId);
            AddConnectionBetweenArtistAndTrackIfNotExist(artistId, track.TrackID);
        }
        private void DeleteAlbumOfATrackInDB(Album album, Artist artist)
        {
            //Check if this Album have more tracks
            if (db.ArtistAlbums.Where(k => k.ArtistID.Equals(artist.ArtistID)).ToList().Count() > 1)
            {
                DeleteArtistAlbumConnectionFromArtistAlbumTable(album, artist);
            }
            else
            {
                db.Albums.Remove(album);
            }
        }
        private void DeleteArtistAlbumConnectionFromArtistAlbumTable(Album album, Artist artist)
        {
            ArtistAlbums artistAlbums = db.ArtistAlbums.Where(k => k.AlbumID.Equals(album.AlbumID)).Where(o => o.ArtistID.Equals(artist.ArtistID)).FirstOrDefault();
            db.ArtistAlbums.Remove(artistAlbums);
        }
        private void DeleteArtistOfATrackInDb(Models.Track track, Artist artist)
        {
            //Check if this Artist have more tracks
            if (db.ArtistTracks.Where(k => k.ArtistID.Equals(artist.ArtistID)).ToList().Count() > 1)
            {
                DeleteArtistTrackConnectionFromArtistTrackTable(track, artist);
            }
            else
            {
                db.Artists.Remove(artist);
            }
        }
        private void DeleteArtistTrackConnectionFromArtistTrackTable(Models.Track track, Artist artist)
        {
            ArtistTracks artistTracks = db.ArtistTracks.Where(k => k.TrackID.Equals(track.TrackID)).Where(o => o.ArtistID.Equals(artist.ArtistID)).FirstOrDefault();
            db.ArtistTracks.Remove(artistTracks);
        }
        #endregion
        #region Accept changes of a track
        private void EditImageAcceptButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ShowAddFileWindowDropHideEditGrid();
            EditImageAcceptButton.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/UI_accept.png"));
            ImageEditTrack.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/UI_edit.png"));

            SomethingWasDropped = false;
        }
        private void EditImageAcceptButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Element_MediaEnded(sender, e);
            System.Threading.Thread.Sleep(250);
            EditArtistOfATrackInDb(tagFile, FilePath, EditArtist.Text.ToString());
            EditPerformersOfATrack(tagFile, EditArtist.Text.ToString());

            EditTitleOfATrack(tagFile, EditTrackTitle.Text.ToString());

            EditAlbumTitleOfATrack(tagFile, EditAlbumTitle.Text.ToString());

            EditCoverOfATrack(tagFile, CoverFilePath);
            EditArtistOfAnAlbum(tagFile, EditArtistAlbum.Text.ToString());

            try
            {
                Artist.Text = EditArtist.Text.ToString();
                ((MainWindow)Application.Current.MainWindow).TrackTitle.Text = EditTrackTitle.Text.ToString();
                ((MainWindow)Application.Current.MainWindow).AlbumTitle.Text = EditAlbumTitle.Text.ToString();
            }
            catch (Exception ee)
            {
                string errorData = "Błąd przy uaktualnianiu textBlocków! " + ee.Data.ToString();
                MessageBox.Show(errorData);
            }
            EditImageAcceptButton.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/UI_accept_Yellow.png"));
            OnMouseDownStopAndReplayMedia(sender, e);
            OnMouseUpStopAndReplayMedia(sender, e);
            ShowAddFileWindowDropHideEditGrid();
        }
        #endregion
        #region Hide/Show buttons
        private void HidePlayShowPauseButtons()
        {
            ImagePlayMedia.Visibility = Visibility.Hidden;
            ImagePauseMedia.Visibility = Visibility.Visible;
        }
        private void HidePauseShowPlayButtons()
        {
            ImagePlayMedia.Visibility = Visibility.Visible;
            ImagePauseMedia.Visibility = Visibility.Hidden;
        }
        private void HideAddFileWindowDropShowEditGrid()
        {
            ImageEditTrack.Visibility = Visibility.Hidden;
            ImageFileUploadByDrop.Visibility = Visibility.Hidden;
            editGrid.Visibility = Visibility.Visible;
        }
        private void ShowAddFileWindowDropHideEditGrid()
        {
            ImageEditTrack.Visibility = Visibility.Visible;
            editGrid.Visibility = Visibility.Hidden;
            ImageFileUploadByDrop.Visibility = Visibility.Visible;
        }
        #endregion
        #region Album list

        #endregion
        #region Add to database
        private void AddToDatabase(TagLib.File tagFile, string filePath)
        {
            int artistId = 1, albumId = 1, trackId = 1;
            Models.Track track2 = new Models.Track();
            CheckIfAlbumExistAddIfNot(tagFile, track2, out albumId);
            CheckIfArtitExistAddIfNot(tagFile, out artistId);
            track2.FilePath = FilePath;
            track2.Title = tagFile.Tag.Title.ToString();
            CheckIfTrackExistAddIfNot(track2, out trackId);
            AddConnectionBetweenArtistAndTrackIfNotExist(artistId, trackId);
            AddConnectionBetweenArtistAndAlbumIfNotExist(artistId, albumId);
        }
        private void CheckIfTrackExistAddIfNot(Models.Track track, out int trackId)
        {
            if (db.Tracks.Where(d => d.FilePath.Equals(track.FilePath)).FirstOrDefault() != null)
            {
                MessageBox.Show("Plik istnieje!");
                trackId = db.Tracks.Where(d => d.FilePath.Equals(track.FilePath)).FirstOrDefault().TrackID;
            }
            else
            {
                try
                {
                    db.Tracks.Add(track);
                    db.SaveChanges();
                }
                catch (Exception ee)
                {
                    string errorAddingTrackToDatabase = "Błąd przy dodawaniu nowej piosenki do bazy danych! " + ee.Data.ToString();
                    MessageBox.Show(errorAddingTrackToDatabase);
                }
                trackId = track.TrackID;
            }
        }
        private void CheckIfAlbumExistAddIfNot(TagLib.File tagFile, Models.Track track, out int albumId)
        {
            Album album = new Album();
            try
            {
                album = db.Albums.Where(d => d.Title.Equals(tagFile.Tag.Album.ToString())).FirstOrDefault();
            }
            catch (Exception)
            {
                MessageBox.Show("Coś poszło nie tak przy przeszukiwnaiu czy Album istenieje! ");
            }
            if (album != null)
            {
                track.AlbumID = album.AlbumID;
                albumId = album.AlbumID;
            }
            else
            {
                if (tagFile.Tag.Album != null)
                {
                    Album album2 = new Album();
                    album2.Title = tagFile.Tag.Album.ToString();
                    AddAlbumAndGetIdOfLatRecord(album2, out albumId);
                    track.AlbumID = albumId;
                }
                else
                {
                    albumId = 1;//ID==1 means Without Album
                    track.AlbumID = albumId;
                }
            }
        }
        private void AddAlbumAndGetIdOfLatRecord(Album newAlbum, out int albumId)
        {
            try
            {
                db.Albums.Add(newAlbum);
                db.SaveChanges();
            }
            catch (Exception ee)
            {
                string errorAddingTrackToDatabase = "Błąd przy dodawaniu nowego albumu do bazy danych! " + ee.Data.ToString();
                MessageBox.Show(errorAddingTrackToDatabase);
            }
            albumId = newAlbum.AlbumID;
        }
        private void CheckIfArtitExistAddIfNot(TagLib.File tagFile, out int artistId)
        {
            Artist artist = new Artist();
            try
            {
                string tmpArtist = tagFile.Tag.Performers[0].ToString();
                artist = db.Artists.Where(d => d.Name.Contains(tmpArtist)).FirstOrDefault();
            }
            catch (Exception e)
            {
                MessageBox.Show("Coś poszło nie tak przy przeszukiwnaiu czy Artysta istenieje! " + e.ToString());
            }
            if (artist != null)
            {
                artistId = artist.ArtistID;
            }
            else
            {
                Artist artist2 = new Artist();
                if (tagFile.Tag.Performers[0] != null)
                {
                    artist2.Name = tagFile.Tag.Performers[0];
                    AddArtistAddGetIdOfLatRecord(artist2, out artistId);
                }
                else
                {
                    artist2.Name = "BRAK";
                    AddArtistAddGetIdOfLatRecord(artist2, out artistId);
                }
            }
        }
        private void AddArtistAddGetIdOfLatRecord(Artist newArtist, out int artistId)
        {
            try
            {
                db.Artists.Add(newArtist);
                db.SaveChanges();
            }
            catch (Exception ee)
            {
                string errorAddingTrackToDatabase = "Błąd przy dodawaniu nowego artysty do bazy danych! " + ee.Data.ToString();
                MessageBox.Show(errorAddingTrackToDatabase);
            }
            artistId = newArtist.ArtistID;
        }
        private void AddConnectionBetweenArtistAndTrackIfNotExist(int artistId, int trackId)
        {
            ArtistTracks artistTracks = new ArtistTracks();
            try
            {
                artistTracks = db.ArtistTracks.Where(d => d.ArtistID.Equals(artistId)).Where(g => g.TrackID.Equals(trackId)).FirstOrDefault();
            }
            catch (Exception)
            {
                MessageBox.Show("Coś poszło nie tak przy przeszukiwnaiu czy połączenie w ArtistTrack istenieje! ");
            }
            if (artistTracks == null)
            {
                ArtistTracks artistTracks2 = new ArtistTracks();
                artistTracks2.ArtistID = artistId;
                artistTracks2.TrackID = trackId;
                try
                {
                    db.ArtistTracks.Add(artistTracks2);
                    db.SaveChanges();
                }
                catch (Exception ee)
                {
                    string errorAddingTrackToDatabase = "Błąd przy dodawaniu połączeń między piosenką, a artystom w bazie danych! " + ee.Data.ToString();
                    MessageBox.Show(errorAddingTrackToDatabase);
                }
            }
            else
            {
                MessageBox.Show("Połączenie między Artystą, a Trackiem istnieje!");
            }
        }
        private void AddConnectionBetweenArtistAndAlbumIfNotExist(int artistId, int albumId)
        {
            ArtistAlbums artistAlbums = new ArtistAlbums();
            try
            {
                artistAlbums = db.ArtistAlbums.Where(d => d.ArtistID.Equals(artistId)).Where(g => g.AlbumID.Equals(albumId)).FirstOrDefault();
            }
            catch (Exception)
            {
                MessageBox.Show("Coś poszło nie tak przy przeszukiwnaiu czy połączenie w ArtistAlbum istenieje! ");
            }
            if (artistAlbums == null)
            {
                ArtistAlbums artistAlbums2 = new ArtistAlbums();
                artistAlbums2.ArtistID = artistId;
                artistAlbums2.AlbumID = albumId;
                try
                {
                    db.ArtistAlbums.Add(artistAlbums2);
                    db.SaveChanges();
                }
                catch (Exception ee)
                {
                    string errorAddingTrackToDatabase = "Błąd przy dodawaniu połączeń między albumem, a artystom w bazie danych! " + ee.Data.ToString();
                    MessageBox.Show(errorAddingTrackToDatabase);
                }
            }
            else
            {
                MessageBox.Show("Połączenie między Artystą, a Albumem istnieje!");
            }
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MemoryStream memoryStream = new MemoryStream(tagFile.Tag.Pictures[0].Data.Data);
            System.Drawing.Image image = System.Drawing.Image.FromStream(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();
            Image imagee = new Image();
            imagee.Source = bitmapImage;
            AlbumsWrapPanel.Children.Add(imagee);

        }
    }
}
