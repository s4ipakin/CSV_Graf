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

namespace CSV_Reader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Temperature1 temperature1;
        Pressure pressure;
        Conductivity conductivity;
        public MainWindow()
        {
            InitializeComponent();
            temperature1 = new Temperature1();
            pressure = new Pressure();
            conductivity = new Conductivity();
            Main.Content = temperature1;
        }

        private void MainBtcClck(object sender, RoutedEventArgs e)
        {
            Main.Content = temperature1;
        }

        
        private void btnChooseFile_clk(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                temperature1.FilePath = filePath;
                pressure.FilePath = filePath;
                conductivity.FilePath = filePath;
            }
        }

        private void PressureBtnClk(object sender, RoutedEventArgs e)
        {
            Main.Content = pressure;
        }

        private void btnCondClck(object sender, RoutedEventArgs e)
        {
            Main.Content = conductivity;
        }
    }
}
