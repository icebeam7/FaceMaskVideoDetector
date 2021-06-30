using System;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Xamarin.Forms;

using FaceMaskVideoDetector.Models;
using FaceMaskVideoDetector.Services;

namespace FaceMaskVideoDetector.ViewModels
{
    public class ListVideosViewModel : BaseViewModel
    {
        private ObservableCollection<VideoResultClean> videos;

        public ObservableCollection<VideoResultClean> Videos
        {
            get => videos;
            set { SetProperty(ref videos, value); }
        }

        private VideoResultClean selectedVideo;

        public VideoResultClean SelectedVideo
        {
            get => selectedVideo;
            set { SetProperty(ref selectedVideo, value); }
        }

        public ICommand RefreshCommand { get; private set; }
        public ICommand GetVideosCommand { get; private set; }
        public ICommand NavigateToDetailCommand { get; private set; }
        private INavigation navigationService;

        private async Task GetVideos()
        {
            IsBusy = true;
        }

        private async Task Refresh()
        {
            var videoList = await VideoIndexerService.GetVideoList();

            Videos.Clear();

            foreach (var item in videoList)
                Videos.Add(item);

            IsBusy = false;
        }

        private async Task NavigateToDetail(Type page)
        {
            if (SelectedVideo != null)
            {
                var vm = new AnalyzeVideosViewModel(SelectedVideo);

                Page detailsPage = (Page)Activator.CreateInstance(page);
                detailsPage.BindingContext = vm;
                await navigationService.PushAsync(detailsPage);
            }
        }

        public ListVideosViewModel(INavigation navigation)
        {
            Videos = new ObservableCollection<VideoResultClean>();

            GetVideosCommand = new Command(async () => await GetVideos());
            RefreshCommand = new Command(async () => await Refresh());
            NavigateToDetailCommand = new Command<Type>(async (p) => await NavigateToDetail(p));

            navigationService = navigation;
        }
    }
}
