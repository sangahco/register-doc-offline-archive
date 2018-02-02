using pmis.i18n;
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

namespace pmis.legendchart
{
    /// <summary>
    /// Interaction logic for ViewerWindow.xaml
    /// </summary>
    public partial class LegendViewerWindow : Window
    {
        private PictureViewerService picService;

        public LegendViewerWindow()
        {
            InitializeComponent();

            // init language
            var lang = new LanguageSupport();
            lang.SetLegendWindowLanguage(this);

            // we prepare the image
            //var imagepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources/ypower-legend.png");
            //var _source = new Uri(imagepath);
            //var _bi = new BitmapImage();
            //_bi.BeginInit();
            //_bi.UriSource = _source;
            //_bi.EndInit();

            var imagepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources/legend");
            picService = new PictureViewerService();
            picService.OnPictureSelected += handlePictureLoadedEvent;
            picService.LoadImageList(imagepath);

            // we set the image
            pictureBox1.Source = picService.NextImage();
        }

        private void picturePreviousButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pictureBox1.Source = picService.PreviousImage();
            }
            catch (Exception ex)
            {
                ex.Log().Display();
            }
        }

        private void pictureNextButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pictureBox1.Source = picService.NextImage();
            }
            catch (Exception ex)
            {
                ex.Log().Display();
            }
        }

        private void handlePictureLoadedEvent(object sender, RegisterFile image)
        {
            try
            {
                imageName.Content = image.FileName;
            }
            catch (Exception ex)
            {
                ex.Log().Display();
            }
        }
    }
}
