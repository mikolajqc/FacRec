using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Accord.Imaging.Filters;
using Commons.Utilities;

//todo: moze wrzuc do FacesMatrix takie cos jak numberOfVectors??????
//todo: wlasna dekompozycja
//todo: ogarnac LDA z tego linka, chyba daloby sie to zrobic jako gotowiec: http://accord-framework.net/docs/html/T_Accord_Statistics_Analysis_LinearDiscriminantAnalysis.htm
//todo: jest opcja zeby zrobic po prostu polaczenie PCA + LDA, zeby mozna bylo policzyc te pojebane macierze.
namespace FisherFaceRecognition
{
    public class FisherFaceLearningService
    {
        private FacesMatrix _unprocessedVectors;
        private FacesMatrix _averageVector;
        private FacesMatrix _averageVectorsForClasses; //count = liczba klas
        private List<FacesMatrix> _matricesSplittedIntoClasses; //count = liczba klas, ale facesmatrix zawiera cala klase
        private List<FacesMatrix> _differecesInClasses;
        private FacesMatrix _withinScatterMatrix;
        private FacesMatrix _betweenClassScatterMatrix;

        private FacesMatrix _differencesBetweenAverageVectorsInClassesAndAverageVectorOfTotal;

        private List<string> _userNames;

        private string _pathToLearningSet; /// Ogarnij zeby sciezke pobieralo z jakiegos configa 

        private const int Width = 92;
        private const int Height = 112;
        private const int NumberOfImagesPerPerson = 10;

        private int NumberOfClasses => _userNames.Count;

        #region constructors
        public FisherFaceLearningService()
        {
            _pathToLearningSet = @"D:\Studia\Inzynierka\LearningSet_AT&T\";
            _unprocessedVectors = new FacesMatrix();
            _userNames = new List<string>();
            _averageVectorsForClasses = new FacesMatrix();
            _matricesSplittedIntoClasses = new List<FacesMatrix>();
            _differecesInClasses = new List<FacesMatrix>();
            _withinScatterMatrix = new FacesMatrix();
        }
        #endregion

        public void Learn()
        {
            LoadLearningSet();
            CalculateAverageVectors();
            CalculateDifferences();
            CalculateScatterMatrices();
            FacesMatrix odwrot = _withinScatterMatrix.InverseMatrix();

            FacesMatrix Sw1Sb = odwrot * _betweenClassScatterMatrix;
            /*
            FacesMatrix differenceVectors = _unprocessedVectors - new FacesMatrix(_unprocessedVectors.X, averageVector);
            FacesMatrix differenceVectorsT = differenceVectors.Transpose();
            FacesMatrix covariation = differenceVectors * differenceVectorsT;
            EigenvalueDecomposition decomposition = new EigenvalueDecomposition(covariation.Content, true, true); 
            FacesMatrix eigenVectors = new FacesMatrix(decomposition.Eigenvectors);
            FacesMatrix eigenFaces = differenceVectorsT * eigenVectors;

            //odcinka 20 najistotniejszych
            eigenFaces = eigenFaces.GetFirstVectors(20, 0);

            List<double[]> eigenFacesAsListOfArrays = eigenFaces.GetMatrixAsListOfArrays(0);
            double[] averageVectorAsArray = averageVector.GetVectorAsArray(0, 1);

            StoreEigenFacesToDatabase(eigenFacesAsListOfArrays);
            StoreAverageVectorToDatabase(averageVectorAsArray);
            */
        }

        private void CalculateAverageVectors()
        {
            _averageVector = _unprocessedVectors.GetAverageVector(1); //wektor sredni calego zbioru - m

            for (int i = 0; i < NumberOfClasses; ++i)
            {
                var currentAverageVector =
                    _unprocessedVectors.GetAverageVector(1, NumberOfImagesPerPerson * i, NumberOfImagesPerPerson);
                //_averageVectorsForClasses.PushBackVector(currentAverageVector.GetVectorAsArray(0, 1), 1);
                _averageVectorsForClasses.Concatenate(currentAverageVector, 1);
            }
        }

        private void CalculateScatterMatrices()
        {

            //Sw:
            FacesMatrix sW = new FacesMatrix(new double[_unprocessedVectors.X, _unprocessedVectors.X]); /// ta macierz ma wymiar nxn gdzie n to liczba pikseli

            for (int i = 0; i < NumberOfClasses; ++i)
            {
                FacesMatrix currentSW = new FacesMatrix(new double[_unprocessedVectors.X, _unprocessedVectors.X]);
                currentSW = _differecesInClasses[i] * _differecesInClasses[i].Transpose();
                if (i == 0) sW = currentSW;
                else sW = sW + currentSW;
            }
            _withinScatterMatrix = sW;

            //Sb:
            _differencesBetweenAverageVectorsInClassesAndAverageVectorOfTotal = _averageVectorsForClasses - new FacesMatrix(_averageVectorsForClasses.X, _averageVector);
            _betweenClassScatterMatrix = _differencesBetweenAverageVectorsInClassesAndAverageVectorOfTotal *
                                         _differencesBetweenAverageVectorsInClassesAndAverageVectorOfTotal.Transpose();

        }

        private void CalculateDifferences()
        {
            for (int i = 0; i < NumberOfClasses; ++i)
            {
                _matricesSplittedIntoClasses.Add(_unprocessedVectors.GetPartOfMatrix(i * 10, 10, 1));
            }

            for (int i = 0; i < NumberOfClasses; ++i)
            {
                _differecesInClasses.Add(_matricesSplittedIntoClasses[i] -
                                         new FacesMatrix(_matricesSplittedIntoClasses[i].X,
                                             _averageVectorsForClasses.GetPartOfMatrix(i,1,1)));
            }

    }

        /// <summary>
        /// Loads learning set of images and 
        /// </summary>
        private void LoadLearningSet()
        {
            Console.WriteLine("Loading images from: " + _pathToLearningSet + "...");
            List<List<double>> temporarySetOfLoadedImages = new List<List<double>>();

            foreach (string dir in Directory.GetDirectories(_pathToLearningSet))
            {
                bool classExist = false;
                foreach (string file in Directory.GetFiles(dir))
                {
                    if (Path.GetExtension(file) == ".pgm" || Path.GetExtension(file) == ".jpg")
                    {
                        classExist = true;
                        temporarySetOfLoadedImages.Add(GetImageVectorInList(file));
                    }
                }

                if(classExist) _userNames.Add(Path.GetFileName(dir));
            }

            _unprocessedVectors.LoadFromListOfList(temporarySetOfLoadedImages, 1);
        }

        private List<double> GetImageVectorInList(string pathToImage)
        {
            Bitmap bitmap = ScaleBitmapToRequredSize(AForge.Imaging.Formats.ImageDecoder.DecodeFromFile(pathToImage)); /// co to kurwa ???? zamien to na accord wtf!!!

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

        private Bitmap ScaleBitmapToRequredSize(Bitmap bitMap)
        {
            return new Bitmap(bitMap, new Size(Width, Height));
        }
    }
}
