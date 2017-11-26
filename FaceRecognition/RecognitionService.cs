using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition
{
    public class RecognitionService
    {
        /*
        private FacesMatrix averageVector = null;
        private FacesMatrix eigenFacesT = null;
        const int WIDTH = 92;
        const int HEIGHT = 112;
        const int ERROR_TOLERANCE = 70000000;

        public RecognitionService(LearningInfo learningInfo)
        {
            averageVector = new FacesMatrix(learningInfo.averageVector, 1);
            eigenFacesT = new FacesMatrix(learningInfo.eigenFaces, 1); //orientacja 1 bo tworzymy EigenFacesT czyli gdzie X jest = 400
        }

        /// <summary>
        /// Temporarily this returns string that is stored in namesOfPeople List
        /// </summary>
        /// <param name="bitMapWithFace"></param>
        /// <returns></returns>
        public string Recognize(Bitmap bitMapWithFace) // temporary: Bitmap zamienic na wlasny typ FaceImage ktory obsluguje pgm itd
        {
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
            return namesOfPeople.ElementAt(numberOfString);
        }


        private double[] GetWagesOfImageInEigenFacesSpace(Bitmap bitMap)
        {
            Bitmap scaledBitmap = new Bitmap(bitMap, new Size(WIDTH, HEIGHT));
            FacesMatrix vectorOfFaceInMatrix = GetFaceMatrixFromBitmap(scaledBitmap);
            FacesMatrix diff = vectorOfFaceInMatrix - new FacesMatrix(vectorOfFaceInMatrix.X, averageVector);
            FacesMatrix currentImageWages = eigenFacesT * diff.Transpose();

            return currentImageWages.GetVectorAsArray(0, 0);
        }
        */
    }

}
