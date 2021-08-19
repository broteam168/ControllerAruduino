using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControllerAruduino
{
    public partial class HandSignal : Form
    {
        public HandSignal()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text += "\n Đang tìm cổng.....";
            try
            { cbCOM1.DataSource = SerialPort.GetPortNames();
                textBox1.Text += "\n Đã tìm thấy cổng ";
            }
            catch (Exception ex)
            {
                textBox1.Text += "\n Đã xảy ra lỗi trong lúc tìm cổng:" + ex.Message;
            }
        }

        private void HandSignal_Load(object sender, EventArgs e)
        {
            cbCOM1.DataSource = SerialPort.GetPortNames();

            for (int i = 2; i <= 53; i++)
            {
                cbPinOut.Items.Add(i);
            }

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (!serialPort1.IsOpen)
                {
                    serialPort1.PortName = cbCOM1.Text;
                    serialPort1.Open();
                    timer1.Start();
                }
            }
            catch (Exception ex)
            {
                textBox1.Text += "\n Đã có lỗi xảy ra:" + ex.Message;
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Close();
                timer1.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã có lỗi xảy ra:" + ex.Message);
            }
        }

        private void HandSignal_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                //button1.Text = ("Kết nối");
                label6.Text = ("Chưa kết nối");
                label6.ForeColor = Color.Red;
                //Nếu Timer được làm mới, Cổng serialPort1 chưa được mở thì thay đổi Text trong button1, label3…đổi màu text label3 thành màu đỏ 
            }
            else if (serialPort1.IsOpen)
            {
                //button1.Text = ("Ngắt kết nối");
                label6.Text = ("Ðã kết nối");
                label6.ForeColor = Color.Green;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.Write("s"+cbPinOut.Text+"pL");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            serialPort1.Write("s" + cbPinOut.Text + "ph");
        }
    }
}
