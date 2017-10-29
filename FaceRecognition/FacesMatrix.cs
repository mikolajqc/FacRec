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
        /// - Czy pola o dlugosci wektora oraz liczby wektorow maja jakis sens?? czy ta klasa nie powinna byc niezalezna od tego co posiada?

        #region fields
        //matrix of values that represent bytes that can represents bytes in pgm file
        private double[,] content;

        //to delete?
        private int lenghtOfVector;
        private int numberOfVectors;

        #endregion

        #region contructors

        public FacesMatrix()
        {
            lenghtOfVector = 0;
            numberOfVectors = 0;
        }
        /// <summary>
        /// [x,y]
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public FacesMatrix(int x, int y, int orientation)
        {
            content = new double[x,y];

            if (orientation == 0)
            {
                numberOfVectors = y;
                lenghtOfVector = x;
            }
            else
            {
                numberOfVectors = x;
                lenghtOfVector = y;
            }
        }

        /// <summary>
        /// Creates FacesMatrix that consist of copies of given vector
        /// </summary>
        /// <param name="numberOfCopies"></param>
        /// <param name="vector"></param>
        public FacesMatrix(int numberOfCopies, FacesMatrix vector)
        {
            content = new double[numberOfCopies, vector.Content.Length];

            for(int i = 0; i < numberOfCopies; ++i)
            {
                for(int j = 0; j < vector.Content.Length; ++j)
                {
                    content[i, j] = vector.Content[0,j];
                }
            }

            numberOfVectors = numberOfCopies;
            lenghtOfVector = vector.Content.Length;
        }

        public FacesMatrix(double[,] matrix, int orientation)
        {
            content = matrix;

            if(orientation == 0)
            {
                numberOfVectors = matrix.GetLength(1);
                lenghtOfVector = matrix.GetLength(0);
            }
            else
            {
                numberOfVectors = matrix.GetLength(0);
                lenghtOfVector = matrix.GetLength(1);
            }

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

        public int LenghtOfVector
        {
            get
            {
                return lenghtOfVector;
            }
        }

        public int NumberOfVectors
        {
            get
            {
                return numberOfVectors;
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

            lenghtOfVector = listOfVectors[0].Count;
            numberOfVectors = listOfVectors.Count;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberOfVector"></param>
        /// <param name="orientation">
        /// 0: vector1
        ///    vector2
        ///    vector3
        ///    .
        ///    .
        ///    .
        ///    
        ///!0: vvvvv
        ///    eeeee
        ///    ccccc
        ///    ttttt
        ///    ooooo
        ///    rrrrr
        ///    12345
        /// </param>
        /// <returns></returns>

        public double[] GetVector(uint numberOfVector, int orientation) // 0 - [_, index], 0 !=  [index, _]
        {
            double[] result = new double[lenghtOfVector];

            for(uint i = 0; i < lenghtOfVector; ++i)
            {
                if (orientation == 0) result[i] = content[i, numberOfVector];
                else result[i] = content[numberOfVector, i];
            }

            return result;
        }

        public List<double> GetVectorAsList(uint numberOfVector, int orientation) // 0 - [_, index], 0 !=  [index, _]
        {
            List<double> result = new List<double>();

            for (uint i = 0; i < lenghtOfVector; ++i)
            {
                if (orientation == 0) result.Add(content[i, numberOfVector]);
                else result.Add(content[numberOfVector, i]);
            }

            return result;
        }

        public double[,] GetVectorInTable(int numberOfVector, int orientation)
        {
            double[,] result = new double[lenghtOfVector, 1];

            for (int i = 0; i < lenghtOfVector; ++i)
            {
                if (orientation == 0) result[i,0] = content[i,numberOfVector];
                else result[i,0] = content[numberOfVector, i];
            }

            return result;
        }

        public FacesMatrix GetAverageVector(int orientation)
        {
            double[] sumVector = new double[lenghtOfVector];

            for (int j = 0; j < lenghtOfVector; ++j)
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
            FacesMatrix transposedMatrix = new FacesMatrix(content.GetLength(1), content.GetLength(0), 1);

            for(int i = 0; i < content.GetLength(0); ++i)
            {
                for(int j = 0; j < content.GetLength(1); ++j)
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
            FacesMatrix result = new FacesMatrix(a.content.GetLength(0), a.content.GetLength(1), 1);

            //if jest potrzebny? jak nie spelni to w sumie wywali exception
            if(a.content.GetLength(0) != b.content.GetLength(0) || a.content.GetLength(1) != b.content.GetLength(1))
            {
                Console.WriteLine("FacesMatrixes must have the same sizes!");
                return null;
            }
            else
            {
                for(int i = 0; i < a.content.GetLength(0); ++i)
                {
                    for (int j = 0; j < a.content.GetLength(1); ++j)
                    {
                        result.content[i, j] = a.content[i, j] - b.content[i, j];
                    }
                }
            }

            return result;
        }

        public static FacesMatrix operator* (FacesMatrix a, FacesMatrix b)
        {
            FacesMatrix result = new FacesMatrix(a.Content.GetLength(1), b.Content.GetLength(0), 1);

            if(a.Content.GetLength(1) != b.Content.GetLength(0))
            {
                Console.WriteLine("FaceMatrixes cannot be multiplied!");
                return null;
            }

            return new FacesMatrix(Accord.Math.Matrix.Dot(a.Content, b.Content), 1);
        }

        #endregion
    }
}
