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
        //Temperature1 temperature1;
        Pressure pressure;
        GrafConductivity conductivity;
        Flaw flaw;
        Level level;
        CO2 cO2;
        GrafTemperature grafPage;
        public MainWindow()
        {
            InitializeComponent();
            //temperature1 = new Temperature1();
            pressure = new Pressure();
            conductivity = new GrafConductivity("Электропроводимость", "uSm", 1.2);
            flaw = new Flaw();
            level = new Level();
            cO2 = new CO2();
            grafPage = new GrafTemperature("Температура","°C", 120);
            Main.Content = grafPage;
        }

        private void MainBtcClck(object sender, RoutedEventArgs e)
        {
            //Main.Content = temperature1;
            Main.Content = grafPage;
        }

        
        private void btnChooseFile_clk(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                //temperature1.FilePath = filePath;
                pressure.FilePath = filePath;
                conductivity.FilePath = filePath;
                flaw.FilePath = filePath;
                level.FilePath = filePath;
                cO2.FilePath = filePath;
                grafPage.FilePath = filePath;
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

        private void btnFlawClck(object sender, RoutedEventArgs e)
        {
            Main.Content = flaw;
        }

        private void btnLewelClck(object sender, RoutedEventArgs e)
        {
            Main.Content = level;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = cO2;
        }
    }
}
