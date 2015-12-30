using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ImageProcess.Emgu
{
    /// <summary>
    /// Interaction logic for FaceDetect.xaml
    /// </summary>
    public partial class FaceDetect : UserControl
    {
        public FaceDetect()
        {
            InitializeComponent();
        }

        private void NewProcess_Click(object sender, RoutedEventArgs e)
        {
            string str = System.Environment.CurrentDirectory;
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = str + @"/Extra/DynamicFaceDetect.exe";
            psi.UseShellExecute = false;
            psi.WorkingDirectory = str + @"/Extra";
            psi.CreateNoWindow = true;
            Process.Start(psi);
        }
    }
}
