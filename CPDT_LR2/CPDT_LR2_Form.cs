using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CPDT_LR2
{
    public partial class CPDT_LR2_Form : Form
    {
        private readonly VerSector[] pointsCloud;
        private readonly double[] shootingAngles;
        private readonly double[] verAngles;
        private readonly FileStream stream;
        private readonly Angle[] horAngles;
        private readonly float angleDelta;
        private readonly int pointSize;
        private bool paused, started;


        public CPDT_LR2_Form()
        {
            InitializeComponent();

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
                    verAngles[i] = Math.Sin(shootingAngles[i] * Math.PI / 180);

                horAngles[i] = new Angle(Math.Sin(i * Math.PI / 180),
                                         Math.Cos(i * Math.PI / 180));

                pointsCloud[i] = new VerSector();
            }

            angleDelta = 0.01f;
            started = false;
            paused = false;
            pointSize = 4;
        }


        private void CPDT_LR2_Form_Load(object sender, System.EventArgs e)
        {
            this.frameRate.Tick += (ss, ee) => { ReadData(); DrawData(); };

            this.Click += (ss, ee) => this.ActiveControl = null;

            this.numFramerate.ValueChanged += (ss, ee) =>
                this.frameRate.Interval = (int)this.numFramerate.Value;

            this.btnPp.Click += StartPause;
        }


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

            if (paused)
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
                                    float distance = (st[instance + 1] * 256 + st[instance]) * 0.002f;

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
        }


        private void DrawData()
        {
            Bitmap bmp = new Bitmap(this.frameIsometric.Width, this.frameIsometric.Height);

            for (int row = 0; row < pointsCloud.Length; row++)
            {
                for (int column = 0; column < pointsCloud[row].Beam.Length; column++)
                {
                    float x = pointsCloud[row].Beam[column].X;
                    float y = pointsCloud[row].Beam[column].Y;
                    float z = pointsCloud[row].Beam[column].Z;

                    Console.WriteLine($"{x}, {y}, {z}");



                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.FillEllipse(new SolidBrush(Color.FromArgb((int)z, 255, 0, 255)), x, y, pointSize, pointSize);
                    }
                }
            }

            this.frameIsometric.Image = bmp;
        }


        private float[][] Vec_To_Mat(Vector v)
        {
            float[][] m = new float[3][];

            m[0] = new[] { v.X };
            m[1] = new[] { v.Y };
            m[2] = new[] { v.Z };

            return m;
        }


        private Vector Mat_To_vec(float[][] m)
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


        private Vector MatMul(float[][] a, Vector b)
        {
            float[][] m = Vec_To_Mat(b);
            return Mat_To_vec(MatMul(a, m));
        }


        private float[][] MatMul(float[][] a, float[][] b)
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

            float[][] result = new float[rowsA][];

            for (int i = 0; i < rowsA; i++)
            {
                for (int j = 0; j < colsB; j++)
                {
                    result[i] = new float[colsB];

                    float sum = 0.0f;
                    for (int k = 0; k < colsA; k++)
                    {
                        sum += a[i][k] * b[k][j];
                    }
                    result[i][j] = sum;
                }
            }
            return result;
        }
    }


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
        private readonly CPDT_LR2_Form f = new CPDT_LR2_Form();

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }


        public Vector()
        {
            this.X = 0; this.Y = 0; this.Z = 0;
        }


        public Vector(float x, float y, float z)
        {
            this.X = x; this.Y = y; this.Z = z;
        }


        public void Mult(float d)
        {
            this.X *= d; this.Y *= d; this.Z *= d;
        }


        public void Move(PointF p)
        {
            this.X += p.X; this.Y += p.Y;
        }


        public int IsometricMap(float value, float fromLow, float fromHigh)
        {
            int toHigh = f.frameIsometric.Width;
            int toLow = 0;

            return (int)((value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow);
        }


        public int OverheadMap(float value, float fromLow, float fromHigh)
        {
            int toHigh = f.frameOverhead.Width;
            int toLow = 0;

            return (int)((value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow);
        }
    }
}
