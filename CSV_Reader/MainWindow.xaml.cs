using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using CSV_Reader.GrafSettings;

namespace CSV_Reader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Temperature1 temperature1;
        //Pressure pressure;
        //GrafConductivity conductivity;
        GrafPage flaw;
        GrafPage level;
        GrafPage cO2;
        //GrafTemperature grafPage;
        GrafPage temperature;
        GrafPage pressure;
        GrafPage conductivity;
        public MainWindow()
        {
            InitializeComponent();
            Dictionary<GrafType, GrafSet> grafSets = new Dictionary<GrafType, GrafSet>();
            //temperature1 = new Temperature1();
            InitializeSets(grafSets);
            temperature = new GrafPage(grafSets[GrafType.Temp]);
            pressure = new GrafPage(grafSets[GrafType.Pressure]);
            conductivity = new GrafPage(grafSets[GrafType.Conductivity]);
            //conductivity = new GrafConductivity("Электропроводимость", "uSm", 1.2);
            flaw = new GrafPage(grafSets[GrafType.Flaw]);
            level = new GrafPage(grafSets[GrafType.Level]);
            cO2 = new GrafPage(grafSets[GrafType.CO2]);
            //grafPage = new GrafTemperature("Температура","°C", 120);
            Main.Content = temperature;
        }

        private void InitializeSets(Dictionary<GrafType, GrafSet> grafSets)
        {
            grafSets.Clear();
            var allGrafTypes = Assembly.GetAssembly(typeof(GrafSet)).GetTypes()
                .Where(t => typeof(GrafSet).IsAssignableFrom(t) && t.IsAbstract == false);

            foreach (var type in allGrafTypes)
            {
                GrafSet grafSet = Activator.CreateInstance(type) as GrafSet;
                grafSets.Add(grafSet.GrafType, grafSet);
            }           
        }

        private void MainBtcClck(object sender, RoutedEventArgs e)
        {           
            Main.Content = temperature;
        }

        
        private void btnChooseFile_clk(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                GrafPage.FilePath = filePath;               
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
