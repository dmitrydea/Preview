using Microsoft.Reporting.WinForms;
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
    public partial class Report_Form_Print : Form
    {
        int[] mas_Scales = null;
        int  IQ = 0;
        List<ScalesInformation> ds_info = null;
        public Report_Form_Print()
        {
            InitializeComponent();
            comboBox1.SelectionChangeCommitted += comboBox1_SelectionChangeCommitted;
            comboBox1.SelectedIndex = 0;
            //comboBox2.SelectedIndex = 0;
            comboBox2.Items.Clear();
            comboBox3.SelectedIndex = 0;
            
            scalesEntities2 bd = new scalesEntities2();
            IQueryable<ScalesInformation> query = bd.ScalesInformation;
            ds_info = query.ToList();
            mas_Scales = new int[ds_info.Count];
            foreach (var item in ds_info)
            {
                comboBox2.Items.Add(item.ID + " " + item.Scales_Number);
                mas_Scales[IQ] = item.ID;
                IQ++;
            }
            try
            {
                comboBox2.SelectedIndex = 0;
                comboBox2.Items.Add("Все весы");
            }
            catch (Exception e)
            {
                
               
            }
           

        }

        private void Report_Form_Print_Load(object sender, EventArgs e)
        {
            //scalesEntities2 bd = new scalesEntities2();
            //IQueryable<ScalesInformation> query = bd.ScalesInformation;
            
            //List<ScalesInformation> ds_info = query.ToList();
            //DataSet1 ds = new DataSet1();

            //foreach (var item in ds_info)
            //{
            //    ds.DataTable1.Rows.Add(item.ID, 5,  DateTime.Now);
            //}

            //ReportDataSource a = new ReportDataSource("DataSet1", ds.Tables[0]);
            //this.reportViewer1.LocalReport.DataSources.Clear();
            //this.reportViewer1.LocalReport.DataSources.Add(a);
            //reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter1", "2"));
            //reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter2", "2"));
            //reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter3", "2"));
            //reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter4", "2"));
            //this.reportViewer1.LocalReport.Refresh();
            //this.reportViewer1.Refresh();
            ////this.reportViewer2.RefreshReport();
            //this.reportViewer1.RefreshReport();
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    //За смену с детализацией по часам
                    label4.Visible = true;
                    dateTimePicker1.Visible = true;
                    dateTimePicker2.Visible = false;
                    label5.Visible = false;
                    label4.Text = "Выберите день";
                    comboBox3.Visible = true;
                    label6.Visible = true;
                    dateTimePicker1.CustomFormat = "dd.MM.yyyy";
                    break;
                case 1:
                    //За день с детализацией по часам
                    label4.Visible = true;
                    dateTimePicker1.Visible = true;
                    dateTimePicker2.Visible = false;
                    label5.Visible = false;
                    comboBox3.Visible = false;
                    label6.Visible = false;
                    label4.Text = "Выберите день";
                    dateTimePicker1.CustomFormat = "dd.MM.yyyy";
                    break;
                case 2:
                    //За месяц с детализацией по дням
                    label4.Visible = true;
                    dateTimePicker1.Visible = true;
                    dateTimePicker2.Visible = false;
                    label5.Visible = false;
                    comboBox3.Visible = false;
                    label6.Visible = false;
                    label4.Text = "Выберите месяц";
                    dateTimePicker1.CustomFormat = "MM.yyyy";
                    break;
                case 3:
                    //За год с детализацией по месяцам
                    dateTimePicker2.Visible = false;
                    label5.Visible = false;
                    comboBox3.Visible = false;
                    label6.Visible = false;
                    label4.Visible = false;
                    dateTimePicker1.Visible = false;
                    break;
                case 4:
                    //За выбранный период времени
                    label4.Visible = true;
                    label4.Text = "С";
                    dateTimePicker1.Visible = true;
                    dateTimePicker2.Visible = true;
                    label5.Visible = true;
                    label5.Text = "По";
                    comboBox3.Visible = false;
                    label6.Visible = false;
                    dateTimePicker1.CustomFormat = "dd.MM.yyyy";
                    dateTimePicker2.CustomFormat = "dd.MM.yyyy";
                    DateTime d1 = DateTime.Now;
                    DateTime d2 = d1.AddDays(-1);
                    dateTimePicker1.Text = Convert.ToString(d2);
                    break;
                default:
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                scalesEntities2 bd = new scalesEntities2();
                if (ds_info.Count < 1)
                {
                    return;
                }
                if (comboBox2.SelectedItem.ToString() == "Все весы")
                {
                    int NumbShift = 0;
                    IQueryable<DataByHours> query = null;
                    switch (comboBox3.SelectedIndex)
                    {
                        case 0: NumbShift = 1;
                            query = bd.DataByHours.Where(x => x.Date == dateTimePicker1.Value.Date && mas_Scales.Contains(x.ScalesNumberID.Value)
                    && x.ShiftNumber == NumbShift);
                            break;
                        case 1: NumbShift = 2;
                            
                            TimeSpan tSNE = TimeSpan.Parse("08:00");
                            TimeSpan tSNEtoday = TimeSpan.Parse("20:00");
                            DateTime NextDateShift = dateTimePicker1.Value.Date.AddDays(1);
                            query = bd.DataByHours.Where(x => ((x.Date.Value == dateTimePicker1.Value.Date && x.Time.Value > tSNEtoday) ||
                            (x.Date.Value == NextDateShift && x.Time.Value < tSNE)) 
                    && mas_Scales.Contains(x.ScalesNumberID.Value)
                   && x.ShiftNumber == NumbShift);
                            break;
                    }
                    //query = bd.DataByHours.Where(x => x.Date == dateTimePicker1.Value.Date && mas_Scales.Contains(x.ScalesNumberID.Value) 
                    //&& x.ShiftNumber == NumbShift);

                    List<DataByHours> ds_info = query.ToList();
                    DataSet1 ds = new DataSet1();
                    ds.DataTable1.Clear();

                    foreach (var item in ds_info)
                    {

                        ds.DataTable1.Rows.Add(item.ID, item.Weight, item.Time.Value.ToString(@"hh\:mm\:ss"));
                    }

                    ReportDataSource a = new ReportDataSource("DataSet1", ds.Tables[0]);
                    this.reportViewer1.LocalReport.DataSources.Clear();
                    this.reportViewer1.LocalReport.DataSources.Add(a);
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter1", comboBox2.SelectedItem.ToString()));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter2", "-"));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter3", dateTimePicker1.Value.ToString()));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter4", Form1.USER));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter5", "Время"));
                    
                    this.reportViewer1.LocalReport.Refresh();
                    this.reportViewer1.Refresh();
                    //this.reportViewer2.RefreshReport();
                    this.reportViewer1.RefreshReport();
                    bd.Dispose();
                }
                else
                {
                    int Predel = int.Parse(comboBox2.SelectedItem.ToString().Split(' ')[0]);
                    int NumbShift = 0;
                    IQueryable<DataByHours> query = null;
                    switch (comboBox3.SelectedIndex)
                    {
                        case 0:
                            NumbShift = 1;
                            query = bd.DataByHours.Where(x => x.Date == dateTimePicker1.Value.Date && x.ScalesNumberID.Value == Predel
                    && x.ShiftNumber == NumbShift);
                            break;
                        case 1:
                            NumbShift = 2;
                            TimeSpan tSNE = TimeSpan.Parse("08:00");
                            TimeSpan tSNEtoday = TimeSpan.Parse("20:00");
                            DateTime NextDateShift = dateTimePicker1.Value.Date.AddDays(1);
                            query = bd.DataByHours.Where(x => ((x.Date.Value == dateTimePicker1.Value.Date && x.Time.Value > tSNEtoday) ||
                            (x.Date.Value == NextDateShift && x.Time.Value < tSNE)) && x.ScalesNumberID.Value == Predel
                   && x.ShiftNumber.Value == NumbShift);
                            break;
                    }
                    //query = bd.DataByHours.Where(x => x.Date == dateTimePicker1.Value.Date && mas_Scales.Contains(x.ScalesNumberID.Value));

                    List<DataByHours> ds_info = query.ToList();
                    DataSet1 ds = new DataSet1();
                    ds.DataTable1.Clear();

                    foreach (var item in ds_info)
                    {

                        ds.DataTable1.Rows.Add(item.ID, item.Weight, item.Time.Value.ToString(@"hh\:mm\:ss"));
                    }

                    ReportDataSource a = new ReportDataSource("DataSet1", ds.Tables[0]);
                    this.reportViewer1.LocalReport.DataSources.Clear();
                    this.reportViewer1.LocalReport.DataSources.Add(a);
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter1", comboBox2.SelectedItem.ToString()));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter2", comboBox3.SelectedItem.ToString()));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter3", dateTimePicker1.Value.ToString()));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter4", Form1.USER));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter5", "Время"));
                    this.reportViewer1.LocalReport.Refresh();
                    this.reportViewer1.Refresh();
                    //this.reportViewer2.RefreshReport();
                    this.reportViewer1.RefreshReport();
                    bd.Dispose();
                }
               

            }
            if (comboBox1.SelectedIndex == 1)
            {
                if (ds_info.Count < 1)
                {
                    return;
                }
                scalesEntities2 bd = new scalesEntities2();
                if (comboBox2.SelectedItem.ToString() == "Все весы")
                {
                    //int Predel = int.Parse(comboBox2.SelectedItem.ToString().Split(' ')[0]);
                    IQueryable<DataByHours> query = bd.DataByHours.Where(x => x.Date == dateTimePicker1.Value.Date && mas_Scales.Contains(x.ScalesNumberID.Value));

                    List<DataByHours> ds_info = query.ToList();
                    DataSet1 ds = new DataSet1();
                    ds.DataTable1.Clear();

                    foreach (var item in ds_info)
                    {

                        ds.DataTable1.Rows.Add(item.ID, item.Weight, item.Time.Value.ToString(@"hh\:mm\:ss"));
                    }

                    ReportDataSource a = new ReportDataSource("DataSet1", ds.Tables[0]);
                    this.reportViewer1.LocalReport.DataSources.Clear();
                    this.reportViewer1.LocalReport.DataSources.Add(a);
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter1", comboBox2.SelectedItem.ToString()));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter2", "-"));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter3", dateTimePicker1.Value.ToString()));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter4", Form1.USER));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter5", "Время"));
                    this.reportViewer1.LocalReport.Refresh();
                    this.reportViewer1.Refresh();
                    //this.reportViewer2.RefreshReport();
                    this.reportViewer1.RefreshReport();
                    bd.Dispose();
                }
                else
                {
                    int Predel = int.Parse(comboBox2.SelectedItem.ToString().Split(' ')[0]);
                    IQueryable<DataByHours> query = bd.DataByHours.Where(x => x.Date == dateTimePicker1.Value.Date &&x.ScalesNumberID == Predel);

                    List<DataByHours> ds_info = query.ToList();
                    DataSet1 ds = new DataSet1();
                    ds.DataTable1.Clear();

                    foreach (var item in ds_info)
                    {

                        ds.DataTable1.Rows.Add(item.ID, item.Weight, item.Time.Value.ToString(@"hh\:mm\:ss"));
                    }

                    ReportDataSource a = new ReportDataSource("DataSet1", ds.Tables[0]);
                    this.reportViewer1.LocalReport.DataSources.Clear();
                    this.reportViewer1.LocalReport.DataSources.Add(a);
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter1", comboBox2.SelectedItem.ToString()));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter2", "-"));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter3", dateTimePicker1.Value.ToString()));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter4", Form1.USER));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter5", "Время"));
                    this.reportViewer1.LocalReport.Refresh();
                    this.reportViewer1.Refresh();
                    //this.reportViewer2.RefreshReport();
                    this.reportViewer1.RefreshReport();
                    bd.Dispose();
                }
                    
                

            }
            if (comboBox1.SelectedIndex == 2)
            {
                if (ds_info.Count < 1)
                {
                    return;
                }
                scalesEntities2 bd = new scalesEntities2();
                if (comboBox2.SelectedItem.ToString() == "Все весы")
                {
                    IQueryable<DataByDays> query = bd.DataByDays.Where(x => x.Date.Value.Month == dateTimePicker1.Value.Date.Month && mas_Scales.Contains(x.ScalesNumberID.Value));

                    List<DataByDays> ds_info = query.ToList();
                    DataSet1 ds = new DataSet1();
                    ds.DataTable2.Clear();

                    foreach (var item in ds_info)
                    {
                        ds.DataTable2.Rows.Add(item.ID, item.Weight, item.Date.Value.ToShortDateString());
                    }

                    ReportDataSource a = new ReportDataSource("DataSet1", ds.Tables[1]);
                    this.reportViewer1.LocalReport.DataSources.Clear();
                    this.reportViewer1.LocalReport.DataSources.Add(a);
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter1", comboBox2.SelectedItem.ToString()));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter2", "-"));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter3", dateTimePicker1.Value.ToString()));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter4", Form1.USER));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter5", "Дата"));
                    this.reportViewer1.LocalReport.Refresh();
                    this.reportViewer1.Refresh();
                    //this.reportViewer2.RefreshReport();
                    this.reportViewer1.RefreshReport();
                }
                else
                {
                    int Predel = int.Parse(comboBox2.SelectedItem.ToString().Split(' ')[0]);
                    IQueryable<DataByDays> query = bd.DataByDays.Where(x => x.Date.Value.Month == dateTimePicker1.Value.Date.Month && x.ScalesNumberID == Predel);

                    List<DataByDays> ds_info = query.ToList();
                    DataSet1 ds = new DataSet1();
                    ds.DataTable2.Clear();

                    foreach (var item in ds_info)
                    {
                        ds.DataTable2.Rows.Add(item.ID, item.Weight, item.Date.Value.ToShortDateString());
                    }

                    ReportDataSource a = new ReportDataSource("DataSet1", ds.Tables[1]);
                    this.reportViewer1.LocalReport.DataSources.Clear();
                    this.reportViewer1.LocalReport.DataSources.Add(a);
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter1", comboBox2.SelectedItem.ToString()));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter2", "-"));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter3", dateTimePicker1.Value.ToString()));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter4", Form1.USER));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter5", "Дата"));
                    this.reportViewer1.LocalReport.Refresh();
                    this.reportViewer1.Refresh();
                    //this.reportViewer2.RefreshReport();
                    this.reportViewer1.RefreshReport();
                }
            }
            if (comboBox1.SelectedIndex == 3)
            {
                if (ds_info.Count < 1)
                {
                    return;
                }
                scalesEntities2 bd = new scalesEntities2();
                if (comboBox2.SelectedItem.ToString() == "Все весы")
                {
                    IQueryable<DataByMonth> query = bd.DataByMonth.Where(x => mas_Scales.Contains(x.ScalesNumberID.Value));

                    List<DataByMonth> ds_info = query.ToList();
                    DataSet1 ds = new DataSet1();
                    ds.DataTable3.Clear();

                    foreach (var item in ds_info)
                    {
                        ds.DataTable3.Rows.Add(item.ID, item.Weight, item.NumberMonth.Value.ToString());
                    }

                    ReportDataSource a = new ReportDataSource("DataSet1", ds.Tables[2]);
                    this.reportViewer1.LocalReport.DataSources.Clear();
                    this.reportViewer1.LocalReport.DataSources.Add(a);
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter1", comboBox2.SelectedItem.ToString()));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter2", "-"));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter3", dateTimePicker1.Value.ToString()));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter4", Form1.USER));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter5", "Месяц"));
                    this.reportViewer1.LocalReport.Refresh();
                    this.reportViewer1.Refresh();
                    //this.reportViewer2.RefreshReport();
                    this.reportViewer1.RefreshReport();
                }
                else
                {
                    int Predel = int.Parse(comboBox2.SelectedItem.ToString().Split(' ')[0]);
                    IQueryable<DataByMonth> query = bd.DataByMonth.Where(x => x.ScalesNumberID == Predel);

                    List<DataByMonth> ds_info = query.ToList();
                    DataSet1 ds = new DataSet1();
                    ds.DataTable3.Clear();

                    foreach (var item in ds_info)
                    {
                        ds.DataTable3.Rows.Add(item.ID, item.Weight, item.NumberMonth.Value.ToString());
                    }

                    ReportDataSource a = new ReportDataSource("DataSet1", ds.Tables[2]);
                    this.reportViewer1.LocalReport.DataSources.Clear();
                    this.reportViewer1.LocalReport.DataSources.Add(a);
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter1", comboBox2.SelectedItem.ToString()));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter2", "-"));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter3", dateTimePicker1.Value.ToString()));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter4", Form1.USER));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter5", "Месяц"));
                    this.reportViewer1.LocalReport.Refresh();
                    this.reportViewer1.Refresh();
                    //this.reportViewer2.RefreshReport();
                    this.reportViewer1.RefreshReport();
                }
            }
            if (comboBox1.SelectedIndex == 4)
            {
                if (ds_info.Count < 1)
                {
                    return;
                }
                DateTime st = dateTimePicker1.Value;
                DateTime end = dateTimePicker2.Value;
                //MessageBox.Show(dateTimePicker2.Value.Date.ToString(), "");
                //DateTime dtS = new DateTime(st.Year,st.Month,st.Day,0,0,0);
                //DateTime dtE = new DateTime(end.Year, end.Month, end.Day, end.Hour, 59, 59);
            

                scalesEntities2 bd = new scalesEntities2();

                if (comboBox2.SelectedItem.ToString() == "Все весы")
                {
                    IQueryable<DataByHours> query = bd.DataByHours.Where(x => (mas_Scales.Contains(x.ScalesNumberID.Value)) && (x.Date >= st.Date && x.Date <= end.Date));

                    List<DataByHours> ds_info = query.ToList();
                    DataSet1 ds = new DataSet1();
                    ds.DataTable3.Clear();

                    foreach (var item in ds_info)
                    {
                        ds.DataTable3.Rows.Add(item.ID, item.Weight, item.Date.Value.ToString("dd.MM.yy") + " " + item.Time.Value.ToString(@"hh"));
                    }

                    ReportDataSource a = new ReportDataSource("DataSet1", ds.Tables[2]);
                    this.reportViewer1.LocalReport.DataSources.Clear();
                    this.reportViewer1.LocalReport.DataSources.Add(a);
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter1", comboBox2.SelectedItem.ToString()));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter2", "-"));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter3", dateTimePicker1.Value.ToString()));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter4", Form1.USER));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter5", "День/Час"));
                    this.reportViewer1.LocalReport.Refresh();
                    this.reportViewer1.Refresh();
                    //this.reportViewer2.RefreshReport();
                    this.reportViewer1.RefreshReport();
                }
                else
                {
                    int Predel = int.Parse(comboBox2.SelectedItem.ToString().Split(' ')[0]);

                    IQueryable<DataByHours> query = bd.DataByHours.Where(x => (x.ScalesNumberID == Predel) && (x.Date >= st.Date && x.Date <= end.Date));

                    List<DataByHours> ds_info = query.ToList();
                    DataSet1 ds = new DataSet1();
                    ds.DataTable3.Clear();

                    foreach (var item in ds_info)
                    {
                        ds.DataTable3.Rows.Add(item.ID, item.Weight, item.Date.Value.ToString("dd.MM.yy") + " " + item.Time.Value.ToString(@"hh"));
                    }

                    ReportDataSource a = new ReportDataSource("DataSet1", ds.Tables[2]);
                    this.reportViewer1.LocalReport.DataSources.Clear();
                    this.reportViewer1.LocalReport.DataSources.Add(a);
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter1", comboBox2.SelectedItem.ToString()));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter2", "-"));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter3", dateTimePicker1.Value.ToString()));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter4", Form1.USER));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("ReportParameter5", "День/Час"));
                    this.reportViewer1.LocalReport.Refresh();
                    this.reportViewer1.Refresh();
                    //this.reportViewer2.RefreshReport();
                    this.reportViewer1.RefreshReport();
                }
            }
        }
    }
}
