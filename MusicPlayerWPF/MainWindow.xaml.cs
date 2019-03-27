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

//https://docs.microsoft.com/pl-pl/dotnet/framework/wpf/graphics-multimedia/how-to-control-a-mediaelement-play-pause-stop-volume-and-speed
namespace MusicPlayerWPF
{
    public partial class MainWindow : Window
    {
        DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            InitializeComponent();

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += new EventHandler(timer_Tick);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (!isDragging)
            {
                timelineSlider.Value = myMediaElement.Position.TotalSeconds;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //MusicPlayerWPF.Models.Tracks.
            string filePath = "O:/Pas Oriona/Magiczne Brzemia/Nowy folder (13)/21 Savage - a lot ft. J. Cole.mp3";
            TagLib.File tagFile = TagLib.File.Create(filePath);
            MemoryStream ms = new MemoryStream(tagFile.Tag.Pictures[0].Data.Data);
            //tagFile.Tag.Pictures[0].Data.Data;
            //TagLib.Image.ImageTag image = TagLib.Image.ImageTag(filePath);

            System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
            ms.Seek(0, SeekOrigin.Begin);

            // ImageSource for System.Windows.Controls.Image
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = ms;
            bitmap.EndInit();

            // Create a System.Windows.Controls.Image control
            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            img.Source = bitmap;
            TrackCover.Source = img.Source;

            myMediaElement.Source = new Uri(filePath);
            myMediaElement.Play();
        }
      
        // Play the media.
        void OnMouseDownPlayMedia(object sender, MouseButtonEventArgs args)
        {
            // The Play method will begin the media if it is not currently active or 
            // resume media if it is paused. This has no effect if the media is
            // already running.
            myMediaElement.Play();
            ImagePlayMedia.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/UI_play_Yellow.png"));
            // Initialize the MediaElement property values.
            InitializePropertyValues();
        }
        private void OnMouseUpPlayMedia(object sender, MouseButtonEventArgs e)
        {
            ImagePlayMedia.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/UI_play.png"));
        }
        // Pause the media.
        void OnMouseDownPauseMedia(object sender, MouseButtonEventArgs args)
        {
            // The Pause method pauses the media if it is currently running.
            // The Play method can be used to resume.
            myMediaElement.Pause();
            ImagePauseMedia.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/UI_pause_Yellow.png"));
        }
        private void OnMouseUpPauseMedia(object sender, MouseButtonEventArgs e)
        {
            ImagePauseMedia.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/UI_pause.png"));
        }
        // Stop the media.
        void OnMouseDownStopMedia(object sender, MouseButtonEventArgs args)
        {
            // The Stop method stops and resets the media to be played from
            // the beginning.
            myMediaElement.Stop();
            timelineSlider.Value = 0;
            dispatcherTimer.Stop();
            ImageStopMedia.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/UI_stop_Yellow.png"));
        }
        private void OnMouseUpStopMedia(object sender, MouseButtonEventArgs e)
        {
            ImageStopMedia.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/UI_stop.png"));
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
                    timelineSlider.SmallChange = 1;
                    timelineSlider.LargeChange = Math.Min(10, myMediaElement.NaturalDuration.TimeSpan.Seconds / 10);

                    CurrentPosition.Text = String.Format("00:00:00");
                    Duration.Text = String.Format("{0:00}:{1:00}:{2:00}",
                        myMediaElement.NaturalDuration.TimeSpan.Hours,
                        myMediaElement.NaturalDuration.TimeSpan.Minutes,
                        myMediaElement.NaturalDuration.TimeSpan.Seconds);
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
            timelineSlider.Value = 0;
        }

        bool isDragging = false;

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


    }
}
