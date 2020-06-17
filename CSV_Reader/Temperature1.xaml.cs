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
using System.Data;
using GenericParsing;
using System.Data.Common;
using System.IO;
using LiveCharts;
using System.ComponentModel;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System.Printing;
//using System.Windows.Xps.Packaging;

using System.Windows.Xps.Packaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using AventStack.ExtentReports.Model;
using System.Windows.Forms;

namespace CSV_Reader
{
    /// <summary>
    /// Interaction logic for Temperature1.xaml
    /// </summary>
    /// 
    public partial class Temperature1 : Page , INotifyPropertyChanged
    {

        protected DataTable csvDataTable;
        protected DataSet dataset = new DataSet();
        public event PropertyChangedEventHandler PropertyChanged;
        public SeriesCollection SeriesCollection { get; set; }
        protected string[] _labels;
        protected byte[] hours = new byte[24];
        protected byte[] minutes = new byte[60];

        protected string filePath;
        public string FilePath 
        { set 
            { 
                filePath = value;
            } 
        }

        
        protected GrafSeries[] series;

        public string[] Labels
        {
            get { return _labels; }
            set
            {
                _labels = value;
                OnPropertyChanged("Labels");
            }
        }
        public Func<double, string> YFormatter { get; set; }


        public Temperature1()
        {
            InitializeComponent();
            csvDataTable = new DataTable();            
            dataset.Tables.Add(csvDataTable);
            SeriesCollection = new SeriesCollection();
            Labels = new[] { System.DateTime.Now.ToString() };
            YFormatter = value => value.ToString() + "°C";            
            series = new GrafSeries[] 
            { 
                new GrafSeries("Температура ВВО(основная петля)", 7),
                new GrafSeries("Температура ВВО(дополнительная петля)", 9),
                new GrafSeries("Температура ВДИ(для хранения)", 23),
                new GrafSeries("Температура ВВИ(циркулирующая)", 27),
            };
            
            for (int i = 0; i < series.Length; i++)
            {
                SeriesCollection.Add(series[i].LineSeries);
            }
            SetArrays();
            comboBoxHourStart.ItemsSource = hours;
            comboBoxHourEnd.ItemsSource = hours;
            comboBoxMinuteStart.ItemsSource = minutes;
            comboBoxMinuteEnd.ItemsSource = minutes;
            checkGraf1.IsChecked = true;
            checkGraf2.IsChecked = true;
            checkGraf3.IsChecked = true;
            checkGraf4.IsChecked = true;
            Chart_P.Zoom = ZoomingOptions.X;            
            DataContext = this;
            PickerDataStop.SelectedDateChanged += PickerDataStop_SelectedDateChanged;
            pickedData.SelectedDateChanged += PickedData_SelectedDateChanged;
        }

        protected void PickedData_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            PickerDataStop.DisplayDateStart = pickedData.SelectedDate;
        }

        protected void PickerDataStop_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            pickedData.DisplayDateEnd = PickerDataStop.SelectedDate;
        }

        protected void btnFromDays_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                csvDataTable = CSV_DataTable.ConvertCSVtoDataTable(filePath);
            }
            catch(Exception ex) {  }
            
            SeriesCollectionOperate seriesCollectionOperate = new SeriesCollectionOperate();
            try
            {
                for (int i = 0; i < series.Length; i++)
                {
                    Labels = seriesCollectionOperate.SetValues(SeriesCollection[i].Values, csvDataTable, 1, 2, series[i].Column,
                    (System.DateTime)pickedData.SelectedDate, (System.DateTime)PickerDataStop.SelectedDate, comboBoxHourStart.SelectedIndex,
                    comboBoxHourEnd.SelectedIndex, comboBoxMinuteStart.SelectedIndex, comboBoxMinuteEnd.SelectedIndex);
                }                
            }
            catch(Exception ex) { System.Windows.MessageBox.Show("Укажите время и дату начала и конца графика"); }            
        }

        

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null) // if subrscribed to event
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void btnOpnFileClk(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == true)
            {
                filePath = openFileDialog.FileName;                
            }
        }

        protected void btnPrintClick(object sender, RoutedEventArgs e)
        {
            
            int width = (int)Chart_P.ActualWidth;
            int height = (int)Chart_P.ActualHeight;
            System.Windows.Point position = Chart_P.PointToScreen(new System.Windows.Point(0d, 0d));
            
            System.Drawing.Rectangle bounds = new System.Drawing.Rectangle(/*(int)position.X*/0, /*(int)position.Y*/0, width, height);
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(/*System.Drawing.Point.Empty*/new System.Drawing.Point((int)position.X, (int)position.Y), System.Drawing.Point.Empty, bounds.Size);
                }
                System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
                saveFileDialog1.Filter = "JPeg Image|*.jpg";
                saveFileDialog1.Title = "Сохранить график как изображение JPeg";
                saveFileDialog1.ShowDialog();
                string imagePath = saveFileDialog1.FileName;
                bitmap.Save(imagePath, ImageFormat.Jpeg);
            }
        }

        

        //public static void TakeCroppedScreenShot(string fileName, int x, int y, int width, int height, ImageFormat format)
        //{
        //    System.Drawing.Rectangle r = new System.Drawing.Rectangle(x, y, width, height);
        //    Bitmap bmp = new Bitmap(r.Width, r.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        //    Graphics g = Graphics.FromImage(bmp);
        //    g.CopyFromScreen(r.Left, r.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
        //    bmp.Save(fileName, format);
        //}


        private void Graf1Chckd(object sender, RoutedEventArgs e)
        {
            series[0].LineSeries.Visibility = Visibility.Visible;
        }

        private void Graf1Unchckd(object sender, RoutedEventArgs e)
        {
            series[0].LineSeries.Visibility = Visibility.Hidden;            
        }


        private void Graf2Chckd(object sender, RoutedEventArgs e)
        {
            series[1].LineSeries.Visibility = Visibility.Visible;
        }

        private void Graf2Unchckd(object sender, RoutedEventArgs e)
        {
            series[1].LineSeries.Visibility = Visibility.Hidden;
        }

        private void Graf3Chckd(object sender, RoutedEventArgs e)
        {
            series[2].LineSeries.Visibility = Visibility.Visible;
        }

        private void Graf3Unchckd(object sender, RoutedEventArgs e)
        {
            series[2].LineSeries.Visibility = Visibility.Hidden;
        }

        private void Graf4Chckd(object sender, RoutedEventArgs e)
        {
            series[3].LineSeries.Visibility = Visibility.Visible;
        }

        private void Graf4Unchckd(object sender, RoutedEventArgs e)
        {
            series[3].LineSeries.Visibility = Visibility.Hidden;
        }

        protected void SetArrays()
        {
            for (byte i = 0; i < 24; i++)
            {
                hours[i] = i;
            }
            for (byte i = 0; i < 60; i++)
            {
                minutes[i] = i;
            }
        }
    }
}
