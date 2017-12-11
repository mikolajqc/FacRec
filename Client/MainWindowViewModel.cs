using Accord.Imaging.Filters;
using Caliburn.Micro;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using Commons;

namespace Client
{
    class MainWindowViewModel : Screen
    {
        ///TODO:
        ///1. requests manager
        ///
        ///Dodaj lustrzane odbicie
        #region fields
        ///Sprawdzic czy tutaj musi byc BitmapImage czy moze byc Bitmap
        private BitmapImage imageWebcam = null;
        private BitmapImage imageSnapshot = null;
        private CameraManager cameraManager = null;
        private string nameOfUser = null;

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

        public string NameOfUser
        {
            get
            {
                return nameOfUser;
            }
            set
            {
                nameOfUser = value;
                NotifyOfPropertyChange(() => NameOfUser);
            }
        }

        #endregion

        #region publicmethods

        public void Snapshot()
        {
            Application.Current.Dispatcher.BeginInvoke(
            new System.Action(
                () => {
                    imageSnapshot = BitmapToImageSource(CropImage(BitmapImage2Bitmap(ImageWebcam), CreateRectangleForFace(ImageWebcam.PixelWidth, ImageWebcam.PixelHeight)));
                    NotifyOfPropertyChange(() => ImageSnapshot);
                }));
        }
        public async void Learn()
        {
            //await Task.Run(() => faceRecognition.Learn());
            MessageBox.Show("Learnt!");
        }
        public async void Recognize()
        {
            //MessageBox.Show(faceRecognition.Recognize(BitmapImage2Bitmap(ImageSnapshot)));
            string result = await UploadBitmapAsync(BitmapImage2Bitmap(ImageSnapshot));
            MessageBox.Show(result);
        }

        public async void AddFace()
        {
            string result = await UploadBitmapAsync(BitmapImage2Bitmap(ImageSnapshot), true, nameOfUser);
            MessageBox.Show(result);
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
                        imageWebcam = BitmapToImageSource(
                            ApplyRectangleToBitmap(
                                cameraManager.GetFrame()
                            ));
                        NotifyOfPropertyChange(() => ImageWebcam);
                    }));
            }
        }


        ///Funkcje externalowe przerobic na swoj kod i moze przenisc do jakichs tools? jako static

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

        /// <summary>
        /// External!!!
        /// </summary>
        /// <param name="source"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        private Bitmap CropImage(Bitmap source, Rectangle section)
        {
            // An empty bitmap which will hold the cropped image
            Bitmap bmp = new Bitmap(section.Width, section.Height);

            Graphics g = Graphics.FromImage(bmp);

            // Draw the given area (section) of the source image
            // at location 0,0 on the empty bitmap (bmp)
            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);

            return bmp;
        }

        private Rectangle CreateRectangleForFace(int bitmapWidth, int bitmapHeight)
        {
            const int WIDTHOFIMAGETOSENT = 92 * 4;
            const int HEIGHTOFIMAGETOSENT = 112 * 4;

            int x = (bitmapWidth - WIDTHOFIMAGETOSENT) / 2;
            int y = (bitmapHeight - HEIGHTOFIMAGETOSENT) / 2;

            return new Rectangle(x, y, WIDTHOFIMAGETOSENT, HEIGHTOFIMAGETOSENT);
        }
        #endregion

    /// <summary>
    /// Czesciowo external, upewnic sie czy nie lepiej wysylac w jakims base64 !!!!!, funkcja do poprawienia i wrzucenia w RequestManager or sth
    /// </summary>
    /// <param name="bitmap"></param>
    /// <returns></returns>
    public async Task<string> UploadBitmapAsync(Bitmap bitmap, bool isAddingNewFace = false, string name = null)
        {
            byte[] bitmapData;
            var stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            bitmapData = stream.ToArray();

            var client = new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:15390")
            };
            // Set the Accept header for BSON.
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/bson"));

            var request = new Request
            {
                Name = name,
                BitmapInArray = bitmapData
            };

            MediaTypeFormatter bsonFormatter = new BsonMediaTypeFormatter();
            HttpResponseMessage response;
            if (isAddingNewFace) response = await client.PostAsync("/api/FaceRecognition/AddFace", request, bsonFormatter);
            else response = await client.PostAsync("/api/FaceRecognition/Recognize", request, bsonFormatter);

            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            return result;
        }

    }
}
