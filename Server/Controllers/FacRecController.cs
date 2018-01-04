using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Drawing;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Commons;
using Commons.Inferfaces.Services;

namespace Server.Controllers
{
    public class FacRecController : ApiController
    {
        //DI:
        private readonly IRecognitonService _recognitionService;
        private readonly IAddNewFaceService _addNewFaceService;
        private readonly ILearningService _learningService;

        public FacRecController(IRecognitonService recognitionService, IAddNewFaceService addNewFaceService, ILearningService learningService)
        {
            _recognitionService = recognitionService;
            _addNewFaceService = addNewFaceService;
            _learningService = learningService;
        }

        [Route("api/FacRec/Learn")]
        public HttpResponseMessage Learn()
        {
            _learningService.Learn();
            return Request.CreateResponse(HttpStatusCode.OK, "Learnt!");
        }

        [Route("api/FacRec/Recognize")]
        public HttpResponseMessage Recognize(ClientRequestData clientRequestData)
        {
            byte[] bitmapWithFaceInArray = clientRequestData.BitmapInArray;
            Bitmap bitmapWithFace = new Bitmap(Image.FromStream(new MemoryStream(bitmapWithFaceInArray)));
            string resultOfRecognition = _recognitionService.Recognize(bitmapWithFace);

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "FaceRecognition response");
            response.Content = new StringContent(JsonConvert.SerializeObject(resultOfRecognition), Encoding.Unicode);

            return response;
        }

        [Route("api/FacRec/AddFace")]
        public HttpResponseMessage AddFace(ClientRequestData clientRequestData)
        {
            byte[] bitmapWithFaceInArray = clientRequestData.BitmapInArray;
            Bitmap bitmapWithFace = new Bitmap(Image.FromStream(new MemoryStream(bitmapWithFaceInArray)));

            _addNewFaceService.AddNewFace(bitmapWithFace, clientRequestData.Name);
            return Request.CreateResponse(HttpStatusCode.OK, "Face added!");
        }

    }
}
