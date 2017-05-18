using System;
using System.Threading.Tasks;
using FeisTaim.Models;
using Sockets.Plugin;
using Xamarin.Forms;

namespace FeisTaim.Controls
{
    public class CameraView : View
    {
        DateTime _lastSent;
        TimeSpan _timeSpan;
        readonly double _fps = 20;
        UdpSocketClient _client;
        int _port = 33333;
        string _address = "10.210.35.142";

        public event EventHandler<PreviewEventArgs> PreviewReady;

        public void OnPreviewReady(byte[] data)
        {
            PreviewReady?.Invoke(this, new PreviewEventArgs { Data = data });
        }

        public CameraView()
        {
            _lastSent = DateTime.Now;
            _client = new UdpSocketClient();
            PreviewReady += HandlePreviewReady;
        }

        public void StartSession()
        {
            StartSessionAction?.Invoke();
        }

        #region Native Calls

        public Task<bool> RequestCameraPermissionTask;
        public Action StartSessionAction;

        #endregion

        private async void HandlePreviewReady(Object source, PreviewEventArgs args)
        {
            if (DateTime.Now.Subtract(_lastSent) > TimeSpan.FromSeconds(1 / _fps))
            {
                await _client.SendToAsync(args.Data, _address, _port);
            }
        }
    }

    public enum CameraType
    {
        Front,
        Rear
    }
}
