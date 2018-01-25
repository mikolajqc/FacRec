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
    public static class RequestManager
    {
        public static async Task<string> Recognize(Bitmap bitmap, bool isLdaSet)
        {
            var stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            var bitmapData = stream.ToArray();

            var client = new HttpClient()
            {
                BaseAddress = new Uri(CommonConsts.Client.ServerAddress)
            };

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
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public static async Task<string> AddFace(Bitmap bitmap, string name)
        {
            var stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            var bitmapData = stream.ToArray();

            var client = new HttpClient()
            {
                BaseAddress = new Uri(CommonConsts.Client.ServerAddress)
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/bson"));

            var request = new ClientRequestData
            {
                Name = name,
                BitmapInArray = bitmapData
            };

            MediaTypeFormatter bsonFormatter = new BsonMediaTypeFormatter();
            var response = await client.PostAsync(CommonConsts.Client.AddFaceActionPath, request, bsonFormatter);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
    }
}
