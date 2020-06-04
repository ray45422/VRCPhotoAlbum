﻿using System;
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
using System.Drawing;
using Image = System.Drawing.Image;
using Gatosyocora.VRCPhotoAlbum.Models;
using KoyashiroKohaku.VrcMetaToolSharp;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Gatosyocora.VRCPhotoAlbum.ViewModel;

namespace Gatosyocora.VRCPhotoAlbum
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_OnLoaded;
        }

        private void MainWindow_OnLoaded(object sender, EventArgs args)
        {
            _mainViewModel = new MainViewModel();
            DataContext = _mainViewModel;
            PhotoListBox.ItemsSource = _mainViewModel.ShowedPhotoList;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _mainViewModel.SearchPhotoWithUserName(_mainViewModel.SearchText);
        }
    }
}
