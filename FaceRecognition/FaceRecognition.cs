using Inzynierka;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition
{
    public class FaceRecognition
    {
        private List<string> namesOfPeople = null;

        private FacesMatrix unprocessedVectors = null;
        private string pathToLearningSet = null;
        private double[] averageVector = null;

        #region constructors

        public FaceRecognition(string pathToLearningSet)
        {
            this.pathToLearningSet = pathToLearningSet;
            unprocessedVectors = new FacesMatrix();
            namesOfPeople = new List<string>();
        }

        #endregion

        #region publicmethods

        public int Recognize(Bitmap bitMapWithFace) // temporary: Bitmap zamienic na wlasny typ FaceImage ktory obsluguje pgm itd
        {

            return 0;
        }

        public void Learn()
        {
            LoadLearningSet();
            averageVector = unprocessedVectors.GetAverageVector(1);
            FacesMatrix differenceVectors = unprocessedVectors - new FacesMatrix(400,averageVector);
            FacesMatrix differenceVectorsT = differenceVectors.Transpose();



            /*
            
            averageVector = unprocessedVectors.GetAverageVector(1);
            Bitmap averageImage = Tools.CreateBitMapFromBytes(averageVector, 92, 112);
            double[,] diffVectors = LearningSetLoader.GetDifferenceVectors(averageVector, allVectors);

            double[,] diffVectorsT = Accord.Math.Matrix.Transpose(diffVectors);
            double[,] covariation = Accord.Math.Matrix.Dot(diffVectors, diffVectorsT);

            EigenvalueDecomposition decomposition = new EigenvalueDecomposition(covariation, true, true);

            double[,] eigenVectors = decomposition.Eigenvectors; // 400x400
            double[,] eigenVectorsT = Accord.Math.Matrix.Transpose(eigenVectors);

            double[,] eigenFaces = Accord.Math.Matrix.Dot(diffVectorsT, eigenVectors); // matrix to calculation 10304x400
            double[,] eigenFacesT = Accord.Math.Matrix.Transpose(eigenFaces);


            Console.WriteLine("All: " + allVectors.GetLength(0) + " x " + allVectors.GetLength(1));
            Console.WriteLine("Diff: " + diffVectors.GetLength(0) + " x " + diffVectors.GetLength(1));
            Console.WriteLine("Cov: " + covariation.GetLength(0) + " x " + covariation.GetLength(1));
            Console.WriteLine("EigenVectors: " + eigenVectors.GetLength(0) + " x " + eigenVectors.GetLength(1));
            Console.WriteLine("EigenFaces: " + eigenFaces.GetLength(0) + " x " + eigenFaces.GetLength(1));

            //   PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis(eigenVectors);
            //   pca.Compute();

            Console.WriteLine("Done computing");
            double[,] firstEigenFace = Tools.GetVectorFromTableInTable(eigenFacesT, 0, 1);
            double[,] firstDiffFace = Tools.GetVectorFromTableInTable(diffVectors, 100, 1);
            double[,] firstDiffFaceT = Accord.Math.Matrix.Transpose(firstDiffFace);

            double[,] wage = Accord.Math.Matrix.Dot(eigenFacesT, diffVectorsT); // wage[eigenface,image]

            double minDiff = double.MaxValue;
            int indexOfSimilarFace = 0;

            int j = 0;
            for (; j < 400; ++j)
            {
                double difference = 0;
                for (int i = 0; i < 400; ++i)
                {
                    difference += Math.Abs(wage[i, j] - wage[i, 13]);
                    //Console.WriteLine(i + ": " + Math.Abs(wage[i, 0] - wage[i,2]));

                }


                if (minDiff > difference && difference != 0)
                {
                    minDiff = difference;
                    indexOfSimilarFace = j;

                }
                if (difference < 400000000) Console.WriteLine("Difference for:" + j + "=" + difference);
            }
            Console.WriteLine("Difference: " + minDiff + " index: " + indexOfSimilarFace);


            Bitmap diffExampleImage = Tools.CreateBitMapFromBytes(Tools.GetVectorFromTable(allVectors, indexOfSimilarFace), 92, 112);


            Dispatcher.Invoke(() =>
            {
                image.Source = BitmapToImageSource(diffExampleImage);
            });

            */


            Console.WriteLine("Done");
            Console.ReadKey();

        }

        #endregion

        #region privatemethods

        private void LoadLearningSet()
        {
            Console.WriteLine("Loading images from: " + pathToLearningSet + "...");

            List<List<double>> temporarySetOfLoadedImages = new List<List<double>>();

            foreach (string dir in Directory.GetDirectories(pathToLearningSet))
            {
                foreach (string file in Directory.GetFiles(dir))
                {
                    List<double> currentImage = new List<double>();

                    if (Path.GetExtension(file) == ".pgm")
                    {
                        var tempVector = Tools.GetImageVectorInList(file);

                        for (int k = 0; k < tempVector.Count; ++k)
                        {
                            currentImage.Add(tempVector.ElementAt(k));
                        }

                        namesOfPeople.Add(Path.GetFileName(Path.GetDirectoryName(file)));
                    }

                    temporarySetOfLoadedImages.Add(currentImage);
                }
            }

            unprocessedVectors.LoadFromListOfList(temporarySetOfLoadedImages, 1);
        }

        #endregion


    }
}
