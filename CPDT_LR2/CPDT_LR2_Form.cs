using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CPDT_LR2
{
    public partial class CPDT_LR2_Form : Form
    {
        #region Initialization and events

        private readonly List<FoundObject> foundObjects;

        private readonly Angle[] horAngles;
        private readonly VerSector[] pointsCloud;
        private readonly FileStream stream;

        private PointF initialPoint;

        private readonly double[] shootingAngles, verAngles;
        private readonly double angleDelta;
        private double distance, angleX, angleY, allowedDistance;

        private readonly int pointSize;
        private string count;

        private bool paused, started;


        public CPDT_LR2_Form()
        {
            InitializeComponent();

            foundObjects = new List<FoundObject>();

            started = paused = false;
            allowedDistance = 0.2;
            angleX = angleY = 0;
            angleDelta = 0.01;
            distance = 150;
            pointSize = 2;

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
            this.frameRate.Interval = (int)this.numFramerate.Value;
            this.frameRate.Tick += (ss, ee) =>
            {
                ReadData();
                FindObjects();
                DrawData();
            };

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
            byte[] line = new byte[300 * 360];

            var pointsCount = 0;
            var maxLengthOfAMessage = $"23:23:23> received {line.Length} points".Length;

            if (stream.Position == 8807418)
                stream.Position = 0;

            stream.Read(line, 0, line.Length);
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

                                    pointsCloud[(int)azimut].Beams[laserId] = new Vector(distance * (float)horAngles[(int)azimut].Sin,
                                                                                            distance * (float)verAngles[laserId],
                                                                                            distance * (float)horAngles[(int)azimut].Cos,
                                                                                            distance, (int)azimut, laserId);

                                    laserId++;
                                    pointsCount++;
                                }
                            }

                            st.Clear();
                        }
                    }
                }
                else
                    st.Add(line[i]);
            }

            foundObjects.Clear();

            if (pointsCount > 10000)
                count = pointsCount.ToString();
            else if (pointsCount > 1000)
                count = $"0{pointsCount}";
            else if (pointsCount > 100)
                count = $"00{pointsCount}";
            else if (pointsCount > 10)
                count = $"000{pointsCount}";
            else
                count = $"0000{pointsCount}";

            if (this.boxLog.Text.Length > maxLengthOfAMessage * 100)
                this.boxLog.Text = "";

            var message = DateTime.Now.ToString("HH:mm:ss") + $"> received {count} points\r\n";

            this.boxLog.Text += message;
        }

        #endregion



        #region Drawing

        private void DrawData()
        {
            Bitmap bmpIsometric = new Bitmap(this.frameIsometric.Width, this.frameIsometric.Height);
            Bitmap bmpOverhead = new Bitmap(this.frameOverhead.Width, this.frameOverhead.Height);

            double[][] rotationX = {
                        new double[] { 1, 0, 0},
                        new double[] { 0, Math.Cos(angleY), -Math.Sin(angleY)},
                        new double[] { 0, Math.Sin(angleY), Math.Cos(angleY)}
            };

            double[][] rotationY = {
                        new double[] { Math.Cos(angleX), 0, Math.Sin(angleX)},
                        new double[] { 0, 1, 0 },
                        new double[] { Math.Sin(angleX), 0, Math.Cos(angleX)}
            };

            double[][] projection = {
                    new double[] { 1, 0, 0 },
                    new double[] { 0, 1, 0 }
            };

            using (Graphics g = Graphics.FromImage(bmpIsometric))
            {
                for (int row = 0; row < pointsCloud.Length; row++)
                {
                    for (int column = 0; column < pointsCloud[row].Beams.Length; column++)
                    {
                        var v = pointsCloud[row].Beams[column];

                        if (!CheckDrawability(v))
                            continue;

                        Vector rotated = MatMul(rotationX, v);
                        rotated = MatMul(rotationY, rotated);

                        Vector projected2d = MatMul(projection, rotated);

                        projected2d.Mult(distance);

                        projected2d.Move(new PointF(this.frameIsometric.Width / 2, this.frameIsometric.Height / 2));

                        int alpha = (int)((rotated.Z + 8.6) * 255 / 8.5 / 2);
                        alpha = alpha > 255 ? 255 : alpha < 50 ? 50 : alpha;

                        g.FillEllipse(new SolidBrush(Color.FromArgb((int)alpha, 255, 0, 255)), (float)projected2d.X, (float)projected2d.Y, pointSize, pointSize);
                    }
                }

                foreach (var obj in foundObjects)
                {
                    if (!CheckDrawability(obj.Centroid))
                        continue;

                    foreach (var v in obj.Vectors)
                    {
                        if (!CheckDrawability(v))
                            continue;

                        Vector rotated = MatMul(rotationX, v);
                        rotated = MatMul(rotationY, rotated);

                        Vector projected2d = MatMul(projection, rotated);

                        projected2d.Mult(distance);

                        projected2d.Move(new PointF(this.frameIsometric.Width / 2, this.frameIsometric.Height / 2));

                        int alpha = (int)((rotated.Z + 8.6) * 255 / 8.5 / 2);
                        alpha = alpha > 255 ? 255 : alpha < 50 ? 50 : alpha;

                        g.FillEllipse(new SolidBrush(Color.FromArgb((int)alpha, 0, 255, 0)), (float)projected2d.X, (float)projected2d.Y, pointSize, pointSize);
                    }

                    var minX = obj.Vectors.Min(v => v.X);
                    var maxX = obj.Vectors.Max(v => v.X);
                    var minY = obj.Vectors.Min(v => v.Y);
                    var maxY = obj.Vectors.Max(v => v.Y);
                    var minZ = obj.Vectors.Min(v => v.Z);
                    var maxZ = obj.Vectors.Max(v => v.Z);

                    var minMaxXYZ = new Vector[] {
                        new Vector(obj.Centroid, minX, minY, minZ),
                        new Vector(obj.Centroid, maxX, minY, minZ),
                        new Vector(obj.Centroid, maxX, maxY, minZ),
                        new Vector(obj.Centroid, minX, maxY, minZ),
                        new Vector(obj.Centroid, minX, minY, maxZ),
                        new Vector(obj.Centroid, maxX, minY, maxZ),
                        new Vector(obj.Centroid, maxX, maxY, maxZ),
                        new Vector(obj.Centroid, minX, maxY, maxZ)
                    };

                    for (int i = 0; i < minMaxXYZ.Length; i++)
                    {
                        Vector rotated = MatMul(rotationX, minMaxXYZ[i]);
                        rotated = MatMul(rotationY, rotated);

                        Vector projected2d = MatMul(projection, rotated);

                        projected2d.Mult(distance);

                        projected2d.Move(new PointF(this.frameIsometric.Width / 2, this.frameIsometric.Height / 2));

                        minMaxXYZ[i].X = projected2d.X;
                        minMaxXYZ[i].Y = projected2d.Y;
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        g.DrawLine(new Pen(Color.FromArgb(0, 0, 255)),
                                  (float)minMaxXYZ[i].X, (float)minMaxXYZ[i].Y,
                                  (float)minMaxXYZ[(i + 1) % 4].X, (float)minMaxXYZ[(i + 1) % 4].Y);

                        g.DrawLine(new Pen(Color.FromArgb(0, 0, 255)),
                                  (float)minMaxXYZ[i + 4].X, (float)minMaxXYZ[i + 4].Y,
                                  (float)minMaxXYZ[((i + 1) % 4) + 4].X, (float)minMaxXYZ[((i + 1) % 4) + 4].Y);

                        g.DrawLine(new Pen(Color.FromArgb(0, 0, 255)),
                                  (float)minMaxXYZ[i].X, (float)minMaxXYZ[i].Y,
                                  (float)minMaxXYZ[i + 4].X, (float)minMaxXYZ[i + 4].Y);
                    }
                }
            }

            this.frameIsometric.Image = bmpIsometric;

            rotationY = new double[][] {
                        new double[] { Math.Cos(Math.PI), 0, Math.Sin(Math.PI) },
                        new double[] { 0, 1, 0 },
                        new double[] { Math.Sin(Math.PI), 0, Math.Cos(Math.PI) }
            };

            double[][] rotationZ = {
                        new double[] { Math.Cos(Math.PI), -Math.Sin(Math.PI), 0},
                        new double[] { Math.Sin(Math.PI), Math.Cos(Math.PI), 0},
                        new double[] { 0, 0, 1}
            };

            projection = new double[][] {
                new double[] { 1, 0, 0 },
                new double[] { 0, 0, 0 },
                new double[] { 0, 0, 1 }
            };

            using (Graphics g = Graphics.FromImage(bmpOverhead))
            {
                for (int row = 0; row < pointsCloud.Length; row++)
                {
                    for (int column = 0; column < pointsCloud[row].Beams.Length; column++)
                    {
                        var v = pointsCloud[row].Beams[column];

                        if (!CheckDrawability(v))
                            continue;

                        Vector rotated = MatMul(rotationY, v);
                        rotated = MatMul(rotationZ, rotated);

                        Vector projected2d = MatMul(projection, rotated);

                        projected2d.Mult(25);

                        int alpha = (int)((rotated.Z + 8.6) * 255 / 8.5 / 2);
                        alpha = alpha > 255 ? 255 : alpha < 50 ? 50 : alpha;

                        g.FillEllipse(new SolidBrush(Color.FromArgb((int)alpha, 255, 0, 255)), (float)projected2d.X + 175, (float)projected2d.Z + 200, pointSize, pointSize);
                    }
                }
            }

            this.frameOverhead.Image = bmpOverhead;
        }

        private bool CheckDrawability(Vector _v)
        {
            double xyPlane = (double)this.numXYPlane.Value,
                   xzPlane = (double)this.numXZPlane.Value,
                   yzPlane = (double)this.numYZPlane.Value;

            int row = _v.Azimut,
                column = _v.Beam;

            if ((_v.X > yzPlane || _v.X < -yzPlane) ||
                (_v.Y > xzPlane || _v.Y < -xzPlane) ||
                (_v.Z > xyPlane || _v.Z < -xyPlane) ||
                row < 180 - (int)this.numFOVHor.Value / 2 ||
                row > 180 + (int)this.numFOVHor.Value / 2 ||
                column > (int)this.numFOVVer.Value ||
                (_v.X <= allowedDistance && _v.Y <= allowedDistance && _v.Z <= allowedDistance))
                return false;
            else
                return true;
        }

        #endregion



        #region Finding Objects

        private void FindObjects()
        {
            Random r = new Random();

            for (int k = 0; k < (int)numK.Value; k++)
                foundObjects.Add(new FoundObject(pointsCloud[r.Next(0, pointsCloud.Length)].Beams[r.Next(0, 32)]));

            var temp = foundObjects;

            for (int sector = 0; sector < pointsCloud.Length; sector++)
            {
                for (int beam = 0; beam < 32; beam++)
                {
                    var comparablePoint = pointsCloud[sector].Beams[beam];

                    if (!CheckDrawability(comparablePoint) ||
                        comparablePoint.IsInObject)
                        continue;

                    var minDistance = double.MaxValue;

                    for (int k = 0; k < (int)numK.Value; k++)
                    {
                        var fo = foundObjects[k];

                        var distance = Math.Sqrt(Math.Pow(Math.Abs(fo.Centroid.X - comparablePoint.X), 2) +
                                                Math.Pow(Math.Abs(fo.Centroid.Y - comparablePoint.Y), 2) +
                                                Math.Pow(Math.Abs(fo.Centroid.Z - comparablePoint.Z), 2));

                        if (distance < minDistance)
                        {
                            minDistance = distance;

                            fo.Vectors.Add(comparablePoint);
                            comparablePoint.IsInObject = true;
                            fo.RecomputeCentroid();
                        }
                    }
                }
            }

            if (foundObjects.Count > 0)
                foundObjects.RemoveAll(o =>
                {
                    var maxX = o.Vectors.Max(v => v.X);
                    var minY = o.Vectors.Min(v => v.Y);
                    var maxY = o.Vectors.Max(v => v.Y);
                    var minZ = o.Vectors.Min(v => v.Z);
                    var maxZ = o.Vectors.Max(v => v.Z);
                    var minX = o.Vectors.Min(v => v.X);

                    if (o.Vectors.Count < (int)this.numMinDensote.Value ||
                        o.Vectors.Count > (int)this.numMaxDensote.Value ||
                        maxX - minX > (double)numMaxRadius.Value ||
                        maxX - minX < (double)numMinRadius.Value ||
                        maxY - minY > (double)numMaxRadius.Value ||
                        maxY - minY < (double)numMinRadius.Value ||
                        maxZ - minZ > (double)numMaxRadius.Value ||
                        maxZ - minZ < (double)numMinRadius.Value)
                        return true;
                    else
                        return false;
                });
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
                o.Text = "Resume";
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
        public Vector[] Beams { get; set; }

        public VerSector()
        {
            this.Beams = new Vector[32];

            for (int i = 0; i < this.Beams.Length; i++)
                this.Beams[i] = new Vector();
        }
    }


    public partial class Vector
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Distance { get; set; }
        public int Azimut { get; set; }
        public int Beam { get; set; }
        public bool IsInObject { get; set; }


        public Vector()
        {
            this.X = 0; this.Y = 0; this.Z = 0;
            this.IsInObject = false;
            this.Distance = 0;
            this.Azimut = 0;
            this.Beam = 0;
        }

        public Vector(Vector c, double x, double y, double z)
        {
            this.X = x; this.Y = y; this.Z = z;
            this.IsInObject = c.IsInObject;
            this.Distance = c.Distance;
            this.Azimut = c.Azimut;
            this.Beam = c.Beam;
        }

        public Vector(double x, double y, double z, double d, int a, int b)
        {
            this.X = x; this.Y = y; this.Z = z;
            this.IsInObject = false;
            this.Distance = d;
            this.Azimut = a;
            this.Beam = b;
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


    public partial class FoundObject
    {
        public List<Vector> Vectors { get; set; }
        public Vector Centroid { get; set; }

        public FoundObject(Vector p)
        {
            this.Vectors = new List<Vector> { p };
            this.Vectors.Last().IsInObject = true;
            this.Centroid = p;
        }

        public void RecomputeCentroid()
        {
            var x = this.Vectors.Sum(vs => vs.X) / this.Vectors.Count;
            var y = this.Vectors.Sum(vs => vs.Y) / this.Vectors.Count;
            var z = this.Vectors.Sum(vs => vs.Z) / this.Vectors.Count;
            var d = this.Vectors.Sum(vs => vs.Distance) / this.Vectors.Count;
            int a = (int)(this.Vectors.Sum(vs => vs.Azimut) / this.Vectors.Count);
            int b = (int)(this.Vectors.Sum(vs => vs.Beam) / this.Vectors.Count);

            this.Centroid = new Vector(x, y, z, d, a, b);
        }
    }

    #endregion
}
