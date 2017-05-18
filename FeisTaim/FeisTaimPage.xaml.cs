using FeisTaim.Controls;
using Xamarin.Forms;

namespace FeisTaim
{
    public partial class FeisTaimPage : ContentPage
    {
        public FeisTaimPage()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (await cameraView.RequestCameraPermissionTask)
            {
                cameraView.StartSession();
            }
            else
            {
                await DisplayAlert("Error", "Camera Permissions", "Ok");
            }
        }
    }
}
