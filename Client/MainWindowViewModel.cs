using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Client
{
    class MainWindowViewModel : PropertyChangedBase
    {
        private BitmapImage imageWebcam = null;
        private CameraManager cameraManager = null;
        private System.Timers.Timer timer = null;//to do

        public void Recognize()
        {
            cameraManager.Start();
        }
        public void Learn()
        {
            cameraManager.Stop();
        }
        public void AddFace()
        {
            UpdateImage();
        }

        public BitmapImage ImageWebcam
        {
            get
            {
                return this.imageWebcam;
            }
            set
            {
                this.imageWebcam = value;
                this.NotifyOfPropertyChange(() => this.ImageWebcam);
            }
        }

        
        public MainWindowViewModel()
        {
            cameraManager = new CameraManager();
            timer = new System.Timers.Timer();
            timer.AutoReset = true;
            timer.Interval = 100;
            timer.Elapsed += (sender, e) => { UpdateImage(); };
        }
        
        private void UpdateImage()
        {
            ImageWebcam = BitmapToImageSource(cameraManager.GetFrame());
        }


        /// <summary>
        /// External code!!!
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
