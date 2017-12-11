using FaceRecognition.Interfaces;
using System.Collections.Generic;
using System.Drawing;
using Commons.Inferfaces.DAOs;
using Commons.BussinessClasses;
using Newtonsoft.Json;
using FaceRecognition.Utilities;

namespace FaceRecognition.Services
{
    public class AddNewFaceService : IAddNewFaceService
    {
        #region fields
        //values loaded from DB
        private FacesMatrix averageVector = null;
        private FacesMatrix eigenFacesT = null;

        //consts - to sth with it !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        const int WIDTH = 92;
        const int HEIGHT = 112;

        //For DI:
        private readonly IAverageVectorDAO averageVectorDAO;
        private readonly IEigenFaceDAO eigenFaceDAO;
        private readonly IWageDAO wageDAO;
        #endregion

        #region contructors
        public AddNewFaceService(IAverageVectorDAO averageVectorDAO, IEigenFaceDAO eigenFaceDAO, IWageDAO wageDAO)
        {
            this.averageVectorDAO = averageVectorDAO;
            this.eigenFaceDAO = eigenFaceDAO;
            this.wageDAO = wageDAO;
        }
        #endregion

        #region publicmethods
        public void AddNewFace(Bitmap bitmapWithFace, string name)
        {
            LoadDataFromDatabase();

            double[] wagesOfNewImage = GetWagesOfImageInEigenFacesSpace(bitmapWithFace); //new face in face space
            wageDAO.Add(new Wage()
                {
                    Name = name,
                    Value = JsonConvert.SerializeObject(wagesOfNewImage)
                });
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
            Bitmap scaledBitmap = new Bitmap(bitmap, new Size(WIDTH, HEIGHT));
            FacesMatrix vectorOfFaceInMatrix = new FacesMatrix(scaledBitmap);
            FacesMatrix diff = vectorOfFaceInMatrix - new FacesMatrix(vectorOfFaceInMatrix.X, averageVector);
            FacesMatrix currentImageWages = eigenFacesT * diff.Transpose();

            return currentImageWages.GetVectorAsArray(0, 0);
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
        #endregion

    }
}
