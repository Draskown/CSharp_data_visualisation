using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CPDT_LR3
{
    public partial class CPDT_LR3_Form : Form
    {
        private Bitmap bmp;

        private readonly TextBox[] textBoxes;

        private readonly FileStream stream;

        private readonly List<Device> devices;

        private readonly Message[] messages;

        private bool paused;

        public CPDT_LR3_Form()
        {
            InitializeComponent();

            this.timer.Interval = 1000;

            paused = false;

            stream = new FileStream("dump.dmp", FileMode.Open);

            messages = new Message[3445];

            for (int i = 0; i < messages.Length; i++)
                messages[i] = new Message();

            devices = new List<Device>();

            textBoxes = new[] { boxAmount, boxFrom, boxInto, boxPeriod, boxValue };
            foreach (var box in textBoxes)
                LeaveText(box, null);

            DrawBorder();
        }


        private void CPDT_LR3_Form_Load(object sender, EventArgs e)
        {
            this.Click += ClearSelection;

            this.timer.Tick += (ss, ee) =>
            {
                ReadData();
                DrawData();
            };

            this.btnPP.Click += StartPause;

            this.gridStats.Click += ClearSelection;
            this.gridLog.Click += ClearSelection;
            this.gridGate.Click += ClearSelection;
            this.gridAddresses.Click += ClearSelection;

            foreach (var box in textBoxes)
            {
                box.Enter += EnterText;
                box.Leave += LeaveText;
            }

            ClearSelection(null, null);

            this.timer.Start();
        }

        private void ReadData()
        {
            byte[] message = new byte[19];

            if (stream.Position == 65456)
                stream.Position = 0;

            stream.Read(message, 0, message.Length);

            var messageIndex = stream.Position / message.Length - 1;

            for (int i = 5; i < message.Length - 9; i++)
            {
                if (i == 6)
                {
                    messages[messageIndex].From = message[i];
                    if (devices.All(d => d.ID != message[i]))
                        devices.Add(new Device(message[i]));
                }
                if (i == 7)
                {
                    messages[messageIndex].Into = message[i];
                    if (devices.All(d => d.ID != message[i]))
                        devices.Add(new Device(message[i]));

                }
                if (i == 9)
                    messages[messageIndex].Value = message[i];
            }

            foreach (var d in devices)
            {
                if (message[6] == d.ID && message[7] != d.ID
                    && d.Joints.All(j => j.ID != message[7]))
                    d.Joints.Add(new Joint(message[7], 1));

                if (message[6] != d.ID && message[7] == d.ID
                    && d.Joints.All(j => j.ID != message[6]))
                    d.Joints.Add(new Joint(message[6], 0));
            }

            gridLog.Rows.Add(new[] { $"Device 0x{message[6]:X2} sent a value of {message[9]} to device 0x{message[7]:X2}" });
        }


        private void DrawData()
        {
            bmp = new Bitmap(frameGraph.Width, frameGraph.Height);

            int shape = devices.Count;
            int angle = 360 / shape;
            int radius = 120;

            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < shape; i++)
                {
                    int rectWidth = 30, rectHeight = 15;

                    Point dRectCenter = new Point((int)(bmp.Width / 2 - rectWidth / 2 + radius * (Math.Cos(angle * (i + 1) * Math.PI / 180))),
                                                  (int)(bmp.Height / 2 - rectHeight / 2 + radius * (Math.Sin(angle * (i + 1) * Math.PI / 180))));

                    foreach (var j in devices[i].Joints)
                    {
                        var jointIndex = devices.IndexOf(devices.First(d => d.ID == j.ID));

                        var jointAngle = angle * (jointIndex + 1);

                        Point jRectCenter = new Point((int)(bmp.Width / 2 - rectWidth / 2 + radius * (Math.Cos(jointAngle * Math.PI / 180))),
                                                      (int)(bmp.Height / 2 - rectHeight / 2 + radius * (Math.Sin(jointAngle * Math.PI / 180))));

                        if (j.Direction == 1)
                        {
                            g.DrawLine(new Pen(Color.Green, 2), dRectCenter, jRectCenter);
                        }
                        else
                            using (Pen p = new Pen(Color.Blue, 2))
                            {
                                float[] dashPatern = { 7, 7 };
                                p.DashPattern = dashPatern;
                                g.DrawLine(p, dRectCenter, jRectCenter);
                            }
                    }

                    g.DrawRectangle(new Pen(Color.Purple), dRectCenter.X, dRectCenter.Y, rectWidth, rectHeight);
                    g.FillRectangle(Brushes.Purple, dRectCenter.X, dRectCenter.Y, rectWidth, rectHeight);
                    g.DrawString("0x" + devices[i].ID.ToString("X2"), DefaultFont, Brushes.White, dRectCenter.X + 1, dRectCenter.Y + 1);
                }
            }

            this.frameGraph.Image = bmp;
        }


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

        private void DrawBorder()
        {
            bmp = new Bitmap(this.frameGraph.Width, this.frameGraph.Height);

            using (Graphics g = Graphics.FromImage(bmp))
                g.DrawRectangle(new Pen(Color.White), 1, 1, bmp.Width - 1, bmp.Height - 1);
        }


        private void ClearSelection(object sender, EventArgs e)
        {
            try
            {
                DataGridView grid = (DataGridView)sender;

                if (grid == null)
                {
                    gridLog.ClearSelection();
                    gridGate.ClearSelection();
                    gridStats.ClearSelection();
                    gridAddresses.ClearSelection();
                    this.ActiveControl = null;
                    return;
                }

                switch (grid.Name)
                {
                    case "gridLog":
                        gridGate.ClearSelection();
                        gridStats.ClearSelection();
                        gridAddresses.ClearSelection();
                        break;
                    case "gridGate":
                        gridLog.ClearSelection();
                        gridStats.ClearSelection();
                        gridAddresses.ClearSelection();
                        break;
                    case "gridStats":
                        gridLog.ClearSelection();
                        gridGate.ClearSelection();
                        gridAddresses.ClearSelection();
                        break;
                    case "gridAddresses":
                        gridLog.ClearSelection();
                        gridGate.ClearSelection();
                        gridStats.ClearSelection();
                        break;
                }
            }
            catch (InvalidCastException)
            {
                gridLog.ClearSelection();
                gridGate.ClearSelection();
                gridStats.ClearSelection();
                gridAddresses.ClearSelection();
                this.ActiveControl = null;
            }
        }


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



    #region Additional classes (Message, Device)

    public partial class Message
    {
        public byte From { get; set; }
        public byte Into { get; set; }
        public byte Value { get; set; }


        public Message()
        {
            this.From = this.Into = this.Value = 0;
        }
    }


    public partial class Device
    {
        public byte ID { get; set; }
        public List<Joint> Joints { get; set; }

        public Device()
        {
            this.ID = 255;
            this.Joints = new List<Joint>();
        }

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

    #endregion
}
