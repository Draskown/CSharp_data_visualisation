using System.Windows.Forms.DataVisualization.Charting;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.IO;
using System;

namespace CPDT_LR3
{
    public partial class CPDT_LR3_Form : Form
    {
        #region Initialization

        private Bitmap bmp;

        private readonly TextBox[] textBoxes;
        private readonly ComboBox[] combos;
        private readonly Chart[] plots;

        private readonly FileStream stream;

        private readonly List<Device> devices;

        private readonly List<Connection> connections;

        private readonly List<Message> messages;
        private Message sendMsg;

        private readonly DateTime startTime;

        private int[] packetAmountOfMessages;
        private int amountOfMessagesSent, time;

        private bool paused, sendingPending;

        private readonly string allPattern;


        public CPDT_LR3_Form()
        {
            InitializeComponent();

            this.timer.Interval = 50;

            amountOfMessagesSent = 0;

            paused = sendingPending = false;

            allPattern = "*";

            stream = new FileStream("dump.dmp", FileMode.Open);

            messages = new List<Message>();

            devices = new List<Device>();
            connections = new List<Connection>();

            plots = new[] { chart1, chart2, chart3, chart4 };

            combos = new[] { comboConnection1, comboConnection2, comboConnection3, comboConnection4 };

            textBoxes = new[] { boxFrom, boxInto, boxValue, boxPeriod, boxAmount };
            foreach (var box in textBoxes)
                LeaveText(box, null);

            startTime = DateTime.Now;
            time = 0;
        }


        private void CPDT_LR3_Form_Load(object sender, EventArgs e)
        {
            this.timer.Tick += (ss, ee) =>
            {
                if (sendingPending)
                {
                    do
                    {
                        if (!FilterMessage(sendMsg))
                        {
                            messages.Add(sendMsg);
                            AddMessage("Message did not pass the filter", sendMsg);
                            this.frameIndicator.BackColor = Color.Red;
                            amountOfMessagesSent++;
                            sendingPending = amountOfMessagesSent < packetAmountOfMessages.Length;
                            break;
                        }
                        if (packetAmountOfMessages[amountOfMessagesSent] == 1)
                        {
                            WriteIntoFile();
                            AddMessage($"The send complete! (0x{sendMsg.From:X2} ({sendMsg.Value}) -> 0x{sendMsg.Into:X2})", sendMsg);
                            amountOfMessagesSent++;

                            sendingPending = amountOfMessagesSent < packetAmountOfMessages.Length;
                        }
                        else
                        {
                            amountOfMessagesSent++;
                            sendingPending = amountOfMessagesSent < packetAmountOfMessages.Length;
                        }
                    } while (false);
                }

                ReadData();
                DrawData(99);
            };

            this.btnPP.Click += StartPause;

            this.gridAddresses.SelectionChanged += DisplayTheStats;
            this.gridLog.SelectionChanged += DisplayTheValues;
            this.gridGate.CellEndEdit += CheckInput;

            foreach (var box in textBoxes)
            {
                box.Enter += EnterText;
                box.Leave += LeaveText;
            }

            this.btnSend.Click += SendPacket;

            this.timer.Start();
        }

        #endregion



        #region Grid actions

        private void DisplayTheStats(object sender, EventArgs e)
        {
            var grid = (DataGridView)sender;

            if (grid.SelectedCells.Count == 0)
                return;

            int index = grid.SelectedCells[0].RowIndex;

            double incoming = devices[index].Joints.Count(j => j.Direction == 0) / (double)connections.Count;
            double outcoming = devices[index].Joints.Count(j => j.Direction == 1) / (double)connections.Count;
            double overall = devices[index].Joints.Count / (double)connections.Count;

            this.gridStats.Rows.Clear();
            this.gridStats.Rows.Add(new object[] { "Incoming", $"{Math.Round(incoming, 2) * 100} %" });
            this.gridStats.Rows.Add(new object[] { "Outcoming", $"{Math.Round(outcoming, 2) * 100} %" });
            this.gridStats.Rows.Add(new object[] { "Overall", $"{Math.Round(overall, 2) * 100} %" });

            DrawData(index);
        }


