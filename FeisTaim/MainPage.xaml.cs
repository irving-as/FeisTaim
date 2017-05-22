using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeisTaim.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FeisTaim
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            string ipAddress = DependencyService.Get<INetworkHelper>().GetLocalIpAddress();
            CurrentIpLabel.Text = "Aqui toy: " + ipAddress;
            if (string.IsNullOrWhiteSpace(IpAddressEntry.Text))
                IpAddressEntry.Text = ipAddress;

        }

        private void StartButton_OnClicked(object sender, EventArgs e)
        {
            var ipAddress = IpAddressEntry.Text;
            var portText = PortEntry.Text;
            int port;

            if (string.IsNullOrWhiteSpace(ipAddress) || string.IsNullOrWhiteSpace(portText) ||
                !Int32.TryParse(portText, out port))
            {
                DisplayAlert("Peligro!", "Tus campos estan mal :(", "Ok");
            }
            else
            {
                Navigation.PushAsync(new FeisTaimPage(ipAddress, port));

            }
        }
    }
}