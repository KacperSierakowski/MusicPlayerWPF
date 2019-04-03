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
//Icons from https://www.flaticon.com/free-icon/pencil-edit-button_61456
//https://docs.microsoft.com/pl-pl/dotnet/framework/wpf/graphics-multimedia/how-to-control-a-mediaelement-play-pause-stop-volume-and-speed
namespace MusicPlayerWPF
{
    public partial class MainWindow : Window
    {
        DispatcherTimer dispatcherTimer;
        bool isDragging = false;
        String[] FileName;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
                dispatcherTimer.Tick += new EventHandler(timer_Tick);
            }
            catch (Exception ee)
            {
                string ErrorTimer = "Timmer:" + ee.Data.ToString();
            }
        }
        void timer_Tick(object sender, EventArgs e)
        {
            if (!isDragging)
            {
                timelineSlider.Value = myMediaElement.Position.TotalSeconds;
            }
        }
        // The Play method will begin the media if it is not currently active or 
        // resume media if it is paused. This has no effect if the media is
        // already running.
        void OnMouseDownPlayMedia(object sender, MouseButtonEventArgs args)
        {
            myMediaElement.Play();
            ImagePlayMedia.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/UI_play_Yellow.png"));
            InitializePropertyValues();
        }
        private void OnMouseUpPlayMedia(object sender, MouseButtonEventArgs e)
        {
            ImagePlayMedia.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/UI_play.png"));

            ImagePlayMedia.Visibility = Visibility.Hidden;
            ImagePauseMedia.Visibility = Visibility.Visible;
        }
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

            ImagePlayMedia.Visibility = Visibility.Visible;
            ImagePauseMedia.Visibility = Visibility.Hidden;
        }
        // The StopReplay method stops and resets the media to be played from
        // the beginning.
        void OnMouseDownStopAndReplayMedia(object sender, MouseButtonEventArgs args)
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
                    else
                    {
                        MessageBox.Show("you are choose wrong file");
                    }
                }
                ImagePauseMedia.Visibility = Visibility.Visible;
                ImagePlayMedia.Visibility = Visibility.Hidden;
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
                ImagePauseMedia.Visibility = Visibility.Visible;
                ImagePlayMedia.Visibility = Visibility.Hidden;
            }
        }
        private void OnMouseUpStopMedia(object sender, MouseButtonEventArgs e)
        {
            ImageStopMedia.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/UI_replay.png"));
        }
        // Change the volume of the media.
        private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            myMediaElement.Volume = (double)volumeSlider.Value;
        }
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
        private void Element_MediaEnded(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            myMediaElement.Stop();
            myMediaElement.Source = null;
            timelineSlider.Value = 0;
            TrackCover.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/TrackCoverUknown.png"));

            ImagePauseMedia.Visibility = Visibility.Hidden;
            ImagePlayMedia.Visibility = Visibility.Hidden;
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
        void InitializePropertyValues()
        {
            // Set the media's starting Volume and SpeedRatio to the current value of the
            // their respective slider controls.
            myMediaElement.Volume = (double)volumeSlider.Value;
        }

        String FilePath;
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
                            // Create a System.Windows.Controls.Image control
                            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                            img.Source = bitmap;
                            TrackCover.Source = img.Source;
                        }
                        catch (Exception ee)
                        {
                            string errorNoCover = ee.Data.ToString();
                            TrackCover.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/TrackCoverUknown.png"));
                        }
                        try
                        {
                            Artist.Text = "" + tagFile.Tag.Performers[0].ToString();
                            TrackTitle.Text = "" + tagFile.Tag.Title.ToString();
                            AlbumTitle.Text = "" + tagFile.Tag.Album.ToString();
                        } catch (Exception ee)
                        {
                            string errorNoArtist = ee.Data.ToString();
                        }
                        myMediaElement.Source = new Uri(FilePath);
                        myMediaElement.Play();
                    }
                    else
                    {
                        MessageBox.Show("you are choose wrong file");
                    }
                }
                e.Handled = true;
                ImagePlayMedia.Visibility = Visibility.Hidden;
                ImagePauseMedia.Visibility = Visibility.Visible;
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
        private void OnMouseDownEditTrack(object sender, MouseButtonEventArgs e)
        {
            ImageEditTrack.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/UI_edit_Yellow.png"));
            EditTrackWindow EditTrackWindow = new EditTrackWindow();
            EditTrackWindow.TrackTitle.Text = TrackTitle.Text;
            EditTrackWindow.AlbumTitle.Text = AlbumTitle.Text;
            EditTrackWindow.Artist.Text = Artist.Text;
            EditTrackWindow.TrackCover.Source = TrackCover.Source;
            EditTrackWindow.FilePathBlock.Text = FilePath;
            EditTrackWindow.ShowDialog();
            ImageEditTrack.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/UI_edit.png"));

        }
        private void OnMouseUpEditTrack(object sender, MouseButtonEventArgs e)
        {

        }

    }
}
