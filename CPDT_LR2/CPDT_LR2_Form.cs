using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CPDT_LR2
{
    public partial class CPDT_LR2_Form : Form
    {
        #region Initialization and events

        private readonly VerSector[] pointsCloud;
        private readonly FileStream stream;
        private readonly Angle[] horAngles;

        private Bitmap bmpIsometric, bmpOverhead;
        private PointF initialPoint;

        private readonly double angleDelta;
        private readonly double[] shootingAngles, verAngles;
        private double[][] rotationX, rotationY;
        private double distance, angleX, angleY;

        private readonly int pointSize;
        private int messagesCount;

        private bool paused, started;


        public CPDT_LR2_Form()
        {
            InitializeComponent();

            started = paused = false;
            angleX = angleY = 0;
            messagesCount = 0;
            angleDelta = 0.01;
            distance = 150;
            pointSize = 4;

            stream = new FileStream("UDPFromVelodyneTest_lidardata.pcap", FileMode.Open);

            shootingAngles = new[] {-30.67, -9.33, -29.33, -8.00, -28.00, -6.66, -26.66, -5.33,
                                    -25.33, -4.00, -24.00, -2.67, -22.67, -1.33, -21.33, 0.00,
                                    -20.00, 1.33, -18.67, 2.67, -17.33, 4.00, -16.00, 5.33,
                                    -14.67, 6.67, -13.33, 8.00, -12.00, 9.33, -10.67, 10.67
            };

            pointsCloud = new VerSector[361];
            horAngles = new Angle[361];
            verAngles = new double[32];

            for (int i = 0; i < horAngles.Length; i++)
            {
                if (i < verAngles.Length)
                    verAngles[i] = -Math.Sin(shootingAngles[i] * Math.PI / 180);

                horAngles[i] = new Angle(Math.Sin(i * Math.PI / 180),
                                         Math.Cos(i * Math.PI / 180));

                pointsCloud[i] = new VerSector();
            }
        }


        private void CPDT_LR2_Form_Load(object sender, System.EventArgs e)
        {
            this.frameRate.Interval = 16; //(int)this.numFramerate.Value;
            this.frameRate.Tick += (ss, ee) => { ReadData(); DrawData(); };

            this.Click += (ss, ee) => this.ActiveControl = null;

            this.numFramerate.ValueChanged += (ss, ee) =>
                this.frameRate.Interval = (int)this.numFramerate.Value;

            this.btnPp.Click += StartPause;

            this.frameIsometric.MouseWheel += PicImageMouseWheel;
            this.frameIsometric.MouseDown += PicImageMouseDown;
            this.frameIsometric.MouseMove += PicImageMouseMove;
        }

        #endregion



        #region MouseMovement

        private void PicImageMouseDown(object sender, MouseEventArgs e)
        {
            initialPoint = e.Location;
        }


        private void PicImageMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left ||
                this.frameIsometric.Image == null)
                return;

            if (e.Button == MouseButtons.Left)
            {
                PointF finalPoint = e.Location;

                angleX -= (float)((finalPoint.X - initialPoint.X) * angleDelta);
                angleY += (float)((finalPoint.Y - initialPoint.Y) * angleDelta);

                DrawData();

                initialPoint = finalPoint;
            }
        }


        private void PicImageMouseWheel(object sender, MouseEventArgs e)
        {
            if (this.frameIsometric.Image == null)
                return;

            distance += e.Delta / 10;
            distance = distance < 0 ? 0 : distance;

            DrawData();
        }

        #endregion



        #region Reading From Dump

        private void ReadData()
        {
            byte[] line = new byte[300];
            stream.Read(line, 0, 300);
            List<byte> st = new List<byte>();

            for (int i = 0; i < line.Length - 1; i++)
            {
                if (line[i] == 255 && line[i + 1] == 238)
                {
                    if (!paused)
                    {
                        i++;
                        if (st.Count != 0)
                        {
                            if (st.Count == 98)
                            {
                                double azimut = (st[1] * 256 + st[0]) / 100;

                                st.RemoveAt(0);
                                st.RemoveAt(1);
                                int laserId = 0;

                                for (int instance = 0; instance < st.Count; instance += 3)
                                {
                                    double distance = (st[instance + 1] * 256 + st[instance]) * 0.002f;

                                    pointsCloud[(int)azimut].Beam[laserId] = new Vector(distance * (float)horAngles[(int)azimut].Sin,
                                                                                        distance * (float)verAngles[laserId],
                                                                                        distance * (float)horAngles[(int)azimut].Cos);

                                    laserId++;
                                }
                            }

                            st.Clear();
                        }
                    }
                }
                else
                    st.Add(line[i]);
            }

            var message = DateTime.Now.ToString("HH:mm:ss") + $"> received {st.Count} points\r\n";

            if (this.boxLog.Lines.Length % 32 == 0)
                this.boxLog.Text = "";
            else
                this.boxLog.Text += message;

            messagesCount = messagesCount > 32 ? 0 : messagesCount++;
        }

        #endregion



        #region Drawing

        private void DrawData()
        {
            bmpIsometric = new Bitmap(this.frameIsometric.Width, this.frameIsometric.Height);
            bmpOverhead = new Bitmap(this.frameOverhead.Width, this.frameOverhead.Height);

            rotationX = new double[][] {
                        new double[] { 1, 0, 0},
                        new double[] { 0, Math.Cos(angleY), -Math.Sin(angleY)},
                        new double[] { 0, Math.Sin(angleY), Math.Cos(angleY)}
            };

            rotationY = new double[][] {
                        new double[] { Math.Cos(angleX), 0, Math.Sin(angleX)},
                        new double[] { 0, 1, 0 },
                        new double[] { Math.Sin(angleX), 0, Math.Cos(angleX)}
            };


            using (Graphics g = Graphics.FromImage(bmpIsometric))
            {
                for (int row = 0; row < pointsCloud.Length; row++)
                {
                    for (int column = 0; column < pointsCloud[row].Beam.Length; column++)
                    {
                        var v = pointsCloud[row].Beam[column];

                        double allowedDistance = 0.2;

                        if (v.X <= allowedDistance && v.Y <= allowedDistance && v.Z <= allowedDistance)
                            continue;

                        Vector rotated = MatMul(rotationX, v);
                        rotated = MatMul(rotationY, rotated);

                        double[][] projection =
                        {
                            new double[] { 1, 0, 0 },
                            new double[] { 0, 1, 0 }
                        };

                        Vector projected2d = MatMul(projection, rotated);

                        projected2d.Mult(distance);

                        projected2d.Move(new PointF(this.frameIsometric.Width / 2, this.frameIsometric.Height / 2));

                        int alpha = (int)((rotated.Z + 8.6) * 255 / 8.5 / 2);
                        alpha = alpha > 255 ? 255 : alpha < 50 ? 50 : alpha;

                        g.FillEllipse(new SolidBrush(Color.FromArgb((int)alpha, 255, 0, 255)), (float)projected2d.X, (float)projected2d.Y, pointSize, pointSize);
                    }
                }
            }

            this.frameIsometric.Image = bmpIsometric;

            using (Graphics g = Graphics.FromImage(bmpOverhead))
            {
                for (int row = 0; row < pointsCloud.Length; row++)
                {
                    for (int column = 0; column < pointsCloud[row].Beam.Length; column++)
                    {
                        var v = pointsCloud[row].Beam[column];

                        double allowedDistance = 0.2;

                        if (v.X <= allowedDistance && v.Y <= allowedDistance && v.Z <= allowedDistance)
                            continue;

                        double[][] projection =
                        {
                            new double[] { 1, 0, 0 },
                            new double[] { 0, 0, 0 },
                            new double[] { 0, 0, 1 }
                        };

                        Vector projected2d = MatMul(projection, v);

                        projected2d.Mult(25);

                        int alpha = (int)((v.Z + 8.6) * 255 / 8.5 / 2);
                        alpha = alpha > 255 ? 255 : alpha < 50 ? 50 : alpha;

                        g.FillEllipse(new SolidBrush(Color.FromArgb((int)alpha, 255, 0, 255)), (float)projected2d.X + 175, (float)projected2d.Z + 100, pointSize, pointSize);
                    }
                }
            }

            this.frameOverhead.Image = bmpOverhead;
        }

        #endregion



        #region Button Action

        private void StartPause(object sender, EventArgs e)
        {
            var o = (Button)sender;

            if (!started)
            {
                this.frameRate.Start();
                o.Text = "Pause";
                started = true;
                return;
            }

            if (!paused)
            {
                o.Text = "Start";
                this.frameRate.Stop();
            }
            else
            {
                o.Text = "Pause";
                this.frameRate.Start();
            }

            paused = !paused;
        }

        #endregion



        #region MatMaths

        private double[][] Vec_To_Mat(Vector v)
        {
            double[][] m = new double[3][];

            m[0] = new[] { v.X };
            m[1] = new[] { v.Y };
            m[2] = new[] { v.Z };

            return m;
        }


        private Vector Mat_To_vec(double[][] m)
        {
            Vector v = new Vector
            {
                X = m[0][0],
                Y = m[1][0]
            };

            if (m.Length > 2)
                v.Z = m[2][0];

            return v;
        }


        private Vector MatMul(double[][] a, Vector b)
        {
            double[][] m = Vec_To_Mat(b);
            return Mat_To_vec(MatMul(a, m));
        }


        private double[][] MatMul(double[][] a, double[][] b)
        {
            int colsA = a[0].Length;
            int rowsA = a.Length;
            int colsB = b[0].Length;
            int rowsB = b.Length;

            if (colsA != rowsB)
            {
                Console.WriteLine("Columns of A must match rows of B");
                return null;
            }

            double[][] result = new double[rowsA][];

            for (int i = 0; i < rowsA; i++)
            {
                for (int j = 0; j < colsB; j++)
                {
                    result[i] = new double[colsB];

                    double sum = 0.0f;
                    for (int k = 0; k < colsA; k++)
                    {
                        sum += a[i][k] * b[k][j];
                    }
                    result[i][j] = sum;
                }
            }
            return result;
        }

        #endregion
    }


    #region Operational Classes (Vectors, Angles etc)

    public partial class Angle
    {
        public double Sin { get; set; }
        public double Cos { get; set; }

        public Angle(double s, double c)
        {
            this.Sin = s; this.Cos = c;
        }
    }


    public partial class VerSector
    {
        public Vector[] Beam { get; set; }

        public VerSector()
        {
            this.Beam = new Vector[32];

            for (int i = 0; i < this.Beam.Length; i++)
                this.Beam[i] = new Vector();
        }
    }


    public partial class Vector
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }


        public Vector()
        {
            this.X = 0; this.Y = 0; this.Z = 0;
        }


        public Vector(double x, double y, double z)
        {
            this.X = x; this.Y = y; this.Z = z;
        }


        public void Mult(double d)
        {
            this.X *= d; this.Y *= d; this.Z *= d;
        }


        public void Move(PointF p)
        {
            this.X += p.X; this.Y += p.Y;
        }
    }

    #endregion
}
