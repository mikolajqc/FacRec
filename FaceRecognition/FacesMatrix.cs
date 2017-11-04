using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition
{
    class FacesMatrix
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
