using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Commons.Consts;
using Commons.Utilities;

namespace Client.Utilities
{
    public class RequestManager
    {
        public async Task<string> Recognize(Bitmap bitmap, bool isLdaSet)
        {
            byte[] bitmapData;
            var stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            bitmapData = stream.ToArray();

            var client = new HttpClient()
            {
                BaseAddress = new Uri(CommonConsts.Client.ServerAddress)
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
            var response = await client.PostAsync(CommonConsts.Client.RecognitionActionPath, request, bsonFormatter);

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
                BaseAddress = new Uri(CommonConsts.Client.ServerAddress)
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
            response = await client.PostAsync(CommonConsts.Client.AddFaceActionPath, request, bsonFormatter);

            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            return result;
        }

    }
}
