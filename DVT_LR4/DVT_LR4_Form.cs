using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DVT_LR4
{
    public partial class DVT_LR4_Form : Form
    {
        private readonly FileStream stream;

        private readonly List<float> valuesX, valuesY, valuesZ;

        private readonly List<Point3F> points;

        private readonly OpenGL GL;

        private PointF initPoint, angle, delta;

        private float[] avgX, avgY, avgZ;

        private readonly float angleDelta;
        private float distance;

        private bool firstMessage;

        private Int16 startMs;

        public DVT_LR4_Form()
        {
            InitializeComponent();

            stream = new FileStream("dump.dmp", FileMode.Open);

            GL = graphMain.OpenGL;

            valuesX = new List<float>();
            valuesY = new List<float>();
            valuesZ = new List<float>();

            initPoint = new PointF();
            angle = new PointF();
            delta = new PointF(-6.6f, -9.4f);

            points = new List<Point3F>();

            angleDelta = 0.2f;
            distance = 24;

            firstMessage = true;

            ReadData();
        }


        private void DVT_LR4_Form_Load(object sender, EventArgs e)
        {
            this.Click += (ss, ee) => this.ActiveControl = null;

            this.graphMain.MouseDown += (ss, ee) => initPoint = ee.Location;
            this.graphMain.MouseWheel += ChangeDistance;
            this.graphMain.MouseMove += ChangePosition;

            this.graphMain.OpenGLDraw += Draw;

            this.btnSave.Click += SaveImage;
        }


        private void ReadData()
        {
            do
            {
                byte[] message = new byte[19];

                if (stream.Position == 19000)
                    break;

                stream.Read(message, 0, message.Length);

                var str = $"0x{message[6]:X2} -> 0x{message[7]:X2}";

                var ms = BitConverter.ToInt16(new byte[] { message[0], message[1] }, 0);

                if (firstMessage)
                {
                    startMs = ms;
                    firstMessage = false;
                }

                if (ms - startMs <= (int)this.numLength.Value)
                {
                    if (str == labelX.Text)
                        valuesX.Add(message[9]);
                    if (str == labelY.Text)
                        valuesY.Add(message[9]);
                    if (str == labelZ.Text)
                        valuesZ.Add(message[9]);
                }
                else
                {
                    if (str == labelX.Text)
                        valuesX.Add(0);
                    if (str == labelY.Text)
                        valuesY.Add(0);
                    if (str == labelZ.Text)
                        valuesZ.Add(0);
                }

                startMs = ms; 
            } 
            while (true);

            for (int i = 0; i < valuesX.Count; i++)
            {
                if (points.Any(p => float.Equals(p.X, valuesX[i] / 10.0f) &&
                                    p.Y == valuesY[i] / 10.0f &&
                                    p.Z == valuesZ[i] / 10.0f))
                {
                    valuesX.RemoveAt(i);
                    valuesY.RemoveAt(i);
                    valuesZ.RemoveAt(i);
                    i = 0;
                    continue;
                }

                points.Add(new Point3F(valuesX[i] / 10.0f, valuesY[i] / 10.0f, valuesZ[i] / 10.0f));
            }

            avgX = new float[valuesX.Count];
            avgY = new float[valuesY.Count];
            avgZ = new float[valuesZ.Count];

            CalculateAvgs(valuesX, "X");
            CalculateAvgs(valuesZ, "Z");

            RecalculateY();
        }


        private void CalculateAvgs(List<float> lst, string key)
        {
            if (lst.Count == 0)
                return;

            for (int j = 1; j < lst.Count; j++)
            {
                var depth = (int)numDepth.Value;
                var sum = 0.0f;
                var count = 0;

                for (int i = lst.Count - j - 1; i >= 0; i--)
                {
                    if (lst.Count - j - i > depth)
                        continue;

                    sum += lst[i];
                    count++;
                }

                switch (key)
                {
                    case "X":
                        avgX[avgX.Length - j] = sum / count;
                        break;
                    case "Y":
                        avgY[avgY.Length - j] = sum / count;
                        break;
                    case "Z":
                        avgZ[avgZ.Length - j] = sum / count;
                        break;
                } 
            }
        }


        private void RecalculateY()
        {
            var minX = valuesX.Min();
            var maxX = valuesX.Max();
            var minZ = valuesZ.Min();
            var maxZ = valuesZ.Max();

            var xDelta = (maxX - minX) / 10.0f;
            var xOffset = 0 - minX / xDelta;
            var zDelta = (maxZ - minZ) / 10.0f;
            var zOffset = 0 - minZ / zDelta;

            var ys = new int[10, 10];

            for (int k = 0; k < valuesX.Count; k++)
            {
                int xIndex = (int)Math.Round(valuesX[k] / xDelta + xOffset);
                int zIndex = (int)Math.Round(valuesZ[k] / zDelta + zOffset);

                for (int i = 0; i < 10; i++)
                    if (xIndex == i)
                        for (int j = 0; j < 10; j++)
                            if (zIndex == j)
                                ys[xIndex, zIndex]++;
            }

            for (int k = 0; k < valuesX.Count; k++)
            {
                int xIndex = (int)Math.Round(valuesX[k] / xDelta + xOffset);
                int zIndex = (int)Math.Round(valuesZ[k] / zDelta + zOffset);

                for (int i = 0; i < 10; i++)
                    if (xIndex == i)
                        for (int j = 0; j < 10; j++)
                            if (zIndex == j)
                                valuesY[k] = ys[xIndex, zIndex] / 10.0f;
            }

            for (int i = 0; i < points.Count; i++)
                points[i].Y = valuesY[i] / 10.0f;
        }


        private void Draw(object sender, RenderEventArgs args)
        {
            if (valuesX.Count == 0 || valuesY.Count == 0 || valuesZ.Count == 0)
                return;

            GL.Enable(OpenGL.GL_BLEND);
            GL.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            GL.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            GL.LoadIdentity();

            GL.Translate(delta.X, delta.Y, -distance);
            GL.Rotate(angle.Y, angle.X, 0.0f);

            GL.PointSize(2.0f);
            GL.Begin(OpenGL.GL_POINTS);
            GL.Color(1.0f, 1.0f, 1.0f);

            foreach (var p in points)
                GL.Vertex(p.X, p.Y, p.Z);

            GL.End();

            GL.Flush();
        }



        #region Mouse moving

        private void ChangePosition(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left ||
                valuesX.Count == 0 || valuesY.Count == 0 || valuesZ.Count == 0)
                return;

            PointF finalPoint = e.Location;

            if (ModifierKeys == Keys.Shift)
            {
                angle.X += (finalPoint.X - initPoint.X) * angleDelta;
                angle.Y += (finalPoint.Y - initPoint.Y) * angleDelta;
            }
            else
            {
                delta.X += (finalPoint.X - initPoint.X) / 100;
                delta.Y -= (finalPoint.Y - initPoint.Y) / 100;
            }

            initPoint = finalPoint;
        }


        private void ChangeDistance(object sender, MouseEventArgs e)
        {
            distance -= e.Delta / 120;
        }

        #endregion



        #region Button Action

        private void SaveImage(object sender, EventArgs e)
        {
            
        }

        #endregion
    }


    public partial class Point3F
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Point3F(float x, float y, float z)
        {
            this.X = x; this.Y = y; this.Z = z;
        }
    }
}
