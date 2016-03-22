using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Emgu.CV;
using Emgu.CV.WPF;
using Emgu.CV.Structure;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ImageProcess.Emgu
{
    /// <summary>
    /// Interaction logic for GetEdge.xaml
    /// </summary>
    public partial class GetEdge : UserControl
    {

        #region Fields

        private Image<Bgr, byte> _leftOriginImage;  //原始图像
        private Image<Gray, byte> _cannyImage;      //Canny处理之后的图像
        private Image<Bgr, float> _laplaceImage;    //Laplace处理后的图像
        private Image<Bgr, float> _sobelImage;      //Sobel处理后的图像

        #endregion Fields

        public GetEdge()
        {
            InitializeComponent();
        }

        #region Event

        private void CbLaplace_Loaded(object sender, RoutedEventArgs e)
        {
            // 用一个枚举类，集合形式绑定ComboBox itemsSource
            List<string> items = new List<string>();
            items.Add("Aperture");
            items.Add("1");
            items.Add("3");
            items.Add("5");
            items.Add("7");

            ComboBox box = sender as ComboBox;
            box.ItemsSource = items;
            box.SelectedIndex = 0;
        }

        private void CbCanny_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> items = new List<string>();
            items.Add("Aperture");
            items.Add("3");
            items.Add("5");
            items.Add("7");

            ComboBox box = sender as ComboBox;
            box.ItemsSource = items;
            box.SelectedIndex = 0;
        }

        private void CbSobel_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> items = new List<string>();
            items.Add("Aperture");
            items.Add("3");
            items.Add("5");
            items.Add("7");

            ComboBox box = sender as ComboBox;
            box.ItemsSource = items;
            box.SelectedIndex = 0;
        }

        #endregion Event

        #region Process

        private void Laplace_Click(object sender, RoutedEventArgs e)
        {
            int apertureSize;
            try
            {
                apertureSize = int.Parse((CbLaplace.SelectedItem).ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("请选择正确的孔径大小！");
                return;
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            _laplaceImage = _leftOriginImage.Laplace(3);
            sw.Stop();
            RightImageDisplay.Source = BitmapSourceConvert.ToBitmapSource(_laplaceImage);
            Result.Text = string.Format(" 使用了拉普拉斯变换，用时{0:F05}毫秒 \n 参数（方形滤波器宽度：{1}）\r\n", sw.Elapsed.TotalMilliseconds, apertureSize);
        }

        private void Canny_Click(object sender, RoutedEventArgs e)
        {
            double lowThresh;
            double hightThresh;
            int apertureSize;
            try
            {
                lowThresh = double.Parse(CannyLow.Text);
                hightThresh = double.Parse(CannyHigh.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("请检查上限与下限是否符合要求！");
                return;
            }
            try
            {
                apertureSize = int.Parse((CbCanny.SelectedItem).ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("请选择正确的孔径大小！");
                return;
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            _cannyImage = _leftOriginImage.Canny(lowThresh, hightThresh, apertureSize, true);
            sw.Stop();
            RightImageDisplay.Source = BitmapSourceConvert.ToBitmapSource(_cannyImage);
            Result.Text = string.Format(" 使用了Canny变换，用时{0:F05}毫秒 \n 参数（阈值下限：{1}，阈值上限：{2}，方形滤波器宽度：{3}）\r\n",
                sw.Elapsed.TotalMilliseconds, lowThresh, hightThresh, apertureSize);
        }

        private void Sobel_Click(object sender, RoutedEventArgs e)
        {
            int xOrder;
            int yOrder;
            int apertureSize;
            try
            {
                xOrder = int.Parse(SobelxOrder.Text);
                yOrder = int.Parse(SobelyOrder.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("请检查输入的X、Y方向上的求导阶数是否符合要求！");
                return;
            }
            try
            {
                apertureSize = int.Parse((CbSobel.SelectedItem).ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("请选择正确的孔径大小！");
                return;
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            _sobelImage = _leftOriginImage.Sobel(xOrder, yOrder, apertureSize);
            sw.Stop();
            RightImageDisplay.Source = BitmapSourceConvert.ToBitmapSource(_sobelImage);
            Result.Text = string.Format(" 使用了Sobel变换，用时{0:F05}毫秒 \n 参数（X方向上的求导阶数：{1}，Y方向上的求导阶数：{2}，方形滤波器宽度：{3}）\r\n",
                sw.Elapsed.TotalMilliseconds, xOrder, yOrder, apertureSize);
        }

        #endregion Process

        #region Method

        protected void SavePic(string filePath)
        {
            BitmapSource bs = (BitmapSource) RightImageDisplay.Source;
            BmpBitmapEncoder BBE = new BmpBitmapEncoder();
            BBE.Frames.Add(BitmapFrame.Create(bs));
            using (Stream stream = File.Create(filePath))
            {
                BBE.Save(stream);
            }
        }

        #endregion Method

        #region btn_click

        private void LeftImagePicker_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.InitialDirectory = "C:\\Users\\" + Environment.UserName + "\\Pictures";
                if (openFile.ShowDialog() == true)
                {
                    if (openFile.FileName != null)
                    {
                        _leftOriginImage = new Image<Bgr, byte>(openFile.FileName);
                        LeftImageDisplay.Source = BitmapSourceConvert.ToBitmapSource(_leftOriginImage);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("您所选择的文件不合法，请重新选择！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RightImageSaver_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.InitialDirectory = "C:\\Users\\" + Environment.UserName + "\\Pictures";
            saveFile.FileName = "processed image";
            saveFile.Filter = "位图(*.bmp)|*.bmp";
            saveFile.RestoreDirectory = true;

            if (saveFile.ShowDialog() == true)
            {
                string fileName = saveFile.FileName;
                SavePic(fileName);
            }
        }

        #endregion btn_click


    }
}