        private void DisplayTheValues(object sender, EventArgs e)
        {
            if (!sendingPending)
            {
                var grid = (DataGridView)sender;

                if (grid.SelectedCells.Count == 0)
                    return;

                var index = grid.SelectedCells[0].RowIndex;

                var from = messages[index].From;
                var into = messages[index].Into;
                var value = messages[index].Value;

                for (int i = 0; i < textBoxes.Length - 2; i++)
                    textBoxes[i].ForeColor = Color.White;

                this.boxFrom.Text = $"0x{from:X2}";
                this.boxInto.Text = $"0x{into:X2}";
                this.boxValue.Text = $"{value}";
            }
        }


        private void CheckInput(object sender, DataGridViewCellEventArgs e)
        {
            var grid = (DataGridView)sender;

            string value = grid.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue.ToString();

            if (value == "*" || value == "")
                return;

            try
            {
                byte a = Convert.ToByte(value, 16);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Check your inputs please!\r\n" + ex.ToString());
                grid.SelectedCells[0].Value = "";
            }
        }

        #endregion



        #region Reading dump

        private void ReadData()
        {
            byte[] message = new byte[19];

            if (stream.Position == 65456)
                stream.Position = 0;

            stream.Read(message, 0, message.Length);

            this.frameIndicator.BackColor = Color.Transparent;

            var msg = new Message(message[6], message[7], message[9]);

            if (!FilterMessage(msg))
            {
                messages.Add(msg);
                AddMessage("Message did not pass the filter", msg);
                this.frameIndicator.BackColor = Color.Red;
                RemoveConnectionsAndDevices();
                return;
            }

            AddMessage($"0x{message[6]:X2} sent a value of {message[9]} to 0x{message[7]:X2}", msg);

            if (devices.All(d => d.ID != msg.From))
                devices.Add(new Device(msg.From));

            if (devices.All(d => d.ID != msg.Into))
                devices.Add(new Device(msg.Into));

            foreach (var d in devices)
            {
                if (message[6] == d.ID &&
                    d.Joints.All(j => j.ID != message[7]))
                {
                    d.Joints.Add(new Joint(message[7], 1));

                    time = (Int16)(DateTime.Now - startTime).TotalSeconds;
                    connections.Add(new Connection(message[6], message[7], message[9], time.ToString()));
                }

                if (message[7] == d.ID &&
                    d.Joints.All(j => j.ID != message[6]))
                {
                    d.Joints.Add(new Joint(message[6], 0));
                }
            }

            gridAddresses.Rows.Clear();
            devices.ForEach(d => gridAddresses.Rows.Add(new object[] { "0x" + d.ID.ToString("X2") }));

            foreach (var box in combos)
            {
                box.Items.Clear();
                foreach (var c in connections)
                    box.Items.Add($"0x{c.From:X2} -> 0x{c.Into:X2}");
            }
        }

        #endregion



        #region Drawing

        private void DrawData(int deviceIndex)
        {
            int shape = devices.Count;
            int angle = 0;
            if (shape != 0)
                angle = 360 / shape;

            bmp = new Bitmap(frameGraph.Width, frameGraph.Height);

            Point dRectCenter;
            int rectWidth = 30, rectHeight = 15;

            int radius = 120;

            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < shape; i++)
                {
                    dRectCenter = new Point((int)(bmp.Width / 2 - rectWidth / 2 + radius * (Math.Cos(angle * (i + 1) * Math.PI / 180))),
                                                  (int)(bmp.Height / 2 - rectHeight / 2 + radius * (Math.Sin(angle * (i + 1) * Math.PI / 180))));

                    g.DrawRectangle(new Pen(Color.Purple), dRectCenter.X, dRectCenter.Y, rectWidth, rectHeight);
                    g.FillRectangle(Brushes.Purple, dRectCenter.X, dRectCenter.Y, rectWidth, rectHeight);
                    g.DrawString("0x" + devices[i].ID.ToString("X2"), DefaultFont, Brushes.White, dRectCenter.X + 1, dRectCenter.Y + 1);
                }

