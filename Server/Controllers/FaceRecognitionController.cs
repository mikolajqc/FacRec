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

namespace Server.Controllers
{
    public class FaceRecognitionController : ApiController
    {
        FaceRecognition.IFaceRecognition fR = new FaceRecognition.FaceRecognition(@"D:\Studia\Inzynierka\LearningSet_AT&T\");

        [Route("api/FaceRecognition/Learn")]
        public async Task<string> Learn()
        {
            ///TODO: zrobic zeby ID sie samo generowalo, ogarnac jak wrzucac wages jako JSON, ale czy obchodza nas wagi osob testowych?
           // List<FacesMatrix> infoFromLearning = fR.Learn();

            //KnownPeoplesController knownPeopleController = new KnownPeoplesController();

            //await knownPeopleController.PostKnownPeople(new Models.KnownPeople { Id = 1, Person = "elo", Wages = "1,2,3,4" });

            return "posted";
        }

        [Route("api/FaceRecognition/Recognize")]
        public HttpResponseMessage Recognize(Request request)
        {
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

    }
}
