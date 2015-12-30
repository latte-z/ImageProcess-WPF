using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageProcess.Emgu
{
    public partial class DisLeftPaint : Form
    {
        private Image<Bgr, byte> _imgFromWpf;

        bool mouseDown = false;
        private int cur_x = 0, cur_y = 0;
        private int pre_x = 0, pre_y = 0;

        #region init

        public DisLeftPaint()
        {
            InitializeComponent();
        }

        public DisLeftPaint(Image<Bgr, byte> imgFromWpf)
        {
            InitializeComponent();
            _imgFromWpf = imgFromWpf;
            paintPic.Image = _imgFromWpf.Bitmap;
            // set the width & height of form
            Width = _imgFromWpf.Bitmap.Width;
            Height = _imgFromWpf.Bitmap.Height;
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
                cur_x = e.X;
                cur_y = e.Y;
                CvInvoke.Line(_imgFromWpf, new Point(pre_x, pre_y), new Point(cur_x, cur_y), new MCvScalar(255, 255, 255), 2);
                pre_x = cur_x;
                pre_y = cur_y;
                paintPic.Image = _imgFromWpf.Bitmap;
            }
        }

        private void paintPic_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                mouseDown = false;
        }

        #endregion buttonEvent
    }
}
