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
    /// Interaction logic for SmoothImage.xaml
    /// </summary>
    public partial class SmoothImage : UserControl
    {
        #region Fields

        private Image<Bgr, byte> _leftOriginImage;  //原始图像
        private Image<Bgr, byte> _gaussianImage;      //处理之后的图像

        #endregion Fields

        public SmoothImage()
        {
            InitializeComponent();
        }

        #region Event

        #endregion Event

        #region Process

        private void Gaussian_Click(object sender, RoutedEventArgs e)
        {
            int kernelSize;
            try
            {
                kernelSize = int.Parse((KernelSizeX.Value).ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("请选择正确的卷积核大小！");
                return;
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            _gaussianImage = _leftOriginImage.SmoothGaussian(kernelSize);
            sw.Stop();
            RightImageDisplay.Source = BitmapSourceConvert.ToBitmapSource(_gaussianImage);
            Result.Text = string.Format(" 使用了高斯滤波，用时{0:F05}毫秒 \n 参数（卷积核大小为：{1}）\r\n",
                sw.Elapsed.TotalMilliseconds, kernelSize);
        }

        private void Blur_Click(object sender, RoutedEventArgs e)
        {
            int kernelSizeX;
            int kernelSizeY;
            try
            {
                kernelSizeX = int.Parse((KernelSizeX.Value).ToString());
                kernelSizeY = int.Parse((KernelSizeY.Value).ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("请选择正确的卷积核大小！");
                return;
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            _gaussianImage = _leftOriginImage.SmoothBlur(kernelSizeX, kernelSizeY);
            sw.Stop();
            RightImageDisplay.Source = BitmapSourceConvert.ToBitmapSource(_gaussianImage);
            Result.Text = string.Format(" 使用了均值滤波，用时{0:F05}毫秒 \n 参数（卷积核像素宽度为：{1}，卷积核像素高度为：{2}）\r\n",
                sw.Elapsed.TotalMilliseconds, kernelSizeX, kernelSizeY);
        }

        private void Median_Click(object sender, RoutedEventArgs e)
        {
            int kernelSizeX;
            try
            {
                kernelSizeX = int.Parse((KernelSizeX.Value).ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("请选择正确的卷积核大小！");
                return;
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            _gaussianImage = _leftOriginImage.SmoothMedian(kernelSizeX);
            sw.Stop();
            RightImageDisplay.Source = BitmapSourceConvert.ToBitmapSource(_gaussianImage);
            Result.Text = string.Format(" 使用了均值滤波，用时{0:F05}毫秒 \n 参数（卷积核大小为：{1}）\r\n",
                sw.Elapsed.TotalMilliseconds, kernelSizeX);
        }
        
        private void Bilatral_Click(object sender, RoutedEventArgs e)
        {
            int kernelSizeX;
            int colorSigma;
            int spaceSigma;
            try
            {
                kernelSizeX = int.Parse((KernelSizeX.Value).ToString());
                colorSigma = int.Parse(BiColor.Text);
                spaceSigma = int.Parse(BiSpace.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("请选择正确的卷积核大小！");
                return;
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            _gaussianImage = _leftOriginImage.SmoothBilatral(kernelSizeX, colorSigma, spaceSigma);
            sw.Stop();
            RightImageDisplay.Source = BitmapSourceConvert.ToBitmapSource(_gaussianImage);
            Result.Text = string.Format(" 使用了双边滤波，用时{0:F05}毫秒 \n 参数（卷积核大小为：{1}，颜色滤波器Sigma为{2} \n 空间滤波器Sigma为{3}）\r\n",
                sw.Elapsed.TotalMilliseconds, kernelSizeX, colorSigma, spaceSigma);
        }

        private void Dilate_Click(object sender, RoutedEventArgs e)
        {
            int kernelSizeX;
            try
            {
                kernelSizeX = int.Parse(DilateKernel.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("请输入正确的卷积核大小！");
                return;
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            _gaussianImage = _leftOriginImage.Dilate(kernelSizeX);
            sw.Stop();
            RightImageDisplay.Source = BitmapSourceConvert.ToBitmapSource(_gaussianImage);
            Result.Text = string.Format(" 使用了形态学滤波-膨胀，用时{0:F05}毫秒 \n 参数（卷积核大小为：{1}）\r\n",
                sw.Elapsed.TotalMilliseconds, kernelSizeX);
        }

        private void Erode_Click(object sender, RoutedEventArgs e)
        {
            int kernelSizeX;
            try
            {
                kernelSizeX = int.Parse(DilateKernel.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("请输入正确的卷积核大小！");
                return;
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            _gaussianImage = _leftOriginImage.Erode(kernelSizeX);
            sw.Stop();
            RightImageDisplay.Source = BitmapSourceConvert.ToBitmapSource(_gaussianImage);
            Result.Text = string.Format(" 使用了形态学滤波-腐蚀，用时{0:F05}毫秒 \n 参数（卷积核大小为：{1}）\r\n",
                sw.Elapsed.TotalMilliseconds, kernelSizeX);
        }

        #endregion Process

        #region Method

        protected void SavePic(string filePath)
        {
            BitmapSource bs = (BitmapSource)RightImageDisplay.Source;
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
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = @"C:\";
            if (openFile.ShowDialog() == true)
            {
                if (openFile.FileName != null)
                {
                    _leftOriginImage = new Image<Bgr, byte>(openFile.FileName);
                    LeftImageDisplay.Source = BitmapSourceConvert.ToBitmapSource(_leftOriginImage);
                }
            }
        }

        private void RightImageSaver_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.InitialDirectory = @"C:\Users\Public\Pictures";
            saveFile.FileName = "处理后的图片";
            saveFile.Filter = "位图(*.bmp)|*.bmp";
            saveFile.RestoreDirectory = true;

            if (saveFile.ShowDialog() == true)
            {
                string fileName = saveFile.FileName;
                SavePic(fileName);
            }
        }

        private void BiHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("第一个参数为颜色空间滤波器Sigma值\n参数值越大，表明该像素邻域内有越宽广的元素混合在一起\n" +
                            "产生较大的半相等颜色区域，一般可内核值*2\n\n" +
                            "第二个参数为空间中滤波器的Sigma值\n参数值越大，表明越远的像素会相互影响，使得更大区域中\n" +
                            "足够相似的颜色获取相同的颜色，一般可内核值/2");
        }

        private void DEHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("膨胀与腐蚀参数的作用均表示迭代使用相应方法的次数\n" +
                            "默认均为3x3的内核进行操作");
        }

        #endregion btn_click
    }
}
