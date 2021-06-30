using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json;
using Xamarin.Essentials;

using FaceMaskVideoDetector.Models;
using FaceMaskVideoDetector.Helpers;

namespace FaceMaskVideoDetector.Services
{
    public class VideoIndexerService
    {
        private static readonly HttpClient client =
            HttpClientHelper.CreateHttpClient($"{Constants.VideoIndexerBaseUrl}/{Constants.AccountId}/",
                "Ocp-Apim-Subscription-Key",
                Constants.SubscriptionKey);

        private static readonly HttpClient clientAuth =
            HttpClientHelper.CreateHttpClient($"{Constants.AuthBaseUrl}/{Constants.AccountId}/",
                "Ocp-Apim-Subscription-Key",
                Constants.SubscriptionKey);

        private static double ConvertTime(string time)
        {
            var culture = CultureInfo.CurrentCulture;
            var format = "H:mm:ss.f"; //0:00:02.7

            if (time.Length < 9)
                time += ".0";

            return DateTime.ParseExact(time.Substring(0, 9), format, culture).TimeOfDay.TotalSeconds;
        }

        private static async Task<string> GetToken()
        {
            var token = string.Empty;
            var tokenResponse = await clientAuth.GetAsync(Constants.TokenService);

            if (tokenResponse.IsSuccessStatusCode)
            {
                var content = await tokenResponse.Content.ReadAsStringAsync();
                token = content.Substring(1, content.Length - 2);
            }

            return token;
        }

        public static byte[] GetBytes(Stream input)
        {
            var buffer = new byte[16 * 1024];

            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }

        public static async Task<bool> UploadVideo(FileResult file)
        {
            if (string.IsNullOrWhiteSpace(Constants.VideoIndexerAccessToken))
                Constants.VideoIndexerAccessToken = await GetToken();

            var uploadUrl = $"{Constants.UploadVideo}={Constants.VideoIndexerAccessToken}&{Constants.NameParameter}={file.FileName}";

            using (var stream = await file.OpenReadAsync())
            {
                var byteData = GetBytes(stream);

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new ByteArrayContent(byteData));
                    var response = await client.PostAsync(uploadUrl, content);
                    return response.IsSuccessStatusCode;
                }
            }
        }

        public static async Task<List<VideoResultClean>> GetVideoList()
        {
            if (string.IsNullOrWhiteSpace(Constants.VideoIndexerAccessToken))
                Constants.VideoIndexerAccessToken = await GetToken();

            var listUrl = $"{Constants.ListVideos}={Constants.VideoIndexerAccessToken}";
            var response = await client.GetAsync(listUrl);

            var videoList = new List<VideoResultClean>();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var videoResult = JsonConvert.DeserializeObject<VideoResult>(json);

                if (videoResult.results.Count > 0)
                {
                    foreach (var item in videoResult.results)
                    {
                        var thumbnail = await GetThumbnail(item.id, item.thumbnailId);

                        videoList.Add(new VideoResultClean()
                        {
                            Id = item.id,
                            Name = item.name,
                            ThumbnailId = item.thumbnailId,
                            Thumbnail = $"{thumbnail}"
                        });
                    }
                }
            }

            return videoList;
        }

        private static async Task<string> GetThumbnail(string videoId, string thumbnailId)
        {
            if (string.IsNullOrWhiteSpace(Constants.VideoIndexerAccessToken))
                Constants.VideoIndexerAccessToken = await GetToken();

            var thumbnailUrl = $"Videos/{videoId}/Thumbnails/{thumbnailId}?format=Base64&accessToken={Constants.VideoIndexerAccessToken}";
            var thumbnailResponse = await client.GetAsync(thumbnailUrl);

            return thumbnailResponse.IsSuccessStatusCode
                ? await thumbnailResponse.Content.ReadAsStringAsync()
                : string.Empty;
        }

        public static async Task<VideoResultInsights> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(Constants.VideoIndexerAccessToken))
                Constants.VideoIndexerAccessToken = await GetToken();

            var insights = new VideoResultInsights();
            insights.Id = id;
            insights.KeyFrameList = new List<KeyFrameClean>();

            var labels = new List<Label>();

            var downloadUriResponse = await client.GetAsync($"Videos/{id}/SourceFile/DownloadUrl?accessToken={Constants.VideoIndexerAccessToken}");

            if (downloadUriResponse.IsSuccessStatusCode)
            {
                var url = await downloadUriResponse.Content.ReadAsStringAsync();
                insights.VideoUri = url.Substring(1, url.Length - 2);
            }

            var indexResponse = await client.GetAsync($"Videos/{id}/Index?reTranslate=False&includeStreamingUrls=True?accessToken={Constants.VideoIndexerAccessToken}");

            if (indexResponse.IsSuccessStatusCode)
            {
                var indexContent = await indexResponse.Content.ReadAsStringAsync();
                var videoIndex = JsonConvert.DeserializeObject<VideoIndex>(indexContent);

                if (videoIndex != null)
                {
                    var video = videoIndex.videos.FirstOrDefault();

                    if (video != null)
                    {
                        foreach (var shot in video.insights.shots)
                        {
                            foreach (var keyFrame in shot.keyFrames)
                            {
                                foreach (var instance in keyFrame.instances)
                                {
                                    var thumbnail = await GetThumbnail(id, instance.thumbnailId);

                                    insights.KeyFrameList.Add(new KeyFrameClean()
                                    {
                                        Start = instance.start,
                                        End = instance.end,
                                        ThumbnailId = instance.thumbnailId,
                                        Thumbnail = $"{thumbnail}"
                                    });
                                }
                            }
                        }

                        labels = video.insights.labels.ToList();
                    }
                }
            }

            var labelsClean = new List<LabelClean>();

            foreach (var item in labels)
            {
                var ac = new List<AppearanceClean>();

                if (item.instances != null)
                {
                    foreach (var app in item.instances)
                    {
                        ac.Add(new AppearanceClean()
                        {
                            StartTime = ConvertTime(app.start),
                            EndTime = ConvertTime(app.end)
                        });
                    }
                }

                labelsClean.Add(new LabelClean()
                {
                    name = item.name,
                    id = item.id,
                    appearances = ac
                });
            }

            foreach (var item in insights.KeyFrameList)
            {
                var startTime = ConvertTime(item.Start);
                var endTime = ConvertTime(item.End);

                foreach (var label in labelsClean)
                {
                    if (label.appearances.Any(x => startTime >= x.StartTime && endTime <= x.EndTime))
                        item.Labels += $"{label.name},  ";
                }

                var base64Image = item.Thumbnail;
                var predictions = await CustomVisionService.DetectObjects(base64Image);

                foreach (var p in predictions)
                    item.CustomLabel += $"{ p.TagName }, ";

                if (string.IsNullOrWhiteSpace(item.CustomLabel))
                    item.CustomLabel = $"No mask";
            }

            return insights;
        }

    }
}