                foreach (var c in connections)
                {
                    var baseIndex = devices.IndexOf(devices.First(d => d.ID == c.From));
                    var baseAngle = angle * (baseIndex + 1);

                    var jointIndex = devices.IndexOf(devices.First(d => d.ID == c.Into));
                    var joinAngle = angle * (jointIndex + 1);

                    dRectCenter = new Point((int)(bmp.Width / 2 - rectWidth / 2 + radius * (Math.Cos(baseAngle * Math.PI / 180))),
                                            (int)(bmp.Height / 2 - rectHeight / 2 + radius * (Math.Sin(baseAngle * Math.PI / 180))));

                    var jRectCenter = new Point((int)(bmp.Width / 2 - rectWidth / 2 + radius * (Math.Cos(joinAngle * Math.PI / 180))),
                                            (int)(bmp.Height / 2 - rectHeight / 2 + radius * (Math.Sin(joinAngle * Math.PI / 180))));

                    if (deviceIndex == 99)
                        g.DrawLine(new Pen(Color.White, 2), dRectCenter, jRectCenter);
                    else if (deviceIndex != baseIndex && deviceIndex != jointIndex)
                        g.DrawLine(new Pen(Color.DimGray, 2), dRectCenter, jRectCenter);
                }

                if (deviceIndex != 99)
                {
                    dRectCenter = new Point((int)(bmp.Width / 2 - rectWidth / 2 + radius * (Math.Cos(angle * (deviceIndex + 1) * Math.PI / 180))),
                                           (int)(bmp.Height / 2 - rectHeight / 2 + radius * (Math.Sin(angle * (deviceIndex + 1) * Math.PI / 180))));

                    foreach (var j in devices[deviceIndex].Joints)
                    {
                        var jointIndex = devices.IndexOf(devices.First(d => d.ID == j.ID));

                        var jointAngle = angle * (jointIndex + 1);

                        Point jRectCenter = new Point((int)(bmp.Width / 2 - rectWidth / 2 + radius * (Math.Cos(jointAngle * Math.PI / 180))),
                                                      (int)(bmp.Height / 2 - rectHeight / 2 + radius * (Math.Sin(jointAngle * Math.PI / 180))));

                        if (j.Direction == 1)
                            using (Pen p = new Pen(Color.Blue, 2))
                            {
                                p.DashPattern = new float[] { 7, 7 };
                                g.DrawLine(p, dRectCenter, jRectCenter);
                            }
                        else
                            g.DrawLine(new Pen(Color.Green, 2), dRectCenter, jRectCenter);
                    }
                }
            }

