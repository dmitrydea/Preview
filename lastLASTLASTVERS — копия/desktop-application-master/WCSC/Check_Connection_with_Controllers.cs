using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WCSC
{
    public partial class Check_Connection_with_Controllers : Form
    {
        DateTime dt = DateTime.Now;
        //System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        //поток чтения-записи
        //NetworkStream serverStream = default(NetworkStream);

        public static List<Device> device_list = new List<Device>();
        public static scalesEntities2 bd = new scalesEntities2();
        Thread cConnect;
       

        public Check_Connection_with_Controllers()
        {
            InitializeComponent();
            this.Shown += Check_Connection_with_Controllers_Shown;
            
        }

        private void Check_Connection_with_Controllers_Shown(object sender, EventArgs e)
        {
            cConnect = new Thread(Chek_Connection);
            cConnect.Start();
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cConnect.Abort();
            this.Close();
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

        private void Chek_Connection()
        {
            
            List<ScalesInformation> test = null;
            try
            {
                IQueryable<ScalesInformation> query = bd.ScalesInformation;
                test = query.ToList();
                this.Invoke((MethodInvoker)delegate
                {
                    bd_error.Visible = false;
                    bd_ok.Visible = true;
                    bd_load.Visible = false;
                    button1.Enabled = false;
                });
            }
            catch (Exception)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    bd_error.Visible = true;
                    bd_ok.Visible = false;
                    bd_load.Visible = false;
                    button1.Enabled = false;
                });
            }

            if (test != null)
            {
                bool connection_controll = false;
                string device_error_connect = "";
                device_error_connect += "<";
                
                foreach (var item in test)
                {
                    bool NoDevice = false;
                    TcpClient clientSocket = new TcpClient();
                    NetworkStream serverStream = default(NetworkStream);
                    try
                    {

                        foreach (var dev in device_list)
                        {
                            if (item.IPaddress == dev.device_ip)
                            {
                                clientSocket = dev.clientSocket;
                                //serverStream = dev.clientSocket.GetStream();
                            }
                            else
                            {
                                NoDevice = true;
                            }
                        }
                        if (NoDevice == true || device_list.Count<1)
                        {
                            clientSocket.Connect(item.IPaddress, 9761);
                            
                        }
                        serverStream = clientSocket.GetStream();
                        clientSocket.ReceiveBufferSize = 8192;
                            byte NumbDevice = byte.Parse(item.ModbusID);
                            byte[] otvet = new byte[2];
                            GetCRC16(new byte[] { NumbDevice, 3, 0, 41, 0, 1 }, ref otvet);
                            string CRC_L = ByteToStrHex(otvet[0]);
                            string CRC_H = ByteToStrHex(otvet[1]);


                            String[] message = new String[] { item.ModbusID, "03", "00", "29", "00", "01", CRC_L, CRC_H };
                            Byte[] mes = new Byte[128];         //переменная, которая будет содержать данные для отправки
                            int i = 0;                      // счетчик

                            for (i = 0; i < 8; i++)
                            {

                                mes[i] = StrHexToByte(message[i]);

                            }

                            serverStream.Write(mes, 0, 8);
                            serverStream.Flush();

                            serverStream = clientSocket.GetStream();            //получаем поток
                            int buffSize = 0;
                            int bytesRead = 0;
                            byte[] inStream = new byte[10025];                  // инициализируем массив для приема данных
                            buffSize = clientSocket.ReceiveBufferSize;          //получаем размер буфера
                            serverStream.ReadTimeout = 1000;
                            bytesRead = serverStream.Read(inStream, 0, buffSize);//считываем данные из потока

                            if (bytesRead > 0)
                            {
                                Device dev = new Device(clientSocket, serverStream, item.IPaddress, item.ModbusID, item.ID, item.Scales_Number);
                                device_list.Add(dev);
                            }
                        
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message,"");
                        connection_controll = true;
                        device_error_connect += " №" + item.ID + " "; 
                    }
                }
                if (connection_controll == true)
                {
                    device_error_connect += ">";
                    this.Invoke((MethodInvoker)delegate
                    {
                        fail_connection.Visible = true;
                        good_conection.Visible = false;
                        preloader1.Visible = false;
                        button1.Enabled = true;
                        label3.Text = device_error_connect;
                    });
                }
                else
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        fail_connection.Visible = false;
                        good_conection.Visible = true;
                        preloader1.Visible = false;
                        button1.Enabled = true;
                    });
                }
            }
            else
            {
                return;
            }        
        }

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
            catch (Exception)
            {
                //bNoError = false;
                return 0;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Check_Connection_with_Controllers_Load_1(object sender, EventArgs e)
        {

            
      
        }
    }
}
