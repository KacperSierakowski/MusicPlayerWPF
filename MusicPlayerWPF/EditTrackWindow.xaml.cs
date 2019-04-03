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
using System.Windows.Shapes;

namespace MusicPlayerWPF
{
    /// <summary>
    /// Interaction logic for EditTrackWindow.xaml
    /// </summary>
    public partial class EditTrackWindow : Window
    {
        String TrackFilePath;
        String CoverFilePath;

        public EditTrackWindow()
        {
            InitializeComponent();
        }

        void OnMouseDownAccept(object sender, MouseButtonEventArgs args)
        {
            ImageAcceptButton.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/UI_accept_Yellow.png"));
            TrackFilePath = FilePathBlock.Text.ToString();
            ChangeMetaDataOfATrack();
            this.Close();
        }
        private void OnMouseUpAccept(object sender, MouseButtonEventArgs e)
        {
            ImageAcceptButton.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/UI_accept.png"));
        }
        void ChangeMetaDataOfATrack()
        {
            TagLib.File tagFile2 = TagLib.File.Create(TrackFilePath);
            try
            {
                tagFile2.Tag.Performers[0] = Artist.Text.ToString();
                tagFile2.Tag.Title = TrackTitle.Text.ToString();
                tagFile2.Tag.Album = AlbumTitle.Text.ToString();
                tagFile2.Save();
                ((MainWindow)Application.Current.MainWindow).Artist.Text = Artist.Text;
                ((MainWindow)Application.Current.MainWindow).AlbumTitle.Text = AlbumTitle.Text;
                ((MainWindow)Application.Current.MainWindow).TrackTitle.Text = TrackTitle.Text;
            }
            catch (Exception ee)
            {
                string errorData = "Błąd przy dodawaniu danych artysty,nazwy albumu i nazwy piosenki" + ee.Data.ToString();
                MessageBox.Show(errorData);
            }
            //try
            //{
            //    TagLib.Picture pic = new TagLib.Picture
            //    {
            //        Type = TagLib.PictureType.FrontCover,
            //        Description = "Cover",
            //        MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg
            //    };
            //    MemoryStream ms = new MemoryStream();
            //    System.Drawing.Image image = System.Drawing.Image.FromFile(CoverFilePath);
            //    image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            //    ms.Position = 0;
            //    pic.Data = TagLib.ByteVector.FromStream(ms);
            //    tagFile2.Tag.Pictures = new TagLib.IPicture[] { pic };

            //}
            //catch (Exception ee)
            //{
            //    TrackCover.Source = new BitmapImage(new Uri("O:/Pas Oriona/Kariera/repos/MusicPlayerWPF/MusicPlayerWPF/Images/TrackCoverUknown.png"));
            //    string errorNoCover = "Nie udało się wgrać okładki" + ee.Data.ToString();
            //    MessageBox.Show(errorNoCover);
            //}
        }
        //private void Cover_Drop(object sender, DragEventArgs e)
        //{
        //    try
        //    {
        //        String[] FileName = (String[])e.Data.GetData(DataFormats.FileDrop, true);

        //        if (FileName.Length > 0)
        //        {
        //            CoverFilePath = FileName[0].ToString();
        //            try
        //            {
        //                BitmapImage image = new BitmapImage();
        //                image.BeginInit();
        //                image.UriSource = new Uri(CoverFilePath, UriKind.Absolute);
        //                image.EndInit();
        //                TrackCover.Source = image;
        //            }
        //            catch (Exception ee)
        //            {
        //                string errorNoCover = ee.Data.ToString();
        //                MessageBox.Show("Something wrog with the cover!");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}
    }
}
