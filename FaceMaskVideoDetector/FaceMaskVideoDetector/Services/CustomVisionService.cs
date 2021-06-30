using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;

using Newtonsoft.Json;

using FaceMaskVideoDetector.Models;
using FaceMaskVideoDetector.Helpers;

namespace FaceMaskVideoDetector.Services
{
    public class CustomVisionService
    {
        private static readonly HttpClient clientCV =
                    HttpClientHelper.CreateHttpClient(Constants.CustomVisionBaseUrl,
                        "Prediction-Key",
                        Constants.CustomVisionKey); 
        
        public async static Task<List<Prediction>> DetectObjects(string base64Image)
        {
            try
            {
                var bytes = Convert.FromBase64String(base64Image);
                var content = new StreamContent(new MemoryStream(bytes));

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var response = await clientCV.PostAsync(Constants.CustomVisionService, content);

                if (response.IsSuccessStatusCode)
                {
                    var predictionResult = await response.Content.ReadAsStringAsync();
                    var customVisionResult = JsonConvert.DeserializeObject<CustomVisionResult>(predictionResult);
                    return customVisionResult.Predictions.Where(x => x.Probability > 0.5).ToList();
                }
            }
            catch (Exception ex)
            {
            }

            return new List<Prediction>();
        }
    }
}
