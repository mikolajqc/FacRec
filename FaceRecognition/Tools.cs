using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Imaging.Formats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition
{
    static class Tools
    {
        static T[,] GetLearningSet<T>(string pathToDirectoryWithLearningSet)
        {
            throw new NotImplementedException();
        }

        static T[] GetImageAsVector<T>(string pathToImage)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Temporary - to delete on release
        /// </summary>
        /// <param name="pathToImage"></param>
        /// <returns></returns>
        public static double[] GetImageVector(string pathToImage)
        {
            Bitmap bitmap = ImageDecoder.DecodeFromFile(pathToImage);
            HistogramEqualization histogramEqualization = new HistogramEqualization();
            bitmap = histogramEqualization.Apply(bitmap);


            int width = bitmap.Size.Width;
            int height = bitmap.Size.Height;

            double[] resultVector = new double[width * height];

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    Color color = bitmap.GetPixel(x, y);

                    double r = color.R;
                    double g = color.G;
                    double b = color.B;
                    double grayscale = (r + g + b) / 3f;

                    resultVector[y * width + x] = grayscale;

                }
            }

            return resultVector;
        }

        public static List<double> GetImageVectorInList(string pathToImage)
        {
            Bitmap bitmap = ImageDecoder.DecodeFromFile(pathToImage);
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

                    double r = color.R;
                    double g = color.G;
                    double b = color.B;
                    double grayscale = (r + g + b) / 3f;

                    resultVector.Add(grayscale);

                }
            }

            return resultVector;
        }

        public static double[,] GetContentFromListOfList(List<List<double>> listOfVectors, int orientation)
        {
            if (listOfVectors.Count == 0)
            {
                return new double[0, 0];
            }

            double[,] result = null;
            if(orientation == 0) result = new double[listOfVectors[0].Count, listOfVectors.Count];
            else result = new double[listOfVectors.Count, listOfVectors[0].Count];


            for (int x = 0; x < listOfVectors.Count; ++x)
            {
                for(int y = 0; y < listOfVectors[0].Count; ++y)
                {
                    if (orientation == 0)
                    {
                        result[y, x] = listOfVectors[x][y];
                    }
                    else
                    {
                        result[x, y] = listOfVectors[x][y];
                    }
                }
            }

            return result;
        }
    }
}
