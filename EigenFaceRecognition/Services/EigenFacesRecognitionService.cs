using Commons.BussinessClasses;
using Commons.Inferfaces.DAOs;
using Commons.Utilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Commons.Consts;
using Commons.Inferfaces.Services;

namespace EigenFaceRecognition.Services
{
    public class EigenFacesRecognitionService : IRecognitonService
    {
        #region fields
        //values loaded from DB
        private FacesMatrix _averageVector;
        private FacesMatrix _eigenFacesT;
        private FacesMatrix _wages; // [eigenface,image]
        private List<string> _namesOfUsers;

        //For DI:
        private readonly IAverageVectorDao _averageVectorDao;
        private readonly IEigenFaceDao _eigenFaceDao;
        private readonly IWageDao _wageDao;
        #endregion

        #region contructors
        public EigenFacesRecognitionService(IAverageVectorDao averageVectorDao, IEigenFaceDao eigenFaceDao, IWageDao wageDao)
        {
            _averageVectorDao = averageVectorDao;
            _eigenFaceDao = eigenFaceDao;
            _wageDao = wageDao;
        }
        #endregion

        #region publicmethods
        /// <summary>
        /// This method returns string that is stored in _namesOfUsers
        /// </summary>
        /// <param name="bitMapWithFace"></param>
        /// <returns></returns>
        public string Recognize(Bitmap bitMapWithFace)
        {
            LoadDataFromDatabase();

            double[] wagesInArray = GetWagesOfImageInEigenFacesSpace(bitMapWithFace);

            double minEuclideanDistance = double.MaxValue;
            int numberOfString = 0;
            for (int numberOfKnownImages = 0; numberOfKnownImages < _wages.Y; ++numberOfKnownImages)
            {
                double[] currentImageWagesInArray = _wages.GetVectorAsArray(numberOfKnownImages, 0);
                double currentEuclideanDistance =
                    Accord.Math.Distance.Euclidean(wagesInArray, currentImageWagesInArray);

                if (minEuclideanDistance > currentEuclideanDistance)
                {
                    minEuclideanDistance = currentEuclideanDistance;
                    numberOfString = numberOfKnownImages;
                }
            }

            if (minEuclideanDistance > CommonConsts.Server.ErrorToleranceForEigenFaces) return "unknown";
            return _namesOfUsers.ElementAt(numberOfString);
        }
        #endregion

        #region privatemethods

        private double[] GetWagesOfImageInEigenFacesSpace(Bitmap bitmap)
        {
            var scaledBitmap = new Bitmap(bitmap, new Size(CommonConsts.Server.DefaultWidthOfPicturesOfFace, CommonConsts.Server.DefaultHeightOfPictureOfFace));
            var vectorOfFaceInMatrix = new FacesMatrix(scaledBitmap);
            var diff = vectorOfFaceInMatrix - new FacesMatrix(vectorOfFaceInMatrix.X, _averageVector);
            FacesMatrix currentImageWages = diff.Transpose() * _eigenFacesT;

            return currentImageWages.GetVectorAsArray(0, 0);
        }

        private void LoadDataFromDatabase()
        {
            LoadAverageVectorFromDatabase();
            LoadEigenFacesTFromDatabase();
            LoadWagesAndNamesOfUsersFromDataBase();
        }

        private void LoadAverageVectorFromDatabase()
        {
            var listOfAverageVectors = _averageVectorDao.GetOverview() as List<AverageVector>;
            if (listOfAverageVectors != null)
            {
                double[] valueOfAverageVector =
                    (JsonConvert.DeserializeObject(listOfAverageVectors[0].Value, typeof(double[])) as double[]);

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

        private void LoadWagesAndNamesOfUsersFromDataBase()
        {
            var listOfWages = _wageDao.GetOverview() as List<Wage>;

            _namesOfUsers = new List<string>();

            var valuesOfWages = new List<double[]>();
            if (listOfWages != null)
                for (int i = 0; i < listOfWages.Count; ++i)
                {
                    valuesOfWages.Add(
                        JsonConvert.DeserializeObject(listOfWages[i].Value, typeof(double[])) as double[]);
                    _namesOfUsers.Add(listOfWages[i].Name);
                }

            _wages = new FacesMatrix(valuesOfWages, 0);
        }
        #endregion
    }

}
