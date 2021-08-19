using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControllerAruduino
{
    public partial class Form1 : Form
    {
        private static readonly HttpClient client = new HttpClient();
        public Form1()
        {

            InitializeComponent();
            string[] BaudRate = { "1200", "2400", "4800", "9600", "19200", "38400", "57600", "115200" };
            comboBox1.Items.AddRange(BaudRate);
            comboBox1.SelectedIndex = 3;
            progressBar1.Value = 100;
            timer1.Start();
         
          
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
           if(!serialPort1.IsOpen&&!serialPort2.IsOpen)
            {
                serialPort1.PortName = cbCOM1.Text;
                serialPort1.BaudRate = int.Parse(comboBox1.Text);
                serialPort1.Open();
                serialPort2.PortName = cbCOM2.Text;
                serialPort2.BaudRate = int.Parse(comboBox1.Text);
                serialPort2.Open();
         

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            HandSignal frm = new HandSignal();
            frm.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cbCOM1.DataSource = SerialPort.GetPortNames();
            cbCOM2.DataSource = SerialPort.GetPortNames();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            serialPort2.Close();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {if (!serialPort1.IsOpen)
             {
                label6.Text = "COM1 close";
                label6.ForeColor = Color.Red;
             }
              else
            {
                label6.Text = "COM1 open";
                label6.ForeColor = Color.Blue;

            }    
          if (!serialPort2.IsOpen)
            {
                label7.Text = "COM2 close";
                label7.ForeColor = Color.Red;
            }
            else
            {
                label7.Text = "COM2 open";
                label7.ForeColor = Color.Blue;

            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            ConfigForm frm = new ConfigForm();
            frm.Show();
        }
        Dictionary<int, Tuple<int, string,bool>> DicPhantu = new Dictionary<int, Tuple<int, string,bool>>();
        delegate void UIDelegate();
        delegate void UIDelegate2();
        DataSet myDataSet;
        private void button1_Click(object sender, EventArgs e)
        {
          
             if (EnoughPermission()) autoData();
        }
        private bool EnoughPermission()
        {
            SqlClientPermission perm = new SqlClientPermission(System.Security.Permissions.PermissionState.Unrestricted);
            try
            {
                perm.Demand();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        SqlConnection connection;
        SqlCommand command;
        public void autoData()
        {
            string ssql;
            command = null;
            string connstr = ControllerAruduino.Properties.Settings.Default.connect;
           
                    ssql = "select TRANG_THAI from dbo.M_TTHAI_PTDIEN ";
               
            SqlDependency.Stop(connstr);
            SqlDependency.Start(connstr);
            if (connection == null)
                connection = new SqlConnection(connstr);
            if (command == null)
                command = new SqlCommand(ssql, connection);
            if (myDataSet == null)
                myDataSet = new DataSet();
            GetAdvtData();
        }
        private void GetAdvtData()
        {
            myDataSet.Clear();
            command.Notification = null;
            SqlDependency dependency = new SqlDependency(command);
            dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                adapter.Fill(myDataSet, "Advt");
            }
        }
      
        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            try
            {
                UIDelegate uidel = new UIDelegate(Xuly);
                this.Invoke(uidel, null);
                SqlDependency dependency = (SqlDependency)sender;
                dependency.OnChange -= dependency_OnChange;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void Xuly()
        {
            DicPhantu.Clear();
            EMSdatacontextDataContext dataContext = new EMSdatacontextDataContext(ControllerAruduino.Properties.Settings.Default.connect);
            List<Arduino2> arduino2s = dataContext.Arduino2s.ToList();
            foreach (var item in arduino2s)
            {
                textBox1.Text += "\n" + item.Id_ptdien;
                M_VITRI_V Phantu = dataContext.M_VITRI_Vs.Where(x => x.ID_PTDIEN == item.Id_ptdien).FirstOrDefault();
                if (Phantu.LEFT_TRANGTHAI == false && Phantu.RIGHT_TRANGTHAI == false)
                {
                    textBox1.Text += "\n Phần tử bị mất điện";
                    DicPhantu.Add(item.Id_ptdien, new Tuple<int, string, bool>(3, item.Pin, true));
                }
                else
                {
                    M_VITRI_V2 Phantu2 = dataContext.M_VITRI_V2s.Where(x => x.ID_PTDIEN == item.Id_ptdien).FirstOrDefault();
                    textBox1.Text += "" + Phantu2.TRANG_THAI + Phantu2.TEN_PTDIEN;
                    int tt = 0;
                    if (Phantu2.TRANG_THAI == 1) tt = 1; else tt = 0;
                    DicPhantu.Add(item.Id_ptdien, new Tuple<int, string, bool>(tt, item.Pin, true));
                }

            }
             this.Invoke(new UIDelegate2(InitSaBan));
            GetAdvtData();
        }
        private void InitSaBan()
        {
            foreach (KeyValuePair<int, Tuple<int, string, bool>> item in DicPhantu)
            {
                ///Console.WriteLine(item.Value.Item2);
               
                  //  DicPhantu[item.Key].Item3 = false;
                    if(serialPort1.IsOpen&&serialPort2.IsOpen)
                    {
                      //  MessageBox.Show(item.Key.ToString());
                        foreach (var item2 in item.Value.Item2.Split('-'))
                    {
                        ConfigForm frm = new ConfigForm();
                        frm.Show();
                        Task.Delay(1);
                        frm.Close();
                        if (item2.Split('|')[2]=="R")
                            {
                                 if (item2.Split('|')[3] == "0")
                                {
                               
                               

                                    if (item.Value.Item1 == 3|| item.Value.Item1==0)
                                    {
                                       
                                        if (item2.Split('|')[0] == "1")
                                        {
                                            serialPort1.Write("s" + item2.Split('|')[1] + "ph");
                                        }
                                        else
                                        {
                                            serialPort2.Write("s" + item2.Split('|')[1] + "ph");
                                        }
                                    }
                                    else
                                    {
                                            EMSdatacontextDataContext ems = new EMSdatacontextDataContext();
                                            M_VITRI_V2 a = ems.M_VITRI_V2s.Where(x => x.ID_PTDIEN == item.Key).FirstOrDefault();
                                            if(a.LOAI_PTDIEN=="TT")
                                            {
                                                if (item.Value.Item1 == 1)
                                                {
                                                    if (item2.Split('|')[0] == "1")
                                                    {
                                                        serialPort1.Write("s" + item2.Split('|')[1] + "ph");
                                                    }
                                                    else
                                                    {
                                                        serialPort2.Write("s" + item2.Split('|')[1] + "ph");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                 if (item2.Split('|')[0] == "1")
                                        {
                                            serialPort1.Write("s" + item2.Split('|')[1] + "pL");
                                        }
                                        else
                                        {
                                            serialPort2.Write("s" + item2.Split('|')[1] + "pL");
                                        }
                                            }
                                       
                                    }    
                                }
                                else
                                {
                                    if (item.Value.Item1 == 3 || item.Value.Item1 == 0)
                                    {
                                        if (item2.Split('|')[0] == "1")
                                        {
                                            serialPort1.Write("s" + item2.Split('|')[1] + "pL");
                                        }
                                        else
                                        {
                                            serialPort2.Write("s" + item2.Split('|')[1] + "pL");
                                        }
                                    }
                                    else
                                    {
                                        if (item2.Split('|')[0] == "1")
                                        {
                                            serialPort1.Write("s" + item2.Split('|')[1] + "ph");
                                        }
                                        else
                                        {
                                            serialPort2.Write("s" + item2.Split('|')[1] + "ph");
                                        }
                                    }
                                }    
                            }
                            else
                            {
                               if(item.Value.Item1==3)
                            {
                               // Console.WriteLine(item2.Split('|')[1]);

                                if (item2.Split('|')[0] == "1")
                                    {
                                        serialPort1.Write("s" + item2.Split('|')[1] + "pL");
                                    }
                                    else
                                    {
                                        serialPort2.Write("s" + item2.Split('|')[1] + "pL");
                                    }
                                }
                                else
                                {
                                Console.WriteLine(item2);

                                if (item.Value.Item1!=int.Parse(item2.Split('|')[3]))
                                    {
                                         if (item2.Split('|')[0] == "1")
                                        {
                                            serialPort1.WriteLine("s" + item2.Split('|')[1] + "ph");
                                        }
                                        else
                                        {
                                            serialPort2.WriteLine("s" + item2.Split('|')[1] + "ph");
                                        }
                                    }   
                                    else
                                    {
                                        if (item2.Split('|')[0] == "1")
                                        {
                                            serialPort1.WriteLine("s" + item2.Split('|')[1] + "pL");
                                      
                                        }
                                        else
                                        {
                                            serialPort2.WriteLine("s" + item2.Split('|')[1] + "pL");
                                        }
                                    }    
                                }    
                            }    
                        }
                    }    
                }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Console.WriteLine(serialPort1.ReadExisting());
            Console.WriteLine(serialPort2.ReadExisting());



        }
    }
}