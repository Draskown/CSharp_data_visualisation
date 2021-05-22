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

        private readonly Angle[] horAngles;

        private readonly VerSector[] pointsCloud;

        private readonly Vector[] arrayOfPlaneFront, drawablePlaneFront,
                                  arrayOfPlaneBack, drawablePlaneBack;

        private readonly FileStream stream;

        private readonly List<FoundObject> foundObjects;

        private PointF initialPoint;

        private readonly double[][] overheadRotationY, overheadRotationZ,
                                    overheadProjection, isometricProjection;

        private readonly double[] shootingAngles, verAngles;

        private readonly double angleDelta, allowedDistance, maximumDepth;
        private double isometricDistance, angleX, angleY, 
                       totalDepthFront, totalDepthBack;

        private readonly int pointSize, overheadDistance, planesAngle;

        private string count;

        private bool paused, started;


        public CPDT_LR2_Form()
        {
            InitializeComponent();

            foundObjects = new List<FoundObject>();

            started = paused = false;
            isometricDistance = 150;
            allowedDistance = 0.2;
            overheadDistance = 25;
            angleX = angleY = 0;
            angleDelta = 0.01;
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

            overheadRotationY = new double[][] {
                        new double[] { Math.Cos(Math.PI), 0, Math.Sin(Math.PI) },
                        new double[] { 0, 1, 0 },
                        new double[] { Math.Sin(Math.PI), 0, Math.Cos(Math.PI) }
            };

            overheadRotationZ = new double[][] {
                        new double[] { Math.Cos(Math.PI), -Math.Sin(Math.PI), 0},
                        new double[] { Math.Sin(Math.PI), Math.Cos(Math.PI), 0},
                        new double[] { 0, 0, 1}
            };

            overheadProjection = new double[][] {
                new double[] { 1, 0, 0 },
                new double[] { 0, 0, 0 },
                new double[] { 0, 0, 1 }
            };

            isometricProjection = new double[][] {
                    new double[] { 1, 0, 0 },
                    new double[] { 0, 1, 0 }
            };

            planesAngle = (int)numCorAngle.Value;

            maximumDepth = 10.0;
            totalDepthFront = maximumDepth;
            totalDepthBack = maximumDepth;

            arrayOfPlaneFront = new Vector[8];
            arrayOfPlaneBack = new Vector[8];

            for (int i = 0; i < arrayOfPlaneFront.Length; i++)
            {
                arrayOfPlaneFront[i] = new Vector();
                arrayOfPlaneBack[i] = new Vector();
            }

            drawablePlaneFront = new Vector[8];
            drawablePlaneBack = new Vector[8];
        }


        private void CPDT_LR2_Form_Load(object sender, System.EventArgs e)
        {
            this.frameRate.Interval = (int)this.numFramerate.Value;
            this.frameRate.Tick += (ss, ee) =>
            {
                ReadData();
                FindObjects();
                ComputeDepth();
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

            isometricDistance += e.Delta / 10;
            isometricDistance = isometricDistance < 0 ? 0 : isometricDistance;

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

            Graphics iG = Graphics.FromImage(bmpIsometric);
            Graphics oG = Graphics.FromImage(bmpOverhead);

            double[][] isometricRotationX = {
                        new double[] { 1, 0, 0},
                        new double[] { 0, Math.Cos(angleY), -Math.Sin(angleY)},
                        new double[] { 0, Math.Sin(angleY), Math.Cos(angleY)}
            };

            double[][] isometricRotationY = {
                        new double[] { Math.Cos(angleX), 0, Math.Sin(angleX)},
                        new double[] { 0, 1, 0 },
                        new double[] { Math.Sin(angleX), 0, Math.Cos(angleX)}
            };

            for (int row = 0; row < pointsCloud.Length; row++)
            {
                for (int column = 0; column < pointsCloud[row].Beams.Length; column++)
                {
                    var v = pointsCloud[row].Beams[column];

                    if (!CheckDrawability(v))
                        continue;

                    Vector rotated = MatMul(isometricRotationX, v);
                    rotated = MatMul(isometricRotationY, rotated);

                    Vector projected2d = MatMul(isometricProjection, rotated);

                    projected2d.Mult(isometricDistance);

                    projected2d.Move(this.frameIsometric.Width / 2, this.frameIsometric.Height / 2, 0);

                    int alpha = (int)((rotated.Z + 8.6) * 255 / 8.5 / 2);
                    alpha = alpha > 255 ? 255 : alpha < 50 ? 50 : alpha;

                    iG.FillEllipse(new SolidBrush(Color.FromArgb((int)alpha, 255, 0, 255)), (float)projected2d.X, (float)projected2d.Y, pointSize, pointSize);

                    rotated = MatMul(overheadRotationY, v);
                    rotated = MatMul(overheadRotationZ, rotated);

                    projected2d = MatMul(overheadProjection, rotated);

                    projected2d.Mult(overheadDistance);

                    projected2d.Move(175, 0, 200);

                    alpha = (int)((rotated.Z + 8.6) * 255 / 8.5 / 2);
                    alpha = alpha > 255 ? 255 : alpha < 50 ? 50 : alpha;

                    oG.FillEllipse(new SolidBrush(Color.FromArgb((int)alpha, 255, 0, 255)), (float)projected2d.X, (float)projected2d.Z, pointSize, pointSize);
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

                    Vector rotated = MatMul(isometricRotationX, v);
                    rotated = MatMul(isometricRotationY, rotated);

                    Vector projected2d = MatMul(isometricProjection, rotated);

                    projected2d.Mult(isometricDistance);

                    projected2d.Move(this.frameIsometric.Width / 2, this.frameIsometric.Height / 2, 0);

                    int alpha = (int)((rotated.Z + 8.6) * 255 / 8.5 / 2);
                    alpha = alpha > 255 ? 255 : alpha < 50 ? 50 : alpha;

                    iG.FillEllipse(new SolidBrush(Color.FromArgb((int)alpha, 0, 255, 0)), (float)projected2d.X, (float)projected2d.Y, pointSize, pointSize);

                    rotated = MatMul(overheadRotationY, v);
                    rotated = MatMul(overheadRotationZ, rotated);

                    projected2d = MatMul(overheadProjection, rotated);

                    projected2d.Mult(overheadDistance);

                    projected2d.Move(175, 0, 200);

                    alpha = (int)((rotated.Z + 8.6) * 255 / 8.5 / 2);
                    alpha = alpha > 255 ? 255 : alpha < 50 ? 50 : alpha;

                    oG.FillEllipse(new SolidBrush(Color.FromArgb((int)alpha, 0, 255, 0)), (float)projected2d.X, (float)projected2d.Z, pointSize, pointSize);
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

                var isometricMinMaxXYZ = minMaxXYZ;

                var overheadMinMaxXYZ = new Vector[]
                {
                    new Vector(minMaxXYZ[2]),
                    new Vector(minMaxXYZ[3]),
                    new Vector(minMaxXYZ[6]),
                    new Vector(minMaxXYZ[7])
                };

                for (int i = 0; i < isometricMinMaxXYZ.Length; i++)
                {
                    Vector rotated = MatMul(isometricRotationX, isometricMinMaxXYZ[i]);
                    rotated = MatMul(isometricRotationY, rotated);

                    Vector projected2d = MatMul(isometricProjection, rotated);

                    projected2d.Mult(isometricDistance);

                    projected2d.Move(this.frameIsometric.Width / 2, this.frameIsometric.Height / 2, 0);

                    isometricMinMaxXYZ[i].X = projected2d.X;
                    isometricMinMaxXYZ[i].Y = projected2d.Y;
                }

                for (int i = 0; i < 4; i++)
                {
                    iG.DrawLine(new Pen(Color.FromArgb(0, 0, 255)),
                             (float)isometricMinMaxXYZ[i].X, (float)isometricMinMaxXYZ[i].Y,
                             (float)isometricMinMaxXYZ[(i + 1) % 4].X, (float)isometricMinMaxXYZ[(i + 1) % 4].Y);

                    iG.DrawLine(new Pen(Color.FromArgb(0, 0, 255)),
                              (float)isometricMinMaxXYZ[i + 4].X, (float)isometricMinMaxXYZ[i + 4].Y,
                              (float)isometricMinMaxXYZ[((i + 1) % 4) + 4].X, (float)isometricMinMaxXYZ[((i + 1) % 4) + 4].Y);

                    iG.DrawLine(new Pen(Color.FromArgb(0, 0, 255)),
                              (float)isometricMinMaxXYZ[i].X, (float)isometricMinMaxXYZ[i].Y,
                              (float)isometricMinMaxXYZ[i + 4].X, (float)isometricMinMaxXYZ[i + 4].Y);
                }

                for (int i = 0; i < overheadMinMaxXYZ.Length; i++)
                {
                    Vector rotated = MatMul(overheadRotationY, overheadMinMaxXYZ[i]);
                    rotated = MatMul(overheadRotationZ, rotated);

                    Vector projected2d = MatMul(overheadProjection, rotated);

                    projected2d.Mult(overheadDistance);

                    projected2d.Move(175, 0, 200);

                    overheadMinMaxXYZ[i].X = projected2d.X;
                    overheadMinMaxXYZ[i].Z = projected2d.Z;
                }

                oG.DrawLine(new Pen(Color.FromArgb(0, 0, 255)),
                              (float)overheadMinMaxXYZ[0].X, (float)overheadMinMaxXYZ[0].Z,
                              (float)overheadMinMaxXYZ[1].X, (float)overheadMinMaxXYZ[1].Z);
                oG.DrawLine(new Pen(Color.FromArgb(0, 0, 255)),
                              (float)overheadMinMaxXYZ[3].X, (float)overheadMinMaxXYZ[3].Z,
                              (float)overheadMinMaxXYZ[1].X, (float)overheadMinMaxXYZ[1].Z);
                oG.DrawLine(new Pen(Color.FromArgb(0, 0, 255)),
                              (float)overheadMinMaxXYZ[3].X, (float)overheadMinMaxXYZ[3].Z,
                              (float)overheadMinMaxXYZ[2].X, (float)overheadMinMaxXYZ[2].Z);
                oG.DrawLine(new Pen(Color.FromArgb(0, 0, 255)),
                              (float)overheadMinMaxXYZ[0].X, (float)overheadMinMaxXYZ[0].Z,
                              (float)overheadMinMaxXYZ[2].X, (float)overheadMinMaxXYZ[2].Z);
            }

            var checkableArrayFront = arrayOfPlaneFront.Where(v => v.Distance > 0);

            if (!checkableArrayFront.All(v => v.X == 0.0 && v.Y == 0.0 && v.Z == 0.0) &&
                checkableArrayFront.All(v => CheckDrawability(v)))
            {
                for (int i = 0; i < arrayOfPlaneFront.Length; i++)
                {
                    var v = arrayOfPlaneFront[i];

                    Vector rotated = MatMul(isometricRotationX, v);
                    rotated = MatMul(isometricRotationY, rotated);

                    Vector projected2d = MatMul(isometricProjection, rotated);

                    projected2d.Mult(isometricDistance);

                    projected2d.Move(this.frameIsometric.Width / 2, this.frameIsometric.Height / 2, 0);

                    drawablePlaneFront[i] = projected2d;
                }

                var arrayOfPlanePoints = new PointF[4];

                for (int i = 4; i < drawablePlaneFront.Length; i++)
                    arrayOfPlanePoints[i - 4] = new PointF((float)drawablePlaneFront[i].X,
                                                       (float)drawablePlaneFront[i].Y);

                iG.FillClosedCurve(new SolidBrush(Color.FromArgb(100, 255, 255, 0)), arrayOfPlanePoints, System.Drawing.Drawing2D.FillMode.Winding, 0.1f);
            }

            var checkableArrayBack = arrayOfPlaneBack.Where(v => v.Distance > 0);

            if (!checkableArrayBack.All(v => v.X == 0.0 && v.Y == 0.0 && v.Z == 0.0) &&
                 checkableArrayBack.All(v => CheckDrawability(v)))
            {
                for (int i = 0; i < arrayOfPlaneBack.Length; i++)
                {
                    var v = arrayOfPlaneBack[i];

                    Vector rotated = MatMul(isometricRotationX, v);
                    rotated = MatMul(isometricRotationY, rotated);

                    Vector projected2d = MatMul(isometricProjection, rotated);

                    projected2d.Mult(isometricDistance);

                    projected2d.Move(this.frameIsometric.Width / 2, this.frameIsometric.Height / 2, 0);

                    drawablePlaneBack[i] = projected2d;
                }

                var arrayOfPlanePoints = new PointF[4];

                for (int i = 4; i < drawablePlaneBack.Length; i++)
                    arrayOfPlanePoints[i - 4] = new PointF((float)drawablePlaneBack[i].X,
                                                       (float)drawablePlaneBack[i].Y);

                iG.FillClosedCurve(new SolidBrush(Color.FromArgb(100, 255, 255, 0)), arrayOfPlanePoints, System.Drawing.Drawing2D.FillMode.Winding, 0.1f);
            }

            if (!checkableArrayFront.All(v => v.X == 0.0 && v.Y == 0.0 && v.Z == 0.0))
            {
                for (int i = 0; i < arrayOfPlaneFront.Length; i++)
                {
                    var v = arrayOfPlaneFront[i];

                    Vector rotated = MatMul(overheadRotationY, v);
                    rotated = MatMul(overheadRotationZ, rotated);

                    Vector projected2d = MatMul(overheadProjection, rotated);

                    projected2d.Mult(overheadDistance);

                    projected2d.Move(175, 0, 200);

                    drawablePlaneFront[i] = projected2d;
                }

                for (int i = 0; i < arrayOfPlaneBack.Length; i++)
                {
                    var v = arrayOfPlaneBack[i];

                    Vector rotated = MatMul(overheadRotationY, v);
                    rotated = MatMul(overheadRotationZ, rotated);

                    Vector projected2d = MatMul(overheadProjection, rotated);

                    projected2d.Mult(overheadDistance);

                    projected2d.Move(175, 0, 200);

                    drawablePlaneBack[i] = projected2d;
                }

                var arrayOfPlanePoints = new PointF[4];

                if (!checkableArrayFront.All(v => CheckDrawability(v)))
                {
                    arrayOfPlanePoints[0] = new PointF((float)drawablePlaneBack[7].X, (float)drawablePlaneBack[7].Z);
                    arrayOfPlanePoints[1] = new PointF((float)drawablePlaneBack[6].X, (float)drawablePlaneBack[6].Z);
                    arrayOfPlanePoints[2] = new PointF((float)drawablePlaneBack[3].X, (float)drawablePlaneBack[3].Z);
                    arrayOfPlanePoints[3] = new PointF((float)drawablePlaneBack[2].X, (float)drawablePlaneBack[2].Z);
                }
                else if (!checkableArrayBack.All(v => CheckDrawability(v)))
                {
                    arrayOfPlanePoints[2] = new PointF((float)drawablePlaneFront[3].X, (float)drawablePlaneFront[3].Z);
                    arrayOfPlanePoints[3] = new PointF((float)drawablePlaneFront[2].X, (float)drawablePlaneFront[2].Z);
                    arrayOfPlanePoints[2] = new PointF((float)drawablePlaneFront[6].X, (float)drawablePlaneFront[6].Z);
                    arrayOfPlanePoints[3] = new PointF((float)drawablePlaneFront[7].X, (float)drawablePlaneFront[7].Z);
                }
                else
                {
                    arrayOfPlanePoints[0] = new PointF((float)drawablePlaneBack[6].X, (float)drawablePlaneBack[6].Z);
                    arrayOfPlanePoints[1] = new PointF((float)drawablePlaneBack[7].X, (float)drawablePlaneBack[7].Z);
                    arrayOfPlanePoints[2] = new PointF((float)drawablePlaneFront[7].X, (float)drawablePlaneFront[7].Z);
                    arrayOfPlanePoints[3] = new PointF((float)drawablePlaneFront[6].X, (float)drawablePlaneFront[6].Z);
                }

                oG.FillClosedCurve(new SolidBrush(Color.FromArgb(100, 255, 255, 0)), arrayOfPlanePoints, System.Drawing.Drawing2D.FillMode.Winding, 0.1f);
            }

            iG.Dispose(); oG.Dispose();

            this.frameIsometric.Image = bmpIsometric;
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
                (_v.Distance <= allowedDistance))
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

                            fo.Vectors.Add(new Vector(comparablePoint));

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



        #region Computing Corridor's Depth

        private void ComputeDepth()
        {
            var width = (double)numCorWidth.Value;
            var height = (double)numCorHeight.Value;

            Vector v1, v2, v3, v4, v1EndFront, v2EndFront, v3EndFront, v4EndFront,
                   v1EndBack, v2EndBack, v3EndBack, v4EndBack;

            Vector startingPoint = new Vector();

            v1 = new Vector(startingPoint,
                            (double)startingPoint.X + width * Math.Cos(-planesAngle * Math.PI / 180),
                            (double)startingPoint.Y - height,
                            (double)startingPoint.Z + width * Math.Sin(-planesAngle * Math.PI / 180));
            v2 = new Vector(startingPoint,
                            (double)startingPoint.X - width * Math.Cos(-planesAngle * Math.PI / 180),
                            (double)startingPoint.Y - height,
                            (double)startingPoint.Z - width * Math.Sin(-planesAngle * Math.PI / 180));
            v3 = new Vector(startingPoint,
                            (double)startingPoint.X + width * Math.Cos(-planesAngle * Math.PI / 180),
                            (double)startingPoint.Y + height,
                            (double)startingPoint.Z + width * Math.Sin(-planesAngle * Math.PI / 180));
            v4 = new Vector(startingPoint,
                            (double)startingPoint.X - width * Math.Cos(-planesAngle * Math.PI / 180),
                            (double)startingPoint.Y + height,
                            (double)startingPoint.Z - width * Math.Sin(-planesAngle * Math.PI / 180));

            v1EndFront = new Vector(v1); v2EndFront = new Vector(v2);
            v3EndFront = new Vector(v3); v4EndFront = new Vector(v4);
            v1EndBack = new Vector(v1); v2EndBack = new Vector(v2);
            v3EndBack = new Vector(v3); v4EndBack = new Vector(v4);

            if (foundObjects.Count > 0)
            {
                foreach (var obj in foundObjects)
                {
                    var depthFront = 0.0;
                    var depthBack = 0.0;
                    var threshForCountingAsAnObstacleFront = 0;
                    var threshForCountingAsAnObstacleBack = 0;

                    foreach (var v in obj.Vectors)
                    {
                        if (v.X == v.Y && v.X == v.Z && v.X == v.Distance)
                            continue;

                        while (depthFront < maximumDepth)
                        {
                            v1EndFront.X = v1.X + depthFront * Math.Sin(planesAngle * Math.PI / 180);
                            v1EndFront.Z = v1.Z + depthFront * Math.Cos(planesAngle * Math.PI / 180);
                            v2EndFront.X = v2.X + depthFront * Math.Sin(planesAngle * Math.PI / 180);
                            v2EndFront.Z = v2.Z + depthFront * Math.Cos(planesAngle * Math.PI / 180);
                            v3EndFront.X = v3.X + depthFront * Math.Sin(planesAngle * Math.PI / 180);
                            v3EndFront.Z = v3.Z + depthFront * Math.Cos(planesAngle * Math.PI / 180);
                            v4EndFront.X = v4.X + depthFront * Math.Sin(planesAngle * Math.PI / 180);
                            v4EndFront.Z = v4.Z + depthFront * Math.Cos(planesAngle * Math.PI / 180);

                            var minXFront = Min(v1.X, v2.X, v1EndFront.X, v2EndFront.X);
                            var maxXFront = Max(v1.X, v2.X, v1EndFront.X, v2EndFront.X);
                            var minZFront = Min(v1.Z, v2.Z, v1EndFront.Z, v2EndFront.Z);
                            var maxZFront = Max(v1.Z, v2.Z, v1EndFront.Z, v2EndFront.Z);

                            if (depthFront >= 1.35)
                                Console.WriteLine();

                            if (v.X > minXFront && v.X < maxXFront)
                                if (v.Y > -height && v.Y < height)
                                    if (v.Z > minZFront && v.Z < maxZFront)
                                        if ((int)(v.Distance - depthFront) == 0)
                                        {
                                            threshForCountingAsAnObstacleFront++;
                                            if (threshForCountingAsAnObstacleFront >= 10)
                                            {
                                                totalDepthFront = depthFront < totalDepthFront ? depthFront : totalDepthFront;
                                                break;
                                            }
                                        }

                            depthFront += 0.01;
                        }

                        while (depthBack < maximumDepth)
                        {
                            v1EndBack.X = v1.X + depthBack * Math.Sin((planesAngle + 180) * Math.PI / 180);
                            v1EndBack.Z = v1.Z + depthBack * Math.Cos((planesAngle + 180) * Math.PI / 180);
                            v2EndBack.X = v2.X + depthBack * Math.Sin((planesAngle + 180) * Math.PI / 180);
                            v2EndBack.Z = v2.Z + depthBack * Math.Cos((planesAngle + 180) * Math.PI / 180);
                            v3EndBack.X = v3.X + depthBack * Math.Sin((planesAngle + 180) * Math.PI / 180);
                            v3EndBack.Z = v3.Z + depthBack * Math.Cos((planesAngle + 180) * Math.PI / 180);
                            v4EndBack.X = v4.X + depthBack * Math.Sin((planesAngle + 180) * Math.PI / 180);
                            v4EndBack.Z = v4.Z + depthBack * Math.Cos((planesAngle + 180) * Math.PI / 180);

                            var minXBack = Min(v1.X, v2.X, v1EndBack.X, v2EndBack.X);
                            var maxXBack = Max(v1.X, v2.X, v1EndBack.X, v2EndBack.X);
                            var minZBack = Min(v1.Z, v2.Z, v1EndBack.X, v2EndBack.Z);
                            var maxZBack = Max(v1.Z, v2.Z, v1EndBack.X, v2EndBack.Z);

                            if (v.X > minXBack && v.X < maxXBack)
                                if (v.Y > -height && v.Y < height)
                                    if (v.Z > minZBack && v.Z < maxZBack)
                                        if ((int)(v.Distance - depthBack) == 0)
                                        {
                                            threshForCountingAsAnObstacleBack++;
                                            if (threshForCountingAsAnObstacleBack >= 10)
                                            {
                                                totalDepthBack = depthBack < totalDepthBack ? depthBack : totalDepthBack;
                                                break;
                                            }
                                        }

                            depthBack += 0.01;
                        }
                    }
                }
            }
            else
            {
                totalDepthFront = maximumDepth;
                totalDepthBack = maximumDepth;
            }

            v1EndFront.X = v1.X + totalDepthFront * Math.Sin(planesAngle * Math.PI / 180);
            v1EndFront.Z = v1.Z + totalDepthFront * Math.Cos(planesAngle * Math.PI / 180);
            v1EndFront.Distance = totalDepthFront;
            v1EndFront.Azimut = (int)(Math.Atan(width / totalDepthFront) * 180 / Math.PI);

            v2EndFront.X = v2.X + totalDepthFront * Math.Sin(planesAngle * Math.PI / 180);
            v2EndFront.Z = v2.Z + totalDepthFront * Math.Cos(planesAngle * Math.PI / 180);
            v2EndFront.Distance = totalDepthFront;
            v2EndFront.Azimut = (int)(360 - Math.Atan(width / totalDepthFront) * 180 / Math.PI);

            v3EndFront.X = v3.X + totalDepthFront * Math.Sin(planesAngle * Math.PI / 180);
            v3EndFront.Z = v3.Z + totalDepthFront * Math.Cos(planesAngle * Math.PI / 180);
            v3EndFront.Distance = totalDepthFront;
            v3EndFront.Azimut = (int)(360 - Math.Atan(width / totalDepthFront) * 180 / Math.PI);
            v3EndFront.Beam = (int)(height / 0.3125);

            v4EndFront.X = v4.X + totalDepthFront * Math.Sin(planesAngle * Math.PI / 180);
            v4EndFront.Z = v4.Z + totalDepthFront * Math.Cos(planesAngle * Math.PI / 180);
            v4EndFront.Distance = totalDepthFront;
            v4EndFront.Azimut = (int)(Math.Atan(width / totalDepthFront) * 180 / Math.PI);
            v4EndFront.Beam = (int)(height / 0.3125);

            v1EndBack.X = v1.X + totalDepthBack * Math.Sin((planesAngle + 180) * Math.PI / 180);
            v1EndBack.Z = v1.Z + totalDepthBack * Math.Cos((planesAngle + 180) * Math.PI / 180);
            v1EndBack.Distance = totalDepthBack;
            v1EndBack.Azimut = (int)(180 + Math.Atan(width / totalDepthBack) * 180 / Math.PI);

            v2EndBack.X = v2.X + totalDepthBack * Math.Sin((planesAngle + 180) * Math.PI / 180);
            v2EndBack.Z = v2.Z + totalDepthBack * Math.Cos((planesAngle + 180) * Math.PI / 180);
            v2EndBack.Distance = totalDepthBack;
            v2EndBack.Azimut = (int)(180 - Math.Atan(width / totalDepthBack) * 180 / Math.PI);

            v3EndBack.X = v3.X + totalDepthBack * Math.Sin((planesAngle + 180) * Math.PI / 180);
            v3EndBack.Z = v3.Z + totalDepthBack * Math.Cos((planesAngle + 180) * Math.PI / 180);
            v3EndBack.Distance = totalDepthBack;
            v3EndBack.Azimut = (int)(180 - Math.Atan(width / totalDepthBack) * 180 / Math.PI);
            v3EndBack.Beam = (int)(height / 0.3125);

            v4EndBack.X = v4.X + totalDepthBack * Math.Sin((planesAngle + 180) * Math.PI / 180);
            v4EndBack.Z = v4.Z + totalDepthBack * Math.Cos((planesAngle + 180) * Math.PI / 180);
            v4EndBack.Distance = totalDepthBack;
            v4EndBack.Azimut = (int)(180 + Math.Atan(width / totalDepthBack) * 180 / Math.PI);
            v4EndBack.Beam = (int)(height / 0.3125);

            arrayOfPlaneFront[0] = v1;
            arrayOfPlaneFront[1] = v2;
            arrayOfPlaneFront[2] = v3;
            arrayOfPlaneFront[3] = v4;
            arrayOfPlaneFront[4] = v1EndFront;
            arrayOfPlaneFront[5] = v2EndFront;
            arrayOfPlaneFront[6] = v4EndFront;
            arrayOfPlaneFront[7] = v3EndFront;

            arrayOfPlaneBack[0] = v1;
            arrayOfPlaneBack[1] = v2;
            arrayOfPlaneBack[2] = v3;
            arrayOfPlaneBack[3] = v4;
            arrayOfPlaneBack[4] = v1EndBack;
            arrayOfPlaneBack[5] = v2EndBack;
            arrayOfPlaneBack[6] = v4EndBack;
            arrayOfPlaneBack[7] = v3EndBack;
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



        #region Math Methods

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


        private double Max(double a, double b, double c, double d)
        {
            double minimumValue = a;

            if (minimumValue < b)
                minimumValue = b;
            if (minimumValue < c)
                minimumValue = c;
            if (minimumValue < d)
                minimumValue = d;

            return minimumValue;
        }


        private double Min(double a, double b, double c, double d)
        {
            double minimumValue = a;

            if (minimumValue > b)
                minimumValue = b;
            if (minimumValue > c)
                minimumValue = c;
            if (minimumValue > d)
                minimumValue = d;

            return minimumValue;
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

        public Vector(Vector _v)
        {
            this.X = _v.X; this.Y = _v.Y; this.Z = _v.Z;
            this.IsInObject = _v.IsInObject;
            this.Distance = _v.Distance;
            this.Azimut = _v.Azimut;
            this.Beam = _v.Beam;
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

        public void Move(int deltaX, int deltaY, int deltaZ)
        {
            this.X += deltaX; this.Y += deltaY; this.Z += deltaZ;
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
