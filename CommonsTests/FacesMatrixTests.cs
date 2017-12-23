using Commons.Utilities;
using NUnit.Framework;

namespace CommonsTests
{
    [TestFixture]
    public class FacesMatrixTests
    {

        [Test]
        public void CheckContentConstructor()
        {
            double[,] testContent = {{1, 2}, {3, 4}};
            FacesMatrix facesMatrix = new FacesMatrix(testContent);
            Assert.AreEqual(facesMatrix.Content, testContent);
        }

        [Test]
        public void GetAverageVectorTest()
        {
            var facesMatrix = new FacesMatrix();
            facesMatrix.Content = new double[,] { { 1,2}, { 3,4} };
            Assert.AreEqual(facesMatrix.GetAverageVector(1).GetVectorAsArray(0,1), new double[] {2, 3});
        }
    }
}
