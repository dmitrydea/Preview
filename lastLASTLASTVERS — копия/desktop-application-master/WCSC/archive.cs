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
    public partial class archive : Form
    {
        int RadioScale;
        int positionInArray = 0;
        int chechko = 15;
        int next = 0;
        List<MeasurementData> arhiveList;
        ChartValues<double> ChartValuesWEIGHT_arhive = new ChartValues<double>();
        List<string> lableX_arhive = new List<string>();

        ChartValues<double> ChartValuesWEIGHT_arhive_CHAST = new ChartValues<double>();

        ChartValues<double> ChartValuesPOWER_arhive = new ChartValues<double>();
        List<string> lableX2_arhive = new List<string>();

        public archive()
        {
            InitializeComponent();
            DrawGraph();
            radioButton1.Checked = true;
            
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void DrawGraph()
        {
            
            
            cartesianChart1.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = ChartValuesWEIGHT_arhive,
                    PointGeometrySize = 18,
                    LineSmoothness = 0,
                    Title = "Вес:"

                }
            };
            cartesianChart1.AxisX.Add(new Axis
            {
                DisableAnimations = true,
                Labels = lableX_arhive,
                MinValue = 0,
                Foreground = Brushes.Black,
                Separator = new Separator
                {
                    Step = 1
                }
            });

            cartesianChart1.AxisY.Add(new Axis
            {
                Title = "Вес, кг",
                LabelFormatter = value => value.ToString("F2"),
                Foreground = System.Windows.Media.Brushes.Black,
                MinValue = 0

            });
            //////////////////////////////////////////////////////
            cartesianChart2.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = ChartValuesPOWER_arhive,
                    PointGeometrySize = 18,
                    LineSmoothness = 0,
                    Title = "Производительность:"

                }
            };
            cartesianChart2.AxisX.Add(new Axis
            {
                DisableAnimations = true,
                Labels = lableX_arhive,
                MinValue = 0,
                Foreground = Brushes.Black,
                Separator = new Separator
                {
                    Step = 1
                }
            });

            cartesianChart2.AxisY.Add(new Axis
            {
                Title = "Производительность, т/ч",
                LabelFormatter = value => value.ToString("F2"),
                Foreground = System.Windows.Media.Brushes.Black,
                MinValue = 0

            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void archive_Load(object sender, EventArgs e)
        {
            dateTimePicker1.CustomFormat = "dd.MM.yy HH:mm:ss";
            //this.MinimumSize = SystemInformation.VirtualScreen.Size;
            scalesEntities2 bd = new scalesEntities2();
            List<ScalesInformation> query = bd.ScalesInformation.ToList();
            foreach (var item in query)
            {
                comboBox1.Items.Add(item.ID + " " + item.Scales_Number);
            }          
        }

        private void GetDataForArhiveGraph(List<MeasurementData> arhiveList)
        {
            ChartValuesPOWER_arhive.Clear();
            ChartValuesWEIGHT_arhive.Clear();
            lableX_arhive.Clear();
            if (chechko >= arhiveList.Count)
            {
                chechko = arhiveList.Count;
                next = chechko - 15;
            }
            if (next < 0)
            {
                chechko = 15;
                next = 0;
            }

            for (int i = next; i < chechko; i++)
            {
                if ( i >= arhiveList.Count)
                {
                    ChartValuesWEIGHT_arhive.Add(0);
                    ChartValuesPOWER_arhive.Add(0);
                    lableX_arhive.Add("");
                }
                else
                {
                    ChartValuesWEIGHT_arhive.Add(arhiveList[i].CurrentWeight.Value);
                    ChartValuesPOWER_arhive.Add(arhiveList[i].CurrentProductivity.Value);
                    lableX_arhive.Add(arhiveList[i].TimeOfMeasurement.Value.ToString("HH:mm:ss") + "\n" + arhiveList[i].TimeOfMeasurement.Value.ToString("dd.MM.yy"));
                }
                
                
            }
            cartesianChart1.AxisX[0].Labels = lableX_arhive;
            cartesianChart1.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = ChartValuesWEIGHT_arhive,
                    PointGeometrySize = 18,
                    LineSmoothness = 0,
                    Title = "Вес:"

                }
            };
            cartesianChart2.AxisX[0].Labels = lableX_arhive;
            cartesianChart2.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = ChartValuesPOWER_arhive,
                    PointGeometrySize = 18,
                    LineSmoothness = 0,
                    Title = "Производительность:"

                }
            };
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            next = 0;
            chechko = 15;
            textBox2.Text = "";
            textBox5.Text = "";
            textBox4.Text = "";
            textBox3.Text = "";
            DateTime dtStart = dateTimePicker1.Value;
            DateTime dtEnd;
            if (radioButton1.Checked)
            {
                dtEnd = dtStart.AddMinutes(RadioScale);
            }
            else
            {
                dtEnd = dtStart.AddHours(RadioScale);
            }
            //MessageBox.Show(dtEnd.ToString());
            int Combo = int.Parse(comboBox1.SelectedItem.ToString().Split(' ')[0]);
            scalesEntities2 bd = new scalesEntities2();
            IQueryable<MeasurementData> query_data_mes = bd.MeasurementData.Where(x => (x.TimeOfMeasurement >= dtStart && x.TimeOfMeasurement <= dtEnd) && x.ScalesNumberID == Combo );
            arhiveList = query_data_mes.ToList();
            if (arhiveList.Count == 0)
            {
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
                button7.Enabled = false;
                ChartValuesPOWER_arhive.Clear();
                ChartValuesWEIGHT_arhive.Clear();
                lableX_arhive.Clear();
            }
            else
            {
                ChartValuesPOWER_arhive.Clear();
                ChartValuesWEIGHT_arhive.Clear();
                lableX_arhive.Clear();
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
                button6.Enabled = true;
                button7.Enabled = true;
                GetDataForArhiveGraph(arhiveList);
                button1.Focus();
            }
            this.WindowState = FormWindowState.Maximized;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = true;
            RadioScale = 10;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = true;
            RadioScale = 6;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = true;
            RadioScale = 1;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = true;
            RadioScale = 23;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            next -= 15;
            chechko -= 15;
            GetDataForArhiveGraph(arhiveList);
            //cartesianChart1.AxisX[0].MinValue -= 15;
            //cartesianChart1.AxisX[0].MaxValue -= 15;

            //cartesianChart2.AxisX[0].MinValue -= 15;
            //cartesianChart2.AxisX[0].MaxValue -= 15;

            //if (cartesianChart1.AxisX[0].MinValue < 0)
            //{
            //    cartesianChart1.AxisX[0].MinValue = 0;
            //    cartesianChart1.AxisX[0].MaxValue = 15;
            //}

            //if (cartesianChart2.AxisX[0].MinValue < 0)
            //{
            //    cartesianChart2.AxisX[0].MinValue = 0;
            //    cartesianChart2.AxisX[0].MaxValue = 15;
            //}

        }

        private void button6_Click(object sender, EventArgs e)
        {
            next += 15;
            chechko += 15;
            GetDataForArhiveGraph(arhiveList);

            //cartesianChart1.AxisX[0].MinValue += 15;
            //cartesianChart1.AxisX[0].MaxValue += 15;

            //if (cartesianChart1.AxisX[0].MaxValue > ChartValuesWEIGHT_arhive.Count)
            //{
            //    cartesianChart1.AxisX[0].MinValue = ChartValuesWEIGHT_arhive.Count - 15;
            //    cartesianChart1.AxisX[0].MaxValue = ChartValuesWEIGHT_arhive.Count;
            //}

            //cartesianChart2.AxisX[0].MinValue += 15;
            //cartesianChart2.AxisX[0].MaxValue += 15;

            //if (cartesianChart2.AxisX[0].MaxValue > ChartValuesPOWER_arhive.Count)
            //{
            //    cartesianChart2.AxisX[0].MinValue = ChartValuesPOWER_arhive.Count - 15;
            //    cartesianChart2.AxisX[0].MaxValue = ChartValuesPOWER_arhive.Count;
            //}

        }

        private void button4_Click(object sender, EventArgs e)
        {
            next -= 1;
            chechko -= 1;
            GetDataForArhiveGraph(arhiveList);
            //cartesianChart1.AxisX[0].MinValue -= 1;
            //cartesianChart1.AxisX[0].MaxValue -= 1;
            //cartesianChart2.AxisX[0].MinValue -= 1;
            //cartesianChart2.AxisX[0].MaxValue -= 1;

            //if (cartesianChart1.AxisX[0].MinValue < 0)
            //{
            //    cartesianChart1.AxisX[0].MinValue = 0;
            //    cartesianChart1.AxisX[0].MaxValue = 15;
            //}

            //if (cartesianChart2.AxisX[0].MinValue < 0)
            //{
            //    cartesianChart2.AxisX[0].MinValue = 0;
            //    cartesianChart2.AxisX[0].MaxValue = 15;
            //}
        }

        private void ChartOnDataClick(object sender, ChartPoint p)
        {
            textBox2.Text = p.Y.ToString("F2");
            int index = Convert.ToInt32(p.X);

            int IndToMas = index + next;
            string[] dat_tim = lableX_arhive[index].Split('\n');
            textBox5.Text = dat_tim[0];
            textBox4.Text = dat_tim[1];
            textBox3.Text = ChartValuesPOWER_arhive[index].ToString("F2");
        }

        private void ChartOnDataClickPower(object sender, ChartPoint p)
        {
            textBox3.Text = p.Y.ToString("F2");
            int index = Convert.ToInt32(p.X);
            string[] dat_tim = lableX_arhive[index].Split('\n');
            textBox5.Text = dat_tim[0];
            textBox4.Text = dat_tim[1];
            textBox2.Text = ChartValuesWEIGHT_arhive[index].ToString("F2");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            next += 1;
            chechko += 1;
            GetDataForArhiveGraph(arhiveList);
            //cartesianChart1.AxisX[0].MinValue += 1;
            //cartesianChart1.AxisX[0].MaxValue += 1;
            //cartesianChart2.AxisX[0].MinValue += 1;
            //cartesianChart2.AxisX[0].MaxValue += 1;

            //if (cartesianChart1.AxisX[0].MaxValue > ChartValuesWEIGHT_arhive.Count)
            //{
            //    cartesianChart1.AxisX[0].MinValue = ChartValuesWEIGHT_arhive.Count - 15;
            //    cartesianChart1.AxisX[0].MaxValue = ChartValuesWEIGHT_arhive.Count;
            //}

            //if (cartesianChart2.AxisX[0].MaxValue > ChartValuesPOWER_arhive.Count)
            //{
            //    cartesianChart2.AxisX[0].MinValue = ChartValuesPOWER_arhive.Count - 15;
            //    cartesianChart2.AxisX[0].MaxValue = ChartValuesPOWER_arhive.Count;
            //}
        }

        private void button2_Click(object sender, EventArgs e)
        {
            chechko = 15;
            next = 0;

            GetDataForArhiveGraph(arhiveList);
            //cartesianChart1.AxisX[0].MinValue = 0;
            //cartesianChart1.AxisX[0].MaxValue = 15;

            //cartesianChart2.AxisX[0].MinValue = 0;
            //cartesianChart2.AxisX[0].MaxValue = 15;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            chechko = arhiveList.Count;
            next = chechko - 15;
            
            GetDataForArhiveGraph(arhiveList);
            //cartesianChart1.AxisX[0].MinValue = ChartValuesWEIGHT_arhive.Count - 15;
            //cartesianChart1.AxisX[0].MaxValue = ChartValuesWEIGHT_arhive.Count;

            //cartesianChart2.AxisX[0].MinValue = ChartValuesPOWER_arhive.Count - 15;
            //cartesianChart2.AxisX[0].MaxValue = ChartValuesPOWER_arhive.Count;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
