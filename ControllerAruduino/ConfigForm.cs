using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControllerAruduino
{
    public partial class ConfigForm : Form
    {
        private string con;
        private SqlConnection conSQL;
        private bool con1 = false;
        public ConfigForm()
        {
            InitializeComponent();
            try
            {
                string s = ControllerAruduino.Properties.Settings.Default.connect;
                string[] s1 = s.Split(';');
                for (int i = 0; i < s1.Length; i++)
                {
                    string[] s2 = s1[i].Split('=');
                    if (i == 0)
                    {
                        txtServer.Text = s2[1];
                    }
                    if (i == 1)
                    {
                        txtDB.Text = s2[1];
                    }
                    if (i == 2)
                    {
                        txtUser.Text = s2[1];
                    }
                    if (i == 3)
                    {
                        txtPass.Text = s2[1];
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Có lỗi xảy ra");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            checkConnection(txtServer.Text, txtDB.Text, txtUser.Text, txtPass.Text);

        }
        private void checkConnection(string server, string data, string user, string pass)
        {
            con = "Data Source=" + server + ";Initial Catalog=" + data + ";User ID=" + user + ";Password=" + pass + ";";
            conSQL = new SqlConnection(con);
            try
            {
                conSQL.Open();
                MessageBox.Show("Kết nối thành công!", "Thông báo");
                con1 = true;
            }
            catch (Exception)
            {
                con1 = false;
                MessageBox.Show("Kết nối thất bại!", "Thông báo");
            }
            finally
            {
                conSQL.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (con1 == true)
            {
                ControllerAruduino.Properties.Settings.Default.connect = con;
                ControllerAruduino.Properties.Settings.Default.Save();
                MessageBox.Show("Ghi cấu hình thành công!", "Thông báo");
                Application.Restart();
            }
            else
            {
                MessageBox.Show("Vui lòng kiểm tra kêt nối trước khi ghi cấu hình", "Thông báo!");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
