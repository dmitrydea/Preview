using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WCSC
{
    public partial class Login : Form
    {
        public static bool Reload = false;
        public static bool CLOSE_ALL = false;
        public static int Role_ID;
        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {

            }
            else
            {
                string Log = textBox1.Text;
                string Pass = textBox2.Text;

                scalesEntities2 bd = new scalesEntities2();

                
                try
                {
                    Personal pers = bd.Personal.Where(x => x.Name == Log.ToLower() && x.Password == Pass.ToLower()).First();
                    if (pers.Role == 1)
                    {
                        Role_ID = 1;
                        Form1.USER = pers.Name;
                    }
                    if (pers.Role == 2)
                    {
                        Role_ID = 2;
                        Form1.USER = pers.Name;
                    }
                    this.Close();
                }
                catch (Exception)
                {
                    textBox2.Text = "";
                    MessageBox.Show("Пользователь не найден!", "Внимание!",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
                bd.Dispose();
  
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Reload == true)
            {
                this.Close();
            }
            else
            {
                CLOSE_ALL = true;
                Application.Exit();
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
