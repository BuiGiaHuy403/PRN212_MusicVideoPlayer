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

namespace MusicPlayList
{
    /// <summary>
    /// Interaction logic for SpotifyWindow.xaml
    /// </summary>
    public partial class SpotifyWindow : Window
    {
        public SpotifyWindow()
        {
            InitializeComponent();
            LoadSpotifyPlayer();

        }
        private async void LoadSpotifyPlayer()
        {
            var auth = new SpotifyAuth();
            string accessToken = await auth.GetAccessTokenAsync();

            // Truyền Access Token vào file HTML
            string htmlPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "spotify-html.html");
            string htmlContent = File.ReadAllText(htmlPath).Replace("{ACCESS_TOKEN}", accessToken);
            File.WriteAllText(htmlPath, htmlContent);

            WebView.Source = new Uri(htmlPath);
        }
    }
}
