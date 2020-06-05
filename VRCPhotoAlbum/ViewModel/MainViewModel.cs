﻿using Gatosyocora.VRCPhotoAlbum.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;
using System.Drawing;
using KoyashiroKohaku.VrcMetaToolSharp;
using System.Windows.Controls;
using Image = System.Drawing.Image;
using System.Windows.Media.Imaging;
using System.Text.Json;
using System.Reflection;
using System.Windows;

namespace Gatosyocora.VRCPhotoAlbum.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Photo> ShowedPhotoList = new ObservableCollection<Photo>();
        private List<Photo> _photoList;

        public List<string> UserList { get; }

        private string _searchText = string.Empty;
        public string SearchText { get { return _searchText; }
            set
            {
                _searchText = value;
                SearchTextPropertyChanged(nameof(SearchText));
                SearchPhotoWithUserName(_searchText);
            }
        }
        private void SearchTextPropertyChanged(string propertyName) => PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            var executeFolderPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var jsonFilePath = executeFolderPath + @"/settings.json";
            SettingData settingData;
            if (File.Exists(jsonFilePath))
            {
                settingData = LoadSettingDataFromJsonFile(jsonFilePath);
            }
            else
            {
                settingData = CreateSettingData();
            }

            try
            {
                _photoList = LoadVRCPhotoList(settingData.FolderPath);
                UserList = GetSortedUserList(_photoList);

                foreach (var photo in _photoList)
                {
                    ShowedPhotoList.Add(photo);
                }
            }
            catch (Exception e)
            {
                Debug.Print($"{e.GetType().Name}: {e.Message}");
            }
        }

        private List<Photo> LoadVRCPhotoList(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                throw new ArgumentException($"{folderPath} is not exist.");
            }

            return Directory.GetFiles(folderPath, "*.png", SearchOption.AllDirectories)
                        .Select(x =>
                        {
                            var source = new BitmapImage();
                            source.BeginInit();
                            source.UriSource = new Uri(x);
                            source.EndInit();
                            return new Photo
                            {
                                FilePath = x,
                                OriginalImage = source,
                                MetaData = VrcMetaDataReader.Read(x)
                            };
                        })
                        .ToList();
        }

        public void SearchPhotoWithUserName(string searchedUserName)
        {
            ShowedPhotoList.Clear();
            var searchedPhotoList = _photoList
                    .Where(x => x.MetaData.Users.Any(u => u.UserName.StartsWith(searchedUserName)))
                    .ToList();

            foreach (var photo in searchedPhotoList)
            {
                ShowedPhotoList.Add(photo);
            }
        }

        private List<string> GetSortedUserList(List<Photo> photoList)
        {
            return photoList
                        .SelectMany(x => x.MetaData.Users)
                        .Select(u => u.UserName)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList();
        }

        private SettingData CreateSettingData()
        {
            return new SettingData
            {
                FolderPath = @"D:\VRTools\vrc_meta_tool\meta_pic"
            };
        }

        private SettingData LoadSettingDataFromJsonFile(string path)
        {
            return JsonSerializer.Deserialize(File.ReadAllText(path), typeof(SettingData)) as SettingData;
        }
    }
}
