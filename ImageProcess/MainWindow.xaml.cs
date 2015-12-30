using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using FirstFloor.ModernUI.Windows.Controls;

namespace ImageProcess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow, INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Project Name
        /// </summary>
        private string _appName;

        /// <summary>
        /// Newton Project Key
        /// </summary>
        private static string s_subscriptionKey;

        /// <summary>
        /// Title
        /// </summary>
        private string _title;

        // save the subscription key
        private readonly string IsolatedStorageSubscriptionKeyFileName = "Subscription.txt";
        private readonly string DefaultSubscriptionKeyPromptMessage = "请在此处填入您的Project Oxford Key";
        #endregion Fields

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
            SubscriptionKey = GetSubscriptionKeyFromIsolatedStorage();

            AppName = "简易图像处理与人脸识别/比对";
            AppTitle = "本程序使用了 EmguCV 和 MS Project Oxford 进行图片处理";
            DataContext = this;

            FaceDetectionDescription = "选择一张个人（或多人）图片，文件将被上传至微软Azure服务器进行人脸检测";
            FaceVerificationDescription = "选择两张个人照片，文件将被上传至微软Azure服务器进行人脸对比";

        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Implement INotifyPropertyChanged interface
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Propertities

        /// <summary>
        /// Gets the app title
        /// </summary>
        public string AppTitle
        {
            get { return _title; }
            set { _title = value;
                OnPropertyChanged<string>();
            }
        }

        /// <summary>
        /// Get app name
        /// </summary>
        public string AppName { get { return _appName; }
            set
            {
                _appName = value;
                OnPropertyChanged<string>();
            }
        }

        /// <summary>
        /// Get the subscription key
        /// </summary>
        public string SubscriptionKey { get { return s_subscriptionKey; }
            set
            {
                s_subscriptionKey = value;
                OnPropertyChanged<string>();
            }
        }

        /// <summary>
        /// get & set description of face detection
        /// </summary>
        public string FaceDetectionDescription { get; set; }

        /// <summary>
        /// get & set description of face verification
        /// </summary>
        public string FaceVerificationDescription { get; set; }

        /// <summary>
        /// get & set description of find similar face
        /// </summary>

        #endregion Propertities


        #region Methods

        /// <summary>
        /// get the subscription key from isolated storage
        /// </summary>
        private string GetSubscriptionKeyFromIsolatedStorage()
        {
            string subscriptionKey = null;

            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null))
            {
                try
                {
                    using (var iStream = new IsolatedStorageFileStream(IsolatedStorageSubscriptionKeyFileName, FileMode.Open, isoStore))
                    {
                        using (var reader = new StreamReader(iStream))
                        {
                            subscriptionKey = reader.ReadLine();
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    subscriptionKey = null;
                }
            }
            if (string.IsNullOrEmpty(subscriptionKey))
            {
                subscriptionKey = DefaultSubscriptionKeyPromptMessage;
            }
            return subscriptionKey;
        }

        /// <summary>
        /// save the subscription key to isolated storage
        /// </summary>
        private void SaveSubscriptionKeyToIsolatedStorage(string subscriptionKey)
        {
            using (
                IsolatedStorageFile isoStore =
                    IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null))
            {
                using (
                    var oStream = new IsolatedStorageFileStream(IsolatedStorageSubscriptionKeyFileName, FileMode.Create,
                        isoStore))
                {
                    using (var writer = new StreamWriter(oStream))
                    {
                        writer.WriteLine(subscriptionKey);
                    }
                }
            }
                
        }

        /// <summary>
        /// INotifyPropertyChanged interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="caller"></param>
        private void OnPropertyChanged<T>([CallerMemberName]string caller = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(caller));
            }
        }
        #endregion Methods

        private void GetKeyBtn_Click(object sender, RoutedEventArgs e)
        {
            // go to microsoft website
            System.Diagnostics.Process.Start("https://www.projectoxford.ai/doc/general/subscription-key-mgmt");
        }

        private void SaveKey_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveSubscriptionKeyToIsolatedStorage(SubscriptionKey);
                MessageBox.Show("您填入的Project Oxford Key已经保存在硬盘中.\n下次您将不需要再次手动输入.", "订阅秘钥");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("读取 Project Oxford Key 失败. 错误信息如下: " + ex.Message, "订阅秘钥", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteKey_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SubscriptionKey = DefaultSubscriptionKeyPromptMessage;
                SaveSubscriptionKeyToIsolatedStorage("");
                MessageBox.Show("您填入的 Project Oxford Key 已经从硬盘中删除.", "订阅秘钥");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("删除 Project Oxford Key 失败. 错误信息如下: " + ex.Message, "订阅秘钥", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HelpKey_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("使用标注有Oxford前缀的功能时,\n您需要在右侧文本框中添加微软牛津计划秘钥访问网络服务.");
        }
    }
}
