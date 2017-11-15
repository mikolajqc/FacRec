using Accord.Imaging.Filters;
using Accord.Imaging.Formats;
using Accord.Math.Decompositions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition
{
    public class FaceRecognition
    {
        ///TODO:
        ///1. wykluczyc mozliwosc ze jedna twarz ma wiecej przykladow - exception przy mnozeniu macierzy lub rozwiazac to jakos

        #region fields
        
        ///czy tutaj koniecznie consty?
        const int WIDTH = 92;
        const int HEIGHT = 112;

        private FacesMatrix unprocessedVectors = null;
        private FacesMatrix averageVector = null;
        private FacesMatrix wages = null; // [eigenface,image]
        private FacesMatrix eigenFacesT = null;

        private List<string> namesOfPeople = null;

        private string pathToLearningSet = null;

        #endregion

        #region constructors

        public FaceRecognition(string pathToLearningSet)
        {
            this.pathToLearningSet = pathToLearningSet;
            unprocessedVectors = new FacesMatrix();
            wages = new FacesMatrix();
            namesOfPeople = new List<string>();
        }

        #endregion

        #region publicmethods

        /// <summary>
        /// Temporarily this returns string that is stored in namesOfPeople List
        /// </summary>
        /// <param name="bitMapWithFace"></param>
        /// <returns></returns>
        public string Recognize(Bitmap bitMapWithFace) // temporary: Bitmap zamienic na wlasny typ FaceImage ktory obsluguje pgm itd
        {
            double[] wagesInArray = GetWagesOfImageInEigenFacesSpace(bitMapWithFace);

            double minEuclideanDistance = double.MaxValue;
            int numberOfString = 0;
            for (int numberOfKnownImage = 0; numberOfKnownImage < wages.Y; ++numberOfKnownImage)
            {
                double[] currentImageWagesInArray = wages.GetVectorAsArray(numberOfKnownImage, 0);
                double currentEuclideanDistance = Accord.Math.Distance.Euclidean(wagesInArray, currentImageWagesInArray);

                if (minEuclideanDistance > currentEuclideanDistance)
                {
                    minEuclideanDistance = currentEuclideanDistance;
                    numberOfString = numberOfKnownImage;
                }
            }
            //treshold trzeba ogarnac

            return namesOfPeople.ElementAt(numberOfString);
        }

        public void AddNewFace(Bitmap bitmapWithNewFace, string name)
        {
            double[] wagesOfNewImage = GetWagesOfImageInEigenFacesSpace(bitmapWithNewFace);
            wages.PushBackVector(wagesOfNewImage, 0);
            namesOfPeople.Add(name);
        }

        public void Learn()
        {
            Console.WriteLine("Learning...");

            LoadLearningSet();
            averageVector = unprocessedVectors.GetAverageVector(1);
            FacesMatrix differenceVectors = unprocessedVectors - new FacesMatrix(unprocessedVectors.X, averageVector);
            FacesMatrix differenceVectorsT = differenceVectors.Transpose();
            FacesMatrix covariation = differenceVectors * differenceVectorsT;

            EigenvalueDecomposition decomposition = new EigenvalueDecomposition(covariation.Content, true, true); // todo: wlasna dekompozycja

            FacesMatrix eigenVectors = new FacesMatrix(decomposition.Eigenvectors);
            FacesMatrix eigenVectorsT = eigenVectors.Transpose();

            FacesMatrix eigenFaces = differenceVectorsT * eigenVectors;
            eigenFacesT = eigenFaces.Transpose();

            wages = eigenFacesT * differenceVectorsT;

            Console.WriteLine("Done");

        }

        #endregion

        #region privatemethods

        private double[] GetWagesOfImageInEigenFacesSpace(Bitmap bitMap)
        {
            Bitmap scaledBitmap = ScaleBitmapToRequredSize(bitMap);
            FacesMatrix vectorOfFaceInMatrix = GetFaceMatrixFromBitmap(scaledBitmap);
            FacesMatrix diff = vectorOfFaceInMatrix - new FacesMatrix(vectorOfFaceInMatrix.X, averageVector);
            FacesMatrix currentImageWages = eigenFacesT * diff.Transpose();

            return currentImageWages.GetVectorAsArray(0, 0);
        }

        private void LoadLearningSet()
        {
            Console.WriteLine("Loading images from: " + pathToLearningSet + "...");
            List<List<double>> temporarySetOfLoadedImages = new List<List<double>>();

            foreach (string dir in Directory.GetDirectories(pathToLearningSet))
            {
                foreach (string file in Directory.GetFiles(dir))
                {
                    if (Path.GetExtension(file) == ".pgm" || Path.GetExtension(file) == ".jpg")
                    {
                        temporarySetOfLoadedImages.Add(GetImageVectorInList(file));
                        namesOfPeople.Add(Path.GetFileName(Path.GetDirectoryName(file)));

                    }
                }
            }

            unprocessedVectors.LoadFromListOfList(temporarySetOfLoadedImages, 1);
        }

        private FacesMatrix GetFaceMatrixFromBitmap(Bitmap bitmap)
        {
            HistogramEqualization histogramEqualization = new HistogramEqualization();
            bitmap = histogramEqualization.Apply(bitmap);

            int width = bitmap.Size.Width;
            int height = bitmap.Size.Height;

            double[,] content = new double[1, width * height];

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    Color color = bitmap.GetPixel(x, y);
                    double grayscale = (color.R + color.G + color.B) / 3f;
                    content[0, y * width + x] = grayscale;
                }
            }

            return new FacesMatrix(content);
        }

        private Bitmap ScaleBitmapToRequredSize(Bitmap bitMap)
        {
            Console.WriteLine("Scalling image to" + WIDTH + "x" + HEIGHT);
            return new Bitmap(bitMap, new Size(WIDTH, HEIGHT));
        }

        private List<double> GetImageVectorInList(string pathToImage)
        {
            Bitmap bitmap = ScaleBitmapToRequredSize(ImageDecoder.DecodeFromFile(pathToImage));
            //bitmap.Save(pathToImage);
            HistogramEqualization histogramEqualization = new HistogramEqualization();
            bitmap = histogramEqualization.Apply(bitmap);

            int width = bitmap.Size.Width;
            int height = bitmap.Size.Height;

            List<double> resultVector = new List<double>();

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    Color color = bitmap.GetPixel(x, y);
                    double grayscale = (color.R + color.G + color.B) / 3f;
                    resultVector.Add(grayscale);
                }
            }

            return resultVector;
        }

        #endregion

    }
}
