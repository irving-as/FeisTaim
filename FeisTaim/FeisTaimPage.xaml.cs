using FeisTaim.Controls;
using Xamarin.Forms;

namespace FeisTaim
{
    public partial class FeisTaimPage : ContentPage
    {

        private string _host;
        private int _port;

        public FeisTaimPage(string host, int port)
        {
            InitializeComponent();
            _host = host;
            _port = port;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (await cameraView.RequestCameraPermissionTask)
            {
                cameraView.IpAddress = _host;
                cameraView.Port = _port;
                cameraView.StartSession();
            }
            else
            {
                await DisplayAlert("Error", "Camera Permissions", "Ok");
            }
        }
    }
}
