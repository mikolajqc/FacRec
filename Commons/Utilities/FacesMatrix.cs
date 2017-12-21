using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using Accord.Imaging.Filters;
using Accord.Math;

namespace Commons.Utilities
{
    //TODO: Tests as quickly as possible
    public class FacesMatrix
    {
        /// Jak mapowac obraz na czlowieka i gdzie to reprezentowac?
        /// dodac obsluge innych rozszerzen niz .pgm
        /// - ogarnac jak pobierac nazwe osoby poprzez nazwe folderu w katalogu zbioru uczacego
        /// - czy ta klasa nie powinna byc niezalezna od tego co posiada?
        /// Orientation - 0 wektory polozone poziomo, 1 - wektory polozone pionowo

        #region fields
        private double[,] _content;

        #endregion

        #region contructors

        public FacesMatrix()
        {
            _content = new double[0,0];
        }
        /// <summary>
        /// [x,y]
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public FacesMatrix(int x, int y)
        {
            _content = new double[x,y];
        }

        /// <summary>
        /// Creates FacesMatrix that consist of copies of given vector
        /// </summary>
        /// <param name="numberOfCopies"></param>
        /// <param name="vector"></param>
        public FacesMatrix(int numberOfCopies, FacesMatrix vector)
        {
            _content = new double[numberOfCopies, vector.Length];

            for(int i = 0; i < numberOfCopies; ++i)
            {
                for(int j = 0; j < vector.Length; ++j)
                {
                    _content[i, j] = vector.Content[0,j];
                }
            }
        }

        public FacesMatrix(double[,] matrix)
        {
            _content = matrix;
        }

        public FacesMatrix(double[] vector, int orientation)
        {
            if(orientation == 0) _content = new double[vector.GetLength(0), 1];
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

            if(content.Count == 0)
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

            _content = new double[x,y];

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

            _content = content;
        }

        #endregion

        #region properties

        public double[,] Content
        {
            get
            {
                return _content;
            }

            set
            {
                _content = value;
            }
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
            get
            {
                return _content.Length;
            }
        }

        #endregion

        #region methods

        public void LoadFromListOfList(List<List<double>> listOfVectors, int orientation)
        {
            if (listOfVectors.Count == 0)
            {
                _content =  new double[0, 0];
            }

            if (orientation == 0) _content = new double[listOfVectors[0].Count, listOfVectors.Count];
            else _content = new double[listOfVectors.Count, listOfVectors[0].Count];


            for (int x = 0; x < listOfVectors.Count; ++x)
            {
                for (int y = 0; y < listOfVectors[0].Count; ++y)
                {
                    if (orientation == 0)
                    {
                        _content[y, x] = listOfVectors[x][y];
                    }
                    else
                    {
                        _content[x, y] = listOfVectors[x][y];
                    }
                }
            }

        }

        public void PushBackVector(double[] newVector, int orientation)
        {
            //for tests only:

            //checks if it was initialized
            if (_content.Length == 0)
            {
                if (orientation == 0) _content = new double[newVector.Length, 1];
                else _content = new double[1, newVector.Length];
            }

            double[,] newContent;
            if (orientation == 0) newContent = new double[X, Y + 1];
            else newContent = new double[X + 1, Y];

            for (int i = 0; i < X; ++i)
            {
                for (int j = 0; j < Y;++j)
                {
                    newContent[i,j] = _content[i,j];
                }
            }


            for (int i = 0; i < newVector.Length; ++i)
            {
                if (orientation == 0) newContent[i, Y] = newVector[i];
                else newContent[X, i] = newVector[i];
            }

            _content = newContent;
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
                    if(orientation == 0) sumOfPixelOnOnePosition += _content[j, i];
                    else sumOfPixelOnOnePosition += _content[i, j];
                }

                sumVector[j] = sumOfPixelOnOnePosition / numberOfVectors;
            }

