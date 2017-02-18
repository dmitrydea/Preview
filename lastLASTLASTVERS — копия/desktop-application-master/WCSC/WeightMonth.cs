using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Brushes = System.Windows.Media.Brushes;

namespace WCSC
{
    public partial class WeightMonth : Form
    {
        int Count_WEIGHT = 0;
        List<DataByMonth> data_month = null;
        ChartValues<double> chart1DataY = new ChartValues<double>();
        ChartValues<double> chart2DataY = new ChartValues<double>();
        List<string> chart1DataX = new List<string>();
        List<string> chart2DataX = new List<string>();

        public WeightMonth()
        {
            InitializeComponent();
            Get_Info_Data_Month();
            Form1.AllGraphRefresh += RefreshGraph;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Get_Info_Data_Month()
        {
            cartesianChart1.DisableAnimations = true;
            scalesEntities2 bd = new scalesEntities2();
            IQueryable<ScalesInformation> query = bd.ScalesInformation;
            Count_WEIGHT = query.Count();
            IQueryable<DataByMonth> query_data_month = bd.DataByMonth.OrderByDescending(x => x.ID).Take(Check_Connection_with_Controllers.device_list.Count).OrderBy(x => x.ID);
            data_month = query_data_month.ToList();
            DrawGraph(data_month);
        }

        private void DrawGraph(List<DataByMonth> dm_info)
        {

            if (Count_WEIGHT > 5)
            {
                for (int i = 0; i < dm_info.Count; i++)
                {
                    if (i < 5)
                    {
                        chart1DataY.Add(dm_info[i].Weight.Value);
                        chart1DataX.Add(dm_info[i].ScalesNumberID.ToString());
                    }
                    else
                    {
                        chart2DataY.Add(dm_info[i].Weight.Value);
                        chart2DataX.Add(dm_info[i].ScalesNumberID.ToString());
                    }
                }
                cartesianChart1.Series = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Вес:",
                        Values = chart1DataY,
                        DataLabels = true
                    }
                };

                cartesianChart1.AxisX.Add(new Axis
                {
                    Title = "Номер весов",
                    Labels = chart1DataX,
                    Separator = new Separator
                    {
                        Step = 1
                    },

                    Foreground = Brushes.Black

                });

                cartesianChart1.AxisY.Add(new Axis
                {
                    Title = "Общий вес по весам",
                    LabelFormatter = value => value.ToString("N"),
                    Separator = new Separator(),
                    Foreground = Brushes.Black
                });

                cartesianChart2.Series = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Вес:",
                        Values = chart2DataY,
                        DataLabels = true
                    }
                };

                cartesianChart2.AxisX.Add(new Axis
                {
                    Title = "Номер весов",
                    Labels = chart2DataX,
                    Separator = new Separator
                    {
                        Step = 1,
                        //Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0))
                    },

                    Foreground = Brushes.Black

                });

                cartesianChart2.AxisY.Add(new Axis
                {
                    Title = "Общий вес по весам",
                    LabelFormatter = value => value.ToString("N"),
                    Separator = new Separator(),

                    Foreground = Brushes.Black
                });

            }
            else
            {
                foreach (var item in dm_info)
                {
                    chart1DataY.Add(item.Weight.Value);
                    chart1DataX.Add(item.ScalesNumberID.ToString());
                }
                cartesianChart1.Series = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Вес:",
                        Values = chart1DataY,
                        DataLabels = true
                    }
                };

                cartesianChart1.AxisX.Add(new Axis
                {
                    Title = "Номер весов",
                    Labels = chart1DataX,
                    Separator = new Separator
                    {
                        Step = 1
                    },

                    Foreground = Brushes.Black

                });

                cartesianChart1.AxisY.Add(new Axis
                {
                    Title = "Общий вес по весам",
                    LabelFormatter = value => value.ToString("N"),
                    Separator = new Separator(),
                    Foreground = Brushes.Black
                });

                cartesianChart2.AxisX.Add(new Axis
                {
                    Title = "Номер весов",
                    MinValue = 0,
                    Separator = new Separator
                    {
                        Step = 1,
                        //Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0))
                    },

                    Foreground = Brushes.Black

                });

                cartesianChart2.AxisY.Add(new Axis
                {
                    Title = "Общий вес по весам",

                    Separator = new Separator(),
                    MinValue = 0,
                    Foreground = Brushes.Black
                });
            }

        }

        private void RefreshGraph()
        {

            chart1DataX.Clear();
            chart2DataX.Clear();
            chart1DataY.Clear();
            chart2DataY.Clear();
            scalesEntities2 bd = new scalesEntities2();
            IQueryable<ScalesInformation> query = bd.ScalesInformation;
            Count_WEIGHT = query.Count();
            IQueryable<DataByMonth> query_data_month = bd.DataByMonth.OrderByDescending(x => x.ID).Take(Check_Connection_with_Controllers.device_list.Count).OrderBy(x => x.ID);
            List<DataByMonth> dm_info = query_data_month.ToList();
            if (Count_WEIGHT > 5)
            {
                for (int i = 0; i < dm_info.Count; i++)
                {
                    if (i < 5)
                    {
                        chart1DataY.Add(dm_info[i].Weight.Value);
                        chart1DataX.Add(dm_info[i].ScalesNumberID.ToString());
                    }
                    else
                    {
                        chart2DataY.Add(dm_info[i].Weight.Value);
                        chart2DataX.Add(dm_info[i].ScalesNumberID.ToString());
                    }
                }
                try
                { 
                    this.Invoke((MethodInvoker)delegate
                    {
                        cartesianChart1.Series = new SeriesCollection
                        {
                            new ColumnSeries
                            {
                                Title = "Вес:",
                                Values = chart1DataY,
                                DataLabels = true
                            }
                        };

                        cartesianChart2.Series = new SeriesCollection
                        {
                            new ColumnSeries
                            {
                                Title = "Вес:",
                                Values = chart2DataY,
                                DataLabels = true
                            }
                        };
                    });
                }
                catch (Exception e)
                {

                }
        }
            else
            {
                foreach (var item in dm_info)
                {
                    chart1DataY.Add(item.Weight.Value);
                    chart1DataX.Add(item.ScalesNumberID.ToString());
                }
                try
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        cartesianChart1.Series = new SeriesCollection
                        {
                            new ColumnSeries
                            {
                                Title = "Вес:",
                                Values = chart1DataY,
                                DataLabels = true
                            }
                        };
                    });
                }
                catch(Exception e)
                {

                }
            }
            bd.Dispose();
        }

        private void WeightMonth_Load(object sender, EventArgs e)
        {

        }
    }
}
