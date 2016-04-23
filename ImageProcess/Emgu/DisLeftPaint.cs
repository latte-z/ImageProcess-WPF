using System;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageProcess.Emgu
{
    public partial class DisLeftPaint : Form
    {
        private Bitmap _imgFromWpf;
        private readonly Image<Bgr, byte> _originImage;

        bool mouseDown = false;
        private int cur_x = 0, cur_y = 0;
        private int pre_x = 0, pre_y = 0;

        #region init

        public DisLeftPaint()
        {
            InitializeComponent();
        }

        public DisLeftPaint(Bitmap temp, Image<Bgr, byte> originImage)
        {
            InitializeComponent();
            _imgFromWpf = temp;
            _originImage = originImage;

            paintPic.BackgroundImage = _originImage.Bitmap;
            paintPic.Image = temp;
            // set the width & height of form
            Width = _imgFromWpf.Width;
            Height = _imgFromWpf.Height;
        }

        #endregion init

        #region buttonEvent

        private void paintPic_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDown = true;
                pre_x = e.X;
                pre_y = e.Y;
            }
        }

        private void paintPic_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown == true)
            {
                //cur_x = e.X;
                //cur_y = e.Y;
                //CvInvoke.Line(_imgFromWpf, new Point(pre_x, pre_y), new Point(cur_x, cur_y), new MCvScalar(255, 255, 255), 2);
                //pre_x = cur_x;
                //pre_y = cur_y;
                //paintPic.Image = (_imgFromWpf + _originImage).Bitmap;
                Pen whitePen = new Pen(Color.White, 3);
                using (Graphics g = Graphics.FromImage(paintPic.Image))
                {
                    cur_x = e.X;
                    cur_y = e.Y;
                    g.DrawLine(whitePen, pre_x, pre_y, cur_x, cur_y);
                    pre_x = cur_x;
                    pre_y = cur_y;
                    //g.Dispose();
                }
                paintPic.Refresh();
            }
        }

        private void paintPic_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                mouseDown = false;
            //_imgFromWpf = new Image<Bgr, byte>(temp);
        }

        #endregion buttonEvent
    }
}
