﻿using LiveCharts;
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
    /// Interaction logic for Conductivity.xaml
    /// </summary>
    public partial class Conductivity : Page, INotifyPropertyChanged
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




        public Conductivity()
        {
            InitializeComponent();
            csvDataTable = new DataTable();
            dataset.Tables.Add(csvDataTable);
            SeriesCollection = new SeriesCollection();
            Labels = new[] { System.DateTime.Now.ToString() };
            YFormatter = value => value.ToString() + "uSm";
            series = new GrafSeries[]
            {
                new GrafSeries("Электропроводность ВВО(емкость)", 11),
                new GrafSeries("Электропроводность ВВО(основная петля)", 13),
                new GrafSeries("Электропроводность ВВО(дополнительная петля)", 15),
                new GrafSeries("Электропроводность ВВИ(емкость)", 21),
                new GrafSeries("Электропроводность ВВИ(циркулирующая)", 25)
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
            checkGraf5.IsChecked = true;

            Chart_P.Zoom = ZoomingOptions.X;
            DataContext = this;
            PickerDataStop.SelectedDateChanged += PickerDataStop_SelectedDateChanged;
            pickedData.SelectedDateChanged += PickedData_SelectedDateChanged;
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
                for (int i = 0; i < series.Length; i++)
                {
                    Labels = seriesCollectionOperate.SetValues(SeriesCollection[i].Values, csvDataTable, 1, 2, series[i].Column,
                    (System.DateTime)pickedData.SelectedDate, (System.DateTime)PickerDataStop.SelectedDate, comboBoxHourStart.SelectedIndex,
                    comboBoxHourEnd.SelectedIndex, comboBoxMinuteStart.SelectedIndex, comboBoxMinuteEnd.SelectedIndex);
                }
            }
            catch (Exception ex) { System.Windows.MessageBox.Show("Укажите время и дату начала и конца графика"); }
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null) // if subrscribed to event
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void PickedData_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            PickerDataStop.DisplayDateStart = pickedData.SelectedDate;
        }

        protected void PickerDataStop_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            pickedData.DisplayDateEnd = PickerDataStop.SelectedDate;
        }

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

        private void Graf5Chckd(object sender, RoutedEventArgs e)
        {
            series[4].LineSeries.Visibility = Visibility.Visible;
        }

        private void Graf5Unchckd(object sender, RoutedEventArgs e)
        {
            series[4].LineSeries.Visibility = Visibility.Hidden;
        }
    }
}
