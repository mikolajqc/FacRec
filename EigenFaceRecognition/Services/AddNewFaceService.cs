using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Commons.Inferfaces.DAOs;
using Commons.BussinessClasses;
using Commons.Consts;
using Commons.Inferfaces.Services;
using Newtonsoft.Json;
using Commons.Utilities;

namespace EigenFaceRecognition.Services
{
    public class AddNewFaceService : IAddNewFaceService
    {
        #region fields
        //values loaded from DB
        private FacesMatrix _averageVector;
        private FacesMatrix _eigenFacesT;

        //For DI:
        private readonly IAverageVectorDao _averageVectorDao;
        private readonly IEigenFaceDao _eigenFaceDao;
        private readonly IWageDao _wageDao;
        #endregion

        #region contructors
        public AddNewFaceService(IAverageVectorDao averageVectorDao, IEigenFaceDao eigenFaceDao, IWageDao wageDao)
        {
            _averageVectorDao = averageVectorDao;
            _eigenFaceDao = eigenFaceDao;
            _wageDao = wageDao;
        }
        #endregion

        #region publicmethods
        public void AddNewFace(Bitmap bitmapWithFace, string name, string directPathToLearningSet)
        {
            LoadDataFromDatabase();

            var wagesOfNewImage = GetWagesOfImageInEigenFacesSpace(bitmapWithFace);
            _wageDao.Add(new Wage()
                {
                    Name = name,
                    Value = JsonConvert.SerializeObject(wagesOfNewImage)
                });

            AddFaceImageToLearningSet(bitmapWithFace, name, directPathToLearningSet);
        }
        #endregion

        #region privatemethods
        private void LoadDataFromDatabase()
        {
            LoadAverageVectorFromDatabase();
            LoadEigenFacesTFromDatabase();
        }

        private double[] GetWagesOfImageInEigenFacesSpace(Bitmap bitmap)
        {
            var scaledBitmap = new Bitmap(bitmap, new Size(CommonConsts.Server.DefaultWidthOfPicturesOfFace, CommonConsts.Server.DefaultHeightOfPictureOfFace));
            var vectorOfFaceInMatrix = new FacesMatrix(scaledBitmap);
            var diff = vectorOfFaceInMatrix - new FacesMatrix(vectorOfFaceInMatrix.X, _averageVector);
            FacesMatrix currentImageWages = diff.Transpose() * _eigenFacesT;

            return currentImageWages.GetVectorAsArray(0, 0);
        }

        private void LoadAverageVectorFromDatabase()
        {
            var listOfAverageVectors = _averageVectorDao.GetOverview() as List<AverageVector>;
            if (listOfAverageVectors != null)
            {
                double[] valueOfAverageVector = (JsonConvert.DeserializeObject(listOfAverageVectors[0].Value, typeof(double[])) as double[]);

                _averageVector = new FacesMatrix(valueOfAverageVector, 1);
            }
        }

        private void LoadEigenFacesTFromDatabase()
        {
            var listOfEigenFaces = _eigenFaceDao.GetOverview() as List<EigenFace>;
            var valuesOfEigenFaces = new List<double[]>();

            if (listOfEigenFaces != null)
                for (int i = 0; i < listOfEigenFaces.Count; ++i)
                {
                    valuesOfEigenFaces.Add(
                        JsonConvert.DeserializeObject(listOfEigenFaces[i].Value, typeof(double[])) as double[]);
                }

            _eigenFacesT = new FacesMatrix(valuesOfEigenFaces, 1);
        }

        private void AddFaceImageToLearningSet(Bitmap bitmapWithFace, string name, string directPathToLearningSet)
        {
            bitmapWithFace = new Bitmap(bitmapWithFace, new Size(CommonConsts.Server.DefaultWidthOfPicturesOfFace, CommonConsts.Server.DefaultHeightOfPictureOfFace));

            string currentDirectory = Path.Combine(directPathToLearningSet, name);
            string nameOfFile;

            if (Directory.Exists(currentDirectory))
            {
                int index = Directory.GetFiles(currentDirectory).Length;
                if (index == CommonConsts.Server.RequiredNumberOfImagesPerPersonForLearning) return;
                nameOfFile = index + ".jpg";
            }
            else
            {
                Directory.CreateDirectory(currentDirectory);
                nameOfFile = "0.jpg";
            }

            bitmapWithFace.Save(Path.Combine(currentDirectory, nameOfFile), System.Drawing.Imaging.ImageFormat.Jpeg);
        }
        #endregion

    }
}
