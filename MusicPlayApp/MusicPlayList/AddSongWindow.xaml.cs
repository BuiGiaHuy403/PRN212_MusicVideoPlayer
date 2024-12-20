﻿using Microsoft.Win32;
using MusicPlayApp.BLL.Service;
using MusicPlayApp.DAL.Entities;
using MusicPlayApp.DAL.Repository;
using System.Windows;
using TagLib;

namespace MusicPlayList
{
    public partial class AddSongWindow : Window
    {
        private SongService _songService;
        public Song SelectedOne { get; set; }
        public List<Song> SelectedSongs { get; set; } = new List<Song>(); 

        public AddSongWindow()
        {
            InitializeComponent();
            _songService = new SongService(new SongRepository());
            if(SelectedOne != null)
            {
                TitleTextBox.Text = SelectedOne.Title;
                ArtistTextBox.Text = SelectedOne.Artist;
                AlbumTextBox.Text = SelectedOne.Album;
            }
            SongsListBox.Visibility = Visibility.Hidden;

        }

        public async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the input values
            string title = TitleTextBox.Text;
            string artist = ArtistTextBox.Text;
            string albumFilePath = AlbumTextBox.Text;

            if (SongsListBox.SelectedItem is Song selectedSong)
            {
                selectedSong.Title = title;
                selectedSong.Artist = artist;
                selectedSong.Album = albumFilePath;
                SongsListBox.SelectedItem = null;
                return;
            }

            if (SelectedOne != null)
            {
                // Update the song details
                SelectedOne.Title = TitleTextBox.Text;
                SelectedOne.Artist = ArtistTextBox.Text;
                SelectedOne.Album = AlbumTextBox.Text;

                await _songService.UpdateSongAsync(SelectedOne);
            }
            else if (SelectedSongs.Count > 1)
            {
                foreach (var song in SelectedSongs)
                {
                    await _songService.AddSongAsync(song);
                }
            }
            else
            {
                // Create a new Song object
                var newSong = new Song
                {
                    Title = title.Split('-')[0],
                    Artist = artist,
                    Album = albumFilePath
                };
                await _songService.AddSongAsync(newSong);
            }

            // Close the window
            this.DialogResult = true;
            this.Close();
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the window without saving
            this.DialogResult = false;
            this.Close();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Open a file dialog to select multiple album file paths
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.mp3;*.mp4;*.wav;*.flac|All Files|*.*",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var fileName in openFileDialog.FileNames)
                {
                    var file = TagLib.File.Create(fileName);
                    string artist = file.Tag.FirstPerformer ?? "Unknown Artist";
                    var newSong = new Song
                    {
                        //Title = System.IO.Path.GetFileNameWithoutExtension(fileName),
                        Title = file.Tag.Title,
                        Artist = artist,
                        Album = fileName
                    };
                    SelectedSongs.Add(newSong);
                }

                // Update the UI to reflect the selected songs
                // For example, you can display the selected songs in a ListBox
                SongsListBox.ItemsSource = null;
                SongsListBox.ItemsSource = SelectedSongs;
                SongsListBox.DisplayMemberPath = "Title";
                if (SelectedSongs.Count > 1)
                {
                    TitleLabel.Visibility = Visibility.Hidden;
                    TitleTextBox.Text = null;
                    TitleTextBox.Visibility = Visibility.Hidden;
                    ArtistLabel.Visibility = Visibility.Hidden;
                    ArtistTextBox.Text = null;
                    ArtistTextBox.Visibility = Visibility.Hidden;
                    SongsListBox.Visibility = Visibility.Visible;
                }
                else
                {
                    var file = TagLib.File.Create(openFileDialog.FileName);
                    string artist = file.Tag.FirstPerformer ?? "Unknown Artist";
                    //TitleTextBox.Text = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    TitleTextBox.Text = file.Tag.Title;
                    ArtistTextBox.Text = SelectedSongs.First().Artist;
                    AlbumTextBox.Text = SelectedSongs.First().Album;
                }
            }
        }

        private void SongsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (SongsListBox.SelectedItem is Song selectedSong)
            {
                // Hiển thị thông tin bài hát đã chọn
                TitleTextBox.Text = selectedSong.Title;
                ArtistTextBox.Text = selectedSong.Artist;
                AlbumTextBox.Text = selectedSong.Album;

                // Cập nhật biến SelectedOne để xử lý update
                //SelectedOne = selectedSong;

                // Hiện các textbox nếu chúng bị ẩn
                TitleLabel.Visibility = Visibility.Visible;
                TitleTextBox.Visibility = Visibility.Visible;
                ArtistLabel.Visibility = Visibility.Visible;
                ArtistTextBox.Visibility = Visibility.Visible;
                AlbumTextBox.Visibility = Visibility.Visible;
            }
        }
    }
}
