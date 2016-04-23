using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.ProjectOxford.Face;

namespace ImageProcess.Controls
{
    /// <summary>
    /// Interaction logic for FaceVerification.xaml
    /// </summary>
    public partial class FaceVerification : UserControl, INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Description dependency property
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(FaceVerification));

        /// <summary>
        /// Output dependency property
        /// </summary>
        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(string), typeof(FaceVerification));

        /// <summary>
        /// Face detection result container for image on the left
        /// </summary>
        private ObservableCollection<Face> _leftResultCollection = new ObservableCollection<Face>();

        /// <summary>
        /// Face detection result container for image on the right
        /// </summary>
        private ObservableCollection<Face> _rightResultCollection = new ObservableCollection<Face>();

        /// <summary>
        /// Face verification result
        /// </summary>
        private string _verifyResult;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FaceVerification" /> class
        /// </summary>
        public FaceVerification()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Implement INotifyPropertyChanged interface
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets description for UI rendering
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
        /// Gets face detection results for image on the left
        /// </summary>
        public ObservableCollection<Face> LeftResultCollection => _leftResultCollection;

        /// <summary>
        /// Gets max image size for UI rendering
        /// </summary>
        public int MaxImageSize => 300;

        /// <summary>
        /// Gets or sets output for UI rendering
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
        /// Gets face detection results for image on the right
        /// </summary>
        public ObservableCollection<Face> RightResultCollection => _rightResultCollection;

        /// <summary>
        /// Gets or sets selected face verification result
        /// </summary>
        public string VerifyResult
        {
            get
            {
                return _verifyResult;
            }

            set
            {
                _verifyResult = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("VerifyResult"));
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Pick image for detection, get detection result and put detection results into LeftResultCollection 
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event argument</param>
        private async void LeftImagePicker_Click(object sender, RoutedEventArgs e)
        {
            // Show image picker, show jpg type files only
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".jpg",
                Filter = "Image files(*.jpg) | *.jpg"
            };
            var result = dlg.ShowDialog();

            if (!result.HasValue || !result.Value) return;
            VerifyResult = string.Empty;

            // User already picked one image
            var pickedImagePath = dlg.FileName;
            var imageInfo = UIHelper.GetImageInfoForRendering(pickedImagePath);
            LeftImageDisplay.Source = new BitmapImage(new Uri(pickedImagePath));

            // Clear last time detection results
            LeftResultCollection.Clear();

            Output = Output.AppendLine($"发送请求: 检测图片 {pickedImagePath} 中");
            var sw = Stopwatch.StartNew();

            // Call detection REST API, detect faces inside the image
            using (var fileStream = File.OpenRead(pickedImagePath))
            {
                try
                {

                    MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                    if (mainWindow == null) return;
                    string subscriptionKey = mainWindow.SubscriptionKey;

                    var faceServiceClient = new FaceServiceClient(subscriptionKey);
                    var faces = await faceServiceClient.DetectAsync(fileStream);

                    // Handle REST API calling error
                    if (faces == null)
                    {
                        return;
                    }

                    Output = Output.AppendLine($"反馈:检测成功. 共发现 {faces.Length} 张脸 在图片 {pickedImagePath}");

                    // Convert detection results into UI binding object for rendering
                    foreach (var face in UIHelper.CalculateFaceRectangleForRendering(faces, MaxImageSize, imageInfo))
                    {
                        // Detected faces are hosted in result container, will be used in the verification later
                        LeftResultCollection.Add(face);
                    }
                }
                catch (ClientException ex)
                {
                    Output = Output.AppendLine($"反馈: 出错啦 {ex.Error.Code}. {ex.Error.Message}");
                }
            }
        }

        /// <summary>
        /// Pick image for detection, get detection result and put detection results into RightResultCollection 
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event argument</param>
        private async void RightImagePicker_Click(object sender, RoutedEventArgs e)
        {
            // Show image picker, show jpg type files only
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".jpg",
                Filter = "Image files(*.jpg) | *.jpg"
            };
            var result = dlg.ShowDialog();

            if (!result.HasValue || !result.Value) return;
            VerifyResult = string.Empty;

            // User already picked one image
            var pickedImagePath = dlg.FileName;
            var imageInfo = UIHelper.GetImageInfoForRendering(pickedImagePath);
            RightImageDisplay.Source = new BitmapImage(new Uri(pickedImagePath));

            // Clear last time detection results
            RightResultCollection.Clear();

            Output = Output.AppendLine($"发送请求: 检测图片 {pickedImagePath} 中");
            var sw = Stopwatch.StartNew();

            // Call detection REST API, detect faces inside the image
            using (var fileStream = File.OpenRead(pickedImagePath))
            {
                try
                {
                    MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                    if (mainWindow == null) return;
                    string subscriptionKey = mainWindow.SubscriptionKey;

                    var faceServiceClient = new FaceServiceClient(subscriptionKey);

                    var faces = await faceServiceClient.DetectAsync(fileStream);

                    // Handle REST API calling error
                    if (faces == null)
                    {
                        return;
                    }

                    Output = Output.AppendLine($"反馈:检测成功. 共发现 {faces.Length} 张脸 在图片 {pickedImagePath}");

                    // Convert detection results into UI binding object for rendering
                    foreach (var face in UIHelper.CalculateFaceRectangleForRendering(faces, MaxImageSize, imageInfo))
                    {
                        // Detected faces are hosted in result container, will be used in the verification later
                        RightResultCollection.Add(face);
                    }
                }
                catch (ClientException ex)
                {
                    Output = Output.AppendLine($"反馈: 出错啦 {ex.Error.Code}. {ex.Error.Message}");
                    return;
                }
            }
        }

        /// <summary>
        /// Verify two detected faces, get whether these two faces belong to the same person
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event argument</param>
        private async void Verification_Click(object sender, RoutedEventArgs e)
        {
            // Call face to face verification, verify REST API supports one face to one face verification only
            // Here, we handle single face image only
            if (LeftResultCollection.Count == 1 && RightResultCollection.Count == 1)
            {
                VerifyResult = "比对检测中...";
                var faceId1 = LeftResultCollection[0].FaceId;
                var faceId2 = RightResultCollection[0].FaceId;

                Output = Output.AppendLine($"发送请求: 比对检测 面部 {faceId1} 和 {faceId2}");

                // Call verify REST API with two face id
                try
                {
                    MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                    string subscriptionKey = mainWindow.SubscriptionKey;

                    var faceServiceClient = new FaceServiceClient(subscriptionKey);
                    var res = await faceServiceClient.VerifyAsync(Guid.Parse(faceId1), Guid.Parse(faceId2));

                    // Verification result contains IsIdentical (true or false) and Confidence (in range 0.0 ~ 1.0),
                    // here we update verify result on UI by VerifyResult binding
                    VerifyResult = $"{(res.IsIdentical ? "相似" : "不相似")} ({res.Confidence:0.0})";
                    Output = Output.AppendLine(
                        $"反馈: 比对检测完毕. 面部 {faceId1} 和 {faceId2} {(res.IsIdentical ? "属于" : "不属于")} 同一个人");
                }
                catch (ClientException ex)
                {
                    Output = Output.AppendLine($"反馈: 出错啦 {ex.Error.Code}. {ex.Error.Message}");
                }
            }
            else
            {
                MessageBox.Show("比对检测仅支持两张个人照片，请检查您的选择.", "出错了", MessageBoxButton.OK);
            }
        }

        #endregion Methods
    }
}
