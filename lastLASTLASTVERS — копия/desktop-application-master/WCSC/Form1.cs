using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.WinForms;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using System.Net.Sockets;
//using System.Timers;
//using System.Threading;
using System.Speech.Synthesis;


namespace WCSC
{
    public partial class Form1 : Form
    {
        SpeechSynthesizer speek = new SpeechSynthesizer();
        //Thread NewConnectionCreate = null;
        // Объявляем делегат
        public delegate void DataChangeHandler();
        // Событие, возникающее при выводе денег
        public static event DataChangeHandler AllGraphRefresh;

        public static double WEIGHT_STATIC_SHIFT = 0;
        // Объявляем делегат
        public delegate void DataChangeFORwi(double weightReal, double powerReal, double hour, double day, double month);
        // Событие, возникающее при выводе денег
        public static event DataChangeFORwi DataLive;

        public static string USER;

        List<Device> device_list = null;
        int Count_WEIGHT = 0;
        //scalesEntities bd = Check_Connection_with_Controllers.bd;
        List<DataByShift> data_shift = null;
        ChartValues<double> chart1DataY = new ChartValues<double>();
        ChartValues<double> chart2DataY = new ChartValues<double>();
        List<string> chart1DataX = new List<string>();
        List<string> chart2DataX = new List<string>();

