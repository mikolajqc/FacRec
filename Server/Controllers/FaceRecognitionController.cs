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
using Server.Repositories;
using Commons.Inferfaces.DAOs;

namespace Server.Controllers
{
    public class FaceRecognitionController : ApiController
    {
        private readonly IFaceRecognition fR;

        public FaceRecognitionController(IFaceRecognition fR)
        {
            this.fR = fR;
        }

        [Route("api/FaceRecognition/Learn")]
        public async Task<string> Learn()
        {
            //fR.Learn();
            //var test = averageVectorDAO.GetOverview();
            /*
            IFaceRecognition fR = new FaceRecognition.FaceRecognition();

            LearningInfo learningInfo = fR.Learn(); // tak byc nie moze niech dane schodza do bazy z serwisu!!!
            await InsertLearningInfoToDatabase(learningInfo);
            */
            return "Learnt!";
        }

        [Route("api/FaceRecognition/Recognize")]
        public HttpResponseMessage Recognize(Request request)
        {
            LearningInfo learningInfo = GetLearningInfoFromDatabase();

            //IFaceRecognition fR = new FaceRecognition.FaceRecognition(averageVectorDAO);
            fR.LoadLearningInfo(learningInfo);

            byte[] bitmapWithFaceInArray = request.BitmapInArray;

            Bitmap bitmapWithFace = new Bitmap(Image.FromStream(new MemoryStream(bitmapWithFaceInArray)));

            string resultOfRecognition = fR.Recognize(bitmapWithFace);

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "FaceRecognition response");
            response.Content = new StringContent(JsonConvert.SerializeObject(resultOfRecognition), Encoding.Unicode);

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

            for (int i = 0; i < numberOfEigenFaces; ++i)
            {
                await eigenFaceController.PostEigenFace(new Models.EigenFace { Value = JsonConvert.SerializeObject(learningInfo.eigenFaces[i]) });
            }

            return 0;
        }

        public LearningInfo GetLearningInfoFromDatabase()
        {
            GenericUnitOfWork guow = new GenericUnitOfWork();

            AverageVectorsController averageVectorController = new AverageVectorsController();
            EigenFacesController eigenFacesController = new EigenFacesController();

            List<Models.AverageVector> listOfAverageVectorModels = guow.Repository<Models.AverageVector>().GetOverview().ToList();//averageVectorController.GetAverageVectors().ToList();

            double[] averageVector = (JsonConvert.DeserializeObject(listOfAverageVectorModels.Last().Value, typeof(double[])) as double[]);

            List<Models.EigenFace> eigenFaceModels = eigenFacesController.GetEigenFaces().ToList();

            List<double[]> eigenFaces = new List<double[]>();

            for(int i = 0; i < eigenFaceModels.Count; ++i)
            {
                eigenFaces.Add(JsonConvert.DeserializeObject(eigenFaceModels[i].Value, typeof(double[])) as double[]);
            }


            return new LearningInfo()
            {
                averageVector = averageVector,
                eigenFaces = eigenFaces
            };
        }

    }
}
