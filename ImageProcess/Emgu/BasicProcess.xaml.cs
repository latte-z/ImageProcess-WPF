using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Emgu.CV;
using Emgu.CV.WPF;
using Emgu.CV.Structure;
using Emgu.CV.Stitching;
using System.Windows.Media.Imaging;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using UserControl = System.Windows.Controls.UserControl;

namespace ImageProcess.Emgu
{
    /// <summary>
    /// Interaction logic for BasicProcess.xaml
    /// </summary>
    public partial class BasicProcess : UserControl
    {
        #region Fields

        private Image<Bgr, byte> _leftOriginImage;  // 原始图像
        private Image<Bgra, byte> _hdrOriginImage;  // HDR待处理图像
        private Image<Bgr, byte> temp;              // 图像修复的临时图片
        private Image<Gray, byte> mask;             // 修复图片用Mask

        bool mouseDown = false;
        double pre_x = 0, pre_y = 0;
        double x = 0, y = 0;

        #endregion Fields

        public BasicProcess()
        {
            InitializeComponent();
        }


        #region Event

        #endregion Event

        #region Process

        private void AutoHDR_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            string colorPattern;

            if (this.RadioGray.IsChecked == true)
            {
                colorPattern = "灰度模式";
                _hdrOriginImage = _leftOriginImage.Convert<Bgra, byte>(); // bgr转bgra（bgr+alpha通道)
                Image<Gray, byte> hdrGrayOrigin = _hdrOriginImage.Convert<Gray, byte>();
                LeftImageDisplay.Source = BitmapSourceConvert.ToBitmapSource(hdrGrayOrigin);
                sw.Start();
                hdrGrayOrigin._EqualizeHist();
                sw.Stop();
                RightImageDisplay.Source = BitmapSourceConvert.ToBitmapSource(hdrGrayOrigin);
            }
            else
            {
                colorPattern = "彩色模式";
                _hdrOriginImage = _leftOriginImage.Convert<Bgra, byte>();
                LeftImageDisplay.Source = BitmapSourceConvert.ToBitmapSource(_hdrOriginImage);
                sw.Start();
                _hdrOriginImage._EqualizeHist();
                sw.Stop();
                RightImageDisplay.Source = BitmapSourceConvert.ToBitmapSource(_hdrOriginImage);
            }

            Result.Text = $"使用了{colorPattern}的自动HDR，用时{sw.Elapsed.TotalMilliseconds:F05}毫秒\r\n";

        }

        //private void Stitcher_Click(object sender, RoutedEventArgs e)
        //{
        //    Image<Bgr, byte> a = new Image<Bgr, byte>("1.jpg");
        //    Image<Bgr, byte> b = new Image<Bgr, byte>("2.jpg");

        //    Image<Bgr, byte>[] sources = new Image<Bgr, byte>[2];
        //    sources[0] = a;
        //    sources[1] = b;
        //    Stitcher stitcher = new Stitcher(true);
        //    Image<Bgr, byte> resultPic = a;
        //    stitcher.Stitch(sources, resultPic);
        //    RightImageDisplay.Source = BitmapSourceConvert.ToBitmapSource(resultPic);
        //}

        private void MousePaint_Click(object sender, RoutedEventArgs e)
        {
            temp = new Image<Bgr, byte>(_leftOriginImage.Width, _leftOriginImage.Height, new Bgr(0, 0, 0));
            DisLeftPaint form = new DisLeftPaint(temp);
            form.ShowDialog();
            LeftImageDisplay.Source = BitmapSourceConvert.ToBitmapSource(temp + _leftOriginImage);
        }

        private void Inpaint_Click(object sender, RoutedEventArgs e)
        {
            if (RadioExam.IsChecked == true)
            {
                Stopwatch sw = new Stopwatch();
                mask = new Image<Gray, byte>("Mask\\mask2.jpg");
                double radius = double.Parse(InPaintArgs.Text);
                sw.Start();
                RightImageDisplay.Source = BitmapSourceConvert.ToBitmapSource(_leftOriginImage.InPaint(mask, radius));
                sw.Stop();
                

                Result.Text = $"演示图像修复完成，用时共{sw.Elapsed.TotalMilliseconds:F05}毫秒\r\n";
            }
            else
            {
                Stopwatch sw = new Stopwatch();
                try
                {
                    mask = temp.Convert<Gray, byte>();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Mask蒙板错误");
                }
                double radius = double.Parse(InPaintArgs.Text);
                sw.Start();
                var tempPic = (temp + _leftOriginImage).InPaint(mask, radius);
                sw.Stop();
                RightImageDisplay.Source = BitmapSourceConvert.ToBitmapSource(tempPic);

                Result.Text = $"图像修复完成，用时共{sw.Elapsed.TotalMilliseconds:F05}毫秒\r\n";
            }
        }

        private void Resize_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            string Interpolation = CbResize.SelectedItem.ToString();
            Image<Bgr, byte> temp;
            switch (Interpolation)
            {
                case "Area":
                    sw.Start();
                    temp = _leftOriginImage.Resize(int.Parse(ResizeWidth.Text), int.Parse(ResizeHeight.Text), Inter.Area);
                    sw.Stop();
                    break;
                case "Cubic":
                    sw.Start();
                    temp = _leftOriginImage.Resize(int.Parse(ResizeWidth.Text), int.Parse(ResizeHeight.Text), Inter.Cubic);
                    sw.Stop();
                    break;
                case "Lanczos4":
                    sw.Start();
                    temp = _leftOriginImage.Resize(int.Parse(ResizeWidth.Text), int.Parse(ResizeHeight.Text), Inter.Lanczos4);
                    sw.Stop();
                    break;
                case "Linear":
                    sw.Start();
                    temp = _leftOriginImage.Resize(int.Parse(ResizeWidth.Text), int.Parse(ResizeHeight.Text), Inter.Linear);
                    sw.Stop();
                    break;
                case "Nearest":
                    sw.Start();
                    temp = _leftOriginImage.Resize(int.Parse(ResizeWidth.Text), int.Parse(ResizeHeight.Text), Inter.Nearest);
                    sw.Stop();
                    break;
                default:
                    return;
            }
            RightImageDisplay.Source = BitmapSourceConvert.ToBitmapSource(temp);
            Result.Text = $"图像缩放完成，用时共{sw.Elapsed.TotalMilliseconds:F05}毫秒\r\n使用了{Interpolation}的插值算法";
        }

        private void CbResize_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> items = new List<string>();
            items.Add("Area");
            items.Add("Cubic");
            items.Add("Lanczos4");
            items.Add("Linear");
            items.Add("Nearest");

            ComboBox box = sender as ComboBox;
            box.ItemsSource = items;
            box.SelectedIndex = 0;
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

        private void HDRHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("彩色图片处理后图像质量会下降\n灰度图片不受影响\nOpenCV中是用于处理8bit的单通道图像");
        }

        private void InpaintHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("参数为修复算法的参考半径，需要修补的每个点的圆形邻域\n 勾上演示，会演示一般情况下的结果");
        }

        #endregion btn_click


    }
}
