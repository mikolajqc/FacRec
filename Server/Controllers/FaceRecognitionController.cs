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
        private readonly IRecognitonService _recognitionService;
        private readonly IAddNewFaceService _addNewFaceService;
        private readonly ILearningService _learningService;

        public FaceRecognitionController(IRecognitonService recognitionService, IAddNewFaceService addNewFaceService, ILearningService learningService)
        {
            _recognitionService = recognitionService;
            _addNewFaceService = addNewFaceService;
            _learningService = learningService;
        }

        [Route("api/FaceRecognition/Learn")]
        public HttpResponseMessage Learn()
        {
            _learningService.Learn();
            return Request.CreateResponse(HttpStatusCode.OK, "Learnt!");
        }

        [Route("api/FaceRecognition/Recognize")]
        public HttpResponseMessage Recognize(Request request)
        {
            byte[] bitmapWithFaceInArray = request.BitmapInArray;
            Bitmap bitmapWithFace = new Bitmap(Image.FromStream(new MemoryStream(bitmapWithFaceInArray)));

            string resultOfRecognition = _recognitionService.Recognize(bitmapWithFace);

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "FaceRecognition response");
            response.Content = new StringContent(JsonConvert.SerializeObject(resultOfRecognition), Encoding.Unicode);

            return response;
        }

        [Route("api/FaceRecognition/AddFace")]
        public HttpResponseMessage AddFace(Request request)
        {
            byte[] bitmapWithFaceInArray = request.BitmapInArray;
            Bitmap bitmapWithFace = new Bitmap(Image.FromStream(new MemoryStream(bitmapWithFaceInArray)));

            _addNewFaceService.AddNewFace(bitmapWithFace, request.Name);
            return Request.CreateResponse(HttpStatusCode.OK, "Face added!");
        }

    }
}
