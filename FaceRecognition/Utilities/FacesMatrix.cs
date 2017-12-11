using Accord.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace FaceRecognition.Utilities
{
    public class FacesMatrix
    {
        ///TODO:
        /// Jak mapowac obraz na czlowieka i gdzie to reprezentowac?
        /// dodac obsluge innych rozszerzen niz .pgm
        /// - ogarnac jak pobierac nazwe osoby poprzez nazwe folderu w katalogu zbioru uczacego
        /// - czy ta klasa nie powinna byc niezalezna od tego co posiada?
        /// Orientation - 0 wektory polozone poziomo, 1 - wektory polozone pionowo

        #region fields
        private double[,] content;

        #endregion

        #region contructors

        public FacesMatrix()
        {
            content = new double[0,0];
        }
        /// <summary>
        /// [x,y]
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public FacesMatrix(int x, int y)
        {
            content = new double[x,y];
        }

        /// <summary>
        /// Creates FacesMatrix that consist of copies of given vector
        /// </summary>
        /// <param name="numberOfCopies"></param>
        /// <param name="vector"></param>
        public FacesMatrix(int numberOfCopies, FacesMatrix vector)
        {
            content = new double[numberOfCopies, vector.Length];

            for(int i = 0; i < numberOfCopies; ++i)
            {
                for(int j = 0; j < vector.Length; ++j)
                {
                    content[i, j] = vector.Content[0,j];
                }
            }
        }

        public FacesMatrix(double[,] matrix)
        {
            content = matrix;
        }

        public FacesMatrix(double[] vector, int orientation)
        {
            if(orientation == 0) content = new double[vector.GetLength(0), 1];
            else content = new double[1, vector.GetLength(0)];

            for (int i = 0; i < vector.GetLength(0); ++i)
            {
                if (orientation == 0) content[i, 0] = vector[i];
                else content[0, i] = vector[i];
            }

        }

        public FacesMatrix(List<double[]> content, int orientation)
        {
            
            int x, y;

            if(content.Count == 0)
            {
                this.content = new double[0, 0];
                return;
            }

            if (orientation == 0)
            {
                x = content[0].Length;
                y = content.Count;
            }
            else
            {
                y = content[0].Length;
                x = content.Count;
            }

            this.content = new double[x,y];

            for (int i = 0; i < x; ++i)
            {
                for (int j = 0; j < y; ++j)
                {
                    if (orientation == 0) this.content[i, j] = content[j][i];
                    else this.content[i, j] = content[i][j];


                }
            }
        }

        /// <summary>
        /// Creates FacesMatrix from bitmap. Icludes histogram equalization. Transforms bitmap into grayscaled image and stores it to FacesMatrix
        /// </summary>
        /// <param name="bitmap"></param>
        public FacesMatrix (Bitmap bitmap)
        {
            HistogramEqualization histogramEqualization = new HistogramEqualization(); // proces wyrownania histogramow
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

            this.content = content;
        }

        #endregion

        #region properties

        public double[,] Content
        {
            get
            {
                return content;
            }

            set
            {
                content = value;
            }
        }

        public int X
        {
            get
            {
                return content.GetLength(0);
            }
        }

        public int Y
        {
            get
            {
                return content.GetLength(1);
            }
        }

        public int Length
        {
            get
            {
                return content.Length;
            }
        }

        #endregion

        #region methods

        public void LoadFromListOfList(List<List<double>> listOfVectors, int orientation)
        {
            if (listOfVectors.Count == 0)
            {
                this.content =  new double[0, 0];
            }

            if (orientation == 0) content = new double[listOfVectors[0].Count, listOfVectors.Count];
            else content = new double[listOfVectors.Count, listOfVectors[0].Count];


            for (int x = 0; x < listOfVectors.Count; ++x)
            {
                for (int y = 0; y < listOfVectors[0].Count; ++y)
                {
                    if (orientation == 0)
                    {
                        content[y, x] = listOfVectors[x][y];
                    }
                    else
                    {
                        content[x, y] = listOfVectors[x][y];
                    }
                }
            }

        }

        public void PushBackVector(double[] newVector, int orientation)
        {
            //for tests only:
            double[,] newContent;
            if (orientation == 0) newContent = new double[X, Y + 1];
            else newContent = new double[X + 1, Y];

            for (int i = 0; i < X; ++i)
            {
                for (int j = 0; j < Y;++j)
                {
                    newContent[i,j] = content[i,j];
                }
            }


            for (int i = 0; i < newVector.Length; ++i)
            {
                if (orientation == 0) newContent[i, Y] = newVector[i];
                else newContent[X, i] = newVector[i];
            }

            content = newContent;
        }

        public FacesMatrix GetAverageVector(int orientation)
        {
            int lengthOfVector, numberOfVectors;

            if (orientation == 0)
            {
                lengthOfVector = X;
                numberOfVectors = Y;
            }
            else
            {
                lengthOfVector = Y;
                numberOfVectors = X;
            }

            double[] sumVector = new double[lengthOfVector];

            for (int j = 0; j < lengthOfVector; ++j)
            {
                double sumOfPixelOnOnePosition = 0;

                for (int i = 0; i < numberOfVectors; ++i)
                {
                    if(orientation == 0) sumOfPixelOnOnePosition += content[j, i];
                    else sumOfPixelOnOnePosition += content[i, j];
                }

                sumVector[j] = sumOfPixelOnOnePosition / numberOfVectors;
            }

            return new FacesMatrix(sumVector,1);
        }

        public FacesMatrix Transpose()
        {
            FacesMatrix transposedMatrix = new FacesMatrix(Y, X);

            for(int i = 0; i < X; ++i)
            {
                for(int j = 0; j < Y; ++j)
                {
                    transposedMatrix.Content[j,i] = content[i,j];
                }
            }

            return transposedMatrix;
        }

        /// <summary>
        /// Method returns vector as array. It takes number of vector and orientation (0 or 1)
        /// </summary>
        /// <param name="numberOfVector"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        public double[] GetVectorAsArray(int numberOfVector,int orientation)
        {
            int lengthOfVector = content.GetLength(orientation);

            double[] result = new double[lengthOfVector];

            for(int i = 0; i < lengthOfVector; ++i)
            {
                if (orientation == 0) result[i] = content[i, numberOfVector];
                else result[i] = content[numberOfVector, i];
            }

            return result;
        }

        public List<double[]> GetMatrixAsListOfArrays(int orientation)
        {
            List<double[]> result = new List<double[]>();

            int numbersOfVectors;
            int lenghtOfVectors;

            if (orientation == 0)
            {
                numbersOfVectors = Y;
                lenghtOfVectors = X;
            }
            else
            {
                numbersOfVectors = X;
                lenghtOfVectors = Y;
            }

            for (int i = 0; i < numbersOfVectors; ++i)
            {
                double[] currentVectorInArray = new double[lenghtOfVectors];
                for(int j = 0; j < lenghtOfVectors; ++j)
                {
                    if (orientation == 0) currentVectorInArray[j] = content[j,i];
                    else currentVectorInArray[j] = content[i, j];
                }

                result.Add(currentVectorInArray);
            }

            return result;
        }

        #endregion

        #region operators

        public static FacesMatrix operator- (FacesMatrix a, FacesMatrix b)
        {
            FacesMatrix result = new FacesMatrix(a.X, a.Y);

            //if for DEBUG time only
            if(a.X != b.X || a.Y != b.Y)
            {
                Console.WriteLine("FacesMatrixes must have the same sizes!");
                return null;
            }
            else
            {
                for(int i = 0; i < a.X; ++i)
                {
                    for (int j = 0; j < a.Y; ++j)
                    {
                        result.content[i, j] = a.content[i, j] - b.content[i, j];
                    }
                }
            }

            return result;
        }

        public static FacesMatrix operator* (FacesMatrix a, FacesMatrix b)
        {
            //if for DEBUG time only
            if(a.Y != b.X)
            {
                Console.WriteLine("FaceMatrixes cannot be multiplied!");
                return null;
            }

            return new FacesMatrix(Accord.Math.Matrix.Dot(a.Content, b.Content));
        }

        #endregion
    }
}
