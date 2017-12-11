using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Drawing;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Commons;
using FaceRecognition.Interfaces;

namespace Server.Controllers
{
    public class FaceRecognitionController : ApiController
    {
        //DI:
        private readonly IRecognitonService recognitionService;
        private readonly IAddNewFaceService addNewFaceService;
        private readonly ILearningService learningService;

        public FaceRecognitionController(IRecognitonService recognitionService, IAddNewFaceService addNewFaceService, ILearningService learningService)
        {
            this.recognitionService = recognitionService;
            this.addNewFaceService = addNewFaceService;
            this.learningService = learningService;
        }

        [Route("api/FaceRecognition/Learn")]
        public HttpResponseMessage Learn()
        {
            learningService.Learn();
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

    }
}
