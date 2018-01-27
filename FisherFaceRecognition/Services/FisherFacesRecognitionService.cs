using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Accord.Statistics.Analysis;
using Commons.BussinessClasses;
using Commons.Consts;
using Commons.Utilities;
using Newtonsoft.Json;
using Commons.Inferfaces.DAOs;
using Commons.Inferfaces.Services;

namespace FisherFaceRecognition.Services
{
    public class FisherFacesRecognitionService : IRecognitonService
    {
        #region fields
        //values loaded from DB
        private FacesMatrix _averageVector;
        private FacesMatrix _eigenFacesT;
        private FacesMatrix _wages; // [eigenface,image]
        private List<string> _namesOfUsers;
        private Dictionary<string, int> _namesAndIndex;

        //For DI:
        private readonly IAverageVectorDao _averageVectorDao;
        private readonly IEigenFaceDao _eigenFaceDao;
        private readonly IWageDao _wageDao;
        #endregion

        #region contructors
        public FisherFacesRecognitionService(IAverageVectorDao averageVectorDao, IEigenFaceDao eigenFaceDao, IWageDao wageDao)
        {
            _averageVectorDao = averageVectorDao;
            _eigenFaceDao = eigenFaceDao;
            _wageDao = wageDao;
        }
        #endregion

        public string Recognize(Bitmap bitmapWithFace)
        {
            LoadDataFromDatabase();
            var dataAfterPca = _wages.Transpose();

            var lda = new LinearDiscriminantAnalysis();
            double[][] dataAfterPcainArray = dataAfterPca.GetMatrixAsArrayOfArray(1);
            double[] wagesOfImageInEigenFacesSpace = GetWagesOfImageInEigenFacesSpace(bitmapWithFace);

            var output = CreateOutputForLda();

            var classifier = lda.Learn(dataAfterPcainArray, output);
            var result = classifier.Decide(wagesOfImageInEigenFacesSpace);
            return _namesAndIndex.FirstOrDefault(x => x.Value == result).Key;
        }

        private int[] CreateOutputForLda()
        {
            var result = new int[_namesOfUsers.Count];

            for (int i = 0; i < _namesOfUsers.Count; ++i)
            {
                result[i] = _namesAndIndex[_namesOfUsers[i]];
            }

            return result;
        }

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
                var valueOfAverageVector = JsonConvert.DeserializeObject(listOfAverageVectors[0].Value, typeof(double[])) as double[];

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
            _namesAndIndex = new Dictionary<string, int>();

            var valuesOfWages = new List<double[]>();
            if (listOfWages != null)
                for (int i = 0; i < listOfWages.Count; ++i)
                {
                    valuesOfWages.Add(
                        JsonConvert.DeserializeObject(listOfWages[i].Value, typeof(double[])) as double[]);
                    _namesOfUsers.Add(listOfWages[i].Name);

                    if (!_namesAndIndex.ContainsKey(listOfWages[i].Name)) //jesli slownik nie zawiera juz taka nazwe to:
                    {
                        _namesAndIndex.Add(listOfWages[i].Name, _namesAndIndex.Count);
                    }
                }

            _wages = new FacesMatrix(valuesOfWages, 0);
        }

    }
}
