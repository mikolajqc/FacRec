using Accord.Imaging.Filters;
using Accord.Math.Decompositions;
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
        #region fields

        private FacesMatrix unprocessedVectors = null;
        private FacesMatrix averageVector = null; //zmienic typ na FacesMatrix !!!
        private FacesMatrix wages = null; // [eigenface,image]
        private FacesMatrix eigenFacesT = null;

        private List<string> namesOfPeople = null;
        private string pathToLearningSet = null;

        #endregion

        #region constructors

        public FaceRecognition(string pathToLearningSet)
        {
            this.pathToLearningSet = pathToLearningSet;
            unprocessedVectors = new FacesMatrix();
            wages = new FacesMatrix();
            namesOfPeople = new List<string>();
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
            Console.WriteLine("Recognizing...");
            FacesMatrix vectorOfFaceInMatrix = GetFaceMatrixFromBitmap(bitMapWithFace);
            FacesMatrix diff =  vectorOfFaceInMatrix - new FacesMatrix(vectorOfFaceInMatrix.X, averageVector);

            FacesMatrix currentImageWages = eigenFacesT * diff.Transpose();

            double minSumOfDifferenceOfWages = double.MaxValue;
            int numberOfString = 0;

            for (int numberOfKnownImage = 0; numberOfKnownImage < wages.Y; ++numberOfKnownImage)
            {
                double currentSumOfDifferenceOfWages = 0;
                for (int numbeOfEigenFace = 0; numbeOfEigenFace < wages.X; ++numbeOfEigenFace)
                {
                    currentSumOfDifferenceOfWages += Math.Abs(wages.Content[numbeOfEigenFace, numberOfKnownImage] - currentImageWages.Content[numbeOfEigenFace, 0]);
                }

                if (minSumOfDifferenceOfWages > currentSumOfDifferenceOfWages && currentSumOfDifferenceOfWages != 0) //!= 0 do usuniecia - tylko dla testow
                {
                    minSumOfDifferenceOfWages = currentSumOfDifferenceOfWages;
                    numberOfString = numberOfKnownImage;
                }
            }

            ///do ogarniecia jakis zakres bledu, np mniejsze niz jakas liczba w odleglosci euklidesowej
            ///poniżej tylko dla testow - do usuniecia

            Console.WriteLine("Wage: " + minSumOfDifferenceOfWages);
            if (minSumOfDifferenceOfWages > 350000000)
            {
                return "uknown";
            }
            
            return namesOfPeople.ElementAt(numberOfString);
        }

        public void Learn()
        {
            Console.WriteLine("Learning...");

            LoadLearningSet();
            averageVector = unprocessedVectors.GetAverageVector(1);
            FacesMatrix differenceVectors = unprocessedVectors - new FacesMatrix(400,averageVector);
            FacesMatrix differenceVectorsT = differenceVectors.Transpose();
            FacesMatrix covariation = differenceVectors * differenceVectorsT;

            EigenvalueDecomposition decomposition = new EigenvalueDecomposition(covariation.Content, true, true); // todo: wlasna dekompozycja

            FacesMatrix eigenVectors = new FacesMatrix(decomposition.Eigenvectors);
            FacesMatrix eigenVectorsT = eigenVectors.Transpose();

            FacesMatrix eigenFaces = differenceVectorsT * eigenVectors;
            eigenFacesT = eigenFaces.Transpose();

            wages = eigenFacesT * differenceVectorsT;

            Console.WriteLine("Done");

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
                    if (Path.GetExtension(file) == ".pgm")
                    {
                        temporarySetOfLoadedImages.Add(Tools.GetImageVectorInList(file));
                        namesOfPeople.Add(Path.GetFileName(Path.GetDirectoryName(file)));

                    }
                }
            }

            unprocessedVectors.LoadFromListOfList(temporarySetOfLoadedImages, 1);
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

        #endregion

    }
}
