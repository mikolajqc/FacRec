﻿using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace Client.Utilities
{
    public static class Tools
    {
        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                if (bitmap == null) return null;
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                var bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        public static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (var outStream = new MemoryStream())
            {
                if (bitmapImage == null) return null;
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                var bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
    }
}
