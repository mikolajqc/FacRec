using System;
using System.Collections.Generic;
using System.Drawing;
using Accord.Imaging.Filters;
using Accord.Math;

namespace Commons.Utilities
{
    public class FacesMatrix
    {
        /// Orientation - 0 wektory polozone poziomo, 1 - wektory polozone pionowo

        #region fields
        private double[,] _content;

        #endregion

        #region contructors

        public FacesMatrix()
        {
            _content = new double[0, 0];
        }

        /// <summary>
        /// [x,y]
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public FacesMatrix(int x, int y)
        {
            _content = new double[x, y];
        }

        /// <summary>
        /// Creates FacesMatrix that consist of copies of given vector
        /// </summary>
        /// <param name="numberOfCopies"></param>
        /// <param name="vector"></param>
        public FacesMatrix(int numberOfCopies, FacesMatrix vector)
        {
            _content = new double[numberOfCopies, vector.Length];

            for (int i = 0; i < numberOfCopies; ++i)
            {
                for (int j = 0; j < vector.Length; ++j)
                {
                    _content[i, j] = vector.Content[0, j];
                }
            }
        }

        public FacesMatrix(double[,] content)
        {
            _content = content;
        }

        public FacesMatrix(double[] vector, int orientation)
        {
            if (orientation == 0) _content = new double[vector.GetLength(0), 1];
            else _content = new double[1, vector.GetLength(0)];

            for (int i = 0; i < vector.GetLength(0); ++i)
            {
                if (orientation == 0) _content[i, 0] = vector[i];
                else _content[0, i] = vector[i];
            }
        }

        public FacesMatrix(List<double[]> content, int orientation)
        {
            int x, y;

            if (content.Count == 0)
            {
                _content = new double[0, 0];
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

            _content = new double[x, y];

            for (int i = 0; i < x; ++i)
            {
                for (int j = 0; j < y; ++j)
                {
                    if (orientation == 0) _content[i, j] = content[j][i];
                    else _content[i, j] = content[i][j];
                }
            }
        }

        /// <summary>
        /// Creates FacesMatrix from bitmap. Icludes histogram equalization. Transforms bitmap into grayscaled image and stores it to FacesMatrix
        /// </summary>
        /// <param name="bitmap"></param>
        public FacesMatrix(Bitmap bitmap)
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

            _content = content;
        }

        #endregion

        #region properties

        public double[,] Content
        {
            get { return _content; }

            set { _content = value; }
        }

        public int X
        {
            get
            {
                if (_content == null) return 0;
                return _content.GetLength(0);
            }
        }

        public int Y
        {
            get
            {
                if (_content == null) return 0;
                return _content.GetLength(1);
            }
        }

        public int Length
        {
            get { return _content.Length; }
        }

        #endregion

        #region methods

        public void LoadFromListOfList(List<List<double>> listOfLists, int orientation)
        {
            if (listOfLists.Count == 0)
            {
                _content = new double[0, 0];
            }

            if (orientation == 0) _content = new double[listOfLists[0].Count, listOfLists.Count];
            else _content = new double[listOfLists.Count, listOfLists[0].Count];


            for (int x = 0; x < listOfLists.Count; ++x)
            {
                for (int y = 0; y < listOfLists[0].Count; ++y)
                {
                    if (orientation == 0)
                    {
                        _content[y, x] = listOfLists[x][y];
                    }
                    else
                    {
                        _content[x, y] = listOfLists[x][y];
                    }
                }
            }
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
                    if (orientation == 0) sumOfPixelOnOnePosition += _content[j, i];
                    else sumOfPixelOnOnePosition += _content[i, j];
                }

                sumVector[j] = sumOfPixelOnOnePosition / numberOfVectors;
            }

            return new FacesMatrix(sumVector, 1);
        }

        public FacesMatrix Transpose()
        {
            FacesMatrix transposedMatrix = new FacesMatrix(Y, X);

            for (int i = 0; i < X; ++i)
            {
                for (int j = 0; j < Y; ++j)
                {
                    transposedMatrix.Content[j, i] = _content[i, j];
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
        public double[] GetVectorAsArray(int numberOfVector, int orientation)
        {
            int lengthOfVector = _content.GetLength(orientation);

            double[] result = new double[lengthOfVector];

            for (int i = 0; i < lengthOfVector; ++i)
            {
                if (orientation == 0) result[i] = _content[i, numberOfVector];
                else result[i] = _content[numberOfVector, i];
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
                for (int j = 0; j < lenghtOfVectors; ++j)
                {
                    if (orientation == 0) currentVectorInArray[j] = _content[j, i];
                    else currentVectorInArray[j] = _content[i, j];
                }

                result.Add(currentVectorInArray);
            }

            return result;
        }

        public double[][] GetMatrixAsArrayOfArray(int orientation)
        {
            return GetMatrixAsListOfArrays(orientation).ToArray();
        }

        public FacesMatrix GetFirstVectors(int numberOfFirstVectors, int orientation)
        {
            double[,] currentContent;
            int vectorSize;

            if (orientation == 0)
            {
                currentContent = new double[X, numberOfFirstVectors];
                vectorSize = X;
            }
            else
            {
                currentContent = new double[numberOfFirstVectors, Y];
                vectorSize = Y;
            }

            for (int i = 0; i < numberOfFirstVectors; ++i)
            {
                for (int j = 0; j < vectorSize; ++j)
                {
                    if (orientation == 0)
                    {
                        currentContent[j, i] = _content[j, i];
                    }
                    else
                    {
                        currentContent[i, j] = _content[i, j];
                    }
                }
            }

            return new FacesMatrix(currentContent);
        }

        #endregion

        #region operators

        public static FacesMatrix operator -(FacesMatrix a, FacesMatrix b)
        {
            FacesMatrix result = new FacesMatrix(a.X, a.Y);

            //if for DEBUG time only
            if (a.X != b.X || a.Y != b.Y)
            {
                Console.WriteLine("FacesMatrixes must have the same sizes!");
                return null;
            }

            for (int i = 0; i < a.X; ++i)
            {
                for (int j = 0; j < a.Y; ++j)
                {
                    result._content[i, j] = a._content[i, j] - b._content[i, j];
                }
            }

            return result;
        }

        public static FacesMatrix operator +(FacesMatrix a, FacesMatrix b)
        {
            FacesMatrix result = new FacesMatrix(a.X, a.Y);

            //if for DEBUG time only
            if (a.X != b.X || a.Y != b.Y)
            {
                Console.WriteLine("FacesMatrixes must have the same sizes!");
                return null;
            }
            else
            {
                for (int i = 0; i < a.X; ++i)
                {
                    for (int j = 0; j < a.Y; ++j)
                    {
                        result._content[i, j] = a._content[i, j] + b._content[i, j];
                    }
                }
            }

            return result;
        }

        public static FacesMatrix operator *(FacesMatrix a, FacesMatrix b)
        {
            //if for DEBUG time only
            if (a.X != b.Y)
            {
                Console.WriteLine("FaceMatrixes cannot be multiplied!");
                return null;
            }

            return new FacesMatrix(Matrix.Dot(b.Content, a.Content));
        }

        #endregion
    }
}
