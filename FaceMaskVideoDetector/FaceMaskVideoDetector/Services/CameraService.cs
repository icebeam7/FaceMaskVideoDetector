using System.IO;
using System.Threading.Tasks;

using Xamarin.Essentials;

using FaceMaskVideoDetector.Models;

namespace FaceMaskVideoDetector.Services
{
    public class CameraService
    {
        public static async Task<LocalVideo> RecordVideo()
        {
            var video = await MediaPicker.CaptureVideoAsync();
            return await SaveVideo(video);
        }

        public static async Task<LocalVideo> ChooseVideo()
        {
            var video = await MediaPicker.PickVideoAsync();
            return await SaveVideo(video);
        }

        private static async Task<LocalVideo> SaveVideo(FileResult file)
        {
            if (file == null)
                return new LocalVideo() { NewFile = string.Empty, VideoFile = null };

            var fileName = file.FileName;
            var folder = FileSystem.AppDataDirectory; // Path.GetTempPath()
            var newFile = Path.Combine(folder, fileName);

            using (var stream = await file.OpenReadAsync())
            {
                using (var newStream = File.OpenWrite(newFile))
                {
                    await stream.CopyToAsync(newStream);
                }
            }

            return new LocalVideo() { NewFile = newFile, VideoFile = file };
        }
    }
}
