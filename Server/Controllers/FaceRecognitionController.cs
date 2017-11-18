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
        FaceRecognition.IFaceRecognition fR = new FaceRecognition.FaceRecognition(@"C:\Users\Mikolaj\Desktop\Studia\Inzynierka\LearningSet_AT&T");

        // GET: api/FaceRecognition
        public string Get()
        {
            fR.Learn();
            return "Learnt!";
        }

        // GET: api/FaceRecognition/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/FaceRecognition
        public async Task<HttpResponseMessage> Post(Request request)
        {
            string req = request.Name;
            byte[] bitmap = request.BitmapInArray;

            Bitmap bitmapWithFace = new Bitmap(Image.FromStream(new MemoryStream(bitmap)));

            string resultOfRecognition = fR.Recognize(bitmapWithFace);

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "FaceRecognition response");
            response.Content = new StringContent(resultOfRecognition, Encoding.Unicode);
            return response;
        }

        // PUT: api/FaceRecognition/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/FaceRecognition/5
        public void Delete(int id)
        {
        }
    }
}
