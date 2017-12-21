using Accord.Imaging.Filters;
using Accord.Math.Decompositions;
using Commons.BussinessClasses;
using Commons.Inferfaces.DAOs;
using FaceRecognition.Interfaces;
using Commons.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace FaceRecognition.Services
{
    public class LearningService : ILearningService
    {
        private FacesMatrix _unprocessedVectors;
        private string _pathToLearningSet; /// Ogarnij zeby sciezke pobieralo z jakiegos configa 

        private const int Width = 92;
        private const int Height = 112;

        private readonly IAverageVectorDao _averageVectorDao;
        private readonly IEigenFaceDao _eigenFaceDao;
        private readonly IWageDao _wageDao;
        private List<string> _userNames;

        #region constructors
        public LearningService(IAverageVectorDao averageVectorDao, IEigenFaceDao eigenFaceDao, IWageDao wageDao)
        {
            _averageVectorDao = averageVectorDao;
            _eigenFaceDao = eigenFaceDao;
            _wageDao = wageDao;

            _pathToLearningSet = @"D:\Studia\Inzynierka\LearningSet_AT&T\";
            _unprocessedVectors = new FacesMatrix();
            _userNames = new List<string>();

        }
        #endregion

        public void Learn()
        {
            LoadLearningSet();

            FacesMatrix averageVector = _unprocessedVectors.GetAverageVector(1);
            FacesMatrix differenceVectors = _unprocessedVectors - new FacesMatrix(_unprocessedVectors.X, averageVector);
            FacesMatrix differenceVectorsT = differenceVectors.Transpose();
            FacesMatrix covariation = differenceVectors * differenceVectorsT;
            EigenvalueDecomposition decomposition = new EigenvalueDecomposition(covariation.Content, true, true); // todo: wlasna dekompozycja
            FacesMatrix eigenVectors = new FacesMatrix(decomposition.Eigenvectors);
            FacesMatrix eigenFaces = differenceVectorsT * eigenVectors;

            //odcinka 20 najistotniejszych
            eigenFaces = eigenFaces.GetFirstVectors(100, 0);

            FacesMatrix dataAfterPCA = eigenFaces.Transpose() * differenceVectorsT;

            List<double[]> eigenFacesAsListOfArrays = eigenFaces.GetMatrixAsListOfArrays(0);
            double[] averageVectorAsArray = averageVector.GetVectorAsArray(0, 1);

            StoreDataAfterPcaToDatabase(dataAfterPCA.GetMatrixAsListOfArrays(0), _userNames);
            StoreEigenFacesToDatabase(eigenFacesAsListOfArrays);
            StoreAverageVectorToDatabase(averageVectorAsArray);
        }

        private void StoreDataAfterPcaToDatabase(List<double[]> wages, List<string> names)
        {
            for (int i = 0; i < wages.Count; ++i)
            {
                _wageDao.Add(
                    new Wage()
                    {
                        Name = names[i],
                        Value = JsonConvert.SerializeObject(wages[i])
                    }
                );
            }
        }

        private void StoreEigenFacesToDatabase(List<double[]> eigenFaces)
        {
            for (int i = 0; i < eigenFaces.Count; ++i)
            {
                _eigenFaceDao.Add(
                        new EigenFace()
                        {
                            Value = JsonConvert.SerializeObject(eigenFaces[i])
                        }
                    );
            }
        }

        private void StoreAverageVectorToDatabase(double[] averageVector)
        {
            _averageVectorDao.Add(
                    new AverageVector()
                    {
                        Value = JsonConvert.SerializeObject(averageVector)
                    }
                );         
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
                foreach (string file in Directory.GetFiles(dir))
                {
                    if (Path.GetExtension(file) == ".pgm" || Path.GetExtension(file) == ".jpg")
                    {
                        temporarySetOfLoadedImages.Add(GetImageVectorInList(file));
                        _userNames.Add(Path.GetFileName(dir));
                    }
                }
            }

            _unprocessedVectors.LoadFromListOfList(temporarySetOfLoadedImages, 1);
        }

        private List<double> GetImageVectorInList(string pathToImage)
        {
            Bitmap bitmap = ScaleBitmapToRequredSize(AForge.Imaging.Formats.ImageDecoder.DecodeFromFile(pathToImage));

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
