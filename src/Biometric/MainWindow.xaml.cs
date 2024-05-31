using Microsoft.Win32;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Biometric
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
        private BitmapImage sidikJari = new BitmapImage();
    private string algorithm = "BM";

    public MainWindow()
    {
        InitializeComponent();
    }

    private void ImageButton_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

        if (openFileDialog.ShowDialog() == true)
        {
            try
            {
                sidikJari = new BitmapImage(new Uri(openFileDialog.FileName));
                imageControl.Source = sidikJari;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading image: " + ex.Message);
            }
        }
    }

    private void start_Click(object sender, RoutedEventArgs e)
    {
        if (radioButton2.IsChecked == true) {algorithm = "KMP";}
        MessageBox.Show("Algorithm: " + algorithm);
    }
}