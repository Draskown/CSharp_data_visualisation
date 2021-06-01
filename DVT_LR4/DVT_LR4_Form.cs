using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
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

        private Point3F[] operatingPoints;
        private PointF initPoint, angle, delta;

        private float[][] calculatedArrays;

        private float[] avgX, avgY, avgZ, recalculatedY;

        private readonly float angleDelta, bitmapsOffset,
                               cubeOffset, axisSize, axisArrowSize;
        private float distance, minX, maxX,
                      minZ, maxZ, minY, maxY,
                      xDelta, xOffset, yDelta, yOffset,
                      zDelta, zOffset, pminX, pminY, pminZ,
                      pmaxX, pmaxY, pmaxZ, width, height,
                      depth, centreX, centreY, centreZ, a;

        private bool firstMessage;

        private Int16 startMs;

        private readonly int[] xFreq, yFreq, zFreq;

        private readonly int scatPointSize;
        private int bitmapDelta;

        private Bitmap xyHistBmp, xzHistBmp, yzHistBmp,
                       xyScatBmp, xzScatBmp, yzScatBmp;

        private readonly uint[] textures = new uint[6];


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
            delta = new PointF(-7.39f, -5.79f);

            pointsReg = new List<Point3F>();
            pointsAvg = new List<Point3F>();

            angleDelta = 0.2f;
            cubeOffset = 0.4f;
            bitmapsOffset = 10.0f;
            axisSize = 3.0f;
            axisArrowSize = 0.4f;
            distance = 55;
            scatPointSize = 4;

            firstMessage = true;

            xFreq = new int[10];
            yFreq = new int[10];
            zFreq = new int[10];

            bitmapDelta = 0;

            ReadData();

            CalculateAvgs(valuesX, "X");
            CalculateAvgs(valuesZ, "Z");

            RecalculateY(valuesX.ToArray(), valuesZ.ToArray(), 0);
            RecalculateY(avgX, avgZ, 1);

            for (int i = 0; i < avgX.Length; i++)
                pointsAvg.Add(new Point3F(avgX[i] / 10.0f, avgY[i] / 10.0f, avgZ[i] / 10.0f));

            CalculateHistograms(null, null);

            InitializeGL();
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
        }


        private void InitializeGL()
        {
            GL.Enable(OpenGL.GL_BLEND);
            GL.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            GL.GenTextures(6, textures);
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

            if (key == 0)
                recalculatedY = new float[valuesY.Count];

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
                                    recalculatedY[k] = ys[xIndex, zIndex];
                                else
                                    avgY[k] = ys[xIndex, zIndex];
            }

            if (key == 0)
                for (int i = 0; i < pointsReg.Count; i++)
                    pointsReg[i].Y = recalculatedY[i] / 10.0f;
            else
                for (int i = 0; i < pointsAvg.Count; i++)
                    pointsAvg[i].Y = avgY[i] / 10.0f;
        }


        private void CalculateHistograms(object sender, EventArgs e)
        {
            if (this.switchDisplay.Checked)
                calculatedArrays = new float[][] { avgX, avgY, avgZ };
            else
                calculatedArrays = new float[][] { valuesX.ToArray(), recalculatedY.ToArray(), valuesZ.ToArray() };

            minX = calculatedArrays[0].Min();
            maxX = calculatedArrays[0].Max();
            minY = calculatedArrays[1].Min();
            maxY = calculatedArrays[1].Max();
            minZ = calculatedArrays[2].Min();
            maxZ = calculatedArrays[2].Max();

            xDelta = (maxX - minX) / 10;
            xOffset = 0 - minX / xDelta;
            yDelta = (maxY - minY) / 10;
            yOffset = 0 - minY / xDelta;
            zDelta = (maxZ - minZ) / 10;
            zOffset = 0 - minZ / zDelta;

            for (int k = 0; k < calculatedArrays[0].Length; k++)
            {
                int xIndex = (int)(calculatedArrays[0][k] / xDelta + xOffset);
                int yIndex = (int)(calculatedArrays[1][k] / yDelta + yOffset);
                int zIndex = (int)(calculatedArrays[2][k] / zDelta + zOffset);

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
            if (xyScatBmp == null)
                xyScatBmp = new Bitmap(xyHistBmp);
            if (xzScatBmp == null)
                xzScatBmp = new Bitmap(xzHistBmp);
            if (yzScatBmp == null)
                yzScatBmp = new Bitmap(yzHistBmp);

            using (Graphics g = Graphics.FromImage(xyHistBmp))
            {
                bitmapDelta = xyHistBmp.Width / xFreq.Length;

                for (int i = 0; i < xFreq.Length; i++)
                {
                    g.DrawRectangle(
                        new Pen(Color.Red),
                        i * bitmapDelta,
                        xyHistBmp.Height - Map(xFreq[i], 0, xFreq.Max(), 0, xyHistBmp.Height),
                        bitmapDelta, 
                        Map(xFreq[i], 0, xFreq.Max(), 0, xyHistBmp.Height) - 1);
                }
            }

            using (Graphics g = Graphics.FromImage(xzHistBmp))
            {
                bitmapDelta = xzHistBmp.Width / zFreq.Length;

                for (int i = 0; i < zFreq.Length; i++)
                {
                    g.DrawRectangle(
                        new Pen(Color.Red),
                        i * bitmapDelta,
                        0,
                        bitmapDelta,
                        Map(zFreq[zFreq.Length - 1 - i], 0, zFreq.Max(), 0, xzHistBmp.Height) - 1);
                }
            }

            using (Graphics g = Graphics.FromImage(yzHistBmp))
            {
                bitmapDelta = yzHistBmp.Height / yFreq.Length;

                for (int i = 0; i < yFreq.Length; i++)
                {
                    g.DrawRectangle(
                        new Pen(Color.Red),
                        0,
                        i * bitmapDelta,
                        Map(yFreq[zFreq.Length - 1 - i], 0, yFreq.Max(), 0, yzHistBmp.Width) - 1,
                        bitmapDelta);
                }
            }

            if (this.switchDisplay.Checked)
                calculatedArrays = new float[][] { avgX, avgY, avgZ };
            else
                calculatedArrays = new float[][] { valuesX.ToArray(), valuesY.ToArray(), valuesZ.ToArray() };

            using (Graphics g = Graphics.FromImage(xyScatBmp))
            {
                for (int i = 0; i < calculatedArrays[0].Length; i++)
                {
                    g.FillEllipse(
                        Brushes.Red,
                        Map(calculatedArrays[0][i], minX, maxX, 0, xyScatBmp.Width - scatPointSize),
                        Map(calculatedArrays[1][i], minY, maxY, 0, xyScatBmp.Height - scatPointSize),
                        scatPointSize, scatPointSize);
                }
            }

            using (Graphics g = Graphics.FromImage(xzScatBmp))
            {
                for (int i = 0; i < calculatedArrays[0].Length; i++)
                {
                    g.FillEllipse(
                        Brushes.Red,
                        Map(calculatedArrays[0][i], minX, maxX, 0, xzScatBmp.Width - scatPointSize),
                        Map(calculatedArrays[2][i], minZ, maxZ, 0, xzScatBmp.Height - scatPointSize),
                        scatPointSize, scatPointSize);
                }
            }

            using (Graphics g = Graphics.FromImage(yzScatBmp))
            {
                for (int i = 0; i < calculatedArrays[1].Length; i++)
                {
                    g.FillEllipse(
                        Brushes.Red,
                        Map(calculatedArrays[1][i], minY, maxY, 0, yzScatBmp.Width - scatPointSize),
                        Map(calculatedArrays[2][i], minZ, maxZ, 0, yzScatBmp.Height - scatPointSize),
                        scatPointSize, scatPointSize);
                }
            }

            //this.pictureBox1.Image = xyScatBmp;
            //this.pictureBox2.Image = xzScatBmp;
            //this.pictureBox3.Image = yzScatBmp;
        }

        #endregion



        #region Drawing

        private void Draw(object sender, RenderEventArgs args)
        {
            if (valuesX.Count == 0 || valuesY.Count == 0 || valuesZ.Count == 0)
                return;

            GL.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            GL.LoadIdentity();

            if (this.switchDisplay.Checked)
                operatingPoints = (Point3F[])pointsAvg.ToArray().Clone();
            else
                operatingPoints = (Point3F[])pointsReg.ToArray().Clone();

            GL.Translate(delta.X, delta.Y, -distance);
            GL.Rotate(angle.Y, angle.X, 0.0f);

            pminX = operatingPoints.Min(p => p.X);
            pmaxX = operatingPoints.Max(p => p.X);
            pminY = operatingPoints.Min(p => p.Y);
            pmaxY = operatingPoints.Max(p => p.Y);
            pminZ = operatingPoints.Min(p => p.Z);
            pmaxZ = operatingPoints.Max(p => p.Z);

            width = pmaxX - pminX;
            height = pmaxY - pminY;
            depth = pmaxZ - pminZ;

            a = new[] { width, height, depth }.Max();

            centreX = (pmaxX + pminX) / 2;
            centreY = (pmaxY + pminY) / 2;
            centreZ = (pmaxZ + pminZ) / 2;

            pminX = centreX - a / 2 - cubeOffset;
            pmaxX = centreX + a / 2 + cubeOffset;
            pminY = centreY - a / 2 - cubeOffset;
            pmaxY = centreY + a / 2 + cubeOffset;
            pminZ = centreZ - a / 2 - cubeOffset;
            pmaxZ = centreZ + a / 2 + cubeOffset;

            if (!hideCube.Checked)
            {
                GL.PointSize(2.0f);
                GL.Begin(OpenGL.GL_POINTS);
                GL.Color(1.0f, 1.0f, 1.0f);

                foreach (var p in operatingPoints)
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

                GL.LineWidth(2.0f);
                GL.Begin(OpenGL.GL_LINES);

                GL.Color(1.0f, 0.0f, 0.0f);
                GL.Vertex(centreX, centreY, centreZ);
                GL.Vertex(centreX + axisSize, centreY, centreZ);
                GL.Vertex(centreX + axisSize, centreY, centreZ);
                GL.Vertex(centreX + axisSize - axisArrowSize, centreY, centreZ - axisArrowSize / 2.0f);
                GL.Vertex(centreX + axisSize, centreY, centreZ);
                GL.Vertex(centreX + axisSize - axisArrowSize, centreY, centreZ + axisArrowSize / 2.0f);

                GL.Color(0.0f, 0.0f, 1.0f);
                GL.Vertex(centreX, centreY, centreZ);
                GL.Vertex(centreX, centreY, centreZ + axisSize);
                GL.Vertex(centreX, centreY, centreZ + axisSize);
                GL.Vertex(centreX + axisArrowSize / 2.0f, centreY, centreZ + axisSize - axisArrowSize);
                GL.Vertex(centreX, centreY, centreZ + axisSize);
                GL.Vertex(centreX - axisArrowSize / 2.0f, centreY, centreZ + axisSize - axisArrowSize);

                GL.Color(1.0f, 1.0f, 0.0f);
                GL.Vertex(centreX, centreY, centreZ);
                GL.Vertex(centreX, centreY + axisSize, centreZ);
                GL.Vertex(centreX, centreY + axisSize, centreZ);
                GL.Vertex(centreX + axisArrowSize / 2.0f, centreY + axisSize - axisArrowSize, centreZ);
                GL.Vertex(centreX, centreY + axisSize, centreZ);
                GL.Vertex(centreX - axisArrowSize / 2.0f, centreY + axisSize - axisArrowSize, centreZ);

                GL.End();

                GL.DrawText(0, 0, 0.0f, 0.0f, 0.0f, "Gilroy", 1, ".");

                var sv = SharpGL.SceneGraph.OpenGLSceneGraphExtensions.Project(GL, new SharpGL.SceneGraph.Vertex(centreX + axisSize, centreY, centreZ));
                GL.DrawText((int)sv.X, (int)sv.Y, 1f, 1f, 1f, "Courier New", 12, labelX.Text);

                sv = SharpGL.SceneGraph.OpenGLSceneGraphExtensions.Project(GL, new SharpGL.SceneGraph.Vertex(centreX, centreY + axisSize, centreZ));
                GL.DrawText((int)sv.X, (int)sv.Y, 1f, 1f, 1f, "Courier New", 12, labelY.Text);

                sv = SharpGL.SceneGraph.OpenGLSceneGraphExtensions.Project(GL, new SharpGL.SceneGraph.Vertex(centreX, centreY, centreZ + axisSize));
                GL.DrawText((int)sv.X, (int)sv.Y, 1f, 1f, 1f, "Courier New", 12, labelZ.Text);
            }

            xyHistBmp = new Bitmap(Map(width, pminX, pmaxX, 0, 512),
                               Map(height, pminY, pmaxY, 0, 512));
            xzHistBmp = new Bitmap(Map(width, pminX, pmaxX, 0, 500),
                               Map(depth, pminZ, pmaxZ, 0, 500));
            yzHistBmp = new Bitmap(Map(height, pminY, pmaxY, 0, 500),
                               Map(depth, pminZ, pmaxZ, 0, 500));

            FormBitmaps();

            //var txt = new uint[1];

            //GL.Enable(OpenGL.GL_TEXTURE_2D);

            //var gbitmapdata = bmp.LockBits(
            //    new Rectangle(0, 0, bmp.Width, bmp.Height),
            //    System.Drawing.Imaging.ImageLockMode.ReadOnly,
            //    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            //GL.GenTextures(1, txt);

            //GL.BindTexture(OpenGL.GL_TEXTURE_2D, txt[0]);

            //GL.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
            //GL.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);

            //GL.TexImage2D(
            //    OpenGL.GL_TEXTURE_2D,
            //    0,
            //    OpenGL.GL_RGBA8,
            //    bmp.Width,
            //    bmp.Height,
            //    0,
            //    OpenGL.GL_RGBA8,
            //    OpenGL.GL_UNSIGNED_BYTE,
            //    gbitmapdata.Scan0);

            //GL.BindTexture(OpenGL.GL_TEXTURE_2D, txt[0]);
            //GL.Enable(OpenGL.GL_TEXTURE_2D);
            //GL.Begin(SharpGL.Enumerations.BeginMode.Quads);

            ////GL.Color(1.0f, 1.0f, 1.0f, 0.4f);
            //GL.TexCoord(0.0f, 0.0f); GL.Vertex(pminX, pminY, pmaxZ + bitmapsOffset);
            //GL.TexCoord(1.0f, 0.0f); GL.Vertex(pmaxX, pminY, pmaxZ + bitmapsOffset);
            //GL.TexCoord(1.0f, 1.0f); GL.Vertex(pmaxX, pmaxY, pmaxZ + bitmapsOffset);
            //GL.TexCoord(0.0f, 1.0f); GL.Vertex(pminX, pmaxY, pmaxZ + bitmapsOffset);

            //GL.Disable(OpenGL.GL_TEXTURE_2D);

            //GL.End();

            //bmp.UnlockBits(gbitmapdata);

            GL.Color(1.0f, 1.0f, 1.0f, 0.4f);

            if (!this.hideXYHist.Checked)
            {
                PopBitmapIn(xyHistBmp, 0);

                GL.BindTexture(OpenGL.GL_TEXTURE_2D, textures[0]);
                GL.Enable(OpenGL.GL_TEXTURE_2D);
                GL.Begin(OpenGL.GL_QUADS);

                GL.TexCoord(0.0f, 0.0f); GL.Vertex(pminX, pminY, pmaxZ + bitmapsOffset);
                GL.TexCoord(1.0f, 0.0f); GL.Vertex(pmaxX, pminY, pmaxZ + bitmapsOffset);
                GL.TexCoord(1.0f, 1.0f); GL.Vertex(pmaxX, pmaxY, pmaxZ + bitmapsOffset);
                GL.TexCoord(0.0f, 1.0f); GL.Vertex(pminX, pmaxY, pmaxZ + bitmapsOffset);

                GL.End();
                GL.Disable(OpenGL.GL_TEXTURE_2D);
            }

            if (!this.hideXYScat.Checked)
            {
                PopBitmapIn(xyScatBmp, 1);

                GL.BindTexture(OpenGL.GL_TEXTURE_2D, textures[0]);
                GL.Enable(OpenGL.GL_TEXTURE_2D);
                GL.Begin(OpenGL.GL_QUADS);

                GL.TexCoord(0.0f, 0.0f); GL.Vertex(pminX, pminY, pminZ - bitmapsOffset);
                GL.TexCoord(1.0f, 0.0f); GL.Vertex(pmaxX, pminY, pminZ - bitmapsOffset);
                GL.TexCoord(1.0f, 1.0f); GL.Vertex(pmaxX, pmaxY, pminZ - bitmapsOffset);
                GL.TexCoord(0.0f, 1.0f); GL.Vertex(pminX, pmaxY, pminZ - bitmapsOffset);

                GL.End();
                GL.Disable(OpenGL.GL_TEXTURE_2D);
            }

            if (!hideXZHist.Checked)
            {
                PopBitmapIn(xzHistBmp, 2);

                GL.BindTexture(OpenGL.GL_TEXTURE_2D, textures[0]);
                GL.Enable(OpenGL.GL_TEXTURE_2D);
                GL.Begin(OpenGL.GL_QUADS);

                GL.TexCoord(0.0f, 0.0f); GL.Vertex(pminX - bitmapsOffset, pminY, pminZ);
                GL.TexCoord(1.0f, 0.0f); GL.Vertex(pminX - bitmapsOffset, pminY, pmaxZ);
                GL.TexCoord(1.0f, 1.0f); GL.Vertex(pminX - bitmapsOffset, pmaxY, pmaxZ);
                GL.TexCoord(0.0f, 1.0f); GL.Vertex(pminX - bitmapsOffset, pmaxY, pminZ);

                GL.End();
                GL.Disable(OpenGL.GL_TEXTURE_2D);
            }

            if (!hideXZScat.Checked)
            {
                PopBitmapIn(xzScatBmp, 3);

                GL.BindTexture(OpenGL.GL_TEXTURE_2D, textures[0]);
                GL.Enable(OpenGL.GL_TEXTURE_2D);
                GL.Begin(OpenGL.GL_QUADS);

                GL.TexCoord(0.0f, 0.0f); GL.Vertex(pmaxX + bitmapsOffset, pminY, pminZ);
                GL.TexCoord(1.0f, 0.0f); GL.Vertex(pmaxX + bitmapsOffset, pminY, pmaxZ);
                GL.TexCoord(1.0f, 1.0f); GL.Vertex(pmaxX + bitmapsOffset, pmaxY, pmaxZ);
                GL.TexCoord(0.0f, 1.0f); GL.Vertex(pmaxX + bitmapsOffset, pmaxY, pminZ);

                GL.End();
                GL.Disable(OpenGL.GL_TEXTURE_2D);
            }

            if (!hideYZHist.Checked)
            {
                PopBitmapIn(yzHistBmp, 4);

                GL.BindTexture(OpenGL.GL_TEXTURE_2D, textures[0]);
                GL.Enable(OpenGL.GL_TEXTURE_2D);
                GL.Begin(OpenGL.GL_QUADS);

                GL.TexCoord(0.0f, 0.0f); GL.Vertex(pminX, pminY - bitmapsOffset, pmaxZ);
                GL.TexCoord(1.0f, 0.0f); GL.Vertex(pmaxX, pminY - bitmapsOffset, pmaxZ);
                GL.TexCoord(1.0f, 1.0f); GL.Vertex(pmaxX, pminY - bitmapsOffset, pminZ);
                GL.TexCoord(0.0f, 1.0f); GL.Vertex(pminX, pminY - bitmapsOffset, pminZ);

                GL.End();
                GL.Disable(OpenGL.GL_TEXTURE_2D);
            }

            if (!hideYZScat.Checked)
            {
                PopBitmapIn(yzScatBmp, 5);

                GL.BindTexture(OpenGL.GL_TEXTURE_2D, textures[0]);
                GL.Enable(OpenGL.GL_TEXTURE_2D);
                GL.Begin(OpenGL.GL_QUADS);

                GL.TexCoord(0.0f, 0.0f); GL.Vertex(pminX, pmaxY + bitmapsOffset, pmaxZ);
                GL.TexCoord(1.0f, 0.0f); GL.Vertex(pmaxX, pmaxY + bitmapsOffset, pmaxZ);
                GL.TexCoord(1.0f, 1.0f); GL.Vertex(pmaxX, pmaxY + bitmapsOffset, pminZ);
                GL.TexCoord(0.0f, 1.0f); GL.Vertex(pminX, pmaxY + bitmapsOffset, pminZ);

                GL.End();
                GL.Disable(OpenGL.GL_TEXTURE_2D);
            }

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
            var bmp = new Bitmap(
                this.graphMain.Width,
                this.graphMain.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.ReadPixels(0, 0, bmp.Width, bmp.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, bmpData.Scan0);

            bmp.UnlockBits(bmpData);
            bmp.RotateFlip(RotateFlipType.Rotate180FlipX);
            bmp.Save("Image.bmp");
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


        private void PopBitmapIn(Bitmap bmp, int texIndex)
        {
            var gbitmapdata = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.Enable(OpenGL.GL_TEXTURE_2D);

            GL.BindTexture(OpenGL.GL_TEXTURE_2D, textures[texIndex]);
            GL.TexImage2D(
                OpenGL.GL_TEXTURE_2D,
                0,
                OpenGL.GL_RGBA8,
                bmp.Width, 
                bmp.Height, 
                0, 
                OpenGL.GL_RGBA8, 
                OpenGL.GL_UNSIGNED_BYTE, 
                gbitmapdata.Scan0);

            GL.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE);
            GL.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE);
            GL.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
            GL.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);

            bmp.UnlockBits(gbitmapdata);
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