            return new FacesMatrix(sumVector,1);
        }

        /// <summary>
        /// Method returns average vector of n vectors. It takes number of first vector and number of vectors to calculate
        /// </summary>
        /// <param name="orientation"></param>
        /// <param name="indexOfFirstVector"></param>
        /// <param name="numberOfVectors"></param>
        /// <returns></returns>
        public FacesMatrix GetAverageVector(int orientation, int indexOfFirstVector, int numberOfVectors)
        {
            int lengthOfVector;
            int maxIndexOfVector = indexOfFirstVector + numberOfVectors;

            if (orientation == 0)
            {
                lengthOfVector = X;
            }
            else
            {
                lengthOfVector = Y;
            }

            double[] sumVector = new double[lengthOfVector];

            for (int j = 0; j < lengthOfVector; ++j)
            {
                double sumOfPixelOnOnePosition = 0;

                for (int i = indexOfFirstVector; i < maxIndexOfVector; ++i)
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

            for(int i = 0; i < X; ++i)
            {
                for(int j = 0; j < Y; ++j)
                {
                    transposedMatrix.Content[j,i] = _content[i,j];
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
            int lengthOfVector = _content.GetLength(orientation);

            double[] result = new double[lengthOfVector];

            for(int i = 0; i < lengthOfVector; ++i)
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
                for(int j = 0; j < lenghtOfVectors; ++j)
                {
                    if (orientation == 0) currentVectorInArray[j] = _content[j,i];
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
                for(int j = 0; j < vectorSize; ++j)
                {
                    if(orientation == 0)
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

        //todo: test of method Concatenate - nie concatenuje sie dla i > 0
        public void Concatenate(FacesMatrix secondMatrix, int orientation)
        {
            double[,] newContent;
            
            if (_content == null || _content.Length == 0)
            {
                _content = secondMatrix.Content.Copy();
                return;
            }
            

            if (orientation == 0) newContent = new double[X,Y+secondMatrix.Y];
            else newContent = new double[X + secondMatrix.X, Y];

            for (int i = 0; i < X; ++i)
            {
                for (int j = 0; j < Y; ++j)
                {
                    newContent[i, j] = _content[i, j];
                }
            }

            for (int i = X; i < secondMatrix.X + X; ++i)
            {
                for (int j = 0; j < secondMatrix.Y; ++j)
                {
                    newContent[i, j] = secondMatrix.Content[i - X, j];
                }
            }

            _content = newContent;
        }

        public FacesMatrix InverseMatrix()
        {
            return new FacesMatrix(_content.Inverse());
        }

        //todo: napraw bug w tej funkcji - ona nie dziala poprawnie - dziala 
        public FacesMatrix GetPartOfMatrix(int indexOfFirstVector, int numberOfVectors, int orientation)
        {
            int lenghtOfVectors;
            double[,] result;

            if (orientation == 0)
            {
                lenghtOfVectors = X;
                result = new double[X,numberOfVectors];
            }
            else
            {
                lenghtOfVectors = Y;
                result = new double[numberOfVectors, Y];
            }

            for (int i = indexOfFirstVector; i < numberOfVectors + indexOfFirstVector; ++i)
            {
                for (int j = 0; j < lenghtOfVectors; ++j)
                {
                    if (orientation == 0)
                    {
                        result[j, i - indexOfFirstVector] = _content[j, i];
                    }
                    else
                    {
                        result[i - indexOfFirstVector, j] = _content[i, j];
                    }
                }
            }

            return new FacesMatrix(result);
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
                        result._content[i, j] = a._content[i, j] - b._content[i, j];
                    }
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

        //todo: zmienić tak, żeby intuicyjnie mnożyło. Aktualnie mnoży w dziwny sposob bo dla Accord.Math.Matrix.Dot .X jest tym czym dla nas .Y
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

        //todo:test
        public static FacesMatrix operator *(int a, FacesMatrix b)
        {
            FacesMatrix result = new FacesMatrix(b.Content);

            for (int i = 0; i < b.X; ++i)
            {
                for (int j = 0; j < b.Y; ++j)
                {
                    result.Content[i,j] = result.Content[i,j] * a;
                }
            }

            return result;
        }

        #endregion
    }
}
