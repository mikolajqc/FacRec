using Accord.Imaging.Filters;
using Commons.BussinessClasses;
using Commons.Inferfaces.DAOs;
using FaceRecognition.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace FaceRecognition
{
    public class RecognitionService : IRecognitonService
    {
        //values loaded from DB
        private FacesMatrix averageVector = null;
        private FacesMatrix eigenFacesT = null;
        private FacesMatrix wages = null; // [eigenface,image]
        private List<string> namesOfUsers = null;

        //consts
        const int WIDTH = 92;
        const int HEIGHT = 112;
        const int ERROR_TOLERANCE = 70000000;

        //For DI:
        private readonly IAverageVectorDAO averageVectorDAO;
        private readonly IEigenFaceDAO eigenFaceDAO;
        private readonly IWageDAO wageDAO;

        public RecognitionService(IAverageVectorDAO averageVectorDAO, IEigenFaceDAO eigenFaceDAO, IWageDAO wageDAO)
        {
            this.averageVectorDAO = averageVectorDAO;
            this.eigenFaceDAO = eigenFaceDAO;
            this.wageDAO = wageDAO;
        }

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


        private double[] GetWagesOfImageInEigenFacesSpace(Bitmap bitMap)
        {
            Bitmap scaledBitmap = new Bitmap(bitMap, new Size(WIDTH, HEIGHT));
            FacesMatrix vectorOfFaceInMatrix = GetFaceMatrixFromBitmap(scaledBitmap);
            FacesMatrix diff = vectorOfFaceInMatrix - new FacesMatrix(vectorOfFaceInMatrix.X, averageVector);
            FacesMatrix currentImageWages = eigenFacesT * diff.Transpose();

            return currentImageWages.GetVectorAsArray(0, 0);
        }

        private FacesMatrix GetFaceMatrixFromBitmap(Bitmap bitmap)
        {
            HistogramEqualization histogramEqualization = new HistogramEqualization();
            bitmap = histogramEqualization.Apply(bitmap);

            int width = bitmap.Size.Width;
            int height = bitmap.Size.Height;

            double[,] content = new double[1, width * height];

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    Color color = bitmap.GetPixel(x, y);
                    double grayscale = (color.R + color.G + color.B) / 3f;
                    content[0, y * width + x] = grayscale;
                }
            }

            return new FacesMatrix(content);
        }

        private void LoadDataFromDatabase()
        {
            //Methods below loads values from database and stores it in local fields of this class
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

            List<double[]> valuesOfWages = new List<double[]>();
            for (int i = 0; i < listOfWages.Count; ++i)
            {
                valuesOfWages.Add(JsonConvert.DeserializeObject(listOfWages[i].Value, typeof(double[])) as double[]);
                namesOfUsers.Add(listOfWages[0].Name);
            }

            wages = new FacesMatrix(valuesOfWages, 0);
        }

    }

}
