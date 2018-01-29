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
        public LegendViewerWindow()
        {
            InitializeComponent();

            // init language
            var lang = new LanguageSupport();
            lang.SetLegendWindowLanguage(this);

            // we prepare the image
            var imagepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources/ypower-legend.png");
            var _source = new Uri(imagepath);
            var _bi = new BitmapImage();
            _bi.BeginInit();
            _bi.UriSource = _source;
            _bi.EndInit();

            // we set the image
            pictureBox1.Source = _bi;
        }
    }
}
