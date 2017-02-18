using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brushes = System.Windows.Media.Brushes;

using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;

namespace WCSC
{
    public partial class WeightInformation : Form
    {
        bool first_start = false;
        bool clearData = false;
        public static string CURRENT_DEVICE_COMBO;

        public ChartValues<MeasureModel> ChartValues { get; set; }
        public ChartValues<MeasureModel> ChartValuesWEIGHT { get; set; }
        List<string> lableX = new List<string>();

        public WeightInformation()
        {
            InitializeComponent();
            Form1.DataLive += Form1_DataLive;
            StartShowTimeDate();
            LoadComboBoxDevice();
            DrawGrph();
            //cartesianChart2.DisableAnimations = true;
            //cartesianChart3.DisableAnimations = true;
            //The next code simulates data changes every 500 ms
        }

        private void Form1_DataLive(double weightReal, double powerReal, double wHour, double wDay, double wMonth)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    textBox4.Text = weightReal.ToString("F2");
                    textBox5.Text = powerReal.ToString("F2");

                    textBox8.Text = wHour.ToString("F2");
                    textBox9.Text = wDay.ToString("F2");
                    textBox10.Text = wMonth.ToString("F2");
                });
                //textBox4.Text = Form1.WEIGHT_STATIC_SHIFT.ToString();


                var now = System.DateTime.Now;

                ChartValues.Add(new MeasureModel
                {
                    Value = powerReal
                });

                ChartValuesWEIGHT.Add(new MeasureModel
                {
                    Value = weightReal
                    //Value = dbs.Weight.Value
                });

                lableX.Add(now.ToString("HH:mm:ss") + "\n" + now.ToString("dd.MM.yy"));
                this.Invoke((MethodInvoker)delegate
                {
                    cartesianChart3.AxisX[0].Labels = lableX;
                    cartesianChart2.AxisX[0].Labels = lableX;
                //MessageBox.Show(TimeSpan.FromSeconds(1).Ticks.ToString(),"");
                //SetAxisLimits(now);
                cartesianChart3.Refresh();
                    cartesianChart2.Refresh();
                });
                //lets only use the last 30 values
                if (ChartValues.Count > 14)
                {
                    ChartValues.RemoveAt(0);
                    ChartValuesWEIGHT.RemoveAt(0);
                    lableX.RemoveAt(0);
                }
            }
            catch(Exception e)
            { }

        }

        ///////////////////////////////////////////////////////////////////
        /// </summary>

        private void LoadComboBoxDevice()
        {
            List<Device> list_con_device = Check_Connection_with_Controllers.device_list;
            comboBox1.Items.Clear();
            foreach (var item in list_con_device)
            {
                comboBox1.Items.Add(item.device_name + " " + item.AorK);
            }
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
                CURRENT_DEVICE_COMBO = comboBox1.SelectedItem.ToString().Split(' ')[0];
            }
            
        }

        private void WeightInformation_Load(object sender, EventArgs e)
        {
           
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void StartShowTimeDate()
        {
            Timer timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_tick);
            timer1.Interval = 100;
            timer1.Start();
        }

        public void timer1_tick(object sender, EventArgs e)
        {
            DateTime ThToday = DateTime.Now;
            string ThData = ThToday.Date.ToShortDateString();
            string ThTime = ThToday.ToLongTimeString();
            this.textBox1.Text = ThData;
            this.textBox2.Text = ThTime;
        }

        public void DrawGrph()
        {
            var mapper = Mappers.Xy<MeasureModel>()   //use DateTime.Ticks as X
               .Y(model => model.Value);           //use the value property as Y

            //lets save the mapper globally.
            Charting.For<MeasureModel>(mapper);

            //the ChartValues property will store our values array
            ChartValues = new ChartValues<MeasureModel>();
            ChartValuesWEIGHT = new ChartValues<MeasureModel>();
            clearData = true;
            //cartesianChart1.DisableAnimations = true;
            cartesianChart3.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = ChartValues,
                    PointGeometrySize = 18,
                    LineSmoothness = 0,
                    Title = "Производительность:"

                }
            };
            cartesianChart3.AxisX.Add(new Axis
            {
                DisableAnimations = true,
                Labels = new string[] { "", "", "", "", "", "", "", "", "" },
                MinValue = 0,
                Foreground = Brushes.Black,
                Separator = new Separator
                {
                    Step = 1
                }
            });

            cartesianChart3.AxisY.Add(new Axis
            {
                Title = "Производительность, т/ч",
                LabelFormatter = value => value.ToString("F4"),
                Foreground = System.Windows.Media.Brushes.Black,
                MinValue = 0

            });

            cartesianChart2.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = ChartValuesWEIGHT,
                    PointGeometrySize = 18,
                    LineSmoothness = 0,
                    Title = "Накопленый вес:"

                }
            };
            cartesianChart2.AxisX.Add(new Axis
            {
                DisableAnimations = true,
                Labels = new string[] { "", "", "", "", "", "", "", "", "" },
                MinValue = 0,
                Foreground = Brushes.Black,
                Separator = new Separator
                {
                    Step = 1
                }
            });

            cartesianChart2.AxisY.Add(new Axis
            {
                Title = "Вес, кг",
                LabelFormatter = value => value.ToString("F2"),
                Foreground = System.Windows.Media.Brushes.Black,
                MinValue = 0

            });
            //////////////////////////////////////////////////////////
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lableX.Clear();
            if (clearData == true)
            {
                ChartValues.Clear();
                ChartValuesWEIGHT.Clear();
            }    
            //chart3DataX.Clear();
            //chart3DataY.Clear();
            CURRENT_DEVICE_COMBO = comboBox1.SelectedItem.ToString().Split(' ')[0];
        }
    }
    public class MeasureModel
    {
        public double Value { get; set; }
    }

}

