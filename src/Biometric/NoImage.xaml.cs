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
using System.Windows.Shapes;

namespace Biometric
{
    /// <summary>
    /// Interaction logic for NoImage.xaml
    /// </summary>
    public partial class NoImage : Window
    {
        public NoImage()
        {
            InitializeComponent();
        }

        private void close_window(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.EnableControls();
            }
            this.Close();
        }

        private void NoImage_Closed(object sender, EventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.EnableControls();
            }
        }
    }
}
