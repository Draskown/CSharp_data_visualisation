using System.Collections.Generic;
using System.Windows.Forms;
using System.Net.Sockets;
using System;

namespace CPDT_LR4
{
    public partial class CPDT_LR4_Server_Form : Form
    {
        private UdpClient sendClient;

        private readonly string address;

        private readonly int connectingPort;

        private double x;

        private bool started;


        public CPDT_LR4_Server_Form()
        {
            InitializeComponent();

            address = "127.0.0.1";
            connectingPort = 8888;

            x = 0.0;

            started = false;
        }


        private void CPDT_LR4_Server_Form_Load(object sender, EventArgs e)
        {
            this.btnStart.Click += ButtonAction;

            this.timer.Tick += SendData;
        }


        private void ButtonAction(object sender, EventArgs e)
        {
            if (started)
            {
                this.timer.Stop();

                this.btnStart.Text = "Start Server";
                started = false;
                sendClient.Close();
                sendClient.Dispose();
            }
            else
            {
                this.timer.Interval = 1000;
                this.timer.Start();

                this.btnStart.Text = "Stop Server";
                started = true;

                sendClient = new UdpClient();
            }
        }


        private void SendData(object sender, EventArgs e)
        {
            try
            {
                var data = GenerateData();
                sendClient.Send(data, data.Length, address, connectingPort);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server did an oopsie" + ex.ToString());
            }
        }


        private byte[] GenerateData()
        {
            var data = new byte[4];

            x += 0.1;

            data[0] = (byte)(Math.Cos(x) * 100);
            data[1] = (byte)(x * 255 - 70);
            data[2] = (byte)(Math.Abs(x - 5) * Math.Tanh(x) - Math.Atan2(x, x));
            data[3] = (byte) (x * x * x);

            return data;
        }
    }
}
