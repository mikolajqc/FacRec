using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Commons;
using Commons.Utilities;

namespace Client.Utilities
{
    public class RequestManager
    {
        //todo: to config
        private const string RequestAddress = "http://localhost/";
        private const string RecognitionPath = "/api/FacRec/Recognize";
        private const string AddFacePath = "/api/FacRec/AddFace";

        public async Task<string> Recognize(Bitmap bitmap, bool isLdaSet)
        {
            byte[] bitmapData;
            var stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            bitmapData = stream.ToArray();

            var client = new HttpClient()
            {
                BaseAddress = new Uri(RequestAddress)
            };
            // Set the Accept header for BSON.
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/bson"));

            var request = new ClientRequestData
            {
                Name = "",
                BitmapInArray = bitmapData,
                IsLdaSet = isLdaSet
            };

            MediaTypeFormatter bsonFormatter = new BsonMediaTypeFormatter();
            var response = await client.PostAsync(RecognitionPath, request, bsonFormatter);

            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public async Task<string> AddFace(Bitmap bitmap, string name)
        {
            byte[] bitmapData;
            var stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            bitmapData = stream.ToArray();

            var client = new HttpClient()
            {
                BaseAddress = new Uri(RequestAddress)
            };
            // Set the Accept header for BSON.
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/bson"));

            var request = new ClientRequestData
            {
                Name = name,
                BitmapInArray = bitmapData
            };

            MediaTypeFormatter bsonFormatter = new BsonMediaTypeFormatter();
            HttpResponseMessage response;
            response = await client.PostAsync(AddFacePath, request, bsonFormatter);

            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            return result;
        }

    }
}
