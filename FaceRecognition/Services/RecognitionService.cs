﻿using Accord.Imaging.Filters;
using Commons.BussinessClasses;
using Commons.Inferfaces.DAOs;
using FaceRecognition.Interfaces;
using FaceRecognition.Utilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace FaceRecognition.Services
{
    public class RecognitionService : IRecognitonService
    {
        #region fields
        //values loaded from DB
        private FacesMatrix averageVector = null;
        private FacesMatrix eigenFacesT = null;
        private FacesMatrix wages = null; // [eigenface,image]
        private List<string> namesOfUsers = null;

        //consts - to sth with it !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        const int WIDTH = 92;
        const int HEIGHT = 112;
        const int ERROR_TOLERANCE = 70000000;

        //For DI:
        private readonly IAverageVectorDAO averageVectorDAO;
        private readonly IEigenFaceDAO eigenFaceDAO;
        private readonly IWageDAO wageDAO;
        #endregion

        #region contructors
        public RecognitionService(IAverageVectorDAO averageVectorDAO, IEigenFaceDAO eigenFaceDAO, IWageDAO wageDAO)
        {
            this.averageVectorDAO = averageVectorDAO;
            this.eigenFaceDAO = eigenFaceDAO;
            this.wageDAO = wageDAO;
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
            HistogramEqualization histogramEqualization = new HistogramEqualization();
            bitMapWithFace = histogramEqualization.Apply(bitMapWithFace);

            LoadDataFromDatabase();

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

            if (minEuclideanDistance > ERROR_TOLERANCE) return "unknown";
            return namesOfUsers.ElementAt(numberOfString);
        }
        #endregion

        #region privatemethods
        private double[] GetWagesOfImageInEigenFacesSpace(Bitmap bitmap)
        {
            Bitmap scaledBitmap = new Bitmap(bitmap, new Size(WIDTH, HEIGHT));
            FacesMatrix vectorOfFaceInMatrix = new FacesMatrix(scaledBitmap);
            FacesMatrix diff = vectorOfFaceInMatrix - new FacesMatrix(vectorOfFaceInMatrix.X, averageVector);
            FacesMatrix currentImageWages = eigenFacesT * diff.Transpose();

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
            List<AverageVector> listOfAverageVectors = averageVectorDAO.GetOverview() as List<AverageVector>;
            double[] valueOfAverageVector = (JsonConvert.DeserializeObject(listOfAverageVectors[0].Value, typeof(double[])) as double[]);

            averageVector = new FacesMatrix(valueOfAverageVector, 1);
        }

        private void LoadEigenFacesTFromDatabase()
        {
            List<EigenFace> listOfEigenFaces = eigenFaceDAO.GetOverview() as List<EigenFace>;
            List<double[]> valuesOfEigenFaces = new List<double[]>();

            for (int i = 0; i < listOfEigenFaces.Count; ++i)
            {
                valuesOfEigenFaces.Add(JsonConvert.DeserializeObject(listOfEigenFaces[i].Value, typeof(double[])) as double[]);
            }

            eigenFacesT = new FacesMatrix(valuesOfEigenFaces, 1); //orientacja 1 bo tworzymy EigenFacesT czyli gdzie X jest = 400
        }

        private void LoadWagesAndNamesOfUsersFromDataBase()
        {
            List<Wage> listOfWages = wageDAO.GetOverview() as List<Wage>;
            ///upewnic sie ale z tego co wiem
            ///zdjecia w matrixie zawierajacym wagi sa trzymane poziomo
            ///czyli jezeli [x,y] zmieniamy y to zmieniamy eigenface czyli jestesmy na jednej twarzy ale przegladamy jej wagi
            ///dlatego orientacja 0
            ///

            namesOfUsers = new List<string>();

            List<double[]> valuesOfWages = new List<double[]>();
            for (int i = 0; i < listOfWages.Count; ++i)
            {
                valuesOfWages.Add(JsonConvert.DeserializeObject(listOfWages[i].Value, typeof(double[])) as double[]);
                namesOfUsers.Add(listOfWages[0].Name);
            }

            wages = new FacesMatrix(valuesOfWages, 0);
        }
        #endregion
    }

}
