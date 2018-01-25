using Accord.Imaging.Filters;
using Accord.Math.Decompositions;
using Commons.BussinessClasses;
using Commons.Inferfaces.DAOs;
using Commons.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Commons.Consts;
using Commons.Inferfaces.Services;

namespace EigenFaceRecognition.Services
{
    public class LearningService : ILearningService
    {
        private readonly IAverageVectorDao _averageVectorDao;
        private readonly IEigenFaceDao _eigenFaceDao;
        private readonly IWageDao _wageDao;

        private readonly FacesMatrix _unprocessedVectors;
        private readonly List<string> _userNames;

        #region constructors

        public LearningService(IAverageVectorDao averageVectorDao, IEigenFaceDao eigenFaceDao, IWageDao wageDao)
        {
            _averageVectorDao = averageVectorDao;
            _eigenFaceDao = eigenFaceDao;
            _wageDao = wageDao;

            _unprocessedVectors = new FacesMatrix();
            _userNames = new List<string>();
        }
        #endregion

        public void Learn()
        {
            ClearDatabase();
            LoadLearningSet();

            FacesMatrix averageVector = _unprocessedVectors.GetAverageVector(1);
            FacesMatrix differenceVectors = _unprocessedVectors - new FacesMatrix(_unprocessedVectors.X, averageVector);
            FacesMatrix differenceVectorsT = differenceVectors.Transpose();
            FacesMatrix covariation = differenceVectorsT * differenceVectors;
            EigenvalueDecomposition decomposition = new EigenvalueDecomposition(covariation.Content, true, true);
            FacesMatrix eigenVectors = new FacesMatrix(decomposition.Eigenvectors);
            FacesMatrix eigenFaces = eigenVectors * differenceVectorsT;

            //take key values
            eigenFaces = eigenFaces.GetFirstVectors(CommonConsts.Server.NumberOfKeyEigenFaces, 0);

            FacesMatrix dataAfterPca = differenceVectorsT * eigenFaces.Transpose();

            List<double[]> eigenFacesAsListOfArrays = eigenFaces.GetMatrixAsListOfArrays(0);
            double[] averageVectorAsArray = averageVector.GetVectorAsArray(0, 1);

            StoreDataAfterPcaToDatabase(dataAfterPca.GetMatrixAsListOfArrays(0), _userNames);
            StoreEigenFacesToDatabase(eigenFacesAsListOfArrays);
            StoreAverageVectorToDatabase(averageVectorAsArray);
        }

        private void ClearDatabase()
        {
            _wageDao.DeleteAll();
            _averageVectorDao.DeleteAll();
            _eigenFaceDao.DeleteAll();
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

        private void LoadLearningSet()
        {
            List<List<double>> temporarySetOfLoadedImages = new List<List<double>>();

            foreach (string dir in Directory.GetDirectories(CommonConsts.Server.PathToLearningSet))
            {
                if (Directory.GetFiles(dir).Length == CommonConsts.Server.RequiredNumberOfImagesPerPersonForLearning)
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
            return new Bitmap(bitMap, new Size(CommonConsts.Server.DefaultWidthOfPicturesOfFace, CommonConsts.Server.DefaultHeightOfPictureOfFace));
        }
    }
}
