using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Commons.Consts;
using Newtonsoft.Json;
using Commons.Inferfaces.Services;
using Commons.Utilities;

namespace Server.Controllers
{
    public class FacRecController : ApiController
    {
        //DI:
        private readonly IEnumerable<IRecognitonService> _recognitionServices;
        private readonly IAddNewFaceService _addNewFaceService;
        private readonly ILearningService _learningService;

        public FacRecController(
            IEnumerable<IRecognitonService> recognitionServices,
            IAddNewFaceService addNewFaceService,
            ILearningService learningService)
        {
            _recognitionServices = recognitionServices;
            _addNewFaceService = addNewFaceService;
            _learningService = learningService;
        }

        [Route("api/FacRec/Learn")]
        public HttpResponseMessage Learn()
        {
            var directPathToLearningSet =
                System.Web.Hosting.HostingEnvironment.MapPath(CommonConsts.Server.PathToLearningSet);
            _learningService.Learn(directPathToLearningSet);
            return Request.CreateResponse(HttpStatusCode.OK, "Learnt!");
        }

        [Route("api/FacRec/Recognize")]
        public HttpResponseMessage Recognize(ClientRequestData clientRequestData)
        {
            var bitmapWithFaceInArray = clientRequestData.BitmapInArray;
            var bitmapWithFace = new Bitmap(Image.FromStream(new MemoryStream(bitmapWithFaceInArray)));
            var resultOfRecognition = clientRequestData.IsLdaSet
                ? _recognitionServices.ElementAt(1).Recognize(bitmapWithFace)
                : _recognitionServices.ElementAt(0).Recognize(bitmapWithFace);

            var response = Request.CreateResponse(HttpStatusCode.OK, "FaceRecognition response");
            response.Content = new StringContent(JsonConvert.SerializeObject(resultOfRecognition), Encoding.Unicode);

            return response;
        }

        [Route("api/FacRec/AddFace")]
        public HttpResponseMessage AddFace(ClientRequestData clientRequestData)
        {
            var bitmapWithFaceInArray = clientRequestData.BitmapInArray;
            var bitmapWithFace = new Bitmap(Image.FromStream(new MemoryStream(bitmapWithFaceInArray)));

            var directPathToLearningSet =
                System.Web.Hosting.HostingEnvironment.MapPath(CommonConsts.Server.PathToLearningSet);

            _addNewFaceService.AddNewFace(bitmapWithFace, clientRequestData.Name, directPathToLearningSet);
            return Request.CreateResponse(HttpStatusCode.OK, "Face added!");
        }
    }
}
