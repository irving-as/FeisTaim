using System;
using System.IO;
using System.Net.Sockets;
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
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Android.Hardware.Camera;

[assembly: ExportRenderer(typeof(CameraView), typeof(CameraViewRenderer))]
namespace FeisTaim.Droid.Renderers
{
    public class CameraViewRenderer : ViewRenderer<CameraView, Android.Views.View>, TextureView.ISurfaceTextureListener, Android.Hardware.Camera.IPreviewCallback
    {
        Android.Hardware.Camera camera;
        TextureView liveCameraStream;

        public CameraViewRenderer()
        {
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Control == null || Element == null)
                return;
        }

        #region Renderer Life Cycle

        protected override void OnElementChanged(ElementChangedEventArgs<CameraView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var layoutparams = new Android.Widget.RelativeLayout.LayoutParams(150, 150);
                layoutparams.AddRule(LayoutRules.AlignParentBottom);
                layoutparams.AddRule(LayoutRules.CenterHorizontal);
                layoutparams.BottomMargin = 50;
                liveCameraStream = new TextureView(this.Context);
                liveCameraStream.SurfaceTextureListener = this;

                var mainView = new Android.Widget.RelativeLayout(this.Context);
                mainView.AddView(liveCameraStream);

                SetNativeControl(mainView);
            }

            if (Control == null || Element == null)
                return;

            if (e.OldElement != null)
            {
                Element.RequestCameraPermissionTask = null;
                Element.StartSessionAction = null;
            }

            if (e.NewElement != null)
            {
                Element.RequestCameraPermissionTask = AuthorizeCameraUse();
                Element.StartSessionAction = SetupLiveCameraRear;
            }
        }

        #endregion

        async Task<bool> AuthorizeCameraUse()
        {
            return true;
        }

        public void SetupLiveCameraRear()
        {
            IsInitRequested = true;
            SetupLiveCameraStream();
        }

        public void SetupLiveCameraStream()
        {
            lock (lockObj)
            {
                if (surface == null)
                    return;
                try
                {
                    camera = Android.Hardware.Camera.Open(1);
                    camera.SetDisplayOrientation(90);
                    var cameraParams = camera.GetParameters();
                    foreach (Android.Hardware.Camera.Size size in cameraParams.SupportedPreviewSizes)
                    {
                        if (size.Width <= 176)
                        {
                            cameraParams.SetPreviewSize(size.Width, size.Height);
                        }
                    }
                    camera.SetParameters(cameraParams);
                    camera.SetPreviewTexture(surface);
                    camera.SetPreviewCallback(this);
                    camera.StartPreview();
                }
                catch (Java.IO.IOException ex)
                {

                }
            }
        }

        private bool IsInitRequested { get; set; }

        static object lockObj = new object();

        #region TextureView.ISurfaceTextureListener

        public SurfaceTexture surface;

        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
        {
            this.surface = surface;
            liveCameraStream.LayoutParameters = new Android.Widget.RelativeLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
            if (IsInitRequested)
            {
                IsInitRequested = false;
                SetupLiveCameraStream();
            }
        }

        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
        {
            camera.StopPreview();
            camera.Release();

            return true;
        }

        public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
        {
            //throw new NotImplementedException();
        }

        public void OnSurfaceTextureUpdated(SurfaceTexture surface)
        {
            //throw new NotImplementedException();
        }

        #endregion

        #region Android.Hardware.Camera.IPictureCallback

        public void OnPreviewFrame(byte[] data, Android.Hardware.Camera camera)
        {
            var outputStream = new MemoryStream();
            YuvImage yuvImage = new YuvImage(data, ImageFormat.Nv21, 176, 144, null);

            yuvImage.CompressToJpeg(new Rect(0, 0, 176, 144), 50, outputStream);

            byte[] imageBytes = outputStream.ToArray();

            Element.OnPreviewReady(imageBytes);


            //        Parameters parameters = camera.GetParameters();
            //        var imageFormat = parameters.PreviewFormat;
            //        if (imageFormat == ImageFormatType.Nv21)
            //        {
            //            Rect rect = new Rect(0, 0, liveCameraStream.Width, liveCameraStream.Height);
            //            YuvImage img = new YuvImage(data, ImageFormat.Nv21, liveCameraStream.Width, liveCameraStream.Height, null);

            //            Java.IO.ByteArrayOutputStream baos = new Java.IO.ByteArrayOutputStream();
            //            Java.IO.ObjectOutputStream oos = new Java.IO.ObjectOutputStream(baos);
            //oos.writeObject(C1);
            //oos.flush();
            //}
        }

        #endregion
    }
}
