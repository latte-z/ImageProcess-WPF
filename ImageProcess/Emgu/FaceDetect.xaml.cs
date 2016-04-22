using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Runtime.InteropServices;
using System.Drawing;

namespace ImageProcess.Emgu
{
    /// <summary>
    /// Interaction logic for FaceDetect.xaml
    /// </summary>
    public partial class FaceDetect : UserControl
    {
        private Capture capture;
        private CascadeClassifier haar;
        private string picturePath = @"./Assets/default.jpg";

        public FaceDetect()
        {
            CvInvoke.UseOpenCL = false;
            InitializeComponent();
            capture = new Capture();
            haar = new CascadeClassifier(@"haarcascade_frontalface_default.xml");
        }

        // public string Path { get { return picturePath; } set { picturePath = value; } }

        private void Capture_ImageGrabbed(object sender, EventArgs e)
        {
            Mat currentFrame = new Mat();
            capture.Retrieve(currentFrame, 0);

            if (currentFrame != null)
            {
                Rectangle[] faces = haar.DetectMultiScale(currentFrame, 1.1, 3, new System.Drawing.Size(40, 40));
                foreach (Rectangle face in faces)
                {
                    CvInvoke.Rectangle(currentFrame, face, new Bgr(Color.Red).MCvScalar, 2);
                }
                Dispatcher.BeginInvoke((Action)delegate ()
                {
                    image1.Source = ToBitmapSource(currentFrame);
                });
                // no gc!
                // currentFrame.Dispose();
            }
        }

        private void captureStart_Click(object sender, RoutedEventArgs e)
        {
            capture.Start();
            capture.ImageGrabbed += Capture_ImageGrabbed;
        }

        private void captureStop_Click(object sender, RoutedEventArgs e)
        {
            capture.ImageGrabbed -= Capture_ImageGrabbed;
            capture.Stop();
            // reset the default background img
            image1.Source = ToBitmapSource(new Image<Bgr, byte>(picturePath));
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); // obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); // release the HBitmap
                return bs;
            }
        }

        // 外部启动
        //private void NewProcess_Click(object sender, RoutedEventArgs e)
        //{
        //    // To start a new process
        //    string str = System.Environment.CurrentDirectory;
        //    ProcessStartInfo psi = new ProcessStartInfo();
        //    psi.FileName = str + @"/Extra/DynamicFaceDetect.exe";
        //    psi.UseShellExecute = false;
        //    psi.WorkingDirectory = str + @"/Extra";
        //    psi.CreateNoWindow = true;
        //    Process.Start(psi);
        //}
    }
}
