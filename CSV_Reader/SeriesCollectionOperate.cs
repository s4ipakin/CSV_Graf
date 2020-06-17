using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using System.Data;
using LiveCharts.Wpf;
using System.Windows;

namespace CSV_Reader
{
    class SeriesCollectionOperate
    {




        public string[] SetValues(IChartValues collection, DataTable dataTable, int dateColumn, int timeColumn, int dataColumn,
                                   System.DateTime startDate, System.DateTime endDate, int startHour, int endHour,
                                    int startMin, int endMin)
        {
            string[] labels = new string[dataTable.Rows.Count];
            Queue<string> queue = new Queue<string>();
            collection.Clear();
            System.DateTime currentDate = startDate;
            System.DateTime currentTime = startDate;//Convert.ToDateTime(dataTable.Rows[0][timeColumn]);
            System.DateTime stopTime = endDate;
            stopTime = stopTime.AddHours(endHour);
            stopTime = stopTime.AddMinutes(endMin);
            System.DateTime startTime = startDate;
            startTime = startTime.AddHours(startHour);
            startTime = startTime.AddMinutes(startMin);
            //object[] dateRow = dataTable.Columns[dateColumn];
            //object[] timeRow = dataTable.Columns[timeColumn];
            object[] values = new object[dataTable.Rows.Count];
            
            //System.DateTime[] _dateRow = new DateTime[dateRow.Length];
            //System.DateTime[] _timeRow = new DateTime[timeRow.Length];
            System.DateTime[] dataTime = new DateTime[dataTable.Rows.Count];
            Dictionary<System.DateTime, int> dates = new Dictionary<System.DateTime, int>();
            for (int j = 0; j < dataTable.Rows.Count; j++)
            {
                try
                {
                    dataTime[j] = Convert.ToDateTime(dataTable.Rows[j][dateColumn]);
                    int hours = Convert.ToDateTime(dataTable.Rows[j][timeColumn]).Hour;
                    int minutes = Convert.ToDateTime(dataTable.Rows[j][timeColumn]).Minute;
                    
                    dataTime[j] = dataTime[j].AddHours(hours);
                    dataTime[j] = dataTime[j].AddMinutes(minutes);
                    dates.Add(dataTime[j], j);
                    values[j] = dataTable.Rows[j][dataColumn];
                }
                catch (Exception ex) { }
            }
            int increment = (int)Math.Ceiling(((stopTime - startTime).TotalMinutes) / 720);
            
            while (startTime < stopTime)
            {
                queue.Enqueue(startTime.ToString());
                if (dates.ContainsKey(startTime))
                {
                    collection.Add(Convert.ToDouble(values[dates[startTime]]));
                }
                else
                {
                    collection.Add(0d);
                }
                startTime = startTime.AddMinutes(increment);
            }
            labels = queue.ToArray();
            return labels;
            
             
        }
    }
}
