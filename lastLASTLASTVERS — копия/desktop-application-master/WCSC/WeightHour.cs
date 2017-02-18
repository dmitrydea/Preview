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
    public partial class WeightHour : Form
    {
        int Count_WEIGHT = 0;
        List<DataByHours> data_hour = null;
        ChartValues<double> chart1DataY = new ChartValues<double>();
        ChartValues<double> chart2DataY = new ChartValues<double>();
        List<string> chart1DataX = new List<string>();
        List<string> chart2DataX = new List<string>();

        public WeightHour()
        {
            InitializeComponent();
            Get_Info_Data_Hour();
            
        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Get_Info_Data_Hour()
        {
            cartesianChart1.DisableAnimations = true;
            scalesEntities2 bd = new scalesEntities2();
            IQueryable<ScalesInformation> query = bd.ScalesInformation;
            Count_WEIGHT = query.Count();
            IQueryable<DataByHours> query_data_hour = bd.DataByHours.OrderByDescending(x => x.ID).Take(Check_Connection_with_Controllers.device_list.Count).OrderBy(x => x.ID);
            data_hour = query_data_hour.ToList();
            DrawGraph(data_hour);

        }

        private void DrawGraph(List<DataByHours> ds_info)
        {

            if (Count_WEIGHT > 5)
            {
                for (int i = 0; i < ds_info.Count; i++)
                {
                    if (i < 5)
                    {
                        chart1DataY.Add(ds_info[i].Weight.Value);
                        chart1DataX.Add(ds_info[i].ScalesNumberID.ToString());
                    }
                    else
                    {
                        chart2DataY.Add(ds_info[i].Weight.Value);
                        chart2DataX.Add(ds_info[i].ScalesNumberID.ToString());
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
                    Foreground = Brushes.Black,
                    MinValue = 0
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
                    MinValue = 0,
                    Foreground = Brushes.Black
                });

            }
            else
            {
                foreach (var item in ds_info)
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
                    MinValue = 0,
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
                    Foreground = Brushes.Black,
                    MinValue = 0
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
            IQueryable<DataByHours> query_data_hour = bd.DataByHours.OrderByDescending(x => x.ID).Take(Check_Connection_with_Controllers.device_list.Count).OrderBy(x => x.ID);
            List<DataByHours> dh_info = query_data_hour.ToList();
            if (Count_WEIGHT > 5)
            {
                for (int i = 0; i < dh_info.Count; i++)
                {
                    if (i < 5)
                    {
                        chart1DataY.Add(dh_info[i].Weight.Value);
                        chart1DataX.Add(dh_info[i].ScalesNumberID.ToString());
                    }
                    else
                    {
                        chart2DataY.Add(dh_info[i].Weight.Value);
                        chart2DataX.Add(dh_info[i].ScalesNumberID.ToString());
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
                foreach (var item in dh_info)
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
                { }
            }
            bd.Dispose();
        }

        private void WeightHour_Load(object sender, EventArgs e)
        {
            Form1.AllGraphRefresh += RefreshGraph;
        }
    }
}
