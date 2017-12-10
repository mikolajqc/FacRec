using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FaceRecognition;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Server.Repositories;
using Commons;
using FaceRecognition.Interfaces;

namespace Server.Controllers
{
    public class FaceRecognitionController : ApiController
    {
        //DI:
        private readonly IFaceRecognition fR;
        private readonly IRecognitonService recognitionService;
        private readonly IAddNewFaceService addNewFaceService;

        public FaceRecognitionController(IFaceRecognition fR, IRecognitonService recognitionService, IAddNewFaceService addNewFaceService)
        {
            this.fR = fR;
            this.recognitionService = recognitionService;
            this.addNewFaceService = addNewFaceService;
        }

        [Route("api/FaceRecognition/Learn")]
        public async Task<HttpResponseMessage> Learn()
        {
            //fR.Learn();
            //var test = averageVectorDAO.GetOverview();
            /*
            IFaceRecognition fR = new FaceRecognition.FaceRecognition();

            LearningInfo learningInfo = fR.Learn(); // tak byc nie moze niech dane schodza do bazy z serwisu!!!
            await InsertLearningInfoToDatabase(learningInfo);
            */
            return Request.CreateResponse(HttpStatusCode.OK, "Learnt!");
        }

        [Route("api/FaceRecognition/Recognize")]
        public HttpResponseMessage Recognize(Request request)
        {
            byte[] bitmapWithFaceInArray = request.BitmapInArray;
            Bitmap bitmapWithFace = new Bitmap(Image.FromStream(new MemoryStream(bitmapWithFaceInArray)));

            string resultOfRecognition = recognitionService.Recognize(bitmapWithFace);

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "FaceRecognition response");
            response.Content = new StringContent(JsonConvert.SerializeObject(resultOfRecognition), Encoding.Unicode);

            return response;
        }

        [Route("api/FaceRecognition/AddFace")]
        public HttpResponseMessage AddFace(Request request)
        {
            byte[] bitmapWithFaceInArray = request.BitmapInArray;
            Bitmap bitmapWithFace = new Bitmap(Image.FromStream(new MemoryStream(bitmapWithFaceInArray)));

            addNewFaceService.AddNewFace(bitmapWithFace, request.Name);
            return Request.CreateResponse(HttpStatusCode.OK, "Face added!");
        }


        public async Task<int> InsertLearningInfoToDatabase(LearningInfo learningInfo)
        {
            AverageVectorsController averageController = new AverageVectorsController();
            EigenFacesController eigenFaceController = new EigenFacesController();

            await averageController.PostAverageVector(new Models.AverageVector { Value = JsonConvert.SerializeObject(learningInfo.averageVector) });

            int numberOfEigenFaces = learningInfo.eigenFaces.Count;

            for (int i = 0; i < numberOfEigenFaces; ++i)
            {
                await eigenFaceController.PostEigenFace(new Models.EigenFace { Value = JsonConvert.SerializeObject(learningInfo.eigenFaces[i]) });
            }

            return 0;
        }

    }
}
