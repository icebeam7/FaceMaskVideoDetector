using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using FaceMaskVideoDetector.ViewModels;

namespace FaceMaskVideoDetector.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnalyzeVideosView : ContentPage
    {
        ListVideosViewModel vm;

        public AnalyzeVideosView()
        {
            InitializeComponent();

            vm = new ListVideosViewModel(this.Navigation);
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await Task.Run(() => vm.GetVideosCommand.Execute(null));
        }
    }
}