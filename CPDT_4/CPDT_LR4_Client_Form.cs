using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.IO;
using System;

namespace CPDT_4
{
    public partial class CPDT_LR4_Client_Form : Form
    {
        private Thread listenerThread;

        private readonly HttpListener listener;
        private HttpListenerContext context;
        private HttpListenerRequest request;

        private readonly string url, port;

        private bool isListening;


        public CPDT_LR4_Client_Form()
        {
            InitializeComponent();

            listener = new HttpListener();

            url = "http://127.0.0.1";
            port = "8888";

            isListening = false;
        }


        private void CPDT_LR4_Client_Form_Load(object sender, System.EventArgs e)
        {
            StartListener();

            this.FormClosing += StopListener;
        }


        private void StopListener(object sender, EventArgs e)
        {
            listenerThread.Suspend();
            isListening = false;
            listener.Stop();
        }


        private void StartListener()
        {
            listenerThread = new Thread(SendData);
            listenerThread.Start();
        }


        private void SendData()
        {
            var prefix = $"{url}:{port}/";

            listener.Prefixes.Add(prefix);
            listener.Start();
            isListening = true;

            while (isListening)
            {
                try
                {
                    context = listener.GetContext();
                    request = context.Request;

                    if (request.HttpMethod == "POST")
                    {
                        using (StreamReader reader = new StreamReader(request.InputStream))
                        {
                            string message = reader.ReadToEnd();

                            this.textBox1.Text = message;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Client did an oopsie\n\r" + ex.ToString());
                }
            }
        }
    }
}
