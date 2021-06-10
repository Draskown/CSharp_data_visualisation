using System.Windows.Forms;
using System.Net;
using System.IO;
using System;

namespace CPDT_4
{
    public partial class CPDT_LR4_Server_Form : Form
    {
        private readonly string url, port;
        private string sendLine;

        private bool started;


        public CPDT_LR4_Server_Form()
        {
            InitializeComponent();

            url = "http://127.0.0.1";
            port = "8888";

            started = false;
        }


        private void CPDT_LR4_Server_Form_Load(object sender, EventArgs e)
        {
            //this.btnStart.Click += ButtonAction;

            //this.timer.Tick += SendData;

            SendData(null, null);
        }


        private void ButtonAction(object sender, EventArgs e)
        {
            if (started)
            {
                this.timer.Stop();

                this.btnStart.Text = "Start Server";
                started = false;
            }
            else
            {
                this.timer.Start();

                this.btnStart.Text = "Stop Server";
                started = true;
            }
        }


        private void SendData(object sender, EventArgs e)
        {
            try
            {
                var httpRequest = (HttpWebRequest)WebRequest.Create($"{url}:{port}/");
                httpRequest.Method = "POST";

                sendLine = String.Join(", ", GenerateData());

                using (var writer = new StreamWriter(httpRequest.GetRequestStream()))
                    writer.Write(sendLine);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server did an oopsie" + ex.ToString());
            }
        }


        private byte[] GenerateData()
        {
            var data = new byte[3];

            data[0] = 53;
            data[1] = 20;
            data[2] = 74;

            return data;
        }
    }
}
