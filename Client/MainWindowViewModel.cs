using Caliburn.Micro;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Client
{
    class MainWindowViewModel : Screen
    {
        #region fields

        private BitmapImage imageWebcam = null;
        private BitmapImage imageSnapshot = null;
        private CameraManager cameraManager = null;
        private System.Timers.Timer timer = null;

        #endregion

        #region properties

        public BitmapImage ImageWebcam
        {
            get
            {
                return imageWebcam;
            }
            set
            {
                imageWebcam = value;
                NotifyOfPropertyChange(() => ImageWebcam);
            }
        }

        public BitmapImage ImageSnapshot
        {
            get
            {
                return imageSnapshot;
            }

            set
            {
                imageSnapshot = value;
                NotifyOfPropertyChange(() => ImageSnapshot);
            }
        }

        #endregion

        #region publicmethods

        public void Recognize()
        {
            Application.Current.Dispatcher.BeginInvoke(
            new System.Action(
                () => {
                    imageSnapshot = ImageWebcam;
                    NotifyOfPropertyChange(() => ImageSnapshot);
                }));
        }
        public void Learn()
        {
        }
        public void AddFace()
        {
        }

        #endregion

        #region ovverriddenmethods

        protected override void OnActivate()
        {
            cameraManager = new CameraManager();
            timer = new System.Timers.Timer();
            timer.AutoReset = true;
            timer.Interval = 20; // ogarnac to inaczej
            timer.Elapsed += (sender, e) =>
            {
                UpdateImage();
            };
            cameraManager.Start();
            timer.Start();
        }

        protected override void OnDeactivate(bool close)
        {
            timer.Stop();
            cameraManager.Stop();
            base.OnDeactivate(close);
        }

        #endregion


        #region privatemethods

        private void UpdateImage()
        {
            if(cameraManager.GetFrame() != null)
            {
                Application.Current.Dispatcher.BeginInvoke(
                new System.Action(
                    () => {
                        imageWebcam = BitmapToImageSource(new Bitmap(cameraManager.GetFrame()));
                        NotifyOfPropertyChange(() => ImageWebcam);
                    }));
            }
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

        #endregion

    }
}
