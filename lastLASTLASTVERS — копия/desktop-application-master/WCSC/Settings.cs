using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WCSC
{
    public partial class Settings : Form
    {
        public static bool kalibrovka = false;
        TreeNode delUser = null;
        TreeNode delDevice = null;
        string patternIP = "^([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\.([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\.([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\.([01]?\\d\\d?|2[0-4]\\d|25[0-5])$";
        string patternMODBUS = "^[0-9]{2}$";
        public Settings()
        {
            InitializeComponent();
            TreeUsers(3);
            DeviceInBD();
            if (kalibrovka == true)
            {
                radioButton1.Checked = true;
                radioButton2.Checked = false;
            }
            else
            {
                radioButton2.Checked = true;
                radioButton1.Checked = false;
            }
        }

        public void DeviceInBD()
        {
            treeView1.Nodes.Clear();
            scalesEntities2 bd1 = new scalesEntities2();
            List<string> ListDevice = bd1.ScalesInformation.Select(x => x.IPaddress).Distinct().ToList();
            List<ScalesInformation> ListDeviceInfo = bd1.ScalesInformation.ToList();
            foreach (var item in ListDevice)
            {
                TreeNode a = new TreeNode(item);
                treeView1.Nodes.Add(a);
                foreach (var item2 in ListDeviceInfo)
                {
                    if (item == item2.IPaddress)
                    {
                        treeView1.Nodes[a.Index].Nodes.Add(new TreeNode("ModBusID: "+item2.ModbusID + " Номер весов: " + item2.ID + " - " + item2.Scales_Number));
                    }
                }
            }
            
            
        }

        public void TreeUsers(int roleID)
        {
            treeView2.Nodes.Clear();
            treeView2.Nodes.Add(new TreeNode("Администраторы"));
            treeView2.Nodes.Add(new TreeNode("Весовщики"));
            scalesEntities2 bd1 = new scalesEntities2();
            List<Personal> ListPersonel = bd1.Personal.ToList();
            foreach (var item in ListPersonel)
            {
                if (item.Role == 2)
                {
                    treeView2.Nodes[0].Nodes.Add(new TreeNode(item.Name));
                }
                else
                {
                    treeView2.Nodes[1].Nodes.Add(new TreeNode(item.Name));
                }
            }
            bd1.Dispose();
            switch (roleID)
            {
                case 1: treeView2.Nodes[1].Expand(); break;
                case 2: treeView2.Nodes[0].Expand(); break;
                case 3: break;
            }

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                kalibrovka = true;
                pictureBoxON.Visible = true;
                pictureBoxOFF.Visible = false;
                //MessageBox.Show("Режим КАЛИБРОВКИ - ВКЛЮЧЕН", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }         
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                kalibrovka = false;
                pictureBoxON.Visible = false;
                pictureBoxOFF.Visible = true;
                //MessageBox.Show("Режим КАЛИБРОВКИ - ВЫКЛЮЧЕН", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int ListPersonelCount;
            scalesEntities2 bd = new scalesEntities2();
            try
            {
                ListPersonelCount = bd.Personal.Where(p => p.Name == login.Text).Count();
            }
            catch (Exception)
            {
                MessageBox.Show("Не все поля заполнены!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            
            if (ListPersonelCount > 0)
            {
                MessageBox.Show("Такой пользователь уже существует", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                bool chek = false;
                if (login.Text == "")
                {
                    chek = true;
                }
                if (passwred.Text == "")
                {
                    chek = true;
                }
                if (roleCombo.SelectedIndex < 1)
                {
                    chek = true;
                }

                if (chek == true)
                {
                    MessageBox.Show("Не все поля заполнены!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    Personal pNew = new Personal();
                    pNew.Name = login.Text.ToLower();
                    pNew.Password = passwred.Text.ToLower();
                    pNew.Role = roleCombo.SelectedIndex;
                    int roleID = roleCombo.SelectedIndex;
                    bd.Personal.Add(pNew);
                    bd.SaveChanges();
                    bd.Dispose();
                    MessageBox.Show("Пользователь добавлен!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    login.Text = "";
                    passwred.Text = "";
                    roleCombo.SelectedIndex = 0;
                    TreeUsers(roleID);
                }
                
            }
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

            
        }

        private void удалитьПользователяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                scalesEntities2 bd = new scalesEntities2();
                var x = bd.Personal.Where(p => p.Name == delUser.Text).First();
                bd.Personal.Remove(x);
                bd.SaveChanges();
                bd.Dispose();
                treeView2.Nodes.Remove(delUser);
                MessageBox.Show("Пользователь удален!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception)
            {

            }
        }

        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
            delUser = e.Node;
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Match m = Regex.Match(textBox1.Text, patternIP);//проверяем правильно ли введен ІР-адрес в текстовое поле
            if (m.Success)
            {
                Match m2 = Regex.Match(textBox2.Text, patternMODBUS);//проверяем правильно ли введен ІР-адрес в текстовое поле
                if (m2.Success)
                {
                    scalesEntities2 bd = new scalesEntities2();
                    var CountCopy = bd.ScalesInformation.Where(p => p.IPaddress == textBox1.Text && p.ModbusID == textBox2.Text).Count();
                    if (CountCopy > 0)
                    {
                        MessageBox.Show("Такое устройство уже существует!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {                      
                        ScalesInformation sc = new ScalesInformation();
                        sc.IPaddress = textBox1.Text;
                        sc.ModbusID = textBox2.Text;
                        if (comboBox1.SelectedIndex == 0)
                        {
                            sc.Scales_Number = "A";
                        }
                        if (comboBox1.SelectedIndex == 1)
                        {
                            sc.Scales_Number = "K";
                        }
                        bd.ScalesInformation.Add(sc);
                        bd.SaveChanges();
                        bd.Dispose();
                        textBox1.Text = "";
                        textBox2.Text = "";
                        DeviceInBD();
                        MessageBox.Show("Устройство добавлено!\nДля того что бы устройство начало функционировать НЕОБХОДИМО перезапустить программу!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
                else
                {
                    MessageBox.Show("ModBus адрес введен не верно!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("IP адрес введен не верно!","Внимание!",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void удалитьУстройствоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                scalesEntities2 bd = new scalesEntities2();
                
                if (delDevice.Level == 0)
                {
                    IQueryable<ScalesInformation> x = bd.ScalesInformation.Where(p => p.IPaddress == delDevice.Text);
                    foreach (var item in x)
                    {
                        bd.ScalesInformation.Remove(item);
                    }
                    bd.SaveChanges();
                    bd.Dispose();
                    treeView1.Nodes.Remove(delDevice);
                    DeviceInBD();
                    MessageBox.Show("Группа устройств удалена!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {                
                    string Eq = delDevice.Text.Split(' ')[1];
                    var x = bd.ScalesInformation.Where(p => p.ModbusID == Eq && p.IPaddress == delDevice.Parent.Text).First();
                    bd.ScalesInformation.Remove(x);
                    bd.SaveChanges();
                    bd.Dispose();
                    treeView1.Nodes.Remove(delDevice);
                    DeviceInBD();
                    MessageBox.Show("Устройство удалено!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
  
            }
            catch (Exception)
            {
                MessageBox.Show("Выберите устройство!\nДля этого нажмите на узел ЛК мышки.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            delDevice = e.Node;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
