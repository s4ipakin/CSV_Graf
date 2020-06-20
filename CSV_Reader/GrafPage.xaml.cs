using LiveCharts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
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
    /// Interaction logic for GrafPage.xaml
    /// </summary>
    public partial class GrafPage : Page, INotifyPropertyChanged
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
        {
            set
            {
                filePath = value;
            }
        }

        protected GrafSeries[] arSeries;
        protected Dictionary<int, CheckBox> checkBoxes = new Dictionary<int, CheckBox>();

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

        public GrafPage(string yAxesName, string unit, double maxValue)
        {
            InitializeComponent();
            SetSeries();
            chartAxisY.Title = yAxesName;
            chartAxisY.MaxValue = maxValue;
            csvDataTable = new DataTable();
            dataset.Tables.Add(csvDataTable);
            SeriesCollection = new SeriesCollection();
            Labels = new[] { System.DateTime.Now.ToString() };
            YFormatter = value => value.ToString() + unit;
            for (int i = 0; i < arSeries.Length; i++)
            {
                SeriesCollection.Add(arSeries[i].LineSeries);
                checkBoxes.Add(i, new CheckBox());
                checkBoxes[i].HorizontalAlignment = HorizontalAlignment.Left;
                checkBoxes[i].VerticalAlignment = VerticalAlignment.Bottom;
                double chartBtmMargin = Chart_P.Margin.Bottom;
                checkBoxes[i].Margin = new Thickness(49,0,0, chartBtmMargin - ((i + 1) * 30));
                checkBoxes[i].Content = arSeries[i].Name;
                checkBoxes[i].IsChecked = true;                
            }
            
            foreach (KeyValuePair<int, CheckBox> keyValuePair in checkBoxes )
            {               
                keyValuePair.Value.Checked += (sender, e) => ChckbxChecked(sender, e, keyValuePair.Key);
                keyValuePair.Value.Unchecked += (sender, e) => ChckbxUnchecked(sender, e, keyValuePair.Key);
                this.grid.Children.Add(keyValuePair.Value);
            }
            
            SetArrays();
            comboBoxHourStart.ItemsSource = hours;
            comboBoxHourEnd.ItemsSource = hours;
            comboBoxMinuteStart.ItemsSource = minutes;
            comboBoxMinuteEnd.ItemsSource = minutes;
            Chart_P.Zoom = ZoomingOptions.X;
            DataContext = this;
            PickerDataStop.SelectedDateChanged += PickerDataStop_SelectedDateChanged;
            pickedData.SelectedDateChanged += PickedData_SelectedDateChanged;
            
        }

        protected virtual void SetSeries()
        {
            arSeries = new GrafSeries[] { };
        }

        private void ChckbxChecked(object sender, RoutedEventArgs e, int index)
        {
            arSeries[index].LineSeries.Visibility = Visibility.Visible;
        }
        private void ChckbxUnchecked(object sender, RoutedEventArgs e, int index)
        {
            arSeries[index].LineSeries.Visibility = Visibility.Hidden;
        }

        protected void PickedData_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            PickerDataStop.DisplayDateStart = pickedData.SelectedDate;
        }

        protected void PickerDataStop_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            pickedData.DisplayDateEnd = PickerDataStop.SelectedDate;
        }



        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null) // if subrscribed to event
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                    g.CopyFromScreen(new System.Drawing.Point((int)position.X, (int)position.Y), System.Drawing.Point.Empty, bounds.Size);
                }
                System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
                saveFileDialog1.Filter = "JPeg Image|*.jpg";
                saveFileDialog1.Title = "Сохранить график как изображение JPeg";
                saveFileDialog1.ShowDialog();
                string imagePath = saveFileDialog1.FileName;
                bitmap.Save(imagePath, ImageFormat.Jpeg);
            }
        }

        protected void btnFromDays_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                csvDataTable = CSV_DataTable.ConvertCSVtoDataTable(filePath);
            }
            catch (Exception ex) { }

            SeriesCollectionOperate seriesCollectionOperate = new SeriesCollectionOperate();
            try
            {
                for (int i = 0; i < arSeries.Length; i++)
                {
                    Labels = seriesCollectionOperate.SetValues(SeriesCollection[i].Values, csvDataTable, 1, 2, arSeries[i].Column,
                    (System.DateTime)pickedData.SelectedDate, (System.DateTime)PickerDataStop.SelectedDate, comboBoxHourStart.SelectedIndex,
                    comboBoxHourEnd.SelectedIndex, comboBoxMinuteStart.SelectedIndex, comboBoxMinuteEnd.SelectedIndex);
                }
            }
            catch (Exception ex) { System.Windows.MessageBox.Show(ex.Message); }
        }
    }
}
