using System;
using System.IO;
using System.Threading.Tasks;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Hardware;
using Android.Views;
using Android.Widget;
using FeisTaim.Controls;
using FeisTaim.Droid.Renderers;
using Java.IO;
using Sockets.Plugin;
using Sockets.Plugin.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Android.Hardware.Camera;

[assembly: ExportRenderer(typeof(ReceiverView), typeof(ReceiverViewRenderer))]
namespace FeisTaim.Droid.Renderers
{
    public class ReceiverViewRenderer : ViewRenderer<ReceiverView, Android.Views.View>
    {
        ImageView _streamImage;
        UdpSocketReceiver _receiver;
        int _port = 41915;

        public ReceiverViewRenderer()
        {
            _receiver = new UdpSocketReceiver();
            _receiver.MessageReceived += HandleMessageReceived;
            iniReceiver();
        }

        private async void iniReceiver()
        {
            await _receiver.StartListeningAsync(_port);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ReceiverView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var layoutparams = new Android.Widget.LinearLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                _streamImage = new ImageView(Context);
                _streamImage.LayoutParameters = layoutparams;
                var mainView = new Android.Widget.LinearLayout(this.Context);
                mainView.AddView(_streamImage);

                SetNativeControl(mainView);
            }

            if (Control == null || Element == null)
                return;

            if (e.OldElement != null)
            {
                //Element being deleted
            }

            if (e.NewElement != null)
            {
                //Element being created
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }

        private async void HandleMessageReceived(object sender, UdpSocketMessageReceivedEventArgs args)
        {
            var data = args.ByteData;

            Matrix matrix = new Matrix();
            matrix.PostRotate(270);

            Bitmap image = BitmapFactory.DecodeByteArray(data, 0, data.Length);
            image = Bitmap.CreateBitmap(image, 0, 0, image.Width, image.Height, matrix, true);

            ((MainActivity)Context).RunOnUiThread(() =>
            {
                _streamImage.SetImageBitmap(image);
            });
        }
    }
}
