using System;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Xamarin.Forms;

using FaceMaskVideoDetector.Models;
using FaceMaskVideoDetector.Services;

namespace FaceMaskVideoDetector.ViewModels
{
    public class AnalyzeVideosViewModel : BaseViewModel
    {
        private ObservableCollection<KeyFrameClean> videoInsights;

        public ObservableCollection<KeyFrameClean> VideoInsights
        {
            get => videoInsights;
            set { SetProperty(ref videoInsights, value); }
        }

        private VideoResultClean video;

        private async Task GetDetails(string id)
        {
            IsBusy = true;

            var insights = await VideoIndexerService.Details(id);

            VideoInsights.Clear();

            foreach (var item in insights.KeyFrameList)
                VideoInsights.Add(item);

            IsBusy = false;
        }

        public AnalyzeVideosViewModel(VideoResultClean video)
        {
            this.video = video;

            VideoInsights = new ObservableCollection<KeyFrameClean>();

            Task.Run(async () => await GetDetails(video.Id));
        }
    }
}
