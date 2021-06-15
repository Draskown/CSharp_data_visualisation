using System.Collections.Generic;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Data.OleDb;
using System.Threading;
using System.Linq;
using System.Net;
using System;

namespace CPDT_4
{
    public partial class CPDT_LR4_Client_Form : Form
    {
        #region Initialization

        private Thread listenerThread;

        private UdpClient receiverClient;

        private OleDbConnection connection;
        private OleDbCommand cmd;

        private readonly NumericUpDown[] numsXScale,
                                         numsYScale,
                                         numsThinning;

        private readonly TextBox[] boxes;

        private readonly CheckBox[] normies;

        private readonly List<DateTime> x;
        private readonly List<string> status;
        private readonly List<double>[] ys;

        private readonly double[] ysMax, ysSlope;

        private readonly int localPort, depth;

        private bool isListening;


        public CPDT_LR4_Client_Form()
        {
            InitializeComponent();

            numsXScale = new NumericUpDown[] { numY1XScale, numY2XScale, numY3XScale, numY4XScale };
            numsYScale = new NumericUpDown[] { numY1YScale, numY2YScale, numY3YScale, numY4YScale };
            numsThinning = new NumericUpDown[] { numY1Thinning, numY2Thinning, numY3Thinning, numY4Thinning };

            boxes = new TextBox[] { tbY1MaxSpan, tbY1MinSpan,
                                    tbY2MaxSpan, tbY2MinSpan,
                                    tbY3MaxSpan, tbY3MinSpan,
                                    tbY4MaxSpan, tbY4MinSpan};

            normies = new CheckBox[] { cbNormY1, cbNormY2, cbNormY3, cbNormY4 };

            x = new List<DateTime>();
            status = new List<string>();

            ys = new List<double>[4];
            for (int i = 0; i < ys.Length; i++)
                ys[i] = new List<double>();

            ysMax = new double[] { 100, 200, 5, 10};
            ysSlope = new double[] { 50.0, 10.0, 2.0, 6.0};

            localPort = 8888;
            depth = 10;

            isListening = false;

            SetDateFormats();

            StartListener();
        }


        private void CPDT_LR4_Client_Form_Load(object sender, System.EventArgs e)
        {
            this.FormClosing += StopListener;

            this.Click += (ss, ee) => this.ActiveControl = null;

            this.timer.Interval = 16;
            this.timer.Tick += UpdateGraphs;

            foreach (var box in boxes)
                box.Leave += CheckInput;

            foreach (var num in numsXScale)
                num.ValueChanged += ZoomPan;

            this.btnSave.Click += (ss, ee) => { this.mainChart.SaveImage("img.png", System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Png); };
        }


        private void SetDateFormats()
        {
            for (int i = 0; i < mainChart.ChartAreas.Count; i++)
            {
                this.mainChart.Series[i].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
                this.mainChart.ChartAreas[i].AxisX.LabelStyle.Format = "HH:mm:ss:ff";
                this.mainChart.ChartAreas[i].AxisX.LabelStyle.Interval = 500;
                this.mainChart.ChartAreas[i].AxisX.LabelStyle.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Milliseconds;
            }
        }

        #endregion



        #region UDP connection

        private void StopListener(object sender, EventArgs e)
        {
            listenerThread.Suspend();
            isListening = false;
            receiverClient.Close();
            receiverClient.Dispose();
            timer.Stop();
            connection.Close();
            connection.Dispose();
        }


        private void StartListener()
        {
            receiverClient = new UdpClient(localPort);
            ConnectToDB();
            isListening = true;
            listenerThread = new Thread(Listen);
            listenerThread.Start();
            timer.Start();
        }


        private void Listen()
        {
            IPEndPoint remoteIp = null;

            while (isListening)
            {
                try
                {
                    var data = receiverClient.Receive(ref remoteIp);

                    if (ys.All(y => y.Count == 0) || 
                        ys.Where((y, i) => y.Last() != data[i]).Count() != 0)
                        FillArrays(data);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Client did an oopsie\n\r" + ex.Message);
                }
            }
        }


