using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using Microsoft.ProjectOxford.Face;

namespace ImageProcess.Controls
{

    /// <summary>
    /// Interaction logic for FaceDetection.xaml
    /// </summary>
    public partial class FaceDetection : UserControl, INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Description dependency property
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(FaceDetection));

        /// <summary>
        /// Output dependency property
        /// </summary>
        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(string), typeof(FaceDetection));

        /// <summary>
        /// Face detection results in list container
        /// </summary>
        private ObservableCollection<Face> _detectedFaces = new ObservableCollection<Face>();

        /// <summary>
        /// Face detection results in text string
        /// </summary>
        private string _detectedResultsInText;

        /// <summary>
        /// Face detection results container
        /// </summary>
        private ObservableCollection<Face> _resultCollection = new ObservableCollection<Face>();

        /// <summary>
        /// Image path used for rendering and detecting
        /// </summary>
        private string _selectedFile;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FaceDetection" /> class
        /// </summary>
        public FaceDetection()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Implement INotifyPropertyChanged event handler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets description
        /// </summary>
        public string Description
        {
            get
            {
                return (string)GetValue(DescriptionProperty);
            }

            set
            {
                SetValue(DescriptionProperty, value);
            }
        }

        /// <summary>
        /// Gets face detection results
        /// </summary>
        public ObservableCollection<Face> DetectedFaces => _detectedFaces;

        /// <summary>
        /// Gets or sets face detection results in text string
        /// </summary>
        public string DetectedResultsInText
        {
            get
            {
                return _detectedResultsInText;
            }

            set
            {
                _detectedResultsInText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DetectedResultsInText"));
            }
        }

        /// <summary>
        /// Gets constant maximum image size for rendering detection result
        /// </summary>
        public int MaxImageSize => 300;

        /// <summary>
        /// Gets or sets output for rendering
        /// </summary>
        public string Output
        {
            get
            {
                return (string)GetValue(OutputProperty);
            }

            set
            {
                SetValue(OutputProperty, value);
            }
        }

        /// <summary>
        /// Gets face detection results
        /// </summary>
        public ObservableCollection<Face> ResultCollection => _resultCollection;

        /// <summary>
        /// Gets or sets image path for rendering and detecting
        /// </summary>
        public string SelectedFile
        {
            get
            {
                return _selectedFile;
            }

            set
            {
                _selectedFile = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedFile"));
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Pick image for face detection and set detection result to result container
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event argument</param>
        private async void ImagePicker_Click(object sender, RoutedEventArgs e)
        {
            // Show file picker dialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".jpg",
                Filter = "Image files(*.jpg) | *.jpg"
            };
            var result = dlg.ShowDialog();

            if (!result.HasValue || !result.Value) return;
            // User picked one image
            var imageInfo = UIHelper.GetImageInfoForRendering(dlg.FileName);
            SelectedFile = dlg.FileName;

            // Clear last detection result
            ResultCollection.Clear();
            DetectedFaces.Clear();
            DetectedResultsInText = "正在检测中...";

            Output = Output.AppendLine($"发送请求: 检测图片 {SelectedFile} 中");
            var sw = Stopwatch.StartNew();

            // Call detection REST API
            using (var fileStream = File.OpenRead(SelectedFile))
            {
                try
                {
                    MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                    if (mainWindow == null) return;
                    string subscriptionKey = mainWindow.SubscriptionKey;

                    var faceServiceClient = new FaceServiceClient(subscriptionKey);
                    Microsoft.ProjectOxford.Face.Contract.Face[] faces = await faceServiceClient.DetectAsync(fileStream, false, true, true, false);
                    Output = Output.AppendLine($"反馈:检测成功. 共发现 {faces.Length} 张脸 在图片 {SelectedFile}");

                    DetectedResultsInText = $"一共检测到了 {faces.Length} 张面孔";

                    foreach (var face in faces)
                    {
                        DetectedFaces.Add(new Face()
                        {
                            ImagePath = SelectedFile,
                            Left = face.FaceRectangle.Left,
                            Top = face.FaceRectangle.Top,
                            Width = face.FaceRectangle.Width,
                            Height = face.FaceRectangle.Height,
                            FaceId = face.FaceId.ToString(),
                            Gender = $"{(face.Attributes.Gender.Equals("female") ? "女性" : "男性")}",
                            Age = $"大约 {face.Attributes.Age:#} 岁",
                        });
                    }

                    // Convert detection result into UI binding object for rendering
                    foreach (var face in UIHelper.CalculateFaceRectangleForRendering(faces, MaxImageSize, imageInfo))
                    {
                        ResultCollection.Add(face);
                    }
                }
                catch (ClientException ex)
                {
                    Output = Output.AppendLine($"反馈:出错啦: {ex.Error.Code}. {ex.Error.Message}");
                }
            }
        }

        #endregion Methods
    }
}
