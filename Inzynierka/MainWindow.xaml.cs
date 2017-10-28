using Accord.Imaging.Filters;
using Accord.Vision.Detection;
using Accord.Vision.Detection.Cascades;
using System.Drawing;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using AForge.Video.DirectShow;
using Microsoft.Win32;
using System.IO;
using Accord.Math.Decompositions;

namespace Inzynierka
{

    public partial class MainWindow : Window
    {
        private bool isEqualizerActive = false;
        VideoCaptureDevice videoSource = null;
        FilterInfoCollection videoDevices = null;
        OpenFileDialog openfiledialog = new OpenFileDialog();
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
            double[,] allVectors = LearningSetLoader.GetImagesAsVectorsFromDirectory(@"C:\Users\mikolaj.ciesielski\Desktop\Studia\Inżynierka\Databases\AT&T");
            double[,] allVectorsT = Accord.Math.Matrix.Transpose(allVectors);

            double[] averageVector = LearningSetLoader.GetAverageFaceVector(allVectors);
            Bitmap averageImage = Tools.CreateBitMapFromBytes(averageVector, 92, 112);
            double[,] diffVectors = LearningSetLoader.GetDifferenceVectors(averageVector, allVectors);

            double[,] diffVectorsT = Accord.Math.Matrix.Transpose(diffVectors);
            double[,] covariation = Accord.Math.Matrix.Dot(diffVectors, diffVectorsT);

            EigenvalueDecomposition decomposition = new EigenvalueDecomposition(covariation, true, true);

            double[,] eigenVectors = decomposition.Eigenvectors; // 400x400
            double[,] eigenVectorsT = Accord.Math.Matrix.Transpose(eigenVectors);

            double[,] eigenFaces = Accord.Math.Matrix.Dot(diffVectorsT, eigenVectors); // matrix to calculation 10304x400
            double[,] eigenFacesT = Accord.Math.Matrix.Transpose(eigenFaces);


            Console.WriteLine("All: " + allVectors.GetLength(0) + " x " + allVectors.GetLength(1));
            Console.WriteLine("Diff: " + diffVectors.GetLength(0) + " x " + diffVectors.GetLength(1));
            Console.WriteLine("Cov: " + covariation.GetLength(0) + " x " + covariation.GetLength(1));
            Console.WriteLine("EigenVectors: " + eigenVectors.GetLength(0) + " x " + eigenVectors.GetLength(1));
            Console.WriteLine("EigenFaces: " + eigenFaces.GetLength(0) + " x " + eigenFaces.GetLength(1));

            //   PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis(eigenVectors);
            //   pca.Compute();

            Console.WriteLine("Done computing");
            double[,] firstEigenFace = Tools.GetVectorFromTableInTable(eigenFacesT, 0, 1);
            double[,] firstDiffFace = Tools.GetVectorFromTableInTable(diffVectors, 100, 1);
            double[,] firstDiffFaceT = Accord.Math.Matrix.Transpose(firstDiffFace);

            double[,] wage = Accord.Math.Matrix.Dot(eigenFacesT, diffVectorsT); // wage[eigenface,image]

            double minDiff = double.MaxValue;
            int indexOfSimilarFace = 0;

            int j = 0;
            for(; j < 400; ++j)
            {
                double difference = 0;
                for (int i = 0; i < 400; ++i)
                {
                    difference += Math.Abs(wage[i, j] - wage[i, 13]);
                    //Console.WriteLine(i + ": " + Math.Abs(wage[i, 0] - wage[i,2]));

                }


                if (minDiff > difference && difference != 0)
                {
                    minDiff = difference;
                    indexOfSimilarFace = j;
                    
                }
               if(difference < 400000000) Console.WriteLine("Difference for:" + j + "=" + difference);
            }
            Console.WriteLine("Difference: " + minDiff + " index: " + indexOfSimilarFace);


            Bitmap diffExampleImage = Tools.CreateBitMapFromBytes(Tools.GetVectorFromTable(allVectors, indexOfSimilarFace), 92, 112);


                Dispatcher.Invoke(() =>
                {
                        image.Source = BitmapToImageSource(diffExampleImage);
                });
            

            Console.WriteLine("Done");
        }

        private void Button_Click_Equalizer(object sender, RoutedEventArgs e)
        {
            isEqualizerActive = !isEqualizerActive;
        }
    }


}