        private void FillArrays(byte[] _data)
        {
            var time = DateTime.Now;
            x.Add(time);

            var keys = "";

            for (int i = 0; i < ys.Length; i++)
            {
                if (normies[i].Checked)
                {
                    var sum = 0.0;
                    var count = 0;

                    for (int j = ys[i].Count - 1; j >= 0; j--)
                        if (ys[i].Count - j < depth)
                        {
                            sum += ys[i][j];
                            count++;
                        }

                    ys[i].Add(sum / count);
                }
                else
                    ys[i].Add(_data[i]);

                if (_data[i] > ysMax[i])
                    keys += "1";
                else if (Math.Abs(ys[i].Last() - _data[i]) >= ysSlope[i])
                    keys += "2";
                else
                    keys += "0";
            }

            status.Add(keys);

            FillDB(time.ToString("HH:mm:ss:ff"), _data, keys);
        }

        #endregion


        #region DB connection

        private void ConnectToDB()
        {
            try
            {
                connection = new OleDbConnection { ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=D:\Projects\C#\LRs\CPDT_4\bin\Debug\Database1.mdb" };
                connection.Open();

                cmd = new OleDbCommand("DELETE FROM log", connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect to the database!" + ex.Message);
            }
        }


        private void FillDB(string stamp, byte[] values, string keys)
        {
            cmd = new OleDbCommand(
                "INSERT INTO log VALUES ('"+stamp+"', '"+values[0]+"', '"+values[1]+"', '"+values[2]+"', '"+values[3]+"', '"+keys+"')",
                connection
            );

            cmd.ExecuteNonQuery();
        }

        #endregion



        #region Charts' Main handler

        private void UpdateGraphs(object sender, EventArgs e)
        {
            if (x.Count != 0)
                for (int i = 0; i < ys.Length; i++)
                    try
                    {
                        this.mainChart.Series[i].Points.DataBindXY(x.Where((a, k) => k % (int)numsThinning[i].Value == 0).ToArray(),
                                                                  ys[i].Where((a, k) => k % (int)numsThinning[i].Value == 0).ToArray());

                        for (int n = 0; n < ys.Length; n++)
                        {
                            if (boxes[n * 2].Text == "" || boxes[n * 2 + 1].Text == "")
                            {
                                this.mainChart.ChartAreas[n].AxisX.Maximum = x.Max().AddSeconds(10 - (double)numsXScale[n].Value / 20).ToOADate();
                                this.mainChart.ChartAreas[n].AxisX.Minimum = x.Last().AddSeconds(-10 + (double)numsXScale[n].Value / 20).ToOADate();
                            }

                            this.mainChart.ChartAreas[n].AxisY.Maximum = ys[n].Last() + (double)numsYScale[n].Maximum - (double)numsYScale[n].Value / 1.1;
                            this.mainChart.ChartAreas[n].AxisY.Minimum = ys[n].Last() - (double)numsYScale[n].Maximum + (double)numsYScale[n].Value / 1.1;
                        }
                    }
                    catch (Exception) { }
        }

        #endregion



        #region Input Checking

        private void CheckInput(object sender, EventArgs e)
        {
            try
            {
                var box = sender as TextBox;

                if (box.Text == "")
                    return;

                var span = Convert.ToDateTime(box.Text);

                var ix = box.Name[3] - '1';

                SetPan();
            }
            catch (FormatException)
            {
                MessageBox.Show("Please make sure all of the inputs match \"HH:mm:ss\" format");
            }
        }

        #endregion



        #region Pan of charts

        private void SetPan()
        {
            for (int i = 0; i < ys.Length * 2; i += 2)
            {
                if (boxes[i].Text != "" && boxes[i + 1].Text != "")
                {
                    this.mainChart.ChartAreas[i / 2].AxisX.Minimum = Convert.ToDateTime(boxes[i + 1].Text).ToOADate();
                    this.mainChart.ChartAreas[i / 2].AxisX.Maximum = Convert.ToDateTime(boxes[i].Text).ToOADate();
                }
            }
        }


        private void ZoomPan(object sender, EventArgs e)
        {
            for (int i = 0; i < ys.Length * 2; i += 2)
            {
                if (boxes[i].Text != "" && boxes[i + 1].Text != "")
                {
                    this.mainChart.ChartAreas[i / 2].AxisX.Maximum = Convert.ToDateTime(boxes[i].Text).AddSeconds(10 - (double)numsXScale[i / 2].Value / 20).ToOADate();
                    this.mainChart.ChartAreas[i / 2].AxisX.Minimum = Convert.ToDateTime(boxes[i + 1].Text).AddSeconds(-10 + (double)numsXScale[i / 2].Value / 20).ToOADate();
                }
            }
        }

        #endregion
    }
}
