using Accord.Imaging.Filters;
using Accord.Math.Decompositions;
using Commons.BussinessClasses;
using Commons.Inferfaces.DAOs;
using FaceRecognition.Interfaces;
using FaceRecognition.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace FaceRecognition.Services
{
    public class LearningService : ILearningService
    {
        private FacesMatrix unprocessedVectors = null;
        private string pathToLearningSet = null; /// Ogarnij zeby sciezke pobieralo z jakiegos configa 

        const int WIDTH = 92;
        const int HEIGHT = 112;
        const int ERROR_TOLERANCE = 70000000;

        private readonly IAverageVectorDAO averageVectorDAO;
        private readonly IEigenFaceDAO eigenFaceDAO;

        #region constructors
        public LearningService(IAverageVectorDAO averageVectorDAO, IEigenFaceDAO eigenFaceDAO)
        {
            this.averageVectorDAO = averageVectorDAO;
            this.eigenFaceDAO = eigenFaceDAO;

            this.pathToLearningSet = @"D:\Studia\Inzynierka\LearningSet_AT&T\";
            this.unprocessedVectors = new FacesMatrix();

        }
        #endregion

        public void Learn()
        {
            LoadLearningSet();

            FacesMatrix averageVector = unprocessedVectors.GetAverageVector(1);
            FacesMatrix differenceVectors = unprocessedVectors - new FacesMatrix(unprocessedVectors.X, averageVector);
            FacesMatrix differenceVectorsT = differenceVectors.Transpose();
            FacesMatrix covariation = differenceVectors * differenceVectorsT;
            EigenvalueDecomposition decomposition = new EigenvalueDecomposition(covariation.Content, true, true); // todo: wlasna dekompozycja
            FacesMatrix eigenVectors = new FacesMatrix(decomposition.Eigenvectors);
            FacesMatrix eigenFaces = differenceVectorsT * eigenVectors;

            //odcinka 20 najistotniejszych
            eigenFaces = eigenFaces.GetFirstVectors(20, 0);

            List<double[]> eigenFacesAsListOfArrays = eigenFaces.GetMatrixAsListOfArrays(0);
            double[] averageVectorAsArray = averageVector.GetVectorAsArray(0, 1);

            StoreEigenFacesToDatabase(eigenFacesAsListOfArrays);
            StoreAverageVectorToDatabase(averageVectorAsArray);
        }

        private void StoreEigenFacesToDatabase(List<double[]> eigenFaces)
        {
            for (int i = 0; i < eigenFaces.Count; ++i)
            {
                eigenFaceDAO.Add(
                        new EigenFace()
                        {
                            Value = JsonConvert.SerializeObject(eigenFaces[i])
                        }
                    );
            }
        }

        private void StoreAverageVectorToDatabase(double[] averageVector)
        {
            averageVectorDAO.Add(
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
            Console.WriteLine("Loading images from: " + pathToLearningSet + "...");
            List<List<double>> temporarySetOfLoadedImages = new List<List<double>>();

            foreach (string dir in Directory.GetDirectories(pathToLearningSet))
            {
                foreach (string file in Directory.GetFiles(dir))
                {
                    if (Path.GetExtension(file) == ".pgm" || Path.GetExtension(file) == ".jpg")
                    {
                        temporarySetOfLoadedImages.Add(GetImageVectorInList(file));
                    }
                }
            }

            unprocessedVectors.LoadFromListOfList(temporarySetOfLoadedImages, 1);
        }

        private List<double> GetImageVectorInList(string pathToImage)
        {
            Bitmap bitmap = ScaleBitmapToRequredSize(AForge.Imaging.Formats.ImageDecoder.DecodeFromFile(pathToImage)); /// co to kurwa ???? zamien to na accord wtf!!!

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
            return new Bitmap(bitMap, new Size(WIDTH, HEIGHT));
        }
    }
}
