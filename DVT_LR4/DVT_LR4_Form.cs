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
        #region Initialization

        private readonly FileStream stream;

        private readonly List<float> valuesX, valuesY, valuesZ;

        private readonly List<Point3F> pointsReg, pointsAvg;

        private readonly OpenGL GL;

        private PointF initPoint, angle, delta;

        private float[] avgX, avgY, avgZ;

        private readonly float angleDelta;
        private float distance, minX, maxX,
                      minZ, maxZ, minY, maxY,
                      xDelta, xOffset, yDelta, yOffset,
                      zDelta, zOffset;

        private bool firstMessage;

        private Int16 startMs;

        private readonly int[] xFreq, yFreq, zFreq;

        private Bitmap xyHistBmp, xzHistBmp, yzHistBmp,
                       xyScatBmp, xzScatBmp, yzScatBmp;


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
            delta = new PointF(-7.73f, -1.42f);

            pointsReg = new List<Point3F>();
            pointsAvg = new List<Point3F>();

            angleDelta = 0.2f;
            distance = 32;

            firstMessage = true;

            xFreq = new int[10];
            yFreq = new int[10];
            zFreq = new int[10];

            ReadData();

            CalculateAvgs(valuesX, "X");
            CalculateAvgs(valuesZ, "Z");

            RecalculateY(valuesX.ToArray(), valuesZ.ToArray(), 0);
            RecalculateY(avgX, avgZ, 1);

            for (int i = 0; i < avgX.Length; i++)
                pointsAvg.Add(new Point3F(avgX[i] / 10.0f, avgY[i] / 10.0f, avgZ[i] / 10.0f));

            CalculateHistograms(null, null);
        }


        private void DVT_LR4_Form_Load(object sender, EventArgs e)
        {
            this.Click += (ss, ee) => this.ActiveControl = null;

            this.graphMain.MouseDown += (ss, ee) => initPoint = ee.Location;
            this.graphMain.MouseWheel += ChangeDistance;
            this.graphMain.MouseMove += ChangePosition;

            this.graphMain.OpenGLDraw += Draw;

            this.btnSave.Click += SaveImage;

            this.switchDisplay.CheckedChanged += CalculateHistograms;



            if (this.hideXYHist.Checked)
                this.pictureBox1.Visible = false;
            if (this.hideXZHist.Checked)
                this.pictureBox2.Visible = false;
            if (this.hideYZHist.Checked)
                this.pictureBox3.Visible = false;
        }

        #endregion



        #region Data Reading

        private void ReadData()
        {
            do
            {
                byte[] message = new byte[19];

                if (stream.Position == 57000)
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
                if (pointsReg.Any(p => float.Equals(p.X, valuesX[i] / 10.0f) &&
                                    p.Y == valuesY[i] / 10.0f &&
                                    p.Z == valuesZ[i] / 10.0f))
                {
                    valuesX.RemoveAt(i);
                    valuesY.RemoveAt(i);
                    valuesZ.RemoveAt(i);
                    i = 0;
                    continue;
                }

                pointsReg.Add(new Point3F(valuesX[i] / 10.0f, valuesY[i] / 10.0f, valuesZ[i] / 10.0f));
            }

            avgX = new float[valuesX.Count];
            avgY = new float[valuesY.Count];
            avgZ = new float[valuesZ.Count];
        }

        #endregion



        #region Calculations

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


        private void RecalculateY(float[] arrayX, float[] arrayZ, int key)
        {
            minX = arrayX.Min();
            maxX = arrayX.Max();
            minZ = arrayZ.Min();
            maxZ = arrayZ.Max();

            xDelta = (maxX - minX) / 10;
            xOffset = 0 - minX / xDelta;
            zDelta = (maxZ - minZ) / 10;
            zOffset = 0 - minZ / zDelta;

            var ys = new int[10, 10];

            for (int k = 0; k < arrayX.Length; k++)
            {
                int xIndex = (int)Math.Round(arrayX[k] / xDelta + xOffset);
                int zIndex = (int)Math.Round(arrayZ[k] / zDelta + zOffset);

                for (int i = 0; i < 10; i++)
                    if (xIndex == i)
                        for (int j = 0; j < 10; j++)
                            if (zIndex == j)
                                ys[xIndex, zIndex]++;
            }

            for (int k = 0; k < arrayX.Length; k++)
            {
                int xIndex = (int)Math.Round(arrayX[k] / xDelta + xOffset);
                int zIndex = (int)Math.Round(arrayZ[k] / zDelta + zOffset);

                for (int i = 0; i < 10; i++)
                    if (xIndex == i)
                        for (int j = 0; j < 10; j++)
                            if (zIndex == j)
                                if (key == 0)
                                    valuesY[k] = ys[xIndex, zIndex];
                                else
                                    avgY[k] = ys[xIndex, zIndex];
            }

            if (key == 0)
                for (int i = 0; i < pointsReg.Count; i++)
                    pointsReg[i].Y = valuesY[i] / 10.0f;
            else
                for (int i = 0; i < pointsAvg.Count; i++)
                    pointsAvg[i].Y = avgY[i] / 10.0f;
        }


        private void CalculateHistograms(object sender, EventArgs e)
        {
            float[][] arrays;

            if (this.switchDisplay.Checked)
                arrays = new float[][] { avgX, avgY, avgZ };
            else
                arrays = new float[][] { valuesX.ToArray(), valuesY.ToArray(), valuesZ.ToArray() };

            minX = arrays[0].Min();
            maxX = arrays[0].Max();
            minY = arrays[1].Min();
            maxY = arrays[1].Max();
            minZ = arrays[2].Min();
            maxZ = arrays[2].Max();

            xDelta = (maxX - minX) / 10;
            xOffset = 0 - minX / xDelta;
            yDelta = (maxY - minY) / 10;
            yOffset = 0 - minY / xDelta;
            zDelta = (maxZ - minZ) / 10;
            zOffset = 0 - minZ / zDelta;

            for (int k = 0; k < arrays[0].Length; k++)
            {
                int xIndex = (int)(arrays[0][k] / xDelta + xOffset);
                int yIndex = (int)(arrays[1][k] / yDelta + yOffset);
                int zIndex = (int)(arrays[2][k] / zDelta + zOffset);

                for (int i = 0; i < 10; i++)
                {
                    if (xIndex == i)
                        xFreq[i]++;
                    if (yIndex == i)
                        yFreq[i]++;
                    if (zIndex == i)
                        zFreq[i]++;
                }
            }
        }


        private void FormBitmaps()
        {
            xyScatBmp = new Bitmap(xyHistBmp);
            xzScatBmp = new Bitmap(xzHistBmp);
            yzScatBmp = new Bitmap(yzHistBmp);

            using (Graphics g = Graphics.FromImage(xyHistBmp))
            {
                var delta = xyHistBmp.Width / xFreq.Length;

                for (int i = 0; i < xFreq.Length; i++)
                {
                    g.DrawRectangle(new Pen(Color.Red),
                                    i * delta, xyHistBmp.Height - Map(xFreq[i], 0, xFreq.Max(), 0, xyHistBmp.Height),
                                    delta, Map(xFreq[i], 0, xFreq.Max(), 0, xyHistBmp.Height) - 1);
                }
            }

            using (Graphics g = Graphics.FromImage(xzHistBmp))
            {
                var delta = xzHistBmp.Width / zFreq.Length;

                for (int i = 0; i < zFreq.Length; i++)
                {
                    g.DrawRectangle(new Pen(Color.Red),
                        i * delta, 0,
                        delta, Map(zFreq[zFreq.Length - 1 - i], 0, zFreq.Max(), 0, xzHistBmp.Height) - 1);
                }
            }

            using (Graphics g = Graphics.FromImage(yzHistBmp))
            {
                var delta = yzHistBmp.Height / yFreq.Length;

                for (int i = 0; i < yFreq.Length; i++)
                {
                    g.DrawRectangle(new Pen(Color.Red),
                        0, i * delta,
                        Map(yFreq[zFreq.Length - 1 - i], 0, yFreq.Max(), 0, yzHistBmp.Width) - 1, delta);
                }
            }

            this.pictureBox1.Image = xyHistBmp;
            this.pictureBox2.Image = xzHistBmp;
            this.pictureBox3.Image = yzHistBmp;
        }

        #endregion



        #region Drawing

        private void Draw(object sender, RenderEventArgs args)
        {
            if (valuesX.Count == 0 || valuesY.Count == 0 || valuesZ.Count == 0)
                return;

            Point3F[] points;
            if (this.switchDisplay.Checked)
                points = (Point3F[])pointsAvg.ToArray().Clone();
            else
                points = (Point3F[])pointsReg.ToArray().Clone();

            GL.Enable(OpenGL.GL_BLEND);
            GL.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            GL.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            GL.LoadIdentity();

            GL.Translate(delta.X, delta.Y, -distance);
            GL.Rotate(angle.Y, angle.X, 0.0f);

            var offset = 0.4f;

            var pminX = points.Min(p => p.X);
            var pmaxX = points.Max(p => p.X);
            var pminY = points.Min(p => p.Y);
            var pmaxY = points.Max(p => p.Y);
            var pminZ = points.Min(p => p.Z);
            var pmaxZ = points.Max(p => p.Z);

            var width = pmaxX - pminX;
            var height = pmaxY - pminY;
            var depth = pmaxZ - pminZ;

            var a = new[] { width, height, depth }.Max();

            var centreX = (pmaxX + pminX) / 2;
            var centreY = (pmaxY + pminY) / 2;
            var centreZ = (pmaxZ + pminZ) / 2;

            pminX = centreX - a / 2 - offset;
            pmaxX = centreX + a / 2 + offset;
            pminY = centreY - a / 2 - offset;
            pmaxY = centreY + a / 2 + offset;
            pminZ = centreZ - a / 2 - offset;
            pmaxZ = centreZ + a / 2 + offset;

            if (!hideCube.Checked)
            {
                GL.PointSize(2.0f);
                GL.Begin(OpenGL.GL_POINTS);
                GL.Color(1.0f, 1.0f, 1.0f);

                foreach (var p in points)
                    GL.Vertex(p.X, p.Y, p.Z);

                GL.End();

                GL.Begin(OpenGL.GL_LINES);

                GL.Vertex(pminX, pminY, pminZ);
                GL.Vertex(pmaxX, pminY, pminZ);

                GL.Vertex(pmaxX, pminY, pminZ);
                GL.Vertex(pmaxX, pminY, pmaxZ);

                GL.Vertex(pmaxX, pminY, pmaxZ);
                GL.Vertex(pminX, pminY, pmaxZ);

                GL.Vertex(pminX, pminY, pmaxZ);
                GL.Vertex(pminX, pminY, pminZ);

                GL.Vertex(pminX, pminY, pminZ);
                GL.Vertex(pminX, pmaxY, pminZ);

                GL.Vertex(pminX, pmaxY, pminZ);
                GL.Vertex(pmaxX, pmaxY, pminZ);

                GL.Vertex(pmaxX, pmaxY, pminZ);
                GL.Vertex(pmaxX, pminY, pminZ);

                GL.Vertex(pmaxX, pmaxY, pminZ);
                GL.Vertex(pmaxX, pmaxY, pmaxZ);

                GL.Vertex(pmaxX, pmaxY, pmaxZ);
                GL.Vertex(pmaxX, pminY, pmaxZ);

                GL.Vertex(pmaxX, pmaxY, pmaxZ);
                GL.Vertex(pminX, pmaxY, pmaxZ);

                GL.Vertex(pminX, pminY, pmaxZ);
                GL.Vertex(pminX, pmaxY, pmaxZ);

                GL.Vertex(pminX, pmaxY, pmaxZ);
                GL.Vertex(pminX, pmaxY, pminZ);

                GL.End();
            }

            xyHistBmp = new Bitmap(Map(width, pminX, pmaxX, 0, 500),
                               Map(height, pminY, pmaxY, 0, 500));
            xzHistBmp = new Bitmap(Map(width, pminX, pmaxX, 0, 500),
                               Map(depth, pminZ, pmaxZ, 0, 500));
            yzHistBmp = new Bitmap(Map(height, pminY, pmaxY, 0, 500),
                               Map(depth, pminZ, pmaxZ, 0, 500));

            FormBitmaps();

            if (!hideXYHist.Checked)
                throw new NotImplementedException();

            if (!hideXYScat.Checked)
                throw new NotImplementedException();

            if (!hideXZHist.Checked)
                throw new NotImplementedException();

            if (!hideXZScat.Checked)
                throw new NotImplementedException();

            if (!hideYZHist.Checked)
                throw new NotImplementedException();

            if (!hideYZScat.Checked)
                throw new NotImplementedException();

            GL.Flush();
        }

        #endregion



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



        #region Saving an image

        private void SaveImage(object sender, EventArgs e)
        {

        }

        #endregion



        #region Secondary methods

        private int Map(float value, float inMin, float inMax, int outMin, int outMax)
        {
            return (int)((value - inMin) * (outMax - outMin) / (inMax - inMin)) + outMin;
        }


        private int Map(int value, int inMin, int inMax, int outMin, int outMax)
        {
            return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }


        private void PopBitmapIn(float x, float y, float z)
        {
            uint[] _textures = new uint[6];

            GL.GenTextures(1, _textures);
            GL.BindTexture(OpenGL.GL_TEXTURE_2D, _textures[0]);
            GL.Begin(OpenGL.GL_QUADS);

            GL.TexCoord(0.0f, 0.0f); GL.Vertex(quad[0]);
            GL.TexCoord(1.0f, 0.0f); GL.Vertex(quad[1]);
            GL.TexCoord(1.0f, 1.0f); GL.Vertex(quad[2]);
            GL.TexCoord(0.0f, 1.0f); GL.Vertex(quad[3]);

            GL.End();
        }

        #endregion
    }



    #region Additional class Point3F

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
    #endregion
}
