﻿using System.Collections.Generic;
using Commons.Utilities;
using NUnit.Framework;

namespace CommonsTests
{
    [TestFixture]
    public class FacesMatrixTests
    {
        [Test] //FacesMatrix(int x, int y)
        public void CheckConstructorWithSizes()
        {
            FacesMatrix fM = new FacesMatrix(3,4);
            Assert.AreEqual(fM.Content.GetLength(0),3);
            Assert.AreEqual(fM.Content.GetLength(1), 4);
        }

        [Test] //FacesMatrix(int numberOfCopies, FacesMatrix vector)
        public void CheckContructorWithCopyingVector()
        {
            var facesMatrix = new FacesMatrix(10, new FacesMatrix(new double[,] { { 1, 2, 3 } }));
            for (int i = 0; i < 10; ++i)
            {
                Assert.AreEqual(facesMatrix.Content[i, 0], 1);
                Assert.AreEqual(facesMatrix.Content[i, 1], 2);
                Assert.AreEqual(facesMatrix.Content[i, 2], 3);
            }
        }

        [Test] //FacesMatrix(double[,] content)
        public void CheckContentConstructor()
        {
            double[,] testContent = {{1, 2}, {3, 4}};
            FacesMatrix facesMatrix = new FacesMatrix(testContent);
            Assert.AreEqual(facesMatrix.Content, testContent);
        }

        [Test] //FacesMatrix(double[] vector, int orientation)
        public void CheckConstructorWithArrayOfDouble()
        {
            var facesMatrix = new FacesMatrix(new double[] {0, 1, 2}, 1);
            var facesMatrix1 = new FacesMatrix(new double[] {0, 1, 2}, 0);
            for (var i = 0; i < 3; ++i)
            {
                Assert.AreEqual(facesMatrix.Content[0, i], i);
                Assert.AreEqual(facesMatrix1.Content[i, 0], i);
            }
        }

        [Test] //FacesMatrix(List<double[]> content, int orientation)
        public void CheckConstructorWithListOfArray()
        {
            List<double[]> testContent = new List<double[]>();
            testContent.Add(new[] {0.0, 0.1, 0.2});
            testContent.Add(new[] {1.0, 1.1, 1.2});
            testContent.Add(new[] {2.0, 2.1, 2.2});
            var facesMatrix = new FacesMatrix(testContent, 1);
            var facesMatrix1 = new FacesMatrix(testContent, 0);

            for (var i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    Assert.AreEqual(facesMatrix.Content[i, j], i + 0.1 * j);
                    Assert.AreEqual(facesMatrix1.Content[j, i], i + 0.1 * j);
                }
            }
        }

        [Test]
        public void LoadFromListOfListTest()
        {
            var listOfList = new List<List<double>>();
            for (int i = 0; i < 3; ++i)
            {
                var list = new List<double>();
                for (int j = 0; j < 3; ++j)
                {
                    list.Add(j * 0.1 + i);
                }
                listOfList.Add(list);
            }
            var facesMatrix = new FacesMatrix();
            facesMatrix.LoadFromListOfList(listOfList, 1);
            var facesMatrix1 = new FacesMatrix();
            facesMatrix1.LoadFromListOfList(listOfList, 0);

            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    Assert.AreEqual(facesMatrix.Content[i, j], j * 0.1 + i);
                    Assert.AreEqual(facesMatrix1.Content[j, i], j * 0.1 + i);
                }
            }
        }

        [Test]
        public void GetAverageVectorTest()
        {
            var facesMatrix = new FacesMatrix();
            facesMatrix.Content = new double[,] { { 1,2}, { 3,4} };
            Assert.AreEqual(facesMatrix.GetAverageVector(1).GetVectorAsArray(0,1), new double[] {2, 3});
        }

        [Test]
        public void TransposeTest()
        {
            var facesMatrix = new FacesMatrix(new[,] {{1.0,2.0,3.0}, {4.0,5.0,6.0}, {7.0,8.0,9.0} });
            var facesMatrixT = facesMatrix.Transpose();

            Assert.AreEqual(facesMatrixT.Content, new[,] { { 1.0,4.0,7.0}, { 2.0,5.0,8.0}, { 3.0,6.0,9.0} });
        }

        [Test]
        public void GetVectorAsArrayTest()
        {
            var facesMatrix = new FacesMatrix(new[,] {{0.0, 1.0, 2.0}, {3.0, 4.0, 5.0}, {6.0, 7.0, 8.0}});

            for (int i = 0; i < 3; ++i)
            {
                Assert.AreEqual(facesMatrix.GetVectorAsArray(i, 0), new[] { i, 3.0 + i, 6.0 + i });
                Assert.AreEqual(facesMatrix.GetVectorAsArray(i, 1), new[] { i * 3.0, i*3.0 + 1.0, i*3.0 + 2.0 });
            }
        }

        [Test]
        public void GetMatrixAsListOfArrayTest()
        {
            
        }

        [Test]
        public void GetMatrixAsArrayOfArrayTest()
        {

        }

        [Test]
        public void GetFirstVectorTest()
        {

        }

        [Test]
        public void OperatorMinusTest()
        {

        }

        [Test]
        public void OperatorPlusTest()
        {

        }

        [Test]
        public void OperatorMultTest()
        {

        }
    }
}
