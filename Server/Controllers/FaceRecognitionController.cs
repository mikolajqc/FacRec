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

namespace Server.Controllers
{
    public class FaceRecognitionController : ApiController
    {
        FaceRecognition.IFaceRecognition fR = new FaceRecognition.FaceRecognition(@"D:\Studia\Inzynierka\LearningSet_AT&T\");

        // GET: api/FaceRecognition
        public string Get()
        {
            fR.Learn();
            return "Learnt!";
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
