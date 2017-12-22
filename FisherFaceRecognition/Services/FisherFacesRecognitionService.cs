﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Accord.Statistics.Analysis;
using Commons.BussinessClasses;
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
        private Dictionary<string, int> namesAndIndex;

        //consts - to sth with it !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        private const int Width = 92;
        private const int Height = 112;

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
            FacesMatrix dataAfterPCA = _wages; //400 wektorów dla k wymiarowej przestrzeni, teraz do przetworzenia przez LDA
            dataAfterPCA = _wages.Transpose();

            var lda = new LinearDiscriminantAnalysis();
            double[][] dataAfterPcainArray = dataAfterPCA.GetMatrixAsArrayOfArray(1);
            double[] wagesOfImageInEigenFacesSpace = GetWagesOfImageInEigenFacesSpace(bitmapWithFace);

            int[] output = CreateOutputForLda();

            var classifier = lda.Learn(dataAfterPcainArray, output);
            int result = classifier.Decide(wagesOfImageInEigenFacesSpace);
            return namesAndIndex.FirstOrDefault(x => x.Value == result).Key;
        }

        private int[] CreateOutputForLda()
        {
            int[] result = new int[_namesOfUsers.Count];

            for (int i = 0; i < _namesOfUsers.Count; ++i)
            {
                result[i] = namesAndIndex[_namesOfUsers[i]];
            }

            return result;
        }

        private double[] GetWagesOfImageInEigenFacesSpace(Bitmap bitmap)
        {
            Bitmap scaledBitmap = new Bitmap(bitmap, new Size(Width, Height));
            FacesMatrix vectorOfFaceInMatrix = new FacesMatrix(scaledBitmap);
            FacesMatrix diff = vectorOfFaceInMatrix - new FacesMatrix(vectorOfFaceInMatrix.X, _averageVector);
            FacesMatrix currentImageWages = _eigenFacesT * diff.Transpose();

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
            double[] valueOfAverageVector = JsonConvert.DeserializeObject(listOfAverageVectors[0].Value, typeof(double[])) as double[];

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

            _namesOfUsers = new List<string>();
            namesAndIndex = new Dictionary<string, int>();

            List<double[]> valuesOfWages = new List<double[]>();
            for (int i = 0; i < listOfWages.Count; ++i)
            {
                valuesOfWages.Add(JsonConvert.DeserializeObject(listOfWages[i].Value, typeof(double[])) as double[]);
                _namesOfUsers.Add(listOfWages[i].Name);

                if (!namesAndIndex.ContainsKey(listOfWages[i].Name)) //jesli slownik nie zawiera juz taka nazwe to:
                {
                    namesAndIndex.Add(listOfWages[i].Name,namesAndIndex.Count);
                }
            }

            _wages = new FacesMatrix(valuesOfWages, 0);
        }

    }
}
