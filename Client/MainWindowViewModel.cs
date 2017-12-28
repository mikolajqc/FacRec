using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Accord.Imaging.Filters;
using Caliburn.Micro;
using Client.Utilities;
using Commons;

namespace Client
{
    //todo: ResizeNearestNeighbor do skalowania? z bilbioteki AForge
    //todo: usunac requestowanie o learna w zwykly sposob
    //todo: lista dostepnych urzadzen video
    class MainWindowViewModel : Screen
    {
        ///TODO: requests manager
        ///TODO: lustrzane odbicie
        #region fields
        ///Sprawdzic czy tutaj musi byc BitmapImage czy moze byc Bitmap
        private BitmapImage _imageWebcam;
        private BitmapImage _imageSnapshot;
        private CameraManager _cameraManager;
        private FaceDetector _faceDetector;
        private RequestManager _requestManager;

        private string _nameOfUser;

        private System.Timers.Timer _timer;
        #endregion

        #region properties

        public BitmapImage ImageWebcam
        {
            get
            {
                return _imageWebcam;
            }
            set
            {
                _imageWebcam = value;
                NotifyOfPropertyChange(() => ImageWebcam);
            }
        }

        public BitmapImage ImageSnapshot
        {
            get
            {
                return _imageSnapshot;
            }

            set
            {
                _imageSnapshot = value;
                NotifyOfPropertyChange(() => ImageSnapshot);
            }
        }

        public string NameOfUser
        {
            get
            {
                return _nameOfUser;
            }
            set
            {
                _nameOfUser = value;
                NotifyOfPropertyChange(() => NameOfUser);
            }
        }

        #endregion

        #region publicmethods

        public void Snapshot()
        {
            Application.Current.Dispatcher.BeginInvoke(
            new System.Action(
                () =>
                {
                    var bitmap = _faceDetector.GetBitmapWithDetectedFace(_cameraManager.GetFramePreview()).Item2;
                    if (bitmap != null) _imageSnapshot = BitmapToImageSource(bitmap);
                    NotifyOfPropertyChange(() => ImageSnapshot);
                }));
        }
        public async void Learn()
        {
            MessageBox.Show("Learnt!");
        }
        public async void Recognize()
        {
            string result = await _requestManager.Recognize(BitmapImage2Bitmap(ImageSnapshot));
            MessageBox.Show(result);
        }

        public async void AddFace()
        {
            string result = await _requestManager.AddFace(BitmapImage2Bitmap(ImageSnapshot), _nameOfUser);
            MessageBox.Show(result);
        }

        #endregion

        #region ovverriddenmethods

        protected override void OnActivate()
        {
            _cameraManager = new CameraManager();
            _faceDetector = new FaceDetector();
            _requestManager = new RequestManager();

            _timer = new System.Timers.Timer
                {
                    AutoReset = true,
                    Interval = 50
                };
            // ogarnac to inaczej
            _timer.Elapsed += (sender, e) =>
                {
                    UpdateImage();
                };
            _cameraManager.Start();
            _timer.Start();
        }

        protected override void OnDeactivate(bool close)
        {
            _timer.Stop();
            _cameraManager.Stop();
            base.OnDeactivate(close);
        }

        #endregion
        

        #region privatemethods

        private void UpdateImage()
        {
            if(_cameraManager.GetFramePreview() != null)
            {
                Application.Current.Dispatcher.BeginInvoke(
                    new System.Action(
                        () => {
                                _imageWebcam = BitmapToImageSource(
                                    _faceDetector.GetBitmapWithDetectedFace(_cameraManager.GetFramePreview()).Item1
                                );
                            NotifyOfPropertyChange(() => ImageWebcam);
                        }));

            }
        }


        ///todo: Funkcje externalowe przerobic na swoj kod i moze przenisc do jakichs tools? jako static

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
        
        /// <summary>
        /// External
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <returns></returns>
        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        private Bitmap ApplyRectangleToBitmap(Bitmap source)
        {
            Rectangle rectangle = CreateRectangleForFace(source.Width, source.Height);
            RectanglesMarker marker = new RectanglesMarker(rectangle);

            return marker.Apply(source);
        }

        private Rectangle CreateRectangleForFace(int bitmapWidth, int bitmapHeight)
        {
            const int widthofimagetosent = 92 * 4;
            const int heightofimagetosent = 112 * 4;

            int x = (bitmapWidth - widthofimagetosent) / 2;
            int y = (bitmapHeight - heightofimagetosent) / 2;

            return new Rectangle(x, y, widthofimagetosent, heightofimagetosent);
        }
        #endregion

    }
}
