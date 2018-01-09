using Accord.Imaging.Filters;
using Commons.BussinessClasses;
using Commons.Inferfaces.DAOs;
using Commons.Utilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Commons.Inferfaces.Services;

//todo: ogarnij przetwarzanie wstepne! masz narazie jedynie wyrownanie histogramow. Dobrze by bylo miec jakies wycinanie zdjecia dodatkowo
namespace FaceRecognition.Services
{
    public class EigenFacesRecognitionService : IRecognitonService
    {
        #region fields
        //values loaded from DB
        private FacesMatrix _averageVector;
        private FacesMatrix _eigenFacesT;
        private FacesMatrix _wages; // [eigenface,image]
        private List<string> _namesOfUsers;

        //todo: co zrobic z error tolerance???
        private const int Width = 104;
        private const int Height = 174;
        private const int ErrorTolerance = int.MaxValue;//70000000;

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
        /// Temporarily this returns string that is stored in namesOfPeople List
        /// </summary>
        /// <param name="bitMapWithFace"></param>
        /// <returns></returns>
        public string Recognize(Bitmap bitMapWithFace) // temporary: Bitmap zamienic na wlasny typ FaceImage ktory obsluguje pgm itd
        {
            LoadDataFromDatabase();

            double[] wagesInArray = GetWagesOfImageInEigenFacesSpace(bitMapWithFace);

            double minEuclideanDistance = double.MaxValue;
            int numberOfString = 0;
            for (int numberOfKnownImage = 0; numberOfKnownImage < _wages.Y; ++numberOfKnownImage)
            {
                double[] currentImageWagesInArray = _wages.GetVectorAsArray(numberOfKnownImage, 0);
                double currentEuclideanDistance =
                    Accord.Math.Distance.Euclidean(wagesInArray, currentImageWagesInArray);

                if (minEuclideanDistance > currentEuclideanDistance)
                {
                    minEuclideanDistance = currentEuclideanDistance;
                    numberOfString = numberOfKnownImage;
                }
            }

            if (minEuclideanDistance > ErrorTolerance) return "unknown";
            return _namesOfUsers.ElementAt(numberOfString);
        }
        #endregion

        #region privatemethods

        private double[] GetWagesOfImageInEigenFacesSpace(Bitmap bitmap)
        {
            Bitmap scaledBitmap = new Bitmap(bitmap, new Size(Width, Height));
            FacesMatrix vectorOfFaceInMatrix = new FacesMatrix(scaledBitmap);
            FacesMatrix diff = vectorOfFaceInMatrix - new FacesMatrix(vectorOfFaceInMatrix.X, _averageVector);
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
            List<AverageVector> listOfAverageVectors = _averageVectorDao.GetOverview() as List<AverageVector>;
            double[] valueOfAverageVector =
                (JsonConvert.DeserializeObject(listOfAverageVectors[0].Value, typeof(double[])) as double[]);

            _averageVector = new FacesMatrix(valueOfAverageVector, 1);
        }

        private void LoadEigenFacesTFromDatabase()
        {
            List<EigenFace> listOfEigenFaces = _eigenFaceDao.GetOverview() as List<EigenFace>;
            List<double[]> valuesOfEigenFaces = new List<double[]>();

            for (int i = 0; i < listOfEigenFaces.Count; ++i)
            {
                valuesOfEigenFaces.Add(JsonConvert.DeserializeObject(listOfEigenFaces[i].Value, typeof(double[])) as double[]);
            }

            _eigenFacesT = new FacesMatrix(valuesOfEigenFaces, 1); //orientacja 1 bo tworzymy EigenFacesT czyli gdzie X jest = 400
        }

        private void LoadWagesAndNamesOfUsersFromDataBase()
        {
            List<Wage> listOfWages = _wageDao.GetOverview() as List<Wage>;
            ///upewnic sie ale z tego co wiem
            ///zdjecia w matrixie zawierajacym wagi sa trzymane poziomo
            ///czyli jezeli [x,y] zmieniamy y to zmieniamy eigenface czyli jestesmy na jednej twarzy ale przegladamy jej wagi
            ///dlatego orientacja 0
            ///

            _namesOfUsers = new List<string>();

            List<double[]> valuesOfWages = new List<double[]>();
            for (int i = 0; i < listOfWages.Count; ++i)
            {
                valuesOfWages.Add(JsonConvert.DeserializeObject(listOfWages[i].Value, typeof(double[])) as double[]);
                _namesOfUsers.Add(listOfWages[i].Name);
            }

            _wages = new FacesMatrix(valuesOfWages, 0);
        }
        #endregion
    }

}
