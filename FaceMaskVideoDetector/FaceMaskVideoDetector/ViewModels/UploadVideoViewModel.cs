using System.Windows.Input;
using System.Threading.Tasks;

using Xamarin.Forms;

using FaceMaskVideoDetector.Models;
using FaceMaskVideoDetector.Services;

namespace FaceMaskVideoDetector.ViewModels
{
    public class UploadVideoViewModel : BaseViewModel
    {
        private LocalVideo localVideo;

        public LocalVideo LocalVideo
        {
            get => localVideo;
            set { SetProperty(ref localVideo, value); OnPropertyChanged("VideoFullPath"); }
        }

        public string VideoFullPath
        {
            get 
            {
                if (LocalVideo != null)
                    if (LocalVideo.VideoFile != null)
                        return $"ms-appdata:///local/{LocalVideo.VideoFile.FileName}"; //temp/

                return null;
            }
        }

        public ICommand RecordCommand { private set; get; }
        public ICommand LoadCommand { private set; get; }
        public ICommand UploadCommand { private set; get; }

        private async Task Record()
        {
            LocalVideo = await CameraService.RecordVideo();
        }

        private async Task Load()
        {
            LocalVideo = await CameraService.ChooseVideo();
        }

        private async Task Upload()
        {
            IsBusy = true;

            if (LocalVideo.VideoFile != null)
            {
                var success = await VideoIndexerService.UploadVideo(LocalVideo.VideoFile);

                await App.Current.MainPage.DisplayAlert(
                    "Result...", 
                    success ? "The video has been uploaded and it's currently being indexed" : "There was an error. Try again later.",
                    "OK");
            }

            IsBusy = false;
        }

        public UploadVideoViewModel()
        {
            RecordCommand = new Command(async () => await Record());
            LoadCommand = new Command(async () => await Load());
            UploadCommand = new Command(async () => await Upload());
        }
    }
}