        public Form1()
        {
            Login logg = new Login();
            logg.ShowDialog();
            if (Login.CLOSE_ALL == true)
            {
                return;
            }
            Check_Connection_with_Controllers ck = new Check_Connection_with_Controllers();
            ck.ShowDialog();
            InitializeComponent();

            if (Login.Role_ID == 1)
            {
                button5.Enabled = false;
            }
            if (Login.Role_ID == 2)
            {
                button5.Enabled = true;
            }
            label2.Text = USER;
            //RefreshGraph();
            Get_Info_Data_shit();
            device_list = Check_Connection_with_Controllers.device_list;

            StartShowTime();
            cartesianChart1.DisableAnimations = true;
            speek.SetOutputToDefaultAudioDevice();
            // FOR CONTROLLER

        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        public void GetCRC16(byte[] message, ref byte[] CRC16)
        {
            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < (message.Length); i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);
                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);
                    if (CRCLSB == 1)
                    {
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                    }
                }
                CRC16[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
                CRC16[0] = CRCLow = (byte)(CRCFull & 0xFF);
            }
        }

        private void Get_Info_Data_shit()
        {
            cartesianChart1.DisableAnimations = true;
            scalesEntities2 bd = new scalesEntities2();
            IQueryable<ScalesInformation> query = bd.ScalesInformation;
            Count_WEIGHT = query.Count();
            IQueryable<DataByShift> query_data_shift = bd.DataByShift.OrderByDescending(x => x.ID).Take(Check_Connection_with_Controllers.device_list.Count).OrderBy(x => x.ID);
            data_shift = query_data_shift.ToList();
            DrawGraph(data_shift);
            
        }

        private void StartShowTime()
        {
            System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
            timer1.Tick += new EventHandler(timer1_tick);
            timer1.Interval = 100;
            timer1.Start();

            System.Windows.Forms.Timer timer2 = new System.Windows.Forms.Timer();
            timer2.Tick += new EventHandler(timer2_tick);
            timer2.Interval = 100;
            timer2.Start();

            //System.Timers.Timer timer4 = new System.Timers.Timer();
            //timer4.Elapsed += new ElapsedEventHandler(Timer4_Elapsed);
            //timer4.Interval = 1000;
            //timer4.Enabled = true;
            Timer timer4 = new Timer();
            timer4.Tick += new EventHandler(timer4_tick);
            timer4.Interval = 1000;
            timer4.Start();



        }

        //private void Timer4_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    GetInfo();
        //}

        public void timer2_tick(object sender, EventArgs e)
        {
            DateTime ThToday = DateTime.Now;
            string shift_day_start = "08:00:00";
            string shift_day_end = "20:00:00";
            TimeSpan dt_day = TimeSpan.Parse(shift_day_start);
            TimeSpan dt_end = TimeSpan.Parse(shift_day_end);


            if (ThToday.TimeOfDay > dt_day && ThToday.TimeOfDay < dt_end)
            {
                textBox2.Text = "1-я смена. День";
                

            }
            else
            {
                textBox2.Text = "2-я смена. Ночь";
                
            }
        }

        public void timer1_tick(object sender, EventArgs e)
        {
            DateTime ThToday = DateTime.Now;
            string ThData = ThToday.ToString();
            this.textBox1.Text = ThData;
            

            

            
            //IQueryable<ScalesInformation> query = bd.ScalesInformation;
            //test = query.ToList();
        }   

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

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
            //var ListDevRab = new List<int>() {1 };
            int[] devL = new int[device_list.Count];
            for (int i = 0; i < devL.Length; i++)
            {
                devL[i] = device_list[i].device_name;
            }
            IQueryable<DataByShift> query_data_shift = bd.DataByShift.OrderByDescending(x => x.ID).Take(device_list.Count).Where( x => devL.Contains(x.ScalesNumberID.Value)).OrderBy(x => x.ID);
            List<DataByShift> ds_info = query_data_shift.ToList();
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
            else
            {
                foreach (var item in ds_info)
                {
                    chart1DataY.Add(item.Weight.Value);
                    chart1DataX.Add(item.ScalesNumberID.ToString());
                }
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
            bd.Dispose();
        }

        private void DrawGraph(List<DataByShift> ds_info)
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
                    MinValue = 0,

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

        private void button8_Click(object sender, EventArgs e)
        {
            //if (NewConnectionCreate == null)
            //{
                
            //}
            //else
            //{
            //    NewConnectionCreate.Abort();
            //}          
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (MessageBox.Show("Вы действительно хотите выйти из приложения?", "Внимание!",
                      MessageBoxButtons.YesNo,
                      MessageBoxIcon.Question) == DialogResult.No)
                e.Cancel = true;
            else
                Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WeightHour wh = new WeightHour();
            wh.Show();
            //RefreshGraph();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            WeightInformation w = new WeightInformation();
            w.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
           
            Settings s = new Settings();
                s.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            archive form_arhive = new archive();
            form_arhive.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            WeightDay wd = new WeightDay();
            wd.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            WeightMonth wm = new WeightMonth();
            wm.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        // FOR CONTROLLER

        private void timer4_tick(object sender, EventArgs e)
        {
            GetInfo();
        }

        private void GetInfo()
        {
           
            //List<Device> NoWork = new List<Device>();
            for (int q= 0; q< device_list.Count; q++)
            {
                NetworkStream serverStream = default(NetworkStream);
                try
                {
                    bool NoDevice = false;
                    TcpClient clientSocket = new TcpClient();
                    foreach (var dev in device_list)
                    {
                        if (device_list[q].device_ip == dev.device_ip)
                        {
                            device_list[q].clientSocket = dev.clientSocket;
                            //serverStream = dev.clientSocket.GetStream();
                        }
                        else
                        {
                            NoDevice = true;
                        }
                    }
                    //clientSocket.Connect(item.device_ip, 9761);
                    serverStream = device_list[q].clientSocket.GetStream();

                    byte NumbDevice = byte.Parse(device_list[q].device_number);
                    byte[] otvet = new byte[2];
                    GetCRC16(new byte[] { NumbDevice, 3, 0, 19, 0, 1 }, ref otvet);
                    string CRC_L = ByteToStrHex(otvet[0]);
                    string CRC_H = ByteToStrHex(otvet[1]);

                    String[] message = new String[] { device_list[q].device_number, "03", "00", "13", "00", "01", CRC_L, CRC_H };
                    Byte[] mes = new Byte[128];         //переменная, которая будет содержать данные для отправки
                    int i = 0;                      // счетчик

                    for (i = 0; i < 8; i++)
                    {

                        mes[i] = StrHexToByte(message[i]);

                    }

                    serverStream.Write(mes, 0, 8);
                    serverStream.Flush();
                    clientSocket.ReceiveTimeout = 1010;
                    
                    serverStream = device_list[q].clientSocket.GetStream();            //получаем поток
                    int buffSize = 0;
                    int bytesRead = 0;
                    byte[] inStream = new byte[10025];                  // инициализируем массив для приема данных
                    buffSize = device_list[q].clientSocket.ReceiveBufferSize;          //получаем размер буфера

                        
                        bytesRead = serverStream.Read(inStream, 0, buffSize);//считываем данные из потока
                        this.Invoke((MethodInvoker)delegate
                        {
                            this.Text = "";
                        });
                   
                    
                    

                    if (bytesRead > 0)
                    {


                       string answer_Power = "";
                        
                        for (int z = 3; z < 7; z++)
                        {
                            answer_Power = answer_Power + ByteToStrHex(inStream[z]);
                            //формируем сообщения для вывода в ListView в 16-ричном виде
                        }
                        //int weight_geter = int.Parse(answer_Weight, System.Globalization.NumberStyles.AllowHexSpecifier);
                        //double REALY_WEIGHT_VARIABLE = weight_geter * 0.001;

                        int power_geter = int.Parse(answer_Power, System.Globalization.NumberStyles.AllowHexSpecifier);
                        double REALY_POWER_VARIABLE = power_geter * 0.01;


                        if (REALY_POWER_VARIABLE < 0)
                        {
                            REALY_POWER_VARIABLE = 0;
                        }
                       

                        if (Settings.kalibrovka == false)
                        {

                            double REALY_WEIGHT_VARIABLE = REALY_POWER_VARIABLE / 3.6;
                            scalesEntities2 bd = new scalesEntities2();
                            MeasurementData inf1 = new MeasurementData();
                            inf1.ScalesNumberID = device_list[q].device_name;
                            inf1.CurrentWeight = REALY_WEIGHT_VARIABLE;
                            inf1.CurrentProductivity = REALY_POWER_VARIABLE;
                            inf1.TimeOfMeasurement = DateTime.Now;
                            bd.MeasurementData.Add(inf1);
                            bd.SaveChanges();
                            bd.Dispose();
                            if (AllGraphRefresh != null)
                                AllGraphRefresh();
                            RefreshGraph();
                        }

                        if (device_list[q].device_name.ToString() == WeightInformation.CURRENT_DEVICE_COMBO)
                        {

                            scalesEntities2 bd = new scalesEntities2();
                            DataByShift dbs = new DataByShift();
                            DataByHours dbh = new DataByHours();
                            DataByDays dbd = new DataByDays();
                            DataByMonth dbm = new DataByMonth();

                            int ee = int.Parse(WeightInformation.CURRENT_DEVICE_COMBO);
                            dbs = bd.DataByShift.OrderByDescending(x => x.ID).Where(x => x.ScalesNumberID == ee).First();
                            dbh = bd.DataByHours.OrderByDescending(x => x.ID).Where(x => x.ScalesNumberID == ee).First();
                            dbd = bd.DataByDays.OrderByDescending(x => x.ID).Where(x => x.ScalesNumberID == ee).First();
                            dbm = bd.DataByMonth.OrderByDescending(x => x.ID).Where(x => x.ScalesNumberID == ee).First();

                            WEIGHT_STATIC_SHIFT = WEIGHT_STATIC_SHIFT + (REALY_POWER_VARIABLE / 3600);
                            double REALY_WEIGHT_VARIABLE = REALY_POWER_VARIABLE / 3.6;
                            if (DataLive != null)
                                DataLive(dbs.Weight.Value, REALY_POWER_VARIABLE, dbh.Weight.Value, dbd.Weight.Value, dbm.Weight.Value);
                            //dbs.Weight.Value
                           
                        }

                    }
                }
                catch (Exception e)
                {
                   
                }
            }
        }

        //public void ReconnectSocket(object s)
        //{       
        //    Device dev = (Device)s;
        //    device_list.Remove(dev);
        //    bool speakstart = false;
        //    if (dev.clientSocket.Connected)
        //    {
        //        device_list.Add(dev);
        //        NewConnectionCreate.Abort();
        //    }
        //    else
        //    {              
        //        for (int i = 0; i < 6; i++)
        //        {
        //            try
        //            {
        //                device_list.Remove(dev);
        //                dev.clientSocket.Connect(dev.device_ip, 9761);
        //                if (dev.clientSocket.Connected)
        //                {
        //                    device_list.Add(dev);
        //                    NewConnectionCreate.Abort();
        //                }
        //                speakstart = true;
                        
        //            }
        //            catch (Exception)
        //            {
        //                speakstart = true;
        //            }               
        //        }
        //        if (speakstart == true)
        //        {
        //            speek.Speak("Внимание! Было потеряно соединение с весАми номер" + dev.device_name + dev.AorK + "Устраните неполадки и перезагрузите приложение");
                    
        //        }
        //    }
        //}

        private byte StrHexToByte(string sHex)
        {
            try
            {
                byte ret = 0;
                //bNoError = true;

                string hxH = "";
                string hxL = "";
                if (sHex.Length == 2)
                {
                    hxH = sHex.Substring(0, 1);
                    hxL = sHex.Substring(1, 1);
                }
                else if (sHex.Length == 1)
                {
                    hxL = sHex.Substring(0, 1);
                }
                else
                {
                    //bNoError = false;
                    return 0;
                }

                if (hxH == "0") ret = 0;
                else if (hxH == "1") ret = 16;
                else if (hxH == "2") ret = 16 * 2;
                else if (hxH == "3") ret = 16 * 3;
                else if (hxH == "4") ret = 16 * 4;
                else if (hxH == "5") ret = 16 * 5;
                else if (hxH == "6") ret = 16 * 6;
                else if (hxH == "7") ret = 16 * 7;
                else if (hxH == "8") ret = 16 * 8;
                else if (hxH == "9") ret = 16 * 9;
                else if (hxH == "A" || hxH == "a") ret = 16 * 10;
                else if (hxH == "B" || hxH == "b") ret = 16 * 11;
                else if (hxH == "C" || hxH == "c") ret = 16 * 12;
                else if (hxH == "D" || hxH == "d") ret = 16 * 13;
                else if (hxH == "E" || hxH == "e") ret = 16 * 14;
                else if (hxH == "F" || hxH == "f") ret = 16 * 15;

                if (hxL == "0") ret += 0;
                else if (hxL == "1") ret += 1;
                else if (hxL == "2") ret += 2;
                else if (hxL == "3") ret += 3;
                else if (hxL == "4") ret += 4;
                else if (hxL == "5") ret += 5;
                else if (hxL == "6") ret += 6;
                else if (hxL == "7") ret += 7;
                else if (hxL == "8") ret += 8;
                else if (hxL == "9") ret += 9;
                else if (hxL == "A" || hxL == "a") ret += 10;
                else if (hxL == "B" || hxL == "b") ret += 11;
                else if (hxL == "C" || hxL == "c") ret += 12;
                else if (hxL == "D" || hxL == "d") ret += 13;
                else if (hxL == "E" || hxL == "e") ret += 14;
                else if (hxL == "F" || hxL == "f") ret += 15;
                else
                {
                    //bNoError = false;
                    return 0;
                }

                return ret;
            }
            catch (Exception ex)
            {
                //bNoError = false;
                return 0;
            }
        }

        private string ByteToStrHex(byte b)
        {
            try
            {
                int iTmpH = b / (byte)16;
                int iTmpL = b % (byte)16;
                string ret = "";

                if (iTmpH < 10)
                    ret = iTmpH.ToString();
                else
                {
                    if (iTmpH == 10) ret = "A";
                    if (iTmpH == 11) ret = "B";
                    if (iTmpH == 12) ret = "C";
                    if (iTmpH == 13) ret = "D";
                    if (iTmpH == 14) ret = "E";
                    if (iTmpH == 15) ret = "F";
                }

                if (iTmpL < 10)
                    ret += iTmpL.ToString();
                else
                {
                    if (iTmpL == 10) ret += "A";
                    if (iTmpL == 11) ret += "B";
                    if (iTmpL == 12) ret += "C";
                    if (iTmpL == 13) ret += "D";
                    if (iTmpL == 14) ret += "E";
                    if (iTmpL == 15) ret += "F";
                }

                return ret;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "";
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Report_Form_Print pr = new Report_Form_Print();
            pr.Show();
        }

        private void tableLayoutPanel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Login.Reload = true;
            Login lg = new Login();
            lg.ShowDialog();
            if (Login.CLOSE_ALL == true)
            {
                return;
            }

            if (Login.Role_ID == 1)
            {
                button5.Enabled = false;
            }
            if (Login.Role_ID == 2)
            {
                button5.Enabled = true;
            }
            label2.Text = USER;
        }

        // END FOR CONTROLLER
    }
}
