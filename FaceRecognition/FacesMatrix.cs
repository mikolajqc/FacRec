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

        //matrix of values that represent bytes that can represents bytes in pgm file
        private double[,] content;
        private int lenghtOfVector;
        private int numberOfVectors;

        public FacesMatrix()
        {
            lenghtOfVector = 0;
            numberOfVectors = 0;
        }

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


        /// <summary>
        /// Method loads all .pgm files from given directory. Specified structure of catalogues is required.
        /// </summary>
        /// <param name="directory">directory with learning set (.pgm files)</param>
        public void LoadFromDirectory(string directory)
        {
            List<List<double>> temporarySetOfLoadedImages = new List<List<double>>();

            foreach (string dir in Directory.GetDirectories(directory))
            {
                List<double> currentImage = new List<double>();

                foreach (string file in Directory.GetFiles(dir))
                {

                    if (Path.GetExtension(file) == ".pgm")
                    {
                        Console.WriteLine("Loading file: " + file + "...");

                        var tempVector = Tools.GetImageVectorInList(file);
                        lenghtOfVector = tempVector.Count;

                        for (int k = 0; k < lenghtOfVector; ++k)
                        {
                            currentImage.Add(tempVector.ElementAt(k));
                        }
                        ++numberOfVectors;
                    }
                }
                temporarySetOfLoadedImages.Add(currentImage);

            }

            numberOfVectors = temporarySetOfLoadedImages.Count;
            lenghtOfVector = temporarySetOfLoadedImages[0].Count;

            content = Tools.GetContentFromListOfList(temporarySetOfLoadedImages, 1);

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
                if (orientation == 0) result[i,1] = content[i, numberOfVector];
                else result[i,1] = content[numberOfVector, i];
            }

            return result;
        }
    }
}
