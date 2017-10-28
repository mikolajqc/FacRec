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
        private FacesMatrix unprocessedVectors = null;
        private FacesMatrix wages = null;

        private List<string> namesOfPeople = null;

        private string pathToLearningSet = null;
        private double[] averageVector = null;

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

        public int Recognize(Bitmap bitMapWithFace) // temporary: Bitmap zamienic na wlasny typ FaceImage ktory obsluguje pgm itd
        {
            throw new NotImplementedException();
        }

        public void Learn()
        {
            Console.WriteLine("Learnign...");

            LoadLearningSet();
            averageVector = unprocessedVectors.GetAverageVector(1);
            FacesMatrix differenceVectors = unprocessedVectors - new FacesMatrix(400,averageVector);
            FacesMatrix differenceVectorsT = differenceVectors.Transpose();
            FacesMatrix covariation = differenceVectors * differenceVectorsT;

            EigenvalueDecomposition decomposition = new EigenvalueDecomposition(covariation.Content, true, true); // todo: wlasna dekompozycja

            FacesMatrix eigenVectors = new FacesMatrix(decomposition.Eigenvectors, 1);
            FacesMatrix eigenVectorsT = eigenVectors.Transpose();

            FacesMatrix eigenFaces = differenceVectorsT * eigenVectors;
            FacesMatrix eigenFacesT = eigenFaces.Transpose();

            Console.WriteLine("All: " + unprocessedVectors.LenghtOfVector + " x " + unprocessedVectors.NumberOfVectors);
            Console.WriteLine("EigenFaces: " + eigenFaces.LenghtOfVector + " x " + eigenFaces.NumberOfVectors);

            wages = eigenFacesT * differenceVectorsT;

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
