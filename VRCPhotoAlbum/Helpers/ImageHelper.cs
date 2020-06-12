﻿using KoyashiroKohaku.VrcMetaTool;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Gatosyocora.VRCPhotoAlbum.Helpers
{
    public class ImageHelper
    {
        private static BitmapImage _failedImage => LoadBitmapImage(@"pack://application:,,,/Resources/failed.png");

        #region BitmapImage
        public static BitmapImage LoadBitmapImage(string filePath)
        {
            BitmapImage bitmapImage = new BitmapImage();
            if (filePath.StartsWith(@"pack://application:,,,"))
            {
                var streamInfo = Application.GetResourceStream(new Uri(filePath));
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = streamInfo.Stream;
                bitmapImage.EndInit();
            }
            else
            {
                using (var stream = File.OpenRead(filePath))
                {
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = stream;
                    bitmapImage.EndInit();
                }
            }

            return bitmapImage;
        }

        public static string GetThumbnailImagePath(string filePath, string cacheFolderPath)
        {
            return $"{cacheFolderPath}/{Path.GetFileNameWithoutExtension(filePath)}.jpg";
        }

        public static BitmapImage GetThumbnailImage(string filePath, string cacheFolderPath)
        {
            var thumbnailImageFilePath = GetThumbnailImagePath(filePath, cacheFolderPath);

            if (!File.Exists(thumbnailImageFilePath))
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var originalImage = Image.FromStream(stream, false, false);
                    var thumbnailImage = originalImage.GetThumbnailImage(originalImage.Width / 8, originalImage.Height / 8, () => { return false; }, IntPtr.Zero);
                    thumbnailImage.Save(thumbnailImageFilePath, ImageFormat.Jpeg);
                    originalImage.Dispose();
                    thumbnailImage.Dispose();
                }
            }
            return LoadBitmapImage(thumbnailImageFilePath);
        }

        public static async Task CreateThumbnailImagePathAsync(string filePath, string cacheFolderPath)
        {
            var thumbnailImageFilePath = GetThumbnailImagePath(filePath, cacheFolderPath);

            await Task.Run(() =>
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var originalImage = Image.FromStream(stream, false, false);
                    var thumbnailImage = originalImage.GetThumbnailImage(originalImage.Width / 8, originalImage.Height / 8, () => { return false; }, IntPtr.Zero);
                    thumbnailImage.Save(thumbnailImageFilePath, ImageFormat.Jpeg);
                    originalImage.Dispose();
                    thumbnailImage.Dispose();
                }
            });
        }
        #endregion

        #region Bitmap
        public static Bitmap LoadImage(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"{filePath} is not found.");
            }

            return new Bitmap(filePath);
        }

        public static void SaveImage(Bitmap image, string filePath)
        {
            if (image is null)
            {
                throw new ArgumentNullException("image is null");
            }
            using (image)
            {
                image.Save(filePath, ImageFormat.Png);
            }
        }

        public static void SaveImage(byte[] imageBuffer, string filePath)
        {
            File.WriteAllBytes(filePath, imageBuffer);
        }
        #endregion

        #region Convert
        public static Bitmap Bytes2Bitmap(byte[] buffer)
        {
            using (var ms = new MemoryStream(buffer))
            {
                var bitmap = new Bitmap(ms);
                ms.Close();
                return bitmap;
            }
        }

        public static byte[] Bitmap2Bytes(Bitmap bitmap)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(bitmap, typeof(byte[]));
        }
        #endregion

        #region ImageProcessing

        public static Bitmap RotateLeft90(string filePath)
        {
            var image = LoadImage(filePath);
            return RotateLeft90(image);
        }

        public static Bitmap RotateRight90(string filePath)
        {
            var image = LoadImage(filePath);
            return RotateRight90(image);
        }

        public static Bitmap RotateLeft90(Bitmap image)
        {
            image.RotateFlip(RotateFlipType.Rotate270FlipNone);
            return image;
        }

        public static Bitmap RotateRight90(Bitmap image)
        {
            image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            return image;
        }

        public static Bitmap FlipHorizontal(Bitmap image)
        {
            image.RotateFlip(RotateFlipType.Rotate180FlipY);
            return image;
        }

        #endregion

        public static void RotateLeft90AndSave(string filePath, VrcMetaData metaData)
        {
            var image = RotateLeft90(LoadImage(filePath));
            var buffer = Bitmap2Bytes(image);
            if (!(metaData is null)) buffer = VrcMetaDataWriter.Write(buffer, metaData);
            SaveImage(buffer, filePath);
        }

        public static void RotateRight90AndSave(string filePath, VrcMetaData metaData)
        {
            var image = RotateRight90(LoadImage(filePath));
            var buffer = Bitmap2Bytes(image);
            if (!(metaData is null)) buffer = VrcMetaDataWriter.Write(buffer, metaData);
            SaveImage(buffer, filePath);
        }

        public static void FilpHorizontalAndSave(string filePath, VrcMetaData metaData)
        {
            var image = FlipHorizontal(LoadImage(filePath));
            var buffer = Bitmap2Bytes(image);
            if (!(metaData is null)) buffer = VrcMetaDataWriter.Write(buffer, metaData);
            SaveImage(buffer, filePath);
        }
    }
}
