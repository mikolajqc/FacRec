using Accord.Imaging.Filters;
using Accord.Vision.Detection;
using Accord.Vision.Detection.Cascades;
using Accord.Math;
using System.Drawing;
using System;
using System.Collections.Generic;
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
using AForge.Video.DirectShow;
using Accord.Video;
using AForge;
using Microsoft.Win32;
using System.IO;
using Accord.Imaging.Formats;

namespace Inzynierka
{

    public partial class MainWindow : Window
    {
        private bool isEqualizerActive = false;
        VideoCaptureDevice videoSource = null;
        FilterInfoCollection videoDevices = null;
        OpenFileDialog openfiledialog = new OpenFileDialog();
        Bitmap currentPicture = null;
        int counter = 0;


        public MainWindow()
        {
            InitializeComponent();

        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
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

        private void Button_Click_Init(object sender, RoutedEventArgs e)
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            // create video source
            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            // set NewFrame event handler
            videoSource.NewFrame += new AForge.Video.NewFrameEventHandler(
                (s, eventArgs)
                =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        if(counter%10 == 0)
                        {
                            Bitmap currentPicture = GetBitmapWithDetectedFace(eventArgs.Frame);
                            if(isEqualizerActive)
                            {
                                HistogramEqualization histogramEqualization = new HistogramEqualization();
                                currentPicture = histogramEqualization.Apply(currentPicture);
                            }
                            image.Source = BitmapToImageSource(currentPicture);
                        }
                        ++counter;
                        
                    });
                }

                );
        }

        private void Button_Click_Start(object sender, RoutedEventArgs e)
        {
            videoSource.Start();
        }
        private void Button_Click_Stop(object sender, RoutedEventArgs e)
        {
            videoSource.SignalToStop();
        }

        private Bitmap GetBitmapWithDetectedFace(Bitmap source)
        {
            HaarCascade cascade = new FaceHaarCascade();
            var detector = new HaarObjectDetector(cascade, 30);

            System.Drawing.Rectangle[] objects = detector.ProcessFrame(source);

            RectanglesMarker marker = new RectanglesMarker(objects, System.Drawing.Color.Fuchsia);

            return marker.Apply(source);
        }

        private void Button_Click_Send(object sender, RoutedEventArgs e)
        {
            LearingTest();
        }

        private void LearingTest()
        {
            float[,] allVectors = LearningSetLoader.GetImagesAsVectorsFromDirectory(@"C:\Users\mikolaj.ciesielski\Desktop\Studia\Inżynierka\Databases\AT&T");
            float[] averageVector = LearningSetLoader.GetAverageFaceVector(allVectors);
            Bitmap averageImage = Tools.CreateBitMapFromBytes(averageVector, 92, 112);
            float[,] diffVectors = LearningSetLoader.GetDifferenceVectors(averageVector, allVectors);

            float[,]transposedDiffVectors = Accord.Math.Matrix.Transpose(diffVectors);

        /*
            Bitmap diffExampleImage = Tools.CreateBitMapFromBytes(Tools.GetVectorFromTable(diffVectors, 0), 92, 112);

            
            Dispatcher.Invoke(() =>
            {
                    image.Source = BitmapToImageSource(diffExampleImage);
            });
        */
            
        }

        /*
        private void WriteAllBytes(Bitmap bitmap)
        {

            List<byte> imageInVector = new List<byte>();

            for (int i = 0; i < bitmap.Size.Width; ++i)
            {
                for (int j = 0; j < bitmap.Size.Height; ++j)
                {
                    System.Drawing.Color color = bitmap.GetPixel(i, j);

                    int r = color.R;
                    int g = color.G;
                    int b = color.B;
                    int grayscale = (r + g + b) / 3;

                    imageInVector.Add((byte)grayscale);
                }
                Console.WriteLine(imageInVector.Count);
            }
        }
        */

        private void Button_Click_Equalizer(object sender, RoutedEventArgs e)
        {
            isEqualizerActive = !isEqualizerActive;
        }
    }


}