            this.frameGraph.Image = bmp;
        }

        #endregion



        #region Message handling

        private bool FilterMessage(Message msg)
        {
            byte from = msg.From;
            byte into = msg.Into;
            int value = msg.Value;

            var conclusion = new bool[this.gridGate.Rows.Count];
            for (int i = 0; i < conclusion.Length; i++)
                conclusion[i] = false;

            foreach (DataGridViewRow row in this.gridGate.Rows)
            {
                if (row.Index == this.gridGate.Rows.Count - 1 ||
                    row.Cells[0].Value == null ||
                    row.Cells[1].Value == null ||
                    row.Cells[2].Value == null || 
                    row.Cells[0].Value.ToString() == "" ||
                    row.Cells[1].Value.ToString() == "" ||
                    row.Cells[2].Value.ToString() == "")
                {
                    conclusion[row.Index] = true;
                    continue;
                }

                var ruleN = row.Index;
                var fromRuleStr = row.Cells[0].Value.ToString();
                var intoRuleStr = row.Cells[1].Value.ToString();
                var valueRuleStr = row.Cells[2].Value.ToString();

                byte fromRule = 0, intoRule = 0, valueRule = 0;

                if (fromRuleStr != allPattern)
                {
                    fromRule = Convert.ToByte(row.Cells[0].Value.ToString(), 16);

                    if (intoRuleStr != allPattern)
                    {
                        intoRule = Convert.ToByte(row.Cells[1].Value.ToString(), 16);

                        if (valueRuleStr != allPattern)
                        {
                            valueRule = Convert.ToByte(row.Cells[2].Value.ToString(), 16);

                            if (from == fromRule && into == intoRule && value <= valueRule)
                                conclusion[ruleN] = true;
                        }
                        else if (from == fromRule && into == intoRule)
                            conclusion[ruleN] = true;
                    }
                    else if (valueRuleStr != allPattern)
                    {
                        valueRule = Convert.ToByte(row.Cells[2].Value.ToString(), 16);

                        if (from == fromRule && value <= valueRule)
                            conclusion[ruleN] = true;
                    }
                    else if (from == fromRule)
                        conclusion[ruleN] = true;
                }
                else
                {
                    if (intoRuleStr != allPattern)
                    {
                        intoRule = Convert.ToByte(row.Cells[1].Value.ToString(), 16);

                        if (valueRuleStr != allPattern)
                        {
                            valueRule = Convert.ToByte(row.Cells[2].Value.ToString(), 16);

                            if (into == intoRule && value <= valueRule)
                                conclusion[ruleN] = true;
                        }
                        else if (into == intoRule)
                            conclusion[ruleN] = true;
                    }
                    else if (valueRuleStr != allPattern)
                    {
                        valueRule = Convert.ToByte(row.Cells[2].Value.ToString(), 16);

                        if (value <= valueRule)
                            conclusion[ruleN] = true;
                    }
                }
            }

            if (conclusion.All(b => b))
                return true;
            else
                return false;
        }


        private void AddMessage(string text, Message message)
        {
            if (this.gridLog.Rows.Count == 3445)
                this.gridLog.Rows.Clear();

            if (messages.Count == 3445)
                messages.Clear();

            if (connections.Count > 0)
                foreach (var c in connections)
                {
                    if ((c.From == message.From || c.Into == message.From) &&
                        (c.Into == message.Into || c.From == message.Into))
                    {
                        time = (Int16)(DateTime.Now - startTime).TotalSeconds;
                        c.Values.Add(message.Value);
                        c.TimeStamps.Add(time.ToString());
                    }

                    UpdatePlot(c);
                }

            messages.Add(message);

            this.gridLog.SelectionChanged -= DisplayTheValues;

            if (this.gridLog.SelectedCells.Count == 0)
                foreach (var box in textBoxes)
                {
                    box.Text = "";
                    LeaveText(box, null);
                }

            time = (Int16)(DateTime.Now - startTime).TotalSeconds;

            this.gridLog.Rows.Add(new[] { time.ToString() + "> " + text });
            this.gridLog.SelectionChanged += DisplayTheValues;
            this.gridLog.FirstDisplayedScrollingRowIndex = this.gridLog.Rows.Count - 1;
        }


        private void RemoveConnectionsAndDevices()
        {
            var safe = new List<byte>();

            byte from = 0, into = 0;
            bool allFrom = false, allInto = false;

            foreach (DataGridViewRow row in this.gridGate.Rows)
            {
                if (row.Index == this.gridGate.Rows.Count - 1)
                    continue;

                if (row.Cells[0].Value.ToString() != allPattern)
                    from = Convert.ToByte(row.Cells[0].Value.ToString(), 16);
                else
                    allFrom = true;

                if (row.Cells[1].Value.ToString() != allPattern)
                    into = (Convert.ToByte(row.Cells[1].Value.ToString(), 16));
                else
                    allInto = true;

                for (int i = 0; i < devices.Count; i++)
                {
                    if (devices[i].ID == from && allInto)
                        devices[i].Joints.ForEach(j =>
                        {
                            if (j.Direction == 1)
                                safe.Add(j.ID);
                        });

                    if (devices[i].ID == into && allFrom)
                        devices[i].Joints.ForEach(j =>
                        {
                            if (j.Direction == 0)
                                safe.Add(j.ID);
                        });
                }

                for (int i = 0; i < devices.Count; i++)
                {
                    if (devices[i].ID != from && !allFrom &&
                        !safe.Contains(devices[i].ID))
                    {
                        devices.RemoveAt(i);
                        i = -1;
                        continue;
                    }

                    if (devices[i].ID != into && !allInto &&
                       !safe.Contains(devices[i].ID))
                    {
                        devices.RemoveAt(i);
                        i = -1;
                        continue;
                    }
                }

                for (int i = 0; i < devices.Count; i++)
                {
                    var d = devices[i];

                    var count = 0;
                    for (int j = 0; j < d.Joints.Count; j++)
                    {
                        for (int l = 0; l < devices.Count; l++)
                            if (d.Joints[j].ID == devices[l].ID)
                                count++;

                        if (count == 0)
                        {
                            d.Joints.RemoveAt(j);
                            j = -1;
                            continue;
                        }
                    }

                }

                for (int i = 0; i < connections.Count; i++)
                {
                    var c = connections[i];
                    if (c.From != from && !allFrom)
                    {
                        connections.RemoveAt(i);
                        i = -1;
                        continue;
                    }
                    if (c.Into != into && !allInto)
                    {
                        connections.RemoveAt(i);
                        i = -1;
                        continue;
                    }
                }
            }
        }

        #endregion



        #region Plotting

        private void UpdatePlot(Connection c)
        {
            var str = $"0x{c.From:X2} -> 0x{c.Into:X2}";

            foreach (var cb in combos)
                if (cb.Text == str)
                {
                    var index = cb.Name.Last();

                    foreach (var p in plots)
                        if (p.Name.Last() == index)
                        {
                            p.Series[0].Points.DataBindXY(c.TimeStamps, c.Values);
                            p.Update();
                        }
                }
        }

        #endregion



        #region Packet sending

        private void SendPacket(object sender, EventArgs e)
        {
            if (!sendingPending)
            {
                try
                {
                    amountOfMessagesSent = 0;

                    var totalAmountOfMessages = Convert.ToInt32(this.boxAmount.Text);
                    var period = Convert.ToInt32(this.boxPeriod.Text);
                    var from = Convert.ToByte(this.boxFrom.Text, 16);
                    var into = Convert.ToByte(this.boxInto.Text, 16);
                    var value = Convert.ToInt32(this.boxValue.Text);

                    if (totalAmountOfMessages > period)
                    {
                        MessageBox.Show("Amount of messages should be <= period");
                        return;
                    }
                    if (from == into)
                    {
                        MessageBox.Show("Devices must differ");
                        return;
                    }

                    Device sendingDevice = new Device(0);
                    Device receivingDevice = new Device(0);

                    if (devices.All(d => d.ID != from))
                    {
                        sendingDevice.ID = from;
                        devices.Add(sendingDevice);
                    }
                    if (devices.All(d => d.ID != into))
                    {
                        receivingDevice.ID = into;
                        devices.Add(receivingDevice);
                    }
                    else
                    {
                        for (int i = 0; i < devices.Count; i++)
                        {
                            if (devices[i].ID == from)
                                sendingDevice = devices[i];
                            if (devices[i].ID == into)
                                receivingDevice = devices[i];
                        }
                    }

                    if (sendingDevice.Joints.All(j => j.ID != into))
                        sendingDevice.Joints.Add(new Joint(into, 0));

                    if (sendingDevice.Joints.All(j => j.ID != from))
                        receivingDevice.Joints.Add(new Joint(from, 1));

                    for (int i = 0; i < connections.Count; i++)
                    {
                        var c = connections[i];

                        time = (Int16)(DateTime.Now - startTime).TotalSeconds;

                        if (c.From != from && c.Into != into ||
                            c.From == from && c.Into != into ||
                            c.From != from && c.Into == into)
                        {
                            connections.Add(new Connection(from, into, value, time.ToString()));
                            break;
                        }
                        if (c.From == from && c.Into == into)
                        {
                            c.Values.Add(value);
                            c.TimeStamps.Add(time.ToString());
                            break;
                        }
                    }

                    packetAmountOfMessages = new int[period];

                    Random r = new Random();

                    while (true)
                    {
                        packetAmountOfMessages[r.Next(0, period)] = 1;

                        if (packetAmountOfMessages.Count(p => p == 1) == totalAmountOfMessages)
                            break;
                    }

                    if (totalAmountOfMessages == period)
                        for (int i = 0; i < period; i++)
                            packetAmountOfMessages[i] = 1;

                    sendingPending = true;
                    sendMsg = new Message(from, into, value);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Check the packet input please \r\n" + ex.ToString());
                }
            }
        }

        #endregion



        #region Writing generetated message

        private void WriteIntoFile()
        {
            FileStream writer = new FileStream("sentData.dmp", FileMode.Open, FileAccess.Write);

            var sendingData = new byte[19];

            time = (Int16)(DateTime.Now - startTime).TotalSeconds;

            var timeBytes = BitConverter.GetBytes(time);

            for (int i = 0; i < sendingData.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        sendingData[0] = timeBytes[0];
                        break;
                    case 1:
                        sendingData[1] = timeBytes[1];
                        break;
                    case 6:
                        sendingData[6] = sendMsg.From;
                        break;
                    case 7:
                        sendingData[7] = sendMsg.Into;
                        break;
                    case 8:
                        sendingData[8] = 0x08;
                        break;
                    case 9:
                        sendingData[9] = (byte)sendMsg.Value;
                        break;
                    case 18:
                        sendingData[18] = 0xff;
                        break;
                    case 19:
                        sendingData[19] = 0xff;
                        break;
                    default:
                        sendingData[i] = 0x00;
                        break;
                }
            }

            writer.Seek(writer.Length, SeekOrigin.Begin);
            writer.Write(sendingData, 0, sendingData.Length);
            writer.Close();
        }

        #endregion



        #region Button Action

        private void StartPause(object sender, EventArgs e)
        {
            var o = (Button)sender;

            if (!paused)
            {
                o.Text = "Resume";
                this.timer.Stop();
            }
            else
            {
                o.Text = "Pause";
                this.timer.Start();
            }

            paused = !paused;
        }

        #endregion



        #region Crutches of visuals

        private void LeaveText(object sender, EventArgs e)
        {
            TextBox box = (TextBox)sender;

            if (box.Text == "")
            {
                box.ForeColor = Color.Gray;
                box.Text = box.Name.Substring(3, box.Name.Length - 3) + ":";
            }
        }


        private void EnterText(object sender, EventArgs e)
        {
            TextBox box = (TextBox)sender;

            if (box.Text == box.Name.Substring(3, box.Name.Length - 3) + ":")
            {
                box.ForeColor = Color.White;
                box.Text = "";
            }
        }

        #endregion
    }



    #region Additional classes (Message, Device, etc)

    public partial class Message
    {
        public byte From { get; set; }
        public byte Into { get; set; }
        public int Value { get; set; }


        public Message(byte f, byte i, int v)
        {
            this.From = f; this.Into = i; this.Value = v;
        }
    }


    public partial class Device
    {
        public byte ID { get; set; }
        public List<Joint> Joints { get; set; }

        public Device(byte id)
        {
            this.ID = id;
            this.Joints = new List<Joint>();
        }
    }


    public partial class Joint
    {
        public int Direction { get; set; }
        public byte ID { get; set; }

        public Joint(byte id, int d)
        {
            this.ID = id;
            this.Direction = d;
        }
    }


    public partial class Connection
    {
        public byte From { get; set; }
        public byte Into { get; set; }
        public List<int> Values { get; set; }
        public List<string> TimeStamps { get; set; }

        public Connection(byte f, byte i, int v, string d)
        {
            this.From = f; this.Into = i;
            this.Values = new List<int> { v };
            this.TimeStamps = new List<string> { d };
        }
    }

    #endregion
}
