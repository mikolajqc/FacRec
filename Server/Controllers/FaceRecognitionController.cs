using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FaceRecognition;
using System.Web;
using System.Threading.Tasks;
using Client;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web.Http.Dependencies;
using Newtonsoft.Json;

namespace Server.Controllers
{
    public class FaceRecognitionController : ApiController
    {

        [Route("api/FaceRecognition/Learn")]
        public async Task<string> Learn()
        {
            IFaceRecognition fR = new FaceRecognition.FaceRecognition(@"D:\Studia\Inzynierka\LearningSet_AT&T\");

            LearningInfo learningInfo = fR.Learn();
            await InsertLearningInfoToDatabase(learningInfo);

            return "Learnt!";
        }

        [Route("api/FaceRecognition/Recognize")]
        public HttpResponseMessage Recognize(Request request)
        {
            FaceRecognition.IFaceRecognition fR = new FaceRecognition.FaceRecognition(@"D:\Studia\Inzynierka\LearningSet_AT&T\");

            fR.Learn();

            string req = request.Name;
            byte[] bitmap = request.BitmapInArray;

            Bitmap bitmapWithFace = new Bitmap(Image.FromStream(new MemoryStream(bitmap)));

            string resultOfRecognition = fR.Recognize(bitmapWithFace);

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "FaceRecognition response");
            response.Content = new StringContent(resultOfRecognition, Encoding.Unicode);

            return response;
        }

        [Route("api/FaceRecognition/AddFace")]
        public HttpResponseMessage AddFace(Request request)
        {
            return Request.CreateResponse(HttpStatusCode.OK, "Face added!");
        }


        public async Task<int> InsertLearningInfoToDatabase(LearningInfo learningInfo)
        {
            AverageVectorsController averageController = new AverageVectorsController();
            EigenFacesController eigenFaceController = new EigenFacesController();

            await averageController.PostAverageVector(new Models.AverageVector { Value = JsonConvert.SerializeObject(learningInfo.averageVector) });

            int numberOfEigenFaces = learningInfo.eigenFaces.Count;

            for(int i = 0; i < numberOfEigenFaces; ++i)
            {
                await eigenFaceController.PostEigenFace(new Models.EigenFace { Value = JsonConvert.SerializeObject(learningInfo.eigenFaces[i])});
            }

            return 0;
        }

    }
}
