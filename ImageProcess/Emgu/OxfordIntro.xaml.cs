using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageProcess.Emgu
{
    /// <summary>
    /// Interaction logic for OxfordIntro.xaml
    /// </summary>
    public partial class OxfordIntro : UserControl
    {
        public OxfordIntro()
        {
            InitializeComponent();
        }

        private void GetInformation_Click(object sender, RoutedEventArgs e)
        {
            // go to oxford website
            System.Diagnostics.Process.Start("https://www.microsoft.com/cognitive-services");
        }
    }
}
