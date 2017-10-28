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
        ///Jak mapowac obraz na czlowieka i gdzie to reprezentowac?
        ///dodac obsluge innych rozszerzen niz .pgm
        /// - ogarnac jak pobierac nazwe osoby poprzez nazwe folderu w katalogu zbioru uczacego

        #region fields
        //matrix of values that represent bytes that can represents bytes in pgm file
        private double[,] content;
        private int lenghtOfVector;
        private int numberOfVectors;

        private List<string> namesOfPeople;

        #endregion

        #region contructors

        public FacesMatrix()
        {
            lenghtOfVector = 0;
            numberOfVectors = 0;
            namesOfPeople = new List<string>();
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
        public FacesMatrix(int numberOfCopies, double[] vector)
        {
            content = new double[numberOfCopies, vector.Length];

            for(int i = 0; i < numberOfCopies; ++i)
            {
                for(int j = 0; j < vector.Length; ++j)
                {
                    content[i, j] = vector[j];
                }
            }

            numberOfVectors = numberOfCopies;
            lenghtOfVector = vector.Length;
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

        public List<string> PeopleNames
        {
            get
            {
                return namesOfPeople;
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

        /// <summary>
        /// Czy aby na pawno ta funkcja tutaj byc powinna???!!!!
        /// Moze lepiej ja wyciagnac stad i zostawic tutaj obsluge niskopoziomowa samej macierzy?!!!!!!!!!!!
        /// Wtedy mozna wywalic calkowicie cos takiego jak orientation i zrobic to wyzej! - hmm ale w sumie orientacja chyba musi byc
        /// 
        /// Method loads all .pgm files from given directory. Specified structure of catalogues is required.
        /// </summary>
        /// <param name="directory">directory with learning set (.pgm files)</param>
        /*
        public void LoadFromDirectory(string directory)
        {
            Console.WriteLine("Loading images from: " + directory + "...");

            List<List<double>> temporarySetOfLoadedImages = new List<List<double>>();

            foreach (string dir in Directory.GetDirectories(directory))
            {
                foreach (string file in Directory.GetFiles(dir))
                {
                    List<double> currentImage = new List<double>();

                    if (Path.GetExtension(file) == ".pgm")
                    {
                        var tempVector = Tools.GetImageVectorInList(file);
                        lenghtOfVector = tempVector.Count;

                        for (int k = 0; k < lenghtOfVector; ++k)
                        {
                            currentImage.Add(tempVector.ElementAt(k));
                        }
                        ++numberOfVectors;

                        PeopleNames.Add(Path.GetFileName(Path.GetDirectoryName(file)));
                    }

                    temporarySetOfLoadedImages.Add(currentImage);
                }
                

            }
            lenghtOfVector = temporarySetOfLoadedImages[0].Count;
            content = Tools.GetContentFromListOfList(temporarySetOfLoadedImages, 1);

        }

        */

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

        public double[] GetAverageVector(int orientation)
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

            return sumVector;
        }

        #endregion

        #region operators

        public static FacesMatrix operator- (FacesMatrix a, FacesMatrix b)
        {
            FacesMatrix result = new FacesMatrix(a.content.GetLength(0), a.content.GetLength(1));

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

        #endregion
    }
}
